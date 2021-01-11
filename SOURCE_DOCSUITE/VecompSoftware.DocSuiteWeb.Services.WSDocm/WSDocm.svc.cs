using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.DocSuiteWeb.Services.WSDocm.Dto;
using VecompSoftware.DocSuiteWeb.Services.WSDocm.ExceptionHandler;
using VecompSoftware.NHibernateManager;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuiteWeb.Services.WSDocm
{
    [ServiceBehavior(Namespace = "http://www.vecompsoftware.it/DocSuiteWS", ConfigurationName = "WSDocm.WSDocmSoap", AddressFilterMode = AddressFilterMode.Any)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [GlobalExceptionHandlerAttribute(typeof(GlobalExceptionHandler))]
    public class WSDocm : IWSDocm
    {
        #region Fields

        private const string LoggerName = "WSDocmLog";
        #endregion

        #region Properties
        public string UserName { get; set; }
        public string Domain { get; set; }
        public string FullName
        {
            get
            {
                return string.Concat(Domain, @"\", UserName);
            }
        }
        #endregion

        #region Service Methods
        public List<Contenitore> GetContenitori(string Utente, string StringaErrore)
        {
            var dtoContainers = new List<Contenitore>();
            try
            {
                InitUsernameAndDomain(Utente);
                if(!CurrentUserRightsEnable())
                    throw new AuthenticationException(string.Format("Utente {0} non trovato nel Dominio {1}", UserName, Domain));

                var containers = FacadeFactory.Instance.ContainerFacade.GetContainers(Domain, UserName,
                    DSWEnvironment.Document, null, true);

                if (containers == null || !containers.Any())
                    throw new FaultException<WSDocmFault>(new WSDocmFault("Containers not found", "Nessun contenitore trovato"));

                dtoContainers.AddRange(containers.Select(container => new Contenitore {Id = container.Id, Name = container.Name}));
                return dtoContainers;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public List<Metadato> GetMetadati(string Utente, string StringaErrore)
        {
            var dtoMetadata = new List<Metadato>();
            try
            {
                InitUsernameAndDomain(Utente);
                if (!CurrentUserRightsEnable())
                    throw new AuthenticationException(string.Format("Utente {0} non trovato nel Dominio {1}", UserName, Domain));

                var metadata = WsDocmUtil.GetDictionaryFromXml(HttpContext.Current.Server.MapPath("~/Mapping/Pratica.xml"), "label", "name");
                dtoMetadata.AddRange(metadata.Select(meta => new Metadato(meta.Key, meta.Value)));

                return dtoMetadata;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public List<Filtro> GetFiltri(string Utente, string StringaErrore)
        {
            var dtoFilters = new List<Filtro>();
            try
            {
                InitUsernameAndDomain(Utente);
                if(!CurrentUserRightsEnable())
                    throw new AuthenticationException(string.Format("Utente {0} non trovato nel Dominio {1}", UserName, Domain));

                var filters = WsDocmUtil.GetDictionaryFromXml(HttpContext.Current.Server.MapPath("~/Mapping/DocumentFinder.xml"), "name", "type");
                dtoFilters.AddRange(filters.Select(filter => new Filtro(filter.Key, filter.Value)));

                return dtoFilters;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public List<StatoPratica> GetStato(string Utente, string StringaErrore)
        {
            var statuses = new List<StatoPratica>();
            try
            {
                InitUsernameAndDomain(Utente);
                if (!CurrentUserRightsEnable())
                    throw new AuthenticationException(string.Format("Utente {0} non trovato nel Dominio {1}", UserName, Domain));

                var all = new StatoPratica(){Id = (int) NHibernateDocumentFinder.SearchStatus.All, Etichetta = "Tutte"};
                var open = new StatoPratica(){Id = (int) NHibernateDocumentFinder.SearchStatus.Open, Etichetta = "Aperte"};
                var closed = new StatoPratica() { Id = (int)NHibernateDocumentFinder.SearchStatus.Closed, Etichetta = "Chiuse" };
                var canceled = new StatoPratica() { Id = (int)NHibernateDocumentFinder.SearchStatus.Canceled, Etichetta = "Annullate" };

                statuses.AddRange(new[] {all, open, closed, canceled});

                return statuses;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public List<Pratica> GetDati(string Utente, Filtro[] Filtri, string[] Metadati, string StringaErrore)
        {
            return GetDati(Utente, Filtri, Metadati, 0, StringaErrore);
        }

        public List<Pratica> GetDati(string Utente, Filtro[] Filtri, string[] Metadati, int MaxResult, string StringaErrore)
        {
            try
            {
                InitUsernameAndDomain(Utente);
                if (!CurrentUserRightsEnable())
                    throw new AuthenticationException(string.Format("Utente {0} non trovato nel Dominio {1}", UserName, Domain));

                var data = WsDocmUtil.GetDataFromFinder(Filtri, Metadati, MaxResult);
                return data;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        public bool CambiaStato(string Utente, short Anno, int Numero, string NuovoStato, DateTime StatoData, string Causale,
            bool AppendiCausale, string StringaErrore)
        {
            try
            {
                InitUsernameAndDomain(Utente);
                if (!CurrentUserRightsEnable())
                    throw new AuthenticationException(string.Format("Utente {0} non trovato nel Dominio {1}", UserName, Domain));

                var document = FacadeFactory.Instance.DocumentFacade.GetById(Anno, Numero);
                if (document == null)
                    throw new FaultException<WSDocmFault>(new WSDocmFault("Document not found", "Nessuna pratica trovata per i parametri passati"));

                FacadeFactory.Instance.DocumentFacade.UpdateStatus(ref document, NuovoStato, StatoData, Causale,
                    AppendiCausale);

                return true;
            }
            finally
            {
                NHibernateSessionManager.Instance.CloseTransactionAndSessions();
            }
        }

        #endregion

        #region Internal Methods

        internal bool CurrentUserRightsEnable()
        {
            try
            {
                IList<SecurityGroups> securityRigths = FacadeFactory.Instance.SecurityUsersFacade.GetGroupsByAccount(FullName);
                return securityRigths != null && securityRigths.Any();
            }
            catch (Exception ex)
            {
                FileLogger.Error("WSDocmLog", ex.Message);
                return false;
            }            
        }

        internal void InitUsernameAndDomain(string user)
        {
            //Domain
            string[] fullNameSplitted = user.Split('\\');
            if (fullNameSplitted.Length <= 1)
            {
                Domain = ServiceSecurityContext.Current.WindowsIdentity.Name.Split('\\')[0];
                UserName = user;
            }
            else
            {
                Domain = fullNameSplitted[0];
                UserName = fullNameSplitted[1];
            }
        }

        #endregion
    }
}
