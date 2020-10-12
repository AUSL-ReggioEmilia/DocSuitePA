/****************************************************************************************************************************************
*	Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)		*
*	<CurrentTenantId, uniqueidentifier,>  --> Settare il valore del tenantid recuperato dal parametro TenantModel di ParameterEnv.      *
*	<CurrentTenantAOOId, uniqueidentifier,>  --> Settare il valore del tenantAOOid														*
*	<CurrentTenantName, nvarchar(10),>  --> Settare il valore del tenantname recuperato dal parametro TenantModel di ParameterEnv.     *
*	<DBProtocollo, varcahr(50), DBProtocollo>  --> Settare il nome del DB di protocollo.				        						*
*	<DBPratiche, varcahr(50), DBPratiche>  --> Se esiste il DB di Pratiche settare il nome.					    					*
*	<DBAtti, varcahr(50), DBAtti>			   --> Se esiste il DB di Atti settare il nome.												*
*	<ESISTE_DB_ATTI, varchar(4), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva		*
*	<ESISTE_DB_PRATICHE, varchar(4), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva*
*****************************************************************************************************************************************/

declare @CurrentTenantId as UNIQUEIDENTIFIER = '<CurrentTenantId, uniqueidentifier,>'
declare @CurrentTenantName as NVARCHAR(256) = '<CurrentTenantName, nvarchar(10),>'
declare @CurrentCorporateName as NVARCHAR(256) = '<CurrentTenantName, nvarchar(256),>'

declare @CurrentTenantAOOId as UNIQUEIDENTIFIER = '<CurrentTenantAOOId, uniqueidentifier,>'

--Gestione tenant
INSERT INTO [dbo].[TenantAOO]([IdTenantAOO],[Name],[Note],[RegistrationUser],[RegistrationDate])
     VALUES(@CurrentTenantAOOId,'AOO default',NULL,'SYSTEM',GETDATE());

INSERT INTO [dbo].[Tenants]([IdTenant],[TenantName],[CompanyName],[StartDate],[EndDate]
           ,[Note],[RegistrationUser],[RegistrationDate],[IdTenantAOO])
     VALUES (@CurrentTenantId,@CurrentTenantName,@CurrentCorporateName
           ,GETUTCDATE(),NULL,NULL,'SYSTEM'
           ,GETUTCDATE(),@CurrentTenantAOOId);

UPDATE [dbo].[Tenants] SET IdTenantAOO = @CurrentTenantAOOId
WHERE IdTenantAOO IS NULL;

ALTER TABLE [dbo].[Tenants]  WITH CHECK ADD CONSTRAINT [FK_Tenants_TenantAOO] FOREIGN KEY([IdTenantAOO])
REFERENCES [dbo].[TenantAOO] ([IdTenantAOO]);

INSERT INTO [dbo].[TenantRoles] ([IdTenantRole],[IdTenant],[EntityShortId],[RegistrationDate])
SELECT NEWID(), @CurrentTenantId, idRole,GETUTCDATE()
FROM [dbo].[Role]
WHERE TenantId = @CurrentTenantId AND isActive = 1;

INSERT INTO [dbo].[TenantContainers]([IdTenantContainer],[IdTenant],[EntityShortId],[RegistrationDate])
SELECT NEWID(), @CurrentTenantId, idContainer,GETUTCDATE()
FROM [dbo].[Container]
WHERE isActive = 1;

INSERT INTO [dbo].[TenantContacts]([IdTenantContact],[IdTenant],[EntityId],[RegistrationDate])
SELECT NEWID(), @CurrentTenantId, Incremental,GETUTCDATE()
FROM [dbo].[Contact]
WHERE isActive = 1;

--Gestione UserLog
UPDATE [dbo].[UserLog] SET CurrentTenantId = @CurrentTenantId;
ALTER TABLE [dbo].[UserLog] ALTER COLUMN CurrentTenantId UNIQUEIDENTIFIER NOT NULL;

