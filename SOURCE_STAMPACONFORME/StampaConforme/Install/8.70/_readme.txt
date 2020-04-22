La soluzione StampaConforme è stata aggiornata alla versione 4.6.1 del .NET Framework.
Assicurarsi che sul server sia installata la versione indicata del Framework.

#AC
################################################
Per un corretto aggiornamento si consiglia di pulire il contenuto della directory bin del servizio
e copiare il nuovo contenuto del pacchetto di installazione.

#AC
################################################
Per un corretto aggiornamento si consiglia di utilizzare il file web.config allegato al pacchetto
di installazione e successivamente modificare i valori di appSettings con quelli precedenti
all'installazione.

#AC
################################################
E' obbligatorio che nella root del servizio sia presente il file EnterpriseLibrary.Logging.config
che trovate nel pacchetto di installazione.

#AC
################################################
Assicurarsi che l'application pool di StampaConforme abbia impostato Enabled 32 bit = true.

#AC
################################################
ATTENZIONE! Solo per i clienti che hanno abilitata la funzionalità di apposizione glifo per securizzazione dei
documenti.
Devono essere presenti i seguenti 2 parametri in appSettings del file Web.Config:
 - IdSecureDocumentService = Indica l'id del servizio fornito da Land utilizzato per la securizzazione.
 - SecureDocumentServiceCertificateThumbprint = Indica il thumbprint del certificato da installare sul server in Personal\LocalMachine.

Deve essere impostata una LogIn Amministrativa all'application pool di StampaConforme per la creazione
di un canale SSL verso i servizi Land.

Per tutti i clienti che non abiliteranno tale funzionalità NON devono essere specificati i parametri sopra.

#AC
################################################