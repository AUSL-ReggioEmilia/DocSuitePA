/****************************************************************************************************************************************
*	Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)		*
*	<DBProtocollo, varcahr(50), DBProtocollo>  --> Settare il nome del DB di protocollo.				        						*
*	<DBPratiche, varcahr(50), DBPratiche>  --> Se esiste il DB di Pratiche settare il nome.					    					*
*	<DBAtti, varcahr(50), DBAtti>			   --> Se esiste il DB di Atti settare il nome.												*
*	<ESISTE_DB_ATTI, varchar(4), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva		*
*	<ESISTE_DB_PRATICHE, varchar(4), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva*
*****************************************************************************************************************************************/

SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;
GO

IF (SELECT OBJECT_ID('tempdb..#tmpErrors')) IS NOT NULL DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
GO
BEGIN TRANSACTION
GO
--#############################################################################
PRINT 'Alter stored procedure [dbo].[Container_Insert]';
GO

ALTER PROCEDURE [dbo].[Container_Insert] 
       @Name nvarchar(1000),
       @Note varchar(2000), 
       @DocmLocation smallint,
       @ProtLocation smallint,
       @ReslLocation smallint,
       @isActive tinyint,
       @Massive tinyint,
       @Conservation smallint,
       @RegistrationDate datetimeoffset(7),
       @RegistrationUser nvarchar(256),
       @LastChangedDate datetimeoffset(7),
       @LastChangedUser nvarchar(256), 
       @DocumentSeriesAnnexedLocation smallint,
       @DocumentSeriesLocation smallint,
       @DocumentSeriesUnpublishedAnnexedLocation smallint,
       @ProtocolRejection tinyint,
       @ActiveFrom datetime,
       @ActiveTo datetime,
       @idArchive int,
       @Privacy tinyint,
       @HeadingFrontalino nvarchar(511),
       @HeadingLetter nvarchar(256),
       @ProtAttachLocation smallint,
       @idProtocolType smallint,
       @DeskLocation smallint,
       @UDSLocation smallint,
       @UniqueId uniqueidentifier,
       @AutomaticSecurityGroups tinyint,
       @PrefixSecurityGroupName nvarchar(256),
       @TenantId uniqueidentifier,
       @ContainerType smallint,
	   @SecurityUserAccount nvarchar(256),
	   @SecurityUserDisplayName nvarchar(256),
	   @ManageSecureDocument bit,
	   @PrivacyLevel int,
	   @PrivacyEnabled bit
AS 

       DECLARE @EntityShortId smallint, @LastUsedIdContainer smallint, @RightsFull nvarchar(20), @ResolutionRightsFull nvarchar(20), @DocumentRightsFull nvarchar(20), @DocumentSeriesRightsFull nvarchar(20), 
			   @DeskRightsFull nvarchar(20), @UDSRightsFull nvarchar(20),@RightsIns nvarchar(20), @ResolutionRightsIns nvarchar(20), @DocumentRightsIns nvarchar(20), @DocumentSeriesRightsIns nvarchar(20), 
			   @DeskRightsIns nvarchar(20), @UDSRightsIns nvarchar(20),@RightsVis nvarchar(20), @ResolutionRightsVis nvarchar(20), @DocumentRightsVis nvarchar(20), @DocumentSeriesRightsVis nvarchar(20), 
			   @DeskRightsVis nvarchar(20), @UDSRightsVis nvarchar(20), @SecurityUserName nvarchar(100), @SecurityUserDescription nvarchar(100), @SecurityUserDomain nvarchar(100), @SecurityGroupIdFull int, 
			   @SecurityGroupIdVis int, @SecurityGroupIdIns int, @SecurityGroupName nvarchar(256)


	SELECT  @LastUsedIdContainer = [LastUsedIdContainer] FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Parameter]

       --Ad oggi è implementata solo la gestione dei diritti degli archivi,
       --Per ottenere un corretto funzionamento con le altre UD occorre implementarne la gestione
       --diritti protocollo

       SET @RightsFull = 
             CASE 
                    WHEN @ContainerType = 1 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @RightsIns = 
             CASE 
                    WHEN @ContainerType = 1 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @RightsVis = 
             CASE 
                    WHEN @ContainerType = 1 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

             --diritti atti

       SET @ResolutionRightsFull = 
             CASE 
                    WHEN @ContainerType = 2 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @ResolutionRightsIns = 
             CASE 
                    WHEN @ContainerType = 2 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @ResolutionRightsVis = 
             CASE 
                    WHEN @ContainerType = 2 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

             --diritti pratiche

       SET @DocumentRightsFull = 
             CASE 
                    WHEN @ContainerType = 4 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DocumentRightsIns = 
             CASE 
                    WHEN @ContainerType = 4 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DocumentRightsVis = 
             CASE 
                    WHEN @ContainerType = 4 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END           

             --diritti serie documentali

       SET @DocumentSeriesRightsFull = 
             CASE 
                    WHEN @ContainerType = 8 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DocumentSeriesRightsIns = 
             CASE 
                    WHEN @ContainerType = 8 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DocumentSeriesRightsVis = 
             CASE 
                    WHEN @ContainerType = 8 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END    

             --diritti tavoli

       SET @DeskRightsFull = 
             CASE 
                    WHEN @ContainerType = 16 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DeskRightsIns = 
             CASE 
                    WHEN @ContainerType = 16 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @DeskRightsVis = 
             CASE 
                    WHEN @ContainerType = 16 THEN '00000000000000000000'
                    ELSE '00000000000000000000'
             END    

             -- diritti UDS
             
       SET @UDSRightsFull = 
             CASE 
                    WHEN @ContainerType = 32 THEN '11111111000000000000'
                    ELSE '00000000000000000000'
             END

       SET @UDSRightsIns = 
             CASE 
                    WHEN @ContainerType = 32 THEN '11111000000000000000'
                    ELSE '00000000000000000000'
             END

       SET @UDSRightsVis = 
             CASE 
                    WHEN @ContainerType = 32 THEN '00110000000000000000'
                    ELSE '00000000000000000000'
             END    

	   IF(@SecurityUserAccount IS NOT NULL AND @SecurityUserAccount != '')
			BEGIN			
			SELECT TOP 1 @SecurityUserDomain = Value FROM dbo.SplitString(@SecurityUserAccount, '\')
			SELECT TOP 1 @SecurityUserName = Value FROM (SELECT Value, row_number() over(order by (select null)) as rownum FROM dbo.SplitString(@SecurityUserAccount, '\')) T where T.rownum > 1 AND T.rownum <= 2
			END

       SET @EntityShortId = @LastUsedIdContainer + 1

       SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
       BEGIN TRY
       BEGIN TRANSACTION ContainerInsert
       UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Parameter] SET [LastUsedidContainer] = @EntityShortId

       --Inserimento contenitore
       INSERT INTO <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Container]([idContainer], [Name], [Note], [DocmLocation], [ProtLocation], [ReslLocation], [isActive], [Massive], [Conservation],
             [RegistrationUser], [RegistrationDate], [DocumentSeriesAnnexedLocation], [DocumentSeriesLocation], 
             [DocumentSeriesUnpublishedAnnexedLocation], [ProtocolRejection], [ActiveFrom], [ActiveTo], [idArchive], [Privacy], [HeadingFrontalino], [HeadingLetter],
             [ProtAttachLocation], [idProtocolType], [DeskLocation], [UDSLocation], [UniqueId], [ManageSecureDocument], [PrivacyLevel], [PrivacyEnabled]) 
             
       VALUES(@EntityShortId, @Name, @Note, @DocmLocation, @ProtLocation,@ReslLocation, @isActive, @Massive, @Conservation,   @RegistrationUser, @RegistrationDate, @DocumentSeriesAnnexedLocation, @DocumentSeriesLocation, @DocumentSeriesUnpublishedAnnexedLocation, @ProtocolRejection, @ActiveFrom,
             @ActiveTo, @idArchive, @Privacy, @HeadingFrontalino, @HeadingLetter, @ProtAttachLocation, @idProtocolType, @DeskLocation, @UDSLocation, @UniqueId, @ManageSecureDocument, @PrivacyLevel, @PrivacyEnabled)

	   IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
       BEGIN
             update  <DBAtti, varchar(50), DBAtti>.[dbo].[Parameter] set [LastUsedidContainer] = @EntityShortId
             --Inserimento contenitore
             INSERT INTO  <DBAtti, varchar(50), DBAtti>.[dbo].[Container]([idContainer], [Name], [Note], [DocmLocation], [ProtLocation], [ReslLocation], [isActive], [Massive], [Conservation],
                    [RegistrationUser], [RegistrationDate], [DocumentSeriesAnnexedLocation], [DocumentSeriesLocation], 
                    [DocumentSeriesUnpublishedAnnexedLocation], [ProtocolRejection], [ActiveFrom], [ActiveTo], [idArchive], [Privacy], [HeadingFrontalino], [HeadingLetter],
                    [ProtAttachLocation], [idProtocolType], [DeskLocation], [UDSLocation], [UniqueId], [ManageSecureDocument], [PrivacyLevel], [PrivacyEnabled])  
             
             VALUES(@EntityShortId, @Name, @Note, @DocmLocation, @ProtLocation,@ReslLocation, @isActive, @Massive, @Conservation,@RegistrationUser, @RegistrationDate, 
					@DocumentSeriesAnnexedLocation, @DocumentSeriesLocation, @DocumentSeriesUnpublishedAnnexedLocation, @ProtocolRejection, @ActiveFrom,
                    @ActiveTo, @idArchive, @Privacy, @HeadingFrontalino, @HeadingLetter, @ProtAttachLocation, @idProtocolType, @DeskLocation, @UDSLocation, @UniqueId, @ManageSecureDocument, @PrivacyLevel, @PrivacyEnabled)

       END

	   IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
       BEGIN 
             update <DBPratiche, varchar(50), DBPratiche>.[dbo].[Parameter] set [LastUsedidContainer] = @EntityShortId

             --Inserimento contenitore
             INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[Container]([idContainer], [Name], [Note], [DocmLocation], [ProtLocation], [ReslLocation], [isActive], [Massive], [Conservation],
                    [RegistrationUser], [RegistrationDate], [DocumentSeriesAnnexedLocation], [DocumentSeriesLocation], 
                    [DocumentSeriesUnpublishedAnnexedLocation], [ProtocolRejection], [ActiveFrom], [ActiveTo], [idArchive], [Privacy], [HeadingFrontalino], [HeadingLetter],
                    [ProtAttachLocation], [idProtocolType], [DeskLocation], [UDSLocation], [UniqueId], [ManageSecureDocument], [PrivacyLevel], [PrivacyEnabled]) 
             
             VALUES(@EntityShortId, @Name, @Note, @DocmLocation, @ProtLocation,@ReslLocation, @isActive, @Massive, @Conservation, @RegistrationUser, @RegistrationDate, 
					@DocumentSeriesAnnexedLocation, @DocumentSeriesLocation, @DocumentSeriesUnpublishedAnnexedLocation, @ProtocolRejection, @ActiveFrom,
                    @ActiveTo, @idArchive, @Privacy, @HeadingFrontalino, @HeadingLetter, @ProtAttachLocation, @idProtocolType, @DeskLocation, @UDSLocation, @UniqueId, @ManageSecureDocument, @PrivacyLevel, @PrivacyEnabled)

        END

	   IF(@AutomaticSecurityGroups = 1)
       BEGIN 
             --inserimento  gruppo con tutti i diritti
			 SET @SecurityGroupName = @PrefixSecurityGroupName + '_full'
             EXEC @SecurityGroupIdFull = [dbo].[SecurityGroups_Insert] @SecurityGroupName, null, null,  @RegistrationUser, @RegistrationDate, null, null, null, @TenantId, null, 0, null 
             EXEC [dbo].[ContainerGroups_Insert] @EntityShortId,@RightsFull, @SecurityGroupName, @ResolutionRightsFull, @DocumentRightsFull, @RegistrationUser, @RegistrationDate, @DocumentSeriesRightsFull, @SecurityGroupIdFull, @DeskRightsFull, @UDSRightsFull, @PrivacyLevel, '00000000000000000000'
             
			 --inserimento  gruppo con diritti di inserimento
			 SET @SecurityGroupName = @PrefixSecurityGroupName + '_ins'
             EXEC @SecurityGroupIdIns = [dbo].[SecurityGroups_Insert] @SecurityGroupName, null, null,  @RegistrationUser, @RegistrationDate, null, null, null, @TenantId, null, 0, null 
			 EXEC [dbo].[ContainerGroups_Insert] @EntityShortId,@RightsIns, @SecurityGroupName, @ResolutionRightsIns, @DocumentRightsIns, @RegistrationUser, @RegistrationDate, @DocumentSeriesRightsIns, @SecurityGroupIdIns, @DeskRightsIns, @UDSRightsIns, @PrivacyLevel, '00000000000000000000'
             
			 --inserimento gruppo con diritti di visualizzazione
			 SET @SecurityGroupName = @PrefixSecurityGroupName + '_vis'
             EXEC @SecurityGroupIdVis = [dbo].[SecurityGroups_Insert] @SecurityGroupName, null, null,  @RegistrationUser, @RegistrationDate, null, null, null, @TenantId, null, 0, null 
             EXEC [dbo].[ContainerGroups_Insert] @EntityShortId,@RightsVis, @SecurityGroupName, @ResolutionRightsVis, @DocumentRightsVis, @RegistrationUser, @RegistrationDate, @DocumentSeriesRightsVis, @SecurityGroupIdVis, @DeskRightsVis, @UDSRightsVis, @PrivacyLevel, '00000000000000000000'
       
			IF(@SecurityUserAccount IS NOT NULL AND @SecurityUserDomain IS NOT NULL AND @SecurityUserName IS NOT NULL)
			BEGIN			

			EXEC [dbo].[SecurityUsers_Insert] @SecurityUserName, @SecurityUserDisplayName, @SecurityUserDomain, @RegistrationUser, @RegistrationDate, null, null, null, @SecurityGroupIdFull 

			END
	   END


       COMMIT TRANSACTION ContainerInsert
       SELECT @EntityShortId as idContainer
       END TRY

       BEGIN CATCH 
           print ERROR_MESSAGE() 
             ROLLBACK TRANSACTION ContainerInsert
       END CATCH
	   GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter stored procedure [dbo].[ContainerGroups_Insert]';
GO

ALTER PROCEDURE [dbo].[ContainerGroups_Insert]
	   @idContainer smallint,
	   @Rights char(20),
       @GroupName varchar(255), 
       @ResolutionRights char(20),
       @DocumentRights char(20),
	   @RegistrationUser nvarchar(256),
	   @RegistrationDate datetimeoffset(7),
       @DocumentSeriesRights char(20),
       @idGroup int,
       @DeskRights char(20),
       @UDSRights char(20),
	   @PrivacyLevel int,
	   @FascicleRights char(20)
	   AS 
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
BEGIN TRY
BEGIN TRANSACTION ContainerGroupInsert

	DECLARE @idContainerGroup uniqueidentifier
	SET @idContainerGroup = newid()

	     INSERT INTO <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[containergroup] ([idContainer],[Rights] , [GroupName], [ResolutionRights], [DocumentRights], [RegistrationUser], [RegistrationDate], [DocumentSeriesRights], [idGroup], [DeskRights], [UDSRights], [IdContainerGroup], [PrivacyLevel], [FascicleRights])
         VALUES(@idContainer, @Rights, @GroupName, @ResolutionRights, @DocumentRights, @RegistrationUser, @RegistrationDate, @DocumentSeriesRights, @idGroup, @DeskRights, @UDSRights, @idContainerGroup, @PrivacyLevel, @FascicleRights)

		 IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		 BEGIN 
		  INSERT INTO  <DBAtti, varchar(50), DBAtti>.[dbo].[containergroup] ([idContainer],[Rights] , [GroupName], [ResolutionRights], [DocumentRights], [RegistrationUser], [RegistrationDate], [DocumentSeriesRights], [idGroup], [DeskRights], [UDSRights], [IdContainerGroup], [PrivacyLevel], [FascicleRights])
          Values(@idContainer, @Rights, @idGroup, @ResolutionRights, @DocumentRights, @RegistrationUser, @RegistrationDate, @DocumentSeriesRights, @idGroup, @DeskRights, @UDSRights, @idContainerGroup, @PrivacyLevel, @FascicleRights)
		END
		IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
		BEGIN 
			INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[containergroup] ([idContainer],[Rights] , [GroupName], [ResolutionRights], [DocumentRights], [RegistrationUser], [RegistrationDate], [DocumentSeriesRights], [idGroup], [DeskRights], [UDSRights], [IdContainerGroup], [PrivacyLevel], [FascicleRights])
			VALUES(@idContainer, @Rights, @GroupName, @ResolutionRights, @DocumentRights, @RegistrationUser, @RegistrationDate, @DocumentSeriesRights, @idGroup, @DeskRights, @UDSRights, @idContainerGroup, @PrivacyLevel, @FascicleRights)
		END
	
      COMMIT TRANSACTION ContainerGroupInsert
END TRY
BEGIN CATCH 
	print ERROR_MESSAGE() 
		ROLLBACK TRANSACTION ContainerGroupInsert
END CATCH
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Conservation_FX_AvailableDocumentSeriesItemLogsToConservate]'
GO

