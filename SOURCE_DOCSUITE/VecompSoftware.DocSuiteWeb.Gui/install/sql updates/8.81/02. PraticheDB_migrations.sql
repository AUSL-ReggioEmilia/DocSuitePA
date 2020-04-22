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
PRINT 'Versionamento database alla 8.81'
GO

-- =============================================
-- Author:		FLazzarotto
-- Create date: 2019-05-27
-- Description:	Storicizzazione della versione del database e della data dell'ultimo aggiornamento
--				Impostare la Version nel formato N'XX.XX'
-- =============================================
ALTER PROCEDURE [dbo].[VersioningDatabase] 
	@Version AS NCHAR(5),
	@AppName AS NCHAR(25),
	@MigrateLabel AS NCHAR(25)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @DateNow AS NCHAR(23)
	SET @DateNow = CONVERT(NCHAR(23), SYSDATETIME(), 126)
	IF EXISTS( select * from sys.extended_properties Where class_desc = 'DATABASE' And name = @AppName)
	BEGIN
		-- Aggiornamento property
		EXEC sys.sp_updateextendedproperty @name = @AppName, @value = @Version;
	END
	ELSE
	BEGIN 
		-- Aggiunta property
		EXEC sys.sp_addextendedproperty @name = @AppName, @value = @Version;		
	END

	IF EXISTS( select * from sys.extended_properties Where class_desc = 'DATABASE' And name = @MigrateLabel)
	BEGIN
		EXEC sys.sp_updateextendedproperty @name = @MigrateLabel, @value = @DateNow;
	END
	ELSE
	BEGIN 
		-- Aggiunta property
		EXEC sys.sp_addextendedproperty @name = @MigrateLabel, @value = @DateNow;
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
PRINT 'Versionamento database alla 8.81'
GO

EXEC dbo.VersioningDatabase N'8.81',N'DSW Version','MigrationDate'
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
PRINT N'DROP [dbo].[CategoryGroup]';
GO

DROP TABLE [dbo].[CategoryGroup]

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