ALTER TABLE [dbo].[UserLog]  WITH CHECK ADD CONSTRAINT [FK_UserLog_Tenants] FOREIGN KEY([CurrentTenantId])
REFERENCES [dbo].[Tenants] ([IdTenant]);

ALTER TABLE [dbo].[UserLog] CHECK CONSTRAINT [FK_UserLog_Tenants];

--Gestione Protocol
UPDATE [dbo].[Protocol] SET IdTenantAOO = @CurrentTenantAOOId;

ALTER TABLE [dbo].[Protocol]  WITH CHECK ADD CONSTRAINT [FK_Protocol_TenantAOO] FOREIGN KEY([IdTenantAOO])
REFERENCES [dbo].[TenantAOO] ([IdTenantAOO]);

ALTER TABLE [dbo].[Protocol] CHECK CONSTRAINT [FK_Protocol_TenantAOO];

--Gestione ProtocolJournalLog
UPDATE [dbo].[ProtocolJournalLog] SET IdTenantAOO = @CurrentTenantAOOId;

ALTER TABLE [dbo].[ProtocolJournalLog]  WITH CHECK ADD CONSTRAINT [FK_ProtocolJournalLog_TenantAOO] FOREIGN KEY([IdTenantAOO])
REFERENCES [dbo].[TenantAOO] ([IdTenantAOO]);

ALTER TABLE [dbo].[ProtocolJournalLog] CHECK CONSTRAINT [FK_ProtocolJournalLog_TenantAOO];

--Gestione DocumentUnits
UPDATE [cqrs].[DocumentUnits] SET IdTenantAOO = @CurrentTenantAOOId;

ALTER TABLE [cqrs].[DocumentUnits]  WITH CHECK ADD CONSTRAINT [FK_DocumentUnits_TenantAOO] FOREIGN KEY([IdTenantAOO])
REFERENCES [dbo].[TenantAOO] ([IdTenantAOO]);

ALTER TABLE [cqrs].[DocumentUnits] CHECK CONSTRAINT [FK_DocumentUnits_TenantAOO];

--Gestione Parameter
UPDATE <DBProtocollo, varcahr(50), DBProtocollo>.[dbo].[Parameter] SET IdTenantAOO = @CurrentTenantAOOId;

IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
BEGIN 
    UPDATE <DBAtti, varcahr(50), DBAtti>.[dbo].[Parameter] SET IdTenantAOO = @CurrentTenantAOOId;
END

IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
BEGIN 
    UPDATE <DBPratiche, varcahr(50), DBPratiche>.[dbo].[Parameter] SET IdTenantAOO = @CurrentTenantAOOId;
END

ALTER TABLE [dbo].[Parameter]  WITH CHECK ADD CONSTRAINT [FK_Parameter_TenantAOO] FOREIGN KEY([IdTenantAOO])
REFERENCES [dbo].[TenantAOO] ([IdTenantAOO]);

ALTER TABLE [dbo].[Parameter] CHECK CONSTRAINT [FK_Parameter_TenantAOO];

--Gestione Category
UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Category] SET IdTenantAOO = @CurrentTenantAOOId;

IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
BEGIN 
    UPDATE <DBAtti, varcahr(50), DBAtti>.[dbo].[Category] SET IdTenantAOO = @CurrentTenantAOOId;
END

IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
BEGIN 
    UPDATE <DBPratiche, varcahr(50), DBPratiche>.[dbo].[Category] SET IdTenantAOO = @CurrentTenantAOOId;
END

ALTER TABLE [dbo].[Category] WITH CHECK ADD CONSTRAINT [FK_Category_TenantAOO] FOREIGN KEY([IdTenantAOO]) 
REFERENCES [dbo].[TenantAOO]([IdTenantAOO]);

ALTER TABLE [dbo].[Category] CHECK CONSTRAINT [FK_Category_TenantAOO];