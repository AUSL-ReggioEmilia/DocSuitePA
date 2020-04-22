using System;
using System.Configuration;
using System.Web;
using VecompSoftware.NHibernateManager.Config;

namespace VecompSoftware.NHibernateManager
{
    /// <summary>
    /// Implements the Open-Session-In-View pattern
    /// using <see cref="NHibernateSessionManager" />.
    /// Inspiration for this class came from Ed Courtenay at 
    /// http://sourceforge.net/forum/message.php?msg_id=2847509.
    /// </summary>
    public class NHibernateSessionModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(BeginTransaction);
            context.EndRequest += new EventHandler(CommitAndCloseSession);
        }

        public void BeginTransaction(object sender, EventArgs e)
        {
        }

        public void CommitAndCloseSession(object sender, EventArgs e)
        {
            OpenSessionInViewSection openSessionInViewSection = GetOpenSessionInViewSection();

            try
            {
                foreach (SessionFactoryElement sessionFactorySettings in openSessionInViewSection.SessionFactories)
                {
                    if (sessionFactorySettings.IsTransactional)
                    {
                        NHibernateSessionManager.Instance.CommitTransactionOn(sessionFactorySettings.Name);
                    }
                }
            }
            finally
            {
                foreach (SessionFactoryElement sessionFactorySettings in openSessionInViewSection.SessionFactories)
                {
                    NHibernateSessionManager.Instance.CloseSessionOn(sessionFactorySettings.Name);
                }
            }
        }

        private OpenSessionInViewSection GetOpenSessionInViewSection()
        {
            var openSessionInViewSection = (OpenSessionInViewSection)ConfigurationManager.GetSection("nhibernateSettings");

            if (openSessionInViewSection == null)
            {
                throw new Exception("Impossibile trovare la sezione nhibernateSettings nel ConfigurationManager.");
            }
            return openSessionInViewSection;
        }

        public void Dispose()
        {

        }
    }
}

