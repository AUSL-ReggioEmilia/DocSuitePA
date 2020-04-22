
Nel modulo Dematerialisation, il file module.configuration.json va configurato nel modo seguente:

	1. Topic_Workflow_Integration -> nome della topic nella quale arrivano gli eventi
	2. Subscription_Dematerialisation -> nome della sottoscrizione specifica dalla quale si leggono gli eventi
	3. Topic_UDS -> nome topic dell'UDS
	4. ProtocolSignature -> segnatura del documento di attestazione di conformità nel caso di protocollo
	5. ResolutionSignature -> segnatura del documento di attestazione di conformità nel caso di atto
	6. TenantName -> proprietà TenantName configurata da parametro DocSuite 'TenantModel' 
	7. TenantId -> proprietà TenantId configurata da parametro DocSuite 'TenantModel'
	8. CorporateAcronym -> valore configurato da parametro DocSuite 'CorporateAcronym' 
#SDC
#################################################################################################