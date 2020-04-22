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
PRINT 'Versionamento database alla 8.50'
GO

EXEC dbo.VersioningDatabase N'8.50'
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

PRINT 'Creazione ForeignKey nella tabella Role'
GO

ALTER TABLE [dbo].[Role] WITH CHECK
  ADD CONSTRAINT [FK_Role_DocmLocation_Location] 
	FOREIGN KEY ([DocmLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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


PRINT 'Creazione ForeignKey nella tabella Role'
GO

ALTER TABLE [dbo].[Role] WITH CHECK
  ADD CONSTRAINT [FK_Role_ProtLocation_Location] 
	FOREIGN KEY ([ProtLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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


PRINT 'Creazione ForeignKey nella tabella Role'
GO

ALTER TABLE [dbo].[Role] WITH CHECK
  ADD CONSTRAINT [FK_Role_ReslLocation_Location] 
	FOREIGN KEY ([ReslLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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

PRINT 'Creazione ForeignKey nella tabella SecurityGroups'
GO

ALTER TABLE [dbo].[SecurityGroups] WITH CHECK
  ADD CONSTRAINT [FK_SecurityGroups_SecurityGroupsFather] 
	FOREIGN KEY ([idGroupFather]) 
	REFERENCES [dbo].[SecurityGroups] ([idGroup]);
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

PRINT 'Creazione ForeignKey nella tabella SecurityUsers'
GO

ALTER TABLE [dbo].[SecurityUsers] WITH CHECK
  ADD CONSTRAINT [FK_SecurityUsers_SecurityGroups] 
	FOREIGN KEY ([idGroup]) 
	REFERENCES [dbo].[SecurityGroups] ([idGroup]);
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


PRINT 'Creazione ForeignKey nella tabella Container'
GO

ALTER TABLE [dbo].[Container] WITH CHECK
  ADD CONSTRAINT [FK_Container_ProtLocation_Location] 
	FOREIGN KEY ([ProtLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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


PRINT 'Creazione ForeignKey nella tabella Container'
GO

ALTER TABLE [dbo].[Container] WITH CHECK
  ADD CONSTRAINT [FK_Container_DocmLocation_Location] 
	FOREIGN KEY ([DocmLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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

PRINT 'Creazione ForeignKey nella tabella Container'
GO

ALTER TABLE [dbo].[Container] WITH CHECK
  ADD CONSTRAINT [FK_Container_ReslLocation_Location] 
	FOREIGN KEY ([ReslLocation]) 
	REFERENCES [dbo].[Location] ([IdLocation]);
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