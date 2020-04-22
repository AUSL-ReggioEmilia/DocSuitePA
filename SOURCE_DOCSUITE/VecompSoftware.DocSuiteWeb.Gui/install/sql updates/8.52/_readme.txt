La DocSuite 8.52 funziona correttamente se e solo se YAC - WebAPI è allineata alla versione 8.52.
Verificare sempre anche i README del progetto YAC - WebAPI e gli eventuali specifici scripts SQL

Per motivi di retro compatibilità non è possibile avere la versione 8.52 in stage assieme a una versione di produzione di inferiore alla 8.52.
L'impatto è per tutti i clienti che usano i Fascicoli.

#FL
###############################################################

La DocSuite 8.52 per un corretto funzionato lato Browser ha bisogno che NON sia attiva la compatibilità nelle impostazioni di IE8. Per le versioni successive il problema non si pone.
Verificare col cliente che di default la seguente impostazione non sia attiva/fleggata
	- strumenti -> impostazioni visualizzazione compatibilità -> 'Visualizza siti intranet in visualizzazione compatibilità' (2° voce)

Nell'eventualità che lo sia, bisogna fare richiesta esplicità che venga tolta. Per dettagli tecnici chiedere a Sviluppo.

#FL
###############################################################

I TenantModel della DSW 8.52 vanno aggiornati alle nuove modifiche sull'autenticazione integrata delle WebAPI, includendo inoltre le referenze alle nuove entitià dei fascicoli.

Per semplicità mostriamo due esempi aggiornati di configurazione del parametro 'TenantModel' aggiornato nel file

	- ExampleMultiTenant_8.52.json nel caso di multitenant
	- ExampleOneTenant_8.52.json nel caso di tenant unico.

Nello specifico abbiamo apportato le seguenti modifiche:
	- Nella sezione Entities abbiamo aggiunto:
			  "Protocol": {
				"IsActive": true,
				"Timeout": "00:00:30",
				"ODATAControllerName": "Protocols"
			  },
			  "DocumentUnitModel": {
				"IsActive": true,
				"Timeout": "00:00:30",
				"ODATAControllerName": "DocumentUnits"
			  },
			  "Fascicle": {
				"IsActive": true,
				"Timeout": "00:00:30",
				"ODATAControllerName": "Fascicles"
			  },
			  "FascicleLog": {
				"IsActive": true,
				"Timeout": "00:00:30",
				"ODATAControllerName": "FascicleLogs"
			  },
			  "FascicleProtocol": {
				"IsActive": true,
				"Timeout": "00:00:30",
				"ODATAControllerName": "FascicleProtocols"
			  },
			  "FascicleResolution": {
				"IsActive": true,
				"Timeout": "00:00:30",
				"ODATAControllerName": "FascicleResolutions"
			  },
			  "Resolution": {
				"IsActive": true,
				"Timeout": "00:00:30",
				"ODATAControllerName": "Resolutions"
			  },
			   "FascicleUDS": {
				"IsActive": true,
				"Timeout": "00:00:30",
				"ODATAControllerName": "FascicleUDS"
			  },
			   "FascicleLink": {
				"IsActive": true,
				"Timeout": "00:00:30",
				"ODATAControllerName": "FascicleLinks"
			  },
			  "DomainUserModel": {
				"IsActive": true,
				"Timeout": "00:00:30",
				"ODATAControllerName": "DomainUsers"
			  }

	- Nella sezione Addresses (sotto sezione figlia del nodo principale WebApiClientConfig) abbiamo tolgo la sequente parte di configurazione:
			  "NetworkCredential": {
				"$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.Credential, VecompSoftware.DocSuiteWeb.Model"
			  }

	- Nella sezione Addresses abbiamo aggiunto:
			{
			  "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
			  "EndpointName": "Fascicle",
			  "AddressName": "API-EntityAddress",
			  "ControllerName": "Fascicle"
			},
			{
			  "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
			  "EndpointName": "FascicleLink",
			  "AddressName": "API-EntityAddress",
			  "ControllerName": "FascicleLink"
			},
			{
			  "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
			  "EndpointName": "FascicleLog",
			  "AddressName": "API-EntityAddress",
			  "ControllerName": "FascicleLog"
			},
			{
			  "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
			  "EndpointName": "FascicleProtocol",
			  "AddressName": "API-EntityAddress",
			  "ControllerName": "FascicleProtocol"
			},
			{
			  "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
			  "EndpointName": "FascicleResolution",
			  "AddressName": "API-EntityAddress",
			  "ControllerName": "FascicleResolution"
			},
			{
			  "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
			  "EndpointName": "FascicleDocumentSeriesItem",
			  "AddressName": "API-EntityAddress",
			  "ControllerName": "FascicleDocumentSeriesItem"
			},
			{
			  "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
			  "EndpointName": "FascicleUDS",
			  "AddressName": "API-EntityAddress",
			  "ControllerName": "FascicleUDS"
			},
		    {
		      "$type": "VecompSoftware.DocSuiteWeb.Model.WebAPI.Client.WebApiControllerEndpoint, VecompSoftware.DocSuiteWeb.Model",
		      "EndpointName": "Resolution",
		      "AddressName": "API-EntityAddress",
		      "ControllerName": "Resolution"
		    }

#SDC / #FL / #AC
###############################################################

Impostare il parametro FascicleContactId con l'indicazione dell' id del ramo di rubrica utilizzato 
per la definizione del responsabile di procedimento.

#AC
###############################################################

[AUSL-PC]
E' stato modificato il report riguardante la stampa dell'elenco dei concorsi CONCORSIGRID_Esporta.
Per dettagli chiedere a Sviluppo.

#SDC 
###############################################################