###############################################################
NEI FILE DI MIGRAZIONE DEI TRE DATABASE CERCARE IL COMANDO UPDATE [dbo].[Role] SET [TenantId] = '<immettere CurrentTenantId>'
E SOSTITUIRE <immettere CurrentTenantId> CON IL VALORE RELATIVO AL PARAMENTRO TenantId del TenantModel corrente.

#FL.
#########################################################################################################################################################

SONO STATI INTRODOTTI DEGLI SCRIPT CHE BONIFICANO I DATI 'REGISTRATIONUSER', 'LASTCHANGEDUSER', 'SYSTEMUSER', 'ACCOUNT' IN MOLTE ENTITA'.
ESEGUIRE GLI SCRIPT COME DESCRITTO SOTTO ANDANDO A SETTARE PRIMA I VALORI DEI PARAMETRI (vedi descrizione dentro lo script)
SET @Dominio = <dominio>
SET @Default = <nome utente di default>

ESEGUIRE GLI SCRIPT NEL MODO SEGUENTE:

	- ProtocolloDB_Bonifica_FullUserName SUL DB DI PROTOCOLLO
	- PraticheDB_Bonifica_FullUserName SUL DB DI PRATICHE
	- AttiDB_Bonifica_FullUserName SUL DB DI ATTI
	- CommonDB_Bonifica_FullUserName SU TUTTI E TRE I DB.

#FL. / #SDC
#########################################################################################################################################################

ATTENZIONE PRIMA DI INSTALLARE IN STAGE LA 8.51 AGGIORNARE IN PRODUZIONE DSW >= 8.27 POST]
ATTENZIONE PER ENAV: PER SEMPLICITA' DI CR SONO STATI PREDISPOSTI DUE SOLI FILE SQL CHE RAGGRUPPANO TUTTI GLI SCRIPT INTERMEDI DA 8.24 -> 8.51

USARE QUESTO STANDARD SUI TENANT ID

    "TenantId": "1F761813-6086-44F1-A507-99954EA64343",
    "TenantName": "AUSL-RE"
  
    "TenantId": "1F761813-6086-44F1-A507-99954EA64342",
    "TenantName": "ASMN"
  
	"TenantId": "F916C0BA-69CF-4589-9B13-DBA7E97CFB8F",
	"TenantName": "ENAV"

	"TenantId": "54203D28-932B-4CB4-BAEA-2E580030F2F0",
	"TenantName": "ASL-TO"

	"TenantId": "7466C45C-ED6F-45C7-9D0A-A24EC53168E5",
	"TenantName": "ENPACL"

	"TenantId": "24D6F89E-A9B9-4B71-960A-C23ED02F25ED",
	"TenantName": "AUSL-PC"

	"TenantId": "C0A21F9F-49D4-4701-9863-B7FC2B16493B",
	"TenantName": "BCOM"

	"TenantId": "26576A1E-8119-469A-8274-4AD40A9D5088",
	"TenantName": "TEAFLEX"

	"TenantId": "FE5FEA93-C143-4517-89CD-788D1BA30736",
	"TenantName": "OMMC"

	"TenantId": "E796EA69-4FCE-4961-AB8F-D1FBCF224476",
	"TenantName": "FLEXOIL"

	"TenantId": "0EEA0371-93C9-4EB6-B028-66D11D5663FF",
	"TenantName": "FLARMA"

	"TenantId": "29C28C7D-98BF-48D9-996A-5603A2648ED5",
	"TenantName": "CTT"

	"TenantId": "36998F74-68B3-44D8-A757-E478277096A8",
	"TenantName": "CAP"

	"TenantId": "861D944D-0B93-4026-948D-0C2D17A6CBD8",
	"TenantName": "VECOMPSW"

