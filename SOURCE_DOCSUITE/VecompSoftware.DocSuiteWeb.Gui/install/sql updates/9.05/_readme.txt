ATTENZIONE LA BUILD RICHIEDE IL FRAMEWORK .NET 4.8. 
PRIMA DI PROCEDERE ALL'INSTALLAZIONE E' NECESSARIO CANCELLARE IL CONTENUTO DELLA CARTELLA BIN

#FL
######################################################################################################
Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 9.05
	- ServiceBus Listeners 9.05
	- Workflow Integrations 9.05
		- PER TUTTI I CLIENTI E' NECESSARIO ESSERE CERTI CHE I SEGUENTI MODULI SIANO INSTALLATI E CONFIGURATI
			-> VSW.DocumentUnitLink
			-> VSW.FascicleClose
			-> VSW.FascicleDocumentUnit
			-> VSW.PeriodicFascicle
			-> VSW.WorkflowAutoNotify   
			-> VSW.WorkflowLogging			<- NUOVO MODULO 9.05 NECESSIO PER I WORKFLOW DOCSUITE
	- Compilazione libreria dinamica UDS alla 9.05 (soli per i clienti che hanno già adottato il modulo UDS)
	- Utilizzare il tool di migrazione dei delle UDS UDSMigrations 9.05
	- Fast Protocol Sender 9.05
	- SOLO PER ENPACL:
		- ImportEnpacl 9.05

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

Sono state apportate molte modifiche alle code e alle topic del service bus localizzate nel file SB01.Docsuitenamespace_Entities.xml.

Aggiornare la definizione con la versione “GED Edition” del ServiceBusExplorer.

#FL
######################################################################################################

SOLO PER ENPACL 
Nel progetto ImportEnpacl aggiugnere il nuovo parametro in appsetting:

	- <add key="IdTenantAOO" value="<valore del TenantAOO>" />

#FL
######################################################################################################

Per tutti i clienti che hanno il modulo PayableInvoice attivo, aggiornare il json di configurazione impostando il seguente parametro
"WorkflowPayableInvoicePECMailBuildComplete": "WorkflowPayableInvoicePECMailBuildComplete",
#MF
######################################################################################################
PER I CLIENTI CHE UTILIZZANO WSProt e/o FastProtocolSender/FastProtocolImport

Assicurarsi di aggiungere nell'AppSettings del WSProt e delle API legacy le seguenti chiavi di configurazione:
	- <add key="TenantAOOId" value="<valore del TenantAOO>" />
    - <add key="DocSuite.Default.ODATA.Finder.TopQuery" value="100" />

#AC
######################################################################################################
