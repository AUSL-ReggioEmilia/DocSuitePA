ATTENZIONE LA BUILD RICHIEDE IL FRAMEWORK .NET 4.8. 
PRIMA DI PROCEDERE ALL'INSTALLAZIONE E' NECESSARIO CANCELLARE IL CONTENUTO DELLA CARTELLA BIN

#FL
######################################################################################################
Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- JeepService 9.07
	- WebAPI 9.07
	- FastProtocolImporter 9.07
	- ServiceBus Listeners 9.07
		- PER TUTTI I CLIENTI E' NECESSARIO ESSERE CERTI CHE I SEGUENTI MODULI SIANO INSTALLATI E CONFIGURATI
			-> DossierInsert
			-> DossierUpdate
			-> FascicleInsert
			-> FascicleUpdate
			-> MessageInsert
			-> CollaborationInsert
	- Workflow Integrations 9.07
		- PER TUTTI I CLIENTI E' NECESSARIO ESSERE CERTI CHE I SEGUENTI MODULI SIANO INSTALLATI E CONFIGURATI
			-> VSW.DocumentUnitLink
			-> VSW.FascicleClose
			-> VSW.FascicleDocumentUnit
			-> VSW.PeriodicFascicle
			-> VSW.WorkflowAutoNotify   
			-> VSW.WorkflowLogging
			-> VSW.FascicleCustomActions	<- NUOVO MODULO 9.06 NECESSIO PER I WORKFLOW DOCSUITE
			-> VSW.WorkflowAggregator	    <- NUOVO MODULO 9.07 NECESSIO PER I WORKFLOW DOCSUITE
	- Compilazione libreria dinamica UDS alla 9.07 (soli per i clienti che hanno già adottato il modulo UDS)
	- Utilizzare il tool di migrazione dei delle UDS UDSMigrations 9.06

#FL
######################################################################################################
La procedura di migrazione del database prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:
	
	1. script DocSuite
		01. ProtocolloDB_migrations.sql
		02. PraticheDB_migrations.sql
		03. AttiDB_migrations.sql
	
	2. script WebAPI
#FL
######################################################################################################

Da questa versione sono state rimossi i seguenti moduli deprecati :
	- Dematerializzazione
	- SecurePaprer
	- SecureDocument

Si rende necessario la rimozione dei seguenti listener (se presenti in produzione):
	- Workflow.Listener.Dematerialization
	- Workflow.Listener.SecureDocument

Si rende necessario la rimozione dei seguenti moduli dell'integrations (se presenti in produzione):
	- VSW.Dematerialization
	- VSW.SecureDocument

E' possibile eliminare il seguenti attibuti (e relativi valori) dagli archivi BiblosDS (procedere se e solo se l'archivio non ha la verifica dell'hash sui metadati).

	- "SecureDocumentId"

#FL
######################################################################################################
E' stato rimosso dal DocSuiteMenuConfig.json la seguente sezione di menu DocSuite.

A tale file è stata aggiunta una nuova voce "Fascicoli da chiudere" nella voce 'Menu3' -> "FirstNode2" -> "FirstNode13": { "Name": "Attestazioni di conformità" }

#FL
######################################################################################################
E' stata aggiunta una nuova chiave di appSettings dei servizi ServiceBus Listeners 9.07. 
E' necessario configurare tale chiave nel file Receiver.appSettings.config, nel seguente modo:

<add key="WorkflowAggregationTopicName" value="workflow_aggregation" />

#AC
######################################################################################################

Sono state apportate molte modifiche alle code e alle topic del service bus localizzate nel file SB01.Docsuitenamespace_Entities.xml.

Aggiornare la definizione con la versione “GED Edition” del ServiceBusExplorer.

#FL
######################################################################################################

ATTENZIONE MODIFICA DA APPORTARE AL MODULO VSW.FascicleClose.
Nel modulo "VSW.FascicleClose" del prodotto "Workflow Integrations" è stata cambiata la definizione del json disponibile nella cartella default configuration\9.07.

Verificare di avere la seguente definizione.
{
  "TopicWorkflowIntegration": "workflow_integration",
  "CategoryFascicleDeleteSubscription": "CategoryFascicleDelete",
  "WorkflowFascicleCloseSubscription": "WorkflowFascicleClose"
}
######################################################################################################
