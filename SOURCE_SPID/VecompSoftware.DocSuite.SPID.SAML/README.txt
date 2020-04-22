# VecompSoftware.DocSuite.SPID.SAML #
Servizio di gestione Request/Response SAML.

-------------------

# Prerequisiti #
.NET Core 2.0

-------------------

# Utilizzo #
Per utilizzare tale progetto nella propria applicazione abilitare il servizio in fase di Startup tramite:
---
services.AddSaml(IdpType);

Esempio di configurazione:
---
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddSaml(idpType);
	...
}

-------------------

# Logging #
Viene utilizzato il Logger con CategoryName = "Saml".