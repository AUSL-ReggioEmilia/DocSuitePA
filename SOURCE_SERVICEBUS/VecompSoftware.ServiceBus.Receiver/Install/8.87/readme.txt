######################################################################################################
Per ottimizzare i processi di migrazioni e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - EnterpriseLibrary.Logging.config
 - Receiver.appSettings.config
 - Receiver.connectionStrings.config
 - Receiver.system.servicemodel.behaviors.config
 - Receiver.system.servicemodel.bindings.config
 - Receiver.system.servicemodel.client.config
 - WebApi.Client.Config.Addresses.json
 - SignatureTemplate.xml

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#FL
######################################################################################################

Per far funzionare il modulo UDS è necessario installare i seguenti prodotti:

Microsoft® System CLR Types for Microsoft® SQL Server® 2012
http://go.microsoft.com/fwlink/?LinkID=239644&clcid=0x409

Microsoft® SQL Server® 2012 Shared Management Objects 
http://go.microsoft.com/fwlink/?LinkID=239659&clcid=0x409


Microsoft Build Tools 2015
http://www.microsoft.com/en-in/download/details.aspx?id=48159

#FL
###############################