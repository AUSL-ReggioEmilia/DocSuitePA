ATTENZIONE LA BUILD RICHIEDE IL FRAMEWORK .NET 4.8. 
PRIMA DI PROCEDERE ALL'INSTALLAZIONE E' NECESSARIO CANCELLARE IL CONTENUTO DELLA CARTELLA BIN

#FL
######################################################################################################Per ottimizzare i processi di migrazioni e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
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


Il motore Microsoft Workflow Manager è stato dismesso dal maintenance di Microsoft e duqnue dispmesso dai prodotti DocSuite.
E' stata rimossa la chaive di appsettings rendendo non fruibile l'opzione WorkflowManager:
  <!-- VecompSoftware.DocSuiteWeb.Service.Workflow.EngineType può assumere il valore JSON o WorkflowManager -->
  <add key="VecompSoftware.DocSuiteWeb.Service.Workflow.EngineType" value="JSON" />

#FL
#####################################################################################