La DocSuite 8.50 è una major release e il requisito minumo di funzionamento è il framework .NET da 4.5.2 e SQL Server 2008 R2.

Per migrare in database la corretta procedura di installazione dei SQL update è la seguente (escluso ***REMOVED***): 
	- Atti\ForeignKey.sql
	- Pratiche\ForeignKey.sql
	- Protocollo\ForeignKey.sql
	- Protocollo\UniqueId.sql
	- AttiDB_migrations.sql
	- PraticheDB_migrations.sql
	- ProtocolloDB_migrations.sql


Sono state cambiate molte chiavi e parametri del Web.Config. Si consiglia di installare in produzione quello riportato in questa cartella

E' stato introdotto un nuovo prodotto VecompSoftware DocSuite.WebAPI necessario per il corretto funzionamento delle UDS e della DocSuite in generale
Questo prodotto per funzionare correttamente ha bisogno dei sequenti prodotti:
	- Microsoft Workflow Manager 1.0 refresh CU3
	- Microsoft Service Bus for Windows Server 1.1 
NB: Per i clienti che non usano i Workflow e UDS non è necessario installare i prodotti Microsoft sopra citati.

Per un dettaglio completo della procedura di installazione e configurazione dei tools Microsoft e DocSuite WebAPI, 
vedere i tre documenti allegati (NB: necessari riavvi del server)

	- 01 - Piano_di_Lavoro_SKYDOC_Installazione.pdf (Questa procedura prevede una installazione con una FARM a più nodi, tralasciare dal punto 11 in poi in ambienti con singolo server)
	- 02 - Piano_di_Lavoro_SKYDOC_Configurazione.pdf
	- 03 - Procedura configurazione ServiceBus.pdf
	- 04 - Piano_di_Lavoro_DSW.WebAPI_Installazione.pdf
	- 05 - Installazione servizi Service Bus.pdf

Considerando la complessità dell'installazione, si consiglia vivamente il presidio di sviluppo.

FL.
#####################################################

Attenzione! E' obbligatorio impostare il valore del parametro TenantId in ProtocolEnv.
Tale parametro viene utilizzato per la comunicazione con le Web API / Service Bus.
Il valore deve essere un GUID univoco.

USARE QUESTO STANDARD SUI TENANT ID

	"TenantId": "1F761813-6086-44F1-A507-99954EA64343",
	"TenantName": "AUSL-RE"
  
	"TenantId": "1F761813-6086-44F1-A507-99954EA64342",
	"TenantName": "ASMN"
  
	"TenantId": "F916C0BA-69CF-4589-9B13-DBA7E97CFB8F",
	"TenantName": "***REMOVED***"
	
	"TenantId": "EAC6F666-639D-4CA6-B264-D0C33797CAFD",
	"TenantName": "TECHNOSKY"

	"TenantId": "54203D28-932B-4CB4-BAEA-2E580030F2F0",
	"TenantName": "ASL-TO"
	
	"TenantId": "E5ED27AA-7FEA-432F-91F4-42B9A9250908",
	"TenantName": "ASL-TO-CITTA"

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

	"TenantId": "F3F61614-F74E-4E7A-AA78-5C2A6EA1FD16",
	"TenantName": "CTTSEDE"

	"TenantId": "19FE0276-F351-420F-B27C-FCDA4D73DECC",
	"TenantName": "CTTLINEA"

	"TenantId": "36998F74-68B3-44D8-A757-E478277096A8",
	"TenantName": "CAP"

	"TenantId": "F1651BCA-35CA-4911-A579-B2F95E203095",
	"TenantName": "CAPSEDE"

	"TenantId": "6A40E805-BE51-4236-9417-3016BDF7622C",
	"TenantName": "CAPLINEA"

	"TenantId": "861D944D-0B93-4026-948D-0C2D17A6CBD8",
	"TenantName": "VECOMPSW"

	"TenantId": "1AA9A825-5CAC-41D7-8C8F-C6E541ADF738",
	"TenantName": "AFOL"

	"TenantId": "DE5FE1FE-74E7-407C-9E18-F5CE0D9D283D",
	"TenantName": "FLCK"

	"TenantId": "0362E780-485B-4791-BF8C-31F1A30466A6",
	"TenantName": "UIRNET"

	"TenantId": "D5D9BCA8-1747-487B-9570-71269D28255B",
	"TenantName": "AASLP"

	"TenantId": "47A6E49F-45EC-4552-809F-C2F8E4A19FB4",
	"TenantName": "ITW"
	
	"TenantId": "37668BF0-3D9E-456F-BADB-3DF8EBF69E3F",
	"TenantName": "ACQVR"

	"TenantId": "FD0CF2BB-5161-4C9C-B7F4-91B6712DE10F",
	"TenantName": "AGENAS"

	"TenantId": "5ACBF4DB-A96B-48FF-9E92-6073BD074270",
	"TenantName": "TecMarket"

	"TenantId": "55315EEB-E623-4C8C-80F8-EACEF328BDC6",
	"TenantName": "COVIP"

	"TenantId": "11D69D07-8314-451A-884C-E7CCF45E587F",
	"TenantName": "Li-nea"

	"TenantId": "6219628E-B6AC-45FC-8B9B-CB8C064CE73E"
	"TenantName": "LAND"

	"TenantId": "91EB5DF6-B39A-4416-ADFB-BCDE83B18E93"
	"TenantName": "ENEA"

	"TenantId": 9BA38CE7-6717-410F-8CBF-59E352C1E340"
	"TenantName": "CNOCDL"

	"TenantId": "FE1A737D-34D8-40AB-9A01-8B089758B4C3"
	"TenantName": "RSM"


FL.
#####################################################
Procedere alla modifica dell'indirizzo (Address) relativo all'addressName API-EnterpriseServiceBusAddress nel file CONFIG\WebApi.Client.Config.json (i)
Il valore deve essere recuperato dalla chiave di AppSettings "DocSuiteWeb.WebAPI.EnterpriseBus.Uri" con il seguente formato:
<valore chiave DocSuiteWeb.WebAPI.EnterpriseBus.Uri>/api/

ES: 
 {
	  "$type": "VecompSoftware.Clients.WebAPI.Configuration.BaseAddress, VecompSoftware.Clients.WebAPI",
	  "AddressName": "API-EnterpriseServiceBusAddress",
	  "Address": "http://sviluppo:81/EnterpriseBus/api/",
	  "NetworkCredential": {
		"$type": "VecompSoftware.Clients.WebAPI.Configuration.Credential, VecompSoftware.Clients.WebAPI"
  }

Successivamente è possibile rimuovere le 2 chiavi di AppSettings "DocSuiteWeb.WebAPI.EnterpriseBus.Uri" e "DocSuiteWeb.WebAPI.EnterpriseBus.API.CQRS".

AC.
######################################################
I file .css sono stati spostati dalla directory "App_Themes" alla directory "Content". La directory "App_Themes" deve quindi avere la seguente struttura:

- DocSuite2008
	- images (cartella)
	- imgset16 (cartella)
	- imgset32 (cartella)
	- DocSuite2008.skin

Tutti gli altri file presenti (.css) devono essere eliminati.
Nella directory "Content" devono essere presenti i nuovi file .css dal setup di installazione, se non sono presenti contattare Sviluppo.

AC.
#######################################################

L'esecuzione degli script ForeignKey.sql può generare errori di esecuzione (non compromettono la stabilità dell'aggiornamento o del database).
Qualora l'esecuzione generasse problemi contattare sviluppo per la correttiva.

SDC.
#####################################################