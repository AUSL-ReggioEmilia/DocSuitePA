#########################################
Per ottimizzare i processi di migrazioni e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - EnterpriseLibrary.Logging.config
 - Receiver.appSettings.config
 - Receiver.connectionStrings.config
 - Receiver.system.servicemodel.behaviors.config
 - Receiver.system.servicemodel.bindings.config
 - Receiver.system.servicemodel.client.config
 - WebApi.Client.Config.Addresses.json
 - WebApi.Client.Config.Endpoints.json
 - SignatureTemplate.xml

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#FL
###############################################################

E' stato modificato il file WebApi.Client.Config.Endpoints.json. Assicurarsi di copiarlo in produzione.

#FL
#####################################################################################


E' importante aggiornare le DLL contenute nel file "Missed UDS Libs.zip" nei listeners delle UDS (dentro la cartella linstener e non nella root. Se presente eliminarli manualmente)

#FL
#####################################################################################

Assicurarsi che il file SignatureTemplate.xml sia presente nella root del servizio in produzione.

#AC
#####################################################################################

E' stato introdotto un nuovo endpoint di configurazione nel file Receiver.system.servicemodel.client.config relativo a StampaConforme.

<endpoint address="http://localhost/StampaConforme/BiblosDSConv.asmx" binding="basicHttpBinding" 
	bindingConfiguration="BasicHttpBinding_BiblosDSConvSoap" contract="StampaConforme.BiblosDSConvSoap" 
	name="BasicHttpBinding_BiblosDSConvSoap" />

Assicurarsi di impostare il nuovo endpoint nella configurazione del servizio come da file di installazione impostando l'url corretto alla StampaConforme 
del cliente.

#AC
#####################################################################################

E' stato introdotto un nuovo binding di configurazione nel file Receiver.system.servicemodel.bindings.config relativo a StampaConforme.

<binding name="BasicHttpBinding_BiblosDSConvSoap" transferMode="Streamed" maxReceivedMessageSize="2147483647" maxBufferSize="2147483647">
      <readerQuotas maxDepth="128" maxStringContentLength="2147483647" maxArrayLength="2147483647" />
      <security mode="None" />
</binding>

Assicurarsi di impostare il nuovo binding nella configurazione del servizio come da file di installazione.

#AC
#####################################################################################