E' necessario creare due nuove sottoscrizioni nella topic "workflow_integration" con le seguenti impostazioni:
	- Nome: "WorkflowStartSWAFNotification"
	  Rules: EventName='EventIntegrationRequest' AND WorkflowName='AUSL-PC - Portale - Notifica evento'
	- Nome: "WorkflowStartShareToSWAFNotification"
	  Rules: EventName='EventShareDocumentUnit' AND (WorkflowName='AUSL-PC - SWAF - Crea protocollo' OR WorkflowName='AUSL-PC - SWAF - Crea archivio' OR WorkflowName='AUSL-PC - Invia a SWAF')
	- Nome: "ForwardSWAFNotificationWorkflowCompleted"
	  Rules: WorkflowName='AUSL-PC - Invia a SWAF' OR WorkflowName='AUSL-PC - Portale - Notifica evento' OR WorkflowName='AUSL-PC - SWAF - Notifica evento'
	  Action: SET WorkflowAutoComplete = 1
	  ForwardTo: builder_event