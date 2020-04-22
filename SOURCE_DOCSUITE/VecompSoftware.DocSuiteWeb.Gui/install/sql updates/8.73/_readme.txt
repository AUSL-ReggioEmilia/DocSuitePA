Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.73
	- ServiceBus Listeners 8.73
	- Workflow Integrations 8.73
	- Fast Protocol Importer 8.73 (solo per chi ha la modalità archivi)
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
Per il corretto funzionamento della nuova gestione degli inserti è necessario che vengano aggiunti i seguenti nuovi attributi agli archivi di 
BiblosDS degli inserti dei fascicoli e dei dossier.
- L'attributo "Note", che dev'essere di tipo stringa e con modalità "Modify Always". L'attributo deve essere 
associato all'attribute group relativo al documento (non required)
- L'attributo "RegistrationUser", che dev'essere di tipo stringa e con modalità "Modify Always". L'attributo deve essere 
associato all'attribute group relativo al documento (non required)
Inoltre è necessario modificare l'attributo Filename impostandolo con modalità "Modify Always".

Modificare tali attributi per tutti i clienti.

#SZ
######################################################################################################
Sono stati introdotti due nuovi parametri in ambiente di protocollo:
- SeriesTitle
- ButtonSeriesTitle
sono da settare come i parametri corrispondenti in ambiente di atti

#SZ
######################################################################################################
In TemplateCollaborations è stata aggiunta la colonna 'JsonParameters' per configurare alcuni parametri specifici del template.
In ASL-TO, per il template di protocollo (DocumentType = 'P') va inserito il seguente valore nella colonna JsonParameters:

	[{
		"Name": "SignersEditEnabled",	
		"PropertyType": 16,
		"ValueBoolean": true
	}]

#SDC
######################################################################################################
Sono stati aggiunti i seguenti Endpoints da aggiungere al TenantModel nella sezione Entities:

      "ContainerProperty": {
        "IsActive": true,
        "Timeout": "00:00:30",
        "ODATAControllerName": "ContainerProperties"
      },
	   "UDSUser": {
        "IsActive": true,
        "Timeout": "00:00:30",
        "ODATAControllerName": "UDSUsers"
      },
	   "UDSRole": {
        "IsActive": true,
        "Timeout": "00:00:30",
        "ODATAControllerName": "UDSRoles"
      }

Vedi gli esempi di configurazione in ExampleMultiTenant_8.73.json e ExampleOneTenant_8.73.json

#AC
######################################################################################################
Per la gestione dei responsabili di procedimento dei fascicoli, nella tabella Contact 
è necessario aggiornare il campo SearchCode con il valore "DOMINIO\Account" per ogni contatto figlio
del ramo di rubrica configurato nel parametro FascicleContactId.

Per determinare velocemente il valore corretto dell'account, utilizzare la ricerca utenti della docsuite
in modo tale da inserire il valore corretto.

In caso di mancata bonifica dei contatti, la funzionalità del modulo di fascicoli è compromessa.

#SZ
######################################################################################################
In fase di rilascio occorre lanciare il DocumentUnitMigrator 8.73 in modalita' 6, per ottenere un corretto funzionamento
del modulo degli archivi. In fase di migrazione occorre specificare quale sara' l'utenza di default in AppSettings "DocumentUnitMigratorUser" 
ES: "vecompsoftware\biblosds"

#MM
######################################################################################################
A fronte della modifica della proprietà di workflow _dsw_p_AccountName in _dsw_p_Accounts, è necessario bonificare
tutti i Json dei worklfow nella tabella WorkflowRepositories in cui è presente la suddetta proprietà, impostando il name
da _dsw_p_Account_Name a _dsw_p_Accounts.

Per esempio: 

