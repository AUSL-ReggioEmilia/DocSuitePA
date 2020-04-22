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

PRINT N'Creazione function [AtVisionSignCollaborations]';
GO

CREATE FUNCTION [dbo].[AtVisionSignCollaborations](
	@UserName nvarchar(255))
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate,
			   
			   C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
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
INSERT INTO #tmpErrors (Error) VALUES (1);
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