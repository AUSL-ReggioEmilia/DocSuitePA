La soluzione BiblosDS è stata aggiornata alla versione 4.6.1 del .NET Framework.
Assicurarsi che sul server sia installata la versione indicata del Framework.

#AC
#############################################
Aggiornare il file web.config dell'applicazione WCF modificando la seguente sezione contenente i riferimenti aggiornati al .NET Framework 4.6.1:
  <system.web>
    <httpRuntime maxRequestLength="2147483647" useFullyQualifiedRedirectUrl="true"/>
    <compilation debug="true" targetFramework="4.6.1"/>
    <authentication mode="Forms"/>
    <globalization culture="it-IT" uiCulture="it-IT"/>
    <pages controlRenderingCompatibilityVersion="4.0"/>
  </system.web>

#AC
#############################################