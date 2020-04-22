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

PRINT N'Modifica SQLFUNCTION [webapiprivate].[DocumentUnit_FX_HasVisibilityRight]';
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_HasVisibilityRight] 
(
	@UserName nvarchar(256), 
    @Domain nvarchar(256),
	@IdDocumentUnit uniqueidentifier
)
RETURNS BIT
AS
BEGIN
    declare @HasRight bit;

	WITH
	MySecurityGroups AS (
        SELECT SG.IdGroup 
        FROM [dbo].[SecurityGroups] SG 
        LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		OR SG.AllUsers = 1
        GROUP BY SG.IdGroup
    )

	SELECT  @HasRight = CAST(COUNT(1) AS BIT)
	FROM cqrs.DocumentUnits DU
	LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DUR.IdDocumentUnit = DU.IdDocumentUnit
	WHERE DU.IdDocumentUnit = @IdDocumentUnit
	
	and ( exists ( select top 1 CG.idContainerGroup
				 from [dbo].[ContainerGroup] CG 
				 INNER JOIN MySecurityGroups C_MSG on CG.IdGroup = C_MSG.IdGroup
				 where CG.IdContainer = DU.IdContainer AND 
				 C_MSG.IdGroup IS NOT NULL AND ((DU.Environment = 1 AND (CG.Rights like '__1%'))
					OR (DU.Environment = 2 AND (CG.ResolutionRights like '__1%'))
					OR (DU.Environment = 3 AND (CG.DocumentSeriesRights like '__1%'))
					OR (DU.Environment > 99 AND (CG.UDSRights like '__1%'))
					))

	 OR exists (select top 1 RG.idRole
				from [dbo].[RoleGroup] RG
				INNER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup
				INNER JOIN Role R on RG.idRole = R.idRole
				where  R.UniqueId = DUR.UniqueIdRole AND
				MSG.IdGroup IS NOT NULL AND ((DU.Environment = 1 AND (RG.ProtocolRights like '1%'))
				OR (DU.Environment = 2 AND (RG.ResolutionRights like '1%'))
				OR (DU.Environment = 3 AND (RG.DocumentSeriesRights like '1%'))
				OR (DU.Environment > 99 AND (RG.DocumentSeriesRights like '1%'))
				))
	OR (DU.Environment = 1 and exists (select top 1 IdProtocolUser from ProtocolUsers PU where PU.UniqueIdProtocol = DU.IdDocumentUnit AND PU.Account= @Domain+'\'+@UserName AND PU.Type in (1,2)))
	OR (DU.Environment = 2 and exists (select top 1 idResolution from Resolution R where R.UniqueId = @IdDocumentUnit and R.EffectivenessDate is not null and R.EffectivenessUser is not null))
	OR (DU.Environment > 99 and exists (select top 1 IdDocumentUnitUser from cqrs.DocumentUnitUsers DUU where DUU.IdDocumentUnit = @IdDocumentUnit and DUU.Account = @Domain+'\'+@UserName))
	)
    
	RETURN @HasRight;

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
PRINT 'CREATA SQL FUNCTION [webapiprivate].[DocumentUnit_FX_IsAlreadyFascicolated]';
GO

CREATE FUNCTION [webapiprivate].[DocumentUnit_FX_IsAlreadyFascicolated](
	@IdDocumentUnit uniqueidentifier,
	@CategoryId smallint
)
RETURNS BIT
AS
BEGIN
	DECLARE @IsAlreadyFascicolated BIT;
	SELECT @IsAlreadyFascicolated = cast(count(1) as bit)
	FROM cqrs.DocumentUnits
	WHERE idDocumentUnit = @IdDocumentUnit
	AND idCategory IN (SELECT idCategory FROM categoryFascicles CF
						WHERE idCategory IN (SELECT idcategory FROM category C 
											WHERE @CategoryId IN (SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|'))
											)
						AND [fascicleType] IN (0,1,2))
	AND NOT idFascicle IS NULL
	RETURN @IsAlreadyFascicolated
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
PRINT 'Modificata sql function [dbo].[Fascicle_FX_HasDocumentVisibilityRight]';
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
       set @EmptyRights = '00000000000000000000';

       WITH
       MySecurityGroups AS (
        SELECT SG.IdGroup 
        FROM [dbo].[SecurityGroups] SG 
        LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		OR SG.AllUsers = 1
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
PRINT N'Creata store procedure [dbo].[FascicleFolder_Insert]'
GO

CREATE PROCEDURE [dbo].[FascicleFolder_Insert] 
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
	@ParentInsertId uniqueidentifier
	AS
	
	DECLARE @parentNode hierarchyid, @maxNode hierarchyid, @node hierarchyid, @allNode hierarchyid, @rootNode hierarchyid, @childNode hierarchyid,@insertiNode hierarchyid,@fascicleNode hierarchyid,@level smallint,@idSubFolder uniqueidentifier

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	
	
	BEGIN TRY
		BEGIN TRANSACTION InsertFascicleFolder
		-- Recupero il parent node
	    SELECT @parentNode = [FascicleFolderNode] FROM [dbo].[FascicleFolders] WHERE [IdFascicleFolder] = @ParentInsertId
		
		IF @parentNode IS NULL
			BEGIN
				SELECT @parentNode = MAX([FascicleFolderNode]) from [dbo].[FascicleFolders] where [FascicleFolderNode].GetAncestor(1) = hierarchyid::GetRoot() AND [IdFascicle] = @IdFascicle
			END
		
		IF @parentNode IS NULL
			BEGIN
				SET @parentNode = hierarchyid::GetRoot()
				SET @IdFascicleFolder = @IdFascicle 
			END

		IF @ParentInsertId IS NOT NULL			
			BEGIN
			    SELECT @childNode = MAX([FascicleFolderNode]) FROM [dbo].[FascicleFolders] WHERE [FascicleFolderNode].GetAncestor(1) = @parentNode;
				SET @node = @parentNode.GetDescendant(@childNode, NULL)
				
				
				INSERT INTO [dbo].[FascicleFolders]([FascicleFolderNode],[IdFascicleFolder],[IdFascicle],[IdCategory],[Name],[Status],[Typology],
				[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser]) 
				VALUES (@node, @IdFascicleFolder, @IdFascicle,@IdCategory, @Name, @Status, @Typology, @RegistrationDate, @RegistrationUser, NULL, NULL)
				
			END	
		ELSE
			BEGIN
			    SELECT @maxNode = MAX([FascicleFolderNode]) FROM [dbo].[FascicleFolders] WHERE [FascicleFolderNode].GetAncestor(1) = @parentNode;
				SET @allNode = @parentNode.GetDescendant(@maxNode, NULL)
				PRINT @allNode.ToString()
				
				INSERT INTO [dbo].[FascicleFolders]([FascicleFolderNode],[IdFascicleFolder],[IdFascicle],[IdCategory],[Name],[Status],[Typology],
													[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser]) 
				VALUES (@allNode, @IdFascicle, @IdFascicle, NULL, @Name, @Status, 0, @RegistrationDate, @RegistrationUser, NULL, NULL)
				SELECT @childNode = MAX([FascicleFolderNode]) FROM [dbo].[FascicleFolders] WHERE [FascicleFolderNode].GetAncestor(1) = @allNode;
				
				SET @insertiNode = @allNode.GetDescendant(@childNode, NULL)
				SET @idSubFolder = NEWID()
				INSERT INTO [dbo].[FascicleFolders]([FascicleFolderNode],[IdFascicleFolder],[IdFascicle],[IdCategory],[Name],[Status],[Typology],
													[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser]) 
				VALUES (@insertiNode, @idSubFolder, @IdFascicle, NULL, 'Inserti', @Status, 2, @RegistrationDate, @RegistrationUser, NULL, NULL)

				SELECT @childNode = MAX([FascicleFolderNode]) FROM [dbo].[FascicleFolders] WHERE [FascicleFolderNode].GetAncestor(1) = @allNode;
				SET @fascicleNode = @allNode.GetDescendant(@childNode, NULL)
				SET @idSubFolder = NEWID()
				INSERT INTO [dbo].[FascicleFolders]([FascicleFolderNode],[IdFascicleFolder],[IdFascicle],[IdCategory],[Name],[Status],[Typology],
													[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser]) 
				VALUES (@fascicleNode, @idSubFolder, @IdFascicle, @IdCategory, 'Fascicolo', @Status, 1, @RegistrationDate, @RegistrationUser, NULL, NULL)
				
				EXEC [dbo].[FascicleSubFolder_Insert] @IdFascicle, @fascicleNode, @IdCategory, @idSubFolder, @RegistrationUser 
			END
	    COMMIT TRANSACTION InsertFascicleFolder

		SELECT [FascicleFolderNode],[IdFascicleFolder] AS UniqueId,[IdFascicle],[IdCategory],[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser],[Status],
			[Name],[Typology],[FascicleFolderPath],[FascicleFolderLevel],[FascicleFolderParentNode],[ParentInsertId],[Timestamp] 
		FROM [dbo].[FascicleFolders] WHERE [IdFascicleFolder] = @IdFascicleFolder
		
		
	END TRY 
	BEGIN CATCH 
		ROLLBACK TRANSACTION InsertFascicleFolder
		
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
PRINT N'Creata stored procedure [FascicleFolder_Update]'
GO

CREATE PROCEDURE [dbo].[FascicleFolder_Update] 
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
		BEGIN TRANSACTION UpdateFascicleFolder
		UPDATE [dbo].[FascicleFolders] SET [IdFascicle] = @IdFascicle, [IdCategory] = @IdCategory, [Name] = @Name, [Status] = @Status, 
												[Typology] = @Typology, [LastChangedDate] = @LastChangedDate, [LastChangedUser] = @LastChangedUser
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
PRINT N'Creata stored procedure [FascicleFolder_Delete]'
GO
	CREATE  PROCEDURE [dbo].[FascicleFolder_Delete] 
       @IdFascicleFolder uniqueidentifier,       
       @IdFascicle uniqueidentifier, 
       @IdCategory smallint,
	   @Timestamp_Original timestamp
	AS
		
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  	
	
	BEGIN TRY
		BEGIN TRANSACTION DeleteFascicleFolder
		IF EXISTS (SELECT TOP 1 [IdFascicleFolder] FROM [FascicleFolders] WHERE [IdFascicleFolder] = @IdFascicleFolder)
		 BEGIN 
              DELETE [dbo].[FascicleFolders] WHERE [IdFascicleFolder] = @IdFascicleFolder
	     END 
		
		
		COMMIT TRANSACTION DeleteFascicleFolder
	END TRY
		
	BEGIN CATCH 
		ROLLBACK TRANSACTION DeleteFascicleFolder
		
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
PRINT N'Creata sql function [webapiprivate].[FascicleFolder_FX_AllChildrenByParent]'
GO

CREATE FUNCTION [webapiprivate].[FascicleFolder_FX_AllChildrenByParent] (
	@IdFascicleFolder uniqueidentifier
	)
	RETURNS TABLE
AS 
	RETURN
	(	  
	SELECT [FF].[IdFascicleFolder]
			,[FF].[Name]
			,[FF].[Status]
			,[FF].[Typology]
			,[FF].[IdFascicle] as Fascicle_IdFascicle
			,[FF].[IdCategory] as Category_IdCategory	
			,CAST((SELECT COUNT(IdFascicleFolder) AS Result
		from FascicleFolders 
		WHERE
		IdFascicleFolder = @IdFascicleFolder
		AND (EXISTS (SELECT 1 FROM FascicleProtocols WHERE idfasciclefolder = FF.IdFascicleFolder)
			OR EXISTS (select 1 from fascicleresolutions where idfasciclefolder = FF.IdFascicleFolder)
			OR EXISTS (select idfascicleFolder from fascicleUDS where idfasciclefolder =FF.IdFascicleFolder)
			OR EXISTS (select idfascicleFolder from fascicleDocuments where idfasciclefolder = FF.IdFascicleFolder)
			OR EXISTS (select idfascicleFolder from fascicleDocumentSeriesItems where idfasciclefolder = FF.IdFascicleFolder))) AS BIT) as HasDocuments
			,CAST(COUNT(FFF.IdFascicleFolder) AS BIT) as HasChildren
			, [FF].[FascicleFolderLevel]	
	   FROM FascicleFolders FF 
	   LEFT OUTER JOIN FascicleFolders AS FFF
	   ON FF.FascicleFolderNode = FFF.FascicleFolderNode.GetAncestor(1)
	   WHERE FF.FascicleFolderNode.GetAncestor(1) = (SELECT TOP 1 FascicleFolderNode 
												 FROM FascicleFolders 
												 WHERE IdFascicleFolder = @IdFascicleFolder)				 
		GROUP BY [FF].[IdFascicleFolder]
		        ,[FF].[Name]
				,[FF].[Status]
				,[FF].[Typology]
				,[FF].[IdFascicle]
				,[FF].[IdCategory]	
				,[FF].[FascicleFolderLevel]
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
PRINT N'Creata sql function [webapiprivate].[FascicleFolder_FX_NameAlreadyExists]'
GO

CREATE FUNCTION [webapiprivate].[FascicleFolder_FX_NameAlreadyExists] 
(
    @FolderName nvarchar(256), 
	@IdParent uniqueidentifier,
	@IdFascicle uniqueidentifier
)
RETURNS BIT
AS
BEGIN
    declare @HasAlreadyExistingName bit;
	
	SELECT  @HasAlreadyExistingName = CAST(COUNT(1) AS BIT)
	FROM FascicleFolders 
	WHERE FascicleFolderNode.GetAncestor(1) = (SELECT TOP(1) FascicleFoldernode 
	                                          FROM FascicleFolders 
											  WHERE IdFascicleFolder = @IdParent)
	      AND Name= @FolderName
		  AND IdFascicle = @IdFascicle
	RETURN @HasAlreadyExistingName;
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
PRINT N'Creata sql function [webapiprivate].[FascicleFolder_FX_CountChildren]'
GO

CREATE FUNCTION [webapiprivate].[FascicleFolder_FX_CountChildren] (
	@IdFascicleFolder uniqueidentifier
	)
RETURNS INT
AS
	BEGIN
	DECLARE @CountChildren INT;
		SELECT @CountChildren = COUNT(FF.IdFascicleFolder)
		FROM FascicleFolders FF 
		WHERE FF.FascicleFolderNode.GetAncestor(1) = (SELECT TOP 1 FascicleFolderNode
	    											 FROM FascicleFolders 
												     WHERE IdFascicleFolder = @IdFascicleFolder)
		RETURN @CountChildren
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
PRINT N'Creata store procedure [dbo].[FascicleSubFolder_Insert]'
GO

CREATE PROCEDURE [dbo].[FascicleSubFolder_Insert]
@IdFascicle uniqueidentifier,
@ParentNode hierarchyid,
@IdParentCategory smallint,
@IdParentFolder uniqueidentifier,
@RegistrationUser nvarchar(256)
AS
DECLARE @categoryFascicleId smallint,@idcategory smallint,@node hierarchyid,@childNode hierarchyid, @name nvarchar(256), @idFascicleFolder uniqueidentifier

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  

BEGIN TRY
		BEGIN TRANSACTION FascicleSubFolderInsert
		DECLARE category_cursor CURSOR LOCAL READ_ONLY FOR 
			SELECT C.name, C.idcategory FROM category C 
			WHERE C.idparent = @IdParentCategory and 
				exists (select F.IdCategoryFascicle FROM CategoryFascicles F where F.FascicleType = 0 and F.IdCategory = C.IdCategory)
		
		OPEN category_cursor
		FETCH NEXT FROM category_cursor INTO @name, @idcategory  
		WHILE @@FETCH_STATUS = 0  
		BEGIN
			SELECT @childNode = MAX([FascicleFolderNode]) FROM [dbo].[FascicleFolders] WHERE [FascicleFolderNode].GetAncestor(1) = @ParentNode;
			SET @node = @ParentNode.GetDescendant(@childNode, NULL)
		    SET @idFascicleFolder = NEWID()
			
			INSERT INTO [dbo].[FascicleFolders]([FascicleFolderNode],[IdFascicleFolder],[IdFascicle],[IdCategory],[Name],[Status],[Typology],
				[RegistrationDate],[RegistrationUser],[LastChangedDate],[LastChangedUser]) 
			VALUES (@node, @idFascicleFolder, @IdFascicle,@idcategory, @name, 1, 4, getutcdate(), @RegistrationUser, NULL, NULL)
			EXEC [dbo].[FascicleSubFolder_Insert] @IdFascicle, @node, @idcategory, @idFascicleFolder , @RegistrationUser
			FETCH NEXT FROM category_cursor INTO  @name, @idcategory  
		END   
		CLOSE category_cursor;  
		DEALLOCATE category_cursor;  
		
		COMMIT TRANSACTION FascicleSubFolderInsert		
		
END TRY 
BEGIN CATCH 
ROLLBACK TRANSACTION FascicleSubFolderInsert
		
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
PRINT N'Creata sql function [webapiprivate].[FascicleFolder_FX_GetParent]'
GO

CREATE FUNCTION [webapiprivate].[FascicleFolder_FX_GetParent] (
	@IdFascicleFolder uniqueidentifier
	)
	RETURNS TABLE
AS 
	RETURN
	(	  

	SELECT [FF].[IdFascicleFolder]
			,[FF].[Name]
			,[FF].[Status]
			,[FF].[Typology]
			,[FF].[IdFascicle] as Fascicle_IdFascicle
			,[FF].[IdCategory] as Category_IdCategory	
			,cast((select count(IdFascicleFolder) as Result from fascicleFolders where (EXISTS (SELECT 1 FROM FascicleProtocols WHERE idfasciclefolder = FF.IdFascicleFolder)
			OR EXISTS (select 1 from fascicleresolutions where idfasciclefolder = FF.IdFascicleFolder)
			OR EXISTS (select idfascicleFolder from fascicleUDS where idfasciclefolder =FF.IdFascicleFolder)
			OR EXISTS (select idfascicleFolder from fascicleDocuments where idfasciclefolder = FF.IdFascicleFolder)
			OR EXISTS (select idfascicleFolder from fascicleDocumentSeriesItems where idfasciclefolder = FF.IdFascicleFolder)) ) as bit) as HasDocuments
			,cast(1 as bit) as HasChildren
			, [FF].[FascicleFolderLevel]	
	   FROM FascicleFolders FF 	   
	   WHERE FF.FascicleFolderNode = (select FascicleFolderNode.GetAncestor(1) from fasciclefolders where idfasciclefolder=@IdFascicleFolder)
	   and FascicleFolderLevel > 1
			 
		GROUP BY [FF].[IdFascicleFolder]
		        ,[FF].[Name]
				,[FF].[Status]
				,[FF].[Typology]
				,[FF].[IdFascicle]
				,[FF].[IdCategory]	
				,[FF].[FascicleFolderLevel]
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
PRINT N'Creata sql function [webapiprivate].[DocumentUnit_FX_IsOnlyReferenziable]'
GO

CREATE FUNCTION [webapiprivate].[DocumentUnit_FX_IsOnlyReferenziable](
	@IdDocumentUnit uniqueidentifier,
	@CategoryFolderId smallint
)
RETURNS BIT
AS
BEGIN
	DECLARE @IsOnlyReferenziable BIT;
	select @IsOnlyReferenziable = cast(count(1) as bit)
	from cqrs.documentunits DU
	where ((select [webapiprivate].[DocumentUnit_FX_IsAlreadyFascicolated](@IdDocumentUnit, DU.IdCategory)) = 1)
	OR (iddocumentUnit = @IdDocumentUnit  and  @CategoryFolderId not in 
		(SELECT Value FROM [dbo].[SplitString]((select FullIncrementalPath from category where idcategory = DU.IdCategory), '|')))

	RETURN @IsOnlyReferenziable
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
PRINT N'Modificata sql function [webapiprivate].[DocumentUnit_FX_FascicleDocumentUnits]'
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_FascicleDocumentUnits]
(
    @FascicleId uniqueidentifier,
	@IdFascicleFolder uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(    
	SELECT DU.[IdDocumentUnit] as UniqueId
      ,DU.[IdFascicle]
      ,DU.[EntityId]
      ,DU.[Year]
      ,CAST(DU.[Number] as varchar) as Number
      ,DU.[Title]
      ,DU.[Subject]
      ,DU.[DocumentUnitName]
      ,DU.[Environment]
      ,DU.[RegistrationUser]
      ,DU.[RegistrationDate]
	  ,COALESCE(FP.ReferenceType, FR.ReferenceType, FD.ReferenceType, FU.ReferenceType) as ReferenceType
	  ,FU.IdUDSRepository as IdUDSRepository
	  ,CT.idCategory as Category_IdCategory
      ,CT.Name as Category_Name
      ,C.idContainer as Container_IdContainer
      ,C.Name as Container_Name
	FROM cqrs.DocumentUnits DU
	LEFT OUTER JOIN dbo.FascicleProtocols FP ON FP.UniqueIdProtocol = DU.IdDocumentUnit
	LEFT OUTER JOIN dbo.FascicleResolutions FR ON FR.UniqueIdResolution = DU.IdDocumentUnit
	LEFT OUTER JOIN dbo.FascicleDocumentSeriesItems FD ON FD.UniqueIdDocumentSeriesItem = DU.IdDocumentUnit
	LEFT OUTER JOIN dbo.FascicleUDS FU ON FU.IdUDS = DU.IdDocumentUnit
	INNER JOIN [dbo].[Category] CT on CT.idCategory = DU.idCategory
	INNER JOIN [dbo].[Container] C on C.idContainer = DU.idContainer
	WHERE 
	(FP.UniqueIdProtocol IS NOT NULL AND FP.IdFascicle = @FascicleId AND ((@IdFascicleFolder IS NOT NULL AND FP.IdFascicleFolder = @IdFascicleFolder) OR (@IdFascicleFolder IS NULL)))
	OR (FR.UniqueIdResolution IS NOT NULL AND FR.IdFascicle = @FascicleId AND ((@IdFascicleFolder IS NOT NULL AND FR.IdFascicleFolder = @IdFascicleFolder) OR (@IdFascicleFolder IS NULL)))
	OR (FD.UniqueIdDocumentSeriesItem IS NOT NULL AND FD.IdFascicle = @FascicleId AND ((@IdFascicleFolder IS NOT NULL AND FD.IdFascicleFolder = @IdFascicleFolder) OR (@IdFascicleFolder IS NULL)))
	OR (FU.IdUDS IS NOT NULL AND FU.IdFascicle = @FascicleId AND ((@IdFascicleFolder IS NOT NULL AND FU.IdFascicleFolder = @IdFascicleFolder) OR (@IdFascicleFolder IS NULL)))

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
PRINT N'Create sql function [webapiprivate].[IsSQL2012Compatible]'
GO

CREATE FUNCTION [dbo].[IsSQL2012Compatible]
(
	
)
RETURNS bit
AS
BEGIN
	DECLARE @SQLVersion AS INT

	SELECT TOP 1 @SQLVersion = value
	FROM [dbo].[SplitString](CAST(SERVERPROPERTY('productversion') AS NVARCHAR(MAX)), '.')

	RETURN CASE
		WHEN @SQLVersion >= 11 THEN 1
		ELSE 0
	END
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
PRINT N'Create sql function [webapiprivate].[Conservation_FX_AvailableDocumentSeriesItemLogsToConservate]'
GO

IF ([dbo].[IsSQL2012Compatible]()) = 1
	BEGIN
		EXECUTE sp_executesql N'CREATE FUNCTION [webapiprivate].[Conservation_FX_AvailableDocumentSeriesItemLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(			
			SELECT DSIL.UniqueId AS Id, UniqueIdDocumentSeriesItem AS ReferenceUniqueId, Subject, CAST(Year AS SMALLINT) AS Year, Number, 
			''DocumentSeriesItemLog'' AS EntityName, ''Archivio'' AS ReferenceEntityName, CAST(LogDate AS datetimeoffset(7)) AS LogDate, LogType, 
			SystemUser AS RegistrationUser, LogDescription AS Description, Hash 
			FROM DocumentSeriesItemLog DSIL
			INNER JOIN DocumentSeriesItem DSI ON DSI.UniqueId = DSIL.UniqueIdDocumentSeriesItem
			WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = DSIL.UniqueId)
			ORDER BY DSIL.Id
			OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
		)'		
	END
ELSE
	BEGIN
		EXECUTE sp_executesql N'CREATE FUNCTION [webapiprivate].[Conservation_FX_AvailableDocumentSeriesItemLogsToConservate]
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
				) AS T
			)

			SELECT Id, ReferenceUniqueId, Subject, CAST(Year AS SMALLINT) AS Year, Number, 
			''DocumentSeriesItemLog'' AS EntityName, ''Archivio'' AS ReferenceEntityName, CAST(LogDate AS datetimeoffset(7)) AS LogDate, LogType, RegistrationUser, Description, Hash 
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
PRINT N'Create sql function [webapiprivate].[Conservation_FX_AvailableDossierLogsToConservate]'
GO

IF ([dbo].[IsSQL2012Compatible]()) = 1
	BEGIN
		EXECUTE sp_executesql N'CREATE FUNCTION [webapiprivate].[Conservation_FX_AvailableDossierLogsToConservate]
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
			ORDER BY DL.LogDate
			OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
		)'	
	END
ELSE
	BEGIN
		EXECUTE sp_executesql N'CREATE FUNCTION [webapiprivate].[Conservation_FX_AvailableDossierLogsToConservate]
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
PRINT N'Create sql function [webapiprivate].[Conservation_FX_AvailableFascicleLogsToConservate]'
GO

IF ([dbo].[IsSQL2012Compatible]()) = 1
	BEGIN
		EXECUTE sp_executesql N'CREATE FUNCTION [webapiprivate].[Conservation_FX_AvailableFascicleLogsToConservate]
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
			ORDER BY FL.LogDate
			OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
		)'	
	END
ELSE
	BEGIN
		EXECUTE sp_executesql N'CREATE FUNCTION [webapiprivate].[Conservation_FX_AvailableFascicleLogsToConservate]
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
PRINT N'Create sql function [webapiprivate].[Conservation_FX_AvailablePECMailLogsToConservate]'
GO

IF ([dbo].[IsSQL2012Compatible]()) = 1
	BEGIN
		EXECUTE sp_executesql N'CREATE FUNCTION [webapiprivate].[Conservation_FX_AvailablePECMailLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(			
			SELECT PML.UniqueId AS Id, PM.UniqueId AS ReferenceUniqueId, ''PECMailLog'' AS EntityName, ''PECMail'' AS ReferenceEntityName, PM.MailSubject AS Subject,
			CAST(Date AS datetimeoffset(7)) AS LogDate, Type AS LogType, SystemUser AS RegistrationUser, Description, Hash 
			FROM PECMailLog PML
			INNER JOIN PECMail PM ON PM.IdPecMail = IDMail
			WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = PML.UniqueId)
			ORDER BY PML.Id
			OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
		)'	
	END
ELSE
	BEGIN
		EXECUTE sp_executesql N'CREATE FUNCTION [webapiprivate].[Conservation_FX_AvailablePECMailLogsToConservate]
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
				) AS T
			)

			SELECT Id, ReferenceUniqueId, ''PECMailLog'' AS EntityName, ''PECMail'' AS ReferenceEntityName, Subject, CAST(LogDate AS datetimeoffset(7)) AS LogDate, LogType, RegistrationUser, Description, Hash FROM TMP
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
PRINT N'Create sql function [webapiprivate].[Conservation_FX_AvailableProtocolLogsToConservate]'
GO

