Per poter far funzionare le WebAPI 8.52 è necessario andare nelle configurazioni del site che ospite le WebAPI e impostare quanto segue:

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
################################

E' stato aggiunto un nuovo parametro nel file WebApi.AppSettings.config che deve riportare il valore del TenantID della DocSuite.
In situazione di multidominio il valore da riportare è il CurrentTenantId.

<add key="VecompSoftware.DocSuiteWeb.TenantId" value="<valore guid uguale al current tenant della DocSuite>" />

#FL
################################

Modificare la validation dei fascicoli con il tool Enterprise Library (decomprimere il file EntLib6.zip) nel modo seguente:

solo per [AUSL-PC], per l'entità FascicleProtocolValidator con ruleset FascicleInsert, 
					aggiungere il custom validator 'IsProtocolAssigned' alla proprietà Self.

#SDC
###############################################################
