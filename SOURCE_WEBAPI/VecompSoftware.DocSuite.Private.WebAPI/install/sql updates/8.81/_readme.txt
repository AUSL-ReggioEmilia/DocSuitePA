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
#####################################################################################
E' stato modificato il file EnterpriseLibrary.Validation.config delle validazioni. Assicurarsi di copiarlo in produzione con l'ultima versione contenuta nella cartella default_configurations.

#FL
#####################################################################################

E' stata rimossa la gestione del SecurityContextType come chiave di AppSettings.config in favore di una migliore gestione nel TenantModel.

Rimuovere la chiave di "VecompSoftware.DocSuiteWeb.Security.ContextType" AppSettings.config impostanto il corrispondente valore nella proprietà "SecurityContext" del tenantModel.

  <!-- VecompSoftware.DocSuiteWeb.Service.Workflow.EngineType può assumere il valore Machine, Domain, ApplicationDirectory, OAuth2  -->
  <add key="VecompSoftware.DocSuiteWeb.Security.ContextType" value="Domain" />

Vedere il readme della DocSuite 8.81 per le relative impostazioni del tenantmodel.
#FL
#####################################################################################