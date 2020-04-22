Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- JeepService 8.83
	- WebAPI 8.83
	- ServiceBus Listeners 8.83
	- Workflow Integrations 8.83 - Per i soli clienti che hanno attivato il servizio
		- Per tutti i clienti che adottano il modulo dei fascicolo "FascicleEnabled" è necessario installare il modulo "VecompSoftware.BPM.Integrations.Modules.VSW.FascicleClose"
	- Compilazione libreria dinamica UDS alla 8.83 (soli per i clienti che hanno già adottato il modulo UDS)
	- Eseguire la nuova versione dell'UDSMigrations 8.82

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