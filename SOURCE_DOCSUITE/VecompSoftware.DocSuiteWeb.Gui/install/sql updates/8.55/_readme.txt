La release 8.55 di DocSuite necessita, per il corretto funzionamento, di WebAPI 8.55, Enterprise Service BUS e Service BUS Listener 8.55.

#AC
###############################################################

Prima di lanciare gli script devono essere lanciati gli script 8.55 delle WebAPI

#AC
###############################################################

Aggiornare i moduli dei servizi listener del ServiceBUS (release 8.55):
 - VecompSoftware.ServiceBus.Module.UDS.Listener.Create
 - VecompSoftware.ServiceBus.Module.UDS.Listener.DataInsert
 - VecompSoftware.ServiceBus.Module.UDS.Listener.DataUpdate
 - VecompSoftware.ServiceBus.Module.UDS.Listener.DataDelete
 - VecompSoftware.ServiceBus.Module.UDS.Listener.Update
 - VecompSoftware.ServiceBus.Module.CQRS.Listener.ModelInsert
 - VecompSoftware.ServiceBus.Module.CQRS.Listener.ModelUpdate

#AC
###############################################################

Ordine di esecuzione degli script SQL:

1) ProtocolloDB_migrations.sql
2) PraticheDB_migrations.sql
3) AttiDB_migrations.sql
4) ATTENZIONE! Lo script AttiDB_production.sql và eseguito solo ed esclusivamente quando si sta
installando in produzione.
Se si sta installando in Stage tale script non deve essere lanciato in quanto renderebbe incompatibile
la versione installata in produzione.

#AC
###############################################################

Aggiornare i seguenti moduli del JeepService 8.55:
 - AVCP
 - CollegioSindacaleTorino (PECOC)
 - DSWRitiroAttiASMN
 - DSWRitiroAUSLRE
 - ODGManager
 - DocSeriesImporter
 - DocSeriesExporter

Aggiornare il JeepService 8.55 solo se si è installata la versione 8.55 di DocSuite
in produzione.

#AC
###############################################################

E' stata introdotta la funzionalità del massimario di scarto.
Per permettere la comunicazione con le WebAPI, 
vanno aggiunti nella sezione Entities del TenantModel la parte seguente:

     "MassimarioScarto": {
        "IsActive": true,
        "Timeout": "00:00:30",
        "ODATAControllerName": "MassimariScarto"
      },
      "Category": {
        "IsActive": true,
        "Timeout": "00:00:30",
        "ODATAControllerName": "Categories"
      }

Si vedano gli esempi aggiornati ExampleMultiTenant_8.55.json e ExampleOneTenant_8.55.json.

#AC
###############################################################

Come specificato nel readme 8.54 per introdurre una modalità di lettura del tenantmodel,nella cartella Config
è presente il file di default dl menu WebApi.Client.Config.Endpoints.json, che ad ogni rilascio va sovrascritto al precedente.
Quindi aggiornare il file di configurazione WebApi.Client.Config.Endpoints.json con quello fornito nel pacchetto
di installazione.

#AC
###############################################################

E' stato introdotto un nuovo parametro "SelectCheckRecipientEnabled" per Piacenza, il quale se abilitato:
quindi ["SelectCheckRecipientEnabled" = true], permette di poter selezionare nell'invio PEC da Protocollo
più destinatari contemporaneamente con un check, quindi selezionando più destinatari è possibile eliminare
contemporaneamente tutti quelli selezionati con il check.
Se ["SelectCheckRecipientEnabled" = false], ritorna alla vecchia gestione, ovvero seleziona un singolo 
destinatario per volta per poterlo eliminare.

Per [AUSL-PC] = true

#GN
#################################################################

E' stato introdotto un nuovo parametro "ShowOnlySignAndNextEnabled" per FALCK, il quale se abilitato:
quindi ["ShowOnlySignAndNextEnabled" = true], permette di poter visualizzare solo il pulsante "Firma e Prosegui" nascondendo 
i pulsanti "Firma"e "Prosegui" in Collaborazione da Visionare/Firmare
Se ["ShowOnlySignAndNextEnabled" = false], ritorna alla vecchia gestione, ovvero permette la visualizzazione di tutti e 
tre i pulsanti, "Firma", "Firma e Prosegui" e "Prosegui"

Per [FALCK] = true

#GN
#################################################################

In peraparazione del nuovo giro Atti Unificato di AUSL-RE/ASMN sono stati introdotti nuovi parametri nella ResolutionEnv.
Questi parametri influenzano la configurazione del cliente AUSL-PC, per cui vanno configurati correttamente. 
Vedere dunque la gestione parametri Atti in DocSuite e relative note.

Per [AUSL-PC] vanno messi a true

#FL
#################################################################

E' stato introdotto un nuovo parametro "AVCPCIGUniqueValidationEnabled" per AUSL-PC, il quale:
se ["AVCPCIGUniqueValidationEnabled" = false], permette di poter escludere il controllo di univocità del CIG
nella Modifica AVCP.
Se ["AVCPCIGUniqueValidationEnabled" = true], ritorna alla vecchia gestione, ovvero permette il controllo rendendo 
univoco il CIG.

Per [AUSL-PC] = false

#GN
#################################################################

E' stata introdotta una nuova chiave di AppSettings per il WS WSSeries.

<add key="ConsultationIncludeDocumentStream" value="true" />

[AUSL-RE, ASMN-RE = true]
[Altri clienti = false]

Tale chiave gestisce la fase di consultazione di una serie documentale pubblicata.
Se value = true, la consultazione oltre ai metadati della serie includerà anche lo stream dei documenti.
Se value = false, la consultazione recupererà solo i metadati della serie (lo stream dovrà essere gestito successivamente).

Riassumendo, la chiave dovrà essere abilitata per quei clienti che non utilizzano il sito di AmministrazioneTrasparente (es. Sharepoint).

#AC
#################################################################

Per far funzionare il  modulo delle PEC del JeepService 8.55 è necessario che l'archivio delle PEC in Conservazione
abbia settati gli attributi definiti nel documento word BiblosDS PEC Conservation.docx

#SZ
#################################################################