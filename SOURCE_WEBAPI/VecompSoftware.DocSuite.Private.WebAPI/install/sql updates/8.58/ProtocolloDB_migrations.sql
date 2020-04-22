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

PRINT 'CREATE FUNCTION [dbo].[GetFascicleContactModels]'
GO


CREATE FUNCTION [dbo].[GetFascicleContactModels]
(
	@IdFascicle uniqueidentifier
)
RETURNS TABLE 
AS
RETURN 
(

WITH tblChild AS
(
    SELECT C.UniqueId, C.Incremental, C.IdContactType, C.[Description], C.IncrementalFather, 1 as IsSelected
        FROM [dbo].[FascicleContacts] FC 
		INNER JOIN [dbo].[Contact] C ON C.Incremental = FC.IdContact AND FC.IdFascicle = @IdFascicle
		GROUP BY C.UniqueId, C.Incremental, C.IncrementalFather, C.IdContactType, C.[Description]
    UNION ALL
    SELECT C.UniqueId, C.Incremental, C.IdContactType, C.[Description], C.IncrementalFather, 0 as IsSelected
		FROM [dbo].[Contact] C 
		INNER JOIN tblChild ON C.Incremental = tblChild.IncrementalFather
),
Results As
(SELECT UniqueId, Incremental as IdContact, IdContactType AS ContactType, [Description], 
		(SELECT TOP 1 I.UniqueId FROM tblChild I WHERE I.Incremental = tblChild.IncrementalFather) AS UniqueIdFather, IsSelected
	FROM tblChild
)
SELECT UniqueId, IdContact, ContactType, [Description], UniqueIdFather, CAST(MAX(IsSelected) as bit) AS IsSelected
FROM Results
GROUP BY UniqueId, IdContact, ContactType, [Description], UniqueIdFather
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

PRINT 'CREATE FUNCTION [dbo].[FascicleGenericDocumentUnits]'
GO

CREATE FUNCTION [dbo].[FascicleGenericDocumentUnits]
(
    @FascicleId uniqueidentifier,
	@FilterNameTitle nvarchar(max)
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
	  ,COALESCE(FP.ReferenceType, FR.ReferenceType, FD.ReferenceType) as ReferenceType
	  ,CT.idCategory as Category_IdCategory
      ,CT.Name as Category_Name
      ,C.idContainer as Container_IdContainer
      ,C.Name as Container_Name
	FROM cqrs.DocumentUnits DU
	LEFT OUTER JOIN dbo.FascicleProtocols FP ON FP.UniqueIdProtocol = DU.IdDocumentUnit
	LEFT OUTER JOIN dbo.FascicleResolutions FR ON FR.UniqueIdResolution = DU.IdDocumentUnit
	LEFT OUTER JOIN dbo.FascicleDocumentSeriesItems FD ON FD.IdDocumentSeriesItem = DU.EntityId
	INNER JOIN [dbo].[Category] CT on CT.idCategory = DU.idCategory
	INNER JOIN [dbo].[Container] C on C.idContainer = DU.idContainer
	WHERE 
	((FP.UniqueIdProtocol IS NOT NULL AND FP.IdFascicle = @FascicleId)
	OR (FR.UniqueIdResolution IS NOT NULL AND FR.IdFascicle = @FascicleId)
	OR (FD.IdDocumentSeriesItem IS NOT NULL AND FD.IdFascicle = @FascicleId))
	AND DU.[DocumentUnitName] + DU.Title like '%'+@FilterNameTitle+'%'
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

PRINT 'CREATE FUNCTION [dbo].[WorkflowInstance_AuthorizedInstances]'
GO

CREATE FUNCTION [dbo].[WorkflowInstance_AuthorizedInstances] 
(	
	@UserName nvarchar(256), 
    @Domain nvarchar(256)
)
RETURNS TABLE 
AS
RETURN 
(
	--TODO: implementare
	SELECT 0 AS empty_column
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