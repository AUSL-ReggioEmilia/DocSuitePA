# VecompSoftware.DocSuite.SPID.AccessoAtti #
Portale di accesso agli atti.

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
  "ClientConfiguration": {
    "APIAddress": "http://localhost/DSW.PublicWebAPI/api",
    "ODATAAddress": "http://localhost/DSW.PublicWebAPI/ODATA",
    "ArchiveName": "Accesso agli atti",
    "WorkflowName": "SPID Accesso agli Atti",
    "TenantId": "C0A21F9F-49D4-4701-9863-B7FC2B16493B",
    "TenantName": "BCOM",
    "ExternalViewerBaseUrl": "http://localhost/ExternalViewer/#/Fascicolo/identificativo"
  },
  "JwtConfiguration": {
    "Issuer": "https://localhost:44355",
    "SecretKeyStoragePath": "C:\\SecretKeyFolder",
    "ProtectionType": 1,
    "X509CertificateThumbprint": ""
  }
}

## Specifiche ClientConfiguration ##
- APIAddress (Indirizzo API Public per la parte REST).
- ODATAAddress (Indirizzo API Public per la parte ODATA).
- ArchiveName (Nome dell'archivio per l'accesso agli atti).
- WorkflowName (Nome del workflow per l'accesso agli atti).
- ExternalViewerBaseUrl (Url dell'externalviewer per la visualizzazione del Fascicolo).

## Specifiche JwtConfiguration ##
- Issuer (dominio dell'applicazione).
- SecretKeyStoragePath (percorso locale o di rete dove salvare le key generate per la criptatura delle informazioni di autenticazione).
- ProtectionType (WindowsDpapi = 1, X509Certificate = 2. Se = 1 utilizzare solamente un percorso locale nel parametro SecretKeyStoragePath).
- X509CertificateThumbprint (Solo se ProtectionType = 2, indicare il thumbprint del certificato da utilizzare).
