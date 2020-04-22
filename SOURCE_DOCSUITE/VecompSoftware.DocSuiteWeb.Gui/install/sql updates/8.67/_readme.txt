Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.67
	- ServiceBus Listeners 8.67
	- Compilazione libreria dinamica UDS alla 8.67 (soli per i clienti che hanno già adottato il modulo UDS)
	- Migrazione dei ModuleXML adeguandoli al nuovo Schema delle UDS 8.67 (soli per i clienti che hanno già adottato il modulo UDS - necessario supporto di sviluppo)

#FL
############################################################################################################################################################################################################
La procedura di migrazione del database prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:
	
	1. script DocSuite
		01. ProtocolloDB_migrations.sql
		02. PraticheDB_migrations.sql
		03. AttiDB_migrations.sql

	2. script WebAPI

#FL
######################################################################################################
ATTENZIONE: la funzionalità di Attestazione di conformità legata al parametro 'DematerialisationEnabled' non è ancora completata. 
		    Per il momento quindi il parametro DematerialisationEnabled non va attivato per nessun cliente.

#SDC
######################################################################################################
E' stato aggiunto un nuovo parametro: UDSLocation, che è necessario definire in qunato determina la location default che l'uds designer utilizzerà in fase di creazione automatica del contenitore.

La location può puntare a qualunque archivio biblos in quanto non verrà mai usata per la persistenza dei documenti.

#MM
######################################################################################################
E' stato aggiunto un nuovo parametro in Atti: UncomplianceRevokeResolutionEnabled, che è necessario attivare come rafforzativo per la funzionalità di Reggio Emilia per annullare un atto in Pubblicazione.
Questo non è conforme alla normativa quindi per tutti gli altri clienti deve essere a FALSE.
Un atto in Pubblicazione infatti NON può essere annullato.

#CC
######################################################################################################
Quando si abilita il parametro 'DematerialisationEnabled' è necessario avere installati in produzione anche i seguenti prodotti :
	- WebService DocSuite.Services.PDFGenerator
	- Modulo VecompSoftware.ServiceBus.Module.Workflow.Listener.Dematerialisation del prodotto Service Bus Receivers

#SZ
######################################################################################################
ATTENZIONE: Il parametro 'DematerialisationEnabled' non può essere abilitato se il parametro 'ProtocolAttachLocationEnabled' è uguale a true

#IS
######################################################################################################

