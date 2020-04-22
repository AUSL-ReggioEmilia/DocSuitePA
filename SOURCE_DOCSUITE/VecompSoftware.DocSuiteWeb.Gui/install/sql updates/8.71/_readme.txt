Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- JeepService 8.71 
		- Modulo PosteWeb
		- Modulo ResolutionWorkflowManager
	- ApiLegacy 8.71

#FL
######################################################################################################
Refactor PosteWeb

Da attivare per tutti i clienti che hanno attive PosteWeb
Necessità di avere una location in DSW che punti ad un archivio standard in Biblos in cui salvare gli stream dei documenti delle PosteWeb.
Similmente a Collaborazioni è sato definito in ProtocolEnv il parametro PosteWebRequestLocation dove value="<Il valore ID della tabella location deve essere corrispondente all'archivio della gestione delle PosteWeb> " />
In fase di installazione occorre anche lanciare il tool di migrazione PosteWebMigrator per salvare in biblos i documenti attualmente esistenti.
Per un corretto funzionamento del tool di migrazione, occorre che venga lanciato dopo aver aggiornato la DocSuite 8.71 e aver impostato una Location nel parametro PosteWebRequestLocation.

#MM
######################################################################################################

Nel modulo ResolutionWorkflowManager del JeepService sono stati rimossi i parametri WebPublishNameFile, WebPublishSign e WebPublishSignTag
quindi va aggiornato il modulo per tutti i clienti che lo hanno attivo.

#SDC
######################################################################################################