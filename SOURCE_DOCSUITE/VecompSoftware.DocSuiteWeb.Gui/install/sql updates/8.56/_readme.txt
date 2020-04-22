NB: Per la DocSuite 8.56 il requisito minumo di funzionamento è il framework .NET 4.6.1


Per il corretto funzionamento della release 8.56 di DocSuite devono essere aggiornare le WebAPI alla versione 8.56.

#AC
#################################################################

PER SVILUPPO
ATTENZIONE: una volta terminata la versione 8.56, gli script di Protocollo e Pratiche (sia DSW sia WebAPI) vanno aggiunti agli script 
			ProtocolloDB_before_migrations.sql e PraticheDB_before_migrations.sql presenti nella cartella ***REMOVED***.

#SDC
#################################################################

E' stato introdotto un nuovo parametro "WorkflowManagerEnabled" per ***REMOVED***, il quale se abilitato:
quindi ["WorkflowManagerEnabled" = true], permette di poter visualizzare la scrivania di "Workflow" nel menu Utente.
Se ["WorkflowManagerEnabled" = false], non permette la visualizzazione di tale modulo di "Workflow"
Per [***REMOVED***] = true

#GN
#################################################################

E' stato introdotto un nuovo parametro "IP4DGroups" che gestisce quali sono i gruppi di dominio utilizzabili in
fase di ricerca destinatari nella pagina di Invia IP4D.
Una configurazione di esempio di tale parametro è la seguente:

[
	"CN=DSW_Lazzarotto,OU=DocSuiteWeb,OU=vecompsoftware,DC=vecompsoftware,DC=local",    
	"CN=DSW_DalCorso,OU=DocSuiteWeb,OU=vecompsoftware,DC=vecompsoftware,DC=local",
    ... ecc
]

Il parametro deve contenere tutti i gruppi nei quali è possibile ricercare l'utente (nel formato Distinguished Name di ldap).
NB: Attenzione che il parametro dipende dalla configurazione del parametro BasicPersonSearcherKey.
Se la query LDAP non funziona verificare se la sintassi è compatibile con la query prodotta.

#AC
#################################################################

E' stato modificato il file WorkflowOperationConfig.json che contiene la configurazione delle WorkflowActivity in DocSuite.
Assicurarsi di aggiornare tale file nella directory Config di DocSuite.

#AC
#################################################################

E' stato modificato il file WebApi.Client.Config.Endpoints.json che contiente la configurazione degli endpoint utilizzati da DocSuite
per comunicare con le WebAPI.
Nello specifico sono state aggiunte le seguenti voci:
	
	{
      "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
      "EndpointName": "WorkflowRoleMapping",
      "AddressName": "API-EntityAddress",
      "ControllerName": "WorkflowRoleMapping"
    },
    {
      "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
      "EndpointName": "ICommandProtocolExternalViewer",
      "AddressName": "API-ServiceBusAddress",
      "ControllerName": "Queue"
    },
    {
      "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
      "EndpointName": "CommandProtocolExternalViewer",
      "AddressName": "API-ServiceBusAddress",
      "ControllerName": "Queue"
    },
    {
      "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
      "EndpointName": "IEventProtocolExternalViewer",
      "AddressName": "API-ServiceBusAddress",
      "ControllerName": "Topic"
    },
    {
      "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
      "EndpointName": "EventProtocolExternalViewer",
      "AddressName": "API-ServiceBusAddress",
      "ControllerName": "Topic"
    }

Assicurarsi di aggiornare tale file nella directory Config di DocSuite.

#AC
#################################################################

Sono stati aggiunti i seguenti Endpoint da aggiungere al TenantModel nella sezione Entities:

      "WorkflowActivity": {
        "IsActive": true,
        "Timeout": "00:00:30",
        "ODATAControllerName": "WorkflowActivities"
      },
      "WorkflowInstance": {
        "IsActive": true,
        "Timeout": "00:00:30",
        "ODATAControllerName": "WorkflowInstances"
      },
      "WorkflowProperty": {
        "IsActive": true,
        "Timeout": "00:00:30",
        "ODATAControllerName": "WorkflowProperties"
      },
      "WorkflowRepository": {
        "IsActive": true,
        "Timeout": "00:00:30",
        "ODATAControllerName": "WorkflowRepositories"
      },
      "WorkflowRoleMapping": {
        "IsActive": true,
        "Timeout": "00:00:30",
        "ODATAControllerName": "WorkflowRoleMappings"
      }

Impostare la proprietà IsActive = true solo ed esclusivamente per il Tenant corrente.
Vedi gli esempi di configurazione in ExampleMultiTenant_8.56.json e ExampleOneTenant_8.56.json

#AC
#################################################################

Il file Docsuitenamespace_Entities_8.56.xml contiene l'export delle ultime configurazioni delle code e dei topic del service bus della DocSuite.
Per aggiorarle è necessario aprire il Tool Service Bus Explorer, specificare la connectiostring del service bus del cliente e dal menu Actions -> Import Entries selezionare
il file Docsuitenamespace_Entities_8.56.xml. Automaticamente eseguirà la configurazione, sovvrasvrivendo l'esistenza ma senza perdere eventuali messaggi presenti nelle code/eventi.

#FL
###############################################################