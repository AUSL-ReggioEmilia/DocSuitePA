using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Configuration;
using NHibernate;
using VecompSoftware.NHibernateManager.Config;
using VecompSoftware.NHibernateManager.SessionFactory;
using Configuration = NHibernate.Cfg.Configuration;

namespace VecompSoftware.NHibernateManager
{
    public class NHibernateSessionManager : IDisposable
    {
        #region [ Fields ]

        private Hashtable sessionFactories = new Hashtable();
        private Hashtable sessionConfigurations = new Hashtable();
        private const string TRANSACTION_KEY = "CONTEXT_TRANSACTIONS";
        private const string SESSION_KEY = "CONTEXT_SESSIONS";

        private readonly Guid _instanceId;
        private bool _disposed;

        #endregion

        #region [ Properties ]

        public static NHibernateSessionManager Instance
        {
            get
            {
                return Nested.NHibernateSessionManager;
            }
        }

        public ConnectionStringSettingsCollection SessionConnectionStrings { get; private set; }

        #endregion

        #region [ Constructors ]

        private NHibernateSessionManager()
        {
            _instanceId = Guid.NewGuid();
            SessionConnectionStrings = HttpContext.Current != null
                ? WebConfigurationManager.ConnectionStrings
                : ConfigurationManager.ConnectionStrings;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                CloseTransactionAndSessions();
            }
            _disposed = true;
        }
        #endregion

        #region [ Methods ]

        private ISessionFactory GetSessionFactoryFor(string sessionFactoryName)
        {
            if (string.IsNullOrEmpty(sessionFactoryName))
                throw new ArgumentNullException("sessionFactoryName", "sessionFactoryName may not be null nor empty");

            ISessionFactory sessionFactory = (ISessionFactory)sessionFactories[sessionFactoryName];

            if (sessionFactory != null)
                return sessionFactory;
            
            //Uso esclusivo delle risorse - Concorrenza
            lock (sessionFactories)
            {
                sessionFactory = (ISessionFactory)sessionFactories[sessionFactoryName];
                if (sessionFactory == null)
                {
                    OpenSessionInViewSection openSessionInViewSection = (OpenSessionInViewSection)ConfigurationManager.GetSection("nhibernateSettings");

                    if (openSessionInViewSection == null)
                    {
                        throw new Exception("Impossibile trovare la sezione nhibernateSettings nel ConfigurationManager.");
                    }

                    SessionFactoryElement factoryElement = openSessionInViewSection.SessionFactories[sessionFactoryName];

                    if (factoryElement == null)
                    {
                        throw new Exception("The SessionFactory with name " + sessionFactoryName +
                                            " Element was not found with ConfigurationManager.");
                    }

                    INHibernateConfiguration configurator;

                    try
                    {
                        configurator = (INHibernateConfiguration)Activator.CreateInstance(Type.GetType(factoryElement.Configurator));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Errore in creazione Configurator. Verificare riferimento a VecompSoftware.DocSuiteWeb.Data.dll", ex);
                    }


                    Configuration configuration = configurator.GetConfigurationFor(factoryElement);
#if DEBUG
                    configuration.SetProperty("generate_statistics", "true");
#endif
                    sessionFactory = configuration.BuildSessionFactory();

                    if (sessionFactory == null)
                        throw new InvalidOperationException("configuration.BuildSessionFactory() returned null.");

                    sessionFactories.Add(sessionFactoryName, sessionFactory);
                    sessionConfigurations.Add(sessionFactoryName, configuration);
                }
            }
            return sessionFactory;
        }

        public void InvalidateSessionFrom(string sessionFactoryName)
        {
            ContextSessions[sessionFactoryName] = null;
        }

        public ISession GetSessionFrom(string sessionFactoryName, bool applyDefaultFilters = true)
        {
            ISession session = (ISession)ContextSessions[sessionFactoryName];

            if (session == null)
            {
                OpenSessionInViewSection openSessionInViewSection = (OpenSessionInViewSection)ConfigurationManager.GetSection("nhibernateSettings");
                if (!string.IsNullOrEmpty(openSessionInViewSection.SessionFactories.Interceptor) && openSessionInViewSection.SessionFactories.DefaultSessionFactory == sessionFactoryName)
                {
                    var interceptor = (IInterceptor)Activator.CreateInstance(Type.GetType(openSessionInViewSection.SessionFactories.Interceptor));
                    session = GetSessionFactoryFor(sessionFactoryName).OpenSession(interceptor);
                    //session = GetSessionFactoryFor(sessionFactoryName).OpenSession(new SqlStatementInterceptor());
                }
                else
                {
                    session = GetSessionFactoryFor(sessionFactoryName).OpenSession();
                }

                ContextSessions[sessionFactoryName] = session;
            }

            if (NHibernateSessionUtil.ApplyFilterActions.Count > 0 && applyDefaultFilters)
            {
                foreach (Action<ISession> applyFilterAction in NHibernateSessionUtil.ApplyFilterActions)
                {
                    applyFilterAction(session);
                }
            }

            if (session == null)
            {
                throw new Exception("session was null");
            }
            return session;
        }
        public ISession OpenSession(string factoryName, bool applyDefaultFilters = true)
        {
            ISession session = GetSessionFactoryFor(factoryName).OpenSession();
            if (NHibernateSessionUtil.ApplyFilterActions.Count > 0 && applyDefaultFilters)
            {
                foreach (Action<ISession> applyFilterAction in NHibernateSessionUtil.ApplyFilterActions)
                {
                    applyFilterAction(session);
                }
            }
            return session;
        }
        public IStatelessSession OpenStatelessSession(string factoryName)
        {
            return GetSessionFactoryFor(factoryName).OpenStatelessSession();
        }

