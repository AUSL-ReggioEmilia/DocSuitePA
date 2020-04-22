Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.77
	- ServiceBus Listeners 8.77
	- Workflow Integrations 8.77
	- Compilazione libreria dinamica UDS alla 8.77 (soli per i clienti che hanno già adottato il modulo UDS)
	- StampaConforme 8.77
	- JeepService 8.77, tutti i moduli
	- DocumentUnitMigrator 8.77

#FL
######################################################################################################
La procedura di migrazione del database prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:
	
	1. script DocSuite
		01. ProtocolloDB_migrations.sql 
			NB: se durante l'esecuzione dello script ci sono errori di esecuzione dovuti alla presenza di indici e/o contrains sulle colonne
			    idCategory e idSubCategory è necessario rieseguire lo script fintantoché gli indici vengono rimossi.
				Se il problema permane eseguire a mano e fuori transazione l'esecuzione dei singoli script che eliminano gli indici e le contrains 
				della tabella Protocol e AdvancedProtocol.

		02. PraticheDB_migrations.sql
		03. AttiDB_migrations.sql
	
	2. script WebAPI

	3. Lanciare DocumentUnitMigrator in modalità 6 "Migrazione archivi 8.77". (soli per i clienti che hanno già adottato il modulo UDS,
	   vedi commento successivo per ulteriori dettagli).

	4. script DocSuite
		04. ProtocolloDB_postdocumentunitmigrator.sql
	
#FL
######################################################################################################
								GESTIONE MIGRAZIONE UDS
Per questo passaggio è necessario prendere accordi con Fabrizio per l'esecuzione dell'attività che verrà fattua manualmente e non tramite UDSMigrator.
Dopo aver ricevuto il nuovo pacchetto è necessario cancellare la vecchia directory inserendo quella fornita.
Successivamente è necessario lanciare il tool UDSMigrator.

#FL
######################################################################################################
								GESTIONE MIGRAZIONE MODULE_XML UDS
E' necessario procedere a mano alla bonifica del MODULE_XML di ogni archivio presente nella tabella uds.udsrepositories.
I cambiamenti apportati sono:
	- Rimozione del blocco relations mantenendo il contenuto.
	- Aggiungere i TAG mancanti ResultPosition="0" ResultVisibility="false" ai blocchi Contacts e Authorizations 
	- Aggiungere i TAG ConservationEnabled="false" e HideRegistrationIdentifier="false" nel blocco principale di testa.

#FL
######################################################################################################

In fase di rilascio occorre lanciare il DocumentUnitMigrator 8.77 in modalita' 6, per allineare le strutture del modulo degli archivi. 
Se tale attività non viene eseguita il modulo archivi non funzionerà correttamente.

In fase di migrazione occorre specificare quale sara' l'utenza di default in AppSettings "DocumentUnitMigratorUser" 
ES: "vecompsoftware\biblosds"

#FL
######################################################################################################

Sono state apportate molte modifiche alle code e alle topic del service bus localizzate nel file SB01.Docsuitenamespace_Entities.xml.

Considerando la criticità di tali modifiche è neccessario delegare l'attività a Fabrizio Lazzarotto. 
Le attività possono essere svolte anche prima del deploy, ma richiedono il fermo applicativo dei seguenti servizi:
	- ServiceBus Listeners 8.7X
	- Workflow Integrations 8.7X
	- JeepService 8.7X

#FL
######################################################################################################
ATTENZIONE AGGIORNAMENTO JEEPSERVICE: 

Prima dell'aggiornamento, per questioni legati alla modifica degli algoritmi di HASH delle PEC (da MD5 a SHA256), è necessario
modificare il parametro ImapStartDate del PECMailBoxConfiguration inserendo l'esatta data di ultimo avvio del modulo PEC.

Se non si procedere in tal senso, verranno riscaricate tutte le PEC presenti sulle webmail in quanto gli hash risulteranno diversi.
Il consiglio è spegnere il JeepService la sera prima dell'aggiornamento in modo tale da essere certi che tutto il pregresso sia stato scodato e 
inserire un valore più facile da configurare.

Gli script di alter, daranno errori per la presenza degli indici sulle colonne Checksum e HeaderChecksum. 

ALTER TABLE [dbo].[PECMail] ALTER COLUMN [Checksum] NCHAR(64) NULL
GO
ALTER TABLE [dbo].[PECMail] ALTER COLUMN [HeaderChecksum] NCHAR(64) null

Procedere manualmente al DROP degli indici, per poi ricrearli dopo averli fatto girare.

#FL
######################################################################################################Nota per Sviluppo:
è stato aggiornato Typescript alla versione 3.0.1. E' necessario quindi installare l'SDK di Typescript alla versione specificata.
E' possibile scaricare il pacchetto di installazione al seguente link: https://www.microsoft.com/en-us/download/details.aspx?id=55258.
Seguire le istruzioni di installazione fornite al link precedente per il corretto funzionamento.

#AC
######################################################################################################
Il servizio converter Office si autoadatta alla versione di Office installata sul server, quindi questa modalità supera il limite
di dover utilizzare esclusivamente Office 2010.
In fase di sviluppo sono stati eseguiti test che garantiscono la compatibilità con Office 2016.
La versione attuale dei pacchetti (converter e WebService) richiedono che sia installato .NET 4.6.1.
Vedere i readme specifici dei converter e del webservice per le specifiche di configurazione.

#AC
######################################################################################################