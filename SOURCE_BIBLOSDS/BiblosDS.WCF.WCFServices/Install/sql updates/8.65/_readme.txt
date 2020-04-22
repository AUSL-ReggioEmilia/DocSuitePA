
Nella versione 8.65 è stata implementata la ricerca full-text per gli storage di tipo file system che hanno abilitata questa proprietà.
Per i clienti ai quali è necessario abilitare la ricerca full text (***REMOVED***) seguire i seguenti step:

	1. Creare nuovo database con il nome: [NomeDBBiblos]+_FullText, quindi ad esempio se il db di biblos si chiama BiblosDS2010, il nuovo db si chiamerà BiblosDS2010_FullText
	2. Abilitare il FILESTREAM e la gestione FileTable sul nuovo database (si veda il PARAGRAFO 1)
	3. Installare gli ifilter mancanti (si veda PARAGRAFO 2)
    4. Lanciare lo script db_migrations.sql sul db di Biblos

#SDC
##########################################

PARAGRAFO 1
ATTENZIONE! Da abilitare solo in SQL Server 2012 e successivi.

Per la ricerca full-text è necessario abilitare il FILESTREAM sul server SQL e abilitare la gestione FileTable sul nuovo db creato precedentemente.
Per abilitare il FILESTREAM sul server SQL seguire le istruzioni sotto (rif. https://docs.microsoft.com/it-it/sql/relational-databases/blob/enable-and-configure-filestream):

	1) Fare clic sul pulsante Start , scegliere Tutti i programmi, SQL Server 2017, Strumenti di configurazione e quindi Gestione configurazione SQL Server.
	2) Nell'elenco dei servizi fare clic con il pulsante destro del mouse su Servizi di SQL Server e quindi scegliere Apri.
	3) Nello snap-in Gestione configurazione SQL Server trovare l'istanza di SQL Server in cui si vuole abilitare FILESTREAM.
	4) Fare clic con il pulsante destro sull'istanza e quindi scegliere Proprietà.
	5) Nella finestra di dialogo delle proprietà di SQL Server fare clic sulla scheda FILESTREAM.
	6) Selezionare la casella di controllo Abilita FILESTREAM per l'accesso Transact-SQL.
	7) Fare clic su Abilita FILESTREAM per l'accesso tramite il flusso di I/O dei file. Immettere il nome della condivisione di Windows nella casella Nome condivisione di Windows.
	8) Se ai dati FILESTREAM archiviati in tale condivisione devono accedere client remoti, selezionare Consenti ai client remoti l'accesso tramite flusso ai dati FILESTREAM.
	9) Fare clic su Applica.
	10) In SQL Server Management Studio fare clic su Nuova query per visualizzare l'editor di query.
	11) Nell'editor di query immettere il codice Transact-SQL seguente:
		EXEC sp_configure filestream_access_level, 2  
		RECONFIGURE
	12) Fare clic su Esegui.
	13) Riavviare il servizio SQL Server.


Si consiglia di accedere all'istanza SQL con l'utenza di sysadmin; la seconda query potrebbe avere un tempo di esecuzione molto lungo.
Per abilitare la gestione FileTable sul DB di BiblosDS2010 seguire le istruzioni sotto (rif. https://docs.microsoft.com/it-it/sql/relational-databases/blob/enable-the-prerequisites-for-filetable):
	1) Nell'editor di query immettere il codice Transact-SQL seguente per specificare il livello di accesso non transazionale:
		ALTER DATABASE BiblosDS2010 SET FILESTREAM (NON_TRANSACTED_ACCESS = FULL)
	2) Nell'editor di query immettere il codice Transact-SQL seguente per specificare la directory a livello di DataBase:
		ALTER DATABASE BiblosDS2010 SET FILESTREAM (DIRECTORY_NAME = 'BiblosDS2010')

Per ulteriori approfondimenti sulle FileTable di SQL Server vedi https://docs.microsoft.com/it-it/sql/relational-databases/blob/filetables-sql-server

#AC
##########################################

PARAGRAFO 2
Per utilizzare la full-text index e permette di fare ricerche su diversi tipi di documenti, bisogna controllare le estensioni sulle quali è permessa la ricerca.
Per visualizzare l'elenco dei filtri già presenti sull'istanza sql, lanciare la seguente query

								EXEC sp_help_fulltext_system_components 'filter';

Nell'elenco dei risultati dovrebbero essere già presenti i filtri per i formati Microsoft (.doc, .docx ecc), ma va contrallato il fullpath, che dev'essere simile a 
C:\Program Files\Common Files\Microsoft Shared\Filters\offfiltx.dll. Se fosse diverso (ad esempio C:\Windows\system32\query.dll), vanno eseguiti i seguenti punti
	1) scaricare e installare l'ifilter pack di Microsoft (si veda il link https://support.microsoft.com/it-it/help/945934/how-to-register-microsoft-filter-pack-ifilters-with-sql-server);
	2) lanciare poi la query
								sp_fulltext_service 'load_os_resources', 1
	3) riavviare SQL Server o lanciare

								sp_fulltext_service 'Update_languages'  
								go  
								sp_fulltext_service 'Restart_all_fdhosts'  
								go 

	4) controllare di nuovo i filtri presenti con la query ->  EXEC sp_help_fulltext_system_components 'filter';

Per installare il filtro riguardante i pdf, seguire la procedura (maggiori info al link https://www.adobe.com/devnet-docs/acrobatetk/tools/AdminGuide/Acrobat_Reader_IFilter_configuration.pdf pagine 3/12):
	1) scaricare e installare l'ifilter pack al link http://supportdownloads.adobe.com/thankyou.jsp?ftpID=5542&fileID=5550
	2) aprire il control panel -> system and security -> system -> advanced system settings -> environment variable, nelle System Variables modificare la PATH
	   aggiungendo il percorso nel quale è presente l'ifilter, che dovrebbe essere C:\Program Files\Adobe\Adobe PDF iFilter 11 for 64-bit platforms\bin\
	4) lanciare sull'istanza sql
								Exec sp_fulltext_service 'load_os_resources', 1
								Exec sp_fulltext_service 'verify_signature', 0
    5) riavviare SQL Server
	6) controllare che sia presente il filtro per i pdf tramite -> EXEC sp_help_fulltext_system_components 'filter';

#SDC
##########################################




				
