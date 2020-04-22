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

#FL
######################################################################################################

E' stata introdotta la funzionalità "Risveglio del modulo in fasce orarie prestabilite".
Questa modalità è altamente consigliata per le integrazioni che prevedono il funzionamento sono il determinata fasce orarie o per problematiche sistemistiche.

E' dunque necessario rivedere la sezione TImers del file global.configuration.json in modo tale che tutti i timers abbiano una orario di inizio e di fine.

ES: Funzionamento del modulo dalle ore 08:00 alle ore 18:00
	
	{
      "Name": "PayableInvoiceFileSystemTimer",
      "Period": "00:05:00",
	  "StartHour": "08:00:00",
	  "EndHour": "18:00:00"
    }

#FL
######################################################################################################
