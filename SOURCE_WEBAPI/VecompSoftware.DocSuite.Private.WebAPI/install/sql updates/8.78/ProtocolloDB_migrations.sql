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
PRINT N'Modificata SQLFUNCTION [webapiprivate].[Tenants_FX_UserTenants]';
GO

CREATE FUNCTION [webapiprivate].[Tenants_FX_UserTenants]
(​
	@UserName nvarchar(256), 
	@Domain nvarchar(256)
​)
RETURNS TABLE
AS
RETURN
(
	WITH
	MySecurityGroups AS (
		SELECT IdGroup FROM [dbo].[UserSecurityGroups](@UserName,@Domain)
	)
​
	SELECT  T.IdTenant, T.TenantName, T.CompanyName, T.StartDate, T.EndDate, T.Note, T.RegistrationUser, T.RegistrationDate, T.LastChangedDate, T.LastChangedUser, T.Timestamp
	FROM Tenants T​
	WHERE EXISTS (SELECT TOP 1 CG.IdContainerGroup ​
		FROM [dbo].[ContainerGroup] CG​
		INNER JOIN [dbo].[TenantContainers] TC ON TC.EntityShortId = CG.idContainer ​
				  WHERE EXISTS (SELECT 1 FROM MySecurityGroups SG WHERE SG.IdGroup = CG.IdGroup)
		AND TC.IdTenant = T.IdTenant)  ​
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