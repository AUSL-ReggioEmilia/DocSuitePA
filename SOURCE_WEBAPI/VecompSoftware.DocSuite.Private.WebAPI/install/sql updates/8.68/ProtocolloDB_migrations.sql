/****************************************************************************************************************************************
*	Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)		*
*	<DBProtocollo, varchar(50), DBProtocollo>  --> Settare il nome del DB di protocollo.				        						*
*	<DBPratiche, varchar(50), DBPratiche>  --> Se esiste il DB di Pratiche settare il nome.					        					*
*	<DBAtti, varchar(50), DBAtti>			   --> Se esiste il DB di Atti settare il nome.												*
*	<ESISTE_DB_ATTI, varchar(4), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva		*
*	<ESISTE_DB_PRATICHE, varchar(4), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva  *
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
PRINT N'Creazione StoredProcedure per creazione di Contact';
GO

CREATE PROCEDURE [dbo].[Contact_Insert] 
 
	@IdContactType char(1),
	@IncrementalFather int,
	@Description nvarchar (4000),
	@BirthDate datetime,
	@Code varchar (8),
	@SearchCode varchar (255),
	@FiscalCode varchar (16),
	@IdPlaceName smallint,
	@Address varchar (60),
	@CivicNumber char (10),
	@ZipCode char (20),
	@City varchar(50),
	@CityCode char(2),
	@TelephoneNumber varchar (50),
	@FaxNumber varchar (50),
	@EMailAddress nvarchar (256),
	@CertifydMail varchar(250),
	@Note varchar(255),
	@idRole smallint,
	@isActive tinyint,
    @isLocked tinyint,
	@isNotExpandable tinyint,
	@RegistrationUser nvarchar (256),
	@RegistrationDate datetimeoffset (7),
	@LastChangedUser nvarchar (256),
	@LastChangedDate datetimeoffset (7),
	@IdTitle int,
	@IdRoleRootContact smallint,
	@ActiveFrom datetime,
	@ActiveTo datetime,
	@isChanged smallint,
	@Language int,
	@Nationality nvarchar (256),
	@BirthPlace nvarchar (256),
	@Timestamp timestamp

AS

DECLARE @LastUsedIdContact INT, @EntityId INT, @ContactId uniqueidentifier, @FullIncrementalPath varchar (255)

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
BEGIN TRY
BEGIN TRANSACTION ContactInsert

SELECT top(1) @LastUsedIdContact = Incremental FROM <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Contact] ORDER BY Incremental DESC
IF(@LastUsedIdContact is null)
BEGIN
	SET @LastUsedIdContact = 0
END

SET @EntityId = @LastUsedIdContact + 1
SET @ContactId = newid()
SET @FullIncrementalPath = @EntityId

IF(@IncrementalFather IS NOT NULL)
BEGIN
	SET @FullIncrementalPath = @IncrementalFather + '|' + @EntityId
END

INSERT INTO  <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Contact] ([Incremental],[IdContactType],[IncrementalFather],[Description],[BirthDate],[Code],[SearchCode],[FiscalCode],[IdPlaceName],[Address],[CivicNumber],[ZipCode],
                                                                       [City],[CityCode],[TelephoneNumber],[FaxNumber],[EMailAddress],[CertifydMail],[Note],[idRole],[isActive],[isLocked],[isNotExpandable],[FullIncrementalPath],
																	   [RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate],[IdTitle],[IdRoleRootContact],[ActiveFrom],[ActiveTo],[isChanged],
																	   [UniqueId],[Language],[Nationality],[BirthPlace]
																	  )
       VALUES (@EntityId, @IdContactType, @IncrementalFather, @Description, @BirthDate,@Code,@SearchCode,@FiscalCode,@IdPlaceName,@Address,@CivicNumber,@ZipCode,
	           @City,@CityCode,@TelephoneNumber,@FaxNumber,@EMailAddress,@CertifydMail,@Note,@idRole,@isActive,@isLocked,@isNotExpandable,@FullIncrementalPath,
			   @RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate, @IdTitle,@IdRoleRootContact,@ActiveFrom ,@ActiveTo ,@isChanged,
	           @ContactId,@Language, @Nationality, @BirthPlace
			)

IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN	 
	INSERT INTO <DBAtti, varchar(50), DBAtti>.[dbo].[Contact] ([Incremental],[IdContactType],[IncrementalFather],[Description],[BirthDate],[Code],[SearchCode],[FiscalCode],[IdPlaceName],[Address],[CivicNumber],[ZipCode],
                                                                       [City],[CityCode],[TelephoneNumber],[FaxNumber],[EMailAddress],[CertifydMail],[Note],[idRole],[isActive],[isLocked],[isNotExpandable],[FullIncrementalPath],
																	   [RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate],[IdTitle],[IdRoleRootContact],[ActiveFrom],[ActiveTo],[isChanged],
																	   [UniqueId],[Language],[Nationality],[BirthPlace]
																	  )
       VALUES (@EntityId, @IdContactType, @IncrementalFather, @Description, @BirthDate,@Code,@SearchCode,@FiscalCode,@IdPlaceName,@Address,@CivicNumber,@ZipCode,
	           @City,@CityCode,@TelephoneNumber,@FaxNumber,@EMailAddress,@CertifydMail,@Note,@idRole,@isActive,@isLocked,@isNotExpandable,@FullIncrementalPath,
			   @RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate, @IdTitle,@IdRoleRootContact,@ActiveFrom ,@ActiveTo ,@isChanged,
	           @ContactId,@Language, @Nationality, @BirthPlace
			)
	END
	
IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 	
	BEGIN	 
	INSERT INTO <DBPratiche, varchar(50), DBPratiche>.[dbo].[Contact] ([Incremental],[IdContactType],[IncrementalFather],[Description],[BirthDate],[Code],[SearchCode],[FiscalCode],[IdPlaceName],[Address],[CivicNumber],[ZipCode],
                                                                       [City],[CityCode],[TelephoneNumber],[FaxNumber],[EMailAddress],[CertifydMail],[Note],[idRole],[isActive],[isLocked],[isNotExpandable],[FullIncrementalPath],
																	   [RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate],[IdTitle],[IdRoleRootContact],[ActiveFrom],[ActiveTo],[isChanged],
																	   [UniqueId],[Language],[Nationality],[BirthPlace]
																	  )
       VALUES (@EntityId, @IdContactType, @IncrementalFather, @Description, @BirthDate,@Code,@SearchCode,@FiscalCode,@IdPlaceName,@Address,@CivicNumber,@ZipCode,
	           @City,@CityCode,@TelephoneNumber,@FaxNumber,@EMailAddress,@CertifydMail,@Note,@idRole,@isActive,@isLocked,@isNotExpandable,@FullIncrementalPath,
			   @RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate, @IdTitle,@IdRoleRootContact,@ActiveFrom ,@ActiveTo ,@isChanged,
	           @ContactId,@Language, @Nationality, @BirthPlace
			)
	END

COMMIT TRANSACTION ContactInsert
RETURN @EntityId
END TRY

BEGIN CATCH 
     print ERROR_MESSAGE() 
     ROLLBACK TRANSACTION ContactInsert
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
PRINT N'Creazione StoredProcedure per Aggiornamento Contatto';
GO

CREATE PROCEDURE [dbo].[Contact_Update] 
	@Incremental int, 
	@IdContactType char(1),
	@IncrementalFather int,
	@Description nvarchar (4000),
	@BirthDate datetime,
	@Code varchar (8),
	@SearchCode varchar (255),
	@FiscalCode varchar (16),
	@IdPlaceName smallint,
	@Address varchar (60),
	@CivicNumber char (10),
	@ZipCode char (20),
	@City varchar(50),
	@CityCode char(2),
	@TelephoneNumber varchar (50),
	@FaxNumber varchar (50),
	@EMailAddress nvarchar (256),
	@CertifydMail varchar(250),
	@Note varchar(255),
	@idRole smallint,
	@isActive tinyint,
    @isLocked tinyint,
	@isNotExpandable tinyint,
	@FullIncrementalPath varchar (255), 
	@RegistrationUser nvarchar (256),
	@RegistrationDate datetimeoffset (7),
	@LastChangedUser nvarchar (256),
	@LastChangedDate datetimeoffset (7),
	@IdTitle int,
	@IdRoleRootContact smallint,
	@ActiveFrom datetime,
	@ActiveTo datetime,
	@isChanged smallint,
	@Language int,
	@Nationality nvarchar (256),
	@BirthPlace nvarchar (256),
	@Timestamp timestamp
AS 

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRANSACTION StoreProcedure
	
	BEGIN TRY

	--Aggiornamento contatto
	UPDATE  <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Contact] SET 
		[IdContactType]=@IdContactType,  [IncrementalFather] = @IncrementalFather,[Description] = @Description,[BirthDate] = @BirthDate,[Code]= @Code,[SearchCode] = @SearchCode,[FiscalCode] = @FiscalCode,[IdPlaceName] = @IdPlaceName,[Address] = @Address,[CivicNumber]= @CivicNumber,[ZipCode] = @ZipCode,
        [City] = @City,[CityCode] = @CityCode,[TelephoneNumber] = @TelephoneNumber,[FaxNumber] = @FaxNumber,[EMailAddress] = @EMailAddress,[CertifydMail] = @CertifydMail,
		[Note] = @Note,[idRole] = @idRole,[isActive] = @isActive,[isLocked] = @isLocked,[isNotExpandable] = @isNotExpandable,[FullIncrementalPath] = @FullIncrementalPath,
		[LastChangedUser] = @LastChangedUser,[LastChangedDate] = @LastChangedDate,[IdTitle] = @IdTitle,[IdRoleRootContact] = @IdRoleRootContact,[ActiveFrom] = @ActiveFrom ,[ActiveTo] =  @ActiveTo,[isChanged] =  @isChanged,
		[Language] = @Language,[Nationality] = @Nationality,[BirthPlace] = @BirthPlace Where [Incremental] = @Incremental

	IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN

		
		UPDATE  <DBAtti, varchar(50), DBAtti>.[dbo].[Contact] SET 
		[IdContactType] = @IdContactType,[IncrementalFather] = @IncrementalFather, [Description] = @Description,[BirthDate] = @BirthDate,[Code]= @Code,[SearchCode] = @SearchCode,[FiscalCode] = @FiscalCode,[IdPlaceName] = @IdPlaceName,[Address] = @Address,[CivicNumber]= @CivicNumber,[ZipCode] = @ZipCode,
        [City] = @City,[CityCode] = @CityCode,[TelephoneNumber] = @TelephoneNumber,[FaxNumber] = @FaxNumber,[EMailAddress] = @EMailAddress,[CertifydMail] = @CertifydMail,
		[Note] = @Note,[idRole] = @idRole,[isActive] = @isActive,[isLocked] = @isLocked,[isNotExpandable] = @isNotExpandable,[FullIncrementalPath] = @FullIncrementalPath,
		[LastChangedUser] = @LastChangedUser,[LastChangedDate] = @LastChangedDate,[IdTitle] = @IdTitle,[IdRoleRootContact] = @IdRoleRootContact,[ActiveFrom] = @ActiveFrom ,[ActiveTo] =  @ActiveTo,[isChanged] =  @isChanged,
		[Language] = @Language,[Nationality] = @Nationality,[BirthPlace] = @BirthPlace Where [Incremental] = @Incremental 
	END

	IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
	BEGIN 
	UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Contact] SET 
		[IdContactType] = @IdContactType, [IncrementalFather] = @IncrementalFather, [Description] = @Description,[BirthDate] = @BirthDate,[Code]= @Code,[SearchCode] = @SearchCode,[FiscalCode] = @FiscalCode,[IdPlaceName] = @IdPlaceName,[Address] = @Address,[CivicNumber]= @CivicNumber,[ZipCode] = @ZipCode,
        [City] = @City,[CityCode] = @CityCode,[TelephoneNumber] = @TelephoneNumber,[FaxNumber] = @FaxNumber,[EMailAddress] = @EMailAddress,[CertifydMail] = @CertifydMail,
		[Note] = @Note,[idRole] = @idRole,[isActive] = @isActive,[isLocked] = @isLocked,[isNotExpandable] = @isNotExpandable,[FullIncrementalPath] = @FullIncrementalPath,
		[LastChangedUser] = @LastChangedUser,[LastChangedDate] = @LastChangedDate,[IdTitle] = @IdTitle,[IdRoleRootContact] = @IdRoleRootContact,[ActiveFrom] = @ActiveFrom ,[ActiveTo] =  @ActiveTo,[isChanged] =  @isChanged,
		[Language] = @Language,[Nationality] = @Nationality,[BirthPlace] = @BirthPlace Where [Incremental] = @Incremental 
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
PRINT 'ALTER FUNCTION SQL [webapiprivate].[Collaboration_FX_AllUserCollaborations]';
GO

ALTER FUNCTION [webapiprivate].[Collaboration_FX_AllUserCollaborations](
	@UserName nvarchar(256))
	RETURNS TABLE
AS 
	RETURN
	(
	 SELECT CAST(C.IdCollaboration AS int) as IdCollaboration, C.DocumentType,C.IdPriority, C.IdStatus, C.SignCount,
			   C.MemorandumDate, C.[Object] as [Subject], C.Note, C.[Year], C.Number, C.IdResolution,
			   C.PublicationUser, C.PublicationDate, C.RegistrationName, C.RegistrationEmail, C.SourceProtocolYear,
			   C.SourceProtocolNumber, C.AlertDate, C.UniqueId, C.RegistrationUser, C.RegistrationDate, C.LastChangedUser,
			   C.LastChangedDate, C.TemplateName, C.IdUDS, C.IdUDSRepository,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate, C_CS.IsAbsent as CollaborationSign_IsAbsent,
			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
			   r2_.IdResolution as Resolution_IdResolution, r2_.[Year] as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.[Year] as DocumentSeriesItem_Year

		    FROM dbo.Collaboration C
		    inner join dbo.CollaborationSigns C_CS on C.IdCollaboration = C_CS.IdCollaboration
			inner join dbo.CollaborationUsers C_CU on C.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on C.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on C.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on C.idDocumentSeriesItem = dsi3_.Id

            WHERE C.IdStatus NOT IN ('PT', 'DP', 'WM') and
            (
                 (exists (SELECT TOP 1 CU.IdCollaboration
	                      FROM CollaborationUsers CU 
	                      WHERE CU.IdCollaboration = C.IdCollaboration and CU.DestinationType = 'S' AND 
	                      exists (SELECT TOP 1 idrole
			                      FROM RoleUser RS 
			                      WHERE RS.Account = @UserName AND RS.[type] = 's' and RS.[enabled] = 1 AND RS.idRole = CU.IDRole)
					      )
				 )
                 OR 
                   (exists (SELECT TOP 1 CU.IdCollaboration
	                        FROM CollaborationUsers CU 
	                        WHERE CU.IdCollaboration = C.IdCollaboration AND CU.DestinationType = 'P' AND CU.Account = @UserName)
	               )
				OR 
                   (exists (SELECT TOP 1 CS.IdCollaboration
	                        FROM CollaborationSigns CS 
	                        WHERE  CS.IdCollaboration= C.IdCollaboration AND CS.SignUser = @UserName)
	               )
		   )
	       AND 
              (exists(SELECT TOP 1 CS.IdCollaboration
                      FROM CollaborationSigns CS
                      WHERE CS.IdCollaboration= C.IdCollaboration and CS.IsActive <> 0
					 )
              )
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
PRINT 'ALTER FUNCTION SQL [webapiprivate].[Collaboration_FX_AlreadySignedCollaborations]';
GO

ALTER FUNCTION [webapiprivate].[Collaboration_FX_AlreadySignedCollaborations](
 	@UserName nvarchar(255))
 	RETURNS TABLE
 AS 
 	RETURN
 	(
 		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
 			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
 			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
 			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
 			   Collaboration.LastChangedDate, Collaboration.TemplateName, Collaboration.IdUDS, Collaboration.IdUDSRepository,
 			   
 			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate, C_CS.IsAbsent as CollaborationSign_IsAbsent,
 			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
 			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
 			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
 			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
 		FROM   dbo.Collaboration Collaboration
 			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration
 			inner join dbo.CollaborationSigns C_CSM on Collaboration.IdCollaboration = C_CSM.IdCollaboration AND C_CSM.IsActive = 1
 			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
 			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
 			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
 			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
 		WHERE ((C_CS.SignUser = @UserName AND C_CS.IsActive <> 1 AND C_CS.Incremental < C_CSM.Incremental) 
 					AND (Collaboration.IdStatus <> 'PT' AND Collaboration.IdStatus <> 'WM'))
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
PRINT 'ALTER FUNCTION SQL [webapiprivate].[Collaboration_FX_ActiveUserCollaborations]';
GO

ALTER FUNCTION [webapiprivate].[Collaboration_FX_ActiveUserCollaborations](
	@UserName nvarchar(255),
	@Signers string_list_tbltype READONLY)
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate, Collaboration.TemplateName, Collaboration.IdUDS, Collaboration.IdUDSRepository,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate, C_CS.IsAbsent as CollaborationSign_IsAbsent,
			   C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
			   C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
			   r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
			   dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
		FROM   dbo.Collaboration Collaboration
			inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration
			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
			inner join dbo.CollaborationSigns CC_CS on Collaboration.IdCollaboration = CC_CS.IdCollaboration
		WHERE ((CC_CS.SignUser = @UserName AND CC_CS.IsActive = 1) 
					AND (Collaboration.IdStatus <> 'PT' AND Collaboration.IdStatus <> 'WM') )
		      OR (Collaboration.IdStatus = 'IN' AND CC_CS.SignUser IN (SELECT val FROM @Signers))
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
PRINT N'Creazione StoredProcedure per Eliminazione Contatto';
GO

CREATE PROCEDURE [dbo].[Contact_Delete] 
	  @Incremental int
AS 
	DECLARE @LastChangedDate datetimeoffset(7)

	SET @LastChangedDate = GETUTCDATE()

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRANSACTION StoreProcedure
	
	BEGIN TRY

	--Cancellazione logica contatto
		UPDATE <DBProtocollo, varchar(50), DBProtocollo>.[dbo].[Contact]  SET [LastChangedDate] = @LastChangedDate,  [isActive] = 0 Where [Incremental] = @Incremental
	
	IF( (CAST('<ESISTE_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
	BEGIN
		UPDATE <DBAtti, varchar(50), DBAtti>.[dbo].[Contact]  SET [LastChangedDate] = @LastChangedDate,  [isActive] = 0 Where [Incremental] = @Incremental
	END

	IF( (CAST('<ESISTE_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
	BEGIN 
		UPDATE <DBPratiche, varchar(50), DBPratiche>.[dbo].[Contact]  SET [LastChangedDate] = @LastChangedDate,  [isActive] = 0 Where [Incremental] = @Incremental
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
PRINT N'Creazione funzione [webapiprivate].[UDSRepository_FX_ViewableRepositoriesByTypology]';
GO

CREATE FUNCTION [webapiprivate].[UDSRepository_FX_ViewableRepositoriesByTypology](
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
	INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
	WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
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

PRINT N'Creazione funzione [webapiprivate].[UDSRepository_FX_InsertableRepositoriesByTypology]';
GO

CREATE FUNCTION [webapiprivate].[UDSRepository_FX_InsertableRepositoriesByTypology](
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
	INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
	WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
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