Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.75
	- ServiceBus Listeners 8.75
	- Workflow Integrations 8.75
	- WSSeries 8.75
	- JeepService 8.75
		- Modulo PEC
	- Compilazione libreria dinamica UDS alla 8.75 (soli per i clienti che hanno già adottato il modulo UDS)
	- Tool di migrazione degli elementi delle serie documentali DSWDocumentSeriesItemMigrator

#FL
######################################################################################################
La versione 8.75 supporta i seguenti browser:
	- Internet Explorer 9 
	- Internet Explorer 10
	- Internet Explorer 11
	- Google Chrome (escluse le funzionalità di firma).

#FL
######################################################################################################
La procedura di migrazione del database prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:
	
	1. script DocSuite
		01. ProtocolloDB_migrations.sql
		02. PraticheDB_migrations.sql
		03. AttiDB_migrations.sql
	
	2. script WebAPI

	3. script DocSuite - solo quando si installa in produzione
		04. AttiDB_production.sql
	
#FL
######################################################################################################
Prima di effettuare l'aggiornamento è necessario rimnuovere il contenuto della directory bin dei seguenti prodotti:
	- WebAPI (Private e Public)
	- DocSuite
	- JeepService

#FL
######################################################################################################
Nella cartella BPM, libreria UDS, è necessario modificare a mano tutti i *controller.cs nel seguente modo:

SOSTITUIRE
using System.Web.OData;
using System.Web.OData.Extensions;
using System.Web.OData.Query

CON 
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query; 

Tale operazione si rende necessaria per evitare problemi con l'UDSMigrations
#FL
######################################################################################################
Dalla directory del sito Content/browserCondition rimuovere il file ie8.css

#AC
######################################################################################################
Nota per Sviluppo:
è stato aggiornato Typescript alla versione 2.9.2. E' necessario quindi installare l'SDK di Typescript alla versione specificata.
E' possibile scaricare il pacchetto di installazione al seguente link: https://www.microsoft.com/en-us/download/details.aspx?id=55258.
Seguire le istruzioni di installazione fornite al link precedente per il corretto funzionamento.

#AC
######################################################################################################
E' stato rivisto completamente la gestione del TenantModel, che da questa versione è automatica e inclusa all'interno della soluzione.
Per un corretto allineamento è necessario aggiornare il parametro TenantModel nel database elimnando il blocco Entities. 

Per un esempio confrontare il proprio valore di database rispetto agli esempio forniti:
	- ExampleMultiTenant_8.75.json per chi ha la gestione multitenants
	- ExampleOneTenant_8.75.json per chi ha la gestione singolo tenant

Dalla prossima versione non ci sarà più la distribuzione degli esempio e assistenza non dovrà più intervenire manualmente nella gestione del TenantModel 
se non per le specifiche sezioni dei parametri DNS dei servizi e autenticazioni applicative.

#FL
######################################################################################################
Rimuovere tutti i seguenti file json:
	- WebApi.Client.Config.Endpoints.json situati nella cartelle Config dei vari WSxxx, DocSuite e JeepService.
	- WorkflowOperationConfig.json situati nella cartelle Config dei vari WSxxx, DocSuite e JeepService.
	- ProtocolAdaptiveSearchMapping.json situati nella cartelle Config dei vari WSxxx, DocSuite e JeepService.
	- RadGridLocalizeConfig.json situati nella cartelle Config dei vari WSxxx, DocSuite e JeepService.

#FL
######################################################################################################
Sono state apportate molte modifiche alle code e alle topic del service bus localizzate nel file SB01.Docsuitenamespace_Entities.xml.

Considerando la criticità di tali modifiche è neccessario delegare l'attività a Fabrizio Lazzarotto. 
Le attività possono essere svolte anche prima del deploy, ma richiedono il fermo applicativo dei seguenti servizi:
	- ServiceBus Listeners 8.7X
	- Workflow Integrations 8.7X
	- JeepService 8.7X

#FL
######################################################################################################
Dopo l'installazione in ambiente di produzione è necessario lanciare il tool DSWDocumentSeriesItemMigrator che bonifica la colonna HasMainDocument di tutti gli elementi presenti in DocumentSeriesItem.
Prima di lanciare il tool configurare la connectionstring presente nel file di configurazione DSWDocumentSeriesItemMigrator.exe.config

#AC
######################################################################################################
L'aggiornamento del servizio WSSeries 8.75 necessità la copia dei file 
	- ProtDB.config
	- DocmDB.config
	- ReslDB.config

Nella cartella Config del servizio dal pacchetto di installazione.

#AC
######################################################################################################
Aggiornato il menù JSON DocsuiteMenuConfig.json nella sezione Menu6, aggiunto il FirstNode3.
"Menu6": {
    "Name": "Serie Documentali e Archivi",
    "Nodes": {
      .....,
      "FirstNode3": {
        "Name": "Monitoraggio trasparenza",
        "Nodes": {
          "SecondNode1": { "Name": "Sezione trasparenza" },
          "SecondNode2": { "Name": "Da settore alla sezione" },
          "SecondNode3": { "Name": "Dalla sezione al settore" },
          "SecondNode4": { "Name": "Registro monitoraggio" }
        }
      },
	  .....

#FL
######################################################################################################
L'aggiornamento del servizio WSSeries 8.75 prevede la possibilità di aggiornare il sito della trasparenza pubblica alla versione 2.3

#FL
######################################################################################################