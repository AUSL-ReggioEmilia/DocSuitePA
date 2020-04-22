Deve essere lanciato lo script SQL sul DB BiblosDS2010

 - db_migrations.sql

#AC
###################################################################
Per ottimizzare i processi di migrazione e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - Web.config

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzione.

#AC
#####################################################################################