IF ([dbo].[IsSQL2012Compatible]()) = 1
	BEGIN
		EXECUTE sp_executesql N'CREATE FUNCTION [webapiprivate].[Conservation_FX_AvailableProtocolLogsToConservate]
		(	
			@Skip AS integer,
			@Take AS integer
		)
		RETURNS TABLE 
		AS
		RETURN 
		(			
			SELECT PL.UniqueId AS Id, UniqueIdProtocol AS ReferenceUniqueId, ''ProtocolLog'' AS EntityName, ''Protocollo'' AS ReferenceEntityName,
			P.Object AS Subject, P.Year AS Year, P.Number AS Number, CAST(LogDate AS datetimeoffset(7)) AS LogDate, LogType, SystemUser AS RegistrationUser, LogDescription AS Description, Hash 
			FROM ProtocolLog PL
			INNER JOIN Protocol P ON P.UniqueId = PL.UniqueIdProtocol
			WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = PL.UniqueId)
			ORDER BY PL.Id
			OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
		)'
	END
ELSE
	BEGIN
		EXECUTE sp_executesql N'CREATE FUNCTION [webapiprivate].[Conservation_FX_AvailableProtocolLogsToConservate]
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
				) AS T
			)

			SELECT Id, ReferenceUniqueId, ''ProtocolLog'' AS EntityName, ''Protocollo'' AS ReferenceEntityName, Subject,
			Year, Number, CAST(LogDate AS datetimeoffset(7)) AS LogDate, 
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
PRINT N'Create sql function [webapiprivate].[Conservation_FX_AvailableTableLogsToConservate]'
GO

