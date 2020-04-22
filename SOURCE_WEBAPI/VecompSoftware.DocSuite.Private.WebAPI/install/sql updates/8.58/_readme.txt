Per ottimizzare i processi di migrazioni e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - EnterpriseLibrary.Logging.config
 - EnterpriseLibrary.Validation.config
 - WebApi.appSettings.config
 - WebApi.connectionStrings.config
 - WebApi.system.servicemodel.behaviors.config
 - WebApi.system.servicemodel.bindings.config
 - WebApi.system.servicemodel.client.config

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#FL
###############################################################
E' stato aggiunto un nuovo parametro nel file WebApi.AppSettings.config che deve riportare i seguenti valori:

<add key="VecompSoftware.DocSuiteWeb.MessageLocationId" value="<Il valore ID della tabella location corrisponente all'archivio dei messaggi>" />
#AC
################################

E' stato aggiunto un nuovo parametro nel file WebApi.AppSettings.config che deve riportare i seguenti valori:

<add key="VecompSoftware.DocSuiteWeb.Security.ContextType" value="Domain" />

Di default su tutte le installazione con ambiente di Dominio va lasciato il valore "Domain".
Per tutte le installazioni in cui i server non sono in join con un Windows Domain impostare "Machine"

Riposto la documentazione tecnica:

        Machine				 -> The computer store. This represents the SAM store.
        Domain				 -> The domain store. This represents the AD DS store.
        ApplicationDirectory -> The application directory store. This represents the AD LDS store.

#FL
################################

E' stato aggiunto un nuovo parametro nel file Workflow.Client.Config.json che corrisponde all'endpoint delle webapi per il workflow manager:

"UriToSendWorkflowManager": "http://localhost/DSW.WebAPI/api/wfm/"

#FL
################################

ATTENZIONE: controllare che nel file WebApi.AppSettings.config siano presenti i parametri 'TenantName' e 'TenantId'.

#SDC
################################