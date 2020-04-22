Per un corretto funzionamento e necessario allineare i seguenti prodotti:
•	WebAPI 8.63
•	Listeners del ServiceBus 8.63
•	Compilazione libreria dinamica UDS alla 8.63 (soli per i clienti che hanno già adottato il modulo UDS)
•	BiblosDS 8.63

#FL
###############################################################

ATTENZIONE: gli script all'interno della cartella Production (ProtocolloDB_production, AttiDB_production e PraticheDB_production) vanno lanciati solo quando 
			si installa la versione 8.63 in ambiente di produzione per mantenere la retrocompatibilità con la 8.62 in produzione e la 8.63 in stage.

			Prima di lanciare gli script ProtocolloDB_production, AttiDB_production e PraticheDB_production è necessario attivare i template (CTRL+SHIFT+M) 
			impostando i parametri come segue:
			<UTENTE_DEFAULT, varchar(256), ''>	--> Per indicare un utente di default nella colonna RegistrationUser (N.B. non scrivere gli apici).
			<DELIBERE_DESCRIPTION, varchar(256), Delibera> --> Settare la descrizione per le Delibere impostate da cliente (Valore in TabMaster).					
			<DETERMINE_DESCRIPTION, varchar(256), Determina> --> Settare la descrizione per le Determine impostate da cliente (Valore in TabMaster).	
			<SERIES_DESCRIPTION, varchar(256), Serie Documentali> --> Settare la descrizione per le Serie Documentali impostate da cliente (Valore del parametro ProtocolEnv.DocumentSeriesName).
			
			Gli altri script di migrations sono standard e vanno lanciati nel solito modo.

			N.B. Fare particolare attenzione agli script di inserimento valori di default per i Template di collaborazione. Devono essere commentati i record che non si vogliono inserire per il cliente
			(es. la tipologia Uoia verrà abilitata solo per AUSL-RE, mentre per gli altri clienti deve essere commentata/eliminata). Non è possibile effettuare una procedura automatica a causa di problematiche
			legate alla transazione.

			Attenzione allo script Production\ProtocolloLog_migration.sql in quanto può risultare molto lungo nella sua esecuzione. Per questo motivo lo abbiamo isolato dagli altri script di migrazione e fuori dalle transazioni.

#SDC
#############################################################################

Nelle WebAPI è stata introdotta l'entità ParameterEnv, quindi ora è possibile accedere ai parametri di protocollo dalle WebAPI.
Quando si modificano i valori dei parametri in DocSuite o sul database, ricordarsi di riavviare il pool delle WebAPI per pulire i vecchi valori 
dei parametri modificati (similmente a quando si esegue la Reset Parameters).

#SDC
#############################################################################

[AUSL-PC]
E' stato introdotto un nuovo parametro 'AbsentManagersCertificates' per la gestione Direttori Assenti, funzionalità richiesta da AUSL-PC
che permette di marcare come assenti i Direttori (sanitario, generale e amministrativo) di una collaborazione, in modo da far proseguire la collaborazione
nel suo flusso quando uno di questi non è presente.
Il parametro ha la seguente struttura json:

		{
            "Roles": {
                "Role1": -32661,
                "Role2": -32765
            },
            "Managers": {
                "Manager1": {
                    "Account": "VECOMPSOFTWARE\\Sara.DalCorso",
                    "Type": "DA",
                    "RoleId": -32681,
                    "SignUser": "Sara Dal Corso",
                    "SignCertificate": "123",
                    "IsAbsenceManaged": 1
                },
                "Manager2": {
                    "Account": "VECOMPSOFTWARE\\Stefania.Zubbi",
                    "Type": "DG",
                    "RoleId": -32661,
                    "SignUser": "Stefania Zubbi",
                    "SignCertificate": "123",
                    "IsAbsenceManaged": 1
                },
                "Manager3": {
                    "Account": "VECOMPSOFTWARE\\Fabrizio.Lazzarotto",
                    "Type": "DS",
                    "RoleId": -32628,
                    "SignUser": "Fabrizio Lazzarotto",
                    "SignCertificate": "152F F2",
                    "IsAbsenceManaged": 1
                },
                "Manager4": {
                    "Account": "VECOMPSOFTWARE\\Fabrizio.Lazzarotto",
                    "Type": "DSS",
                    "RoleId": -32628,
                    "SignUser": "Marco Mascii",
                    "SignCertificate": "152F F2",
                    "IsAbsenceManaged": 0
                }
            }
        }

