La release 8.53 di DocSuite necessita, per il corretto funzionamento, di WebAPI 8.53, Enterprise Service BUS e Service BUS Listener 8.53.

#AC
###############################################################

Il TenantModel della DSW 8.53 va aggiornato con le nuove modifiche riguardanti l'introduzione della DocumentUnit.
Si vedano i file di esempio ExampleMultiTenant_8.53.json e ExampleOneTenant_8.53.json.

#SDC
###############################################################

E' stato creato un nuovo tool di gestione delle nuove tabelle DocuementUnits.
Il tool si chiama DocumentUnitMigrator.exe e permette di creare una 'copia' mormalizzata [Oggetto, Titolo, Anno, Numero, Classificatore, Contenitore, Settori, Catene documentali] delle entità Protocolli, Atti, Serie Documentali.
Questo ci permetterà di gestire al meglio tutte le operazioni trasversali che operano su questi oggetti (si pensi ad esempio alla fascicolazione) senza la necessità di avere query non performanti (unions) tra più tabelle.
Lo giustifichiamo in quanto dopo il rilascio 8.52 ci siamo accorti dai clienti (Torino e Piacenza) che le nuove viste di scrivania dei fascicoli andavano in timeout anche per range di date molto ristretti.

Attualmente DSW gestisce solamente Atti e Protocolli, quindi lanciare la migrazione per queste 2 tipologie.
Il funzionamento è molto semplice, la console chiede di inserire l'anno di riferimento da migrare e il valore numerico che corrispondente alla tipologia della Unità documentaria da migrare.
Il tool avviato mostrerà a video le informazioni necessarie per una corretta esecuzione.

La configurazione del tool prevede:
- ServiceClient (app.system.servicemodel.client.config)
	- Modificare l'url del client di BiblosDs2010 con l'indicazione del valore corretto.
	- Verificare che i bindings che si trovano nel programma siano allineati con quelli di produzione

- AppSettings (app.appsettings.config):
	- Modificare la chiave "DocSuiteWeb.Biblos.URI" con l'indirizzo corretto di BiblosDs2010 (sovrascrive il valore presente nei ServiceClient).
	- Modificare la chiave "ConfigurationName" con il valore corrispondente alla chiave di ParameterEnv (DB Atti) "Configuration" solo se è presente il DB Atti.
	- Modificare la chiave "IsProtocolAttachLocationEnabled" con il valore corrispondente alla chiave di ParameterEnv (DB Protocollo) "ProtocolAttachLocationEnabled" (default = false).

- AppConfig (DocumentUnitMigrator.exe.config)
	- Modificare le connectionstring di Protocollo e Atti (se esiste).

#AC review #FL
###############################################################

Aggiornare i moduli PEC, AVCP, DocSeriesImporter e PosteWeb del JeepService 8.53.

#SDC
###############################################################

Aggiornare i moduli dei servizi listener del ServiceBUS (release 8.53):
 - VecompSoftware.ServiceBus.Module.UDS.Listener.Create
 - VecompSoftware.ServiceBus.Module.UDS.Listener.DataInsert
 - VecompSoftware.ServiceBus.Module.UDS.Listener.DataUpdate
 - VecompSoftware.ServiceBus.Module.UDS.Listener.DataDelete
 - VecompSoftware.ServiceBus.Module.UDS.Listener.Update
 
e installare i nuovi moduli del CQRS:
 - VecompSoftware.ServiceBus.Module.CQRS.Listener.ModelInsert
 - VecompSoftware.ServiceBus.Module.CQRS.Listener.ModelUpdate

#AC
###############################################################

Lanciare il tool di migrazione BonificaContattiManualiUDS.exe che permette di migrare i contatti manuali salvati per tutte le UDS presenti.
La configurazione del tool prevede la modifica della sola ConnectionString nel file BonificaContattiManualiUDS.exe.config.

#AC
###############################################################

Nella tabella Location del db di Protocollo, allineare tutti i record di Location presenti nei db di Pratiche e Atti, 
in modo da avere tutte le location dei 3 db presenti sul db di Protocollo.

NB: Sono stati introdotti i parametri CollaborationLocation e ODGLocation per indicare rispettivamente l'Id delle location di Collaborazione e dell'ODG da parametro.

