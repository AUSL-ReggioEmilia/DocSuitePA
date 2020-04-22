#########################################
Per ottimizzare i processi di migrazioni e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - EnterpriseLibrary.Logging.config
 - Receiver.appSettings.config
 - Receiver.connectionStrings.config
 - Receiver.system.servicemodel.behaviors.config
 - Receiver.system.servicemodel.bindings.config
 - Receiver.system.servicemodel.client.config
 - Receiver.system.servicemodel.client.config
 - WebApi.Client.Config.Addresses.json

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#FL
###############################################################

E' stato modificato il file WebApi.Client.Config.Endpoints.json. Assicurarsi di copiarlo in produzione.

#FL
#####################################################################################

E' stato modificato il file EnterpriseLibrary.Logging.config. Assicurarsi di copiarlo in produzione.

#FL
#####################################################################################
