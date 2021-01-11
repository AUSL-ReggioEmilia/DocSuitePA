# VecompSoftware.DocSuite.SPID.Web #
Motore WEB di autenticazione tramite SAML.

-------------------

# Prerequisiti #
ASP.NET Core 2.0

-------------------

# Utilizzo #
Per utilizzare tale progetto nella propria applicazione abilitare il servizio in fase di Startup tramite:
---
services.AddSAMLAuth(...);

Successivamente abilitare gli elementi embedded.
Esempio di configurazione:
---
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddSAMLAuth(this.Configuration);
	//View embedded
	Assembly externalAssembly = typeof(SamlController).GetTypeInfo().Assembly;
    EmbeddedFileProvider embeddedFileProvider = new EmbeddedFileProvider(externalAssembly, "VecompSoftware.DocSuite.SPID.AuthEngine");
	services.Configure<RazorViewEngineOptions>(options =>
    {
		options.FileProviders.Add(embeddedFileProvider);
    });
	...
}

# Authenticazione #
La SAML Response valida genera automaticamente un token JWT di autenticazione. E' necessario quindi abilitare l'autenticazione tramite JWTBearer nell'applicazione.
Esempio di configurazione:
---
public void ConfigureServices(IServiceCollection services)
{
	...
	services.AddSAMLAuth(this.Configuration);
	ServiceProvider sp = services.BuildServiceProvider();
	services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, cfg =>
                {
                    SecurityKeyFactory securityKeyFactory = sp.GetRequiredService<SecurityKeyFactory>();
                    IJwtFactory jwtFactory = sp.GetService<IJwtFactory>();
                    SymmetricSecurityKey signingKey = securityKeyFactory.GetSymmetricSecurityKey();

                    cfg.RequireHttpsMetadata = true;
                    cfg.SaveToken = true;

                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtAppSettings[nameof(JwtConfiguration.Issuer)],

                        ValidateAudience = true,
                        ValidAudience = jwtAppSettings[nameof(JwtConfiguration.Issuer)],

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
	...
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
	...
	app.UseAuthentication();
	app.UseMvc(...
	...
}

-------------------

# Configurazione #
Il progetto necessita la definizione di 2 applicationsettings distinti:
1) JwtConfiguration (configurazione per la creazione del token di autenticazione per l'applicazione):
	- Issuer (dominio dell'applicazione).
	- SecretKeyStoragePath (percorso locale o di rete dove salvare le key generate per la criptatura delle informazioni di autenticazione).
	- ProtectionType (WindowsDpapi = 1, X509Certificate = 2. Se = 1 utilizzare solamente un percorso locale nel parametro SecretKeyStoragePath).
	- X509CertificateThumbprint (Solo se ProtectionType = 2, indicare il thumbprint del certificato da utilizzare).
	
2) SPIDConfiguration (configurazione generica del progetto):
	- IdpAuthLevel (Valori possibili 1,2,3. Livello di autenticazione IDP. Vedi http://spid-regole-tecniche.readthedocs.io/en/latest/regole-tecniche-idp.html#requestedauthncontext. N.B. In produzione impostare livello 2 o superiore).
	- SPDomain (EntityId/Dominio del Service Provider fornito a SPID o FedERa tramite il Metadata generato).
	- AssertionConsumerServiceIndex (Indice posizionale facente riferimento ad uno degli elementi <AttributeConsumerService> presenti nei metadata del Service Provider. Vedi http://spid-regole-tecniche.readthedocs.io/en/latest/regole-tecniche-idp.html#id1).
	- AttributeConsumingServiceIndex (Indice posizionale in riferimento alla struttura <AttributeConsumingService> presente nei metadata del Service Provider. Vedi http://spid-regole-tecniche.readthedocs.io/en/latest/regole-tecniche-idp.html#id1).
	- CertificateThumbprint (Solo se CertificateFromFile = false. Thumbprint del certificato in CurrentUser/Personal).
	- CertificatePassword (Password del certificato).
	- CertificateFromFile (Booleano, indica se il certificato deve essere letto da file o dal certificate store).
	- CertificatePath (Solo se CertificateFromFile = true. Percorso dove leggere il certificato).
	- CertificatePrivateKey (Opzionale, XML RSAParameters del certificato).
	- ACSCallback (Callback url di autenticazione fornito all'Identity Provider).
	- LogoutCallback (Callback url a cui reindirizzare l'applicazione dopo la procedura di logout).
	- IdpType (Valori possibili 0,1. Se 0 abilita lo SPID, se 1 abilita FedERa).

-------------------

# Logging #
Viene utilizzato il Logger con CategoryName = "AuthEngine".