IF ([dbo].[IsSQL2012Compatible]()) = 1
	BEGIN
		EXECUTE sp_executesql N'ALTER FUNCTION [webapiprivate].[Conservation_FX_AvailableDocumentSeriesItemLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(			
			SELECT DSIL.UniqueId AS Id, UniqueIdDocumentSeriesItem AS ReferenceUniqueId, Subject, CAST(Year AS SMALLINT) AS Year, Number, 
			''DocumentSeriesItemLog'' AS EntityName, ''Archivio'' AS ReferenceEntityName, TODATETIMEOFFSET(LogDate, 0) AS LogDate, LogType, 
			SystemUser AS RegistrationUser, LogDescription AS Description, Hash 
			FROM DocumentSeriesItemLog DSIL
			INNER JOIN DocumentSeriesItem DSI ON DSI.UniqueId = DSIL.UniqueIdDocumentSeriesItem
			WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = DSIL.UniqueId)
			AND YEAR(LogDate) >= 2018
			ORDER BY DSIL.Id
			OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
		)'		
	END
ELSE
	BEGIN
		EXECUTE sp_executesql N'ALTER FUNCTION [webapiprivate].[Conservation_FX_AvailableDocumentSeriesItemLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(
			WITH TMP as (
				SELECT ROW_NUMBER() OVER(ORDER BY IdDossierLog ASC) AS RowNumber, Id, 
				ReferenceUniqueId, Subject, Year, Number, LogDate, LogType, RegistrationUser, 
				Description, Hash 
				FROM 
				(
					SELECT DSIL.Id AS IdDossierLog, DSIL.UniqueId AS Id, UniqueIdDocumentSeriesItem AS ReferenceUniqueId, 
					DSI.Subject AS Subject, DSI.Year AS Year, DSI.Number AS Number, LogDate, 
					LogType, SystemUser AS RegistrationUser, LogDescription AS Description, Hash 
					FROM DocumentSeriesItemLog DSIL
					INNER JOIN DocumentSeriesItem DSI ON DSI.UniqueId = DSIL.UniqueIdDocumentSeriesItem
					WHERE YEAR(LogDate) >= 2018
				) AS T
			)

			SELECT Id, ReferenceUniqueId, Subject, CAST(Year AS SMALLINT) AS Year, Number, 
			''DocumentSeriesItemLog'' AS EntityName, ''Archivio'' AS ReferenceEntityName, TODATETIMEOFFSET(LogDate, 0) AS LogDate, LogType, RegistrationUser, Description, Hash 
			FROM TMP
			WHERE RowNumber > @SKIP and RowNumber <= @Skip + @Take
			AND NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = Id)
		)'
	END


IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Conservation_FX_AvailableDossierLogsToConservate]'
GO

IF ([dbo].[IsSQL2012Compatible]()) = 1
	BEGIN
		EXECUTE sp_executesql N'ALTER FUNCTION [webapiprivate].[Conservation_FX_AvailableDossierLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(			
			SELECT IdDossierLog AS Id, DL.IdDossier AS ReferenceUniqueId, ''DossierLog'' AS EntityName, ''Dossier'' AS ReferenceEntityName, D.Subject AS Subject,
			D.Year AS Year, D.Number AS Number, LogDate, CAST(LogType AS NVARCHAR) AS LogType, SystemUser AS RegistrationUser, LogDescription AS Description, Hash 
			FROM DossierLogs DL
			INNER JOIN Dossiers D ON D.IdDossier = DL.IdDossier
			WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = DL.IdDossierLog)
			AND YEAR(LogDate) >= 2018
			ORDER BY DL.LogDate
			OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
		)'	
	END