#SDC
###############################################################

Il file Docsuitenamespace_Entities_8.53.xml contiene l'export delle ultime configurazioni delle code e dei topic del service bus della DocSuite.
Per aggiorarle è necessario aprire il Tool Service Bus Explorer, specificare la connectiostring del service bus del cliente e dal menu Actions -> Import Entries selezionare
il file Docsuitenamespace_Entities_8.53.xml. Automaticamente eseguirà la configurazione, sovvrasvrivendo l'esistenza ma senza perdere eventuali messaggi presenti nelle code/eventi.

#FL
###############################################################

E' stata introdotta la funzionalità di annullamento di una UDS.
Per permettere l'invio di un comando/evento di annullamento di un UDS al ServiceBus, 
va aggiunta nella sezione EndPoints del TenantModel la parte seguente:

    {
      "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
      "EndpointName": "ICommandDeleteUDSData",
      "AddressName": "API-ServiceBusAddress",
      "ControllerName": "Queue"
    },
    {
      "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
      "EndpointName": "CommandDeleteUDSData",
      "AddressName": "API-ServiceBusAddress",
      "ControllerName": "Queue"
    },
	{
      "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
      "EndpointName": "IEventDeleteUDSData",
      "AddressName": "API-ServiceBusAddress",
      "ControllerName": "Topic"
    },
    {
      "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
      "EndpointName": "EventDeleteUDSData",
      "AddressName": "API-ServiceBusAddress",
      "ControllerName": "Topic"
    },

Si vedano gli esempi aggiornati ExampleMultiTenant_8.53.json e ExampleOneTenant_8.53.json.

#SDC
###############################################################

E' stato creato un nuovo modulo listeners del service bus che permette l'annullamento di una UDS.
Il modulo si chiama VecompSoftware.ServiceBus.Module.UDS.Listener.DataDelete.
E' necessario quindi installare un nuovo listeners con il nuovo modulo indicato.

#AC
###############################################################

Deve essere ricompilata la libreria dinamica delle UDS presente nella bin delle WebAPI con le ultime modifiche apportate 
alla struttura delle entità di contatti e autorizzazioni.
Chiedere a Sviluppo per tale attività durante l'aggiornamento.

#AC
###############################################################

Per tutte le tabelle UDS dinamicamente già create in DB devono essere aggiunte manualmente le seguenti ForeignKey:
	1)	ForeignKey contatti (ALTER TABLE [uds].[UDS_T_<Nome archivio>_Contacts]  ADD  CONSTRAINT [FK_UDS_T_<Nome archivio>_Contacts_IdContact] FOREIGN KEY([IdContact])
							 REFERENCES [dbo].[Contact] ([Incremental])
							 GO)
	2)	ForeignKey settori  (ALTER TABLE [uds].[UDS_T_<Nome archivio>_Authorizations]  ADD  CONSTRAINT [FK_UDS_T_<Nome archivio>_Authorizations_IdRole] FOREIGN KEY([IdRole])
							 REFERENCES [dbo].[Role] ([idRole])
							 GO)
	3)	ForeignKey protocollo  (ALTER TABLE [uds].[UDS_T_<Nome archivio>_Protocols]  ADD  CONSTRAINT [FK_UDS_T_<Nome archivio>_Protocols_IdProtocol] FOREIGN KEY([IdProtocol])
								REFERENCES [dbo].[Protocol] ([UniqueId])
								GO)
	4)	ForeignKey atti     (ALTER TABLE [uds].[UDS_T_<Nome archivio>_Resolutions]  ADD  CONSTRAINT [FK_UDS_T_<Nome archivio>_Resolutions_IdResolution] FOREIGN KEY([IdResolution])
							 REFERENCES [dbo].[Resolution] ([idResolution])
							 GO)
	5)	ForeignKey pec     (ALTER TABLE [uds].[UDS_T_<Nome archivio>_PECMails]  ADD  CONSTRAINT [FK_UDS_T_<Nome archivio>_PECMails_IdPECMail] FOREIGN KEY([IdPECMail])
							REFERENCES [dbo].[PECMail] ([IDPECMail])
							GO)
	6)	ForeignKey messaggi     (ALTER TABLE [uds].[UDS_T_<Nome archivio>_Messages]  ADD  CONSTRAINT [FK_UDS_T_<Nome archivio>_Messages_IdMessage] FOREIGN KEY([IdMessage])
								 REFERENCES [dbo].[Message] ([IDMessage])
								 GO)

