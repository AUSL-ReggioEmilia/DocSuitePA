Per ottimizzare i processi di migrazioni e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - EnterpriseLibrary.Logging.config
 - EnterpriseLibrary.Validation.config
 - WebApi.appSettings.config
 - WebApi.connectionStrings.config
 - WebApi.system.servicemodel.behaviors.config
 - WebApi.system.servicemodel.bindings.config
 - WebApi.system.servicemodel.client.config

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#FL
###############################################################

Le WebAPI 8.56 hanno un requisito minumo di funzionamento è del framework .NET da 4.6.1.

#FL
###############################################################


Per i clienti che hanno le UDS attive è necessario ricompilare il codice delle UDS di produzione.
Per dettagli chiedere supporto a Sviluppo.

#SDC
###############################################################

Sono stati aggiunti due nuovi parametri nel file WebApi.AppSettings.config che deve riportare i seguenti valori:

<add key="VecompSoftware.DocSuiteWeb.CollaborationLocationId" value="<Il valore ID della tabella location corrisponente all'archivio delle collaborazioni>" />
<add key="VecompSoftware.DocSuiteWeb.Service.Workflow.EngineType" value="<può assumere il valore JSON o WorkflowManager>" />
<add key="VecompSoftware.DocSuiteWeb.AllowCrossOrigin" value="<può assumere il valore ALL (tutte le oringini client) o RESTRICTED (necessita della gestione del json in App_Data\ConfigurationFiles\WebAPI.Origin.Allow.Lists.json>" />
#FL
################################