ELSE
	BEGIN
		EXECUTE sp_executesql N'ALTER FUNCTION [webapiprivate].[Conservation_FX_AvailableDossierLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(
			WITH TMP as (
				SELECT ROW_NUMBER() OVER(ORDER BY LogDate ASC) AS RowNumber, Id, 
				ReferenceUniqueId, Subject, Year, Number, LogDate, LogType, RegistrationUser, 
				Description, Hash 
				FROM 
				(
					SELECT IdDossierLog AS Id, DL.IdDossier AS ReferenceUniqueId, D.Subject AS Subject,
					D.Year AS Year, D.Number AS Number, LogDate, 
					LogType, SystemUser AS RegistrationUser, LogDescription AS Description, Hash 
					FROM DossierLogs DL
					INNER JOIN Dossiers D ON D.IdDossier = DL.IdDossier
					WHERE YEAR(LogDate) >= 2018
				) AS T
			)

			SELECT Id, ReferenceUniqueId, ''DossierLog'' AS EntityName, ''Dossier'' AS ReferenceEntityName, Subject, 
			Year, Number, LogDate, CAST(LogType AS NVARCHAR) AS LogType, RegistrationUser, Description, Hash FROM TMP
			WHERE RowNumber > @SKIP and RowNumber <= @Skip + @Take
			AND NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = Id)
		)'
	END

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Conservation_FX_AvailableFascicleLogsToConservate]'
GO

IF ([dbo].[IsSQL2012Compatible]()) = 1
	BEGIN
		EXECUTE sp_executesql N'ALTER FUNCTION [webapiprivate].[Conservation_FX_AvailableFascicleLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(			
			SELECT IdFascicleLog AS Id, FL.IdFascicle AS ReferenceUniqueId, ''FascicleLog'' AS EntityName, ''Fascicolo'' AS ReferenceEntityName,
			F.Object AS Subject, F.Year AS Year, F.Number AS Number,
			LogDate, CAST(LogType AS NVARCHAR) AS LogType, SystemUser AS RegistrationUser, LogDescription AS Description, Hash 
			FROM FascicleLogs FL
			INNER JOIN Fascicles F ON F.IdFascicle = FL.IdFascicle
			WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = FL.IdFascicleLog)
			AND YEAR(LogDate) >= 2018
			ORDER BY FL.LogDate
			OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
		)'	
	END
ELSE
	BEGIN
		EXECUTE sp_executesql N'ALTER FUNCTION [webapiprivate].[Conservation_FX_AvailableFascicleLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(
			WITH TMP as (
				SELECT ROW_NUMBER() OVER(ORDER BY LogDate ASC) AS RowNumber, Id, 
				ReferenceUniqueId, Subject, Year, Number, LogDate, LogType, RegistrationUser, 
				Description, Hash 
				FROM 
				(
					SELECT IdFascicleLog AS Id, FL.IdFascicle AS ReferenceUniqueId, 
					F.Object AS Subject, F.Year AS Year, F.Number AS Number,
					LogDate, LogType, SystemUser AS RegistrationUser, LogDescription AS Description, Hash 
					FROM FascicleLogs FL
					INNER JOIN Fascicles F ON F.IdFascicle = FL.IdFascicle
					WHERE YEAR(LogDate) >= 2018
				) AS T
			)

			SELECT Id, ReferenceUniqueId, ''FascicleLog'' AS EntityName, ''Fascicolo'' AS ReferenceEntityName, Subject,
			Year, Number, LogDate, CAST(LogType AS NVARCHAR) AS LogType, RegistrationUser, Description, Hash 
			FROM TMP
			WHERE RowNumber > @SKIP and RowNumber <= @Skip + @Take
			AND NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = Id)
		)'
	END

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Conservation_FX_AvailableProtocolLogsToConservate]'
GO

IF ([dbo].[IsSQL2012Compatible]()) = 1
	BEGIN
		EXECUTE sp_executesql N'ALTER FUNCTION [webapiprivate].[Conservation_FX_AvailableProtocolLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(			
			SELECT PL.UniqueId AS Id, UniqueIdProtocol AS ReferenceUniqueId, ''ProtocolLog'' AS EntityName, ''Protocollo'' AS ReferenceEntityName,
			P.Object AS Subject, P.Year AS Year, P.Number AS Number, TODATETIMEOFFSET(LogDate, 0) AS LogDate, LogType, SystemUser AS RegistrationUser, LogDescription AS Description, Hash 
			FROM ProtocolLog PL
			INNER JOIN Protocol P ON P.UniqueId = PL.UniqueIdProtocol
			WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = PL.UniqueId)
			AND YEAR(LogDate) >= 2018
			ORDER BY PL.Id
			OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
		)'
	END
ELSE
	BEGIN
		EXECUTE sp_executesql N'ALTER FUNCTION [webapiprivate].[Conservation_FX_AvailableProtocolLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(
			WITH TMP as (
				SELECT ROW_NUMBER() OVER(ORDER BY IdProtocolLog ASC) AS RowNumber, Id, 
				ReferenceUniqueId, Subject, Year, Number, LogDate, LogType, RegistrationUser, 
				Description, Hash
				FROM 
				(
					SELECT PL.Id AS IdProtocolLog, PL.UniqueId AS Id, UniqueIdProtocol AS ReferenceUniqueId, 
					P.Object AS Subject, P.Year AS Year, P.Number AS Number,
					LogDate, LogType, SystemUser AS RegistrationUser, LogDescription AS Description, Hash 
					FROM ProtocolLog PL
					INNER JOIN Protocol P ON P.UniqueId = PL.UniqueIdProtocol
					WHERE YEAR(LogDate) >= 2018
				) AS T
			)

			SELECT Id, ReferenceUniqueId, ''ProtocolLog'' AS EntityName, ''Protocollo'' AS ReferenceEntityName, Subject,
			Year, Number, TODATETIMEOFFSET(LogDate, 0) AS LogDate, 
			LogType, RegistrationUser, Description, Hash FROM TMP
			WHERE RowNumber > @SKIP and RowNumber <= @Skip + @Take
			AND NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = Id)
		)'
	END


IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Conservation_FX_AvailablePECMailLogsToConservate]'
GO

IF ([dbo].[IsSQL2012Compatible]()) = 1
	BEGIN
		EXECUTE sp_executesql N'ALTER FUNCTION [webapiprivate].[Conservation_FX_AvailablePECMailLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(			
			SELECT PML.UniqueId AS Id, PM.UniqueId AS ReferenceUniqueId, ''PECMailLog'' AS EntityName, ''PECMail'' AS ReferenceEntityName, PM.MailSubject AS Subject,
			TODATETIMEOFFSET(Date, 0) AS LogDate, Type AS LogType, SystemUser AS RegistrationUser, Description, Hash 
			FROM PECMailLog PML
			INNER JOIN PECMail PM ON PM.IdPecMail = IDMail
			WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = PML.UniqueId)
			AND YEAR(Date) >= 2018
			ORDER BY PML.Id
			OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
		)'	
	END
ELSE
	BEGIN
		EXECUTE sp_executesql N'ALTER FUNCTION [webapiprivate].[Conservation_FX_AvailablePECMailLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(
			WITH TMP as (
				SELECT ROW_NUMBER() OVER(ORDER BY IdPECMailLog ASC) AS RowNumber, Id, ReferenceUniqueId, Subject, LogDate, LogType, RegistrationUser, Description, Hash  FROM 
				(
					SELECT PML.Id AS IdPECMailLog, PML.UniqueId AS Id, PM.UniqueId AS ReferenceUniqueId, PM.MailSubject AS Subject,
					Date AS LogDate, Type AS LogType, SystemUser AS RegistrationUser, Description, Hash 
					FROM PECMailLog PML
					INNER JOIN PECMail PM ON PM.IdPecMail = IDMail
					WHERE YEAR(Date) >= 2018
				) AS T
			)

			SELECT Id, ReferenceUniqueId, ''PECMailLog'' AS EntityName, ''PECMail'' AS ReferenceEntityName, Subject, TODATETIMEOFFSET(LogDate, 0) AS LogDate, LogType, RegistrationUser, Description, Hash FROM TMP
			WHERE RowNumber > @SKIP and RowNumber <= @Skip + @Take
			AND NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = Id)
		)'
	END


IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Conservation_FX_AvailableTableLogsToConservate]'
GO

IF ([dbo].[IsSQL2012Compatible]()) = 1
	BEGIN
		EXECUTE sp_executesql N'ALTER FUNCTION [webapiprivate].[Conservation_FX_AvailableTableLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(			
			SELECT IdTableLog AS Id, EntityUniqueId AS ReferenceUniqueId, ''TableLog'' AS EntityName, ''Amministrazione'' AS ReferenceEntityName, LogDate, CAST(LogType AS NVARCHAR) AS LogType, 
			SystemUser AS RegistrationUser,
			((CASE TableName 
					WHEN ''Category'' THEN ''Classificatore''
					WHEN ''CategoryGroup'' THEN ''Gruppo classificatore''
					WHEN ''Container'' THEN ''Contenitore''
					WHEN ''ContainerGroup'' THEN ''Gruppo contenitore''
					WHEN ''Role'' THEN ''Settore''
					WHEN ''RoleGroup'' THEN ''Gruppo settore''
					WHEN ''SecurityGroups'' THEN ''Gruppo sicurezza''
					WHEN ''SecurityUser'' THEN ''Utente sicurezza''
					WHEN ''PrivacyLevel'' THEN ''Livello di sicurezza''
					WHEN ''TemplateCollaboration'' THEN ''Template di collaborazione''
					ELSE TableName
					END) + '' - '' + LogDescription) AS Description, Hash 
			FROM TableLog
			WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = IdTableLog)
			AND YEAR(LogDate) >= 2018
			ORDER BY LogDate
			OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
		)'
	END
ELSE
	BEGIN
		EXECUTE sp_executesql N'ALTER FUNCTION [webapiprivate].[Conservation_FX_AvailableTableLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(
			WITH TMP as (
				SELECT ROW_NUMBER() OVER(ORDER BY LogDate ASC) AS RowNumber, Id, ReferenceUniqueId, ReferenceEntityName, LogDate, LogType, RegistrationUser, Description, Hash  FROM 
				(
					SELECT IdTableLog AS Id, EntityUniqueId AS ReferenceUniqueId, ''Amministrazione'' AS ReferenceEntityName, LogDate, LogType, SystemUser AS RegistrationUser, ((CASE TableName 
					WHEN ''Category'' THEN ''Classificatore''
					WHEN ''CategoryGroup'' THEN ''Gruppo classificatore''
					WHEN ''Container'' THEN ''Contenitore''
					WHEN ''ContainerGroup'' THEN ''Gruppo contenitore''
					WHEN ''Role'' THEN ''Settore''
					WHEN ''RoleGroup'' THEN ''Gruppo settore''
					WHEN ''SecurityGroups'' THEN ''Gruppo sicurezza''
					WHEN ''SecurityUser'' THEN ''Utente sicurezza''
					WHEN ''PrivacyLevel'' THEN ''Livello di sicurezza''
					WHEN ''TemplateCollaboration'' THEN ''Template di collaborazione''
					ELSE TableName
					END) + '' - '' + LogDescription) AS Description, Hash FROM TableLog
					WHERE YEAR(LogDate) >= 2018
				) AS T
			)

			SELECT Id, ReferenceUniqueId, ''TableLog'' AS EntityName, ReferenceEntityName, LogDate, CAST(LogType AS NVARCHAR) AS LogType, RegistrationUser, Description, Hash FROM TMP
			WHERE RowNumber > @SKIP and RowNumber <= @Skip + @Take
			AND NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = Id)
		)'
	END

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Conservation_FX_AvailableUDSLogsToConservate]'
GO