#AC
###############################################################

E' stato introdotto il parametro SignedIconRenderingModality che indica il modo in cui viene rappresentata l'icona del documento firmato CADES o in modalità originaria,nelle griglie di 
ricerca di Atti ,Protocolli,UDS e Serie Documentali.Per ora  non si riesce a gestire la tipologia di icona da visualizzare nella griglia dei risultati delle UDS, Atti e Serie Documentali.
Esempio della struttura Json del parametro :

{"ProtocolIconModality":0,"ResolutionIconModality":0,"DocumentSeriesIconModality":0,"UDSIconModality":0} 

Per AUSL-RE,in base alle richieste ProtocolIconModality deve essere posto = 1.

#GN
##############################################################

Per i clienti che provengono da una versione precedente alla 8.53, lanciate tutti gli script (compresi quelli nella cartella update).
Per i clienti che hanno già una 8.53 installata, lanciare solo lo script ProtocolloDB_update.sql presente nella cartella update.
Lo script di update contiene le modifiche apportate alle UDS con il rilascio 8.53 di fine Agosto 2016. 

#SDC
###############################################################

[Modifica richiesta da AUSL-RE. Da aggiornare al cliente]

Il modulo JeepService/DocSeriesImporter è stato rivisto e sono stati modificati alcuni parametri.
- Parametro DoneFolderName -> Conterrà tutti gli XML andati a buon fine. Creare una directory specifica.
- Parametro StatusFolder -> Rimosso
- Parametro RecoveryFolder -> Rimosso

E' importante che per riprocessare un Task sia presente nella directory DROP l'ultimo XML di importazione 
(di default il servizio, se ci sono errori, lo lascia nella DROP).

#AC
###############################################################

[AmministrazioneTrasparente -> Per tutti i clienti]
Si consiglia di aggiungere, al binding utilizzato per la comunicazione con il WSSeries, le seguenti proprietà:

receiveTimeout="00:30:00" sendTimeout="00:30:00"

questo per evitare timeout di comunicazione in visualizzazione dei dettagli di una serie documentale con molti allegati.

Es. di configurazione:

<binding name="BasicHttpStreamedBinding" receiveTimeout="00:30:00" sendTimeout="00:30:00"
        transferMode="Streamed"
        maxReceivedMessageSize="2147483647"
        maxBufferSize="2147483647">
        <readerQuotas maxDepth="128"
                    maxStringContentLength="2147483647"
                    maxArrayLength="2147483647" />
        <security mode="None" />
    </binding>

#AC
###############################################################

[WSSeries -> Per tutti i clienti]
Si consiglia di configurare il Recycle del pool automatico ad intervalli regolari di 4 ore.
Questo per liberare eventuali risorse rimaste appese durante la visualizzazione dei documenti che potrebbero
provocare problematiche di performance.

#AC
###############################################################

[Solo per AUSL-RE, ASMN]
Aggiornare il modulo AVCP del JeepService 8.53.
Note per la configurazione: Sono stati aggiunti dei nuovi parametri e un file di configurazione specifici per
l'importazione di Reggio Emilia.

- Settare il parametro AVCPDropFolder con l'indicazione della directory dove salvare temporaneamente gli XLS da gestire
per la lettura dei partecipanti da un atto.
- Settare il parametro PartecipanteTemplate_File con l'indicazione del percorso dove trovare il file XML UslRePartecipanti.xml
fonito nel pacchetto di installazione (NB non modificare gli elementi al suo interno).

#AC
###############################################################

[Solo per AUSL-PC]
Aggiornare il modulo AVCP Excel del JeepService 8.53.
In tale modulo è stata implementata la possibilità di referenziare sia delibere che determine ad una serie AVCP.
Da cliente deve essere abilitato il parametro FindAllResolutionTypes.

Note per il cliente: il riferimento al numero di provvedimento presente nel file excel deve avere il medesimo formato 
di quanto visualizzato in DSW.

#AC
###############################################################