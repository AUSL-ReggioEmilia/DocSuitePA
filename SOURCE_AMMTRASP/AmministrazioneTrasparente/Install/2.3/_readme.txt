Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WSSeries 8.75

#AC
######################################################################################################
E' stato rimosso il parametro di AppSetting ShowPriorityGrid e sostituito con il nuovo parametro SeriesPreviewBehaviours (impostabile dalla sezione Admin)
che permette di indicare le varie tipologie di visualizzazione delle serie documentali nella pagina di ricerca.
Il parametro permette di abilitare le seguenti tipologie:
	- PriorityEnabled = Corrisponde al vecchio parametro ShowPriorityGrid, abilitare se dal cliente ShowPriorityGrid = true.
	- LatestSeriesEnabled = Corrisponde alla visualizzazione delle ultime serie lavorate.

Es. di configurazione:
	{ "PriorityEnabled": true, "LatestSeriesEnabled": false }

#AC
######################################################################################################