Nella sezione 'Roles' vanno indicati gli id dei settori delle segreterie dei direttori.
Nella sezione 'Managers' vanno indicati i dati dei direttori, per ogni manager i dati richiesti sono:
			- Account -> composto da dominio\\username
			- Type -> i valori possibili in base al ruolo sono DS, DG, DA, DSS
			- RoleId -> id del settore del direttore
			- SignUser -> nome del direttore che appare nel certificato di firma
			- SignCertificate -> codice seriale del certificato di firma del direttore 
								(si tratta del S.N. Certificato e va riportato ESATTAMENTE com'è indicato, compreso di spazi)
			- IsAbsenceManaged -> valore booleano (1 o 0) che indica se il direttore può essere marcato come assente oppure no

NOTA: il cliente ha comunicato che solo DG, DS, DA possono essere marcati come assenti, quindi DSS avrà 'IsAbsenceManaged' = 0.

La funzionalità non si attiva se il parametro è vuoto.
#SDC

#############################################################################
Sono stati introdotti i Template di Collaborazione che similmente a Protocollo popolano automaticamente alcuni campi della Collaborazione per l'inserimento.
I Template base vengono creati tramite lo script SQL ProtocolloDB_migrations.

Nel menù tabelle, la pagina dedicata alla gestione dei Template di Collaborazione sarà visibile sono a specifici gruppi utente che devono
essere impostati nel parametro TemplateCollaborationGroups

#AC
#############################################################################
E' stato modificato il file WebApi.Client.Config.Endpoints.json che contiente la configurazione degli endpoint utilizzati da DocSuite
per comunicare con le WebAPI.
Assicurarsi di copiarlo in produzione col file presente nella directory Config di DocSuite.

#AC
#############################################################################
Sono stati aggiunti i seguenti Endpoint da aggiungere al TenantModel nella sezione Entities:

      "TemplateCollaboration": {
        "IsActive": true,
        "Timeout": "00:00:15",
        "ODATAControllerName": "TemplateCollaborations"
      },
      "TemplateDocumentRepository": {
        "IsActive": true,
        "Timeout": "00:00:15",
        "ODATAControllerName": "TemplateDocumentRepositories"
      },
      "TemplateCollaborationDocumentRepository": {
        "IsActive": true,
        "Timeout": "00:00:15",
        "ODATAControllerName": "TemplateCollaborationDocumentRepositories"
      },
      "TemplateCollaborationUser": {
        "IsActive": true,
        "Timeout": "00:00:15",
        "ODATAControllerName": "TemplateCollaborationUsers"
      }

Vedi gli esempi di configurazione in ExampleMultiTenant_8.63.json e ExampleOneTenant_8.63.json

#AC
#############################################################################
E' stata aggiunta la seguente voce di menu nella sezione Menu 11 (Tabelle):
				"FirstNode21": { "Name": "Template di Collaborazione" }

La voce dipende dal parametro 'TemplateCollaborationGroups', che abilita la funzionalità del Template di Collaborazione.

#AC
#############################################################################

ATTENZIONE: è stata aggiunta alla tabella RoleUser una nuova colonna obbligatoria: UniqueId. Verificare, prima dell'aggiornamento, 
che i database siano tra loro allineati. In caso contrario lo script potrebbe dare errore nel momento in cui la suddetta colonna viene 
resa obbligatoria. 

In fase di installazione inoltre, verificare di lanciare prima lo script di protocollo, poi quelli di pratiche e atti, 
seguendo le istruzioni nell'intestazione degli script.

#MM
#############################################################################

Il file Docsuitenamespace_Entities_8.63.xml contiene l'export delle ultime configurazioni delle code e dei topic del service bus della DocSuite.
Per aggiorarle è necessario aprire il Tool Service Bus Explorer, specificare la connectiostring del service bus del cliente e dal menu Actions -> Import Entries selezionare
il file Docsuitenamespace_Entities_8.63.xml. Automaticamente eseguirà la configurazione, sovvrasvrivendo l'esistenza ma senza perdere eventuali messaggi presenti nelle code/eventi.

#FL
###############################################################

Aggiornato il FastProtocolImporter correggendo l'anomalia che si presentava quando una fattura veniva processata due volte
e si presentava errore dato dal tentativo di scrivere un file già esistente. 

#MM
###############################################################
[ASL-TO]

ATTI REGISTRO PUBBLICAZIONE:

è stata aggiunta la colonna 'Data Fine Pubblicazione' al report del registro Pubblicazioni. La visibilità della colonna è guidata dal parametro 'ResolutionJournalEndPublishingDateEnabled'.
Ricordarsi di sostituire dentro il percorso di produzione i file forniti da sviluppo nel rilascio 8.63 corrispondenti ai path 
1)VecompSoftware.DocSuiteWeb.Gui\Comm\Report\ResolutionJournal\RegistroPubblicazioniDeliberazioni_ASLTO
2)VecompSoftware.DocSuiteWeb.Gui\Comm\Report\ResolutionJournal\RegistroPubblicazioniDeterminazioni_ASLTO

