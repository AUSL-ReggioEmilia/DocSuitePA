La DocSuite 9.00 è una major release e i requisiti minimi sono:
	- Framework .NET da 4.8
	- SQL Server 2012
	- Windows Server 2008 R2 SP1

#FL
######################################################################################################
Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 9.00
	- ServiceBus Listeners 9.00
	- Workflow Integrations 9.00
	- Compilazione libreria dinamica UDS alla 9.00 (soli per i clienti che hanno già adottato il modulo UDS)

#FL
######################################################################################################

La procedura di migrazione del database prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:
	
	1. script DocSuite
		01. ProtocolloDB_migrations.sql
		02. PraticheDB_migrations.sql
		03. AttiDB_migrations.sql
	
	2. script WebAPI

	3. script 04. ProtocolloDB_post_migrations.sql
	
#FL
######################################################################################################
ATTENZIONE! Lo script di migrazione 04. ProtocolloDB_post_migrations.sql non è compatibile con CTT e CAP.

#AC
######################################################################################################
ATTENZIONE! Se lo script ProtocolloDB_migrations.sql ritorna errore nella creazione della nuova primary key
per la tabella ProtocolLog, è necessario commentare tale istruzione e procedere ad eseguire la 
creazione della primary key successivamente all'esecuzione dello script.

#AC
######################################################################################################
ATTENZIONE! La release corrente non è compatibile con la gestione della fatturazione elettronica.

#AC
######################################################################################################
ATTENZIONE! Le API legacy non sono funzionanti, quindi tutti gli applicativi che le utilizzano (FastProtocolImport, FastProtocolSender...)
non sono utilizzabili nella release corrente.

#AC
######################################################################################################
ATTENZIONE! Per utilizzare le API WSProt è necessario aggiungere una nuova chiave di appSettings di nome TenantAOOId,
con il valore del TenantAOOId recuperato dalla tabella Tenants del DB di Protocollo.

Es.
<add key="TenantAOOId" value="ff2efa82-4ea3-471e-af22-f784bbf09c25" />

#AC
######################################################################################################
Il modulo del JeepService DSWProtocolJournal è stato modificato per gestire la creazione dei registri di Protocollo
per singola AOO.
E' necessario aggiungere un nuovo attributo per il relativo archivio di Biblos con nome AOO di tipo string e
IsRequired = true.

#AC
######################################################################################################
Per attivare correttamente tutte le funzionalità MultiAOO della DocSuite è necessario configurare i seguenti parametri di protocollo:

	- MultiTenantEnabled = true
	- PraticheEnabled = false
	- FascicleEnabled = true
		- FascicleContanctId = Associare il ramo blu di rubrica per i "Responsabili di procedimento" /"Responsabili di funzione"
		- FascicleMiscellaneaLocation = creare la location come specificato nel readme 8.64 
					- Attributo Filename -> Modify always
					- Attributo Signature -> Modify always

	- DossierEnabled = true
		- DossierMiscellaneaLocation =  = creare la location come specificato nel readme 8.64 
					- Attributo Filename -> Modify always
					- Attributo Signature -> Modify always

	- WorkflowManagerEnabled= true
		- WorkflowLocation =  = creare la location come specificato nel readme 8.64 
					- Attributo Filename -> Modify always
					- Attributo Signature -> Modify always

	- PECEnabled= false
	- TNotice = true
	- ProcessEnabled = true (vedere i dettagli )
		- ProcessContainer = "Dossier speciale delle serie archivistiche" 
			E' necessario creare un nuovo contenitore di tipologia Dossier per definire la gestione dei repertori di fascicoli.
			Per tutti i clienti che hanno FascicleEnabled è necessario attivare DossierEnabled
			NON CONCEDERE ASSOCIARE MAI LA GESTIONE A UTENTI NORMALI.

		- ProcessRoles = "settore speciale delle serie archivistiche"
			E' necessario creare un nuovo settore che servirà come "Settore di default" associato ai Dossier autogenerati dalla gestione dei repertori di fascicoli.
			Dentro tale settore è necessario essere certi che ci siano solo utenti "amministratori" della docsuite o PowerUser selezionati dal cliente. 
			NON CONCEDERE ASSOCIARE MAI LA GESTIONE A UTENTI NORMALI.

#FL
######################################################################################################

Per configurare il prodotto Micorosoft Service Bus in modo tale che permette l'invio di file di dimensioni > 20MB è necessario eseguire i seguenti passaggi:

 - Aprire il file C:\Program Files\Service Bus\1.1\Microsoft.ServiceBus.Gateway.exe.config
 - Cercare i l binding col nome netMessagingProtocolHead
 - Cambiare i valori dei maxReceivedMessageSize="157286400" e maxBufferSize="157286400"
 - Riavviare tutti i servizi di windows del service bus services
 
 il risultato dopo la modifica della sezione del config è la seguente:
 
		<binding name="netMessagingProtocolHead" receiveTimeout="24:00:00" listenBacklog="64" maxConnections="64" maxReceivedMessageSize="157286400" maxBufferSize="157286400">
          <readerQuotas maxArrayLength="5242880" maxDepth="64" maxStringContentLength="5242880" maxBytesPerRead="4096" maxNameTableCharCount="16384" />
          <security>
            <transport clientCredentialType="None"></transport>
          </security>
        </binding>
		
#FL
############################################################################

Il Windows Fabric Host è un prodotto Microsoft che viene installato insieme al Service Bus, ma genera dei file di trace molto corposi nel tempo.
Per tutti i clienti che hanno attivo il Service Bus è necessario eseguire il sequente comando nel promnt dei comandi su tutti nodi della farm del Service Bus.

Logman update trace FabricLeaseLayerTraces -ow –-cnf

Questo commando sovrascriverebbe il file ogni volta che si raggiungono i 128 Mb

#FL
############################################################################
