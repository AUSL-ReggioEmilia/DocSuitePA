Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- non sono previsti ulteriori progetti 8.88
#FL
######################################################################################################

La procedura di migrazione del database prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:
	
	1. script DocSuite
		01. ProtocolloDB_migrations.sql
		02. PraticheDB_migrations.sql
		03. AttiDB_migrations.sql

ATTENZIONE PER I CLIENTI "STORICO" : Verificare l'esistenza della tabella PosteOnLineAccountRole nel database di protocollo.

Eventualmente ri-crearla a mano
		CREATE TABLE [dbo].[PosteOnLineAccountRole](
			   [IdPosteOnLineAccount] [smallint] NOT NULL,
			   [idRole] [smallint] NOT NULL,
		CONSTRAINT [PK_PosteOnLineAccountRole] PRIMARY KEY CLUSTERED 
		(
			   [IdPosteOnLineAccount] ASC,
			   [idRole] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
		GO

#FL
######################################################################################################
SOLO PER ASLTO: 
Aggiornare la  tabella TabWorkflow.
Le seguenti update spostano in avanti di due step lo stato di adozione, in mod da permettere di inserire 2 nuovi step

update TabWorkflow set step= 6 where step= 4 and WorkflowType = 'W04'
update TabWorkflow set step= 5 where step= 3 and WorkflowType = 'W04'
update TabWorkflow set step= 4 where step= 2 and WorkflowType = 'W04'

Aggiungere nella tabella TabWorkflow 2 nuovi step per il flusso delle Delibere

Aggiunge lo step "Agli affari generali"
[WorkflowType]=W04
[Step]=2
[Description]=ControlloIn
[FieldDocument]=idProposalFile
[DocumentImageFile]=FileProposta.gif
[DocumentDescription]=Doc. in approv.
[FieldDate]=ProposeDate
[FieldUser]=ProposeUser
[Attachment]=1
[FieldAttachment]=idAttachements
[BiblosFileProperty]=.DEL.
[OperationStep]=MODIFY[1]
[ViewDocumentBitRight]=3
[ViewAttachmentBitRight]=3
[ViewOnlyActive]=1
[ViewPreStep]=0
[ChangeableData]=Container|Role|Object|Note|Category|Recipient|Manager|StatusData
[ManagedWorkflowData]=Date[.TODAY.OBB.]
[Template]=NULL
[CustomDescription]=Agli affari generali
[FieldPrivacyAttachment]=idPrivacyAttachments
[FieldFrontalinoRitiro]=NULL
[PubblicaRevocaPage]=NULL
[FieldAnnexed]=idAnnexes
[FieldDocumentsOmissis]=idMainDocumentsOmissis
[FieldAttachmentsOmissis]=idAttachmentsOmissis
[ViewCurrentDocument]=0
[ViewCurrentAttachment]=0
[FieldProtocol]=NULL

Aggiunge lo step "Da adottare"
[WorkflowType]=W04
[Step]=3
[Description]=ControllooUT
[FieldDocument]=idProposalFile
[DocumentImageFile]=FileProposta.gif
[DocumentDescription]=Doc. Approvato
[FieldDate]=ProposerWarningDate
[FieldUser]=ProposerWarningUser
[Attachment]=1
[FieldAttachment]=idAttachements
[BiblosFileProperty]=.DEL.
[OperationStep]=NEXT[7]
[ViewDocumentBitRight]=3
[ViewAttachmentBitRight]=3
[ViewOnlyActive]=1
[ViewPreStep]=0
[ChangeableData]=Note|Manager|OCData[.CS.]|StatusData
[ManagedWorkflowData]=Date[.TODAY.OBB.]
[Template]=NULL
[CustomDescription]=Da adottare
[FieldPrivacyAttachment]=idPrivacyAttachments
[FieldFrontalinoRitiro]=NULL
[PubblicaRevocaPage]=NULL
[FieldAnnexed]=idAnnexes
[FieldDocumentsOmissis]=idMainDocumentsOmissis
[FieldAttachmentsOmissis]=idAttachmentsOmissis
[ViewCurrentDocument]=0
[ViewCurrentAttachment]=0
[FieldProtocol]=NULL

Aggiornare la tabella ResolutionWorkflow
le seguenti update spostano di due step in avanti le resolution di tipo delibera.

update RW set step=6 from ResolutionWorkflow as RW  inner join Resolution on  RW.idResolution= Resolution.idResolution 
where Resolution.WorkflowType='W04' and Step =4

update RW set step=5 from ResolutionWorkflow as RW  inner join Resolution on  RW.idResolution= Resolution.idResolution 
where Resolution.WorkflowType='W04' and Step =3

update RW set step=4 from ResolutionWorkflow as RW  inner join Resolution on  RW.idResolution= Resolution.idResolution 
where Resolution.WorkflowType='W04' and Step =2


Modificare gli step di Adozione in modo da permette di firmare anche le altre catene documentali modificando in campo "ManagedWorkflowData" della TabWorkflow
inserendo per
[WorkflowType]=W04
[Step]=4

il valore "Date[.INS-Tested.MOD-Tested.OBB.DEL-STEP.]|Document[.INS.MOD.]|Attachment[.INS.MOD.SIGN.]|DocumentsOmissis[.INS.MOD.INS-SIGN.]|AttachmentOmissis[.INS.MOD.INS-SIGN.]|Annexed[.INS.MOD.INS-SIGN.]"

[WorkflowType]=W05
[Step]=3
il valore "Date[.TODAY-Tested.MOD-Tested.OBB.DEL-STEP.]|Document[.INS.MOD.]|Attachment[.INS.MOD.INS-SIGN.]|DocumentsOmissis[.INS.MOD.INS-SIGN.]|AttachmentOmissis[.INS.MOD.INS-SIGN.]|Annexed[.INS.MOD.INS-SIGN.]"

Modificare lo step di "Agli affari generali" in modo da permette di fare un retro step in "proposta" modificando il campo "ManagedWorkflowData" della TabWorkflow
aggiungendo per
[WorkflowType]=W04
[Step]=2
il valore |DELETE[2]
 
#MF
######################################################################################################
Introdotto nuovo parametro di ProtocolEnv 'ForceDeleteCollaborationEnabled', permette di cancellare una collaborazione, previa conferma, anche se ci sono documenti in checkout.
di default è "False".
Per il cliente ASLTO :  "True""
#MF
######################################################################################################
Introdotto nuovo parametro di ProtocolEnv 'CheckResolutionCollaborationOriginEnabled', abilita la verifica se una collaborazione è stata generata dal flusso affari generali.
di default è "False".
Per il cliente ASLTO :  "True""
#MF
######################################################################################################
Introdotto nuovo parametro di ProtocolEnv 'EnabledEmailAttachmentPages', 
Gestisce la possibilità di abilitare l'inserimento degli allegati nelle varie pagine mailsender.
Settare di default come da esempio
{
	"CollaborationMailSender": false,
	"GenericMailSender": false,
	"ResolutionMailSender": false,
	"UDSMailSender": (vedere valore EnableProtocolMailAttachment),
	"ProtocolMailSender": (vedere valore EnableProtocolMailAttachment),
}

Per [ASL-TO]
{
	"CollaborationMailSender": true,
	"GenericMailSender": false,
	"ResolutionMailSender": true,
	"UDSMailSender": (vedere valore EnableProtocolMailAttachment),
	"ProtocolMailSender": (vedere valore EnableProtocolMailAttachment),
}
Rimosso il parametro EnableProtocolMailAttachment
#MF
######################################################################################################
Introdotto nuovo parametro di ProtocolEnv 'SearchProtocolRblDefaultValue',
Gestisce i valori di default delle RadioButtonList nella pagina di ricerca di protocollo,
nel caricamento della pagina e al click del tasto "Svuota Ricerca"
Settare di default come da esempio
{
"rblAtLeastOne": "0",
"rblTextMatchMode": "Contains",
"rblClaim":  "2",
"rblClausola": "AND"
}

-rblClausola riferito al campo "Oggetto" significato/valore
	Tutte le parole = AND
	Almeno una parola = OR

-rblClaim riferito al campo "Reclamo"  significato/valore
	Si = 0
	No = 1
	Tutti = 2

-rblTextMatchMode riferito al campo "Mittente/Destinatario"  significato/valore
	Contiene = Contains
	Combinazioni = Anywhere
	Esattamente = Equals
	Inizia con = StartsWith
	Finisce con = EndsWith

-rblAtLeastOne riferito al campo "Mittente/Destinatario" significato/valore
	Tutte le parole= 0
	Almeno una parola = 1
#MF
######################################################################################################
######################################################################################################
Introdotto nuovo parametro di ResolutionEnv 'ResolutionDeclineReasonLabel', 
Definisce il nome della etichetta 'Motivazione di annullamento' nel sommario d'atto.

ASLTO:Restituzione al proponente
Default: Motivazione di annullamento

#MF
######################################################################################################
Introdotto nuovo parametro di ResolutionEnv 'CheckResolutionStep', 
Identifica uno specifico step nella tabella 'TabWorkflow'

per ASLTO: W04|CONTROLLOIN 

Abilita per il WorkflowType 'W04' (Deliberazione) e lo step description 'CONTROLLOIN',
usato insieme al paramentro 'ColumnReturnFromCollaborationVisible' attivo,
la visualizzazione ,nella griglia dei risultati di ricerca flusso degli atti,
delle icone che indentificano se tale atto è stato precedentemento restituito al proponente o da una collaborazione.
#MF
######################################################################################################
Introdotto nuovo parametro di ResolutionEnv 'ResolutionDeclineTextEnabled'
Abilita la visualizzazione, nel sommario d'atto, del pannello con la motivazione dell'annullamento se presente.

per:
ASLTO: True
AUSL-PC: True
AUSL-RE: True
Default: False

#MF
######################################################################################################
Il modello xls relativo all'importazione dei contatti manuali è stato modificato.
Ora la lista delle colonne del documento sono:

 - RagioneSociale
 - PEC
 - Cognome
 - Nome
 - DataNascita
 - e-mail
 - CodFisc/PIVA
 - TitoloStudio
 - Via/Piazza/Corso
 - Indirizzo
 - NCivico
 - CAP
 - Citta
 - Provincia
 - Telefono
 - Fax
 - Note

#AC
######################################################################################################
E' stata introdotta la possibilità di visualizzare lo stato di conservazione verso ParER di un archivio nello stile
di protocollo.
Il pannello è visualizzato solamente se l'archivio è configurato per la conservazione, ossia se nell'xml dell'archivio il parametro
"ConservationEnabled" risulta attivato (ConservationEnabled = "true").
Per AUSL-PC verificare che gli archivi Libro Giornale e Libro Inventario abbiano attivo il parametro "ConservationEnabled".

#AC
######################################################################################################

E' stata introdotta la possibilità di far vedere i protocolli creati dal WSProt nella vista di scrivania "da leggere". 
Il comportamento è parametrizzato a livello locale del WSProt, mediante chiave nel file wsprot.appsettings.config

  <add key="DocSuite.WSProt.CreateLogToRead" value="false"/>

 Di default è settato su false in quanto è il comportamento Standard. 
 
 Ci potrebbero essere intallazioni complesse, come ad esempio a PIACENZA che prevede due WSProt :
	1 - Nel server pubblico per l'integrazione SIRER, il paranetro DocSuite.WSProt.CreateLogToRead va settato a true
	2 - Il mentre uno interno per Syntech, il paranetro DocSuite.WSProt.CreateLogToRead va mantenuto a false.

E' necessario essere certi che per tutti i clienti venga inserito il valore nell file wsprot.appsettings.config
  <add key="DocSuite.WSProt.CreateLogToRead" value="false"/>

#FL
######################################################################################################
Introdotto nuovo parametro di ProtocolEnv 'RemoteSignDelegateEnabled',di default a false,
Tale parametro abilita la firma delle collaborazioni agli utenti delegati senza la necessità di effettuare il cambio responsabile.

Per
CTT: True

#MF
######################################################################################################
INVIA ECCEZZIONALE
lanciare lo script per la creazione della seguente SQL function

CREATE FUNCTION [webapiprivate].[Collaboration_FX_CollaborationsDeletationSigning](
	@Signers string_list_tbltype READONLY,
	@IsRequired bit)
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate, Collaboration.TemplateName,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate, C_CS.IsAbsent as CollaborationSign_IsAbsent,
			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
		
		FROM   dbo.Collaboration Collaboration
			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration			
			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
			right outer join dbo.CollaborationSigns CC_CS on Collaboration.IdCollaboration = CC_CS.IdCollaboration				
				and CC_CS.SignUser IN (SELECT val FROM @Signers)
				and CC_CS.IsActive = 1
				and ((@IsRequired is null and CC_CS.IsRequired in (1,0)) or (@IsRequired is not null and CC_CS.IsRequired = @IsRequired))
		WHERE			
			Collaboration.IdStatus = 'IN'
			and Collaboration.IdCollaboration not in (SELECT CA.idCollaborationChild
												 FROM   dbo.CollaborationAggregate CA)
												 );


#MF
######################################################################################################