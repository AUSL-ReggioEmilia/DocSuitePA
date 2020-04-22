Verificare nella tabella WorkflowRepository che i record dei Workflow già inseriti presentino solamente
il valore Xaml o Json prima dell'esecuzione dello script di Protocollo.

#AC
#################################################################################################
In fase di installazione in Stage eseguire gli script nel modo seguente:
	- ProtocolloDB_migrations SUL DB DI PROTOCOLLO
	- AttiDB_migrations SUL DB DI ATTI
	- PraticheDB_migrations SUL DB DI PRATICHE

In fase di installazione in Produzione eseguire gli script nel seguente ordine:
	- ProtocolloDB_migrations SUL DB DI PROTOCOLLO
	- ProtocolloDB_bonifica SUL DB DI PROTOCOLLO
	- AttiDB_migrations SUL DB DI ATTI
	- PraticheDB_migrations SUL DB DI PRATICHE
 
#SZ
#################################################################################################
La release 8.58 di DocSuite necessita, per il corretto funzionamento, di WebAPI 8.58.

#SZ
#################################################################################################
Aggiornare i moduli DSWMessages del JeepService 8.58.

#SZ
###################################################################################################
[ENPACL]
E' stato introdotto un nuovo parametro per ENPACL "CostiPosteWebAccountEnabled" il quale permette di 
poter visualizzare la voce di menu "Costi"degli Account di PosteWeb all'interno della quale a differenza di 
quanto accade per gli altri clienti, per ENPACL il costo totale è associato al tipo di Posta e all'Account
e non al Settore.
[CostiPosteWebAccountEnabled = true] permette la visibilità di tale pagina Costi relativa agli Account
[CostiPosteWebAccountEnabled = false ] non abilita la visibilità di tale voce di menu ma rimane con la
logica tradizionale in cui la pagina Costi si riferisce ai costi totali relativi ai settori

#GN
###################################################################################################
[AUSL-RE]
In modifica di Delibera soggetta a controllo, il pannello che riguarda le informazioni sull'Organo di Controllo deve essere visibile solo se l'atto è esecutivo.
Per questo motivo la configurazione deve essere la seguente:
		1. TabMaster: la proprietà OCData deve essere presente in ManagedData per W08 e W09
		2. TabWorkflow: la proprietà OCData deve essere presente in ChangeableData solo per lo step di Esecutività di W09.

#SDC
###################################################################################################

Il parametro PublishToOnlineRegisterEnabled dovrà essere attivato solo quando sarà completato il nuovo Albo online. 
Per il momento non va attivato.

#SDC
###################################################################################################
[ASL-TO]
Nel file riguardante il menu specifico per il cliente indicato, devono essere presenti nel Menu 9 (Delibere e Determine) le voci
			     "FirstNode6": { "Name": "Ricerca Flusso" },
				 "FirstNode7": { "Name": "Firma ultima pagina" },
				 "FirstNode8": { "Name": "Statistiche" },
per visualizzare nel menu la pagina di Ricerca Flusso e di Firma ultima pagina.
Per la visualizzazione di 'Ricerca Flusso' va attivato il parametro 'ShowMassiveResolutionSearchPageEnabled'.
Per la pagina 'Firma Ultima Pagina'
			1. Attivare il parametro 'ShowMassiveResolutionSearchPageEnabled'
			2. Attivare il parametro 'DigitalLastPage'
			3. Indicare nel parametro 'DigitalLastPageGroup' il gruppo di utenti che può visualizzare la pagina.
Per la pagina 'Statistiche' va attivato il parametro 'ShowResolutionStatisticsEnabled'.

#SDC
###################################################################################################

Per togliere l'obbligatorietà di firma del frontalino di pubblicazione, disabilitare il parametro CheckPublishDocumentSigned.

#SDC
###################################################################################################
