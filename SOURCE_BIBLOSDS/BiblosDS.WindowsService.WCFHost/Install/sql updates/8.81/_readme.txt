Per ottimizzare i processi di migrazione e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - BiblosDS.WindowsService.WCFHost.exe.config

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#AC
#####################################################################################
E' stato aggiunto un nuovo parametro di configurazione "WebAPIUrl" che indica l'indirizzo HTTP
utilizzato per esporre le nuove API.
E' necessario quindi impostare il parametro nella sezione appSettings nel seguento modo:

	<add key="WebAPIUrl" value="<Indirizzo API>" />

N.B. Si consiglia di esporre tale servizio sempre in localhost in quanto non esposto su IIS.

#AC
##############################################################################################################