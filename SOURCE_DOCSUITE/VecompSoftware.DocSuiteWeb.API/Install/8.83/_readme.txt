######################################################################################################
Per ottimizzare i processi di migrazione e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - docsuite.appsettings.config
 - docsuite.connectionstrings.config
 - docsuite.system.servicemodel.behaviors.config
 - docsuite.system.servicemodel.bindings.config
 - docsuite.system.servicemodel.client.config
 - docsuite.system.servicemodel.services.config

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#AC
######################################################################################################