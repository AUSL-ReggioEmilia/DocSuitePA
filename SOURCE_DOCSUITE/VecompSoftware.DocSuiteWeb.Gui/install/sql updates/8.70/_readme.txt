Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.70
	- JeepService 8.70 - Modulo PEC
	- ServiceBus Listeners 8.70
	- Workflow Integrations 8.70
	
#FL
######################################################################################################
La procedura di migrazione del database prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:
	
	1. script DocSuite
		01. ProtocolloDB_migrations.sql
		02. PraticheDB_migrations.sql
		03. AttiDB_migrations.sql
	
	2. script WebAPI
	

#SDC
######################################################################################################
Sono stati aggiunti i seguenti Endpoints da aggiungere al TenantModel nella sezione Entities:

"MetadataRepository": {
				"IsActive": true,
				"Timeout": "00:00:30",
				"ODATAControllerName": "MetadataRepositories"
			},
"PrivacyLevel": {
        "IsActive": true,
        "Timeout": "00:00:30",
        "ODATAControllerName": "PrivacyLevels"
      }

Vedi gli esempi di configurazione in ExampleMultiTenant_8.70.json e ExampleOneTenant_8.70.json

#MM - #SZ
######################################################################################################
E' stata aggiornato il menu, in particolare sono state apportate modifiche al menù delle Tabelle 
(aggiunte le voci "Metadati Fascicoli", "Livelli Privacy" e "Utenti" e raggruppate alcune voci sotto "Titolario" e "Anagrafiche"):

"Menu11": {
    "Name": "Tabelle",
    "Nodes": {
      "FirstNode1": {
        "Name": "Organigramma",
        "Nodes": {
          "SecondNode1": { "Name": "Gestione" }
        }
      },
      "FirstNode2": { "Name": "Oggetti" },
      "FirstNode3": { "Name": "Categorie Servizio" },
      "FirstNode4": { "Name": "Tipologia spedizione " },
      "FirstNode5": { "Name": "Template di Protocollo" },
      "FirstNode6": { "Name": "Notifiche SMS ricezione PEC" },
      "FirstNode7": {
        "Name": "Archivi",
        "Nodes": {
          "SecondNode1": { "Name": "Designer" },
          "SecondNode2": { "Name": "Ricerca" },
          "SecondNode3": { "Name": "Tipologie" }
        }
      },
      "FirstNode8": {
        "Name": "Titolario",
        "Nodes": {
          "SecondNode1": { "Name": "Classificatore" },
          "SecondNode2": { "Name": "Massimario di scarto" },
          "SecondNode3": { "Name": "Versione del Classificatore" }
        }
      },
      "FirstNode9": {
        "Name": "Anagrafiche",
        "Nodes": {
          "SecondNode1": { "Name": "Rubrica" },
          "SecondNode2": { "Name": "Liste di contatti" },
          "SecondNode3": { "Name": "Titoli Studio" }
        }
      },
      "FirstNode11": { "Name": "Settori" },
      "FirstNode12": { "Name": "Contenitori" },
      "FirstNode13": { "Name": "Tipologia Atto" },
      "FirstNode14": { "Name": "Scatoloni" },
      "FirstNode15": {
        "Name": "Stampe",
        "Nodes": {
          "SecondNode1": { "Name": "Classificatore" },
          "SecondNode2": { "Name": "Settori" },
          "SecondNode3": { "Name": "Contenitori" },
          "SecondNode4": { "Name": "Settori con Sicurezza" },
          "SecondNode5": { "Name": "Contenitori con Sicurezza" }
        }
      },
      "FirstNode18": { "Name": "Workflow" },
      "FirstNode19": { "Name": "Gruppi" },
      "FirstNode24": { "Name": "Utenti" },
      "FirstNode21": { "Name": "Template di Collaborazione" },
      "FirstNode22": { "Name": "Deposito Documentale" },
      "FirstNode23": { "Name": "Metadati" },
      "FirstNode25": { "Name": "Livelli Privacy"}
    }
  }

Inoltre nel menù Amministrazione è stata tolta il nodo "Security User", che era figlio di "Utenti Gruppi", ed è stata rinominata la voce "Utenti" in "Lista accessi":

"FirstNode2": { "Name": "Lista accessi" }

e 

 "FirstNode5": {
        "Name": "Utenti Gruppi",
        "Nodes": {
          "SecondNode1": { "Name": "Lista" },
          "SecondNode4": { "Name": "Importa Gruppi" }
        }
      }


#MM - #SDC - #SZ
######################################################################################################

Sono state apportate molte modifiche alle code e alle topic del service bus.
E' necessario procecede come segue:
	- aprire il Tool Service Bus Explorer
	- specificare la ConnectionString del service bus del cliente
	- Eliminare la topic di nome "entity_event", situata nel nodo principale "Topics"
	- Eliminare la topic di nome "workflow_integration", situata nel nodo principale "Topics"
	- dal menu Actions selezionare la voce Import Entries, nella finestra importare il file SB01.Docsuitenamespace_Entity_Event_Topic.xml
	- dal menu Actions selezionare la voce Import Entries, nella finestra importare il file SB02.Docsuitenamespace_Workflow_Integration_Topic.xml
	- dal menu Actions selezionare la voce Import Entries, nella finestra importare il file SB03.Docsuitenamespace_SOLO_AUSLRE_Integration_ZEN.xml

Se da errori in fase di import, contattare sviluppo. Il prodotto senza tali configurazioni potrebbe creare anomalie critiche. 
#FL
######################################################################################################

E' stato aggiunto il parametro PasswordEncryptionKey che permette di criptare la password delle PECMailBox se presente.
Per funzionare correttamente e' necessario utilizzare un valore stringa di 32 caratteri. Si suggerisce l'utilizzo del sito:
https://passwordsgenerator.net/ per la generazione della chiave.

#MM
######################################################################################################

ATTENZIONE! Solo per i clienti che abilitano le funzionalità di apposizione glifo di securizzazione dei documenti.
Gli archivi Biblos relativi a Collaborazione, Protocollo, Atti, Serie Documentali e Archivi devono avere un nuovo attributo 
in cui verrà settato il riferimento all'iddocument del documento securizzato.
L'attributo deve avere nome SecureDocumentId e deve avere le seguenti caratteristiche:
 - Type = String
 - Mode = Modify Always
 - AttributeGroup = Default
 - Required = False

#AC
######################################################################################################