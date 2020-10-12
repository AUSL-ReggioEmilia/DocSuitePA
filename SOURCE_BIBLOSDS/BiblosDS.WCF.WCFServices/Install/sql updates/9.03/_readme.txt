Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- BiblosDS WCFHost (Windows Service) 9.03
    - Admin Central 9.03
    - Preservation Portal 9.03

#AC
###################################################################
Deve essere lanciato lo script SQL sul DB BiblosDS2010

 - 01. db_migrations.sql

 E' necessario eseguire il tool di bonifica BiblosPreservationMigrator in modalità 2 (Migrazione configurazione archivi), prima dell'esecuzione del successivo script.
 In caso di anomalie nell'esecuzione del tool contattare il reparto Sviluppo.

 - 02. db_postmigrations.sql

#AC
###################################################################
Per ottimizzare i processi di migrazione e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - biblosds.connectionstrings.config
 - biblosds.system.servicemodel.behaviours.config
 - biblosds.system.servicemodel.bindings.confg
 - wcfservices.appsettings.config
 - wcfservices.system.servicemodel.client.config
 - wcfservices.system.servicemodel.services.config
 - Web.config

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzione.

#AC
#####################################################################################
Nella versione [9.03] sono state aggiornate numerose librerie, si consiglia quindi di cancellare il contenuto della directory "bin"
(o tutte le dll se si sta aggiornado il BiblosDS WCFHost) prima dell'aggiornamento.

#AC
#####################################################################################
E' stata introdotta una nuova funzionalità per avere archivi remoti tra server di Biblos diversi.
Per la configurazione di tali archivi si rimanda al documento presente in questa directory di Install.

#AC
#####################################################################################