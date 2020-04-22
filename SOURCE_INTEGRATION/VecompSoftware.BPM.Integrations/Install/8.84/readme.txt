######################################################################################################
Per ottimizzare i processi di migrazioni e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - EnterpriseLibrary.Logging.config
 - global.configuration.json
 - Integrations.connectionStrings.config
 - Integrations.system.servicemodel.behaviors.config
 - Integrations.system.servicemodel.bindings.config
 - Integrations.system.servicemodel.client.config
 - WebApi.Client.Config.Addresses.json

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#FL
######################################################################################################
