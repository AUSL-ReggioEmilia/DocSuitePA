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
PRINT 'ALTER FUNCTION [webapiprivate].[Fascicles_FX_AvailableFascicles]'
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
	    AND ((@Name is NOT null AND ( F.Title like '%'+@Name+'%' OR F.Object like '%'+@Name+'%')) OR (@Name Is null))
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
PRINT 'ALTER FUNCTION [webapiprivate].[Fascicle_FX_HasDocumentVisibilityRight]';
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
       declare @EmptyRights nvarchar(10);
       set @EmptyRights = '0000000000';

       WITH
       MySecurityGroups AS (
        SELECT SG.IdGroup 
        FROM [dbo].[SecurityGroups] SG 
        INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
        GROUP BY SG.IdGroup
    )

       SELECT  @HasRight = CAST(COUNT(1) AS BIT)
       FROM [dbo].[Fascicles] F
       WHERE F.IdFascicle = @IdFascicle
       AND F.VisibilityType = 1
       AND ( exists (select top 1 RG.idRole
                           from [dbo].[RoleGroup] RG
                           INNER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup
                           INNER JOIN [dbo].[FascicleRoles] FR on FR.IdFascicle = F.IdFascicle
                           where  RG.IdRole = FR.IdRole AND FR.RoleAuthorizationType in (0, 1) AND
                           MSG.IdGroup IS NOT NULL AND ((RG.ProtocolRights <> @EmptyRights)
                           OR (RG.ResolutionRights <> @EmptyRights)
                           OR (RG.DocumentRights <> @EmptyRights)
                           OR (RG.DocumentSeriesRights <> @EmptyRights))
                           )
             OR 
                exists (select top 1 CG.idCategory
                           from [dbo].[CategoryGroup] CG
                           INNER JOIN MySecurityGroups MSG on CG.IdGroup = MSG.IdGroup
                           where F.IdCategory = CG.IdCategory AND MSG.IdGroup IS NOT NULL AND CG.ProtocolRights like '__1%'
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
PRINT N'Modificata StoredProcedure per inserimento Containergroups';
GO
ALTER PROCEDURE [dbo].[ContainerGroups_Insert]
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
       @UDSRights char(10),
	   @PrivacyLevel int
	   AS 
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
BEGIN TRY
BEGIN TRANSACTION ContainerGroupInsert

	DECLARE @idContainerGroup uniqueidentifier
	SET @idContainerGroup = newid()

	     INSERT INTO <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[containergroup] ([idContainer],[Rights] , [GroupName], [ResolutionRights], [DocumentRights], [RegistrationUser], [RegistrationDate], [DocumentSeriesRights], [idGroup], [DeskRights], [UDSRights], [IdContainerGroup], [PrivacyLevel])
         VALUES(@idContainer, @Rights, @GroupName, @ResolutionRights, @DocumentRights, @RegistrationUser, @RegistrationDate, @DocumentSeriesRights, @idGroup, @DeskRights, @UDSRights, @idContainerGroup, @PrivacyLevel)

		 IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		 BEGIN 
		  INSERT INTO  <DBAtti, varchar(50), DBAtti>.[dbo].[containergroup] ([idContainer],[Rights] , [GroupName], [ResolutionRights], [DocumentRights], [RegistrationUser], [RegistrationDate], [DocumentSeriesRights], [idGroup], [DeskRights], [UDSRights], [IdContainerGroup], [PrivacyLevel])
          Values(@idContainer, @Rights, @idGroup, @ResolutionRights, @DocumentRights, @RegistrationUser, @RegistrationDate, @DocumentSeriesRights, @idGroup, @DeskRights, @UDSRights, @idContainerGroup, @PrivacyLevel)
		END
		IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
		BEGIN 
			INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[containergroup] ([idContainer],[Rights] , [GroupName], [ResolutionRights], [DocumentRights], [RegistrationUser], [RegistrationDate], [DocumentSeriesRights], [idGroup], [DeskRights], [UDSRights], [IdContainerGroup], [PrivacyLevel])
			VALUES(@idContainer, @Rights, @GroupName, @ResolutionRights, @DocumentRights, @RegistrationUser, @RegistrationDate, @DocumentSeriesRights, @idGroup, @DeskRights, @UDSRights, @idContainerGroup, @PrivacyLevel)
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
PRINT 'Modificata STORED PROCEDURE [dbo].[Container_Insert] ';
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
             EXEC [dbo].[ContainerGroups_Insert] @EntityShortId,@RightsFull, @SecurityGroupName, @ResolutionRightsFull, @DocumentRightsFull, @RegistrationUser, @RegistrationDate, @DocumentSeriesRightsFull, @SecurityGroupIdFull, @DeskRightsFull, @UDSRightsFull, @PrivacyLevel
             
			 --inserimento  gruppo con diritti di inserimento
			 SET @SecurityGroupName = @PrefixSecurityGroupName + '_ins'
             EXEC @SecurityGroupIdIns = [dbo].[SecurityGroups_Insert] @SecurityGroupName, null, null,  @RegistrationUser, @RegistrationDate, null, null, null, @TenantId, null, 0, null 
			 EXEC [dbo].[ContainerGroups_Insert] @EntityShortId,@RightsIns, @SecurityGroupName, @ResolutionRightsIns, @DocumentRightsIns, @RegistrationUser, @RegistrationDate, @DocumentSeriesRightsIns, @SecurityGroupIdIns, @DeskRightsIns, @UDSRightsIns, @PrivacyLevel
             
			 --inserimento gruppo con diritti di visualizzazione
			 SET @SecurityGroupName = @PrefixSecurityGroupName + '_vis'
             EXEC @SecurityGroupIdVis = [dbo].[SecurityGroups_Insert] @SecurityGroupName, null, null,  @RegistrationUser, @RegistrationDate, null, null, null, @TenantId, null, 0, null 
             EXEC [dbo].[ContainerGroups_Insert] @EntityShortId,@RightsVis, @SecurityGroupName, @ResolutionRightsVis, @DocumentRightsVis, @RegistrationUser, @RegistrationDate, @DocumentSeriesRightsVis, @SecurityGroupIdVis, @DeskRightsVis, @UDSRightsVis, @PrivacyLevel
       
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
PRINT 'Modificata STORED PROCEDURE [dbo].[Container_Update] ';
GO

ALTER PROCEDURE [dbo].[Container_Update] 
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
	@ContainerType smallint,
	@SecurityUserAccount nvarchar(256),
	@SecurityUserDisplayName nvarchar(256),
	@ManageSecureDocument bit,
	@PrivacyLevel int,
	@PrivacyEnabled bit
AS 

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRANSACTION StoreProcedure
	
	BEGIN TRY

	--Aggiornamento contenitore
	UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Container] SET [Name] = @Name, [Note] = @Note, [DocmLocation] = @DocmLocation, [ProtLocation] = @ProtLocation, [ReslLocation] = @ReslLocation,
		[Massive] = @Massive, [Conservation] = @Conservation,[LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser, [DocumentSeriesAnnexedLocation] = @DocumentSeriesAnnexedLocation,
		[DocumentSeriesLocation] = @DocumentSeriesLocation,	[DocumentSeriesUnpublishedAnnexedLocation] = @DocumentSeriesUnpublishedAnnexedLocation, [ProtocolRejection] = @ProtocolRejection,
		[ActiveFrom] = @ActiveFrom, [ActiveTo] = @ActiveTo, [idArchive] = @idArchive, [Privacy] = @Privacy, [HeadingFrontalino] = @HeadingFrontalino, [HeadingLetter] = @HeadingLetter,
		[ProtAttachLocation] = @ProtAttachLocation, [idProtocolType] = @idProtocolType, [DeskLocation] = @DeskLocation, [UDSLocation] = @UDSLocation, [ManageSecureDocument] = @ManageSecureDocument, [PrivacyLevel] = @PrivacyLevel, [PrivacyEnabled] = @PrivacyEnabled  Where [idContainer] = @idContainer

	IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN

		
		UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Container] SET [Name] = @Name, [Note] = @Note, [DocmLocation] = @DocmLocation, [ProtLocation] = @ProtLocation, [ReslLocation] = @ReslLocation,
		[Massive] = @Massive, [Conservation] = @Conservation,[LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser, [DocumentSeriesAnnexedLocation] = @DocumentSeriesAnnexedLocation,
		[DocumentSeriesLocation] = @DocumentSeriesLocation,	[DocumentSeriesUnpublishedAnnexedLocation] = @DocumentSeriesUnpublishedAnnexedLocation, [ProtocolRejection] = @ProtocolRejection,
		[ActiveFrom] = @ActiveFrom, [ActiveTo] = @ActiveTo, [idArchive] = @idArchive, [Privacy] = @Privacy, [HeadingFrontalino] = @HeadingFrontalino, [HeadingLetter] = @HeadingLetter,
		[ProtAttachLocation] = @ProtAttachLocation, [idProtocolType] = @idProtocolType, [DeskLocation] = @DeskLocation, [UDSLocation] = @UDSLocation, [ManageSecureDocument] = @ManageSecureDocument, [PrivacyLevel] = @PrivacyLevel, [PrivacyEnabled] = @PrivacyEnabled  Where [idContainer] = @idContainer
	END

	IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
	BEGIN 
	UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Container] SET [Name] = @Name, [Note] = @Note, [DocmLocation] = @DocmLocation, [ProtLocation] = @ProtLocation, [ReslLocation] = @ReslLocation,
		[Massive] = @Massive, [Conservation] = @Conservation,[LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser, [DocumentSeriesAnnexedLocation] = @DocumentSeriesAnnexedLocation,
		[DocumentSeriesLocation] = @DocumentSeriesLocation,	[DocumentSeriesUnpublishedAnnexedLocation] = @DocumentSeriesUnpublishedAnnexedLocation, [ProtocolRejection] = @ProtocolRejection,
		[ActiveFrom] = @ActiveFrom, [ActiveTo] = @ActiveTo, [idArchive] = @idArchive, [Privacy] = @Privacy, [HeadingFrontalino] = @HeadingFrontalino, [HeadingLetter] = @HeadingLetter,
		[ProtAttachLocation] = @ProtAttachLocation, [idProtocolType] = @idProtocolType, [DeskLocation] = @DeskLocation, [UDSLocation] = @UDSLocation, [ManageSecureDocument] = @ManageSecureDocument, [PrivacyLevel] = @PrivacyLevel, [PrivacyEnabled] = @PrivacyEnabled  Where [idContainer] = @idContainer
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
PRINT 'Modificata sql function [dbo].[UDSRepository_FX_ViewableRepositoriesByTypology]';
GO

ALTER FUNCTION [webapiprivate].[UDSRepository_FX_ViewableRepositoriesByTypology](
	@UserName nvarchar(256),
	@Domain nvarchar(256),
	@IDUDSTypology uniqueidentifier,
	@PECAnnexedEnabled bit
)
RETURNS TABLE
AS
RETURN
(
WITH
MySecurityGroups AS (
	SELECT SG.IdGroup 
	FROM [dbo].[SecurityGroups] SG 
	LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
	WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
	OR SG.AllUsers = 1
	GROUP BY SG.IdGroup
)

SELECT R.IdUDSRepository,
	   R.Name,
	   R.IdContainer as Container_IdContainer,
	   R.Version,
	   R.Status,
	   R.ActiveDate,
	   R.ExpiredDate, 
	   R.RegistrationUser,
	   R.RegistrationDate,
	   R.LastChangedUser,
	   R.LastChangedDate,
	   R.SequenceCurrentNumber,
	   R.SequenceCurrentYear,
	   R.DSWEnvironment,
	   R.Alias
FROM uds.UDSRepositories R
WHERE R.Status = 2 AND R.ActiveDate <= getutcdate() AND (R.ExpiredDate is null OR R.ExpiredDate >= getutcdate())  
	  AND EXISTS (select top 1 CG.IdContainerGroup
						from dbo.ContainerGroup CG
						INNER JOIN MySecurityGroups C_MSG on CG.idGroup = C_MSG.idGroup
						where CG.IdContainer = R.IdContainer and C_MSG.IdGroup is not null
							  and (CG.UDSRights like '__1%' or CG.UDSRights like '___1%'))
	  AND (@IDUDSTypology is null OR EXISTS (select top 1 RT.IdUDSRepositoryTypology
											 from uds.UDSRepositoryTypologies RT
											 where RT.IdUDSTypology = @IDUDSTypology and RT.IdUDSRepository = R.IdUDSRepository))
      AND (@PECAnnexedEnabled is null OR @PECAnnexedEnabled = 0 
	       OR (R.ModuleXML.exist('/*[(@PECEnabled) eq true()]') = 1 AND R.ModuleXML.exist('/*/*[local-name()=''Documents'']/*[local-name()=''DocumentAnnexed'']') = 1))
GROUP BY R.IdUDSRepository,
	     R.Name,
	     R.IdContainer,
	     R.Version,
	     R.Status,
		 R.ActiveDate,
		 R.ExpiredDate, 
		 R.RegistrationUser,
		 R.RegistrationDate,
		 R.LastChangedUser,
		 R.LastChangedDate,
	     R.SequenceCurrentNumber,
	     R.SequenceCurrentYear,
	     R.DSWEnvironment,
	     R.Alias
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
PRINT 'Modificata sql function [dbo].[UDSRepository_FX_InsertableRepositoriesByTypology]';
GO

ALTER FUNCTION [webapiprivate].[UDSRepository_FX_InsertableRepositoriesByTypology](
	@UserName nvarchar(256),
	@Domain nvarchar(256),
	@IDUDSTypology uniqueidentifier,
	@PECAnnexedEnabled bit
)
RETURNS TABLE
AS
RETURN
(
WITH
MySecurityGroups AS (
	SELECT SG.IdGroup 
	FROM [dbo].[SecurityGroups] SG 
	LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
	WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
	OR SG.AllUsers = 1
	GROUP BY SG.IdGroup
)

SELECT R.IdUDSRepository,
	   R.Name,
	   R.IdContainer as Container_IdContainer,
	   R.Version,
	   R.Status,
	   R.ActiveDate,
	   R.ExpiredDate, 
	   R.RegistrationUser,
	   R.RegistrationDate,
	   R.LastChangedUser,
	   R.LastChangedDate,
	   R.SequenceCurrentNumber,
	   R.SequenceCurrentYear,
	   R.DSWEnvironment,
	   R.Alias
FROM uds.UDSRepositories R
WHERE R.Status = 2 AND R.ActiveDate <= getutcdate() AND (R.ExpiredDate is null OR R.ExpiredDate >= getutcdate())  
	  AND EXISTS (select top 1 CG.IdContainerGroup
						from dbo.ContainerGroup CG
						INNER JOIN MySecurityGroups C_MSG on CG.idGroup = C_MSG.idGroup
						where CG.IdContainer = R.IdContainer and C_MSG.IdGroup is not null
							  and CG.UDSRights like '1%')
	  AND (@IDUDSTypology is null OR EXISTS (select top 1 RT.IdUDSRepositoryTypology
											 from uds.UDSRepositoryTypologies RT
											 where RT.IdUDSTypology = @IDUDSTypology and RT.IdUDSRepository = R.IdUDSRepository))
      AND (@PECAnnexedEnabled is null OR @PECAnnexedEnabled = 0 
	       OR (R.ModuleXML.exist('/*[(@PECEnabled) eq true()]') = 1 AND R.ModuleXML.exist('/*/*[local-name()=''Documents'']/*[local-name()=''DocumentAnnexed'']') = 1))
GROUP BY R.IdUDSRepository,
	     R.Name,
	     R.IdContainer,
	     R.Version,
	     R.Status,
		 R.ActiveDate,
		 R.ExpiredDate, 
		 R.RegistrationUser,
		 R.RegistrationDate,
		 R.LastChangedUser,
		 R.LastChangedDate,
	     R.SequenceCurrentNumber,
	     R.SequenceCurrentYear,
	     R.DSWEnvironment,
	     R.Alias
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