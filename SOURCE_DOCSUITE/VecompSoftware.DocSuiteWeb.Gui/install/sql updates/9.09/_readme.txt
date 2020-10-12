######################################################################################################
Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 9.09
	- Fast Protocol Sender 9.09
	- ServiceBus Listeners 9.09
		- PER TUTTI I CLIENTI E' NECESSARIO ESSERE CERTI CHE I SEGUENTI MODULI SIANO INSTALLATI E CONFIGURATI
			-> DossierInsert
			-> DossierUpdate
			-> FascicleInsert
			-> FascicleUpdate
			-> MessageInsert
			-> CollaborationInsert
	- Workflow Integrations 9.09
		- PER TUTTI I CLIENTI E' NECESSARIO ESSERE CERTI CHE I SEGUENTI MODULI SIANO INSTALLATI E CONFIGURATI
			-> VSW.DocumentUnitLink
			-> VSW.FascicleClose
			-> VSW.FascicleDocumentUnit
			-> VSW.PeriodicFascicle
			-> VSW.WorkflowAutoNotify   
			-> VSW.WorkflowLogging
			-> VSW.FascicleCustomActions	<- NUOVO MODULO 9.06 NECESSIO PER I WORKFLOW DOCSUITE
			-> VSW.WorkflowAggregator	    <- NUOVO MODULO 9.07 NECESSIO PER I WORKFLOW DOCSUITE
	- Compilazione libreria dinamica UDS alla 9.09 (soli per i clienti che hanno già adottato il modulo UDS)
	- Utilizzare il tool di migrazione dei delle UDS UDSMigrations 9.07

#FL
######################################################################################################
Prima di procedere con l'esecuzione degli script SQL è necessario assicurarsi che nella tabella Category ci sia il nodo di livello 0.

Il nodo di livello 0 è stato introdotto con l'evoluzione della DocSuite - MULTI TENANT di San Marino e prevede la coesistenza di più classificatori
sulle diverse AOO. Il nodo 0 non è mai visibili dall'utente ma è necessario che sia presente nel database. Tale nodo deve essere il primo "PAPA'" di tutti i sotto nodi.

Lascio lo script SQL che può essere usato per creare agevolmente il nodo in questione, con in fondo le query per allineare la struttura della fullincrementalpath dei vecchi record.

ATTENZIONE: GLI UPDATE VANNO ESEGUITI UNA SOLA VOLTA PER DATABASE. SE PER ERRORE SI ESEGUNO PIU' VOLTE IL DATABASE RISULTERA' COMPROMESSO E LA DOCSUITE DIVENTERA' INSTABILE E NON USABILE!!!



DECLARE @RC int
DECLARE @idCategory smallint = null
DECLARE @Name nvarchar(100) = '<NOME DELLA AOO>'
DECLARE @idParent smallint = null
DECLARE @isActive tinyint = 1
DECLARE @Code smallint = 0
DECLARE @FullIncrementalPath nvarchar(256) = null
DECLARE @FullCode nvarchar(256) = ''
DECLARE @RegistrationUser nvarchar(256) = 'SYSTEM'
DECLARE @RegistrationDate datetimeoffset(7) = getutcdate()
DECLARE @LastChangedUser nvarchar(256) = null
DECLARE @LastChangedDate datetimeoffset(7) = null
DECLARE @UniqueId uniqueidentifier = newid()
DECLARE @IdMassimarioScarto uniqueidentifier  = null
DECLARE @IdCategorySchema uniqueidentifier = '<VALORE DEL RECORD SCHEMACATEGORY>'
DECLARE @StartDate datetimeoffset(7) = '<METTERE LA DATA DI STARTDATE DEL RECORD SCHEMACATEGORY>'
DECLARE @EndDate datetimeoffset(7) = null
DECLARE @IdMetadataRepository uniqueidentifier= null
DECLARE @IdTenantAOO uniqueidentifier = '<ID DEL TANANT AOO>'