IF ([dbo].[IsSQL2012Compatible]()) = 1
	BEGIN
		EXECUTE sp_executesql N'ALTER FUNCTION [webapiprivate].[Conservation_FX_AvailableUDSLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(			
			SELECT IdUDSLog AS Id, IdUDS AS ReferenceUniqueId, ''UDSLog'' AS EntityName, UR.Name AS ReferenceEntityName, LogDate, CAST(LogType AS NVARCHAR) AS LogType, 
			SystemUser AS RegistrationUser, LogDescription AS Description, Hash 
			FROM uds.UDSLogs UL
			INNER JOIN uds.UDSRepositories UR ON UR.IdUDSRepository = UL.IdUDSRepository
			WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = IdUDSLog)
			AND YEAR(LogDate) >= 2018
			ORDER BY LogDate
			OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
		)'
	END
ELSE
	BEGIN
		EXECUTE sp_executesql N'ALTER FUNCTION [webapiprivate].[Conservation_FX_AvailableUDSLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(
			WITH TMP as (
				SELECT ROW_NUMBER() OVER(ORDER BY LogDate ASC) AS RowNumber, Id, ReferenceUniqueId, ReferenceEntityName, LogDate, LogType, RegistrationUser, Description, Hash  FROM 
				(
					SELECT IdUDSLog AS Id, IdUDS AS ReferenceUniqueId, UR.Name AS ReferenceEntityName, LogDate, LogType, SystemUser AS RegistrationUser, LogDescription AS Description, Hash FROM uds.UDSLogs UL
					INNER JOIN uds.UDSRepositories UR ON UR.IdUDSRepository = UL.IdUDSRepository
					WHERE YEAR(LogDate) >= 2018
				) AS T
			)

			SELECT Id, ReferenceUniqueId, ''UDSLog'' AS EntityName, ReferenceEntityName, LogDate, CAST(LogType AS NVARCHAR) AS LogType, RegistrationUser, Description, Hash FROM TMP
			WHERE RowNumber > @SKIP and RowNumber <= @Skip + @Take
			AND NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = Id)
		)'
	END

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Conservation_FX_CountDocumentSeriesItemLogsToConservate]'
GO

ALTER FUNCTION [webapiprivate].[Conservation_FX_CountDocumentSeriesItemLogsToConservate]
(	
	
)
RETURNS INT 
AS
BEGIN 
	DECLARE @Result AS integer

	SELECT @Result = COUNT(UniqueId)
	FROM DocumentSeriesItemLog
	WHERE YEAR(LogDate) >= 2018 AND NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = UniqueId)

	RETURN @Result
END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Conservation_FX_CountDossierLogsToConservate]'
GO

ALTER FUNCTION [webapiprivate].[Conservation_FX_CountDossierLogsToConservate]
(	
	
)
RETURNS INT 
AS
BEGIN 
	DECLARE @Result AS integer

	SELECT @Result = COUNT(IdDossierLog)
	FROM DossierLogs
	WHERE YEAR(LogDate) >= 2018 AND NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = IdDossierLog)

	RETURN @Result
END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Conservation_FX_CountFascicleLogsToConservate]'
GO

ALTER FUNCTION [webapiprivate].[Conservation_FX_CountFascicleLogsToConservate]
(	
	
)
RETURNS INT 
AS
BEGIN 
	DECLARE @Result AS integer

	SELECT @Result = COUNT(IdFascicleLog)
	FROM FascicleLogs
	WHERE YEAR(LogDate) >= 2018 AND NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = IdFascicleLog)

	RETURN @Result
END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Conservation_FX_CountPECMailLogsToConservate]'
GO

ALTER FUNCTION [webapiprivate].[Conservation_FX_CountPECMailLogsToConservate]
(	
	
)
RETURNS INT 
AS
BEGIN 
	DECLARE @Result AS integer

	SELECT @Result = COUNT(PML.UniqueId)
	FROM PECMailLog PML
	INNER JOIN PECMail PM ON PM.IdPecMail = IDMail
	WHERE YEAR(PML.Date) >= 2018 AND NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = PML.UniqueId)

	RETURN @Result
END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Conservation_FX_CountProtocolLogsToConservate]'
GO

ALTER FUNCTION [webapiprivate].[Conservation_FX_CountProtocolLogsToConservate]
(	
	
)
RETURNS INT 
AS
BEGIN 
	DECLARE @Result AS integer

	SELECT @Result = COUNT(UniqueId)
	FROM ProtocolLog
	WHERE YEAR(LogDate) >= 2018 AND NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = UniqueId)

	RETURN @Result
END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Conservation_FX_CountTableLogsToConservate]'
GO

ALTER FUNCTION [webapiprivate].[Conservation_FX_CountTableLogsToConservate]
(	
	
)
RETURNS INT 
AS
BEGIN 
	DECLARE @Result AS integer

	SELECT @Result = COUNT(IdTableLog)
	FROM TableLog
	WHERE YEAR(LogDate) >= 2018 AND NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = IdTableLog)

	RETURN @Result
END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Conservation_FX_CountUDSLogsToConservate]'
GO

ALTER FUNCTION [webapiprivate].[Conservation_FX_CountUDSLogsToConservate]
(	
	
)
RETURNS INT 
AS
BEGIN 
	DECLARE @Result AS integer

	SELECT @Result = COUNT(IdUDSLog)
	FROM uds.UDSLogs
	WHERE YEAR(LogDate) >= 2018 AND NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = IdUDSLog)

	RETURN @Result
END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter stored procedure [dbo].[FascicleFolder_Update]'
GO

