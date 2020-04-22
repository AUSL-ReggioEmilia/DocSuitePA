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
PRINT 'CREATE FUNCTION SQL TemplateCollaboration_FX_CountInvalidTemplateUsers';
GO

CREATE FUNCTION [webapiprivate].[TemplateCollaboration_FX_CountInvalidTemplateUsers]
(
	@IdTemplateCollaboration uniqueidentifier
)
RETURNS INT
AS
BEGIN

	DECLARE @InvalidUsers INT
	SELECT @InvalidUsers = COUNT (*)
	FROM [dbo].[TemplateCollaborationUsers] TCU
	LEFT OUTER JOIN [dbo].[RoleUser] RU ON RU.IdRole = TCU.IdRole and RU.Account = TCU.Account
	WHERE TCU.IdTemplateCollaboration = @IdTemplateCollaboration
	AND TCU.Account IS NOT NULL AND TCU.UserType = 1 AND RU.IdRole IS NULL

	RETURN @InvalidUsers

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
PRINT 'CREATE FUNCTION SQL TemplateCollaboration_FX_CountInvalidTemplateRoles';
GO

CREATE FUNCTION [webapiprivate].[TemplateCollaboration_FX_CountInvalidTemplateRoles]
(
	@IdTemplateCollaboration uniqueidentifier
)
RETURNS INT
AS
BEGIN

	DECLARE @InvalidRoles INT
	SELECT @InvalidRoles = COUNT (*)
	FROM [dbo].[TemplateCollaborationUsers] TCU
	LEFT OUTER JOIN [dbo].[Role] RO ON RO.IdRole = TCU.IdRole
	WHERE TCU.IdTemplateCollaboration = @IdTemplateCollaboration
	AND RO.IdRole IS NOT NULL AND RO.IsActive = 0

	RETURN @InvalidRoles

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
PRINT 'CREATE FUNCTION SQL TemplateCollaboration_FX_AuthorizedTemplates';
GO

CREATE FUNCTION [webapiprivate].[TemplateCollaboration_FX_AuthorizedTemplates]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256)
)
RETURNS TABLE
AS
RETURN
(
SELECT TC.[IdTemplateCollaboration] AS UniqueId
      ,TC.[Name]
      ,TC.[Status]
      ,TC.[DocumentType]
      ,TC.[IdPriority]
      ,TC.[Object]
      ,TC.[Note]
      ,TC.[IsLocked]
      ,TC.[WSManageable]
      ,TC.[WSDeletable]
      ,TC.[RegistrationUser]
      ,TC.[RegistrationDate]
      ,TC.[LastChangedUser]
      ,TC.[LastChangedDate]
	FROM [dbo].[TemplateCollaborations] TC
	LEFT OUTER JOIN [dbo].[TemplateCollaborationRoles] TCR on TC.IdTemplateCollaboration = TCR.IdTemplateCollaboration
	LEFT OUTER JOIN [dbo].[Role] R on TCR.idRole = R.idRole 
	LEFT OUTER JOIN [dbo].[RoleGroup] RG on R.idRole = RG.idRole
	LEFT OUTER JOIN [dbo].[SecurityGroups] SG on RG.idGroup = SG.idGroup
	LEFT OUTER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
	WHERE (TCR.IdTemplateCollaboration IS NULL OR (((TC.DocumentType = 'P' AND RG.ProtocolRights like '1%')
	OR ((TC.DocumentType = 'A' OR TC.DocumentType = 'D') AND RG.ResolutionRights like '1%')
	OR ((TC.DocumentType = 'S' OR TC.DocumentType = 'UDS' OR ISNUMERIC(TC.DocumentType) = 1) AND RG.DocumentSeriesRights like '1%'))
	AND (SU.Account = @UserName AND SU.UserDomain = @Domain)))
	AND TC.Status = 1
GROUP BY TC.[IdTemplateCollaboration]
      ,TC.[Name]
      ,TC.[Status]
      ,TC.[DocumentType]
      ,TC.[IdPriority]
      ,TC.[Object]
      ,TC.[Note]
      ,TC.[IsLocked]
      ,TC.[WSManageable]
      ,TC.[WSDeletable]
      ,TC.[RegistrationUser]
      ,TC.[RegistrationDate]
      ,TC.[LastChangedUser]
      ,TC.[LastChangedDate])
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
PRINT'Modificata function [Collaboration_FX_CollaborationsSigning]'
GO

