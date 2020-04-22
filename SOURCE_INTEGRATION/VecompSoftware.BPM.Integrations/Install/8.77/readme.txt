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

Per semplificare il processo di deploy e configurazione sono stati rimossi i singoli file 
	- modules.configuration.json
	- period.configuration.json
in un unico file global.configuration.json che unisce in una unica stuttura le configurazioni.

E' necessario dunque riconfigurare il servizio nel nuovo file di configurazione. 
Vedere esempios standard global.configuration.json

#FL
######################################################################################################

Per semplificare il processo di deploy e configurazione sono stati rimossi i singoli file di configurazione modules.configuration.json dalle 
cartelle dei vari moduli.

E' necessario dunque ricordarsi in fase di installazione di verificare se nella cartella default_configurations è presente il corrispettivo
file module.configuration.json.

#FL
######################################################################################################