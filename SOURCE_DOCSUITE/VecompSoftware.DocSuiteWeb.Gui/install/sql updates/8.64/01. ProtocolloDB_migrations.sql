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
PRINT 'Versionamento database alla 8.64'
GO

EXEC dbo.VersioningDatabase N'8.64'
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
PRINT N'Creazione della colonna [IdArchiveChain] nella tabella [dbo].[TemplateDocumentRepositories]';
GO

ALTER TABLE [dbo].[TemplateDocumentRepositories] ADD [IdArchiveChain] [uniqueidentifier] NOT NULL;
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
PRINT N'Modifica della colonna [QualityTag] nella tabella [dbo].[TemplateDocumentRepositories]';
GO

ALTER TABLE [dbo].[TemplateDocumentRepositories] ALTER COLUMN [QualityTag] [nvarchar](1000) NULL;
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

PRINT N'Cancellazione e creazione CONSTRAINT [FK_TemplateProtocol_Category] ';
GO
 
IF EXISTS(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
	WHERE TABLE_NAME = 'TemplateProtocol' 
	AND TABLE_SCHEMA ='dbo'
	AND CONSTRAINT_NAME = 'FK_TemplateProtocol_Category')
BEGIN
	ALTER TABLE [dbo].[TemplateProtocol]  DROP CONSTRAINT [FK_TemplateProtocol_Category]	
END
GO
 
IF NOT EXISTS(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
	WHERE TABLE_NAME = 'TemplateProtocol' 
	AND TABLE_SCHEMA ='dbo'
	AND CONSTRAINT_NAME = 'FK_TemplateProtocol_Category')
BEGIN	
    ALTER TABLE [dbo].[TemplateProtocol]  WITH CHECK ADD CONSTRAINT [FK_TemplateProtocol_Category] FOREIGN KEY([idCategory])
    REFERENCES [dbo].[Category] ([idCategory])
END
GO
--#############################################################################         

PRINT N'Cancellazione e creazione CONSTRAINT [FK_TemplateProtocol_Container] ';
GO
 
IF EXISTS(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
	WHERE TABLE_NAME = 'TemplateProtocol' 
	AND TABLE_SCHEMA ='dbo'
	AND CONSTRAINT_NAME = 'FK_TemplateProtocol_Container')
BEGIN
	ALTER TABLE [dbo].[TemplateProtocol]  DROP CONSTRAINT [FK_TemplateProtocol_Container]	
END
GO
 
IF NOT EXISTS(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
	WHERE TABLE_NAME = 'TemplateProtocol' 
	AND TABLE_SCHEMA ='dbo'
	AND CONSTRAINT_NAME = 'FK_TemplateProtocol_Container')
BEGIN	
    ALTER TABLE [dbo].[TemplateProtocol]  WITH CHECK ADD CONSTRAINT [FK_TemplateProtocol_Container] FOREIGN KEY([idContainer])
    REFERENCES [dbo].[Container] ([idContainer])
END
GO
--#############################################################################         

PRINT N'Cancellazione e creazione CONSTRAINT [FK_TemplateProtocol_Type] ';
GO
 
IF EXISTS(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
	WHERE TABLE_NAME = 'TemplateProtocol' 
	AND TABLE_SCHEMA ='dbo'
	AND CONSTRAINT_NAME = 'FK_TemplateProtocol_Type')
BEGIN
	ALTER TABLE [dbo].[TemplateProtocol]  DROP CONSTRAINT [FK_TemplateProtocol_Type]	
END
GO
 
IF NOT EXISTS(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
	WHERE TABLE_NAME = 'TemplateProtocol' 
	AND TABLE_SCHEMA ='dbo'
	AND CONSTRAINT_NAME = 'FK_TemplateProtocol_Type')
BEGIN	
    ALTER TABLE [dbo].[TemplateProtocol]  WITH CHECK ADD CONSTRAINT [FK_TemplateProtocol_Type] FOREIGN KEY([idType])
    REFERENCES [dbo].[Type] ([idType])
END
GO
--#############################################################################        

PRINT N'Cancellazione e creazione CONSTRAINT [FK_TemplateAdvancedProtocol_Category] ';
GO
 
