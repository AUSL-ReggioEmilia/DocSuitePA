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

Per far funzionare il listener VecompSoftware.ServiceBus.Module.Workflow.Listener.Dematerialisation è necessario avere installato in produzione il DocSuite.Services.PDFGenerator

#SZ
######################################################################################

Per generare correttamente il file di attestazione è necessario impostare il file  del 
listener VecompSoftware.ServiceBus.Module.Workflow.Listener.Dematerialisation settando:
- il parametro 'IdTemplateDematerialisation' = l'identificativo univoco della cartella generata dal DocSuite.Services.PDFGenerator, in cui si trova il modulo di attestazione
- il parametro 'DematerialisationWorkflowName' = il nome del workflowRepository dell'Attestazione di dematerializzazione

#SZ
######################################################################################

Sono stati modificati i seguenti file di configurazione
- Receiver.system.servicemodel.bindings.config
- Receiver.system.servicemodel.client.config

Assicurarsi di copiarli in produzione.

#SZ
######################################################################################