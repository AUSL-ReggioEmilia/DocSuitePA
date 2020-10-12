######################################################################################################
Per ottimizzare i processi di migrazioni e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - EnterpriseLibrary.Logging.config
 - global.configuration.json
 - Integrations.connectionStrings.config
 - Integrations.system.servicemodel.behaviors.config
 - Integrations.system.servicemodel.bindings.config
 - Integrations.system.servicemodel.client.config
 - WebApi.Client.Config.Addresses.json

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#
######################################################################################################
Per il modulo AFOL.ERP.RDA è necessario configurare il modello "FascicleObject" con la 
proprietà corretta dall'oggetto RDADocSuiteCommand.

es.
"FascicleObject": "RDA {0:number}"

La corrispondenza tra le proprietà dell'oggetto RDADocSuiteCommand e le colonne del database è la seguente:

    Proprietà RDADocSuiteCommand  -  Nome della colonna del database
            Number                      Numero
            SupplierName                Fornitore_Denominazione
            PIVACF                      Fornitore_PIVACF
            CostCenter                  Centro_Costo
            Typology                    Tipologia
            ApplicantArea               Area_Richiedente

#FL
######################################################################################################
Per alcuni moduli del servizio VecompSoftware.BPM.Integrations è necessario aggiungere nel json "module.configuration.json"
una nuova chiave di configurazione relativa all'identificativo del TenantAOOId (prendere il valore del TenantAOO relativo al TenantId
già specificato nel json).

es.
"TenantAOOId": "ff2efa82-4ea3-471e-af22-f784bbf09c25"

Per la corretta configurazione si prega di utilizzare come master i file di configurazione di ogni singolo modulo nella directory Install -> 9.05 -> default_configurations
del servizio VecompSoftware.BPM.Integrations.

#AC
######################################################################################################

NOTA TECNICA PER IL CLIENTE TECMARKET - VALIDA SOLO PER LA CONFIGURAZIONE DEGLI AGENT: 
    Modificare il file "WebApi.Client.Config.Addresses.json" in modo tale da specificare a mano le credenziali dell'utente che effettua le chiamate alle webapi, come esempio sotto riportato.
    
    Valorizzare corretamente l'utente CHE NON DEVE ESSERE QUELLO APPLICATIVO AMMINISTRATIVO DELLA DOCSUITE.

        "Username": "VECOMPSOFTWARE\\utente_non_admin_della_macchina",
        "Password": "Passw0rd"

{
  "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.HttpClientConfiguration, VecompSoftware.DocSuiteWeb.Model",
  "Addresses": [
    {
      "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.BaseAddress, VecompSoftware.DocSuiteWeb.Model",
      "AddressName": "API-WorkflowManagerAddress",
      "Address": "http://localhost/DSW.WebAPI/api/wfm/",
      "Timeout": "00:01:00",
      "NetworkCredential": {
        "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.Credential, VecompSoftware.DocSuiteWeb.Model",
        "Username": "VECOMPSOFTWARE\\utente_non_admin_della_macchina",
        "Password": "Passw0rd"
      }
    },
    {
      "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.BaseAddress, VecompSoftware.DocSuiteWeb.Model",
      "AddressName": "API-EntityAddress",
      "Address": "http://localhost/DSW.WebAPI/api/entity/",
      "Timeout": "00:01:00",
      "NetworkCredential": {
        "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.Credential, VecompSoftware.DocSuiteWeb.Model",
        "Username": "VECOMPSOFTWARE\\utente_non_admin_della_macchina",
        "Password": "Passw0rd"
      }
    },
    {
      "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.BaseAddress, VecompSoftware.DocSuiteWeb.Model",
      "AddressName": "API-ServiceBusAddress",
      "Address": "http://localhost/DSW.WebAPI/api/sb/",
      "Timeout": "00:01:00",
      "NetworkCredential": {
        "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.Credential, VecompSoftware.DocSuiteWeb.Model",
        "Username": "VECOMPSOFTWARE\\utente_non_admin_della_macchina",
        "Password": "Passw0rd"
      }
    },
    {
      "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.BaseAddress, VecompSoftware.DocSuiteWeb.Model",
      "AddressName": "API-UDSAddress",
      "Address": "http://localhost/DSW.WebAPI/api/uds/",
      "Timeout": "00:01:00",
      "NetworkCredential": {
        "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.Credential, VecompSoftware.DocSuiteWeb.Model",
        "Username": "VECOMPSOFTWARE\\utente_non_admin_della_macchina",
        "Password": "Passw0rd"
      }
    },
    {
      "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.BaseAddress, VecompSoftware.DocSuiteWeb.Model",
      "AddressName": "ODATA-EntityAddress",
      "Address": "http://localhost/DSW.WebAPI/ODATA/",
      "Timeout": "00:01:00",
      "NetworkCredential": {
        "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.Credential, VecompSoftware.DocSuiteWeb.Model",
        "Username": "VECOMPSOFTWARE\\utente_non_admin_della_macchina",
        "Password": "Passw0rd"
      }
    }
  ]
}


#FL
######################################################################################################