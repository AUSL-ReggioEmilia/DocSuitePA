Per ottimizzare i processi di migrazioni e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - EnterpriseLibrary.Logging.config
 - EnterpriseLibrary.Validation.config
 - WebApi.appSettings.config
 - WebApi.connectionStrings.config
 - WebApi.system.servicemodel.behaviors.config
 - WebApi.system.servicemodel.bindings.config
 - WebApi.system.servicemodel.client.config

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#FL
#####################################################################################
E' stato modificato il file EnterpriseLibrary.Validation.config delle validazioni. Assicurarsi di copiarlo in produzione con l'ultima versione contenuta nella cartella default_configurations.

#FL
#####################################################################################
Durante l'esecuzione dello script ProtocolloDB_migrations durante la creazione della store procedure [dbo].[DossierFolder_SP_PropagateAuthorizationToDescendants]
potrebbe esserci un errore sulle collation delle colonne Title e Object nella tabella Fascicles.
E' necessario allineare la collation delle due colonne.

Di seguito un esempio dello script per correggere l'errore:

ALTER TABLE dbo.Fascicles ALTER COLUMN Object
            nvarchar(256)COLLATE Latin1_General_CI_AS NOT NULL;
GO

#SZ
#####################################################################################