EXECUTE @RC = [dbo].[Category_Insert] 
   @idCategory
  ,@Name
  ,@idParent
  ,@isActive
  ,@Code
  ,@FullIncrementalPath
  ,@FullCode
  ,@RegistrationUser
  ,@RegistrationDate
  ,@LastChangedUser
  ,@LastChangedDate
  ,@UniqueId
  ,@IdMassimarioScarto
  ,@IdCategorySchema
  ,@StartDate
  ,@EndDate
  ,@IdMetadataRepository
  ,@IdTenantAOO

UPDATE PROTOCOLLO.DBO.Category set idParent= <ID_CATEGORY DEL NODO LIVELLO 0> WHERE idCategory <= <ULTIMO ID ESCLUSO ID_CATEGORY NODO LIVELLO 0> and idParent is null
UPDATE PROTOCOLLO.DBO.Category set FullIncrementalPath='<ID_CATEGORY DEL NODO LIVELLO 0>|'+FullIncrementalPath WHERE idCategory <= <ULTIMO ID_CATEGORY ESCLUSO ID_CATEGORY NODO LIVELLO 0>

UPDATE PRATICHE.DBO.Category set idParent= <ID_CATEGORY DEL NODO LIVELLO 0> WHERE idCategory <= <ULTIMO ID ESCLUSO ID_CATEGORY NODO LIVELLO 0> and idParent is null
UPDATE PRATICHE.DBO.Category set FullIncrementalPath='<ID_CATEGORY DEL NODO LIVELLO 0>|'+FullIncrementalPath WHERE idCategory <= <ULTIMO ID_CATEGORY ESCLUSO ID_CATEGORY NODO LIVELLO 0>

UPDATE ATTI.DBO.Category set idParent= <ID_CATEGORY DEL NODO LIVELLO 0> WHERE idCategory <= <ULTIMO ID ESCLUSO ID_CATEGORY NODO LIVELLO 0> and idParent is null
UPDATE UPDATE.DBO.Category set FullIncrementalPath='<ID_CATEGORY DEL NODO LIVELLO 0>|'+FullIncrementalPath WHERE idCategory <= <ULTIMO ID_CATEGORY ESCLUSO ID_CATEGORY NODO LIVELLO 0>

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

[SM]
E' stato introdotto un nuovo parametro 'MultiAOOFascicleEnabled' per visualizzare tutti i documenti di diverse AOO presenti all'interno dello stesso fascicolo 
per il cliente San Marino [SM]=true
Se MultiAOOFascicleEnabled = True Visualizza tutti i documenti di diverse AOO presenti all'interno dello stesso fascicolo..
Se MultiAOOFascicleEnabled = False il comportamento rimane invariato.
#MF
######################################################################################################

Il modello Excel utilizzato dal FastProtocolSender è stato modificato con l'aggiunta di una nuova colonna:

 - CodiceServizio (Indica il codice della categoria di servizio da utilizzare per la protocollazione)

La colonna deve obbligatoriamente essere presente nel file excel ma può contenere valori nulli.
E' necessario comunicare tale cambiamento a tutti i clienti che utilizzano FastProtocolSender.

#AC
######################################################################################################

Nel FastProtocolSender sono stati introdotti due nuovi parametri in app.confing

