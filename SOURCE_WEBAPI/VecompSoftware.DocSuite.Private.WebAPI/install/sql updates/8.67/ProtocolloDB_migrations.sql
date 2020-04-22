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
PRINT N'Creazione StoredProcedure per inserimento Containergroups';
GO
CREATE PROCEDURE [dbo].[ContainerGroups_Insert]
	   @idContainer smallint,
	   @Rights char(10),
       @GroupName varchar(255), 
       @ResolutionRights char(10),
       @DocumentRights char(10),
	   @RegistrationUser nvarchar(256),
	   @RegistrationDate datetimeoffset(7),
       @DocumentSeriesRights char(10),
       @idGroup int,
       @DeskRights char(10),
       @UDSRights char(10)
	   AS 
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
BEGIN TRY
BEGIN TRANSACTION ContainerGroupInsert

	DECLARE @idContainerGroup uniqueidentifier
	SET @idContainerGroup = newid()

	     INSERT INTO <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[containergroup] ([idContainer],[Rights] , [GroupName], [ResolutionRights], [DocumentRights], [RegistrationUser], [RegistrationDate], [DocumentSeriesRights], [idGroup], [DeskRights], [UDSRights], [IdContainerGroup])
         VALUES(@idContainer, @Rights, @GroupName, @ResolutionRights, @DocumentRights, @RegistrationUser, @RegistrationDate, @DocumentSeriesRights, @idGroup, @DeskRights, @UDSRights, @idContainerGroup)

		 IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		 BEGIN 
		  INSERT INTO  <DBAtti, varchar(50), DBAtti>.[dbo].[containergroup] ([idContainer],[Rights] , [GroupName], [ResolutionRights], [DocumentRights], [RegistrationUser], [RegistrationDate], [DocumentSeriesRights], [idGroup], [DeskRights], [UDSRights], [IdContainerGroup])
          Values(@idContainer, @Rights, @idGroup, @ResolutionRights, @DocumentRights, @RegistrationUser, @RegistrationDate, @DocumentSeriesRights, @idGroup, @DeskRights, @UDSRights, @idContainerGroup)
		END
		IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
		BEGIN 
			INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[containergroup] ([idContainer],[Rights] , [GroupName], [ResolutionRights], [DocumentRights], [RegistrationUser], [RegistrationDate], [DocumentSeriesRights], [idGroup], [DeskRights], [UDSRights], [IdContainerGroup])
			VALUES(@idContainer, @Rights, @GroupName, @ResolutionRights, @DocumentRights, @RegistrationUser, @RegistrationDate, @DocumentSeriesRights, @idGroup, @DeskRights, @UDSRights, @idContainerGroup)
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
PRINT N'Creazione StoredProcedure per creazione SecurityGroup';
GO

CREATE PROCEDURE [dbo].[SecurityGroups_Insert] 
       @GroupName nvarchar(256), 
       @FullIncrementalPath nvarchar(256),
       @LogDescription nvarchar(256),
	   @RegistrationUser nvarchar(256),
       @RegistrationDate datetimeoffset(7),
       @LastChangedDate datetimeoffset(7),
       @LastChangedUser nvarchar(256), 
	   @UniqueId uniqueidentifier,
	   @TenantId uniqueidentifier,
	   @IdSecurityGroupTenant int,
	   @AllUsers bit,
	   @idGroupFather int
AS

DECLARE @LastUsedIdGroup INT, @EntityId INT, @SecurityGroupId uniqueidentifier

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
BEGIN TRY
BEGIN TRANSACTION SecurityGroupsInsert

SELECT top(1) @LastUsedIdGroup = IdGroup FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[SecurityGroups] ORDER BY idGroup DESC
IF(@LastUsedIdGroup is null)
BEGIN
	SET @LastUsedIdGroup = 0
END

SET @EntityId = @LastUsedIdGroup + 1
SET @SecurityGroupId = newid()
SET @FullIncrementalPath = @EntityId

IF(@idGroupFather IS NOT NULL)
BEGIN
	SET @FullIncrementalPath = @idGroupFather + '|' + @EntityId
END