{
	"Name": "_dsw_p_Accounts",
	"PropertyType": 1,
	"ValueInt": null,
	"ValueDate": null,
	"ValueDouble": null,
	"ValueBoolean": null,
	"ValueGuid": null,
	"ValueString": "[{\"Account\":{\"AccountName\":\"VECOMPSOFTWARE\\Fabrizio.Lazzarotto\", \"EmailAddress\":\"Fabrizio.Lazzarotto@dgroove.it\",\"DisplayName\":\"Fabrizio Lazzarotto\",\"Required\":0}}]"
}

#SZ
######################################################################################################
Per tutti i clienti che hanno abilitata la gestione della securizzazione dei documenti sono stati creati i seguenti nuovi
parametri in ParameterEnv.

 - SecureDocumentWorkflowStatementVisibility = Definisce quali flussi di lavoro, relativi alla securizzazione,
   visualizzare nella finestra relativa. Modificare quindi il JSON con i flussi abilitati al cliente.
   Es. AGENAS = {
          "DOU": false,
          "SecurePaper": true
        }

 - SecurePaperServiceId = ID del servizio fornito da Land specifico per cliente necessario alla comunicazione con il servizio SecurePaper.

 - SecurePaperCertificateThumbprint = Thumbprint del certificato per la comunicazione con il servizio SecurePaper di Land. Il certificato deve
   essere installato per l'utente configurato nel listener SecurePaper del ServiceBus Receiver.
   Per recuperare tale informazione seguire le istruzioni al link seguente:
   https://docs.microsoft.com/it-it/dotnet/framework/wcf/feature-details/how-to-retrieve-the-thumbprint-of-a-certificate

   N.B. i caratteri esadecimali del thumbprint non devono essere modificati (mantenere gli spazi tra i caratteri).

  - SecurePaperServiceUrl: Indirizzo del servizio SecurePaper.

#AC
######################################################################################################
Per la gestione del contrassegno a stampa, è necessario lanciare il tool DSWBiblosDSMigrator che allinea tutti gli attributi necessari
a tutti gli archivi biblos indicati nelle location del cliente specifico.
Prima di lanciare il tool configurare:
		1. il file docuite.connectionstrings.config con le connectionstring del cliente
		2. il file DSWBiblosDSMigrator.exe.config, nella parte <client> con l'indirizzo del client di Biblos

#AC
######################################################################################################

Sono state apportate molte modifiche alle code e alle topic del service bus.
E' necessario procecede come segue:
	- aprire il Tool Service Bus Explorer
	- specificare la ConnectionString del service bus del cliente
	- Eliminare la topic di nome "entity_event", situata nel nodo principale "Topics"
	- Eliminare la topic di nome "workflow_integration", situata nel nodo principale "Topics"
	- dal menu Actions selezionare la voce Import Entries, nella finestra importare il file SB01.Docsuitenamespace_Entity_Event_Topic.xml
	- dal menu Actions selezionare la voce Import Entries, nella finestra importare il file SB02.Docsuitenamespace_Workflow_Integration_Topic.xml
	- dal menu Actions selezionare la voce Import Entries, nella finestra importare il file SB03.Docsuitenamespace_Secure_Document_Request_Queue.xml
	- dal menu Actions selezionare la voce Import Entries, nella finestra importare il file SB04.Docsuitenamespace_Secure_Paper_Request_Queue.xml

Se da errori in fase di import, contattare sviluppo. Il prodotto senza tali configurazioni potrebbe creare anomalie critiche. 
#AC
######################################################################################################

[ASL-TO]
Tra i parametri specifici per i contenitori è stato introdotto 'LinkedContainers', che indica i contenitori collegati al contenitore selezionato.
Il parametro per ASL-TO va configurato indicando l'id del contenitore privacy corrispondente in modo che quando si genera la lettera di trasmissione 
al collegio sindacale, siano comprese tra i risultati di ricerca anche le delibere che hanno il contenitore privacy associato.
Il parametro deve avere la seguente struttura:
							[-32519, -32518, -32517]

#SDC
######################################################################################################