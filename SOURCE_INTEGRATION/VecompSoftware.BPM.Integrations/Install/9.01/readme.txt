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
Per alcuni moduli del servizio VecompSoftware.BPM.Integrations è necessario aggiungere nel json "module.configuration.json"
una nuova chiave di configurazione relativa all'identificativo del TenantAOOId (prendere il valore del TenantAOO relativo al TenantId
già specificato nel json).

es.
"TenantAOOId": "ff2efa82-4ea3-471e-af22-f784bbf09c25"

Per la corretta configurazione si prega di utilizzare come master i file di configurazione di ogni singolo modulo nella directory Install -> 9.01 -> default_configurations
del servizio VecompSoftware.BPM.Integrations.

#AC
######################################################################################################