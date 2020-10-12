/****************************************************************************************************************************************
*	Prima di lanciare lo script è necessario attivare i template (CTRL+SHIFT+M). (Query->Specify values for template parameters)		*
*	<IdCategory,smallint,IdCategory> --> Imposta IdCategory di livello 0 del tenant corrente.														*
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
PRINT 'Versionamento database alla 9.09'
GO

EXEC dbo.VersioningDatabase N'9.09',N'DSW Version','MigrationDate'
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
PRINT N'ALTER TABLE [dbo].[Dossiers] ADD COLUMN [IdCategory]'
GO

ALTER TABLE [dbo].[Dossiers] ADD [IdCategory] smallint null
GO
ALTER TABLE [dbo].[Dossiers] WITH CHECK ADD CONSTRAINT [FK_Dossiers_Categories] FOREIGN KEY([IdCategory]) REFERENCES [dbo].[Category] ([idCategory])
GO
UPDATE [dbo].[Dossiers] SET IdCategory = <IdCategory,smallint,IdCategory>
GO
ALTER TABLE [dbo].[Dossiers] ALTER COLUMN [IdCategory] smallint NOT NULL
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
PRINT N'ALTER TABLE [dbo].[Dossiers] ADD COLUMN [DossierType]'
GO

ALTER TABLE [dbo].[Dossiers] ADD [DossierType] smallint null
GO
ALTER TABLE [dbo].[Dossiers] ADD CONSTRAINT [DF_Dossiers_DossierType] DEFAULT (2) FOR [DossierType]
GO
UPDATE D SET DossierType = 3 FROM [dbo].[Dossiers] D WHERE EXISTS (SELECT 1 FROM [dbo].[Processes] P WHERE P.IdDossier = D.IdDossier)
GO
UPDATE [dbo].[Dossiers] SET DossierType = 2 WHERE DossierType IS NULL --Procedimento
GO
ALTER TABLE [dbo].[Dossiers] ALTER COLUMN [DossierType] smallint NOT NULL
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
PRINT N'ALTER TABLE [dbo].[Dossiers] ADD COLUMN [Status]'
GO

ALTER TABLE [dbo].[Dossiers] ADD [Status] smallint null
GO
ALTER TABLE [dbo].[Dossiers] ADD CONSTRAINT [DF_Dossiers_Status] DEFAULT (1) FOR [Status]
GO
UPDATE [dbo].[Dossiers] SET Status = 1 --Aperto
GO
ALTER TABLE [dbo].[Dossiers] ALTER COLUMN [Status] smallint NOT NULL
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