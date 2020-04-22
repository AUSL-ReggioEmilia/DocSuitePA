La soluzione StampaConforme è stata aggiornata alla versione 4.6.1 del .NET Framework.
Assicurarsi che sul server sia installata la versione indicata del Framework.

#AC
################################################
Sono state aggiornate le referenze al pacchetto di OpenOffice versione 4.1.5.
Aggiornare OpenOffice alla versione specificata prima di aggiornare il servizio.

#AC
################################################
Per un corretto aggiornamento disinstallare il servizio corrente e pulire il contenuto della directory.
Successivamente copiare il nuovo contenuto del pacchetto di installazione.

#AC
################################################
Per un corretto aggiornamento si consiglia di utilizzare il file BiblosDS.WindowsService.WCFStampaConformeConverterHost.exe.config 
allegato al pacchetto di installazione e successivamente modificare i valori di appSettings ed eventuali url con quelli precedenti all'installazione.

#AC
################################################
Procedura di installazione del servizio:
-	Eseguire Prompt di DOS con privilegi amministrativi.
-	Portarsi nel seguente percorso: “C:\Windows\Microsoft.NET\Framework\v4.0.30319”
-	Eseguire comando “InstallUtil.exe” con i seguenti parametri:  /i “<percorso servizio OpenOffice da installare>/<.exe servizio>”.
-	Eseguire il comando e verificare la corretta installazione del servizio.

#AC
################################################