- SendToSWAFEnabled (Indica se è attivo, in fase di configurazione, la checkbox di invio a SWAF.)
- SWAFWorkflowName (Indica il nome del workflow relativo all'invio a SWAF.)

AUSL-PC
SendToSWAFEnabled = True
SWAFWorkflowName = <Nome del workflow invia a SWAF>

#AC
######################################################################################################

A new workflow has been introduced which offer the possibility to generate a new Word document with informations about a desired fascicle.
This new workflow creates new a document based on generic template stored in a word document. The generic template should contains the following details of fascicle:
	
	Field Name									|			Metadata Key
	--------------------------------------------|-------------------------------------------
	1.  Fascicle year							|	#{Year}
	2.  Fascicle number							|	#{Number}
	3.  Fascicle start date						|	#{StartDate}
	4.  Fascicle title							|	#{Title}
	5.  Fascicle object							|	#{FascicleObject}
	6.  First contact description of fascicle	|	#{Contact}
	7.  Metadata key names for every metadata	|	(for example: Matricola -> ${Matricola})

In the process of document generation the metadata keys will be replaced with real data related to fascicle.
At the end of the document 2 new tables will be attached. The first table contains informations about document units, the second table contains informations about documents.

	Column names of table related to fascicle document units:
	1.  Subject
	2.  Category
	3.  The registration date
	4.  The registration date in the fascicle
	5.  The registration user in the fascicle

	Column names of table related to fascicle documents:
	1.  File name
	2.  The created date of document
	3.  The registration user

Executing the workflow involves the following steps:
	I. Template creation
		1. Create a Word document with the desired content that contains some metadata keys
		2. Upload the word document in DocSuite (Tabelle -> Template -> Template documenti)
	II.	Workflow creation
		1. Create a new workflow repository related to fascicle (Tabelle -> Attivita)
		2. In "Proprieta di avvio" add the following 2 proprieties:
			a) "Identificativo (vedi sezione deposito documentale) del documento da generare" with the template already created at step (I)
			b) "Generazione dinamica del documento di fase avvio (OpenXML)" with value checked (true)
		3. Create a workflow step with the workflow action GenerateReport and the following input arguments (Proprieta in ingreso):
			a) Identificativo (vedi sezione deposito documentale) del documento da generare
			b) _dsw_e_Generate_DocumentMetadatas
			c) Generazione dinamica del documento di fase avvio (OpenXML)
			d) _dsw_p_FolderSelected
	III. Workflow start
		1. In DocSuite open a fascicle in view mode
		2. Select a fascicle folder to store the word document that will be generated
		3. Press "Avvia attivita" button
		4. In the window that appears select the workflow created at step (II) from dropdown list
		5. Press "Conferma" button and wait until the process will be completed
		6. See the result stored in grid of documents
#IG
######################################################################################################

[Workflow - SM - Spessa Publica ] - Guide

*Description*
	A new workflow has been introduced, which has the following responsabilities: 
		- Create a new Fascicle (destination) starting from a Dossier Folder Template
		- Find the Fascicle Folders from original and destination fascicles, which have the same name, and clone
	Fascicle Documents and Fascicle Document Units from original folders into destination folders
		- Update the MetadataValues of the destination fascicle with the values from the MetadataValues of original fascicle,
	which have the same name
		- Send email to authorized users, which contains a link to the new created fascicle