ALTER PROCEDURE [dbo].[FascicleFolder_Update] 
       @IdFascicleFolder uniqueidentifier,        
       @IdFascicle uniqueidentifier, 
       @IdCategory smallint,
       @Name nvarchar(256), 
       @Status smallint,
       @Typology smallint,
	   @RegistrationDate datetimeoffset(7),
       @RegistrationUser nvarchar(256), 
       @LastChangedDate datetimeoffset(7),
       @LastChangedUser nvarchar(256),
	   @ParentInsertId uniqueidentifier,
	   @Timestamp_Original timestamp
	AS
		
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
		
	BEGIN TRY
		DECLARE @parentNode hierarchyid, @childNode hierarchyid, @node hierarchyid

		BEGIN TRANSACTION UpdateFascicleFolder

		SELECT @node = [FascicleFolderNode] FROM [dbo].[FascicleFolders] WHERE [IdFascicleFolder] = @IdFascicleFolder
		IF @ParentInsertId IS NOT NULL
		BEGIN
			SELECT @parentNode = [FascicleFolderNode] FROM [dbo].[FascicleFolders] WHERE [IdFascicleFolder] = @ParentInsertId
			SELECT @childNode = MAX([FascicleFolderNode]) FROM [dbo].[FascicleFolders] WHERE [FascicleFolderNode].GetAncestor(1) = @parentNode;
			SET @node = @parentNode.GetDescendant(@childNode, NULL)
		END

		UPDATE [dbo].[FascicleFolders] SET [IdFascicle] = @IdFascicle, [IdCategory] = @IdCategory, [Name] = @Name, [Status] = @Status, 
		[Typology] = @Typology, [LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser,
		[FascicleFolderNode] = @node
		WHERE [IdFascicleFolder] = @IdFascicleFolder

        SELECT [FascicleFolderNode],[IdFascicleFolder] as UniqueId,[IdFascicle],[IdCategory],[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
        [Name],[Typology],[FascicleFolderPath],[FascicleFolderLevel],[FascicleFolderParentNode],[ParentInsertId],[Timestamp] 
		FROM [dbo].[FascicleFolders] WHERE [IdFascicleFolder] = @IdFascicleFolder
		
		COMMIT TRANSACTION UpdateFascicleFolder
	END TRY
		
	BEGIN CATCH 
		ROLLBACK TRANSACTION UpdateFascicleFolder
		
		declare @ErrorNumber as int = ERROR_NUMBER()
		declare @ErrorSeverity as int = ERROR_SEVERITY()
		declare @ErrorMessage as nvarchar(4000)
		declare @ErrorState as int = ERROR_STATE()
		declare @ErrorLine as int = ERROR_LINE()
		declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		SET @ErrorMessage = 'Error Code: '+cast(@ErrorNumber as nvarchar(max))+' Message: '+ ERROR_MESSAGE();

		 RAISERROR (@ErrorMessage, -- Message text.  
               @ErrorSeverity, -- Severity.  
               @ErrorState, -- State.  			   
			   @ErrorProcedure, -- parameter: original error procedure name.
			@ErrorLine       -- parameter: original error line number.
               );  
	END CATCH
	GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Create sql function [webapiprivate].[Fascicle_FX_HasViewableRight]'
GO

CREATE  FUNCTION [webapiprivate].[Fascicles_FX_HasViewableRight]
(
       @UserName nvarchar(256), 
       @Domain nvarchar(256),
       @IdFascicle uniqueidentifier
)
RETURNS BIT
AS
BEGIN
       declare @HasRight bit;
       declare @EmptyRights nvarchar(20);
	   SET @EmptyRights = '00000000000000000000';

       WITH
       MySecurityGroups AS (
        SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
       ),
	   MyResponsibleRoles AS (
	    SELECT idRole FROM [dbo].[FascicleRoles] WHERE IdFascicle = @IdFascicle AND RoleAuthorizationType = 0 and IsMaster = 1
	   )

       SELECT  @HasRight = CAST(COUNT(1) AS BIT)
       FROM [dbo].[Fascicles] F
       WHERE F.IdFascicle = @IdFascicle
       AND ( exists (select 1
                     FROM MySecurityGroups SG
                     INNER JOIN [dbo].[RoleGroup] RG on SG.IdGroup = RG.IdGroup
                     INNER JOIN [dbo].[FascicleRoles] FR on FR.IdFascicle = F.IdFascicle AND RG.IdRole = FR.IdRole
                     WHERE (
								(F.FascicleType IN (4) AND FR.RoleAuthorizationType = 0) OR
								(F.FascicleType IN (1,2) AND FR.RoleAuthorizationType = 1) OR
								(F.FascicleType IN (1,2) AND FR.RoleAuthorizationType = 0 AND IsMaster = 0)
							)
					 AND ((RG.ProtocolRights <> @EmptyRights)
                        OR (RG.ResolutionRights <> @EmptyRights)
                        OR (RG.DocumentRights <> @EmptyRights)
                        OR (RG.DocumentSeriesRights <> @EmptyRights))
                    )
             OR 
                exists (select 1 
						from [dbo].[RoleUser] RU
						INNER JOIN [dbo].[CategoryFascicleRights] CFR on CFR.IdRole = RU.idRole
						INNER JOIN [dbo].[CategoryFascicles] CF on CFR.IdCategoryFascicle = CF.IdCategoryFascicle
						where CF.IdCategory = F.IdCategory
						and RU.Account=@Domain+'\'+@UserName
						and F.FascicleType IN (1, 2) and RU.[Type] in ('RP', 'SP')
						and (
								(exists(select 1 from MyResponsibleRoles) and (RU.idRole in (select idRole from MyResponsibleRoles))) OR 
								(not exists(select 1 from MyResponsibleRoles))
							)
                       )
			--OR
			--	exists (
			--			select 1
			--			from [dbo].[Contact] C
			--			INNER JOIN [dbo].[FascicleContacts] FC on FC.IdContact = C.Incremental
			--			where FC.IdFascicle = F.IdFascicle AND C.SearchCode = @Domain+'\'+@UserName
			--		   )
			OR
				exists (
						select 1
						from [dbo].[ContainerGroup] CG
						where CG.idContainer = F.IdContainer
						AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup) 
						AND CG.FascicleRights LIKE '__1%'
					   )
       )
       
       RETURN @HasRight
END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Create sql function [webapiprivate].[Fascicle_FX_HasManageableRight]'
GO

CREATE FUNCTION [webapiprivate].[Fascicles_FX_HasManageableRight]
(
       @UserName nvarchar(256), 
       @Domain nvarchar(256),
       @IdFascicle uniqueidentifier
)
RETURNS BIT
AS
BEGIN
       declare @HasRight bit;
       declare @EmptyRights nvarchar(20);
	   SET @EmptyRights = '00000000000000000000';

       WITH
       MySecurityGroups AS (
        SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
       ),
	   MyResponsibleRoles AS (
	    SELECT idRole FROM [dbo].[FascicleRoles] WHERE IdFascicle = @IdFascicle AND RoleAuthorizationType = 0
	   )

       SELECT  @HasRight = CAST(COUNT(1) AS BIT)
       FROM [dbo].[Fascicles] F
       WHERE F.IdFascicle = @IdFascicle
       AND ( exists (select 1
                           from [dbo].[RoleGroup] RG
                           where RG.IdRole in (select idRole from MyResponsibleRoles)
						   AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup)  
						   AND ((RG.ProtocolRights <> @EmptyRights)
                             OR (RG.ResolutionRights <> @EmptyRights)
                             OR (RG.DocumentRights <> @EmptyRights)
                             OR (RG.DocumentSeriesRights <> @EmptyRights))
                           )
             OR 
                exists (select 1 
						from [dbo].[RoleUser] RU
						INNER JOIN [dbo].[CategoryFascicleRights] CFR on CFR.IdRole = RU.idRole
						INNER JOIN [dbo].[CategoryFascicles] CF on CFR.IdCategoryFascicle = CF.IdCategoryFascicle
						where CF.IdCategory = F.IdCategory
						and RU.Account=@Domain+'\'+@UserName
						and F.FascicleType IN (1, 2) and RU.[Type] in ('RP', 'SP')
						and (
								(exists(select 1 from MyResponsibleRoles) and (RU.idRole in (select idRole from MyResponsibleRoles))) OR 
								(not exists(select 1 from MyResponsibleRoles))
							)
                       )
			OR
				exists (
						select 1
						from [dbo].[Contact] C
						INNER JOIN [dbo].[FascicleContacts] FC on FC.IdContact = C.Incremental
						where FC.IdFascicle = F.IdFascicle AND C.SearchCode = @Domain+'\'+@UserName
					   )
			OR
				exists (
						select 1
						from [dbo].[ContainerGroup] CG
						where CG.idContainer = F.IdContainer
						AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup) 
						AND (CG.FascicleRights LIKE '1%' OR CG.FascicleRights LIKE '_1%')
					   )
       )
       
       RETURN @HasRight

END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Create sql function [webapiprivate].[Fascicle_FX_HasInsertRight]'
GO

CREATE FUNCTION [webapiprivate].[Fascicles_FX_HasInsertRight]
(
       @UserName nvarchar(256), 
       @Domain nvarchar(256),
	   @FascicleType int
)
RETURNS BIT
AS
BEGIN
       declare @HasRight bit;
       declare @EmptyRights nvarchar(20);
	   SET @EmptyRights = '00000000000000000000';

       WITH
       MySecurityGroups AS (
        SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
       )

       SELECT  @HasRight = CAST(COUNT(1) AS BIT)
       FROM [dbo].[Role] R
	   WHERE (exists (SELECT 1 FROM [dbo].[RoleUser] RU
					  INNER JOIN [dbo].[CategoryFascicleRights] CFR on CFR.IdRole = RU.idRole
					  INNER JOIN [dbo].[CategoryFascicles] CF on CFR.IdCategoryFascicle = CF.IdCategoryFascicle
					  WHERE (@FascicleType = 1 OR @FascicleType = 2)
					  AND R.idRole = RU.idRole 
					  AND RU.Account=@Domain + '\' + @UserName
					  AND Type in ('RP', 'SP')
					 )
			  OR
		      exists (SELECT 1 FROM [dbo].[RoleGroup] RG
					  where @FascicleType =  4
					  AND RG.IdRole = R.IdRole
					  AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup)  
					  AND ((RG.ProtocolRights <> @EmptyRights)
							OR (RG.ResolutionRights <> @EmptyRights)
							OR (RG.DocumentRights <> @EmptyRights)
							OR (RG.DocumentSeriesRights <> @EmptyRights)))
			  )
       
       
       RETURN @HasRight
END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Fascicle_FX_HasDocumentVisibilityRight]'
GO

ALTER FUNCTION [webapiprivate].[Fascicle_FX_HasDocumentVisibilityRight]
(
       @UserName nvarchar(256), 
       @Domain nvarchar(256),
       @IdFascicle uniqueidentifier
)
RETURNS BIT
AS
BEGIN
       declare @HasRight bit;

       SELECT  @HasRight = CAST(COUNT(1) AS BIT)
       FROM [dbo].[Fascicles] F
       WHERE F.IdFascicle = @IdFascicle
       AND F.VisibilityType = 1
       AND ((select [webapiprivate].[Fascicles_FX_HasViewableRight](@UserName, @Domain, @IdFascicle)) = 1)
       
       RETURN @HasRight

END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Fascicle_FX_IsCurrentUserManagerOnActivityFascicle]'
GO

ALTER FUNCTION [webapiprivate].[Fascicle_FX_IsCurrentUserManagerOnActivityFascicle]
(
    @Description nvarchar(256),
	@IdFascicle uniqueidentifier
)
RETURNS BIT
AS
BEGIN
	declare @IsManager bit;
	declare @EmptyRights nvarchar(20);
	SET @EmptyRights = '00000000000000000000';

	WITH
	MySecurityGroups AS (
        SELECT SG.IdGroup 
        FROM [dbo].[SecurityGroups] SG 
        LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Description = @Description 
		OR SG.AllUsers = 1
        GROUP BY SG.IdGroup
    ),
	MyResponsibleRoles AS (
	    SELECT idRole FROM [dbo].[FascicleRoles] WHERE IdFascicle = @IdFascicle AND RoleAuthorizationType = 0
	)

	SELECT  @IsManager = CAST(COUNT(1) AS BIT)
	FROM [dbo].[Fascicles] F
	WHERE F.IdFascicle = @IdFascicle
	AND (exists (select 1
				from [dbo].[RoleGroup] RG
				where RG.IdRole in (select idRole from MyResponsibleRoles)
				AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup)
				AND ((RG.ProtocolRights <> @EmptyRights)
					OR (RG.ResolutionRights <> @EmptyRights)
					OR (RG.DocumentRights <> @EmptyRights)
					OR (RG.DocumentSeriesRights <> @EmptyRights))                    
			   )
		OR
			exists (select 1
					from [dbo].[ContainerGroup] CG
					where CG.idContainer = F.IdContainer
					AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup) 
					AND (CG.FascicleRights LIKE '1%' OR CG.FascicleRights LIKE '_1%')
				   ))
	
	RETURN @IsManager
