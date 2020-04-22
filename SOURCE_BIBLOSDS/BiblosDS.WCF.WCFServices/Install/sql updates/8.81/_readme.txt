Deve essere lanciato lo script SQL sul DB BiblosDS2010

 - db_migrations.sql
 - db_postmigrations.sql

#AC
###################################################################
Successivamente all'esecuzione degli script di migrazione è necessario lanciare il tool di bonifica BiblosPreservationMigrator per calcolare il percorso
dei file presenti in conservazione.
La configurazione del tool prevede la sola modifica della connectionstring.
Una volta eseguito, il tool automaticamente processerà tutte le conservazioni chiuse.
Per eventuali dubbi contattare il team di Sviluppo.

#AC
###################################################################
Per ottimizzare i processi di migrazione e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - Web.config

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzione.

#AC
#####################################################################################