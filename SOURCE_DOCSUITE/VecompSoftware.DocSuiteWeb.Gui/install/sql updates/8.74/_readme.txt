Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.74
	- ServiceBus Listeners 8.74
	- Workflow Integrations 8.74
	- DocumentUnitMigrator 8.74
	- BiblosDS 8.73
	- JeepService 8.74
		- Modulo PEC
		- Modulo DocSeriesImporter
	- Compilazione libreria dinamica UDS alla 8.74 (soli per i clienti che hanno già adottato il modulo UDS)
	- Tool di migrazione dei fascicoli DSWFascicleMigrator
	- Tool di migrazione dei log DSWHashGeneratorMigrator

#FL
######################################################################################################

Gli script SQL vanno lanciati nell'ordine seguente:

	01. ProtocolloDB_migrations.sql
	02. PraticgheDB_migrations.sql
	03. AttiDB_migrations.sql
	04. ProtocolloDB_production.sql (da lanciare solo in ambiente di produzione)

NB: prima di lanciare gli script accertarsi che nella tabella PrivacyLevels nella colonna Description non ci siano valori NULL.
	In caso contrario contattare il cliente per accordarsi con il cliente per attribuire la giusta descrizione del livello privacy 

#SDC
######################################################################################################
[AUSL-RE]
E' stato modificato il parametro "FasciclesPanelVisibilities" che governa la visibilità dei pannelli degli inserti e dei fascicoli collegati nel Sommario dei Fascicoli.
La struttura del parametro in configurazione di Default è la seguente:

{	
	"FasciclesLinkedPanelVisibility": true,
	"GridSearchPanelVisibility": true
}

- Per [AUSL-RE] il parametro va configurato impostando i valori nel seguente modo:

{
	"FasciclesLinkedPanelVisibility": false,
	"GridSearchPanelVisibility": false
}

Per impostare il parametro per [AUSL-RE] copiare la precedente configurazione e incollarla nella casella di testo del parametro "FasciclesPanelVisibilities".

#SZ
######################################################################################################
Nel DocsuiteMenuConfig.json modificare la voce 'Livelli privacy' in 'Livelli riservatezza' nella sezione Menu 11(Tabelle).

#SDC
######################################################################################################
[AUSL-RE]
E' stato introdotto il parametro "SearchOnlyAuthorizedFasciclesEnabled" che restringe la ricerca ai soli fascicoli a cui l'utente è autorizzato.
La funzionalità è attiva se SearchOnlyAuthorizedFasciclesEnabled = True e Security = False