END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Fascicles_FX_AvailableFascicles]'
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_AvailableFascicles]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@idCategory smallint,
	@Name nvarchar(256)

)
RETURNS TABLE
AS
RETURN
(

SELECT F.IdFascicle AS UniqueId,
		F.Year,
		F.Number,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object as FascicleObject,
		F.Manager,
		F.IdCategory,
		F.FascicleType,
		F.VisibilityType,
		F.RegistrationUser,
		F.RegistrationDate,
		F_C.IdCategory as Category_idCategory,
		F_C.Name as Category_Name		
FROM Fascicles F
INNER JOIN [dbo].[Category] F_C ON F.idCategory = F_C.idCategory
	WHERE (@idCategory = 0 OR  F.IdCategory = @idCategory) 
	    AND ((@Name is NOT null AND ( F.Title like '%'+@Name+'%' OR F.Object like '%'+@Name+'%')) OR (@Name Is null))
		AND ((select [webapiprivate].[Fascicles_FX_HasViewableRight](@UserName, @Domain, F.IdFascicle)) = 1)       
GROUP BY F.IdFascicle,
		 F.Year,
		 F.Number,
		 F.EndDate,
		 F.Title,
		 F.Name,
		 F.Object,
		 F.Manager,
		 F.IdCategory,
		 F.FascicleType,
		 F.VisibilityType,
		 F.RegistrationUser,
		 F.RegistrationDate,
		 F_C.IdCategory,
		 F_C.Name
)	
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Fascicles_FX_AvailableFasciclesFromProtocol]'
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromProtocol]
(
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@UniqueIdProtocol uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(

	SELECT 
		F.IdFascicle as UniqueId,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object as FascicleObject,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.VisibilityType,
		F.RegistrationUser,
		F.RegistrationDate			
	FROM Fascicles F 
		LEFT JOIN FascicleProtocols FP on F.IdFascicle = FP.IdFascicle AND FP.UniqueIdProtocol = @UniqueIdProtocol
		LEFT JOIN Protocol P on P.UniqueId = @UniqueIdProtocol
		LEFT JOIN Category C on C.IdCategory = P.IdCategoryAPI
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 0 AND CF.FascicleType in (1 , 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
		WHERE (F.IdCategory = P.IdCategoryAPI or (CF2.FascicleType = 0 and CF2.DSWEnvironment = 0 and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = 0 and FascicleType = 1
			ORDER BY IdCategory DESC))) AND FP.IdFascicleProtocol IS NULL
		AND F.EndDate IS NULL
		AND ((select [webapiprivate].[Fascicles_FX_HasViewableRight](@UserName, @Domain, F.IdFascicle)) = 1)
	GROUP BY F.IdFascicle,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object ,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.VisibilityType,
		F.RegistrationUser,
		F.RegistrationDate
)
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Fascicles_FX_AvailableFasciclesFromResolution]'
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromResolution]
(
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@UniqueIdResolution uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(
	SELECT 
		F.IdFascicle as UniqueId,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object as FascicleObject,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.VisibilityType,
		F.RegistrationUser,
		F.RegistrationDate						
	FROM Fascicles F 
		LEFT JOIN FascicleResolutions FR on F.IdFascicle = FR.IdFascicle AND FR.UniqueIdResolution = @UniqueIdResolution
		LEFT JOIN Resolution R on R.UniqueId = @UniqueIdResolution
		LEFT JOIN Category C on C.IdCategory = R.IdCategoryAPI
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 0 AND CF.FascicleType in (1, 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
		WHERE (F.IdCategory = R.IdCategoryAPI or (CF2.FascicleType = 0 and CF2.DSWEnvironment = 0 and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = 0 and FascicleType = 1
			ORDER BY IdCategory DESC))) AND FR.IdFascicleResolution IS NULL
		AND F.EndDate IS NULL
		AND ((select [webapiprivate].[Fascicles_FX_HasViewableRight](@UserName, @Domain, F.IdFascicle)) = 1)
	GROUP BY F.IdFascicle,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object ,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.VisibilityType,
		F.RegistrationUser,
		F.RegistrationDate
)
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Fascicles_FX_AvailableFasciclesFromUDS]'
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_AvailableFasciclesFromUDS]
(
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@UniqueIdUD uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(

	SELECT 
		F.IdFascicle as UniqueId,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object as FascicleObject,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.VisibilityType,
		F.RegistrationUser,
		F.RegistrationDate			
	FROM Fascicles F
		LEFT JOIN FascicleUDS FU on F.IdFascicle=FU.IdFascicle and FU.IdUDS = @UniqueIdUD
		LEFT JOIN cqrs.DocumentUnits D on D.IdDocumentUnit = @UniqueIdUD
		LEFT JOIN Category C on C.IdCategory = D.IdCategory
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 0 AND CF.FascicleType in (1 , 0)
		INNER JOIN CategoryFascicles CF2 on CF2.IdCategory = C.IdCategory
WHERE (F.IdCategory = D.IdCategory or (CF2.FascicleType = 0 and CF2.DSWEnvironment = 0 and F.IdCategory in
		(SELECT TOP 1 IdCategory from CategoryFascicles where IdCategory in (
			SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
			AND DSWEnvironment = 0 and FascicleType = 1
			ORDER BY IdCategory DESC))) AND FU.IdFascicleUDS IS NULL
		AND F.EndDate IS NULL
		AND (NOT F.FascicleType = 4)
		AND ((select [webapiprivate].[Fascicles_FX_HasViewableRight](@UserName, @Domain, F.IdFascicle)) = 1)
GROUP BY F.IdFascicle,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object ,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.VisibilityType,
		F.RegistrationUser,
		F.RegistrationDate
)
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Alter sql function [webapiprivate].[Fascicles_FX_IsProcedureSecretary]'
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_IsProcedureSecretary](
	@UserName nvarchar(255),
	@Domain nvarchar(255),
	@CategoryId smallint
)
RETURNS BIT
AS
BEGIN
DECLARE @IsSecretary BIT;
SELECT @IsSecretary = CAST(COUNT(1) AS BIT)
FROM   dbo.RoleUser RU
INNER JOIN CategoryFascicleRights CFR ON CFR.IdRole = RU.idRole
INNER JOIN CategoryFascicles CF ON CF.IdCategoryFascicle = CFR.IdCategoryFascicle
WHERE  RU.Type = 'SP'
       and RU.Account = @Domain + '\' + @UserName
       and CF.IdCategory = @CategoryId
RETURN @IsSecretary
END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Create sql function [webapiprivate].[Fascicles_FX_CountAuthorizedFascicles]'
GO

CREATE FUNCTION [webapiprivate].[Fascicles_FX_CountAuthorizedFascicles](
	@UserName nvarchar(255),
	@Domain nvarchar(255),
	@Year smallint,
	@StartDateFrom datetimeoffset,
	@StartDateTo datetimeoffset,
	@EndDateFrom datetimeoffset,
	@EndDateTo datetimeoffset,
	@FascicleStatus int,
	@Manager nvarchar(256),
	@Name nvarchar(256),
	@ViewConfidential bit,
	@ViewAccessible bit,
	@Subject nvarchar(256),
	@Note nvarchar(256),
	@Rack nvarchar(256),
	@IdMetadataRepository nvarchar(256),
	@MetadataValue nvarchar (256),
	@Classifications nvarchar(256),
	@IncludeChildClassifications bit,
	@Roles nvarchar(MAX),
	@ApplySecurity bit
)
RETURNS INT
AS
	BEGIN
	DECLARE @CountFascicles INT;
	WITH 	
	MySecurityGroups AS (
		SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
	)
	
	SELECT @CountFascicles = COUNT(DISTINCT Fascicle.IdFascicle)
			 FROM dbo.Fascicles Fascicle
			 WHERE (@Year is null or Fascicle.Year = @Year)
			 AND (@StartDateFrom is null or Fascicle.StartDate >= @StartDateFrom)
			 AND (@StartDateTo is null or Fascicle.StartDate <= @StartDateTo)
			 AND (@EndDateFrom is null or Fascicle.EndDate >= @StartDateFrom)
			 AND (@EndDateTo is null or Fascicle.EndDate <= @EndDateTo)
			 AND (@FascicleStatus is null or 
					((@FascicleStatus = 2 and Fascicle.EndDate is not null and Fascicle.EndDate <= GETUTCDATE())
					 or (@FascicleStatus = 1 and Fascicle.EndDate is null and Fascicle.StartDate <= GETUTCDATE()))
				 )
			 AND (@Manager is null or exists (SELECT 1 FROM FascicleContacts FC
												inner join Contact C on C.Incremental = FC.IdContact
												WHERE C.FullIncrementalPath = @Manager AND FC.IdFascicle = Fascicle.IdFascicle)
				 )
			 AND (@Name is null or Fascicle.Name like '%'+@Name+'%')
			 AND ((@ViewConfidential is null and @ViewAccessible is null)
					or ((@ViewConfidential is null and @ViewAccessible is not null) and (Fascicle.VisibilityType = 1))
					or ((@ViewConfidential is not null and @ViewAccessible is null) and (Fascicle.VisibilityType = 0))
					or ((@ViewConfidential is not null and @ViewAccessible is not null) and (Fascicle.VisibilityType in (0,1)))
				 )
			 AND (@Subject is null or Fascicle.Object like '%'+@Subject+'%')
			 AND (@Rack is null or Fascicle.Rack like '%'+@Rack+'%')
			 AND (@Note is null or Fascicle.Note like '%'+@Note+'%')
			 AND (@IdMetadataRepository is null or Fascicle.IdMetadataRepository = @IdMetadataRepository)
			 AND (@MetadataValue is null or Fascicle.MetadataValues like '%'+@MetadataValue+'%')
			 AND (@Classifications is null or ((@IncludeChildClassifications = 1 and exists (SELECT 1 FROM Category
																							 WHERE IdCategory = Fascicle.IdCategory 
																							 AND FullIncrementalPath like '%'+@Classifications))
												or ((@IncludeChildClassifications is null or @IncludeChildClassifications = 0) and exists (SELECT 1 FROM Category
																							 WHERE IdCategory = Fascicle.IdCategory 
																							 AND FullIncrementalPath = @Classifications))
											  )
				 )
			 AND (@Roles is null or exists (SELECT 1 FROM FascicleRoles FR
															   WHERE FR.IdFascicle = Fascicle.IdFascicle
															   AND FR.IdRole IN (SELECT CAST([Value] AS smallint) FROM dbo.SplitString(@Roles, '|')))
				 )
			 AND ((@ApplySecurity is null or @ApplySecurity = 0) or (@ApplySecurity = 1 AND 
					(exists (select 1
                             FROM MySecurityGroups SG
                             INNER JOIN [dbo].[RoleGroup] RG on SG.IdGroup = RG.IdGroup
                             INNER JOIN [dbo].[FascicleRoles] FR on FR.IdFascicle = Fascicle.IdFascicle AND RG.IdRole = FR.IdRole
                             WHERE (
									 (Fascicle.FascicleType IN (4) AND FR.RoleAuthorizationType = 0) OR
									 (Fascicle.FascicleType IN (1,2) AND FR.RoleAuthorizationType = 1) OR
									 (Fascicle.FascicleType IN (1,2) AND FR.RoleAuthorizationType = 0 AND IsMaster = 0)
								   )
							   AND ((RG.ProtocolRights <> '00000000000000000000')
								 OR (RG.ResolutionRights <> '00000000000000000000')
								 OR (RG.DocumentRights <> '00000000000000000000')
								 OR (RG.DocumentSeriesRights <> '00000000000000000000'))
                           )
					 OR 
						exists (select 1 
								from [dbo].[RoleUser] RU
								INNER JOIN [dbo].[CategoryFascicleRights] CFR on CFR.IdRole = RU.idRole
								INNER JOIN [dbo].[CategoryFascicles] CF on CFR.IdCategoryFascicle = CF.IdCategoryFascicle
								where CF.IdCategory = Fascicle.IdCategory
								and RU.Account=@Domain+'\'+@UserName
								and Fascicle.FascicleType IN (1, 2)
								and RU.[Type] in ('RP', 'SP')
								and (
										NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[FascicleRoles] WHERE IdFascicle = Fascicle.IdFascicle AND RoleAuthorizationType = 0 AND IsMaster = 1)
										OR (RU.idRole in ((SELECT idRole FROM [dbo].[FascicleRoles] WHERE IdFascicle = Fascicle.IdFascicle AND RoleAuthorizationType = 0 AND IsMaster = 1))))
									)
					--OR
					--	exists (
					--			select 1
					--			from [dbo].[Contact] C
					--			INNER JOIN [dbo].[FascicleContacts] FC on FC.IdContact = C.Incremental
					--			where FC.IdFascicle = Fascicle.IdFascicle AND C.SearchCode = @Domain+'\'+@UserName
					--		   )
					OR
						exists (
								select 1
								from [dbo].[ContainerGroup] CG
								where CG.idContainer = Fascicle.IdContainer
								AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup) 
								AND CG.FascicleRights LIKE '__1%'
							   )
					)
				))

	RETURN @CountFascicles
