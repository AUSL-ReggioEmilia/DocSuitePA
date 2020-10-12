Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 9.01
	- ServiceBus Listeners 9.01
	- Workflow Integrations 9.01
	- Compilazione libreria dinamica UDS alla 9.01 (soli per i clienti che hanno già adottato il modulo UDS)
	- Eseguire la nuova versione dell'UDSMigrations 9.01

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
Sono state apportate particolari modifiche al WebConfig che necessariamente devono essere riportate in tutti i clienti.
Per tutti i clienti che hanno il WebConfig standard basta ricopiarlo prendendolo dal pacchetto di rilascio, diversamente 
per AUSL-RE (o altri eventuali clienti che hanno delle personalizzazioni) è necessario procedere nel seguente modo:

	1. Nella sezione <httpModules> aggiungere il seguente modulo.
			<add name="AuthenticationModule" type="VecompSoftware.DocSuiteWeb.Gui.AuthenticationModule" />

	2. Nella sezione <modules runAllManagedModulesForAllRequests="true"> aggiungere i seguenti blocchi
			<remove name="AuthenticationModule" />
			... (attenzione che i blocchi remove devono essere tutti consecutivi prima dei blocchi add)
			<add name="AuthenticationModule" type="VecompSoftware.DocSuiteWeb.Gui.AuthenticationModule" preCondition="integratedMode" />

	3. Aggiornare il runtime -> assemblyBinding


#FL
######################################################################################################
Per alcuni moduli del servizio VecompSoftware.BPM.Integrations è necessario aggiungere nel json "module.configuration.json"
una nuova chiave di configurazione relativa all'identificativo del TenantAOOId (prendere il valore del TenantAOO relativo al TenantId
già specificato nel json).

es.
"TenantAOOId": "ff2efa82-4ea3-471e-af22-f784bbf09c25"

Per la corretta configurazione si prega di utilizzare come master i file di configurazione di ogni singolo modulo nella directory Install -> 9.01 -> default_configurations
del servizio VecompSoftware.BPM.Integrations.

#AC
######################################################################################################
Per il corretto funzionamento della fatturazione elettronica è necessario installare i seguenti moduli del servizio VecompSoftware.BPM.Integrations:

	- "VSW.DocumentUnitLink"
	- "VSW.ReceivableInvoice"
	- "VSW.PayableInvoiceFileSystem"

Se un cliente ha una gestione personalizzata del ciclo di fatturazione attiva è necessario sostituire il modulo "VSW.PayableInvoiceFileSystem"
con il relativo modulo custom del cliente.
Es. Per il cliente ***REMOVED*** è necessario installare il modulo "***REMOVED***.ERP.Invoice".

Per la corretta configurazione si prega di utilizzare come master i file di configurazione di ogni singolo modulo nella directory Install -> 9.01 -> default_configurations
del servizio VecompSoftware.BPM.Integrations.

#AC
######################################################################################################
E' stato creato un nuovo parametro "InvoiceDashboardFilterByTenantEnabled" che definisce se il caricamento degli archivi nelle viste di fatturazione elettronica 
debba tener conto della AOO corrente dell'utente.
Abilitare tale parametro per CTT e per tutti quei clienti con MultiTenantEnabled = true e gestione separata degli archivi di fatturazione.

#AC
######################################################################################################

TODO: In DocSuite 9.02 version we need to update DossierFolderRoles when we update ProcessRoles.

######################################################################################################

TODO: In DocSuite 9.02 version we need change the process filtering in "Gestione serie e volumi" page.

######################################################################################################
E' stato creato un nuovo parametro ForceCollaborationSignDateEnabled 

Forza il settaggio del campo 'signdate' anche per le firme semplici (visto a procedere)
e memorizza lo stato delle firme come attributo (json model) in tutti i documento della collaborazione

Per AGENAS = True
#MF
######################################################################################################
Per il cliente AGENAS è necessario lanciare il tool DSWBiblosDSMigrator che aggiunge l'attributo 
'SignModels' a tutti gli archivi biblos indicati nelle location del cliente specifico.
Prima di lanciare il tool configurare:
		1. il file docuite.connectionstrings.config con le connectionstring del cliente
		2. il file DSWBiblosDSMigrator.exe.config, nella parte <client> con l'indirizzo del client di Biblos

#MF
######################################################################################################