#SZ
######################################################################################################
[ASMN-RE]
Sono state apportate delle modifiche al modulo VecompSoftware.JeepService.BDSImporter del servizio JeepService che si occupa
di conservare i referti di ASMN-RE.
Le modifiche riguardano la verifica della correttezza dei file firmati digitalmente, prima dell'effettiva importazione in BiblosDS/ParER.
E' quindi necessario aggiornare il JeepService e il relativo modulo del cliente ASMN-RE alla versione 8.74.
Il modulo prevede i seguenti nuovi parametri (configurazione da JeepService Dashboard):
 - SignatureValidationEnabled = abilita/disabilita la gestione degli elementi da scartare per errata validazione firma.
 - SignatureValidationBaseUrl = Url del servizio REST di validazione firma documenti (es. https://dss.agid.gov.it).

Il modulo, se il documento presenta una firma digitale non valida, sposta il documento (con relativa indicazione dell'errore) in una directory con nome "Scarto",
che deve necessariamente esistere come sottodirectory del valore impostato nel parametro "WorkingFolder".

Per tale gestione è necessario anche aggiornare il sito ParerWebMonitor.

#AC
######################################################################################################

Dopo l'installazione in ambiente di produzione è necessario lanciare il tool DSWFascicleMigrator che bonifica i fascicoli con le nuove strutture del database 8.74.
Prima di lanciare il tool configurare la connectionstring presente nel file di configurazione DSWFascicleMigrator.exe.config

NB: se il tool non viene lanciato, il modulo fascicoli non funzionerà correttamente.

#FL
######################################################################################################

Dopo l'installazione in ambiente di produzione è necessario lanciare il tool DSWHashGeneratorMigrator che bonifica le tabelle di log (ProtocolLog,TableLog,PECMailLog,DossierLogs,FascicleLogs,uds.udsLogs,documentSeriesItemLog) per popolare la nuova colonna hash per la nuove strutture del database 8.74.
Prima di lanciare il tool configurare la connectionstring presente nel file di configurazione DSWHashGeneratorMigrator.exe.config e impostare il parametro in appsetting che indica il numero di record che si vogliono migrare.

#CC
######################################################################################################
[ENAV]
E' stato creato un nuovo modulo del JeepService che permette la conservazione in BiblosDS dei Log applicativi della DocSuite.
Il modulo con nome VecompSoftware.JeepService.LogConservation permette di conservare le seguenti tipologie di Log:
 - ProtocolLog
 - DocumentSeriesItemLog
 - DossierLog
 - FascicleLog
 - UDSLog
 - PECMailLog
 - TableLog

Il modulo per il suo corretto funzionamento necessita la configurazione, tramite Dashboard amministrativa, dei seguenti parametri:
 - Abilita conservazione log protocollo = Abilita il servizio di conservazione dei log di protocollo.
 - Abilita conservazione log serie documentali = Abilita il servizio di conservazione dei log di serie documentali.
 - Abilita conservazione log archivio = Abilita il servizio di conservazione dei log di archivio.
 - Abilita conservazione log amministrazione = = Abilita il servizio di conservazione dei log di amministrazione.
 - Abilita conservazione log fascicolo = = Abilita il servizio di conservazione dei log di fascicolo.
 - Abilita conservazione log PEC = Abilita il servizio di conservazione dei log PEC.
 - Abilita conservazione log dossier = Abilita il servizio di conservazione dei log di dossier.

E' necessario definire anche una nuova location per definire l'archivio di BiblosDS in cui salvare i documenti.
L'ID della nuova location dovrà poi essere impostato nel parametro di ParameterEnv LogConservationLocation.

Il modulo LogConservation è disegnato per processare un set massimo di risultati per volta per singola tipologia, definito da un parametro di
appSettings del servizio JeepService.
Di default tale chiave di configurazione è così definita:
<add key="DocSuite.Default.ODATA.Finder.TopQuery" value="100" />

Modificare tale chiave con il valore della medesima presente in DocSuite definita nella sezione <appSettings>.
N.B. se la chiave "DocSuite.Default.ODATA.Finder.TopQuery" non viene impostata verrà utilizzato il valore di default 500.

Come visto sopra è necessario creare un nuovo archivio BiblosDS abilitato alla conservazione con i seguenti attributi:
 - Identificativo: (Type=String, Required=true, KeyOrder=1)
 - Data: (Type=DateTime, Required=true, Format={0:dd/MM/yyyy HH:mm:ss}, MainDate=true)
 - DataVersamento: (Type=DateTime, Required=true, Format={0:dd/MM/yyyy})
 - Evento: (Type=String, Required=true)
 - Descrizione: (Type=String, Required=false)
 - Hash: (Type=String, Required=false)
 - Tipologia: (Type=String, Required=true)
 - IdRiferimento: (Type=String, Required=false)
 - Riferimento: (Type=String, Required=false)
 - AnnoRiferimento: (Type=Int, Required=false)
 - NumeroRiferimento: (Type=String, Required=false)
 - OggettoRiferimento: (Type=String, Required=false)

Per abilitare un archivio BiblosDS alla conservazione digitale è necessario impostare i seguenti valori della tabella Archive (non tutti i valori sono gestibili tramite AdminCentral):
 - IsLegal = true
 - PathPreservation = directory preservation
 - FullSignEnabled = true
 - VerifyPreservationDateEnabled = true
 - VerifyPreservationIncrementalEnabled = false (da abilitare se si vuole verificare la sequenzialità dei documenti)
 - FiscalDocumentType = tipologia di documento

Nella tabella ArchiveCompany aggiungere poi la correlazione con l'archivio generato.

I documenti conservati in BiblosDS saranno degli XML con una struttura simile alla seguente:
Es. (Conservazione di un log dell'entità DossierLog)
<?xml version="1.0" encoding="utf-8"?>
<Log>
  <DataRegistrazione>2018-05-25T07:30:52.3081486+00:00</DataRegistrazione>
  <UtenteRegistrazione>VECOMPSOFTWARE\Chiavegato</UtenteRegistrazione>
  <TipologiaLog>Creazione cartella in dossier</TipologiaLog>
  <Riferimento>
    <Anno>2018</Anno>
    <Numero>62</Numero>
    <IdentificativoUnivoco>f7c2263c-b5be-4cc0-9d72-099a25712f5f</IdentificativoUnivoco>
    <Oggetto>Verifica tecnica</Oggetto>
  </Riferimento>
  <TipoRiferimento>Dossier</TipoRiferimento>
  <Descrizione>Creata cartella 'Sotto cartellina 3' </Descrizione>
  <Hash>030689FABA22778EAC440A6ED291D48C2AD6FB167A421D14517AA67F2E812BA5</Hash>
</Log>

#AC
######################################################################################################
Sono stati aggiunti i seguenti Endpoints da aggiungere al TenantModel nella sezione Entities:

      "Conservation": {
        "IsActive": true,
        "Timeout": "00:00:30",
        "ODATAControllerName": "Conservations"
      }

Vedi gli esempi di configurazione in ExampleMultiTenant_8.74.json e ExampleOneTenant_8.74.json

#AC
######################################################################################################
Per il corretto funzionamento del servizio JeepService 8.74 è necessario verificare che nella directory Config del servizio sia presente il file
di configurazione WebApi.Client.Config.Endpoints.json allineato alla versione corrente.

#AC
######################################################################################################