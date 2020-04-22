# VecompSoftware.DocSuite.SPID.Portal #
Portale di accesso alle applicazioni aziendali tramite SPID o FedERa.

-------------------

# Prerequisiti #
ASP.NET Core 2.0
Node.js ultima versione (https://nodejs.org/it/)
Angular CLI ultima versione (https://cli.angular.io/)

# Prerequisiti server produzione #
Node.js ultima versione
.NET core 2.0 SDK
Per pubblicare in IIS installare il pacchetto Hosting .NET core Windows Server (https://aka.ms/dotnetcore.2.0.0-windowshosting)

-------------------

# Configurazione #
Esempio di configurazione:
{
  "Logging": {
    "IncludeScopes": false,
    "Debug": {
      "LogLevel": {
        "Default": "Debug",
        "System": "Information",
        "Microsoft": "Information"
      }
    },
    "Console": {
      "LogLevel": {
        "Default": "Debug",
        "System": "Information",
        "Microsoft": "Information"
      }
    }
  },
  "AuthConfiguration": {
    "IdpAuthLevel": 1,
    "SPDomain": "https://www.vecompsoftware.it",
    "AssertionConsumerServiceIndex": 0,
    "AttributeConsumingServiceIndex": 0,
    "CertificateThumbprint": "",
    "CertificatePassword": "",
    "CertificateFromFile": false,
    "CertificatePath": "",
    "CertificatePrivateKey": "",
    "ACSCallback": "https://localhost:44355/AuthenticationCallback",
    "IdpType": 0
  },
  "JwtConfiguration": {
    "Issuer": "https://localhost:44355",
    "SecretKeyStoragePath": "C:\\SecretKeyFolder",
    "ProtectionType": 1,
    "X509CertificateThumbprint": ""
  },
  "AllowedOrigins": [
    "http://localhost:49750"
  ]
}

## Specifiche AuthConfiguration ##
- IdpAuthLevel (Valori possibili 1,2,3. Livello di autenticazione IDP. Vedi http://spid-regole-tecniche.readthedocs.io/en/latest/regole-tecniche-idp.html#requestedauthncontext. N.B. In produzione impostare livello 2 o superiore).
- SPDomain (EntityId/Dominio del Service Provider fornito a SPID o FedERa tramite il Metadata generato).
- AssertionConsumerServiceIndex (Indice posizionale facente riferimento ad uno degli elementi <AttributeConsumerService> presenti nei metadata del Service Provider. Vedi http://spid-regole-tecniche.readthedocs.io/en/latest/regole-tecniche-idp.html#id1).
- AttributeConsumingServiceIndex (Indice posizionale in riferimento alla struttura <AttributeConsumingService> presente nei metadata del Service Provider. Vedi http://spid-regole-tecniche.readthedocs.io/en/latest/regole-tecniche-idp.html#id1).
- CertificateThumbprint (Solo se CertificateFromFile = false. Thumbprint del certificato in CurrentUser/Personal da utilizzare per firmare le richieste di autenticazione SAML).
- CertificatePassword (Password del certificato).
- CertificateFromFile (Booleano, indica se il certificato deve essere letto da file o dal certificate store).
- CertificatePath (Solo se CertificateFromFile = true. Percorso dove leggere il certificato da utilizzare per firmare le richieste di autenticazione SAML).
- CertificatePrivateKey (Opzionale, XML RSAParameters del certificato).
- ACSCallback (Callback url di autenticazione fornito all'Identity Provider).
- LogoutCallback (Callback url a cui reindirizzare l'applicazione dopo la procedura di logout).
- IdpType (Valori possibili 0,1. Se 0 abilita lo SPID, se 1 abilita FedERa).

## Specifiche JwtConfiguration ##
- Issuer (dominio dell'applicazione).
- SecretKeyStoragePath (percorso locale o di rete dove salvare le key generate per la criptatura delle informazioni di autenticazione).
- ProtectionType (WindowsDpapi = 1, X509Certificate = 2. Se = 1 utilizzare solamente un percorso locale nel parametro SecretKeyStoragePath).
- X509CertificateThumbprint (Solo se ProtectionType = 2, indicare il thumbprint del certificato da utilizzare).

-------------------

# Configurazione applicazioni disponibili (applications.json) #
Esempio di configurazione:
[
  {
    "Id": "4194b227-4b9f-4db7-9c2c-e195180bbc78",
    "Name": "Accesso civico agli atti",
    "Description": "<p>...</p>",
    "Url": "http://localhost:49750/Home/ExternalAuth"
  },
  {
    ...
]

## Specifiche ##
- Id (Identificativo univoco (Guid) dell'applicazione da gestire).
- Name (Nome dell'applicazione visualizzato come titole nel portale).
- Description (Descrizione dell'applicazione. E' possibile inserire tag HTML per la formattazione del testo).
- Url (Definisce l'url di ingresso dell'applicazione a cui passare il referenceCode per il recupero dell'utente collegato).

-------------------

# Configurazione identity providers (identityproviders.json) #
Esempio di configurazione:
[
  {
    "IdpCode": "idp_aruba",
    "SingleSignOnService": "https://loginspid.aruba.it/ServiceLoginWelcome",
    "SingleLogoutService": "https://loginspid.aruba.it/ServiceLogoutRequest"
  },
  {
    ...
]

## Specifiche ##
- IdpCode (Valori possibili "idp_aruba","idp_intesa","idp_infocert","idp_namirial","idp_poste","idp_register","idp_sielte","idp_tim","idp_federa". Codice identificativo dell'identity provider).
- SingleSignOnService (Url per l'autenticazione all'Identity provider. Recuperabile come informazione dal metadata.xml dell'Identity Provider stesso).
- SingleLogoutService (Url per il logout dall'Identity provider. Recuperabile come informazione dal metadata.xml dell'Identity Provider stesso).

-------------------