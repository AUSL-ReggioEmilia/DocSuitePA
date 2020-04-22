Per un corretto aggiornamento pulire il contenuto della directory del servizio prima dell'installazione.
Successivamente copiare il nuovo contenuto del pacchetto di installazione.

#AC
################################################
Per una migliore gestione della fase di aggiornamento, il file di configurazione è stato esploso in più file,
ognuno responsabile di logiche di configurazione che possono essere customizate per cliente.
Nella directory default_configurations sono presenti i file di configurazione necessari al corretto funzionamento del servizio.
Ogni file di configurazione deve essere modificato con le impostazione attuali del servizio recuperate dal file di configurazione.

 - officeconverter.appsettings.config (Lista dei parametri di appSettings).
 - officeconverter.system.servicemodel.behaviours.config (Lista dei behaviours di configurazione).
 - officeconverter.system.servicemodel.bindings.config (Lista dei bindings di configurazione).
 - officeconverter.system.servicemodel.client.config (Impostare l'indirizzo al WebService di Stampa Conforme).
 - officeconverter.system.servicemodel.services.config (Lista dei bindings di configurazione).
 - EnterpriseLibrary.Logging.config.

#AC
################################################
E' obbligatorio sovrascrivere il file BiblosDS.WindowsService.StampaConforme.Office.Converter.exe.config con quello fornito nel setup
di installazione.

#AC
################################################