IF ([dbo].[IsSQL2012Compatible]()) = 1
	BEGIN
		EXECUTE sp_executesql N'CREATE FUNCTION [webapiprivate].[Conservation_FX_AvailableTableLogsToConservate]
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
			ORDER BY LogDate
			OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
		)'
	END
ELSE
	BEGIN
		EXECUTE sp_executesql N'CREATE FUNCTION [webapiprivate].[Conservation_FX_AvailableTableLogsToConservate]
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
PRINT N'Create sql function [webapiprivate].[Conservation_FX_AvailableUDSLogsToConservate]'
GO

IF ([dbo].[IsSQL2012Compatible]()) = 1
	BEGIN
		EXECUTE sp_executesql N'CREATE FUNCTION [webapiprivate].[Conservation_FX_AvailableUDSLogsToConservate]
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
			ORDER BY LogDate
			OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
		)'
	END
ELSE
	BEGIN
		EXECUTE sp_executesql N'CREATE FUNCTION [webapiprivate].[Conservation_FX_AvailableUDSLogsToConservate]
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
PRINT N'Create sql function [webapiprivate].[Conservation_FX_CountDocumentSeriesItemLogsToConservate]'
GO

CREATE FUNCTION [webapiprivate].[Conservation_FX_CountDocumentSeriesItemLogsToConservate]
(	
	
)
RETURNS INT 
AS
BEGIN 
	DECLARE @Result AS integer

	SELECT @Result = COUNT(UniqueId)
	FROM DocumentSeriesItemLog
	WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = UniqueId)

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
PRINT N'Create sql function [webapiprivate].[Conservation_FX_CountDossierLogsToConservate]'
GO

