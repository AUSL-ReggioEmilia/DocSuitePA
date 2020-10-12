ATTENZIONE LA BUILD RICHIEDE IL FRAMEWORK .NET 4.8. 
PRIMA DI PROCEDERE ALL'INSTALLAZIONE E' NECESSARIO CANCELLARE IL CONTENUTO DELLA CARTELLA BIN

#FL
######################################################################################################
Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 9.02
	- ServiceBus Listeners 9.02
	- Workflow Integrations 9.02
		- PER TUTTI I CLIENTI E' NECESSARIO ESSERE CERTI CHE I SEGUENTI MODULI SIANO INSTALLATI E CONFIGURATI
			-> VSW.DocumentUnitLink
			-> VSW.FascicleClose
			-> VSW.FascicleDocumentUnit
			-> VSW.PeriodicFascicle
	- Compilazione libreria dinamica UDS alla 9.02 (soli per i clienti che hanno già adottato il modulo UDS)

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

TODO: remove this projects from ServiceBusReceiver [9.02]:
	- VecompSoftware.ServiceBus.Module.Workflow.Listener.SecureDocument
	- VecompSoftware.ServiceBus.Module.Workflow.Listener.SecurePaper

######################################################################################################

E' stato scoperto un issue di sicurezza della Docsuite legato alla gestione dell'upload document.
E' richiesto di procedere alla seguente configurazione, leggendo i passaggi spiegati alla seguente pagina internet.
https://docs.telerik.com/devtools/aspnet-ajax/controls/asyncupload/security?_ga=2.167984999.791541109.1583850146-1090178406.1566990535#allowedcustommetadatatypes 
  
In sintesi è richiesta l'aggiunta in docsuite.appsettings.config dei parametri, generando su ogni server le corrette chiavi "<generare la chiave>",
come spiegato nell'immagine https://docs.telerik.com/devtools/aspnet-ajax/general-information/images/generate-keys-iis.png.

  <add key="Telerik.AsyncUpload.ConfigurationEncryptionKey" value="<generare la chiave>"/>
  <add key="Telerik.Upload.ConfigurationHashKey" value="<generare la chiave>"/>
  <add key="Telerik.Upload.AllowedCustomMetaDataTypes" value="Telerik.Web.UI.AsyncUploadConfiguration" />

######################################################################################################

Per ottimizzare i processi di migrazioni e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - docsuite.appsettings.config
 - docsuite.connectionstrings.config
 - docsuite.sessionstate.config
 - docsuite.system.servicemodel.behaviors.config
 - docsuite.system.servicemodel.bindings.config
 - docsuite.system.servicemodel.client.config

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#FL
#####################################################################################

Introdotto nuovo parametro di ProtocolEnv 'FulltextEnabled', abilita la una nuova sezione di ricerca Full Text e
visualizza il tasto di accesso alla sezione (tasto a sinistra del help in alto a destra)
[SM = True]
Se FulltextEnabled = True mostra una nuova sezione "di ricerca".
Se FulltextEnabled = False la nuova sezione di ricerca è visibile


ATTENZIONE! L'abilitazione del parametro FulltextEnabled comporta la seguente configurazione:
	- Installazione delle WebAPI di BiblosDS SearchableWebAPI (vedi readme specifico per l'installazione readme 8.65).
	- Configurazione nel TenantModel di una nuova proprietà BiblosWebAPIUrl specificante l'indirizzo
	  relativo alle WebAPI di cui al punto precedente.
	  Es. http://localhost/Biblos.SearchableWebAPI/ODATA/Documents

######################################################################################################

E' stato creato il nuovo modulo "Raccolta dei procedimenti" da attivare per San Marino e SOLO su richiesta per tutti i cliento che hanno FascileEnabled = true.

Per attivare correttamente il modulo e' necessario creare un nuovo contenitore di tipologia Dossier e un settore "speciale di amministrazione" (specificando come Groups solo DocSuiteAdmin) 
Inoltre è necessario specificare i parametri di ProtocolENV, nel seguento modo:
	- FascicleEnabled = true
	- DossierEnabled = true
	- PraticheEnabled = false
	- ProcessContainer = Specificare il valore int della PK del nuovo IdContainer. 
	- ProcessRole = Specificare il valore int della PK del nuovo IdRole.

#FL
######################################################################################################