INSERT INTO  <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[securitygroups] ([idGroup], [GroupName], [FullIncrementalPath], [idGroupFather], [RegistrationUser], [RegistrationDate], [AllUsers], [UniqueId], [TenantId], [IdSecurityGroupTenant])
       VALUES (@EntityId , @GroupName, @FullIncrementalPath, @idGroupFather, @RegistrationUser, @RegistrationDate, @AllUsers, @SecurityGroupId, @TenantId, @EntityId)

IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN	 
	INSERT INTO <DBAtti, varchar(50), DBAtti>.[dbo].[securitygroups] ([idGroup], [GroupName], [FullIncrementalPath], [idGroupFather], [RegistrationUser], [RegistrationDate], [AllUsers], [UniqueId], [TenantId], [IdSecurityGroupTenant])
       VALUES (@EntityId , @GroupName, @FullIncrementalPath, @idGroupFather, @RegistrationUser, @RegistrationDate, @AllUsers, @SecurityGroupId, @TenantId, @EntityId)
	END
	
IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 	
	BEGIN	 
	INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[securitygroups] ([idGroup], [GroupName], [FullIncrementalPath], [idGroupFather], [RegistrationUser], [RegistrationDate], [AllUsers], [UniqueId], [TenantId], [IdSecurityGroupTenant])
       VALUES (@EntityId , @GroupName, @FullIncrementalPath, @idGroupFather, @RegistrationUser, @RegistrationDate, @AllUsers, @SecurityGroupId, @TenantId, @EntityId)
	END

COMMIT TRANSACTION SecurityGroupsInsert
RETURN @EntityId
END TRY

BEGIN CATCH 
     print ERROR_MESSAGE() 
     ROLLBACK TRANSACTION SecurityGroupsInsert
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
PRINT N'Creazione StoredProcedure per creazione SecurityUser';
GO

CREATE PROCEDURE [dbo].[SecurityUsers_Insert] 
       @Account nvarchar(256), 
       @Description nvarchar(256),
       @UserDomain nvarchar(256),
	   @RegistrationUser nvarchar(256),
       @RegistrationDate datetimeoffset(7),
       @LastChangedDate datetimeoffset(7),
       @LastChangedUser nvarchar(256), 
	   @UniqueId uniqueidentifier,
	   @IdGroup int
AS 

DECLARE @LastUsedIdUser INT, @EntityId INT, @SecurityUserId uniqueidentifier

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
BEGIN TRY
BEGIN TRANSACTION SecurityUsersInsert

SELECT top(1) @LastUsedIdUser = IdUser FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[SecurityUsers] ORDER BY idUser DESC
IF(@LastUsedIdUser is null)
BEGIN
	SET @LastUsedIdUser = 0
END

SET @EntityId = @LastUsedIdUser + 1
SET @SecurityUserId = newid()

INSERT INTO  <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[SecurityUsers] ([idUser], [Account], [Description], [idGroup], [RegistrationUser], [RegistrationDate], [UserDomain], [UniqueId])
       VALUES (@EntityId , @Account, @Description, @IdGroup, @RegistrationUser, @RegistrationDate, @UserDomain, @SecurityUserId)

IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN	 
	INSERT INTO  <DBAtti, varchar(50), DBAtti>.[dbo].[SecurityUsers] ([idUser], [Account], [Description], [idGroup], [RegistrationUser], [RegistrationDate], [UserDomain], [UniqueId])
       VALUES (@EntityId , @Account, @Description, @IdGroup, @RegistrationUser, @RegistrationDate, @UserDomain, @SecurityUserId)
	END
	
IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 	
	BEGIN	 
	INSERT INTO  <DBPratiche, varchar(50), DBPratiche>.[dbo].[SecurityUsers] ([idUser], [Account], [Description], [idGroup], [RegistrationUser], [RegistrationDate], [UserDomain], [UniqueId])
       VALUES (@EntityId , @Account, @Description, @IdGroup, @RegistrationUser, @RegistrationDate, @UserDomain, @SecurityUserId)
	END

COMMIT TRANSACTION SecurityUsersInsert
END TRY

BEGIN CATCH 
     print ERROR_MESSAGE() 
     ROLLBACK TRANSACTION SecurityUsersInsert
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

PRINT N'Creazione StoredProcedure per inserimento Container';
GO

CREATE PROCEDURE [dbo].[Container_Insert] 
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
	   @SecurityUserDisplayName nvarchar(256)