ALTER FUNCTION [webapiprivate].[Collaboration_FX_CollaborationsSigning](
	@UserName nvarchar(255),
	@IsRequired bit)
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
			right outer join dbo.CollaborationSigns CC_CS on Collaboration.IdCollaboration = CC_CS.IdCollaboration				
				and CC_CS.SignUser = @UserName
				and CC_CS.IsActive = 1
				and ((@IsRequired is null and CC_CS.IsRequired in (1,0)) or (@IsRequired is not null and CC_CS.IsRequired = @IsRequired))
		WHERE			
			Collaboration.IdStatus = 'IN'
			and Collaboration.IdCollaboration not in (SELECT CA.idCollaborationChild
												 FROM   dbo.CollaborationAggregate CA)
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
PRINT'Modificata function [Collaboration_FX_AllUserCollaborations]'
GO

ALTER FUNCTION [webapiprivate].[Collaboration_FX_AllUserCollaborations](
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
		WHERE ((CC_CS.SignUser = @UserName AND CC_CS.IsActive <> 1) AND Collaboration.IdStatus NOT IN ('PT', 'DP')) 
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
PRINT'Modificata function [Collaboration_FX_ProposedCollaborations]'
GO

ALTER FUNCTION [webapiprivate].[Collaboration_FX_ProposedCollaborations](
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
			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
			right outer join dbo.CollaborationSigns CC_CS on Collaboration.IdCollaboration = CC_CS.IdCollaboration				
		WHERE			
			Collaboration.IdStatus in ('IN', 'DL') and Collaboration.IdStatus is not null
			and Collaboration.RegistrationUser = @UserName
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


--############################################################################
PRINT 'Modificata function [Collaboration_FX_ActiveUserCollaborations]'
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
					AND Collaboration.IdStatus <> 'PT') 
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
PRINT 'Modificata function [Collaboration_FX_CheckedOutCollaborations]'
GO

ALTER FUNCTION [webapiprivate].[Collaboration_FX_CheckedOutCollaborations](
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
			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
		WHERE	C_CV.CheckedOut = 1 AND C_CV.CheckOutUser = @UserName
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

--############################################################################
 PRINT 'Modificata function [Collaboration_FX_ProtocolAdmissionCollaborations]'
GO
 
 ALTER FUNCTION [webapiprivate].[Collaboration_FX_ProtocolAdmissionCollaborations](
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
 			   
 			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,  C_CS.IsAbsent as CollaborationSign_IsAbsent,
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
 		WHERE	(Collaboration.RegistrationUser = @UserName OR CC_CS.SignUser = @UserName) AND Collaboration.IdStatus = 'DP'
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

PRINT 'Modificata function [Collaboration_FX_AlreadySignedCollaborations]'
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
 					AND Collaboration.IdStatus <> 'PT')
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
PRINT 'Modificata function [Collaboration_FX_CollaborationsManaging]'
GO
  
 ALTER FUNCTION [webapiprivate].[Collaboration_FX_CollaborationsManaging](
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
 			inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
 			inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
 			left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
 			left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
 		WHERE  Collaboration.IdStatus = 'DP'
 			   and (1 = (SELECT TOP (1) 1
 						   FROM   dbo.CollaborationUsers C_CU
 						   WHERE  C_CU.IdCollaboration = Collaboration.IdCollaboration
 								  and C_CU.Account = @UserName
 								  and C_CU.DestinationType = 'P')
 					and 1 = 1
 					 OR EXISTS (SELECT Account
 								FROM   RoleUser
 								WHERE  Type = 'S'
 									   AND Enabled = 1
 									   AND Account = @UserName
 									   AND IdRole IN (SELECT IdRole
 													  FROM   CollaborationUsers
 													  WHERE  IdCollaboration = Collaboration.IdCollaboration
 															 AND DestinationType = 'S'
 															 and DestinationFirst = 1)
 									   AND Account NOT IN (SELECT Account
 														   FROM   CollaborationUsers
 														   WHERE  IdCollaboration = Collaboration.IdCollaboration
 																  AND DestinationType = 'P'
 																  and DestinationFirst = 1)))
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
PRINT 'Modificata function [Collaboration_FX_RegisteredCollaborations]'
GO

ALTER FUNCTION [webapiprivate].[Collaboration_FX_RegisteredCollaborations](
 	@UserName nvarchar(255), 
 	@DateFrom datetimeoffset,
 	@DateTo datetimeoffset)
 	RETURNS TABLE
 AS 
 	RETURN
 	(
 		WITH CollaborationTableValue AS(
             SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
                      Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
                      Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
                      Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
                      Collaboration.LastChangedDate, Collaboration.idDocumentSeriesItem, Collaboration.TemplateName, Collaboration.IdUDS, Collaboration.IdUDSRepository
             FROM dbo.Collaboration Collaboration                                   
 			left outer join dbo.CollaborationSigns s4_ on Collaboration.IdCollaboration = s4_.IdCollaboration and (s4_.SignUser = @UserName)
             left outer join dbo.CollaborationUsers u5_ on Collaboration.IdCollaboration = u5_.IdCollaboration and (u5_.Account = @UserName)
             left outer join dbo.RoleUser RU_C1 on Collaboration.RegistrationUser = RU_C1.Account AND RU_C1.Account = @UserName AND RU_C1.[Enabled] = 1   
             left outer join dbo.CollaborationUsers RU_C2 on Collaboration.IdCollaboration = RU_C2.IdCollaboration and RU_C2.DestinationType = 'S'
             left outer join dbo.RoleUser RU_C2_S on RU_C2.IdRole = RU_C2_S.IdRole and RU_C2_S.Account = @UserName AND RU_C2_S.[Enabled] = 1 
             left outer join dbo.CollaborationSigns RU_C3 on Collaboration.IdCollaboration = RU_C3.IdCollaboration
             left outer join dbo.RoleUser RU_C3_S on RU_C3.SignUser = RU_C3_S.Account and RU_C3_S.Account = @UserName AND RU_C3_S.[Enabled] = 1                   
             WHERE Collaboration.RegistrationDate between @DateFrom and @DateTo AND
             (1 = 0 or Collaboration.RegistrationUser = @UserName
                               or s4_.IdCollaborationSign is not null
                               or u5_.IdCollaborationUser is not null
                               or RU_C1.IdRole is not null
                               or RU_C2_S.Incremental is not null
                               or RU_C3_S.Incremental is not null)       
                               and Collaboration.IdStatus = 'PT'
 			GROUP BY Collaboration.IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
                      Collaboration.MemorandumDate, Collaboration.Object, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
                      Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
                      Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
                      Collaboration.LastChangedDate, Collaboration.idDocumentSeriesItem, Collaboration.TemplateName, Collaboration.IdUDS, Collaboration.IdUDSRepository
 			)
 
             SELECT Collaboration.*, C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate, C_CS.IsAbsent as CollaborationSign_IsAbsent,
 					C_CU.IdCollaborationUser as CollaborationUser_IdCollaborationUser, C_CU.DestinationFirst as CollaborationUser_DestinationFirst, C_CU.DestinationName as CollaborationUser_DestinationName,
                     C_CV.IdCollaborationVersioning as CollaborationVersioning_IdCollaborationVersioning, C_CV.DocumentName as CollaborationVersioning_DocumentName, C_CV.CollaborationIncremental as CollaborationVersioning_CollaborationIncremental, C_CV.RegistrationUser as CollaborationVersioning_RegistrationUser, C_CV.Incremental as CollaborationVersioning_Incremental,
                     r2_.IdResolution as Resolution_IdResolution, r2_.Year as Resolution_Year, r2_.Number as Resolution_Number, r2_.ServiceNumber as Resolution_ServiceNumber, r2_.AdoptionDate as Resolution_AdoptionDate, r2_.PublishingDate as Resolution_PublishingDate,
                     dsi3_.Id as DocumentSeriesItem_IdDocumentSeriesItem, dsi3_.Number as DocumentSeriesItem_Number, dsi3_.Year as DocumentSeriesItem_Year
 			FROM CollaborationTableValue Collaboration
             inner join dbo.CollaborationSigns C_CS on Collaboration.IdCollaboration = C_CS.IdCollaboration
             inner join dbo.CollaborationUsers C_CU on Collaboration.IdCollaboration = C_CU.IdCollaboration
             inner join dbo.CollaborationVersioning C_CV on Collaboration.IdCollaboration = C_CV.IdCollaboration
             left outer join dbo.Resolution r2_ on Collaboration.IdResolution = r2_.idResolution
             left outer join dbo.DocumentSeriesItem dsi3_ on Collaboration.idDocumentSeriesItem = dsi3_.Id
             left outer join dbo.Protocol p1_ on Collaboration.Year = p1_.Year and Collaboration.Number = p1_.Number
			 left outer join cqrs.DocumentUnits du4_ on Collaboration.IdUDS = du4_.IdDocumentUnit
             WHERE (p1_.RegistrationDate between @DateFrom and @DateTo and p1_.idStatus = 0)
 						or (r2_.ProposeDate between @DateFrom and @DateTo and r2_.idStatus = 0)
                         or (dsi3_.RegistrationDate between @DateFrom and @DateTo and dsi3_.Status in (1, 3))
						 or (du4_.RegistrationDate between @DateFrom and @DateTo)   
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

PRINT 'Create function [webapiprivate].[Category_FX_ParentWithCategoryFascicle]'
GO
 
CREATE FUNCTION [webapiprivate].[Category_FX_ParentWithCategoryFascicle] (
	@IdCategory smallint,
	@Environment smallint
	)
	RETURNS TABLE
AS 
	RETURN
	(
		WITH
		MyCategories AS (
			SELECT * FROM SplitString((SELECT [FullIncrementalPath] 
										FROM [dbo].[Category] 
										WHERE [idCategory] = @IdCategory), '|')
		)
	  
	  SELECT TOP 1 [CF].[IdCategoryFascicle] as UniqueId
      ,[CF].[IdCategory]
      ,[CF].[IdFasciclePeriod]
      ,[CF].[FascicleType]
      ,[CF].[Manager]
      ,[CF].[DSWEnvironment]
      ,[CF].[RegistrationUser]
      ,[CF].[RegistrationDate]
      ,[CF].[LastChangedUser]
      ,[CF].[LastChangedDate]
      ,[CF].[Timestamp]
	    FROM [dbo].[CategoryFascicles] CF
		WHERE [idCategory] IN (select * from MyCategories)
		AND [FascicleType] IN (1,2,3) AND [DSWEnvironment] = @Environment
		ORDER BY [idCategory] DESC
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
PRINT 'Create function [webapiprivate].[TemplateCollaboration_FX_InvalidTemplatesByUserAccount]'
GO

CREATE FUNCTION [webapiprivate].[TemplateCollaboration_FX_InvalidTemplatesByUserAccount]
(
	@Account nvarchar(256),
	@idRole smallint
)
RETURNS TABLE
AS
RETURN
(
SELECT TC.[IdTemplateCollaboration] AS UniqueId
      ,TC.[Name]
      ,TC.[Status]
      ,TC.[DocumentType]
      ,TC.[IdPriority]
      ,TC.[Object]
      ,TC.[Note]
      ,TC.[IsLocked]
      ,TC.[WSManageable]
      ,TC.[WSDeletable]
      ,TC.[RegistrationUser]
      ,TC.[RegistrationDate]
      ,TC.[LastChangedUser]
      ,TC.[LastChangedDate]
	FROM [dbo].[TemplateCollaborations] TC
	INNER JOIN [dbo].[TemplateCollaborationUsers] TCU on TCU.IdTemplateCollaboration = TC.IdTemplateCollaboration
	WHERE TCU.Account = @Account AND TCU.IdRole = @idRole AND TCU.UserType = 1
GROUP BY TC.[IdTemplateCollaboration]
      ,TC.[Name]
      ,TC.[Status]
      ,TC.[DocumentType]
      ,TC.[IdPriority]
      ,TC.[Object]
      ,TC.[Note]
      ,TC.[IsLocked]
      ,TC.[WSManageable]
      ,TC.[WSDeletable]
      ,TC.[RegistrationUser]
      ,TC.[RegistrationDate]
      ,TC.[LastChangedUser]
      ,TC.[LastChangedDate])
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