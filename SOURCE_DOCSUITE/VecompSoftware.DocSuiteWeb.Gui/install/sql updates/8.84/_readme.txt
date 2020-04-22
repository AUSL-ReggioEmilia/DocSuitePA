Per un corretto funzionamento è necessario allineare i seguenti prodotti:
 - WebAPI 8.83 - ASSICURARSI DI AVERE L'ULTIMA VERSIONE DELLE WEBAPI 8.83
 - Workflow Integrations 8.84 - Per i soli clienti che hanno attivato il servizio

#FL
######################################################################################################
La procedura di migrazione del database prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:
	
	1. script DocSuite
		01. ProtocolloDB_migrations.sql
		02. PraticheDB_migrations.sql
		03. AttiDB_migrations.sql
		
#FL
#########################################################################################

Sono state apportate molte modifiche alle code e alle topic del service bus localizzate nel file SB01.Docsuitenamespace_Entities.xml.

Considerando la criticità del modello di deploy mediante tool Service Bus Explorer, è neccessario delegare l'attività a Fabrizio Lazzarotto. 
Le attività possono essere svolte anche dopo il deploy a servizi attivi.

#FL
######################################################################################################
STAMPA ETICHETTE ZEBRA:

Sono stati aggiunti due nuovi TAG per la stampa del classificatore di protocollo
	 - "%CATEGORY.CODE%" -> Stampa codice completo del classificatore di protocollo
	 - "%CATEGORY.TXT[(][0-9]{1,2}[)]%" -> Stampa il nome del classificatore (MASSIMO 9 CARATTERI)

#FL
######################################################################################################
PER AGENAS:

Impostare il parametro CurrentUserEMailSenderEnabled = false e verificare di aver impostato il parametro ProtPecSendSender con l'indirizzo email anomimizzato.
Prendere accordi con Cardone.

#FL
######################################################################################################
Introdotto nuovo parametro "WorkFlowImgPath" per la gestione delle icone di avvio attività in elenco attività

PER AUSL_RE
	{
	"DocSuite" : "~/App_Themes/DocSuite2008/imgset16/WF_DocSuite.png",
	"MyDocSuite" : "~/App_Themes/DocSuite2008/imgset16/WF_MyDocSuite.png"
	}

#MF
######################################################################################################
Introdotto nuovo parametro di ProtocolEnv 'DocumentUnitDocumentsLabel' per la gestione delle etichette dei documenti in atti/protocollo/collaborazioni
Esempio: 
Indicare per la singola tipologia di catena il nome dalla label richiesta dal cliente (es. configurazione per standard)  
	{
	"Miscellanea" : "Miscellanea",
	"MainChain": "Documenti",
	"AttachmentsChain" : "Allegati (parte integrante)",
	"AnnexedChain": "Annessi (non parte integrante)",
	"UnpublishedAnnexedChain": "Annessi da non pubblicare",
	"ProposalChain" : "Proposta",
	"ControllerChain" : "Risposta Organo Controllo",
	"AssumedProposalChain" : "Proposta assunta",
	"FrontespizioChain" : "Fronte spizio",
	"PrivacyAttachmentChain" : "Allegati riservati",
	"FrontalinoRitiroChain" : "Frontalino",
	"PrivacyPublicationDocumentChain" : "Documento di pubblicazione sulla privacy",
	"MainOmissisChain" : "Documento (omissis)",
	"AttachmentOmissisChain" : "Allegati Omissis (parte integrante)",
	"UltimaPaginaChain" : "Ultima pagina",
	"SupervisoryBoardChain" : "Consiglio di vigilanza",
	"DematerialisationChain": "Attestazione di conformità"
	}

#MF
######################################################################################################