END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Create sql function [webapiprivate].[Fascicles_FX_AuthorizedFascicles]'
GO

CREATE FUNCTION [webapiprivate].[Fascicles_FX_AuthorizedFascicles](
	@UserName nvarchar(255),
	@Domain nvarchar(255),
	@Skip int,
	@Top int,
	@Year smallint,
	@StartDateFrom datetimeoffset,
	@StartDateTo datetimeoffset,
	@EndDateFrom datetimeoffset,
	@EndDateTo datetimeoffset,
	@FascicleStatus int,
	@Manager nvarchar(256),
	@Name nvarchar(256),
	@ViewConfidential bit,
	@ViewAccessible bit,
	@Subject nvarchar(256),
	@Note nvarchar(256),
	@Rack nvarchar(256),
	@IdMetadataRepository nvarchar(256),
	@MetadataValue nvarchar (256),
	@Classifications nvarchar(256),
	@IncludeChildClassifications bit,
	@Roles nvarchar(MAX),
	@ApplySecurity bit
)
RETURNS TABLE
AS
RETURN
	(
		WITH 	
		MySecurityGroups AS (
			SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
		),
		MyFascicles AS (
			select IdFascicle from
			(select Fascicle.IdFascicle, row_number() over(order by Fascicle.RegistrationDate ASC, Fascicle.Title ASC) AS rownum
			 FROM dbo.Fascicles Fascicle
			 WHERE (@Year is null or Fascicle.Year = @Year)
			 AND (@StartDateFrom is null or Fascicle.StartDate >= @StartDateFrom)
			 AND (@StartDateTo is null or Fascicle.StartDate <= @StartDateTo)
			 AND (@EndDateFrom is null or Fascicle.EndDate >= @StartDateFrom)
			 AND (@EndDateTo is null or Fascicle.EndDate <= @EndDateTo)
			 AND (@FascicleStatus is null or 
					((@FascicleStatus = 2 and Fascicle.EndDate is not null and Fascicle.EndDate <= GETUTCDATE())
					 or (@FascicleStatus = 1 and Fascicle.EndDate is null and Fascicle.StartDate <= GETUTCDATE()))
				 )
			 AND (@Manager is null or exists (SELECT 1 FROM FascicleContacts FC
												inner join Contact C on C.Incremental = FC.IdContact
												WHERE C.FullIncrementalPath = @Manager AND FC.IdFascicle = Fascicle.IdFascicle)
				 )
			 AND (@Name is null or Fascicle.Name like '%'+@Name+'%')
			 AND ((@ViewConfidential is null and @ViewAccessible is null)
					or ((@ViewConfidential is null and @ViewAccessible is not null) and (Fascicle.VisibilityType = 1))
					or ((@ViewConfidential is not null and @ViewAccessible is null) and (Fascicle.VisibilityType = 0))
					or ((@ViewConfidential is not null and @ViewAccessible is not null) and (Fascicle.VisibilityType in (0,1)))
				 )
			 AND (@Subject is null or Fascicle.Object like '%'+@Subject+'%')
			 AND (@Rack is null or Fascicle.Rack like '%'+@Rack+'%')
			 AND (@Note is null or Fascicle.Note like '%'+@Note+'%')
			 AND (@IdMetadataRepository is null or Fascicle.IdMetadataRepository = @IdMetadataRepository)
			 AND (@MetadataValue is null or Fascicle.MetadataValues like '%'+@MetadataValue+'%')
			 AND (@Classifications is null or ((@IncludeChildClassifications = 1 and exists (SELECT 1 FROM Category
																							 WHERE IdCategory = Fascicle.IdCategory 
																							 AND FullIncrementalPath like '%'+@Classifications))
												or ((@IncludeChildClassifications is null or @IncludeChildClassifications = 0) and exists (SELECT 1 FROM Category
																							 WHERE IdCategory = Fascicle.IdCategory 
																							 AND FullIncrementalPath = @Classifications))
											  )
				 )
			 AND (@Roles is null or exists (SELECT 1 FROM FascicleRoles FR
															   WHERE FR.IdFascicle = Fascicle.IdFascicle
															   AND FR.IdRole IN (SELECT CAST([Value] AS smallint) FROM dbo.SplitString(@Roles, '|')))
				 )
			 AND ((@ApplySecurity is null or @ApplySecurity = 0) or (@ApplySecurity = 1 AND 
					(exists (SELECT 1
							 FROM MySecurityGroups SG
                             INNER JOIN [dbo].[RoleGroup] RG on SG.IdGroup = RG.IdGroup
                             INNER JOIN [dbo].[FascicleRoles] FR on FR.IdFascicle = Fascicle.IdFascicle AND RG.IdRole = FR.IdRole
                             WHERE (
									 (Fascicle.FascicleType IN (4) AND FR.RoleAuthorizationType = 0) OR
									 (Fascicle.FascicleType IN (1,2) AND FR.RoleAuthorizationType = 1) OR
									 (Fascicle.FascicleType IN (1,2) AND FR.RoleAuthorizationType = 0 AND IsMaster = 0)
								   )
						     AND ((RG.ProtocolRights <> '00000000000000000000')
                              OR (RG.ResolutionRights <> '00000000000000000000')
                              OR (RG.DocumentRights <> '00000000000000000000')
                              OR (RG.DocumentSeriesRights <> '00000000000000000000'))
                           )
					 OR 
						exists (SELECT 1 
								FROM [dbo].[RoleUser] RU
								INNER JOIN [dbo].[CategoryFascicleRights] CFR on CFR.IdRole = RU.idRole
								INNER JOIN [dbo].[CategoryFascicles] CF on CFR.IdCategoryFascicle = CF.IdCategoryFascicle
								where CF.IdCategory = Fascicle.IdCategory
								and RU.Account=@Domain+'\'+@UserName
								and Fascicle.FascicleType IN (1, 2)
								and RU.[Type] in ('RP', 'SP')
								and (
										NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[FascicleRoles] WHERE IdFascicle = Fascicle.IdFascicle AND RoleAuthorizationType = 0 AND IsMaster = 1)
										OR (RU.idRole in ((SELECT idRole FROM [dbo].[FascicleRoles] WHERE IdFascicle = Fascicle.IdFascicle AND RoleAuthorizationType = 0 AND IsMaster = 1))))
									)
					--OR
					--	exists (
					--			SELECT 1
					--			FROM [dbo].[Contact] C
					--			INNER JOIN [dbo].[FascicleContacts] FC on FC.IdContact = C.Incremental
					--			where FC.IdFascicle = Fascicle.IdFascicle AND C.SearchCode = @Domain+'\'+@UserName
					--		   )
					OR
						exists (
								SELECT 1
								FROM [dbo].[ContainerGroup] CG
								where CG.idContainer = Fascicle.IdContainer
								AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup) 
								AND CG.FascicleRights LIKE '__1%'
							   )
					)
				))
			Group by Fascicle.IdFascicle, Fascicle.RegistrationDate, Fascicle.Title) T 
			where T.rownum > @Skip AND T.rownum <= @Top			
		)

SELECT F.IdFascicle AS UniqueId, F.Year, F.Number, F.EndDate, F.Title, F.Name, F.Object as FascicleObject, F.Manager,
	   F.IdCategory, F.FascicleType, F.VisibilityType, F.RegistrationUser, F.RegistrationDate, F_C.IdCategory as Category_idCategory,
	   F_C.Name as Category_Name, FC.IdContact AS Contact_Incremental, F_CON.Description AS Contact_Description
FROM Fascicles F
INNER JOIN MyFascicles MF ON MF.IdFascicle = F.IdFascicle
INNER JOIN Category F_C on F_C.idCategory = F.IdCategory
LEFT OUTER JOIN FascicleContacts FC ON FC.IdFascicle = F.IdFascicle
LEFT OUTER JOIN Contact F_CON ON F_CON.Incremental = FC.IdContact
);
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Create sql function [webapiprivate].[Fascicles_FX_IsManager]'
GO

CREATE FUNCTION [webapiprivate].[Fascicles_FX_IsManager](
	@UserName nvarchar(255),
	@Domain nvarchar(255),
	@IdFascicle uniqueidentifier
)
RETURNS BIT
AS
BEGIN
	DECLARE @IsManager BIT;
	declare @EmptyRights nvarchar(20);
	SET @EmptyRights = '00000000000000000000';

	WITH
	MySecurityGroups AS (
        SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
    ),
	MyResponsibleRoles AS (
		SELECT idRole FROM [dbo].[FascicleRoles] WHERE IdFascicle = @IdFascicle AND RoleAuthorizationType = 0
	)

	SELECT @IsManager = CAST(COUNT(1) AS BIT)
	FROM   dbo.Fascicles F
	WHERE F.IdFascicle = @IdFascicle
	AND (exists (select 1
				from [dbo].[RoleGroup] RG
                where F.FascicleType IN (-1,2,4)
				AND RG.IdRole in (select idRole from MyResponsibleRoles)
				AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup)  
				AND ((RG.ProtocolRights <> @EmptyRights)
					OR (RG.ResolutionRights <> @EmptyRights)
                    OR (RG.DocumentRights <> @EmptyRights)
                    OR (RG.DocumentSeriesRights <> @EmptyRights))
                )
		 OR
			exists (select 1
					from [dbo].[Contact] C
					INNER JOIN [dbo].[FascicleContacts] FC on FC.IdContact = C.Incremental
					where FC.IdFascicle = F.IdFascicle AND F.FascicleType IN (0,1)
					AND C.SearchCode = @Domain+'\'+@UserName
				   )
		 OR
			exists (select 1
					from [dbo].[ContainerGroup] CG
					where CG.idContainer = F.IdContainer
					AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup) 
					AND (CG.FascicleRights LIKE '1%' OR CG.FascicleRights LIKE '_1%')
				   )
		)
RETURN @IsManager
END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Create sql function [webapiprivate].[Category_FX_FindCategories]'
GO

