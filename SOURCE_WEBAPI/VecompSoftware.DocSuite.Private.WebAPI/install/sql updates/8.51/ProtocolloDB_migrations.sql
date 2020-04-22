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
PRINT 'CREATE FUNCTION [dbo].[AuthorizedProtocols]'
GO

CREATE FUNCTION [dbo].[AuthorizedProtocols]
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
	  ,P.idCategory
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
	INNER JOIN [dbo].[SecurityGroups] SG on RG.idGroup = SG.idGroup
	INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
	WHERE 
	P.RegistrationDate between @DateFrom AND @DateTo 
	AND (RG.ProtocolRights like '1%')
	AND (SU.Account = @UserName AND SU.UserDomain = @Domain) 
GROUP BY P.Year, P.Number, P.idCategory, P.idContainer, P.RegistrationDate, P.DocumentCode, P.idStatus, P.[Object], P.idType, P.idDocument, P.UniqueId)
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
		INNER JOIN [dbo].[Category] CT on P.idCategory = CT.idCategory
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
PRINT 'CREATE FUNCTION [dbo].[RegisteredCollaborations]'
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[RegisteredCollaborations](
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
                     Collaboration.LastChangedDate, Collaboration.idDocumentSeriesItem                                          
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
                              and Collaboration.IdStatus = 'PT' and Collaboration.DocumentType in ('P', 'D', 'A', 'O', 'S', 'U')
			)

            SELECT Collaboration.*, C_CS.Incremental as CollaborationSign_Incremental, C_CS.IdCollaborationSign as CollaborationSign_IdCollaborationSign, C_CS.IsActive as CollaborationSign_IsActive, C_CS.IsRequired as CollaborationSign_IsRequired, C_CS.SignName as CollaborationSign_SignName, C_CS.SignUser as CollaborationSign_SignUser, C_CS.SignDate as CollaborationSign_SignDate,
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
            WHERE (p1_.RegistrationDate between @DateFrom and @DateTo and p1_.idStatus = 0)
						or (r2_.ProposeDate between @DateFrom and @DateTo and r2_.idStatus = 0)
                        or (dsi3_.RegistrationDate between @DateFrom and @DateTo and dsi3_.Status in (1, 3))          	
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
PRINT 'CREATE FUNCTION [dbo].[ToVisionSignCollaborations]'
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[ToVisionSignCollaborations](
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
				and CC_CS.SignUser = @UserName
				and CC_CS.IsActive = 1
				and ((@IsRequired is null and CC_CS.IsRequired in (1,0)) or (@IsRequired is not null and CC_CS.IsRequired = @IsRequired))
		WHERE			
			Collaboration.IdStatus = 'IN'
			and Collaboration.IdCollaboration not in (SELECT CA.idCollaborationChild
												 FROM   dbo.CollaborationAggregate CA)

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
PRINT 'CREATE FUNCTION [dbo].[ODGCollaborations]'
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[ODGCollaborations](
	@UserName nvarchar(255),
	@Configuration nvarchar(255))
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(RODG.IdODG AS int) as IdCollaboration, 'O' as DocumentType, null as IdPriority, null as IdStatus, null as SignCount,
			   null as MemorandumDate, 
			   case RODG.IdTipologia
				when 1 then 'Delibera - Ordine del giorno ' + CAST(CONVERT(varchar, RODG.DataOdg, 103) AS nvarchar)
				when 0 then 'Determina - Ordine del giorno ' + CAST(CONVERT(varchar, RODG.DataOdg, 103) AS nvarchar)
				end as Subject, null as Note, null as Year, null as Number, null as IdResolution,
			   null as PublicationUser, null as PublicationDate, null as RegistrationName, null as RegistrationEmail, null as SourceProtocolYear,
			   null as SourceProtocolNumber, null as AlertDate, null as UniqueId, RODG.RegistrationUser as RegistrationUser, RODG.RegistrationDate as RegistrationDate, null as LastChangedUser,
			   null as LastChangedDate,
			   
			   null as CollaborationSign_IdCollaborationSign, CAST(0 AS bit) as CollaborationSign_IsActive, null as CollaborationSign_IsRequired, null as CollaborationSign_SignName, null as CollaborationSign_SignUser, null as CollaborationSign_SignDate,
			   null as CollaborationUser_IdCollaborationUser, CAST(0 AS bit) as CollaborationUser_DestinationFirst, null as CollaborationUser_DestinationName,
			   null as CollaborationVersioning_IdCollaborationVersioning, '' as CollaborationVersioning_DocumentName, CAST(0 AS smallint) as CollaborationVersioning_CollaborationIncremental, null as CollaborationVersioning_RegistrationUser, CAST(0 AS smallint) as CollaborationVersioning_Incremental,
			   null as Resolution_IdResolution, null as Resolution_Year, null as Resolution_Number, null as Resolution_ServiceNumber, null as Resolution_AdoptionDate, null as Resolution_PublishingDate,
			   null as DocumentSeriesItem_IdDocumentSeriesItem, null as DocumentSeriesItem_Number, null as DocumentSeriesItem_Year
		FROM   dbo.ResolutionODG RODG
			   inner join dbo.TabMaster C_TM on C_TM.ResolutionType = RODG.idTipologia
					and C_TM.Configuration = @Configuration
			   left outer join dbo.Container C_C on RODG.idContainer = C_C.IdContainer
		WHERE  (RODG.idDocFDQ = 0
					  or RODG.idDocFDQ is null)
				and RODG.DestinationUser = @UserName
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
PRINT 'CREATE FUNCTION [dbo].[ODGFlgCheckedCollaborations]'
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[ODGFlgCheckedCollaborations](
	@UserName nvarchar(255),
	@Configuration nvarchar(255))
	RETURNS TABLE
AS 
	RETURN
	(
		SELECT CAST(RODG.IdODG AS int) as IdCollaboration, 'O' as DocumentType, null as IdPriority, null as IdStatus, null as SignCount,
			   null as MemorandumDate, 
			   case RODG.IdTipologia
				when 1 then 'Delibera - Ordine del giorno ' + CAST(CONVERT(varchar, RODG.DataOdg, 103) AS nvarchar)
				when 0 then 'Determina - Ordine del giorno ' + CAST(CONVERT(varchar, RODG.DataOdg, 103) AS nvarchar)
				end as Subject, null as Note, null as Year, null as Number, null as IdResolution,
			   null as PublicationUser, null as PublicationDate, null as RegistrationName, null as RegistrationEmail, null as SourceProtocolYear,
			   null as SourceProtocolNumber, null as AlertDate, null as UniqueId, RODG.RegistrationUser as RegistrationUser, RODG.RegistrationDate as RegistrationDate, null as LastChangedUser,
			   null as LastChangedDate,
			   
			   null as CollaborationSign_IdCollaborationSign, CAST(0 AS bit) as CollaborationSign_IsActive, null as CollaborationSign_IsRequired, null as CollaborationSign_SignName, null as CollaborationSign_SignUser, null as CollaborationSign_SignDate,
			   null as CollaborationUser_IdCollaborationUser, CAST(0 AS bit) as CollaborationUser_DestinationFirst, null as CollaborationUser_DestinationName,
			   null as CollaborationVersioning_IdCollaborationVersioning, '' as CollaborationVersioning_DocumentName, CAST(0 AS smallint) as CollaborationVersioning_CollaborationIncremental, null as CollaborationVersioning_RegistrationUser, CAST(0 AS smallint) as CollaborationVersioning_Incremental,
			   null as Resolution_IdResolution, null as Resolution_Year, null as Resolution_Number, null as Resolution_ServiceNumber, null as Resolution_AdoptionDate, null as Resolution_PublishingDate,
			   null as DocumentSeriesItem_IdDocumentSeriesItem, null as DocumentSeriesItem_Number, null as DocumentSeriesItem_Year
		FROM   dbo.ResolutionODG RODG
			   inner join dbo.TabMaster C_TM on C_TM.ResolutionType = RODG.idTipologia
					and C_TM.Configuration = @Configuration
			   left outer join dbo.Container C_C on RODG.idContainer = C_C.IdContainer
		WHERE  (((RODG.idDocFDQ = 0 or RODG.idDocFDQ is null) and RODG.DestinationUser = @UserName) 
			   or (RODG.idDocFDQ <> 0))
			   and (RODG.FlgChecked = 0 or RODG.FlgChecked is null)
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
PRINT 'CREATE FUNCTION [dbo].[ToManageCollaborations]'
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[ToManageCollaborations](
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
			   and Collaboration.DocumentType in ('P', 'D', 'A', 'O', 'S', 'U')
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