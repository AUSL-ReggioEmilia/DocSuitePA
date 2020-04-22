Deve essere lanciato lo script SQL sul DB BiblosDS2010

 - db_migrations.sql

#AC
###################################################################
Il windows service BiblosDS WCFHost implementa un nuovo servizio per la cancellazione dagli storage dei documenti in detach mode.
Tale servizio rimuove fisicamente i documenti detach dagli storage e imposta lo Status del documento = 6 (Removed from Storage).
Sono stati aggiunti quindi dei nuovi parametri di configurazione in AppSettings per abilitare le funzionalità descritte sopra:

- EnableCleanDocumentsTimer = true/false se abilitare o meno il servizio.
- CleanDocumentsTimerWaitMinute = valore intero che definisce la tempistica in minuti di esecuzione del timer.
- CleanDocumentsFromDate = dd/MM/yyyy data inferiore limite per la ricerca dei documenti (se non impostato non saranno impostati limiti temporali in ricerca).
- CleanDocumentsToDate = dd/MM/yyyy data superiore limite per la ricerca dei documenti (se non impostato non saranno impostati limiti temporali in ricerca).

Assicurarsi di creare il file di configurazione JSON nella directory "Config" del servizio con nome "ArchiveRestrictions.json" che permette di
definire la lista degli IdArchive utilizzabili dal servizio in fase di processo.
Se la lista è vuota il servizio processa tutti gli archivi presenti in BiblosDS.
Se impostati gli idArchive la ricerca verrà ristretta per i soli archivi impostati nel file JSON.
Es. di configurazione:

[
	"B78FCF6E-43F4-4DF5-B7F8-115C53BD17E1"
]

N.B.
Nel caso di installazione del windows service BiblosDS WCFHost su più server è obbligatorio abilitare il servizio di cancellazione documenti detached
solo su un'unica istanza per evitare concorrenza di gestione dei documenti da eliminare.

#AC
##################################################################
Per ottimizzare i processi di migrazione e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - Web.config

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#AC
#####################################################################################