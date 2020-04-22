L'applicazione è stata aggiornata alla versione 4.6.1 del .NET Framework.
Assicurarsi che sul server sia presente la versione indicata del Framework.

#AC
#############################################
Per il corretto funzionamento dell'applicazione è necessario aggiornare le seguenti componenti:

	- BiblosDS WCF 8.70.
	- BiblosDS WCF-Host 8.70.

#AC
#############################################
Per un corretto aggiornamento dell'applicazione si consiglia di eliminare tutto il contenuto della
precedente installazione e installare il nuovo pacchetto.
Modificare successivamente il web.config con le precedenti indicazioni di appsettings e url.

#AC
#############################################
Le funzionalità di upload dei documenti sono state riviste e migliorate. E' necessario verificare
che esista nel sito la directory "App_Data" e dare a tale directory diritti full control
all'utente configurato nell'application pool (in alternativa impostare everyone).

#AC
#############################################
ATTENZIONE! La seguente configurazione è da fare obbligatoriamente per ogni archivio.

Per rendere più dinamica la configurazione del portale, alcune logiche di configurazione sono state spostate dalla sezione appSettings del file web.config,
al singolo archivio abilitato alla conservazione.
E' stata aggiunta una nuova funzionalità di "Configurazione Archivio" (Archivi/Configurazione Archivio) che permette di impostare le seguenti proprietà:

	- Data inizio prima conservazione = Corrisponde alla data minima di ricerca documenti per la conservazione. Il range di risultati viene poi ristretto 
										a quanto impostato nel task di conservazione.
	- Ricalcola incrementale = (Da abilitare per archivi di tipo PEC) Ad ogni esecuzione di un task viene ricalcolato l'attributo impostato come AutoInc dell'archivio.
	- Chiusura automatica conservazione = Se abilitato, chiude automaticamente la conservazione con la data di esecuzione del task di conservazione.
	- Genera task successivo = Se abilitato, alla fine dell'esecuzione di un task di conservazione, viene generato il successivo task di lavoro.
	- Chiusura senza verifica conservazione = Se abilitato, nella vista delle "Conservazioni da Chiudere", la fase di chiusura non eseguirà la verifica della conservazione gestita.
	- Controlla file .tsd = (Di default deve essere abilitato) Se abilitato, nella fase di verifica conservazione, controlla la validità del file IPDA.

#AC
#############################################
Sono stati creati 2 nuovi parametri di appsettings (web.config) per definire gli archivi utilizzati
per il salvataggio dei pacchetti di versamento e rapporti di versamento di un singolo versamento.
I parametri sono i seguenti:
	
	- PDVArchiveName (Nome dell'archivio relativo ai pacchetti di versamento).
    - RDVArchiveName (Nome dell'archivio relativo ai rapporti di versamento).

Configurazione Archivio PDV:
L'archivio relativo ai pacchetti di versamento deve avere le seguenti caratteristiche:
	- NON deve essere un archivio abilitato alla conservazione (IsLegal = false).
	- Deve avere almeno i seguenti attributi:
		> Filename (obbligatorio di tipo string)
		> Signature (non obbligatorio di tipo string)

Configurazione Archivio RDV:
L'archivio relativo ai rapporti di versamento deve avere le seguenti caratteristiche:
	- Deve essere un archivio abilitato alla conservazione (IsLegal = true), in quanto sottoposto anch'esso
	  a conservazione sostitutiva.
	- Deve avere almeno i seguenti attributi:
		> Filename (obbligatorio di tipo string, N.B. impostare Modify Always nel campo Mode).
		> Descrizione (non obbligatorio di tipo string).
		> DataInizio (non obbligatorio di tipo datetime).
		> DataFine (non obbligatorio di tipo datetime).
		> DataGenerazione (obbligatorio di tipo datetime).
		> NumeroDocumenti (obbligatorio di tipo int).

#AC
#############################################