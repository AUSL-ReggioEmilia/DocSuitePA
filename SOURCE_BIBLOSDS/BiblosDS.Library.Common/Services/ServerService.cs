using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.Enums;
using BiblosDS.Library.Common.Enums;
using System.Reflection;
using BiblosDS.Library.Common.Objects.UtilityService;
using System.Configuration;
using BiblosDS.Library.Common.Exceptions;

namespace BiblosDS.Library.Common.Services
{
    public class ServerService : ServiceBase
    {
        public const string WCF_Document_HostName = "ServiceDocument";
        public const string WCF_DocumentStorage_HostName = "ServiceDocumentStorage";

        static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ServerService));
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static BindingList<Server> GetServers()
        {
            logger.Debug("GetServers");
            return DbProvider.GetServers();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public static Server GetServer(Guid serverId)
        {
            logger.DebugFormat("GetServer - ID:{0}", serverId);
            try
            {
                return DbProvider.GetServer(serverId);
            }
            catch (Exception e)
            {
                logger.Error(e);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General, "ServerService." + MethodBase.GetCurrentMethod().Name, e.ToString(), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Errors);
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="createNew"></param>
        /// <returns></returns>
        public static Server UpdateServer(Server server, bool createNew)
        {
            logger.DebugFormat("UpdateServer - ID:{0} IS_NEW:{1}", (server == null) ? "N/A" : server.IdServer.ToString(), (createNew) ? "YES" : "NO");
            try
            {
                return (createNew) ? DbProvider.AddServer(server) : DbProvider.UpdateServer(server);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General, "ServerService." + MethodBase.GetCurrentMethod().Name, ex.ToString(), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Errors);
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public static bool DeleteServer(Server server)
        {
            logger.DebugFormat("DeleteServer - ID:{0}", (server == null) ? "N/A" : server.IdServer.ToString());
            try
            {
                if (server == null || server.IdServer == Guid.Empty)
                    return false;

                return DbProvider.DeleteServer(server.IdServer);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General, "ServerService." + MethodBase.GetCurrentMethod().Name, ex.ToString(), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Errors);
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ArchiveServerConfig AddArchiveServerConfig(ArchiveServerConfig config)
        {
            logger.Debug("AddArchiveServerConfig");
            try
            {
                return DbProvider.AddArchiveServerConfig(config);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General, "ServerService." + MethodBase.GetCurrentMethod().Name, ex.ToString(), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Errors);
                throw;
            }
        }

        public static void DeleteArchiveServerConfig(ArchiveServerConfig config)
        {
            logger.Debug("DeleteArchiveServerConfig");
            try
            {
                DbProvider.DeleteArchiveServerConfig(config);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General, "ServerService." + MethodBase.GetCurrentMethod().Name, ex.ToString(), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Errors);
                throw;
            }
        }

        public static Server GetCurrentServer()
        {
            logger.Debug("GetCurrentServer");
            try
            {
                return DbProvider.GetServer(MachineService.GetServerName());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General, "ServerService." + MethodBase.GetCurrentMethod().Name, ex.ToString(), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Errors);
                throw;
            }
        }

        public static Server GetMasterServer()
        {
            logger.Debug("GetMasterServer");
            try
            {
                var servers = DbProvider.GetServers();
                return servers.FirstOrDefault(x => x.ServerRole == ServerRole.Master);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Logging.WriteLogEvent(LoggingSource.BiblosDS_General, "ServerService." + MethodBase.GetCurrentMethod().Name, ex.ToString(), LoggingOperationType.BiblosDS_General, LoggingLevel.BiblosDS_Errors);
                throw;
            }
        }

        public static ArchiveServerConfig GetArchiveServerConfig(Guid idServer, Guid idArchive)
        {
            return DbProvider.GetArchiveServerConfig(idServer, idArchive);
        }

        public static string GetPathTransito(DocumentArchive archive, Guid idServer)
        {
            string pathTransito = string.Empty;
            var serverConfig = DbProvider.GetArchiveServerConfig(idServer, archive.IdArchive);
            if (serverConfig != null)
            {
                pathTransito = serverConfig.TransitPath;
            }
            else
            {
                pathTransito = archive.PathTransito;
            }

            return pathTransito;
        }

        public static List<DocumentServer> GetDocumentInServer(Guid idDocument)
        {
            return DbProvider.GetDocumentInServer(idDocument);
        }

        public static bool CheckDocumentInServer(Server currentServer, List<DocumentServer> documentInServer)
        {
            if (documentInServer == null || documentInServer.Count() <= 0)
                return true;
            return documentInServer.Any(x => x.Server.IdServer == currentServer.IdServer);
        }

        public static DocumentServer GetDocumentInServer(Server currentServer, Guid idDocument)
        {
            if (currentServer == null)
                throw new Exceptions.Generic_Exception("Parameter server is required for Methos: GetDocumentInServer");
            return DbProvider.GetDocumentInServer(currentServer.IdServer, idDocument);
        }

        /// <summary>
        /// Risolve il nome del server per il suo ruolo.
        /// </summary>
        /// <returns></returns>
        public static string GetServerName()
        {
            if (GetCurrentServer().ServerRole != ServerRole.Proxy)
                return string.Empty;

            var master = ServerService.GetMasterServer();
            if (master == null)
                throw new ServerNotDefined_Exception("Undefined master server. Invalid configuration for proxy server: {0}.",
                    MachineService.GetServerName());

            logger.DebugFormat("Server is a proxy, Redirect to: {0}", master.ServerName);
            return master.ServerName;

        }
    }
}
