Per ottimizzazione e evitare al minimo i problemi di migrazione è stato rivisto il file WebApi.Client.Config.json, separandolo in due files
 - WebApi.Client.Config.Addresses.json
 - WebApi.Client.Config.Endpoints.json

Quindi si deve cancellare il vecchio file WebApi.Client.Config.json riportando i valori corretti nel file WebApi.Client.Config.Addresses.json 
con i vari indirizzi [Address": "http://localhost/DSW.WebAPI/api/wfm/"].

Nei rilasci successivi a questo il file WebApi.Client.Config.Endpoints.json deve essere sempre sovvrasritto con quello riportato nella cartella di rilascio, mentre il file 
WebApi.Client.Config.Addresses.json va mantenuto quello originale di produzione. Nei rilasci successivi il file WebApi.Client.Config.Addresses.json verrà omesso e riportato
solo nella cartella di readme.

#FL
###############################

Per migliorare il processo di rilascio sono stati separati i file di configurazione variabile dal VecompSoftware.ServiceBus.Receiver.exe.config
Rivedere quindi tutti i file attuali prima di migrare i vari listeners installati in produzione separando correttamente i seguenti files:
	- Receiver.appSettings.config
	- Receiver.connectionStrings.config
	- Receiver.system.servicemodel.behaviors.config
	- Receiver.system.servicemodel.bindings.config
	- Receiver.system.servicemodel.bindings.config
	- Receiver.system.servicemodel.client.config

#FL
###############################