AS 

       DECLARE @EntityShortId smallint, @LastUsedIdContainer smallint, @RightsFull nvarchar(10), @ResolutionRightsFull nvarchar(10), @DocumentRightsFull nvarchar(10), @DocumentSeriesRightsFull nvarchar(10), 
			   @DeskRightsFull nvarchar(10), @UDSRightsFull nvarchar(10),@RightsIns nvarchar(10), @ResolutionRightsIns nvarchar(10), @DocumentRightsIns nvarchar(10), @DocumentSeriesRightsIns nvarchar(10), 
			   @DeskRightsIns nvarchar(10), @UDSRightsIns nvarchar(10),@RightsVis nvarchar(10), @ResolutionRightsVis nvarchar(10), @DocumentRightsVis nvarchar(10), @DocumentSeriesRightsVis nvarchar(10), 
			   @DeskRightsVis nvarchar(10), @UDSRightsVis nvarchar(10), @SecurityUserName nvarchar(100), @SecurityUserDescription nvarchar(100), @SecurityUserDomain nvarchar(100), @SecurityGroupIdFull int, 
			   @SecurityGroupIdVis int, @SecurityGroupIdIns int, @SecurityGroupName nvarchar(256)


	SELECT  @LastUsedIdContainer = [LastUsedIdContainer] FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Parameter]

       --Ad oggi è implementata solo la gestione dei diritti degli archivi,
       --Per ottenere un corretto funzionamento con le altre UD occorre implementarne la gestione
       --diritti protocollo

       SET @RightsFull = 
             CASE 
                    WHEN @ContainerType = 1 THEN '0000000000'
                    ELSE '0000000000'
             END

       SET @RightsIns = 
             CASE 
                    WHEN @ContainerType = 1 THEN '0000000000'
                    ELSE '0000000000'
             END

       SET @RightsVis = 
             CASE 
                    WHEN @ContainerType = 1 THEN '0000000000'
                    ELSE '0000000000'
             END

             --diritti atti

       SET @ResolutionRightsFull = 
             CASE 
                    WHEN @ContainerType = 2 THEN '0000000000'
                    ELSE '0000000000'
             END

       SET @ResolutionRightsIns = 
             CASE 
                    WHEN @ContainerType = 2 THEN '0000000000'
                    ELSE '0000000000'
             END

       SET @ResolutionRightsVis = 
             CASE 
                    WHEN @ContainerType = 2 THEN '0000000000'
                    ELSE '0000000000'
             END

             --diritti pratiche

       SET @DocumentRightsFull = 
             CASE 
                    WHEN @ContainerType = 4 THEN '0000000000'
                    ELSE '0000000000'
             END

       SET @DocumentRightsIns = 
             CASE 
                    WHEN @ContainerType = 4 THEN '0000000000'
                    ELSE '0000000000'
             END

       SET @DocumentRightsVis = 
             CASE 
                    WHEN @ContainerType = 4 THEN '0000000000'
                    ELSE '0000000000'
             END           

             --diritti serie documentali

       SET @DocumentSeriesRightsFull = 
             CASE 
                    WHEN @ContainerType = 8 THEN '0000000000'
                    ELSE '0000000000'
             END

       SET @DocumentSeriesRightsIns = 
             CASE 
                    WHEN @ContainerType = 8 THEN '0000000000'
                    ELSE '0000000000'
             END

       SET @DocumentSeriesRightsVis = 
             CASE 
                    WHEN @ContainerType = 8 THEN '0000000000'
                    ELSE '0000000000'
             END    

             --diritti tavoli

       SET @DeskRightsFull = 
             CASE 
                    WHEN @ContainerType = 16 THEN '0000000000'
                    ELSE '0000000000'
             END

       SET @DeskRightsIns = 
             CASE 
                    WHEN @ContainerType = 16 THEN '0000000000'
                    ELSE '0000000000'
             END

       SET @DeskRightsVis = 
             CASE 
                    WHEN @ContainerType = 16 THEN '0000000000'
                    ELSE '0000000000'
             END    

             -- diritti UDS
             
       SET @UDSRightsFull = 
             CASE 
                    WHEN @ContainerType = 32 THEN '1111111100'
                    ELSE '0000000000'
             END

       SET @UDSRightsIns = 
             CASE 
                    WHEN @ContainerType = 32 THEN '1111100000'
                    ELSE '0000000000'
             END

       SET @UDSRightsVis = 
             CASE 
                    WHEN @ContainerType = 32 THEN '0011000000'
                    ELSE '0000000000'
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
             [ProtAttachLocation], [idProtocolType], [DeskLocation], [UDSLocation], [UniqueId]) 
             
       VALUES(@EntityShortId, @Name, @Note, @DocmLocation, @ProtLocation,@ReslLocation, @isActive, @Massive, @Conservation,   @RegistrationUser, @RegistrationDate, @DocumentSeriesAnnexedLocation, @DocumentSeriesLocation, @DocumentSeriesUnpublishedAnnexedLocation, @ProtocolRejection, @ActiveFrom,
             @ActiveTo, @idArchive, @Privacy, @HeadingFrontalino, @HeadingLetter, @ProtAttachLocation, @idProtocolType, @DeskLocation, @UDSLocation, @UniqueId)

	   IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
       BEGIN
             update  <DBAtti, varchar(50), DBAtti>.[dbo].[Parameter] set [LastUsedidContainer] = @EntityShortId
             --Inserimento contenitore
             INSERT INTO  <DBAtti, varchar(50), DBAtti>.[dbo].[Container]([idContainer], [Name], [Note], [DocmLocation], [ProtLocation], [ReslLocation], [isActive], [Massive], [Conservation],
                    [RegistrationUser], [RegistrationDate], [DocumentSeriesAnnexedLocation], [DocumentSeriesLocation], 
                    [DocumentSeriesUnpublishedAnnexedLocation], [ProtocolRejection], [ActiveFrom], [ActiveTo], [idArchive], [Privacy], [HeadingFrontalino], [HeadingLetter],
                    [ProtAttachLocation], [idProtocolType], [DeskLocation], [UDSLocation], [UniqueId]) 
             
             VALUES(@EntityShortId, @Name, @Note, @DocmLocation, @ProtLocation,@ReslLocation, @isActive, @Massive, @Conservation,@RegistrationUser, @RegistrationDate, 
					@DocumentSeriesAnnexedLocation, @DocumentSeriesLocation, @DocumentSeriesUnpublishedAnnexedLocation, @ProtocolRejection, @ActiveFrom,
                    @ActiveTo, @idArchive, @Privacy, @HeadingFrontalino, @HeadingLetter, @ProtAttachLocation, @idProtocolType, @DeskLocation, @UDSLocation, @UniqueId)

       END

	   IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
       BEGIN 
             update <DBPratiche, varchar(50), DBPratiche>.[dbo].[Parameter] set [LastUsedidContainer] = @EntityShortId

             --Inserimento contenitore
             INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[Container]([idContainer], [Name], [Note], [DocmLocation], [ProtLocation], [ReslLocation], [isActive], [Massive], [Conservation],
                    [RegistrationUser], [RegistrationDate], [DocumentSeriesAnnexedLocation], [DocumentSeriesLocation], 
                    [DocumentSeriesUnpublishedAnnexedLocation], [ProtocolRejection], [ActiveFrom], [ActiveTo], [idArchive], [Privacy], [HeadingFrontalino], [HeadingLetter],
                    [ProtAttachLocation], [idProtocolType], [DeskLocation], [UDSLocation], [UniqueId]) 
             
             VALUES(@EntityShortId, @Name, @Note, @DocmLocation, @ProtLocation,@ReslLocation, @isActive, @Massive, @Conservation, @RegistrationUser, @RegistrationDate, 
					@DocumentSeriesAnnexedLocation, @DocumentSeriesLocation, @DocumentSeriesUnpublishedAnnexedLocation, @ProtocolRejection, @ActiveFrom,
                    @ActiveTo, @idArchive, @Privacy, @HeadingFrontalino, @HeadingLetter, @ProtAttachLocation, @idProtocolType, @DeskLocation, @UDSLocation, @UniqueId)

        END

	   IF(@AutomaticSecurityGroups = 1)
       BEGIN 
             --inserimento  gruppo con tutti i diritti
			 SET @SecurityGroupName = @PrefixSecurityGroupName + '_full'
             EXEC @SecurityGroupIdFull = [dbo].[SecurityGroups_Insert] @SecurityGroupName, null, null,  @RegistrationUser, @RegistrationDate, null, null, null, @TenantId, null, 0, null 
             EXEC [dbo].[ContainerGroups_Insert] @EntityShortId,@RightsFull, @SecurityGroupName, @ResolutionRightsFull, @DocumentRightsFull, @RegistrationUser, @RegistrationDate, @DocumentSeriesRightsFull, @SecurityGroupIdFull, @DeskRightsFull, @UDSRightsFull
             
			 --inserimento  gruppo con diritti di inserimento
			 SET @SecurityGroupName = @PrefixSecurityGroupName + '_ins'
             EXEC @SecurityGroupIdIns = [dbo].[SecurityGroups_Insert] @SecurityGroupName, null, null,  @RegistrationUser, @RegistrationDate, null, null, null, @TenantId, null, 0, null 
			 EXEC [dbo].[ContainerGroups_Insert] @EntityShortId,@RightsIns, @SecurityGroupName, @ResolutionRightsIns, @DocumentRightsIns, @RegistrationUser, @RegistrationDate, @DocumentSeriesRightsIns, @SecurityGroupIdIns, @DeskRightsIns, @UDSRightsIns
             
			 --inserimento gruppo con diritti di visualizzazione
			 SET @SecurityGroupName = @PrefixSecurityGroupName + '_vis'
             EXEC @SecurityGroupIdVis = [dbo].[SecurityGroups_Insert] @SecurityGroupName, null, null,  @RegistrationUser, @RegistrationDate, null, null, null, @TenantId, null, 0, null 
             EXEC [dbo].[ContainerGroups_Insert] @EntityShortId,@RightsVis, @SecurityGroupName, @ResolutionRightsVis, @DocumentRightsVis, @RegistrationUser, @RegistrationDate, @DocumentSeriesRightsVis, @SecurityGroupIdVis, @DeskRightsVis, @UDSRightsVis
       
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
PRINT N'Creazione StoredProcedure per Eliminazione Container';
GO

