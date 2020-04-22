ATTENZIONE: 
PRIMA DI INSTALLARE VERIFICARE SE LE PEC DSW 7 LEGACY SONO STATE MIGRATE IN BIBLOS. 
QUESTO AGGIORNAMENTO ESEGUIRA' LA RIMOZIONE FISICA DELLA COLONNA AttachmentStream DELLA TABELLA PECMailAttachment.
NE CONSEGUE LA PERDITA DI TUTTI GLI STREAM DSW 7, VERIFICARE PRIMA DI AVER MIGRATO IN BIBLOS LE PEC.

INOLTRE NON LA VERSIONE 8.54 NON E' RETROCOMPATIBILE

#FL
######################################################

Lo script di migrazione prevede una serie di drop table di entità mai usate e/o deprecate. Verificare che siano effettivamente vuote in produzione 
e in caso contattare sviluppo prima di procedere

Le Tabelle sono:

ATTI		-> ResolutionVersioning 
			-> AdvancedResolution
			-> ResolutionFolder
PROTOCOLLO	-> ProtocolCheckLog

#FL
######################################################

Prima di eseguire lo script di migrazione verificare che le colonne [ProtocolNumberOut], [ProtocolNumberIn], [ProtocolYearOut] e [ProtocolYearOut] della tabella PECMail
siano effettivamente vuote in produzione.
In caso contattare sviluppo.

#AC
######################################################

Sono state bonificate le tabelle del gruppo Protocollo aggiungendo le colonne (dove mancanti):

		- UniqueId
		- UniqueIdProtocol
		- RegistrationDate
		- RegistrationUser
		- LastChangedDate
		- LastChangedUser
		- Timestamp.

Lo scopo futuro è quello di avere la colonna UniqueId come chiave primaria delle entità e di conseguenza nelle tabelle figlie UniqueIdProtocol 
chiave esterna a Protocollo, bonificando quindi tutte le chiavi primarie ed esterne composte.

Lo script ProtocolloDB_bonifica contenente la bonifica potrebbe richiedere molto tempo (anche ore) dal momento che riguarda tabelle 
con quantità considerevoli di record (es. ProtocolLog). Per questo motivo abbiamo preferito separarlo dallo script di migrazione e
prevedere un'attività a parte.

Per ENAV: NON LANCIARE ProtocolloDB_bonifica, ma lanciare i due script contenuti nella cartella ENAV in questo ordine:
						01_ENAV_ProtocolloDB_bonifica_NULL {DA NON DA LANCIARE IN PRODUZIONE}
						02_ENAV_ProtocolloDB_bonifica_UPDATE {DA NON DA LANCIARE IN PRODUZIONE}
						03_ENAV_ProtocolloDB_bonifica_NOTNULL {UNICO SCRIPT DA LANCIARE IN PRODUZIONE}

#SDC
######################################################

Il parametro 'SmartAuthorization' è stato eliminato. 
Questo parametro era stato introdotto nella versione 7.2.2 quando è stata modificata in protocollo la logica di gestione delle autorizzazioni e della distribuzione di protocollo. 
Serviva per mantenere la vecchia logica delle autorizzazioni in attesa di verificare che la nuova logica fosse abbastanza stabile.
Se attivo, veniva utilizzata la nuova logica, se non attivo la vecchia.
Eliminando il parametro si è andati a rimuovere la vecchia logica, avendo verificato che quella nuova è quella corretta.

#SDC
######################################################

E' stato creato un Windows Service che invia in automatico dei comandi al ServiceBus per la creazione dei fascicoli periodici.
Per questa funzionalità sono necessarie due operazioni:

 1. Installare il modulo VecompSoftware.ServiceBus.Module.Fascicle.GeneratePeriodic del listener del ServiceBUS (release 8.54).	 
 2. Installare il nuovo Windows Service 'VecompSoftware.ServiceBus.MessageGenerator'.

#SDC
###############################################################

In 8.53, nella cartella update è presente lo script ProtocolloDB_update.sql che riguarda le modifiche apportate alle UDS 
con il rilascio 8.53 di fine Agosto 2016. 
Questo script va lanciato per i seguenti clienti: CTT, CAP, FALCK.

#SDC
###############################################################

Per assistenza è stata introdotta la US 5963 "Introdurre una modalità di lettura del menu json che non perda le personalizzazioni in fase di migrazione della DocSuite"
Nella cartella Config è presente il file di default del menu - DocSuiteMenuConfig.json - che ad ogni rilascio va sovrascritto al precedente. Per evitare dimenticanze in fase di deploy 
o di dover ogni volta modificare a mano il files, è possibile creare un file gemello DocSuiteMenuConfig_<valore Name del CurrentTenant che si trova nel TenantModel>.json .
In questo file è possibile inserire le sole voci di menu personalizzate, sarà poi cura del Software sostituire le singole voci.

E' stato caricato un esempio Example_DocSuiteMenuConfig_AUSLRE.json, utile per capire la personalizzazione di AUSL-RE (CurrentTenantModel = AUSLRE).

#FL
###############################################################

Per assistenza è stata introdotta la US 5998 "[DSW] Introdurre una modalità di lettura del tenanatmodel non preveda sempre l'aggiornamento dei valori di produzione durante fase di migrazione della DocSuite"
Nella cartella Config è presente il file di default del menu - WebApi.Client.Config.Endpoints.json - che ad ogni rilascio va sovrascritto al precedente.

Il tenant model può dunque essere semplificato (vedere i files di esempio) togliendo la sezione Endpoints situata nella proprietà WebApiClientConfig dei singoli tenant.
Sarà cura di sviluppo evolvere e mentenere sempre allineato il file degli endpoints. Quindi assistenza dovrà solo verificare il corretto funzionamento della DocSuite in fase 
di avvio dei servizi (es viste di scrivania di Collaborazione, Fascicoli e UDS).

