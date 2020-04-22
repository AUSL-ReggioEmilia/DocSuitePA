La versione 8.61 di DocSuite rimuove la visibilità dell'icona "Pratiche" nella griglia dei risultati di Protocollo.
Questa modifica migliora le performance della ricerca di Protocollo e implementa un primo passaggio nella revisione del
modulo Pratiche.
Per i clienti ENPACL, CTT e ASMN tale modifica deve essere condivisa con il referente prima dell'aggiornamento.
Nel caso in cui il cliente non accetti la modifica, la versione da rilasciare sarà la 8.60.
Per tutti gli altri clienti è possibile procedere con l'installazione della release 8.61.

#AC
###########################################################

Attenzione: prima di installare la DocSuite 8.61 cancellare tutti i file nella bin della Gui.
            Il parametro di appsettings 'DefaultOdataTopQuery' va rinominato con 'DocSuite.Default.ODATA.Finder.TopQuery'.

#SDC
###########################################################

E' stato aggiornato il TenantModel nella sezione Entities.
Impostare la proprietà IsActive = true solo ed esclusivamente per il Tenant corrente.
Vedi gli esempi di configurazione in ExampleMultiTenant_8.61.json e ExampleOneTenant_8.61.json

#SZ
###########################################################

Per tutti i clienti che utilizzano le PEC deve essere aggiunto un nuovo attributo in BiblosDs2010 nell'archivio di conservazione
con le seguenti caratteristiche:

Nome: MailDate
Type: DateTime
Mode: Modify Always
Required: False
Attribute Group: Default
Required for preservation: True

#AC
###########################################################

Aggiornata nel JeepService 8.59 la possibilità di importare direttamente dalla tabella excel le sottosezioni, 
per poter usufruire di questa funzionalità è necessario aggiungere al foglio excel una colonna "Subsection"
nella quale viene indicato il nome della sottosezione.

#MM
###########################################################