CREATE PROCEDURE [dbo].[Container_Delete] 
	@idContainer smallint, 
	@DocmLocation smallint,
	@ProtLocation smallint,
	@ReslLocation smallint,
	@idProtocolType smallint,
	@DeskLocation smallint,
	@DocumentSeriesAnnexedLocation smallint,
	@DocumentSeriesLocation smallint,
	@DocumentSeriesUnpublishedAnnexedLocation smallint,
	@ProtAttachLocation smallint,
	@UDSLocation smallint
AS 
	DECLARE @LastChangedDate datetimeoffset(7)

	SET @LastChangedDate = GETDATE()

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRANSACTION StoreProcedure
	
	BEGIN TRY

	--Cancellazione logica contenitore
		UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Container]  SET [LastChangedDate] = @LastChangedDate,  [isActive] = 0 Where [idContainer] = @idContainer
	
	IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN
		UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Container]  SET [LastChangedDate] = @LastChangedDate,  [isActive] = 0 Where [idContainer] = @idContainer
	END

	IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
	BEGIN 
		UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Container]  SET [LastChangedDate] = @LastChangedDate,  [isActive] = 0 Where [idContainer] = @idContainer
	END
	COMMIT TRANSACTION StoreProcedure
	END TRY
	BEGIN CATCH 
		ROLLBACK TRANSACTION StoreProcedure
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
PRINT N'Creazione StoredProcedure per Aggiornamento Container';
GO