*Workflow arguments*
	
	* [Abilita selezione cartelle di Dossier] (true/false) -> enables/disables the selection of the destination Dossier Folder in the start workflow window

	* [Abilita la creazione del builder model durante l'avvio di workflow] (true/false) -> enables/disables the creation of the build model required by the workflow, if
  it is false, an error window will appear with message "Funzionalità non supportata"

	* [Imposta il filtro sul livello di gerarchia delle cartelle di fascicolo. Il filtro deve essere >= 2] (number >= 2) -> sets the hierarchy level from which the workflow starts
  analyzing the Fascicle Folders of original and destination fascicles

	* [Abilita la gestione del settore destinatario leggendolo dal modello di fascicolo] (true/false) -> enables/disables the initialization of the recipient role of the workflow,
  as the master role from the fascicle template

	* [Abilita la copia dei metadati di fascicolo] (true/false) -> enables/disables the cloning of the Metadata Values from the original fascicle into destination fascicle

	* [Abilita la copia delle unità documentali di fascicolo] (true/false) -> enables/disables the cloning of Fascicle Document Units from original fascicle folders 
  into destination fascicle folders

	* [Abilita la copia degli inserti di fascicolo] (true/false) -> enables/disables the cloning of Fascicle Documents from original fascicle folders 
  into destination fascicle folders

	*[Abilita la chiusura del fascicolo (da cui è partito il workflow)] (true/false) -> enables/disables the closing of the original fascicle after the cloning of the contents finishes

	*[Imposta il filtro sulla tipologia di Dossier] [0=Persona fisica o giuridica, 1=Oggetto fisico, 2=Procedimento, 3=Serie archivistica] = 2

#IM
######################################################################################################
[Workflow - SM - Spessa Publica ] - Note
*Nella versione corrente: non utilizzare repository con metadati obbligatori
#MF
######################################################################################################

[Workflow - SM - Riapri Fascicolo ] - Guide

*Workflow arguments*
	
	* [Imposta il filtro sul livello di gerarchia delle cartelle di fascicolo. Il filtro deve essere >= 2] (number >= 2) -> sets the hierarchy level from which the workflow starts
  analyzing the Fascicle Folders of original and destination fascicles

	* [Abilita la copia dei metadati di fascicolo] (true/false) -> enables/disables the cloning of the Metadata Values from the original fascicle into destination fascicle

	* [Abilita la copia delle unità documentali di fascicolo] (true/false) -> enables/disables the cloning of Fascicle Document Units from original fascicle folders 
  into destination fascicle folders

	* [Abilita la copia degli inserti di fascicolo] (true/false) -> enables/disables the cloning of Fascicle Documents from original fascicle folders 
  into destination fascicle folders

	* [Abilita la il collegamento trai fascicoli creati dal workflow] (true/false) -> enables/disables the linking of the fascicles managed by the workflow

	* [Workflow richiede che il fascicolo sia chiuso] (true/false) -> enables/disables workflow will be visible only if fascicle is closed
	
		* [Abilita la creazione del builder model durante l'avvio di workflow] (true/false) -> enables/disables the creation of the build model required by the workflow, if
  it is false, an error window will appear with message "Funzionalità non supportata"
#FL
######################################################################################################

[AUSL-RE]

E' necessario aggiornare la definizione dei seguenti workflow di Reggio Emilia.
	- Workflow - AUSL-RE - INVIO DOCUMENTI MYDOCSUITE
	- Workflow - AUSL-RE - INVIO DOCUMENTI A MYDOCSUITE
	- Workflow - AUSL-RE - INVIO PROTOCOLLO MYDOCSUITE
	- Workflow - AUSL-RE - PARERE MASL
	- Workflow - AUSL-RE - PROPOSTA DELIBERA

Nello specifico per tutte le activity di tipo "Invio Email":
	1) la sezione ActivityOperation relative va cambia con le seguenti nuove impostazioni.

		"ActivityOperation": {
		  "Action": "ToMessage",
		  "Area": "Build"
		}

	2) la sezione AuthorizationType  va impostata col valore "None"
		
		"AuthorizationType": "None"

	3) nella sezione EvaluationArguments, la proprietà _dsw_p_EmailEvaluationRecipient deve avere il valore PropertyString nella sezione PropertyType.

		"EvaluationArguments": [
		  {
			"Name": "_dsw_p_EmailEvaluationRecipient",
			"PropertyType": "PropertyString",

Unicamente per lo STEP 0 rimuovere tra le InputProperties e le OutputProperties le proprietà:
	- _dsw_e_Acceptance
	- _dsw_p_Operation

#FL
######################################################################################################
[Workflow - AUSL-RE - MyDocSuite ]

Nello specifico per tutte le activity di tipo "Invio documenti":
	1) nella sezione EvaluationArguments, deve essere aggiunta la seguente impostazione.

			{
				"Name": "_dsw_e_ActivityEndReferenceModel",
				"PropertyType": 1,
				"ValueInt": null,
				"ValueDate": null,
				"ValueDouble": null,
				"ValueBoolean": null,
				"ValueGuid": null,
				"ValueString": null
			}

I workflow presenti nella cartella di installazioni possono essere aggiornati direttamnte in produzione (senza modifche manuali)

#FL
######################################################################################################