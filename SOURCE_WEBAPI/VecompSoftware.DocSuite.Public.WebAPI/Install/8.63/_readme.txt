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

E' stata introdotta l'entità ParameterEnv per permettere alle WebAPI di accedere ai parametri di ParameterEnv di protocollo.
Sono quindi stati tolti da appSettings i seguenti parametri:
		- VecompSoftware.DocSuiteWeb.CollaborationLocationId
		- VecompSoftware.DocSuiteWeb.MessageLocationId.

#SDC
#####################################################################################

Modificato il file EnterpriseLibrary.Validation.config delle validazioni.

#MM
#####################################################################################

Sono stati inseriti i seguenti nuovi parametri nell'appSettings

  <!-- VecompSoftware.DocSuiteWeb.ServiceBus.Subscription.AutoDeleteOnIdle valore in secondi che determina la durata massima della sottoscrizione delle UDS  -->
  <add key="VecompSoftware.DocSuiteWeb.ServiceBus.Subscription.AutoDeleteOnIdle" value="600" />
  <!-- VecompSoftware.DocSuiteWeb.ServiceBus.Subscription.DefaultMessageTimeToLive valore in secondi che determina la durata del messaggio prima che venga eliminato dalla coda delle UDS  -->
  <add key="VecompSoftware.DocSuiteWeb.ServiceBus.Subscription.DefaultMessageTimeToLive" value="600" />
  <!-- VecompSoftware.DocSuiteWeb.ServiceBus.Subscription.LockDuration valore in secondi che determina il timeout prima di generare errore dell'operazione dell'UDS -->
  <add key="VecompSoftware.DocSuiteWeb.ServiceBus.Subscription.LockDuration" value="60" />
  <!-- VecompSoftware.DocSuiteWeb.ServiceBus.Subscription.MaxDeliveryCount valore di tentativi prima di fallire per notificare la risposta dell'azione dell'UDS -->
  <add key="VecompSoftware.DocSuiteWeb.ServiceBus.Subscription.MaxDeliveryCount" value="3" />

#FL
###############################################################