        public Configuration GetConfigurationFrom(string sessionFactoryName)
        {
            if (!sessionConfigurations.ContainsKey(sessionFactoryName))
            {
                GetSessionFrom(sessionFactoryName);
            }

            return (Configuration)sessionConfigurations[sessionFactoryName];
        }


        /// <summary>
        /// Used to refresh session factories in case of reload
        /// Using this method nhibernate configurator will be reinvoked
        /// </summary>
        public void CloseAllSession() { }


        public void CloseSessionOn(string sessionFactoryName)
        {
            ISession session = (ISession)ContextSessions[sessionFactoryName];

            try
            {
                if (session != null && session.IsOpen)
                {
                    session.Flush();
                }
            }
            catch
            { }
            finally
            {
                if (session != null && session.IsOpen)
                {
                    session.Clear();
                    session.Close();
                    session.Dispose();
                }
                ContextSessions[sessionFactoryName] = null;
                ContextSessions.Remove(sessionFactoryName);
            }
        }

        public ITransaction BeginTransactionOn(string sessionFactoryName)
        {
            ITransaction transaction = (ITransaction)ContextTransactions[sessionFactoryName];

            if (transaction == null)
            {
                transaction = GetSessionFrom(sessionFactoryName).BeginTransaction(IsolationLevel.ReadCommitted);
                ContextTransactions.Add(sessionFactoryName, transaction);
            }

            return transaction;
        }

        /// <summary>
        /// Chiude ed esegue il dispose di Transizioni e Sessioni
        /// </summary>
        /// <remarks>Esegue il rollback delle eventuali transazioni aperte</remarks>
        public void CloseTransactionAndSessions()
        {
            if (ContextTransactions != null && ContextTransactions.Count > 0)
            {
                foreach (ITransaction trans in ContextTransactions.Values)
                {
                    if (trans.IsActive)
                    {
                        trans.Rollback();
                    }
                    ((IDisposable)trans).Dispose();
                }
                ContextTransactions.Clear();
            }

            if (ContextSessions != null && ContextSessions.Count > 0)
            {
                foreach (ISession session in ContextSessions.Values)
                {
                    if (session.IsOpen)
                    {
                        session.Close();
                    }
                    //((IDisposable)session).Dispose();
                }
                ContextSessions.Clear();
            }
        }

        public void ClearFactories()
        {
            foreach (ISessionFactory s in sessionFactories.Values)
            {
                s.Close();
                ((IDisposable)s).Dispose();
            }
            sessionFactories.Clear();
            sessionConfigurations.Clear();
        }

        public void CommitTransactionOn(string sessionFactoryName)
        {
            ITransaction transaction = (ITransaction)ContextTransactions[sessionFactoryName];
            try
            {
                if (HasOpenTransactionOn(sessionFactoryName))
                {
                    transaction.Commit();
                    ContextTransactions.Remove(sessionFactoryName);
                }
            }
            catch (HibernateException)
            {
                RollbackTransactionOn(sessionFactoryName);
                throw;
            }
        }

        public bool HasOpenTransactionOn(string sessionFactoryName)
        {
            ITransaction transaction = (ITransaction)ContextTransactions[sessionFactoryName];

            return transaction != null && !transaction.WasCommitted && !transaction.WasRolledBack;
        }

        public void RollbackTransactionOn(string sessionFactoryName)
        {
            ITransaction transaction = (ITransaction)ContextTransactions[sessionFactoryName];

            try
            {
                if (HasOpenTransactionOn(sessionFactoryName))
                {
                    var session = (ISession)ContextSessions[sessionFactoryName];
                    session.Flush();
                    transaction.Rollback();
                }

                ContextTransactions.Remove(sessionFactoryName);
            }
            finally
            {
                //CloseSessionOn(sessionFactoryName);
            }
        }

        private Hashtable ContextTransactions
        {
            get
            {
                if (IsInWebContext())
                {
                    if (HttpContext.Current.Items[TRANSACTION_KEY] == null)
                    {
                        HttpContext.Current.Items[TRANSACTION_KEY] = new Hashtable();
                    }

                    return (Hashtable)HttpContext.Current.Items[TRANSACTION_KEY];
                }
                else
                {
                    if (CallContext.GetData(TRANSACTION_KEY) == null)
                    {
                        CallContext.SetData(TRANSACTION_KEY, new Hashtable());
                    }

                    return (Hashtable)CallContext.GetData(TRANSACTION_KEY);
                }
            }
        }

        private Hashtable ContextSessions
        {
            get
            {
                if (IsInWebContext())
                {
                    if (HttpContext.Current.Items[SESSION_KEY] == null)
                    {
                        HttpContext.Current.Items[SESSION_KEY] = new Hashtable();
                    }

                    return (Hashtable)HttpContext.Current.Items[SESSION_KEY];
                }
                else
                {
                    if (CallContext.GetData(SESSION_KEY) == null)
                    {
                        CallContext.SetData(SESSION_KEY, new Hashtable());
                    }

                    return (Hashtable)CallContext.GetData(SESSION_KEY);
                }
            }
        }

        private Boolean IsInWebContext()
        {
            return HttpContext.Current != null;
        }

        #endregion

        private static class Nested
        {
            static internal readonly NHibernateSessionManager NHibernateSessionManager = new NHibernateSessionManager();
        }
    }
}
