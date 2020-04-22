Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.80
	- Stampa Conforme 8.80
	- ServiceBus Listeners 8.80
	- Workflow Integrations 8.80 - Per i soli clienti che hanno attivato il servizio
	- Compilazione libreria dinamica UDS alla 8.80 (soli per i clienti che hanno già adottato il modulo UDS)
	- DocumentUnitMigrator 8.80

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
#############################################################################

E' stata inserita la funzionalità Location dei Workflow che è necessaria definirla per tutti i clienti che hanno attivo il modulo Workflow.
I clienti sono:
	- AUSL-RE
	- AUSL-PC
	- ***REMOVED***/TecnoSky/DFlight
	- TEAFLEX
	- CTT/CAP/LiNea
	- AFOL

E' necessario dunque creare una nuova location che punti ad un archivio standard (come fatto per le collaborazioni) in BiblosDS in cui salvare questi files.
L'archivio BiblosDS deve avere i seguenti attributi (a livello documento - undefined) :
	- Filename required di tipo string [modify always]
	- Signature optional di tipo string [modify always]

Successivamente configurare correttamente il parametro di protocollo WorkflowLocation
#FL
#############################################################################

Sono state apportate molte modifiche alle code e alle topic del service bus localizzate nel file SB01.Docsuitenamespace_Entities.xml.

Considerando la criticità del modello di deploy mediante tool Service Bus Explorer, è neccessario delegare l'attività a Fabrizio Lazzarotto. 
Le attività possono essere svolte anche dopo il deploy a servizi attivi.

#FL
######################################################################################################

In fase di rilascio occorre lanciare il DocumentUnitMigrator 8.80 in modalita' 7 per allineare complementamente le UD.
Successivamente lanciare il DocumentUnitMigrator in modalità 9, per allineare le strutture del modulo fascicoli. 
Se tale attività non viene eseguita il modulo fascicoli non funzionerà correttamente.

#FL
######################################################################################################