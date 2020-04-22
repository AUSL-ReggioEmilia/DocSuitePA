
In 8.53, nella cartella update è presente lo script ProtocolloDB_update.sql che riguarda le modifiche apportate alle UDS 
con il rilascio 8.53 di fine Agosto 2016. 
Questo script va lanciato per i seguenti clienti: CTT, CAP, FALCK.

#SDC
###############################################################

Per ottimizzare i processi di migrazioni e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - EnterpriseLibrary.Logging.config
 - EnterpriseLibrary.Validation.config
 - WebApi.appSettings.config
 - WebApi.connectionStrings.config

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#FL
###############################################################


Per i clienti che hanno le UDS attive è necessario ricompilare il codice delle UDS di produzione, aggiungendo i seguenti cambiamenti al progetto dinamico
Per dettagli chiedere supporto a Sviluppo.
 - AGGIUNGERE LA COLONNA alter table uds.<UDSNAME> add _cancelMotivation nvarchar(1000) null

 - CREAREA LA TABELLA alter table uds.<UDSNAME>_Logs 

#SDC
###############################################################