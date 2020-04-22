Per un corretto aggiornamento pulire il contenuto della directory del servizio prima dell'installazione.
Successivamente copiare il nuovo contenuto del pacchetto di installazione.

#AC
################################################
Per una migliore gestione della fase di aggiornamento, il file di configurazione è stato esploso in più file,
ognuno responsabile di logiche di configurazione che possono essere customizate per cliente.
Nella directory default_configurations sono presenti i file di configurazione necessari al corretto funzionamento del servizio.
Ogni file di configurazione deve essere modificato con le impostazione attuali del servizio recuperate dal file di configurazione.

 - openofficeconverter.appsettings.config (Lista dei parametri di appSettings).
 - openofficeconverter.system.servicemodel.behaviours.config (Lista dei behaviours di configurazione).
 - openofficeconverter.system.servicemodel.bindings.config (Lista dei bindings di configurazione).
 - openofficeconverter.system.servicemodel.client.config (Impostare l'indirizzo al WebService di Stampa Conforme).
 - openofficeconverter.system.servicemodel.services.config (Lista dei bindings di configurazione).

#AC
################################################
E' obbligatorio sovrascrivere il file BiblosDS.WindowsService.WCFStampaConformeConverterHost.exe.config con quello fornito nel setup
di installazione.

#AC
################################################
Per migliorare le performance di comunicazione tra il WS StampaConforme e i servizi converter Windows di Office
e OpenOffice si consiglia di configurare la comunicazione tramite NetTcpBinding.
I file di configurazione presenti nella directory default_configurations sono già impostati per tale configurazione
e non richiedono ulteriori attività.

#AC
################################################