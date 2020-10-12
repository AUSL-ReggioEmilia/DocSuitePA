ATTENZIONE LA BUILD RICHIEDE IL FRAMEWORK .NET 4.8. 
PRIMA DI PROCEDERE ALL'INSTALLAZIONE E' NECESSARIO CANCELLARE IL CONTENUTO DELLA CARTELLA BIN

#FL
######################################################################################################
Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- JeepService 9.03
	- WebAPI 9.03
	- FastProtocolImporter 9.03
	- ServiceBus Listeners 9.03
	- Workflow Integrations 9.03
		- PER TUTTI I CLIENTI E' NECESSARIO ESSERE CERTI CHE I SEGUENTI MODULI SIANO INSTALLATI E CONFIGURATI
			-> VSW.DocumentUnitLink
			-> VSW.FascicleClose
			-> VSW.FascicleDocumentUnit
			-> VSW.PeriodicFascicle
			-> VSW.WorkflowAutoNotify   <- NUOVO MODULO 9.03 NECESSIO PER I WORKFLOW DOCSUITE
	- Compilazione libreria dinamica UDS alla 9.03 (soli per i clienti che hanno già adottato il modulo UDS)
	- Utilizzare il tool di migrazione dei delle UDS UDSMigrations 9.03
	- Utilizzare il tool di migrazione dei Template di Collaborazione DSWTemplateCollaborationMigrator.

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

Attenzione è stato introdotto il nuovo parametro "SignatureTemplate" che preve da dismissione del file fisico "SignatureTemplate.xml"
da tutti i vari progetti docsuite. 
Dalla versione 9.03 l'unica impostazione che governa lo stile della segnature di documento viene letto dal parametro SignatureTemplate di 
parameterenv di protocollo.

Rimuovere da appsettings il parametro e relativo "DocSuiteWeb.StampaConforme.Template" e relativo file fisico.

NB: ATTENZIONE E' NECESSARIO RIPORTARE L'ESATTA IMPOSTAZIONE DI OGNI CLIENTE, DIVERSAMENTE VERRA' PRESO IL DEFAULT. 
	PURTROPPO PER UN BUG DELLA TELERICK NON USARE L'INTERFACCIA GRAFICA PER IMPOSTARLO MA ANDARE SEMPRE DA DB.

<Label>
  <Text>(SIGNATURE) Pagina (pagina) di (pagine)</Text>
  <Font Face="Arial" Size="10" Style="Bold" />
</Label>

#FL
######################################################################################################

Attenzione in questa versione il parametro ScannerLightRestEnabled viene attivato di default.
Assicurarsi anticipatamente che tutti i client che utilizzano la docsuite abbiano installato la versione DynamsoftServiceSetup.msi contenuta nel pacchetto di rilascio "dynamsoft V15.zip".

Diversamente spegnere il parametro, ma nel corso dell'anno verrà dismessa la versione dynamsoft 8.

#FL
######################################################################################################

Sono state apportate molte modifiche alle code e alle topic del service bus localizzate nel file SB01.Docsuitenamespace_Entities.xml.

Aggiornare la definizione con la versione “GED Edition” del ServiceBusExplorer.

#FL
######################################################################################################

E' stato creato il tool di migrazione dei metadati di fascicoli e dossier DSWMetadataMigrator. E' necessaio eseguirlo per non compromettere la stabilità del modulo Fascicoli e Dossier.

Per configurare il tool è necessario specificare la corretta connectionstring statndo attenti a non rimuovere il valore MultipleActiveResultSets=true

#FL
######################################################################################################

NOTA PER AGENAS:

Nel database verificare nelle tabelle WorkflowEvaluationProperties e WorkflowProperties se esistono delle proprietà 
di nome "_dsw_a_CollaborationSignSummaryTemplateId" aggiornale cambiando il nome in "_dsw_p_CollaborationSignSummaryTemplateId"

#FL
######################################################################################################

NOTA PER TUTTI I CLIENTI:

Sono state apportate corpose modifiche al modulo UDS. E' obbligatorio configurare il parametro di parameterenv WorkflowLocation.

Inoltre è obbligatorio configurare il servizio di Windows BiblosWCF_HOST con la configurazione EnableCleanDocumentsTimer attiva.
Riporto i valori:

    <add key="EnableCleanDocumentsTimer" value="true" />
    <add key="CleanDocumentsTimerWaitMinute" value="60" />
    <add key="CleanDocumentsFromDate" value="<data di inizio>" />
    <add key="CleanDocumentsToDate" value="<data di fine anche nel futuro 2030>" />

	oltre al file ArchiveRestrictions.json con l'ID archivio di Biblos corrispondente alla WorkflowLocation.

Per i dettagli più specifici, leggere il relativo readme BiblosDS.WCF.WCFServices\Install\sql updates\8.73.

#FL
######################################################################################################

Sono state apportate modifiche al motore di Workflow che impattano sui clienti che hanno attivato uno dei seguenti workflow :
 - Archiviazione (***REMOVED***)
 - Protocollazione (***REMOVED***)
 - Invio PEC (***REMOVED***)

E' dunque necessario aggiornare i WorkflowRepositories col seguente json sotto riportati.

Vedere dunque:
	- Workflow - ***REMOVED*** - Protocolla contratto

Per i flussi di collaborazioni già avviati e non conclusi è necessario bonificare a mano la struttura (o chiedere al cliente di chiudere anticipatamente tutti i worklflow non conclusi)

#FL
######################################################################################################

DALLA VERSIONE 9.03 NON é PIU' NECESSARIO INSTALLARE IL PRODOTTO "Microsoft Build Tools 2015".
SUI SERVER DI PRODUZIONE E' NECESSARIO DISINSTALLARLO IN MODO DA NON COMPROMETTERE LA STABILITA' DI PRODOTTO.

E' necessario installare la nuova componente Build Tools per Visual Studio 2019
https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=BuildTools&rel=16

Inoltre è necessario essere certi che sia installato il .NET Framework Developer Packs 4.8
https://aka.ms/msbuild/developerpacks

#FL
######################################################################################################

INFORMAZIONE PER ***REMOVED***: E' stato rimosso il pulsante IP4D in sommario di protocollo in favore dell'uso dei moduli di workflow.
E' necessaio configurare correttamete il workflow 'Workflow - ***REMOVED*** - Invia IP4D.json'

#FL
######################################################################################################