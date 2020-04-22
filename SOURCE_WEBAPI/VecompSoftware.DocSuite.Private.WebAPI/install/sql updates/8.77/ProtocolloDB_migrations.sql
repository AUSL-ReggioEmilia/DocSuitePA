/****************************************************************************************************************************************
* Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)  *
* <DBProtocollo, varcahr(255), DSProtocollo>  --> Settare il nome del DB di protocollo.                  *
* <DBPratiche, varcahr(255), DSPratiche>  --> Se esiste il DB di Pratiche settare il nome.              *
* <DBAtti, varcahr(255), DSAtti>      --> Se esiste il DB di Atti settare il nome.            *
* <EXIST_DB_ATTI, varchar(255), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva  *
* <EXIST_DB_PRATICHE, varchar(255), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva*
* <EXIST_DB_PROTOCOLLO, varchar(255), True> --> Impostare False non presente, Impostare True se presente e settare la chiave successiva*
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
PRINT N'MODIFICATA SQL-FUNCTION [webapiprivate].[Fascicles_FX_AvailableFascicles]';
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
			SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
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
	    AND ((@Name is NOT null AND ( F.Title like '%'+@Name+'%' OR F.Object like '%'+@Name+'%')) OR (@Name Is null))
		AND((F.FascicleType = 1 AND
				EXISTS (SELECT TOP 1 CG.IdCategory FROM [dbo].[CategoryGroup] CG
				WHERE CG.ProtocolRights LIKE '____1%' AND CG.idcategory = F_C.idcategory
					  AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup)		
			))
			OR (F.FascicleType = 4 AND FR.RoleAuthorizationType = 0 AND
				EXISTS (SELECT TOP 1 R.idRole from [dbo].[Role] R
				INNER JOIN [dbo].[RoleGroup] RG ON R.idRole = RG.idRole
				WHERE FR.idrole = R.idRole AND R.IsActive = 1 
				      AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup)
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
PRINT N'Modifica SQL-FUNCTION [webapiprivate].[Protocol_FX_AuthorizedProtocols]';
GO

ALTER FUNCTION [webapiprivate].[Protocol_FX_AuthorizedProtocols]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@DateFrom datetimeoffset,
	@DateTo datetimeoffset
)
RETURNS TABLE
AS
RETURN
(
WITH ProtocolTableValued AS(
SELECT P.Year As Year
      ,P.Number As Number
	  ,P.IdCategoryAPI
	  ,P.idContainer
      ,P.RegistrationDate As RegistrationDate
      ,P.DocumentCode As DocumentCode
      ,P.idStatus As IdStatus
      ,P.[Object]
	  ,P.idType As IdType
	  ,P.idDocument As IdDocument	 
	  ,P.UniqueId AS UniqueId 
	FROM [dbo].[Protocol] P
	INNER JOIN [dbo].[ProtocolRole] PR on P.Year = PR.Year AND P.Number = PR.Number
	INNER JOIN [dbo].[Role] R on PR.idRole = R.idRole 
	INNER JOIN [dbo].[RoleGroup] RG on R.idRole = RG.idRole
	WHERE 
	P.RegistrationDate between @DateFrom AND @DateTo 
	AND (RG.ProtocolRights like '1%') 
	AND EXISTS (SELECT 1 FROM [dbo].[UserSecurityGroups](@UserName,@Domain) SG WHERE SG.IdGroup = RG.IdGroup)
GROUP BY P.Year, P.Number, P.IdCategoryAPI, P.idContainer, P.RegistrationDate, P.DocumentCode, P.idStatus, P.[Object], P.idType, P.idDocument, P.UniqueId)

SELECT P.*, T.[idType] As ProtocolType_IdType
	  ,T.[Description] As ProtocolType_Description
	  ,CT.idCategory as Category_IdCategory
	  ,CT.Name as Category_Name
	  ,C.idContainer as Container_IdContainer
	  ,C.Name as Container_Name
	  ,PC.[IDContact] As ProtocolContact_IDContact
	  ,CN.[Description] As ProtocolContact_Description
	  ,PCM.[Incremental] As ProtocolContactManual_Incremental
	  ,PCM.[Description] As ProtocolContactManual_Description
FROM ProtocolTableValued P
		INNER JOIN [dbo].[Type] T on P.idType = T.idType
		INNER JOIN [dbo].[Category] CT on P.IdCategoryAPI = CT.idCategory
		LEFT OUTER JOIN [dbo].[ProtocolContact] PC on P.Year = PC.Year AND P.Number = PC.Number
		LEFT OUTER JOIN [dbo].[Contact] CN on PC.IDContact = CN.Incremental
		LEFT OUTER JOIN [dbo].[ProtocolContactManual] PCM on P.Year = PCM.Year AND P.Number = PCM.Number	
		INNER JOIN [dbo].[Container] C on P.idContainer = C.idContainer);
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
PRINT N'Modifica SQL-FUNCTION [webapiprivate].[Collaboration_FX_ActiveUserCollaborations]';
GO

ALTER FUNCTION [webapiprivate].[Collaboration_FX_ActiveUserCollaborations]
(
	@UserName nvarchar(255),
	@Signers string_list_tbltype READONLY
)
RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
			   Collaboration.LastChangedDate, Collaboration.TemplateName,
			   
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
PRINT N'Modifica SQL-FUNCTION [webapiprivate].[Collaboration_FX_AllUserCollaborations]';
GO

ALTER FUNCTION [webapiprivate].[Collaboration_FX_AllUserCollaborations]
(
	@UserName nvarchar(256)
)
RETURNS TABLE
AS 
	RETURN
	(
	 SELECT CAST(C.IdCollaboration AS int) as IdCollaboration, C.DocumentType,C.IdPriority, C.IdStatus, C.SignCount,
			   C.MemorandumDate, C.[Object] as [Subject], C.Note, C.[Year], C.Number, C.IdResolution,
			   C.PublicationUser, C.PublicationDate, C.RegistrationName, C.RegistrationEmail, C.SourceProtocolYear,
			   C.SourceProtocolNumber, C.AlertDate, C.UniqueId, C.RegistrationUser, C.RegistrationDate, C.LastChangedUser,
			   C.LastChangedDate, C.TemplateName,
			   
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
PRINT N'Modifica SQL-FUNCTION [webapiprivate].[Collaboration_FX_AlreadySignedCollaborations]';
GO

ALTER FUNCTION [webapiprivate].[Collaboration_FX_AlreadySignedCollaborations]
(
 	@UserName nvarchar(255)
)
RETURNS TABLE
AS 
RETURN
 	(
 		SELECT CAST(Collaboration.IdCollaboration AS int) as IdCollaboration, Collaboration.DocumentType, Collaboration.IdPriority, Collaboration.IdStatus, Collaboration.SignCount,
 			   Collaboration.MemorandumDate, Collaboration.Object as Subject, Collaboration.Note, Collaboration.Year, Collaboration.Number, Collaboration.IdResolution,
 			   Collaboration.PublicationUser, Collaboration.PublicationDate, Collaboration.RegistrationName, Collaboration.RegistrationEmail, Collaboration.SourceProtocolYear,
 			   Collaboration.SourceProtocolNumber, Collaboration.AlertDate, Collaboration.UniqueId, Collaboration.RegistrationUser, Collaboration.RegistrationDate, Collaboration.LastChangedUser,
 			   Collaboration.LastChangedDate, Collaboration.TemplateName,
 			   
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
PRINT N'Modifica SQL-FUNCTION [webapiprivate].[Collaboration_FX_CheckedOutCollaborations]';
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
			   Collaboration.LastChangedDate, Collaboration.TemplateName,
			   
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
PRINT N'Modifica SQL-FUNCTION [webapiprivate].[Collaboration_FX_CollaborationsManaging]';
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
 			   Collaboration.LastChangedDate, Collaboration.TemplateName,
 			   
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
PRINT N'Modifica SQL-FUNCTION [webapiprivate].[Collaboration_FX_CollaborationsSigning]';
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
			   Collaboration.LastChangedDate, Collaboration.TemplateName,
			   
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
PRINT N'Modifica SQL-FUNCTION [webapiprivate].[Collaboration_FX_ProposedCollaborations]';
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
			   Collaboration.LastChangedDate, Collaboration.TemplateName,
			   
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
PRINT N'Modifica SQL-FUNCTION [webapiprivate].[Collaboration_FX_ProtocolAdmissionCollaborations]';
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
 			   Collaboration.LastChangedDate, Collaboration.TemplateName,
 			   
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
PRINT N'Modifica SQL-FUNCTION [webapiprivate].[Collaboration_FX_RegisteredCollaborations]';
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
                      Collaboration.LastChangedDate, Collaboration.idDocumentSeriesItem, Collaboration.TemplateName
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
                      Collaboration.LastChangedDate, Collaboration.idDocumentSeriesItem, Collaboration.TemplateName
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
			 LEFT OUTER JOIN uds.UDSCollaborations udsc_ ON Collaboration.IdCollaboration = udsc_.IdCollaboration
			 left outer join cqrs.DocumentUnits du4_ on udsc_.IdUDS = du4_.IdDocumentUnit
             WHERE (p1_.RegistrationDate between @DateFrom and @DateTo and p1_.idStatus = 0)
 						or (r2_.ProposeDate between @DateFrom and @DateTo and r2_.idStatus = 0)
                         or (dsi3_.RegistrationDate between @DateFrom and @DateTo and dsi3_.Status in (1, 3))
						 or (du4_.RegistrationDate between @DateFrom and @DateTo)   
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
PRINT N'ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_CountAuthorizedDocumentUnitsByFascicle]';
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_CountAuthorizedDocumentUnitsByFascicle](
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@FascicleId uniqueidentifier,
	@Year smallint,
	@Number nvarchar(256),
	@DocumentUnitName nvarchar(256),
	@CategoryId smallint,
	@ContainerId smallint,
	@Subject nvarchar(256),
	@IncludeChildClassification bit
)
RETURNS INT
AS
	BEGIN
	DECLARE @CountDocumentUnits INT;
WITH 	
	MySecurityGroups AS (
		SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
	),
	CategoryChildren AS (
		SELECT CC.IdCategory
		FROM [dbo].Category CC
		WHERE (@IncludeChildClassification = 0 AND CC.IdCategory = @CategoryId ) OR ( @IncludeChildClassification = 1 AND (CC.FullIncrementalPath like CONVERT(varchar(10), @CategoryId) +'|%' OR CC.IdCategory = @CategoryId))
				GROUP BY CC.IdCategory
	)
	
	SELECT @CountDocumentUnits = COUNT(DISTINCT DU.IdDocumentUnit)

	FROM cqrs.DocumentUnits DU
			 	INNER JOIN [dbo].[Container] C ON DU.IdContainer = C.IdContainer
				INNER JOIN [dbo].[Category] CT ON DU.IdCategory = CT.IdCategory
				LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR ON DUR.IdDocumentUnit = DU.IdDocumentUnit
		
	WHERE      (@Year IS NULL OR DU.Year = @Year) 
			 	AND (@Number IS NULL OR DU.Title like  '____/%' +  REPLACE(@Number, '|', '/'))
				AND (@DocumentUnitName IS NULL OR DU.DocumentUnitName = @DocumentUnitName OR (@DocumentUnitName = 'Archivio' AND Environment > 99) OR (@DocumentUnitName = 'Serie documentale' AND Environment = 4))
				AND (@CategoryId IS NULL OR EXISTS ( SELECT TOP 1 CC.IdCategory FROM CategoryChildren CC WHERE DU.IdCategory = CC.IdCategory))
				AND (@ContainerId IS NULL OR DU.IdContainer = @ContainerId)
				AND (@Subject IS NULL OR DU.Subject like '%'+@Subject+'%')
				AND ((EXISTS (SELECT TOP 1 IdFascicle FROM [dbo].[Fascicles] WHERE IdFascicle = @FascicleId AND FascicleType in (4, 1))
				  AND (
				     EXISTS (SELECT top 1 CG.idContainerGroup
					 FROM [dbo].[ContainerGroup] CG 
					 WHERE CG.IdContainer = DU.IdContainer 
					 AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup)
					 AND (
					 (DU.Environment = 1 AND (CG.Rights like '__1%'))
					 OR (DU.Environment = 2 AND (CG.ResolutionRights like '__1%'))
					 OR (DU.Environment = 3 AND (CG.DocumentSeriesRights like '__1%'))
					 OR ((DU.Environment = 7 OR DU.Environment > 99) AND (CG.UDSRights like '__1%'))
					)) 
					 OR exists (SELECT top 1 RG.idRole
						FROM [dbo].[RoleGroup] RG
						INNER JOIN Role R on RG.idRole = R.idRole
						WHERE  R.UniqueId = DUR.UniqueIdRole
							   AND ((DU.Environment = 1 AND (RG.ProtocolRights like '1%'))
							   OR (DU.Environment = 2 AND (RG.ResolutionRights like '1%'))
						       OR (DU.Environment = 3 AND (RG.DocumentSeriesRights like '1%'))
						       OR ((DU.Environment = 7 OR DU.Environment > 99) AND (RG.DocumentSeriesRights like '1%'))) 
						       AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup))
				 )
		    )
			OR (NOT EXISTS (SELECT TOP 1 IdFascicle FROM [dbo].[Fascicles] WHERE IdFascicle = @FascicleId AND FascicleType in (4, 1)) 
			AND NOT EXISTS (SELECT CF.IdCategoryFascicle FROM [dbo].[CategoryFascicles] CF
						  INNER JOIN [dbo].[Fascicles] F ON F.IdCategory = CF.IdCategory
						  WHERE CF.FascicleType != 1 AND F.IdFascicle = @FascicleId AND CF.DSWEnvironment = DU.Environment)))
			AND (DU.IdFascicle IS NULL OR DU.IdFascicle != @FascicleId)

	RETURN @CountDocumentUnits
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
PRINT N'ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_AuthorizedDocumentUnitsByFascicle] ';
GO

ALTER FUNCTION [webapiprivate].[DocumentUnit_FX_AuthorizedDocumentUnitsByFascicle] 
(	
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@FascicleId uniqueidentifier,
	@Year smallint,
	@Number nvarchar(256),
	@DocumentUnitName nvarchar(256),
	@CategoryId smallint,
	@ContainerId smallint,
	@Subject nvarchar(256),
	@IncludeChildClassification bit,
	@Skip int,
	@Top int
)
RETURNS TABLE 
AS
RETURN 
(
	WITH 	
	MySecurityGroups AS (
		 SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
	), 	
	MyCategory AS (
		SELECT TOP 1 C.IdCategory
		FROM [dbo].[Category] C 
		INNER JOIN [dbo].[Fascicles] F ON F.IdCategory = C.IdCategory
		WHERE F.IdFascicle = @FascicleId
		GROUP BY C.IdCategory
	), 	
	MyCategoryFascicles AS (
		SELECT CF.IdCategory
		FROM [dbo].[CategoryFascicles] CF 
		INNER JOIN [dbo].[Category] C ON C.idCategory = CF.IdCategory
		WHERE (EXISTS (SELECT MyCategory.IdCategory FROM MyCategory WHERE CF.IdCategory = MyCategory.IdCategory and CF.FascicleType = 1))
			  OR (EXISTS (SELECT MyCategory.IdCategory FROM MyCategory WHERE MyCategory.IdCategory in (SELECT Value FROM [dbo].[SplitString](C.FullIncrementalPath, '|')) and CF.FascicleType = 0))
		GROUP BY CF.IdCategory
	),
	CategoryChildren AS (
		SELECT CC.IdCategory
		FROM [dbo].Category CC
		WHERE (@IncludeChildClassification = 0 AND CC.IdCategory = @CategoryId ) OR ( @IncludeChildClassification = 1 AND (CC.FullIncrementalPath like CONVERT(varchar(10), @CategoryId) +'|%' OR CC.IdCategory = @CategoryId))
				GROUP BY CC.IdCategory
	),
	
	MydocumentUnits AS (
			SELECT T.IdDocumentUnit, T.rownum FROM
			(SELECT DU.[IdDocumentUnit], row_number() over(order by DU.[IdDocumentUnit]) as rownum 
			 FROM cqrs.DocumentUnits DU
			 	INNER JOIN [dbo].[Container] C ON DU.IdContainer = C.IdContainer
				INNER JOIN [dbo].[Category] CT ON DU.IdCategory = CT.IdCategory
				LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR ON DUR.IdDocumentUnit = DU.IdDocumentUnit
			 WHERE (@Year IS NULL OR DU.Year = @Year) 
			 	AND (@Number IS NULL OR DU.Title like  '____/%' + REPLACE(@Number, '|', '/'))
				AND (@DocumentUnitName IS NULL OR DU.DocumentUnitName = @DocumentUnitName OR (@DocumentUnitName = 'Archivio' AND Environment >99) OR (@DocumentUnitName = 'Serie documentale' AND Environment = 4))
				AND (@CategoryId IS NULL OR EXISTS (SELECT TOP 1 CC.IdCategory FROM CategoryChildren CC WHERE DU.IdCategory = CC.IdCategory))
				AND (@ContainerId IS NULL OR DU.IdContainer = @ContainerId)
				AND (@Subject IS NULL OR DU.Subject like '%'+@Subject+'%')
				AND ((EXISTS (SELECT TOP 1 IdFascicle FROM [dbo].[Fascicles] WHERE IdFascicle = @FascicleId AND FascicleType in (4, 1))
				  AND (
				     EXISTS (SELECT top 1 CG.idContainerGroup
					 FROM [dbo].[ContainerGroup] CG 
					 WHERE CG.IdContainer = DU.IdContainer 
					 AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup)
					 AND (
					 (DU.Environment = 1 AND (CG.Rights like '__1%'))
					 OR (DU.Environment = 2 AND (CG.ResolutionRights like '__1%'))
					 OR (DU.Environment = 3 AND (CG.DocumentSeriesRights like '__1%'))
					 OR ((DU.Environment = 7 OR DU.Environment > 99) AND (CG.UDSRights like '__1%'))
					)) 
					 OR EXISTS (SELECT top 1 RG.idRole
						FROM [dbo].[RoleGroup] RG
						INNER JOIN Role R ON RG.idRole = R.idRole
						WHERE  R.UniqueId = DUR.UniqueIdRole
							   AND ((DU.Environment = 1 AND (RG.ProtocolRights like '1%'))
							   OR (DU.Environment = 2 AND (RG.ResolutionRights like '1%'))
						       OR (DU.Environment = 3 AND (RG.DocumentSeriesRights like '1%'))
						       OR ((DU.Environment = 7 OR DU.Environment > 99) AND (RG.DocumentSeriesRights like '1%'))) 
						       AND EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = RG.IdGroup))
				 )
		    )
            OR (NOT EXISTS (SELECT TOP 1 IdFascicle FROM [dbo].[Fascicles] WHERE IdFascicle = @FascicleId AND FascicleType in (4, 1)) 
			AND NOT EXISTS (SELECT CF.IdCategoryFascicle FROM [dbo].[CategoryFascicles] CF
						  INNER JOIN [dbo].[Fascicles] F ON F.IdCategory = CF.IdCategory
						  WHERE CF.FascicleType != 1 AND F.IdFascicle = @FascicleId AND CF.DSWEnvironment = DU.Environment)))
			AND (DU.IdFascicle IS NULL OR DU.IdFascicle != @FascicleId)
			Group by DU.[IdDocumentUnit]) T WHERE T.rownum > @Skip AND T.rownum <= @Top
		)

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
		  ,DU.[IdUDSRepository]
		  ,CT.idCategory AS Category_IdCategory
		  ,CT.Name AS Category_Name
		  ,C.idContainer AS Container_IdContainer
		  ,C.Name AS Container_Name
		  ,(SELECT CAST(COUNT(1) AS BIT) from MyCategoryFascicles where MyCategoryFascicles.IdCategory = CT.IdCategory) as IsFascicolable
		  from cqrs.DocumentUnits DU
	INNER JOIN [dbo].[Container] C ON DU.IdContainer = C.IdContainer
	INNER JOIN [dbo].[Category] CT ON DU.IdCategory = CT.IdCategory
WHERE EXISTS (SELECT MydocumentUnits.[IdDocumentUnit] from MydocumentUnits where DU.[IdDocumentUnit] = MydocumentUnits.IdDocumentUnit)
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
PRINT N'Modifica StoredProcedure [Contact_Insert]';
GO

ALTER PROCEDURE [dbo].[Contact_Insert] 
    @UniqueId uniqueidentifier,
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
	@FullIncrementalPath varchar (255),
	@SDIIdentification nvarchar (256)
	
AS

	DECLARE @isAttiEnable BIT
	DECLARE @isPraticheEnable BIT
	DECLARE @isProtocolloEnable BIT
	SET @isAttiEnable = CAST('<EXIST_DB_ATTI, varchar(255), True>' AS BIT)
	SET @isPraticheEnable = CAST('<EXIST_DB_PRATICHE, varchar(255), True>' AS BIT)
	SET @isProtocolloEnable = CAST('<EXIST_DB_PROTOCOLLO, varchar(255), True>' AS BIT)

	DECLARE @LastUsedIdContact INT, @EntityId INT, @FullIncrementalFatherPath varchar(255)

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
	BEGIN TRY
	BEGIN TRANSACTION ContactInsert

	SELECT top(1) @LastUsedIdContact = Incremental FROM <DBProtocollo, varchar(255), Nome db Protocollo o lasciare vuoto>.[dbo].[Contact] ORDER BY Incremental DESC
	IF(@LastUsedIdContact is null)
	BEGIN
		SET @LastUsedIdContact = 0
	END

	SET @EntityId = @LastUsedIdContact + 1
	SET @FullIncrementalPath = @EntityId

	SET @FullIncrementalFatherPath = (SELECT FullIncrementalPath FROM dbo.Contact WHERE [Incremental] = @IncrementalFather)
	IF(@FullIncrementalFatherPath IS NOT NULL)
	BEGIN
		SET @FullIncrementalPath = @FullIncrementalFatherPath + '|' + CAST(@EntityId AS VARCHAR(50))
	END

	INSERT INTO  <DBProtocollo, varchar(255), Nome db Protocollo o lasciare vuoto>.[dbo].[Contact] ([Incremental],[IdContactType],[IncrementalFather],[Description],[BirthDate],[Code],[SearchCode],[FiscalCode],[IdPlaceName],[Address],[CivicNumber],[ZipCode],
         [City],[CityCode],[TelephoneNumber],[FaxNumber],[EMailAddress],[CertifydMail],[Note],[idRole],[isActive],[isLocked],[isNotExpandable],[FullIncrementalPath],
		 [RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate],[IdTitle],[IdRoleRootContact],[ActiveFrom],[ActiveTo],[isChanged],
		 [UniqueId],[Language],[Nationality],[BirthPlace], [SDIIdentification])
       VALUES (@EntityId, @IdContactType, @IncrementalFather, @Description, @BirthDate,@Code,@SearchCode,@FiscalCode,@IdPlaceName,@Address,@CivicNumber,@ZipCode,
	           @City,@CityCode,@TelephoneNumber,@FaxNumber,@EMailAddress,@CertifydMail,@Note,@idRole,@isActive,@isLocked,@isNotExpandable,@FullIncrementalPath,
			   @RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate, @IdTitle,@IdRoleRootContact,@ActiveFrom ,@ActiveTo ,@isChanged,
	           @UniqueId,@Language, @Nationality, @BirthPlace, @SDIIdentification
			)

	IF( (CAST('<EXIST_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		BEGIN	 
		INSERT INTO <DBAtti, varchar(255), Nome db Atti o lasciare vuoto>.[dbo].[Contact] ([Incremental],[IdContactType],[IncrementalFather],[Description],[BirthDate],[Code],[SearchCode],[FiscalCode],[IdPlaceName],[Address],[CivicNumber],[ZipCode],
			[City],[CityCode],[TelephoneNumber],[FaxNumber],[EMailAddress],[CertifydMail],[Note],[idRole],[isActive],[isLocked],[isNotExpandable],[FullIncrementalPath],
			[RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate],[IdTitle],[IdRoleRootContact],[ActiveFrom],[ActiveTo],[isChanged],
			[UniqueId],[Language],[Nationality],[BirthPlace], [SDIIdentification])
		   VALUES (@EntityId, @IdContactType, @IncrementalFather, @Description, @BirthDate,@Code,@SearchCode,@FiscalCode,@IdPlaceName,@Address,@CivicNumber,@ZipCode,
				   @City,@CityCode,@TelephoneNumber,@FaxNumber,@EMailAddress,@CertifydMail,@Note,@idRole,@isActive,@isLocked,@isNotExpandable,@FullIncrementalPath,
				   @RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate, @IdTitle,@IdRoleRootContact,@ActiveFrom ,@ActiveTo ,@isChanged,
				   @UniqueId,@Language, @Nationality, @BirthPlace, @SDIIdentification)
		END
	
	IF( (CAST('<EXIST_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 		
		BEGIN	 
		INSERT INTO <DBPratiche, varchar(255), Nome db Pratiche o lasciare vuoto>.[dbo].[Contact] ([Incremental],[IdContactType],[IncrementalFather],[Description],[BirthDate],[Code],[SearchCode],[FiscalCode],[IdPlaceName],[Address],[CivicNumber],[ZipCode],
			[City],[CityCode],[TelephoneNumber],[FaxNumber],[EMailAddress],[CertifydMail],[Note],[idRole],[isActive],[isLocked],[isNotExpandable],[FullIncrementalPath],
			[RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate],[IdTitle],[IdRoleRootContact],[ActiveFrom],[ActiveTo],[isChanged],
			[UniqueId],[Language],[Nationality],[BirthPlace], [SDIIdentification])
		   VALUES (@EntityId, @IdContactType, @IncrementalFather, @Description, @BirthDate,@Code,@SearchCode,@FiscalCode,@IdPlaceName,@Address,@CivicNumber,@ZipCode,
				   @City,@CityCode,@TelephoneNumber,@FaxNumber,@EMailAddress,@CertifydMail,@Note,@idRole,@isActive,@isLocked,@isNotExpandable,@FullIncrementalPath,
				   @RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate, @IdTitle,@IdRoleRootContact,@ActiveFrom ,@ActiveTo ,@isChanged,
				   @UniqueId,@Language, @Nationality, @BirthPlace, @SDIIdentification)
		END

	COMMIT TRANSACTION ContactInsert

	SELECT [Incremental],[UniqueId],[IdContactType],[IdTitle],[IdPlaceName],[idRole],[IdRoleRootContact],[IncrementalFather],[Description],[BirthDate],[Code],[SearchCode],[FiscalCode],[Address],[CivicNumber]
      ,[ZipCode],[City],[CityCode],[TelephoneNumber],[FaxNumber],[EMailAddress],[CertifydMail],[Note],[isActive],[isLocked],[isNotExpandable],[FullIncrementalPath],[ActiveFrom],[ActiveTo],[isChanged]
      ,[RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate],[Language],[Nationality],[BirthPlace],[Timestamp],[SDIIdentification]
    FROM <DBProtocollo, varchar(255), Nome db Protocollo o lasciare vuoto>.[dbo].[Contact] 
	WHERE [UniqueId] = @UniqueId
																	   
	END TRY

	BEGIN CATCH 
		 ROLLBACK TRANSACTION ContactInsert

		declare @ErrorNumber as int = ERROR_NUMBER()
		declare @ErrorSeverity as int = ERROR_SEVERITY()
		declare @ErrorMessage as nvarchar(4000)
		declare @ErrorState as int = ERROR_STATE()
		declare @ErrorLine as int = ERROR_LINE()
		declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		RAISERROR 
			(
			@ErrorSeverity, 
			1,               
			@ErrorNumber,    -- parameter: original error number.
			@ErrorSeverity,  -- parameter: original error severity.
			@ErrorState,     -- parameter: original error state.
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
PRINT N'Modifica StoredProcedure [Contact_Update]';

GO

ALTER PROCEDURE [dbo].[Contact_Update]
		@UniqueId uniqueidentifier,
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
		@Timestamp_Original timestamp,
		@SDIIdentification nvarchar (256)
	AS 

		DECLARE @isAttiEnable BIT
		DECLARE @isPraticheEnable BIT
		DECLARE @isProtocolloEnable BIT
		SET @isAttiEnable = CAST('<EXIST_DB_ATTI, varchar(255), True>' AS BIT)
		SET @isPraticheEnable = CAST('<EXIST_DB_PRATICHE, varchar(255), True>' AS BIT)
		SET @isProtocolloEnable = CAST('<EXIST_DB_PROTOCOLLO, varchar(255), True>' AS BIT)

		SET TRANSACTION ISOLATION LEVEL SERIALIZABLE  
		BEGIN TRANSACTION ContactUpdate
		
	BEGIN TRY
		
		--Aggiornamento contatto
		UPDATE  <DBProtocollo, varchar(255), Nome db Protocolo o lasciare vuoto>.[dbo].[Contact] SET 
			[IdContactType]=@IdContactType,  [IncrementalFather] = @IncrementalFather,[Description] = @Description,[BirthDate] = @BirthDate,[Code]= @Code,[SearchCode] = @SearchCode,[FiscalCode] = @FiscalCode,[IdPlaceName] = @IdPlaceName,[Address] = @Address,[CivicNumber]= @CivicNumber,[ZipCode] = @ZipCode,
			[City] = @City,[CityCode] = @CityCode,[TelephoneNumber] = @TelephoneNumber,[FaxNumber] = @FaxNumber,[EMailAddress] = @EMailAddress,[CertifydMail] = @CertifydMail,
			[Note] = @Note,[idRole] = @idRole,[isActive] = @isActive,[isLocked] = @isLocked,[isNotExpandable] = @isNotExpandable,[FullIncrementalPath] = @FullIncrementalPath,
			[LastChangedUser] = @LastChangedUser,[LastChangedDate] = @LastChangedDate,[IdTitle] = @IdTitle,[IdRoleRootContact] = @IdRoleRootContact,[ActiveFrom] = @ActiveFrom ,[ActiveTo] =  @ActiveTo,[isChanged] =  @isChanged,
			[Language] = @Language,[Nationality] = @Nationality,[BirthPlace] = @BirthPlace, [SDIIdentification] = @SDIIdentification 
			WHERE [Incremental] = @Incremental
			
		
		IF( (CAST('<EXIST_DB_ATTI, varchar(4), True>'  AS BIT) = CAST('True' AS BIT))) 
		BEGIN
			
			UPDATE  <DBAtti, varchar(255), Nome db ATTI o lasciare vuoto>.[dbo].[Contact] SET 
			[IdContactType] = @IdContactType,[IncrementalFather] = @IncrementalFather, [Description] = @Description,[BirthDate] = @BirthDate,[Code]= @Code,[SearchCode] = @SearchCode,[FiscalCode] = @FiscalCode,[IdPlaceName] = @IdPlaceName,[Address] = @Address,[CivicNumber]= @CivicNumber,[ZipCode] = @ZipCode,
			[City] = @City,[CityCode] = @CityCode,[TelephoneNumber] = @TelephoneNumber,[FaxNumber] = @FaxNumber,[EMailAddress] = @EMailAddress,[CertifydMail] = @CertifydMail,
			[Note] = @Note,[idRole] = @idRole,[isActive] = @isActive,[isLocked] = @isLocked,[isNotExpandable] = @isNotExpandable,[FullIncrementalPath] = @FullIncrementalPath,
			[LastChangedUser] = @LastChangedUser,[LastChangedDate] = @LastChangedDate,[IdTitle] = @IdTitle,[IdRoleRootContact] = @IdRoleRootContact,[ActiveFrom] = @ActiveFrom ,[ActiveTo] =  @ActiveTo,[isChanged] =  @isChanged,
			[Language] = @Language,[Nationality] = @Nationality,[BirthPlace] = @BirthPlace, [SDIIdentification] = @SDIIdentification 
			WHERE [Incremental] = @Incremental 

		END

		IF( (CAST('<EXIST_DB_PRATICHE, varchar(4), True>'  AS BIT) = CAST('True' AS BIT)))
			BEGIN 
				UPDATE <DBPratiche, varchar(255), Nome db Pratiche o lasciare vuoto>.[dbo].[Contact] SET 
					[IdContactType] = @IdContactType, [IncrementalFather] = @IncrementalFather, [Description] = @Description,[BirthDate] = @BirthDate,[Code]= @Code,[SearchCode] = @SearchCode,[FiscalCode] = @FiscalCode,[IdPlaceName] = @IdPlaceName,[Address] = @Address,[CivicNumber]= @CivicNumber,[ZipCode] = @ZipCode,
					[City] = @City,[CityCode] = @CityCode,[TelephoneNumber] = @TelephoneNumber,[FaxNumber] = @FaxNumber,[EMailAddress] = @EMailAddress,[CertifydMail] = @CertifydMail,
					[Note] = @Note,[idRole] = @idRole,[isActive] = @isActive,[isLocked] = @isLocked,[isNotExpandable] = @isNotExpandable,[FullIncrementalPath] = @FullIncrementalPath,
					[LastChangedUser] = @LastChangedUser,[LastChangedDate] = @LastChangedDate,[IdTitle] = @IdTitle,[IdRoleRootContact] = @IdRoleRootContact,[ActiveFrom] = @ActiveFrom ,[ActiveTo] =  @ActiveTo,[isChanged] =  @isChanged,
					[Language] = @Language,[Nationality] = @Nationality,[BirthPlace] = @BirthPlace, [SDIIdentification] = @SDIIdentification 
				WHERE [Incremental] = @Incremental 

			END

		COMMIT TRANSACTION ContactUpdate
	
		SELECT [Incremental] as EntityId,[IdContactType],[IncrementalFather],[Description],[BirthDate],[Code],[SearchCode],[FiscalCode],[IdPlaceName],[Address],[CivicNumber],[ZipCode],
                                                                       [City],[CityCode],[TelephoneNumber],[FaxNumber],[EMailAddress],[CertifydMail] AS CertifiedMail,[Note],[idRole],[isActive],[isLocked],[isNotExpandable],[FullIncrementalPath],
																	   [RegistrationUser],[RegistrationDate],[LastChangedUser],[LastChangedDate],[IdTitle],[IdRoleRootContact],[ActiveFrom],[ActiveTo],[isChanged],
																	   [UniqueId],[Language],[Nationality],[BirthPlace],[Timestamp], [SDIIdentification] 
		FROM <DBProtocollo, varchar(255), Nome db Protocollo o lasciare vuoto>.[dbo].[Contact] 
		WHERE Incremental = @Incremental
	END TRY
	
	BEGIN CATCH 
		ROLLBACK TRANSACTION ContactUpdate
		
		declare @ErrorNumber as int = ERROR_NUMBER()
		declare @ErrorSeverity as int = ERROR_SEVERITY()
		declare @ErrorMessage as nvarchar(4000)
		declare @ErrorState as int = ERROR_STATE()
		declare @ErrorLine as int = ERROR_LINE()
		declare @ErrorProcedure as nvarchar(200) = ISNULL(ERROR_PROCEDURE(), '-');

		SET @ErrorMessage = 'Message: '+ ERROR_MESSAGE();

		RAISERROR 
			(
			@ErrorSeverity, 
			1,               
			@ErrorNumber,    -- parameter: original error number.
			@ErrorSeverity,  -- parameter: original error severity.
			@ErrorState,     -- parameter: original error state.
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
PRINT N'ALTER FUNCTION [webapiprivate].[TransparentAdministrationMonitors_FX_MonitoringQualityDetails]';

GO
ALTER FUNCTION [webapiprivate].[TransparentAdministrationMonitors_FX_MonitoringQualityDetails]
(
	@IdDocumentSeries int,
	@IdRole smallint,
	@DateFrom datetimeoffset,
	@DateTo datetimeoffset
)
RETURNS TABLE
AS
RETURN 
(
	WITH 
		Published AS 
		(
			SELECT I.Id AS IdDocumentSeriesItem, CAST(I.year AS nvarchar(4))+'/'+RIGHT('0000000'+CAST(i.Number AS nvarchar(7)), 7) AS Identifier, year AS [Year], Number AS [Number], 1 AS [Counter]
			FROM DocumentSeriesItem I 
			LEFT JOIN DocumentSeriesItemRole R ON I.Id = R.IdDocumentSeriesItem 
			WHERE  I.Status = 1 AND I.PublishingDate IS NOT NULL AND I.IdDocumentSeries = @IdDocumentSeries AND 
				   ((R.Id IS NULL AND @IdRole IS NULL) OR (R.Id IS NOT NULL AND R.LinkType = 0 AND R.IdRole = @IdRole)) AND
				    COALESCE(I.LastChangedDate,I.RegistrationDate) BETWEEN @DateFrom AND @DateTo
		),
		WithoutDocuments AS 
		(
			SELECT I.Id AS IdDocumentSeriesItem, CAST(I.year AS nvarchar(4))+'/'+RIGHT('0000000'+CAST(i.Number AS nvarchar(7)), 7) AS Identifier, year AS [Year], Number AS [Number], 1 AS [Counter]
			FROM DocumentSeriesItem I 
			LEFT JOIN DocumentSeriesItemRole R ON I.Id = R.IdDocumentSeriesItem 
			WHERE  I.Status = 1 AND I.PublishingDate IS NOT NULL AND I.IdDocumentSeries = @IdDocumentSeries AND 
				   ((R.Id IS NULL AND @IdRole IS NULL) OR (R.Id IS NOT NULL AND R.LinkType = 0 AND R.IdRole = @IdRole)) AND
				    COALESCE(I.LastChangedDate,I.RegistrationDate) BETWEEN @DateFrom AND @DateTo AND
				   ('00000000-0000-0000-0000-000000000000' = ANY (SELECT IdAnnexed) AND ('00000000-0000-0000-0000-000000000000' = ANY (SELECT IdUnpublishedAnnexed)) AND (0 = ANY (SELECT HasMainDocument)))
		),
		PublishedFromResolutions AS 
		(
			SELECT I.Id AS IdDocumentSeriesItem, CAST(I.year AS nvarchar(4))+'/'+RIGHT('0000000'+CAST(i.Number AS nvarchar(7)), 7) AS Identifier,year AS [Year], Number AS [Number], 1 AS [Counter]
			FROM DocumentSeriesItem I 
			INNER JOIN ResolutionDocumentSeriesItem A ON I.Id = a.IdDocumentSeriesItem 
			LEFT JOIN DocumentSeriesItemRole R ON I.Id = R.IdDocumentSeriesItem 
			WHERE  I.Status = 1 AND I.PublishingDate IS NOT NULL AND I.IdDocumentSeries = @IdDocumentSeries AND 
				   ((R.Id IS NULL AND @IdRole IS NULL) OR (R.Id IS NOT NULL AND R.LinkType = 0 AND R.IdRole = @IdRole)) AND
				    COALESCE(I.LastChangedDate,I.RegistrationDate) BETWEEN @DateFrom AND @DateTo
		),
		PublishedFromProtocols AS 
		(
			SELECT I.Id AS IdDocumentSeriesItem, CAST(I.year AS nvarchar(4))+'/'+RIGHT('0000000'+CAST(i.Number AS nvarchar(7)), 7) AS Identifier,I.year AS [Year], I.Number AS [Number], 1 AS [Counter]
			FROM DocumentSeriesItem I 
			INNER JOIN ProtocolDocumentSeriesItems A ON I.Id = A.IdDocumentSeriesItem 
			LEFT JOIN DocumentSeriesItemRole R ON I.Id = R.IdDocumentSeriesItem 
			WHERE  I.Status = 1 AND I.PublishingDate IS NOT NULL AND I.IdDocumentSeries = @IdDocumentSeries AND 
			       ((R.Id IS NULL AND @IdRole IS NULL) OR (R.Id IS NOT NULL AND R.LinkType = 0 AND R.IdRole = @IdRole)) AND
				    COALESCE(I.LastChangedDate,I.RegistrationDate) BETWEEN @DateFrom AND @DateTo
		)

SELECT P.IdDocumentSeriesItem, P.Identifier, P.[Year], P.[Number], 
	   IsNull(P.[Counter], 0) AS Published, 
	   IsNull(A.[Counter], 0) AS FromResolution, 
	   IsNull(I.[Counter], 0) AS FromProtocol, 
	   IsNull(P.[Counter], 0) - IsNull(A.[Counter], 0) - IsNull(I.[Counter],0) AS WithoutLink,
	   IsNull(D.[Counter], 0) AS WithoutDocument
FROM Published P 
LEFT JOIN PublishedFromResolutions A ON P.IdDocumentSeriesItem = A.IdDocumentSeriesItem 
LEFT JOIN PublishedFromProtocols I ON P.IdDocumentSeriesItem = I.IdDocumentSeriesItem
LEFT JOIN WithoutDocuments D ON P.IdDocumentSeriesItem = D.IdDocumentSeriesItem
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
PRINT N'ALTER FUNCTION [webapiprivate].[TransparentAdministrationMonitors_FX_MonitoringQualitySummary]';

GO
ALTER FUNCTION [webapiprivate].[TransparentAdministrationMonitors_FX_MonitoringQualitySummary]
(
	@IdDocumentSeries int,
	@DateFROM datetimeoffset,
	@DateTo datetimeoffset
)
RETURNS TABLE
AS
RETURN 
(
	WITH 
		Published AS 
		(
			SELECT S.[Name] AS DocumentSeries, RR.[Name] AS [Role], S.Id AS IdDocumentSeries, RR.IdRole, count(I.Id) AS [Counter]
			FROM DocumentSeries S 
			INNER JOIN DocumentSeriesItem I ON I.IdDocumentSeries = S.Id 
			LEFT JOIN DocumentSeriesItemRole R ON I.Id = R.IdDocumentSeriesItem 
			LEFT JOIN [Role] RR ON RR.IdRole = R.IdRole 
			WHERE S.PublicationEnabled = 1 AND (R.Id IS NULL OR (R.Id IS NOT NULL AND R.LinkType = 0)) AND 
			      (@IdDocumentSeries IS NULL OR (@IdDocumentSeries IS NOT NULL AND S.Id = @IdDocumentSeries)) AND 
				  I.[Status] = 1 AND I.PublishingDate IS NOT NULL AND 
				  COALESCE(I.LastChangedDate,I.RegistrationDate) BETWEEN @DateFROM AND @DateTo
			GROUP BY S.[Name], RR.[Name], S.Id, RR.IdRole
		),
		WithoutDocuments AS 
		(
		    SELECT S.[Name] AS DocumentSeries, RR.[Name] AS [Role], S.Id AS IdDocumentSeries, RR.IdRole, count(I.Id) AS [Counter]
			FROM DocumentSeries S 
			INNER JOIN DocumentSeriesItem I ON I.IdDocumentSeries = S.Id 
			LEFT JOIN DocumentSeriesItemRole R ON I.Id = R.IdDocumentSeriesItem 
			LEFT JOIN [Role] RR ON RR.IdRole = R.IdRole 
			WHERE S.PublicationEnabled = 1 AND (R.Id IS NULL OR (R.Id IS NOT NULL AND R.LinkType = 0)) AND 
			      (@IdDocumentSeries IS NULL OR (@IdDocumentSeries IS NOT NULL AND S.Id = @IdDocumentSeries)) AND 
				  I.[Status] = 1 AND I.PublishingDate IS NOT NULL AND 
				  COALESCE(I.LastChangedDate,I.RegistrationDate) BETWEEN @DateFROM AND @DateTo AND
				  ('00000000-0000-0000-0000-000000000000' = ANY (SELECT IdAnnexed) AND ('00000000-0000-0000-0000-000000000000' = ANY (SELECT IdUnpublishedAnnexed)) AND (0 = ANY (SELECT HasMainDocument)))
			GROUP BY S.[Name], RR.[Name], S.Id, RR.IdRole
		),
		PublishedFromResolutions AS 
		(
			SELECT S.[Name] AS DocumentSeries, RR.[Name] AS [Role], S.Id AS IdDocumentSeries, RR.IdRole, count(I.Id) AS [Counter]
			FROM DocumentSeries S 
			INNER JOIN DocumentSeriesItem I ON I.IdDocumentSeries = S.Id 
			INNER JOIN ResolutionDocumentSeriesItem A ON I.Id = A.IdDocumentSeriesItem 
			LEFT JOIN  DocumentSeriesItemRole R ON I.Id = R.IdDocumentSeriesItem 
			LEFT JOIN [Role] RR ON RR.IdRole = R.IdRole 
			WHERE S.PublicationEnabled = 1 AND (R.Id IS NULL OR (r.Id IS NOT NULL AND R.LinkType = 0)) AND 
			      (@IdDocumentSeries IS NULL OR (@IdDocumentSeries IS NOT NULL AND S.Id = @IdDocumentSeries)) AND 
				  I.[Status] = 1 AND I.PublishingDate IS NOT NULL AND
				  COALESCE(I.LastChangedDate,I.RegistrationDate) BETWEEN @DateFROM AND @DateTo
			GROUP BY S.[Name], RR.[Name], S.Id, RR.IdRole
		),
		PublishedFromProtocols AS 
		(
			SELECT S.[Name] AS DocumentSeries, RR.[Name] AS [Role], S.Id AS IdDocumentSeries, RR.IdRole, Count(*) AS [Counter]
			FROM DocumentSeries S 
			INNER JOIN DocumentSeriesItem I ON I.IdDocumentSeries = S.Id 
			INNER JOIN ProtocolDocumentSeriesItems A ON I.Id = A.IdDocumentSeriesItem 
			LEFT JOIN  DocumentSeriesItemRole R ON I.Id = R.IdDocumentSeriesItem 
			LEFT JOIN [Role] RR ON RR.IdRole = R.IdRole 
			WHERE S.PublicationEnabled = 1 AND (R.Id IS NULL OR (r.Id IS NOT NULL AND R.LinkType = 0)) AND 
			      (@IdDocumentSeries IS NULL OR (@IdDocumentSeries IS NOT NULL AND S.Id = @IdDocumentSeries)) AND 
				  I.[Status] = 1 AND I.PublishingDate IS NOT NULL AND
				  COALESCE(I.LastChangedDate,I.RegistrationDate) BETWEEN @DateFROM AND @DateTo
			GROUP BY S.[Name], RR.[Name], S.Id, RR.IdRole
		)

SELECT P.DocumentSeries, IsNull(P.[Role], '(Senza settore)') AS [Role],P.IdDocumentSeries, P.IdRole,
	   IsNull(P.[Counter], 0) AS Published, 
	   IsNull(A.[Counter], 0) AS FromResolution, 
	   IsNull(I.[Counter], 0) AS FromProtocol, 
	   IsNull(P.[Counter], 0) - IsNull(A.[Counter], 0) - IsNull(I.[Counter],0) AS WithoutLink,
	   IsNull(D.[Counter], 0) AS WithoutDocument
FROM Published P 
LEFT JOIN PublishedFromResolutions A ON P.DocumentSeries = A.DocumentSeries AND P.[Role] = A.[Role]
LEFT JOIN PublishedFromProtocols I ON P.DocumentSeries = I.DocumentSeries AND P.[Role] = I.[Role]
LEFT JOIN WithoutDocuments D ON P.DocumentSeries = D.DocumentSeries AND P.[Role] = D.[Role]
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