CREATE FUNCTION [webapiprivate].[Category_FX_FindCategories]
(	
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@Name nvarchar(256),
	@LoadRoot bit,
	@ParentId smallint,
	@ParentAllDescendants bit,
	@FullCode nvarchar(256),
	@FascicleFilterEnabled bit,
	@FascicleType smallint,	
	@HasFascicleInsertRights bit,
	@Manager nvarchar(256),
	@Secretary nvarchar(256),
	@Role smallint
)
RETURNS TABLE 
AS
RETURN 
(
	WITH FoundCategories AS (
		SELECT C.idCategory
			  FROM [dbo].[Category] C
		      WHERE IsActive = 1 AND StartDate <= GETDATE() AND (EndDate is null OR EndDate > GETDATE())
			  AND (@LoadRoot = 1 AND C.idParent is null)
			  OR ((@LoadRoot is null OR @LoadRoot = 0) 
			  AND (@ParentId is null OR (((@ParentAllDescendants is null OR @ParentAllDescendants = 0) 
											AND C.idParent = @ParentId) OR (@ParentAllDescendants = 1 
											AND C.FullIncrementalPath like CAST(@ParentId AS nvarchar) + '|%')))
			  AND (@FullCode is null OR C.FullCode = @FullCode)
			  AND (@Name is null OR C.Name like '%'+@Name+'%')
			  AND ((@FascicleFilterEnabled is null OR @FascicleFilterEnabled = 0)
				   OR (@FascicleFilterEnabled = 1
					AND ((@HasFascicleInsertRights is null OR @HasFascicleInsertRights = 0 OR @FascicleType <> 1) OR 
						 (@HasFascicleInsertRights = 1 AND @FascicleType = 1 AND exists (SELECT DISTINCT 1 from [dbo].[CategoryFascicles] CF
							INNER JOIN [dbo].[CategoryFascicleRights] CFR on CFR.IdCategoryFascicle = CF.IdCategoryFascicle
							where CF.idCategory = C.idCategory
							AND (exists (SELECT 1 FROM [dbo].[RoleUser] RU				  				  
											  WHERE RU.idRole = CFR.IdRole
											  AND (RU.Account=@Domain+'\'+@UserName
											  AND Type IN ('RP','SP')))
								 OR exists (SELECT 1 FROM SecurityGroups SG
											INNER JOIN RoleGroup RG ON RG.idGroup = SG.idGroup
											WHERE RG.idRole = CFR.IdRole
											AND SG.AllUsers = 1)			  
							)))
						)
					AND (@FascicleType is null OR exists (SELECT 1 FROM [dbo].[CategoryFascicles] CF WHERE CF.idCategory = C.idCategory AND CF.FascicleType = @FascicleType))
					AND (@Manager is null OR exists (SELECT 1 FROM [dbo].[RoleUser] RU
						 INNER JOIN [dbo].[CategoryFascicleRights] CFR on CFR.IdRole = RU.idRole
						 INNER JOIN [dbo].[CategoryFascicles] CF on CFR.IdCategoryFascicle = CF.IdCategoryFascicle
						 WHERE CF.IdCategory = C.idCategory
						 AND RU.Account=@Manager
						 AND Type = 'RP')
						)
					AND (@Secretary is null OR exists (SELECT 1 FROM [dbo].[RoleUser] RU
						 INNER JOIN [dbo].[CategoryFascicleRights] CFR on CFR.IdRole = RU.idRole
						 INNER JOIN [dbo].[CategoryFascicles] CF on CFR.IdCategoryFascicle = CF.IdCategoryFascicle
						 WHERE CF.IdCategory = C.idCategory
						 AND RU.Account=@Secretary
						 AND Type = 'SP')
						)
					AND (@Role is null OR exists (SELECT 1 FROM [dbo].[CategoryFascicleRights] CFR
						 INNER JOIN [dbo].[CategoryFascicles] CF on CFR.IdCategoryFascicle = CF.IdCategoryFascicle
						 WHERE CF.IdCategory = C.idCategory
						 AND CFR.IdRole = @Role)
						)
					  )))		  			 
	)

	SELECT [Category].[idCategory] AS IdCategory,[Category].[Name],[Category].[isActive],[Category].[Code],[Category].[FullIncrementalPath],[Category].[FullCode],
	[Category].[RegistrationDate],[Category].[RegistrationUser],[Category].[UniqueId],[Category].[StartDate],[Category].[EndDate],

    [CategoryParent].[idCategory] AS CategoryParent_IdCategory,
	(
	 SELECT TOP 1 CAST(1 AS bit) FROM Category C_TMP 
			WHERE C_TMP.idParent = Category.idCategory
	) As HasChildren,
	(
	 CASE @FascicleType
	 WHEN null THEN CAST(0 AS bit)
	 WHEN 1 THEN (SELECT DISTINCT CAST(1 AS bit) from [dbo].[CategoryFascicles] CF
							INNER JOIN [dbo].[CategoryFascicleRights] CFR on CFR.IdCategoryFascicle = CF.IdCategoryFascicle
							where CF.idCategory = Category.idCategory
							AND (exists (SELECT 1 FROM [dbo].[RoleUser] RU				  				  
											  WHERE RU.idRole = CFR.IdRole
											  AND (RU.Account=@Domain+'\'+@UserName
											  AND Type IN ('RP','SP')))
								 OR exists (SELECT 1 FROM SecurityGroups SG
											INNER JOIN RoleGroup RG ON RG.idGroup = SG.idGroup
											WHERE RG.idRole = CFR.IdRole
											AND SG.AllUsers = 1)			  
							))
	 WHEN 2 THEN (SELECT DISTINCT CAST(1 AS bit) FROM [dbo].[CategoryFascicles] CF WHERE CF.idCategory = Category.idCategory AND CF.FascicleType = @FascicleType)
	 END
	) AS HasFascicleDefinition
	FROM Category
	LEFT OUTER JOIN Category CategoryParent ON CategoryParent.idCategory = Category.idParent
	WHERE Category.isActive = 1 AND
	Category.idCategory IN (SELECT idCategory from FoundCategories)
)
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Create sql function [webapiprivate].[Category_FX_FindCategory]'
GO

CREATE FUNCTION [webapiprivate].[Category_FX_FindCategory]
(	
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@CategoryId smallint,
	@FascicleType smallint
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT [Category].[idCategory] AS IdCategory,[Category].[Name],[Category].[isActive],[Category].[Code],[Category].[FullIncrementalPath],[Category].[FullCode],
	[Category].[RegistrationDate],[Category].[RegistrationUser],[Category].[UniqueId],[Category].[StartDate],[Category].[EndDate],
	[CategoryParent].[idCategory] AS CategoryParent_IdCategory,
	(SELECT TOP 1 CAST(1 AS bit) FROM Category C_TMP 
			WHERE C_TMP.idParent = Category.idCategory
	) As HasChildren, 
	(
	 CASE @FascicleType
	 WHEN null THEN CAST(0 AS bit)
	 WHEN 1 THEN (SELECT DISTINCT CAST(1 AS bit) from [dbo].[CategoryFascicles] CF
							INNER JOIN [dbo].[CategoryFascicleRights] CFR on CFR.IdCategoryFascicle = CF.IdCategoryFascicle
							where CF.idCategory = Category.idCategory
							AND (exists (SELECT 1 FROM [dbo].[RoleUser] RU				  				  
											  WHERE RU.idRole = CFR.IdRole
											  AND (RU.Account=@Domain+'\'+@UserName
											  AND Type IN ('RP','SP')))
								 OR exists (SELECT 1 FROM SecurityGroups SG
											INNER JOIN RoleGroup RG ON RG.idGroup = SG.idGroup
											WHERE RG.idRole = CFR.IdRole
											AND SG.AllUsers = 1)			  
							))
	 WHEN 2 THEN (SELECT DISTINCT CAST(1 AS bit) FROM [dbo].[CategoryFascicles] CF WHERE CF.idCategory = Category.idCategory AND CF.FascicleType = @FascicleType)
	 END
	) AS HasFascicleDefinition
	FROM Category
	LEFT OUTER JOIN Category CategoryParent ON CategoryParent.idCategory = Category.idParent
	WHERE Category.idCategory = @CategoryId
)
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Create sql function [webapiprivate].[Fascicles_FX_HasProcedureDistributionInsertRight]'
GO

CREATE FUNCTION [webapiprivate].[Fascicles_FX_HasProcedureDistributionInsertRight]
(
       @UserName nvarchar(256), 
       @Domain nvarchar(256),
	   @IdCategory smallint
)
RETURNS BIT
AS
BEGIN
       declare @HasRight bit;

       WITH
       MySecurityGroups AS (
        SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
       )

       SELECT  @HasRight = CAST(COUNT(1) AS BIT)
       FROM [dbo].[Category] C
	   WHERE C.FullIncrementalPath like CAST(@IdCategory AS nvarchar) + '|%'
	   AND exists (SELECT 1 FROM [dbo].[RoleUser] RU
					  INNER JOIN [dbo].[CategoryFascicleRights] CFR on CFR.IdRole = RU.idRole
					  INNER JOIN [dbo].[CategoryFascicles] CF on CFR.IdCategoryFascicle = CF.IdCategoryFascicle
					  WHERE FascicleType = 1
					  AND CF.idCategory = C.idCategory
					  AND RU.Account=@Domain + '\' + @UserName
					  AND Type in ('RP', 'SP')
					 )
       
       
       RETURN @HasRight
END
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Create index [webapiprivate].[IX_FascicleRoles_IdFascicle]'
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FascicleRoles]') AND name = N'IX_FascicleRoles_IdFascicle')
      DROP INDEX [IX_FascicleRoles_IdFascicle] ON [dbo].[FascicleRoles] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [IX_FascicleRoles_IdFascicle]
ON [dbo].[FascicleRoles] ([IdFascicle])
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################
PRINT N'Create index [webapiprivate].[IX_SecurityUsers_idGroup]'
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SecurityUsers]') AND name = N'IX_SecurityUsers_idGroup')
      DROP INDEX [IX_SecurityUsers_idGroup] ON [dbo].[SecurityUsers] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [IX_SecurityUsers_idGroup]
ON [dbo].[SecurityUsers] ([idGroup])
INCLUDE ([Account],[UserDomain])
GO


IF @@ERROR <> 0 AND @@TRANCOUNT > 0
    BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error) VALUES (1);
        BEGIN TRANSACTION;
    END
GO
--#############################################################################

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
	PRINT N'The transacted portion of the database update succeeded.'
COMMIT TRANSACTION
END
ELSE PRINT N'The transacted portion of the database update FAILED.'
GO
DROP TABLE #tmpErrors
GO