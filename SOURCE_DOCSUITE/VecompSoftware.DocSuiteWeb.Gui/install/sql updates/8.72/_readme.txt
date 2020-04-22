Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.72
	- ServiceBus Listeners 8.72
	- DocumentUnitMigrator 8.72
	
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

Attenzione: per il funzionamento del pacchetto privacy è necessario che la tabella document unit sia completamente allineata.
Prima di installare quindi va lanciato il DocumentUnitMigrator per tutte le unità documentarie presenti (di qualsiasi anno).

#SDC
######################################################################################################

Per la gestione della privacy, è necessario lanciare il tool DSWBiblosDSMigrator che aggiunge l'attributo 
'PrivacyLevel' a tutti gli archivi biblos indicati nelle location del cliente specifico.
Prima di lanciare il tool configurare:
		1. il file docuite.connectionstrings.config con le connectionstring del cliente
		2. il file DSWBiblosDSMigrator.exe.config, nella parte <client> con l'indirizzo del client di Biblos

#SDC
######################################################################################################
La privacy in Inserimento di Serie Documentali è abilitata solo se il parametro ProtocolEnv.DocumentSeriesReorderDocumentEnabled è a False

#CC
######################################################################################################
E' stata aggiornato il menu, in particolare sono state apportate modifiche al menù delle Tabelle 
(rinominata la voce "Tipologia Atto" in "Template di trasparenza" e raggruppate sotto la voce "Template" i nodi 
 "Template di protocollo", "Template di collaborazione", "Template di trasparenza" e "Deposito documentale"):

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
      "FirstNode3": { "Name": "Categorie servizio" },
      "FirstNode4": { "Name": "Tipologia spedizione " },
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
          "SecondNode3": { "Name": "Versione del classificatore" }
        }
      },
      "FirstNode9": {
        "Name": "Anagrafiche",
        "Nodes": {
          "SecondNode1": { "Name": "Rubrica" },
          "SecondNode2": { "Name": "Liste di contatti" },
          "SecondNode3": { "Name": "Titoli studio" }
        }
      },
      "FirstNode11": { "Name": "Settori" },
      "FirstNode12": { "Name": "Contenitori" },
      "FirstNode14": { "Name": "Scatoloni" },
      "FirstNode15": {
        "Name": "Stampe",
        "Nodes": {
          "SecondNode1": { "Name": "Classificatore" },
          "SecondNode2": { "Name": "Settori" },
          "SecondNode3": { "Name": "Contenitori" },
          "SecondNode4": { "Name": "Settori con sicurezza" },
          "SecondNode5": { "Name": "Contenitori con sicurezza" }
        }
      },
      "FirstNode18": { "Name": "Workflow" },
      "FirstNode19": { "Name": "Gruppi" },
      "FirstNode24": { "Name": "Utenti" },
      "FirstNode21": {
        "Name": "Template",
        "Nodes": {
          "SecondNode1": { "Name": "Template di protocollo" },
          "SecondNode2": { "Name": "Template di collaborazione" },
          "SecondNode3": { "Name": "Template di trasparenza" },
          "SecondNode4": { "Name": "Deposito documentale" }
        }
      },
      "FirstNode23": { "Name": "Metadati" },
      "FirstNode25": { "Name": "Livelli privacy"}
    }
  }

#SDC
######################################################################################################
Sono stati aggiunti i seguenti Endpoints da aggiungere al TenantModel nella sezione Entities:


      "UDSLog": {
        "IsActive": true,
        "Timeout": "00:00:30",
        "ODATAControllerName": "UDSLogs"
      },
      "UDSAuthorization": {
        "IsActive": true,
        "Timeout": "00:00:30",
        "ODATAControllerName": "UDSAuthorizations"
      },
      "TemplateReport": {
        "IsActive": true,
        "Timeout": "00:00:30",
        "ODATAControllerName": "TemplateReports"
      }

Vedi gli esempi di configurazione in ExampleMultiTenant_8.72.json e ExampleOneTenant_8.72.json

#MM 
######################################################################################################