CREATE FUNCTION [webapiprivate].[Conservation_FX_CountDossierLogsToConservate]
(	
	
)
RETURNS INT 
AS
BEGIN 
	DECLARE @Result AS integer

	SELECT @Result = COUNT(IdDossierLog)
	FROM DossierLogs
	WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = IdDossierLog)

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
PRINT N'Create sql function [webapiprivate].[Conservation_FX_CountFascicleLogsToConservate]'
GO

CREATE FUNCTION [webapiprivate].[Conservation_FX_CountFascicleLogsToConservate]
(	
	
)
RETURNS INT 
AS
BEGIN 
	DECLARE @Result AS integer

	SELECT @Result = COUNT(IdFascicleLog)
	FROM FascicleLogs
	WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = IdFascicleLog)

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
PRINT N'Create sql function [webapiprivate].[Conservation_FX_CountPECMailLogsToConservate]'
GO

CREATE FUNCTION [webapiprivate].[Conservation_FX_CountPECMailLogsToConservate]
(	
	
)
RETURNS INT 
AS
BEGIN 
	DECLARE @Result AS integer

	SELECT @Result = COUNT(PML.UniqueId)
	FROM PECMailLog PML
	INNER JOIN PECMail PM ON PM.IdPecMail = IDMail
	WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = PML.UniqueId)

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
PRINT N'Create sql function [webapiprivate].[Conservation_FX_CountProtocolLogsToConservate]'
GO

