ATTENZIONE LA BUILD RICHIEDE IL FRAMEWORK .NET 4.8. 
PRIMA DI PROCEDERE ALL'INSTALLAZIONE E' NECESSARIO CANCELLARE IL CONTENUTO DELLA CARTELLA BIN

#FL
######################################################################################################

Per ottimizzare i processi di migrazioni e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - EnterpriseLibrary.Logging.config
 - Receiver.appSettings.config
 - Receiver.connectionStrings.config
 - Receiver.system.servicemodel.behaviors.config
 - Receiver.system.servicemodel.bindings.config
 - Receiver.system.servicemodel.client.config
 - WebApi.Client.Config.Addresses.json
 - SignatureTemplate.xml

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#FL
######################################################################################################

Per far funzionare il modulo UDS è necessario installare i seguenti prodotti:

Microsoft® System CLR Types for Microsoft® SQL Server® 2012
http://go.microsoft.com/fwlink/?LinkID=239644&clcid=0x409

Microsoft® SQL Server® 2012 Shared Management Objects 
http://go.microsoft.com/fwlink/?LinkID=239659&clcid=0x409


#FL
######################################################################################################

DALLA VERSIONE 9.03 NON é PIU' NECESSARIO INSTALLARE IL PRODOTTO "Microsoft Build Tools 2015".
SUI SERVER DI PRODUZIONE E' NECESSARIO DISINSTALLARLO IN MODO DA NON COMPROMETTERE LA STABILITA' DI PRODOTTO.

E' necessario installare la nuova componente Build Tools per Visual Studio 2019
https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=BuildTools&rel=16

Inoltre è necessario essere certi che sia installato il .NET Framework Developer Packs 4.8
https://aka.ms/msbuild/developerpacks

#FL
######################################################################################################

I seguenti moduli , non sono più supportati e vanno dunque disinstallati.

	- Workflow.Listener.Dematerialisation
	- Workflow.Listener.SecureDocument
	- Workflow.Listener.SecurePaper

#FL
######################################################################################################