ATTI REGISTRO ADOZIONE:

è stata aggiunta la colonna 'Data Esecutività' al report del registro Adozioni. La visibilità della colonna è guidata dal parametro 'ResolutionJournalEffectivenessDateEnabled'.
Ricordarsi di sostituire dentro il percorso di produzione i file forniti da sviluppo nel rilascio 8.63 corrispondenti ai path 
1)VecompSoftware.DocSuiteWeb.Gui\Comm\Report\ResolutionJournal\ResolutionJournal_Registro_Deliberazioni_ASLTO
2)VecompSoftware.DocSuiteWeb.Gui\Comm\Report\ResolutionJournal\ResolutionJournal_Registro_Determinazioni_ASLTO

#IS
###############################################################

Il file ProtocolloLog_migration.sql contiene il file di migrazione esterno all'esecuzione per quei clienti che hanno molti log.
Infatti riguarda la creazione di un nuovo LogType per l'invio a settori in ProtocolLog per ***REMOVED*** che si attiva con il parametro IsLogSendToRolesEnable.

#CC
###############################################################

Nel JeepService sono stati aggiunti tre parametri al DocSeriesImporter che consentono di scegliere se eliminare, in fase di importazione,
il documento principale, gli annessi, e gli annessi non pubblicati in modo separato.

#MM
###############################################################

Il parametro di Atti 'MailCategorySelection', che governava la possibilità di preselezionare i settori prima di entrare nella pagina di invia mail, è stato rimosso. 
Questa funzionalità, fino ad ora presente solo per l'Invio Settori da Atti, è stata estesa anche a Protocollo, Archivi e Serie documentali. 
Per questo è stato introdotto il nuovo parametro 'MailRecipientsSelectionEnabled', che governa la funzionalità in tutti e 4 i moduli.
Per ***REMOVED*** e tutti i clienti che avevano il parametro 'MailCategorySelection' attivo, va attivato il nuovo parametro 'MailRecipientsSelectionEnabled'.

#SDC
#############################################################################

E' stato aggiunto il parametro 'TemplatesAuthorizations' che consente di modificare l'ordine dei firmatari e di scegliere dei firmatari non rimuovibili
nelle collaborazioni. Il parametro ha una struttura json del tipo:

	  [{
        "TemplateName": "test marco settore",
        "ManagerDeletable": true,
        "AddSignersOnTop": true,
        "LockedSigners": ["Marco Mascii", "Sara Dal Corso"]
        }]

dove TemplateName è il nome del template di collaborazione scelto, AddSignersOnTop permette di cambiare l'ordine dei firmatari, e ManagerDeletable permette di
rendere non rimuovibili i firmatari LockedSigners selezionati. NB: dei firmatari non rimuovibili va impostata la DESCRIZIONE. Va aggiunta una struttura di questo tipo per 
ogni template di collaborazione in cui si desiderano impostare queste modifiche.

#MM
#############################################################################