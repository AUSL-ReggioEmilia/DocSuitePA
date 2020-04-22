Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.82
	- ServiceBus Listeners 8.82
	- Workflow Integrations 8.82 - Per i soli clienti che hanno attivato il servizio
		- Per tutti i clienti che adottano il modulo dei fascicolo "FascicleEnabled" è necessario installare il modulo "VecompSoftware.BPM.Integrations.Modules.VSW.FascicleClose"
	- Compilazione libreria dinamica UDS alla 8.82 (soli per i clienti che hanno già adottato il modulo UDS)
	- Eseguire la nuova versione dell'UDSMigrations

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
#########################################################################################

La nuova versione del tool UDSMigrations, sostituise il tool teamporaneo ConvertUDS_8.81 introdotto per la conversione dei ModuleXML dei vari UDSRepositories.
Per questo motivo, rispetto alla versione precedente è necessario inserire la connectionstring del database di protocollo del file di config.
Inoltre è stato riportato anche il file addresse.json per la comunicazione con le WebAPI dunque è necessario specificare l'url delle Private WebAPI.

#FL
#########################################################################################

Per i clienti che vogliono modificare l'ordine delle colonne nella visualizzazione dei risultati di Protocollo
configurare il parametro ProtocolGridOrderColumns utilizzando lo schema JSON (ProtGrid.json) modificando il valore dell'ordinamento 

Per ENPACL utilizzare lo schema definito in ProtGrid_ENPACL.json
#MF
#########################################################################################
E' stata modificata la modalità di autenticazione delle API legacy DocSuite da "Anonimous" a "Windows authentication".
La modifica richiede i seguenti passaggi:
       --- API Legacy ---
       1) Aggiungere un nuovo binding nel file di configurazione "docsuite.system.servicemodel.bindings.config" con le seguenti caratteristiche:
             <binding name="BasicHttpEndpointBinding" maxBufferSize="2147483647" maxReceivedMessageSize="2147483647" closeTimeout="00:30:00" openTimeout="00:30:00" receiveTimeout="00:30:00" sendTimeout="00:30:00">
               <readerQuotas maxDepth="128" maxStringContentLength="2147483647" maxArrayLength="2147483647" />
               <security mode="TransportCredentialOnly">
                    <transport clientCredentialType="Windows" />
               </security>
             </binding>
       2) Modificare il file "docsuite.system.servicemodel.services.config" impostando per ogni service l'attributo bindingConfiguration con il nuovo binding
          creato al punto 1.
          "bindingConfiguration="BasicHttpEndpointBinding":
          es. 
          <service behaviorConfiguration="MetadataServiceBehavior" name="VecompSoftware.DocSuiteWeb.API.MailService">
                    <endpoint address="" binding="basicHttpBinding" bindingConfiguration="BasicHttpEndpointBinding"
                      contract="VecompSoftware.DocSuiteWeb.API.IMailService" />
                    <endpoint address="mex" binding="mexHttpBinding" contract="IMetadataExchange" />
          </service>
       3) Modificare il file "Web.config" abilitando l'autenticazione di Windows all'interno del tag "system.Web":
          <authentication mode="Windows" />
       4) Abilitare in IIS l'autenticazione di Windows per il site delle API e disabilitare l'autenticazione anonima.
       5) Impostare nel pool delle API un utenza amministratore come fatto per il pool di DocSuite.

       --- FPS ---
       1) Aggiungere un nuovo binding nel file di configurazione "docsuite.system.servicemodel.bindings.config" con le seguenti caratteristiche:
             <binding name="BasicHttpEndpointBinding" maxBufferSize="2147483647" maxReceivedMessageSize="2147483647" closeTimeout="00:30:00" openTimeout="00:30:00" receiveTimeout="00:30:00" sendTimeout="00:30:00">
               <readerQuotas maxDepth="128" maxStringContentLength="2147483647" maxArrayLength="2147483647" />
               <security mode="TransportCredentialOnly">
                    <transport clientCredentialType="Windows" />
               </security>
             </binding>
       2) Modificare il file di configurazione "docsuite.system.servicemodel.client" impostando per ogni client l'attributo bindingConfiguration con il nuovo binding
          definito al punto 1.
          "bindingConfiguration="BasicHttpEndpointBinding":
          es.
          <endpoint address="http://localhost/API/FastMergeService.svc"
        binding="basicHttpBinding" bindingConfiguration="BasicHttpEndpointBinding"
        contract="FastMergeService.IFastMergeService" name="VecompSoftware.DocSuiteWeb.API.FastMergeService" />

Per ogni client, come il FastProtocolInsert deve essere replicata la configurazione di esempio per il FPS.
E' possibile verificare le configurazioni da modificare nei file di configurazione presenti nel pacchetto di installazione.

P.S. Se la modalità "Windows" dovesse dare errore, impostare il valore "Ntlm" relativamente all'authentication mode.

#AC
#########################################################################################