CREATE FUNCTION [webapiprivate].[Conservation_FX_CountProtocolLogsToConservate]
(	
	
)
RETURNS INT 
AS
BEGIN 
	DECLARE @Result AS integer

	SELECT @Result = COUNT(UniqueId)
	FROM ProtocolLog
	WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = UniqueId)

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
PRINT N'Create sql function [webapiprivate].[Conservation_FX_CountTableLogsToConservate]'
GO

CREATE FUNCTION [webapiprivate].[Conservation_FX_CountTableLogsToConservate]
(	
	
)
RETURNS INT 
AS
BEGIN 
	DECLARE @Result AS integer

	SELECT @Result = COUNT(IdTableLog)
	FROM TableLog
	WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = IdTableLog)

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
PRINT N'Create sql function [webapiprivate].[Conservation_FX_CountUDSLogsToConservate]'
GO

CREATE FUNCTION [webapiprivate].[Conservation_FX_CountUDSLogsToConservate]
(	
	
)
RETURNS INT 
AS
BEGIN 
	DECLARE @Result AS integer

	SELECT @Result = COUNT(IdUDSLog)
	FROM uds.UDSLogs
	WHERE NOT EXISTS (SELECT 1 FROM conservation.Conservations WHERE IdConservation = IdUDSLog)

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
PRINT N'Modificata sql function [webapiprivate].[Dossiers_FX_CountAuthorizedDossiers]'
GO