#FL
###############################################################

TFS #USR 5790: La problematica relativa all'errore in protocollazione di PEC interoperabile è replicabile solamente nella release 8.27, 
mentre nella release 8.54 la procedura funziona correttamente. La problematica dei contatti doppi si risolve impostando il parametro 
PECInteropContactSearch = 2 [Descr. Parametro = "Indica il tipo di ricerca che viene fatta 
per i contatti da Interop. (0 = solo per Codice, 1= solo per Descrizione, 2 = prima per Codice, poi per descrizione)."].

#AC
###############################################################


E' stato introdotto il parametro CollDocSignedNotEditable che abilita la possibilità di modifica ed eliminazione dei documenti di una collaborazione solo se nessun documento è stato ancora firmato nella vista Da Visionare/Firmare.
Questo parametro è attivo se e solo se CollAttachmentEditable è attivo.

Per ASL-TO dev'essere posto a True.

#SZ
###############################################################

E' stato introdotto il parametro AmministrazioneTrasparenteConfigurazionOrder per ASL-TO che nelle griglie di ricerca del Personale, Dirigenti in Amministrazione Trasparente ordina alfabeticamente
sul cognome e nome in maniera crescente.Ordinamento fatto nella WSSeries della DocSuite.

Esempio della struttura Json del parametro:
[{"IdDocumentSeries":inserire id del DocumentSeries ,"DynamicColumns":[{"ColumnName":"Cognome","Order":0},{"ColumnName":"Nome","Order":0}]}]

Per AUSL-TO copiare l'intera stringa: [{"IdDocumentSeries":inserire id del DocumentSeries ,"DynamicColumns":[{"ColumnName":"Cognome","Order":0},{"ColumnName":"Nome","Order":0}]}]
inserendo l'id del DodumentSeries.
"Order :0" sta ad indicare l'ordinamento crescente voluto dal cliente.

#GN
###############################################################

In questi serivizi dell'API:
- WSSeries
- WSProt
- WSDocm
- WSColl
- AuslPcPubblicazione
se vengono aggiornati bisogna inserire la chiave nell' appsettings :

<add key="owin:AutomaticAppStartup" value="false"/>     

#GN
###############################################################

E' stato introdotto un nuovo contatore che mostra il numero di protocolli in evidenza all'utente.
Per vederlo è necessario aggiungere al parametro di scrivania 'NotificationTypes', il tipo 6.

[AUSL-PC] = aggiungere al parametro NotificationTypes, il numero 6.

#SDC
###############################################################

E' stato introdotto un nuovo parametro json dal nome 'HighlightProtocolGroups'.
Quando si mette in evidenza un protocollo ad un utente, si restringe la ricerca in AD dell'utente ai soli gruppi indicati nel parametro.
In questo modo si potranno mettere in evidenza i protocolli solo a determinati gruppi di utenti (nel formato Distinguished Name di ldap).
Il parametro ha la struttura seguente:
        {
			"IsActive": true,
			"GroupList": {
				"Group1": "CN=DSW_Lazzarotto,OU=DocSuiteWeb,OU=vecompsoftware,DC=vecompsoftware,DC=local",
				"Group2": "CN=DSW_DalCorso,OU=DocSuiteWeb,OU=vecompsoftware,DC=vecompsoftware,DC=local",
				... ecc
			 }
        }
La proprietà 'IsActive' indica se attivare il parametro e quindi se restringere la ricerca a determinati gruppi.
La proprietà 'GroupList' indica l'elenco dei gruppi di AD nei quali è possibile ricercare l'utente (nel formato Distinguished Name di ldap)

NB: Attenzione che il parametro dipende dalla configurazione del parametro BasicPersonSearcherKey.
    Se la query LDAP non funziona verificare se la sintassi è compatibile con la query prodotta. 
	Esempio : (&({BasicPersonSearcherKey})(|(memberof=CN=DSW_Lazzarotto,OU=DocSuiteWeb,OU=vecompsoftware,DC=vecompsoftware,DC=local)(memberof=CN=DSW_DalCorso,OU=DocSuiteWeb,OU=vecompsoftware,DC=vecompsoftware,DC=local)))


Si attiva solo se 'IsActive' = true e se il parametro ProtocolHighlightEnabled è abilitato.
Se 'IsActive' = false, la ricerca viene fatta su tutti i gruppi di AD.

Per AUSL-PC, deve essere configurato con 'IsActive': true e la lista di gruppi definita dal cliente (nel formato Distinguished Name di ldap)

Se non viene configurato, il valore di default è

		{
			"IsActive": false,
			"GroupList": { }
		}
        
#SDC
###############################################################

E' necessario aggiornare il JeepService alla 8.54. Tutti i moduli vanno allineati alla versione 8.54.
Eventuali anomalie di vecchi moduli di produzioni vanno analizzati con Sviluppo prima di aggiornarli.

#FL
###############################################################

Gestione richiesta Torino US #6269 - Obbligatorietà inserimento del proponente da rubrica in fase di inserimento proposta
La configurazione prevede l'utilizzo di una nuova chiave in TabMaster da impostare per le tipologie di Atto
richieste (Torino -> sia Delibere che Determine).

Nella tabella TabMaster (DB Atti) modificare la colonna ManagedData inserendo alla corrispondenza Proposer[... la stringa ".ONLYFIRST."]
L'inserimento della stringa ONLYFIRST è indipendente dalla posizione in cui viene inserita.
Con l'inserimento di tale paramentro per Torino è possibile richiedere l'obbligatorietà, con un controllo,
dell'inserimento del proponente principale da rubrica.


#GN
##############################################################
