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