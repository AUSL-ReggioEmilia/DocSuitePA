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

Rimuovere la chiave di "VecompSoftware.DocSuiteWeb.TenantId" AppSettings.config in quanto il valore viene letto direttamente dal tenantModel.

#FL
#####################################################################################

Rimuovere la chiave di "VecompSoftware.DocSuiteWeb.TenantName" AppSettings.config in quanto il valore viene letto direttamente dal tenantModel.

#FL
#####################################################################################