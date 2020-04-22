Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.87
	- ServiceBus Listeners 8.87
	- Workflow Integrations 8.87
	- Compilazione libreria dinamica UDS alla 8.87 (soli per i clienti che hanno già adottato il modulo UDS)
	- Eseguire la nuova versione dell'UDSMigrations 8.87

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

E' stato modificato il file DocSuiteMenuConfig.json che contiente la configurazione del menù di DocSuite.
A tale file è stata aggiunta una nuova voce "Raccolta di procedimento" nella voce 'Menu11' -> "FirstNode28".

#FL
######################################################################################################

Sono state apportate molte modifiche alle code e alle topic del service bus localizzate nel file SB01.Docsuitenamespace_Entities.xml.

Aggiornare la definizione con la versione “GED Edition” del ServiceBusExplorer.

#FL
######################################################################################################

ISSUE: in caso di archivio con campo HTMLEnable, nell'inserimento di un documento il campo potrebbe perdere il valore.

#MF
######################################################################################################

Sono state apportate modifiche al motore di Workflow che impattano sui clienti che hanno attivato workflow che prevedono l'uso si attività automatice, come la fatturazione elettronica o SPID.
E' dunque necessario aggiornare i WorkflowRepositories dei workflow aggiornandoli il json con quanto previsto nei file sotto riportati.

Vedere dunque:
	- Workflow - Fatturazione elettronica - Fattura pubblica amministrazione attiva
	- Workflow - Fatturazione elettronica - Fattura tra privati attiva
	- Workflow - Fatturazione elettronica - Fattura tra privati passiva
	- Workflow - AUSL-RE - SPID

#FL
######################################################################################################
