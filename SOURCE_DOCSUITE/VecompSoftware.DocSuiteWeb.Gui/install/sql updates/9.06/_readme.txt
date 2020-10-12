ATTENZIONE LA BUILD RICHIEDE IL FRAMEWORK .NET 4.8. 
PRIMA DI PROCEDERE ALL'INSTALLAZIONE E' NECESSARIO CANCELLARE IL CONTENUTO DELLA CARTELLA BIN

#FL
######################################################################################################
Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- JeepService 9.06
	- WebAPI 9.06
	- FastProtocolImporter 9.06
	- ServiceBus Listeners 9.06
		- PER TUTTI I CLIENTI E' NECESSARIO ESSERE CERTI CHE I SEGUENTI MODULI SIANO INSTALLATI E CONFIGURATI
			-> FascicleInsert
			-> FascicleUpdate
			-> MessageInsert
			-> CollaborationInsert
	- Workflow Integrations 9.06
		- PER TUTTI I CLIENTI E' NECESSARIO ESSERE CERTI CHE I SEGUENTI MODULI SIANO INSTALLATI E CONFIGURATI
			-> VSW.DocumentUnitLink
			-> VSW.FascicleClose
			-> VSW.FascicleDocumentUnit
			-> VSW.PeriodicFascicle
			-> VSW.WorkflowAutoNotify   
			-> VSW.WorkflowLogging			<- NUOVO MODULO 9.05 NECESSIO PER I WORKFLOW DOCSUITE
			-> VSW.FascicleCustomActions	<- NUOVO MODULO 9.06 NECESSIO PER I WORKFLOW DOCSUITE
	- Compilazione libreria dinamica UDS alla 9.06 (soli per i clienti che hanno già adottato il modulo UDS)
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

SOLO PER ASLTO:
Per abilitare 2 funzionalità richieste dal cliente (User Story 23412: https://dev.azure.com/docsuiteweb/DocSuite/_queries/edit/23412/?triage=true),
è necessario modificare alcuni valori all'interno della tabella TabWorkflow.
Aggiungere la seguente definizione nella colonna ManagedWorkflowData per lo step di Adozione di Delibere e Determine:
	Frontespizio[.SEL-Digitale]

Sempre nella colonna ManagedWorkflowData dello step Adozione di Delibere e Determine, aggiungere se non presenti i seguenti valori alla proprietà Date:
	.INS.TODAY

#AC
######################################################################################################

SOLO PER ASLTO:
Per abilitare una funzionalità richiesta dal cliente (Feature 23407: https://dev.azure.com/docsuiteweb/DocSuite/_queries/edit/23407/?triage=true),
è necessario modificare alcuni valori all'interno della tabella TabMaster.
Aggiungere nella colonna ManagedData, alla corrispondenza Proposer[... , la stringa ".MULTISELECT.", sia per Delibere che Determine.

#AC
######################################################################################################

SOLO PER ASLTO:
E' stato modificato il file UltimaPagina.rdlc, è necessario aggiornarlo solamente per il cliente ASLTO.

#AC
######################################################################################################

Sono state apportate molte modifiche alle code e alle topic del service bus localizzate nel file SB01.Docsuitenamespace_Entities.xml.

Aggiornare la definizione con la versione “GED Edition” del ServiceBusExplorer.

#FL
######################################################################################################
E' stato modificato il file DocSuiteMenuConfig.json che contiente la configurazione del menù di DocSuite.
A tale file è stata aggiunta una nuova voce "Fascicoli da chiudere" nella voce 'Menu7' -> "FirstNode2" -> "SecondNode5".

#FL
######################################################################################################
######################################################################################################

SOLO PER AUSLRE:

E' necessario aggiornare i WorkflowRepositories:
	- AUSL-RE - AVELCO - Protocolla semplice
	- AUSL-RE - AVELCO - Protocolla con fascicolazione

I json sono presenti nella cartella di installazione.

#AC
######################################################################################################