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

PRINT 'CREATE FUNCTION [dbo].[AuthorizedDocuments]'
GO

CREATE FUNCTION [dbo].[AuthorizedDocuments] 
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
WITH
	MySecurityGroups AS (
        SELECT SG.IdGroup 
        FROM [dbo].[SecurityGroups] SG 
        INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
        WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
        GROUP BY SG.IdGroup
    )

	SELECT distinct DU.[IdDocumentUnit] as UniqueId
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
	  ,DU.idCategory as Category_IdCategory
	  ,CT.Name as Category_Name
	  ,DU.idContainer as Container_IdContainer
	  ,C.Name as Container_Name
	  ,(SELECT TOP 1 DocumentName 
	   FROM cqrs.DocumentUnitChains CUC 
	   WHERE CUC.IdDocumentUnit = DU.IdDocumentUnit AND CUC.DocumentName is not null AND CUC.DocumentName <> ''
	   ORDER BY CUC.RegistrationDate DESC) as DocumentUnitChain_DocumentName
	 FROM cqrs.DocumentUnits DU
	LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DUR.IdDocumentUnit = DU.IdDocumentUnit
    LEFT OUTER JOIN [dbo].[Role] R on DUR.UniqueIdRole = R.UniqueId
    LEFT OUTER JOIN [dbo].[RoleGroup] RG on R.idRole = RG.idRole
    LEFT OUTER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup
	INNER JOIN [dbo].[Category] CT on CT.idCategory = DU.idCategory
	INNER JOIN [dbo].[Container] C on C.idContainer = DU.idContainer
	WHERE DU.RegistrationDate BETWEEN @DateFrom AND @DateTo 
	AND ( (DUR.UniqueIdRole IS NOT NULL
				AND ((DU.Environment = 1 AND (RG.ProtocolRights like '1%'))
				OR (DU.Environment = 2 AND (RG.ResolutionRights like '1%'))
				OR ((DU.Environment = 4 OR DU.Environment > 100) AND (RG.DocumentSeriesRights like '1%'))) 
				AND MSG.IdGroup IS NOT NULL)
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

PRINT N'Creazione indice univoco [IX_DocumentUnits_DocumentUnitName_Year_Number]';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_DocumentUnits_DocumentUnitName_Year_Number]
    ON [cqrs].[DocumentUnits]([DocumentUnitName] ASC, [Year] ASC, [Number] ASC);
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