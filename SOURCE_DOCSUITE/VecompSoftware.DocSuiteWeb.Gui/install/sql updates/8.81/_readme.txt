Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- JeepService 8.81
	- WebAPI 8.81
	- ServiceBus Listeners 8.81
	- Workflow Integrations 8.81 - Per i soli clienti che hanno attivato il servizio
		- Per tutti i clienti che adottano il modulo dei fascicolo "FascicleEnabled" è necessario installare il modulo "VecompSoftware.BPM.Integrations.Modules.VSW.FascicleClose"
	- Compilazione libreria dinamica UDS alla 8.81 (soli per i clienti che hanno già adottato il modulo UDS)

#FL
######################################################################################################

La procedura di migrazione del database prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:
	
	1. script DocSuite
		01. ProtocolloDB_migrations.sql
		02. PraticheDB_migrations.sql
		03. AttiDB_migrations.sql
	
	2. script WebAPI

	3. Lanciare il tool "ConvertUDS_8.81" per la bonificare automatica dei file XML degli archivi UDSRepositories.
	   Il tool effettua in autoamtico le seguenti attività:
		- Aggiunge l'attributo LinkButtonEnabled="false"
		- Per ogni tag DATE aggiunge il tag DefaultTodayEnabled="false"
		- Bonifica i valori nel database corrispondendi ai tag EnumField e LookupField trasformandoli in strutture Json.
	
#FL
#########################################################################################

Per configurare correttamente il tool ConvertUDS_8.81 è necessario inserire la connectionstring del database di protocollo e l'url delle WebAPI.

#FL
#########################################################################################
Il file di scripr script "01. ProtocolloDB_migrations.sql" contiene la crazione del nuovo indice [IX_PECMail_UniqueId]
che potrebbe dare errore nel caso in cui sia già presente nel database.
In tale eventualità è sufficiente non eseguire quella sezione dello script commentando la creazione dell'indice.

#FL
#########################################################################################

Aggiungere al file docsuite.appsettings.config la seguente chiave.

<add key="Signer.Infocert.ProxySign" value="" />
<add key="Signer.Aruba.Actalis" value="" />

Per chi attiva la firma remota di aruba è inoltre necessario :

	- aggiungere al file docsuite.system.servicemodel.bindings.config il seguente binding HTTP

		<binding name="BasicHttpBinding_ArubaSignService" messageEncoding="Mtom" maxReceivedMessageSize="2147483647" maxBufferSize="2147483647">
		  <readerQuotas maxDepth="128" maxStringContentLength="2147483647" maxArrayLength="2147483647" />
		  <security mode="Transport" />
		</binding>
	
	- aggiungere al file docsuite.system.servicemodel.client.config il seguente endpoint

		<endpoint address="https://arss-land.actalis.it:443/ArubaSignService/ArubaSignService"
				binding="basicHttpBinding" bindingConfiguration="BasicHttpBinding_ArubaSignService"
				contract="Aruba.ArubaSignService" name="ArubaSignServicePort" />

#FL
#########################################################################################

E' stato aggiunta una nuova proprietà "SecurityContext" nel TenantModel in modo tale da dismettere la chiave "VecompSoftware.DocSuiteWeb.Security.ContextType"
di AppSettings.config nelle Private WebAPI.

Per un corretto allineamento è necessario aggiornare il parametro TenantModel nel database aggiungendo la proprietà "SecurityContext":"Domain" per gli ambienti in Dominio
o "SecurityContext":"Machine" per gli ambienti non in dominio.

Per un esempio confrontare il proprio valore di database rispetto agli esempio forniti:
	- ExampleMultiTenant_8.81.json per chi ha la gestione multitenants
	- ExampleOneTenant_8.81.json per chi ha la gestione singolo tenant

#FL
######################################################################################################

Per tutti i clienti che adottano il modulo dei fascicolo (vedi parameter env di protocollo "FascicleEnabled" =true) è necessario configurare e installare 
nell'integration il seguente modulo:

	- "VecompSoftware.BPM.Integrations.Modules.VSW.FascicleClose"

La configurazione di default (come specifiato nel relativo default_configurations) è la seguente:
	{
      "Name": "VecompSoftware.BPM.Integrations.Modules.VSW.FascicleClose",
      "ServiceWakeType": "Single",
      "Enabled": true
    }

#FL
######################################################################################################