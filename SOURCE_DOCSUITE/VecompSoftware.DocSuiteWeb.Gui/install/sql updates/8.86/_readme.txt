Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.86
	- ServiceBus Listeners 8.86
	- Workflow Integrations 8.86
	- Compilazione libreria dinamica UDS alla 8.86 (soli per i clienti che hanno già adottato il modulo UDS)
	- PER AUSL-RE 
		-> MyDocSuite 8.86
		-> SPID
		-> Modulo integration AUSLRE.SPID

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

Introdotto nuovo parametro di ProtocolEnv 'ProtocolStatusConfirmRequest' definisce una lista di stringhe 
che contiene valori attribuibili alla colonna ProtocolStatus della Tabella ProtocolStatus,
di default è Lista vuota "[]".

Per il cliente ENPACL :  ["C"]
#MF
######################################################################################################
AUSL-RE 
	Inserire in MetadataRepositories il contenuto del json "Metadata - AUSL-RE - SPID Accesso agli atti"
	
#MF
######################################################################################################

E stata aggiornata la nuova componente WebWTAIN 15.2 che gestisce lo scanner rimuovendo la dipendenza con IE.
Per mantenere una retro-compatibilità è stato introdotto il parametro ScannerLightRestEnabled da attivare per abilitare la nuova componente.
I browser supportati sono:​
	- Chrome and Firefox v27 o superiore (32bit/64bit) su piattaforma Windows
	- Internet Explorer 8 o superiore (32 bit/64 bit), Edge
	- Safari, Chrome, and Firefox on macOS 10.6 o superiore 
	- Chrome e Firefox v27 or superiori (64bit) su piattaforma Ubuntu 10-18, Debian 8-9, o Fedora 19+

#FL
######################################################################################################