Sono stati deprecati i seguenti parametri da ParameterEnv

	- MDDomains,
	- MDUsers,
	- MDPasswords,
	- TenantId,
	- DefaultADPassword,
	- Domain (solo dell'ambiente di Protocollo)

e da eliminare in appsettings.config i seguenti parametri:

	- ADUser,
	- ADPassword,
	- DocSuiteWeb.WebAPI.SignalRServerAddress.

Introdotto nuovo parametro di ProtocolEnv 'TenantModel' riguardante la gestione della DSW. 
(NOTA BENE: La prima volta che si inserisce questo parametro va scritto a mano nel database)
Il parametro consiste in una lista di elementi dove ogni elemento contiene i dati del singolo dominio; 
quindi il parametro conterrà tanti elementi quanti sono i domini in base alla struttura seguente (esempio nel caso di due domini):

[
  {
    "TenantId": "",
    "TenantName": "",
    "CurrentTenant": ,
    "DomainAddress": "",
    "DomainPassword": "",
    "DomainUser": "",
    "SignalRAddress": "",
	"DSWUrl":"",
	"ODATAUrl":"",
	"EnvironmentsEnable" : ,
	"Entities" : ,
    "WebApiClientConfig":
  },
  {
    "TenantId": "",
    "TenantName": "",
    "CurrentTenant": ,
    "DomainAddress": "",
    "DomainPassword": "",
    "DomainUser": "",
    "SignalRAddress": "",
	"DSWUrl": "",
	"ODATAUrl":"",
	"EnvironmentsEnable" : ,
	"Entities" : ,
    "WebApiClientConfig": 
  }
]

nel caso di dominio unico quindi conterrà un solo elemento:

[
  {
	"TenantId": "",
	"TenantName": "",
	"CurrentTenant": ,
	"DomainAddress": "",
	"DomainPassword": "",
	"DomainUser": "",
	"SignalRAddress": "",
	"DSWUrl": "",
	"ODATAUrl":"",
	"EnvironmentsEnable": ,
	"Entities": ,
	"WebApiClientConfig":
  }
]

Ogni campo va popolato inserendo il valore tra gli apici, eccetto per :
	- "CurrentTenant", dove va inserito un valore booleano di true o false senza apici, 
	- "WebApiClientConfig"
	- "EnvironmentsEnable"
	- "Entities"

Ogni campo va configurato come segue

	1. TenantId = valore indicato in 'TenantId' (vedere elenco sopra)
	2. TenantName = nome del cliente (vedere elenco sopra)
	3. CurrentTenant = true se il dominio corrisponde a quello corrente, false altrimenti. 
					   CurrentTenant deve essere impostato a true nel caso del dominio corrente, se il dominio è unico il valore è sempre true.
	4. DomainAddress = valore indicato in 'Domain'(caso dominio unico) oppure uno dei valori di 'MDDomains' (caso multidominio)				
	5. DomainPassword = valore indicato in appsetting in 'ADPassword' (caso dominio unico) oppure uno dei valori di 'MDPasswords' (caso multidominio).
					    [ENAV] nel caso di password "NON GESTITA", la password verrà specificata nel TenantModel.
	6. DomainUser = valore indicato in appsetting in 'ADUser' (caso dominio unico) oppure uno dei valori di 'MDUsers' (caso multidominio).
					Il valore deve contenere il nome del dominio e il nome utente separati da un doppio backslash, in base alla struttura
												VECOMPSOFTWARE\\developer
	7. SignalRAddress =  valore indicato in appsetting di 'DocSuiteWeb.WebAPI.SignalRServerAddress'
	8. DSWUrl = url della DocSuite del dominio corrispondente
	9. ODATAUrl = url delle WebApi seguito da /ODATA/
	9. EnvironmentsEnable = questa proprietà deve seguire la struttura
									
									{
									 "Protocollo" :  true,
									 "Pratiche" : true,
									 "Atti" : true
									} 

	10. Entities = questa proprietà deve seguire la struttura in base ad ogni entità
								
									{
									  "Collaboration": {
										"IsActive": true,
										"Timeout": "00:00:10",
										"ODATAControllerName": "Collaborations"
									  },
									  "CollaborationSign": {
										"IsActive": true,
										"Timeout": "00:00:10",
										"ODATAControllerName": "CollaborationSigns"
									  }
								    }

	11. WebApiClientConfig = copiare e incollare il contenuto del file CONFIG\WebApi.Client.Config.json

ATTENZIONE : Il parametro 'Domain' di ProtocolEnv è stato sostituito da una proprietà che va a recuperare il DomainName del dominio corrente. 
			 DomainName si ricava da DomainUser, separando il nome del dominio dal nome dell'utente, quindi quando si inserisce il valore di DomainUser bisogna fare attenzione
			 ad inserire correttamente il dominio.

Esempio di configurazione del parametro nel caso di due domini nel file ExampleMultiTenant.json.
Esempio di configurazione del parametro nel caso di un unico dominio nel file ExampleOneTenant.json.

#SDC
####################################################
NOTA PER SVILUPPO:

Aggiunta gestione Timeout nella configurazione di WebApiClientConfig.
Impostare  per ogni Addresses il parametro "Timeout": "00:01:00" (ATTENZIONE! Modificare con la tempistica desiderata mantendo lo stesso formato. (Il valore 60 non è ammesso)).

#AC
####################################################

Modificare il parametro CollaborationColumnsVisibility modificando i nomi delle colonne come da struttura seguente:

	  "Entity.IdCollaboration": true,
      "ClientSelectColumn": false,
      "cDocType": true,
      "cDelete": true,
      "cPriority": true,
      "cPerson": false,
      "cType": true,
      "cDocSign": true,
      "cUserDocSign": false,
      "VersioningCount": true,
      "Number": false,
      "Entity.MemorandumDate": true,
      "Entity.Subject": true,
      "Entity.Note": true,
      "Proposer": true,
      "cToSign": true,
      "cToProtocol": false,
      "cDownload": false,
      "signDocRequired": false,
	  "TenantModel.TenantName": true 

Esempio di configurazione del parametro nel file _collGridColumns_8.51.txt.

#AC
#####################################################

Per configurare le etichette del menu, modificare il file DocSuiteMenuConfig.json nella cartella Config. 
Di seguito vengono riportati i nodi da modificare in base al valore che hanno i rispettivi parametri per ogni cliente:

		Menu1 -> FirstNode4 - Name = valore di MessageViewName
		Menu3 - Name = valore di CollaborationMenuLabel
		Menu3 -> FirstNode10 - Name = valore di MieiCheckOutMenuLabel
		Menu6 - Name = valore di DocumentSeriesName
		Menu8 - Name = valore di Contract.TableNamePlural
		Menu11 -> FirstNode1 - Name = valore di OChartModuleName

#SDC
####################################################

E' stato aggiornato BiblosDS 2010, necessario al corretto funzionamento della DocSuite 8.51.
Apportare le modifiche descritte nel readme contenuto nel BiblosDS.WCF.WCFService e nel WCF_HOST.
Nello specifico sono stati cambiato e aggiunti i contratti specificati nel wcfservices.system.servicemodel.services.config.
Quindi è necessario allineare entrambi i pacchetti WCF e HOST con il file di configurazione aggiornato.

#FL.
###############################################################

Sono stati rimossi da ParameterEnv i seguenti parametri:

	- "PECInHiddenColumns";
	- "PECOutHiddenColumns".

Si è introdotto un nuovo parametro chiamato "PECInOutColumnVisibility" che governa la visibilità delle colonne in Posta in Arrivo e Posta Inviata.
La configurazione di default è quella indicata nel file PECInOutColumns.json e va modificata in base al cliente nel modo seguente:

	- [ENAV] Modificare a true il valore di "colMailBoxName" nella vista PECIncomingMails;	
	- [TUTTI I CLIENTI] Le colonne che erano indicate nel parametro "PECInHiddenColumns" vanno configurate con visibilità a false in PECIncomingMails;
	- [TUTTI I CLIENTI] Le colonne che erano indicate nel parametro "PECOutHiddenColumns" vanno configurate con visibilità a false in PECOutgoingMails;
	- [TUTTI I CLIENTI] Se il parametro "EnableCC" è impostato a true, modificare a true il valore di "cIsCc" in PECIncomingMails e "colRecipientsCc" in PECOutgoingMails;
	- [TUTTI I CLIENTI] Se il parametro "PECHandlerEnabled" è impostato a true, modificare a true il valore di "Handler" in PECIncomingMails;
	- [TUTTI I CLIENTI] Se il parametro "ProtocolBoxEnabled" è impostato a true, modificare a false i valori di "cInterop", "cIsPEC", "cMoved", "Reply", "Forward" in PECIncomingMails.

Dopo aver modificato la configurazione, copiare tutto il contenuto del file e incollarlo nella casella di testo del parametro "PECInOutColumnsVisibility".

#AB.
###############################################################
