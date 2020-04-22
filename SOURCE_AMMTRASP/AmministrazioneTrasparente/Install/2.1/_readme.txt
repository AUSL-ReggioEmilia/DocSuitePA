1. CREAZIONE PROGETTO ANALYTICS

- Accedere alla console delle Google API e selezionare il menu a tendina dei progetti in alto a sinistra (https://console.developers.google.com/)
- Se non è ancora stato creato cliccare sul pulstante (+) per aggiungere un progetto Analytics
- Inserire il nome e premere "crea"
- Dalla Dashboard assicurarsi di aver selezionato il progetto corrente (menu in alto a sinistra) e premere "abilità API e servizi"
- Ricercare "reporting" e selezionare "Google Analytics Reporting API" 
- Abilitare le API

2. CREAZIONE ACCOUNT DI SERVIZIO + DOWNLOAD TOKEN JWT

- Accedere alla sezione "IAM e amministrazione" della console delle API (https://console.developers.google.com/iam-admin/)
- Nella schermata "Account di servizio" premere "Crea account di servizio" 
- Inserire un nome per l'account e spuntare entrambe le checkbox 
- Assicurarsi che il tipo di chiave generata sia impostato su JSON
- Alla voce "Ruolo -> account di servizio " selezionare "Utente account di servizio" e "Autore token account di servizio"

NB:
	 Dopo aver premuto il pulsante "crea" verrà avviato il download di un file JSON contenente le credenziali dell'utente di servizio, SALVARLO IN UNA POSIZIONE SICURA

3. CONCESSIONE AUTORIZZAZIONI ALL'ACCOUNT DI SERVIZIO

- Dalla console dI Google ANALYTICS accedere alla sezione "Amministrazione" (https://analytics.google.com/analytics/web) 
- Nella sezione "Account" selezionare "Gestione utenti"
- Aggiungere un utente utilizzando il pulsante in alto a destra e selezionare "aggiungi nuovi utenti"
- Inserire l'indirizzo email dell'accunt di servizio (reperibile a https://console.developers.google.com/iam-admin/serviceaccounts/ come "ID account di servizio")
- Concedere le autorizzazioni in "Lettura ed analisi" e premere "Aggiungi"

AGGIUNTA DEL TOKEN JWT ALL'APPLICAZIONE

In fase di sviluppo il token scaricato al punto 2 è stato aggiunto direttamente al progetto come "AnalyticsJWTSecret.json" e nelle sue properties il flag "copy to output directory" è stato impostato su "copy always"

L'applicazione per autenticare l'account di servizio cerca AnalyticsJWTSecret.json nella directory di output del progettto chiamando Server.MapPath("~/App_Data/AnalyticsJWTSecret.json") all'interno del metodo di autenticazione

4. PARAMETRI DI CONFIGURAZIONE

Per gestire l'utilizzo dei report di Analytics sono stati aggiunti due parametri nella configurazione della webapp:
 - AnalyticsStartDate che indica la data di partenza utilizzata come riferimento per la generazione dei report, è indicata con il formato yyyy-MM-dd
 - AnalyticsIDView è l'identificativo della vista utilizzata dall'account di servizio per la visualizzazione dei dati, ulteriori informazioni sulle viste possono essere reperite a https://support.google.com/analytics/answer/2649553?hl=it; l'applicazione utilizza la vista di default creata alla generazione del progetto (ID_VIEW), se fosse necessario creare ed utilizzare vista diversa, la procedura è reperibile qui: https://support.google.com/analytics/answer/1009714?hl=it&ref_topic=6014102
 - Per leggere ID_VIEW accedere al portale amministrazione dell'account analytics e selezionare "impostazioni viste" nell'ultimo menu a destra
 

#MZ
######################################