IF EXISTS(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
	WHERE TABLE_NAME = 'TemplateAdvancedProtocol' 
	AND TABLE_SCHEMA ='dbo'
	AND CONSTRAINT_NAME = 'FK_TemplateAdvancedProtocol_Category')
BEGIN
	ALTER TABLE [dbo].[TemplateAdvancedProtocol]  DROP CONSTRAINT [FK_TemplateAdvancedProtocol_Category]	
END
GO
 
IF NOT EXISTS(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
	WHERE TABLE_NAME = 'TemplateAdvancedProtocol' 
	AND TABLE_SCHEMA ='dbo'
	AND CONSTRAINT_NAME = 'FK_TemplateAdvancedProtocol_Category')
BEGIN	
    ALTER TABLE [dbo].[TemplateAdvancedProtocol]  WITH CHECK ADD CONSTRAINT [FK_TemplateAdvancedProtocol_Category] FOREIGN KEY([idSubCategory])
    REFERENCES [dbo].[Category] ([idCategory])
END
GO
--#############################################################################        

PRINT N'Cancellazione e creazione CONSTRAINT [FK_TemplateAdvancedProtocol_TemplateProtocol] ';
GO
 
IF EXISTS(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
	WHERE TABLE_NAME = 'TemplateAdvancedProtocol' 
	AND TABLE_SCHEMA ='dbo'
	AND CONSTRAINT_NAME = 'FK_TemplateAdvancedProtocol_TemplateProtocol')
BEGIN
	ALTER TABLE [dbo].[TemplateAdvancedProtocol]  DROP CONSTRAINT [FK_TemplateAdvancedProtocol_TemplateProtocol]	
END
GO
 
IF NOT EXISTS(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
	WHERE TABLE_NAME = 'TemplateAdvancedProtocol' 
	AND TABLE_SCHEMA ='dbo'
	AND CONSTRAINT_NAME = 'FK_TemplateAdvancedProtocol_TemplateProtocol')
BEGIN	
    ALTER TABLE [dbo].[TemplateAdvancedProtocol]  WITH CHECK ADD CONSTRAINT [FK_TemplateAdvancedProtocol_TemplateProtocol] FOREIGN KEY([idTemplateProtocol])
   REFERENCES [dbo].[TemplateProtocol] ([idTemplateProtocol])
   ON UPDATE CASCADE
END
GO
--#############################################################################       

PRINT N'Cancellazione e creazione CONSTRAINT [FK_ContainerDocType_Container] ';
GO
 
IF EXISTS(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
	WHERE TABLE_NAME = 'ContainerDocType' 
	AND TABLE_SCHEMA ='dbo'
	AND CONSTRAINT_NAME = 'FK_ContainerDocType_Container')
BEGIN
	ALTER TABLE [dbo].[ContainerDocType]  DROP CONSTRAINT [FK_ContainerDocType_Container]	
END
GO
 
IF NOT EXISTS(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
	WHERE TABLE_NAME = 'ContainerDocType' 
	AND TABLE_SCHEMA ='dbo'
	AND CONSTRAINT_NAME = 'FK_ContainerDocType_Container')
BEGIN	
    ALTER TABLE [dbo].[ContainerDocType]  WITH CHECK ADD CONSTRAINT [FK_ContainerDocType_Container] FOREIGN KEY([IdContainer]) 
	REFERENCES [dbo].[Container] ([idContainer])
END
GO
--#############################################################################
PRINT N'Creazione di una nuova colonna [VisibilityType] nella tabella [dbo].[Fascicles]';
GO

ALTER TABLE [dbo].[Fascicles] ADD [VisibilityType] SMALLINT NULL
GO

UPDATE [dbo].[Fascicles] SET [VisibilityType] = 0
GO

ALTER TABLE [dbo].[Fascicles] ALTER COLUMN [VisibilityType] SMALLINT NOT NULL
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

PRINT N'Cancellazione tabella [dbo].[MassimariScartoSchema]';
GO

ALTER TABLE [dbo].[MassimariScartoSchema] DROP CONSTRAINT [FK_MassimariScartoSchema_MassimariScarto]
GO

DROP TABLE [dbo].[MassimariScartoSchema]
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