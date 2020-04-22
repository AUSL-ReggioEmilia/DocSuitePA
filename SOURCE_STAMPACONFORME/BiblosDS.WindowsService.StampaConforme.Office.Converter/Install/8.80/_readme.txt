Per un corretto aggiornamento pulire il contenuto della directory del servizio prima dell'installazione.
Successivamente copiare il nuovo contenuto del pacchetto di installazione.

#AC
################################################
Per ottimizzare i processi di migrazioni e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 
 - officeconverter.appsettings.config
 - officeconverter.system.servicemodel.behaviours.config
 - officeconverter.system.servicemodel.bindings.config
 - officeconverter.system.servicemodel.client.config
 - officeconverter.system.servicemodel.services.config
 - EnterpriseLibrary.Logging.config.

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#AC
################################################
Per migliorare le performance di comunicazione tra il WS StampaConforme e i servizi converter Windows di Office
e OpenOffice si consiglia di configurare la comunicazione tramite NetTcpBinding.
I file di configurazione presenti nella directory default_configurations sono già impostati per tale configurazione
e non richiedono ulteriori attività.

#AC
################################################