CREATE PROCEDURE [dbo].[Container_Update] 
	@idContainer smallint, 
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
	@ContainerType smallint
AS 

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRANSACTION StoreProcedure
	
	BEGIN TRY

	--Aggiornamento contenitore
	UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Container] SET [Name] = @Name, [Note] = @Note, [DocmLocation] = @DocmLocation, [ProtLocation] = @ProtLocation, [ReslLocation] = @ReslLocation,
		[Massive] = @Massive, [Conservation] = @Conservation,[LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser, [DocumentSeriesAnnexedLocation] = @DocumentSeriesAnnexedLocation,
		[DocumentSeriesLocation] = @DocumentSeriesLocation,	[DocumentSeriesUnpublishedAnnexedLocation] = @DocumentSeriesUnpublishedAnnexedLocation, [ProtocolRejection] = @ProtocolRejection,
		[ActiveFrom] = @ActiveFrom, [ActiveTo] = @ActiveTo, [idArchive] = @idArchive, [Privacy] = @Privacy, [HeadingFrontalino] = @HeadingFrontalino, [HeadingLetter] = @HeadingLetter,
		[ProtAttachLocation] = @ProtAttachLocation, [idProtocolType] = @idProtocolType, [DeskLocation] = @DeskLocation, [UDSLocation] = @UDSLocation Where [idContainer] = @idContainer

	IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN

		
		UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Container] SET [Name] = @Name, [Note] = @Note, [DocmLocation] = @DocmLocation, [ProtLocation] = @ProtLocation, [ReslLocation] = @ReslLocation,
		[Massive] = @Massive, [Conservation] = @Conservation,[LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser, [DocumentSeriesAnnexedLocation] = @DocumentSeriesAnnexedLocation,
		[DocumentSeriesLocation] = @DocumentSeriesLocation,	[DocumentSeriesUnpublishedAnnexedLocation] = @DocumentSeriesUnpublishedAnnexedLocation, [ProtocolRejection] = @ProtocolRejection,
		[ActiveFrom] = @ActiveFrom, [ActiveTo] = @ActiveTo, [idArchive] = @idArchive, [Privacy] = @Privacy, [HeadingFrontalino] = @HeadingFrontalino, [HeadingLetter] = @HeadingLetter,
		[ProtAttachLocation] = @ProtAttachLocation, [idProtocolType] = @idProtocolType, [DeskLocation] = @DeskLocation, [UDSLocation] = @UDSLocation Where [idContainer] = @idContainer
	END

	IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
	BEGIN 
	UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Container] SET [Name] = @Name, [Note] = @Note, [DocmLocation] = @DocmLocation, [ProtLocation] = @ProtLocation, [ReslLocation] = @ReslLocation,
		[Massive] = @Massive, [Conservation] = @Conservation,[LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser, [DocumentSeriesAnnexedLocation] = @DocumentSeriesAnnexedLocation,
		[DocumentSeriesLocation] = @DocumentSeriesLocation,	[DocumentSeriesUnpublishedAnnexedLocation] = @DocumentSeriesUnpublishedAnnexedLocation, [ProtocolRejection] = @ProtocolRejection,
		[ActiveFrom] = @ActiveFrom, [ActiveTo] = @ActiveTo, [idArchive] = @idArchive, [Privacy] = @Privacy, [HeadingFrontalino] = @HeadingFrontalino, [HeadingLetter] = @HeadingLetter,
		[ProtAttachLocation] = @ProtAttachLocation, [idProtocolType] = @idProtocolType, [DeskLocation] = @DeskLocation, [UDSLocation] = @UDSLocation Where [idContainer] = @idContainer
	END
	COMMIT TRANSACTION StoreProcedure
	END TRY
	BEGIN CATCH 
		ROLLBACK TRANSACTION StoreProcedure
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

PRINT N'Modificata sql function [Fascicles_FX_AvailableFascicles]';
GO

ALTER FUNCTION [webapiprivate].[Fascicles_FX_AvailableFascicles]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@idCategory smallint
)
RETURNS TABLE
AS
RETURN
(
WITH
MySecurityGroups AS (
	SELECT SG.IdGroup 
	FROM [dbo].[SecurityGroups] SG 
	INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
	WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
	GROUP BY SG.IdGroup
)

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
LEFT OUTER JOIN [dbo].[FascicleRoles] FR ON FR.idFascicle = F.idFascicle	
	WHERE (@idCategory = 0 OR  F.IdCategory = @idCategory) 
		AND f.EndDate IS NULL
		AND((F.FascicleType = 1 AND
				EXISTS (SELECT TOP 1 CG.IdCategory FROM [dbo].[CategoryGroup] CG
				INNER JOIN MySecurityGroups C_MSG ON CG.idgroup = C_MSG.idGroup
				WHERE CG.ProtocolRights LIKE '____1' AND CG.idcategory = F_C.idcategory				
			))
			OR (F.FascicleType = 4 AND FR.RoleAuthorizationType = 0 AND
				EXISTS (SELECT TOP 1 R.idRole from [dbo].[Role] R
				INNER JOIN [dbo].[RoleGroup] RG ON R.idRole = RG.idRole
				INNER JOIN MySecurityGroups R_MSG ON RG.idGroup = R_MSG.idGroup
				WHERE FR.idrole = R.idRole AND R.IsActive = 1 
					)
				)
			)	
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