ALTER FUNCTION [webapiprivate].[Dossiers_FX_CountAuthorizedDossiers](
	@UserName nvarchar(255),
	@Domain nvarchar(255),
	@Year smallint,
	@Number smallint,
	@Subject nvarchar(255),
	@ContainerId smallint,
	@MetadataRepositoryId uniqueidentifier,
	@MetadataValue nvarchar(255)
)
RETURNS INT
AS
	BEGIN
	DECLARE @CountDossiers INT;
	WITH 	
	MySecurityGroups AS (
		SELECT SG.IdGroup 
		FROM [dbo].[SecurityGroups] SG 
		LEFT JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		OR SG.AllUsers = 1
		GROUP BY SG.IdGroup
	)
	
	SELECT @CountDossiers = COUNT(DISTINCT Dossier.IdDossier)
	FROM dbo.Dossiers Dossier
	left join dbo.DossierContacts DC on Dossier.IdDossier = Dc.IdDossier
	left join dbo.Contact Contact on DC.IdContact = Contact.Incremental
	inner join dbo.Container C on Dossier.IdContainer = C.idContainer
	inner join dbo.DossierRoles DR on Dossier.IdDossier = DR.IdDossier and DR.RoleAuthorizationType = 0
	inner join dbo.Role R on DR.IdRole = R.idRole
		
	WHERE  (
			exists (select top 1 CG.IdContainerGroup
					from dbo.ContainerGroup CG
					INNER JOIN MySecurityGroups C_MSG on CG.idGroup = C_MSG.idGroup
					where CG.IdContainer = Dossier.IdContainer and C_MSG.IdGroup is not null
							and CG.DocumentRights like '___1%')
			or exists (select top 1 RG.idRole
					   from dbo.RoleGroup RG
					   INNER JOIN Role R on RG.idRole = R.idRole
					   INNER JOIN MySecurityGroups MSG on RG.idGroup = MSG.idGroup
					   where R.idRole = DR.IdRole and MSG.IdGroup is not null
							and RG.DocumentRights like '1%')
			)
			and (@Year is null or Dossier.Year = @Year)
		    and (@Number is null or Dossier.Number = @Number)
			and (@Subject is null or Dossier.Subject like '%'+@Subject+'%')
			and (@ContainerId is null or C.idContainer = @ContainerId)
			and (@MetadataRepositoryId is null or Dossier.IdMetadataRepository= @MetadataRepositoryId)
			 AND(@MetadataValue is null or Dossier.JsonMetadata like '%'+@MetadataValue+'%')

	RETURN @CountDossiers
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
PRINT 'Modificata funzione [webapiprivate].[Fascicles_FX_IsProcedureSecretary]'
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
WHERE  RU.Type = 'SP'
       and RU.Account = @Domain + '\' + @UserName
       and exists (select top 1 SU.idUser
                                 FROM   dbo.SecurityUsers SU
                                        inner join dbo.SecurityGroups SG
                                          on SU.idGroup = SG.idGroup
                                        inner join dbo.CategoryGroup CG
                                          on SG.idGroup = CG.idGroup
                                 WHERE  CG.IdCategory = @CategoryId and SU.Description = @UserName and SU.UserDomain = @Domain)
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