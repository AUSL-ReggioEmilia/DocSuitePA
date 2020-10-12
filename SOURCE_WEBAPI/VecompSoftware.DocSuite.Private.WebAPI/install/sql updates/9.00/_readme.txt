Per ottimizzare i processi di migrazioni e non compromettere le personalizzazione dei files di configurazione, non verranno più distribuiti nella cartella di root i seguenti files:
 - EnterpriseLibrary.Logging.config
 - EnterpriseLibrary.Validation.config
 - WebApi.appSettings.config
 - WebApi.connectionStrings.config
 - WebApi.system.servicemodel.behaviors.config
 - WebApi.system.servicemodel.bindings.config
 - WebApi.system.servicemodel.client.config

Come nuovo standard di sviluppo i files verranno sempre copiati nella cartella Install -> sql updates -> [Versioning] -> default_configurations.
Questi files devono considerarsi come master di default, utili ad assistenza per verificare eventuali anomalie o mancaze di produzionene.

#FL
#####################################################################################
Per poter far funzionare le WebAPI è necessario andare nelle configurazioni del site che ospite le WebAPI e impostare quanto segue:

	- Sezione IIS -> Authentication/Autenticazione : 
		* Anonymous Authentication -> Disable
		* Windows Authentication -> Enabled

	- Sezione Application Pools -> DSW.WebAPI : 
		* Enable 32-Bit Application : False

NB: in alcune configurazioni sistemistiche in cui le WebAPI sono installate nello stesso server della DSW, è possibile avere dei problemi di 'autenticazione'
dovuti al noto problema dei Loopback Check di DNS. Il problema si manifesta con un errore generico appena si prova ad accedere, per esempio, a una delle viste di scrivania/collaborazione. 
La risoluzione prevede l'esecuzione di questo script script powershell (vedi https://blogs.msdn.microsoft.com/richin/2012/02/07/using-powershell-to-disable-loopback-check/):

	New-ItemProperty HKLM:\System\CurrentControlSet\Control\Lsa -Name "DisableLoopbackCheck" -Value "1" -PropertyType dword


#FL
#####################################################################################
Assicurarsi che l'utente con cui le Web API si collegano a SQL, abbia diritti di EXECUTE per le Stored Procedure.
rif. https://msdn.microsoft.com/it-it/library/ms188371.aspx

N.B. Se l'utente di connessione a SQL è DB owner non è necessario verificare i diritti di EXECUTE.

#FL
#####################################################################################