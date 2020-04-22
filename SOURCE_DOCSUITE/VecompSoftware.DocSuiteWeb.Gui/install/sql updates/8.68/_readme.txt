Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.68
	- ServiceBus Listeners 8.68
	- Workflow Integrations 8.68
	- JeepService 8.68
	- WSColl 8.68
	- DocumentUnitMigrator 8.68
	
#FL
######################################################################################################

La procedura di migrazione del database prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:
	
	1. script DocSuite
		01. ProtocolloDB_migrations.sql
		02. PraticheDB_migrations.sql
		03. AttiDB_migrations.sql

	2. script WebAPI
	
	3. script DocSuite - solo quando si installa in produzione
		04. AttiDB_production.sql
		05. ProtocolloDB_production.sql
	
	4 script DocSuite - LogMigrations
		06. AttiLog_migrations.sql
		07. ProtocolloLog_migrations.sql
	
	ATTENZIONE: per gli script Docsuite nella cartella LogMigrations ( 06. AttiLog_migrations.sql, 07. ProtocolloLog_migrations.sql) è necessario prendere accordi con sviluppo per decidere quando lanciarli

#FL
######################################################################################################

Lo script di migrazione prevede una serie di drop table di entità obsolete (Funzionalità ODG, Contratti)

Le Tabelle sono:

PROTOCOLLO  -> [Contract]
		    -> [ContractTable]

ATTI		-> [ResolutionODGTasks] 
			-> [ResolutionODGTaskDetails]
			-> [ResolutionODGDetails]
			-> [ResolutionODG]

#FL
######################################################################################################
Per un corretto funzionamento della funzionalità 'Attestazione di Conformità' attivata tramite il parametro 'DematerialisationEnabled' 
è necessario installare i seguenti prodotti:
	- il listener VecompSoftware.ServiceBus.Module.Workflow.Listener.Dematerialisation del servizio ServiceBus Listeners
	- il modulo VecompSoftware.DocSuite.BPM.Integrations.Modules.GenericProcesses.Dematerialisation del servizio Workflow Integration

Vedere documento PDF allegato per installazione completa di tutti i servizi.

#FL
######################################################################################################

Sono state apportate molte modifiche alle code e alle topic del service bus.
E' necessario procecede come segue:
	- aprire il Tool Service Bus Explorer
	- specificare la ConnectionString del service bus del cliente
	- Eliminare la topic di nome "entity_event", situata nel nodo principale "Topics"
	- dal menu Actions selezionare la voce Import Entries, nella finestra importare il file SB01.Docsuitenamespace_Entity_Event_Topic.xml
	- dal menu Actions selezionare la voce Import Entries, nella finestra importare il file SB02.Docsuitenamespace_Workflow_Integration_Topic.xml
	- dal menu Actions selezionare la voce Import Entries, nella finestra importare il file SB03.Docsuitenamespace_Dematerialisation_Request_Queue.xml

Se da errori in fase di import, contattare sviluppo. Il prodotto senza tali configurazioni potrebbe creare anomalie critiche. 
#FL
######################################################################################################

E' stata aggiunta una etichetta standard "Attestazione di conformità"  al parametro "DocumentSeriesDocumentsLabel" legata alla funzionalità di Attestazione di conformità. 
Quando viene attivato il parametro DematerialisationEnabled è necessario aggiungere l'etichetta "Attestazione di conformità"  per tutti i clienti che non usano le etichette stadard.
Esempio: 
	 Indicare per la singola tipologia di catena il nome dalla label richiesta dal cliente (es. configurazione per ***REMOVED***)  
	   {
        "MainChain": "Procedure",
        "AnnexedChain": "Linee Guida",
        "UnpublishedAnnexedChain": "Modulistica",
		"DematerialisationChain": "Attestazione di conformità"
        }

#IS
######################################################################################################

L'aggiornamento del servizio JeepService 8.68 deve essere effettuato cancellando tutti i file .dll e .pdb dalla
directory del servizio e dei moduli installati, successivamente è possibile incollare il contenuto della nuova build.
E' necessario verificare che nella directory Config del servizio JeepService 8.68 sia presente il file
di configurazione WebApi.Client.Config.Endpoints.json.
Le chiavi di appSettings del servizio JeepService 8.68 presenti in App.Config "DocSuiteWeb.WebAPI.EnterpriseBus.Uri" e 
"WebApiConfig" sono cancellabili dal file di configurazione.

#AC
######################################################################################################

Per l'integrazione ZEN in AUSL-RE è necessario aggiornare il modulo PEC del servizio JeepService 8.68
abilitando il parametro "SendReceiptEventsEnabled".

#FL
######################################################################################################

Per il solo cliente AUSL-RE nel servizio WSColl è necessario inserire nell'appSettings 
la chiave "DocSuite.WSColl.ProtocolTemplateCollaborationName" specificando il nome del template di collaborazione "GASP"

#FL
######################################################################################################

Sono stati aggiunti i seguenti Endpoint da aggiungere al TenantModel nella sezione Entities:

         		"UDSTypology": {
					"IsActive": true,
					"Timeout": "00:00:30",
					"ODATAControllerName": "UDSTypologies"
				}

Vedi gli esempi di configurazione in ExampleMultiTenant_8.68.json e ExampleOneTenant_8.68.json

#SDC
######################################################################################################

Per il solo cliente ***REMOVED*** nel modulo VecompSoftware.DocSuite.BPM.Integrations.Modules.***REMOVED***.ENavigare del servizio Workflow Integrations
sono stati aggiunti 3 nuovi parametri di configurazione nel file module.configuration.json
	- TenantName
	- TenantId 
	- DocumentSeriesItemSubscriptionRetired

#FL
######################################################################################################
