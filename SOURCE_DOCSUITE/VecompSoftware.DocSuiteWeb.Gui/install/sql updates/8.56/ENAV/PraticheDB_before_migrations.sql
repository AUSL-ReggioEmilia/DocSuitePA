
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
PRINT 'Versionamento database alla 8.56'
GO

EXEC dbo.VersioningDatabase N'8.56'
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


PRINT N'Modificata colonna [Conservation] in Container';
GO

ALTER TABLE [dbo].[Container] ALTER COLUMN [Conservation] smallint null
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

PRINT N'Modificata colonna [isActive] in Role';
GO

ALTER TABLE [dbo].[Role] ALTER COLUMN [isActive] tinyint null
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

PRINT N'Modificata colonna [Collapsed] in Role';
GO

ALTER TABLE [dbo].[Role] ALTER COLUMN [Collapsed] tinyint null
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

--#############################################################################

PRINT 'Aggiungo la colonna [Language] nella tabella [Contact]';
GO

ALTER TABLE [dbo].[Contact] ADD [Language] int null
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

PRINT 'Aggiungo la colonna [Nationality] nella tabella [Contact]';
GO

ALTER TABLE [dbo].[Contact] ADD [Nationality] nvarchar(256) null
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

PRINT 'Modificata colonna RegistrationUser in DocumentToken';
GO

ALTER TABLE [dbo].[DocumentToken] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in DocumentToken';
GO

ALTER TABLE [dbo].[DocumentToken] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in Document';
GO

ALTER TABLE [dbo].[Document] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in Document';
GO

ALTER TABLE [dbo].[Document] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in DocumentObject';
GO

ALTER TABLE [dbo].[DocumentObject] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in DocumentObject';
GO

ALTER TABLE [dbo].[DocumentObject] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna CheckOutUser in DocumentVersioning';
GO

ALTER TABLE [dbo].[DocumentVersioning] ALTER COLUMN [CheckOutUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna CheckSystemComputer in DocumentVersioning';
GO

ALTER TABLE [dbo].[DocumentVersioning] ALTER COLUMN [CheckSystemComputer] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna CheckInUser in DocumentVersioning';
GO

ALTER TABLE [dbo].[DocumentVersioning] ALTER COLUMN [CheckInUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna CheckCancelUser in DocumentVersioning';
GO

ALTER TABLE [dbo].[DocumentVersioning] ALTER COLUMN [CheckCancelUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in Category';
GO

ALTER TABLE [dbo].[Category] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in Category';
GO

ALTER TABLE [dbo].[Category] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in CategoryGroup';
GO

ALTER TABLE [dbo].[CategoryGroup] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in CategoryGroup';
GO

ALTER TABLE [dbo].[CategoryGroup] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in Contact';
GO

ALTER TABLE [dbo].[Contact] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in Contact';
GO

ALTER TABLE [dbo].[Contact] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in Container';
GO

ALTER TABLE [dbo].[Container] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in Container';
GO

ALTER TABLE [dbo].[Container] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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


PRINT 'Modificata colonna RegistrationUser in ContainerGroup';
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in ContainerGroup';
GO

ALTER TABLE [dbo].[ContainerGroup] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in Role';
GO

ALTER TABLE [dbo].[Role] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in Role';
GO

ALTER TABLE [dbo].[Role] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT 'Modificata colonna RegistrationUser in RoleGroup';
GO

ALTER TABLE [dbo].[RoleGroup] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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

PRINT 'Modificata colonna LastChangedUser in RoleGroup';
GO

ALTER TABLE [dbo].[RoleGroup] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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
--#############################################################################

PRINT 'Aggiungo la colonna IdMassimarioScarto nella tabella Category'
GO

ALTER TABLE [dbo].[Category] ADD [IdMassimarioScarto] uniqueidentifier NULL
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

PRINT 'Creo la Tabella CategorySchemas'
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CategorySchemas](
	[idCategorySchema] [uniqueidentifier] NOT NULL,
	[Version] [smallint] NOT NULL,
	[StartDate] [datetimeoffset](7) NOT NULL,
	[EndDate] [datetimeoffset](7) NULL,
	[Note] [nvarchar](256) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256)  NULL,
	[LastChangedDate] [datetimeoffset](7)  NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_CategorySchemas] PRIMARY KEY NONCLUSTERED 
(
	[idCategorySchema] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

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

PRINT 'Aggiunto clustered indice [IX_CategorySchema_RegistrationDate]';
GO

CREATE CLUSTERED INDEX [IX_CategorySchemas_RegistrationDate]
    ON [dbo].[CategorySchemas]([RegistrationDate] ASC);
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

PRINT 'Creo la colonna IdCategorySchema in Category'
GO

ALTER TABLE [dbo].[Category]
ADD [IdCategorySchema] uniqueidentifier NULL
GO

UPDATE [dbo].[Category]
SET [IdCategorySchema] = 'A4E50885-87EA-4DD2-988C-97F54EA5E645'
GO

ALTER TABLE [dbo].[Category]
ALTER COLUMN [IdCategorySchema] uniqueidentifier NOT NULL
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

PRINT 'Inserisco un record nella tabella CategorySchemas con Version=1'
GO

INSERT INTO [dbo].[CategorySchemas]
           ([IdCategorySchema]
           ,[Version]
           ,[StartDate]
           ,[Note]
           ,[RegistrationDate]
           ,[RegistrationUser])
     VALUES
           ('A4E50885-87EA-4DD2-988C-97F54EA5E645'
           ,1
           ,(SELECT TOP (1) [StartDate]
			FROM Protocollo.[dbo].[CategorySchemas])
           ,'TITOLARIO DELL''ENTE'
           ,GETDATE()
           ,'system')
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

PRINT 'Creazione FK IdCategorySchema in Category'
GO

ALTER TABLE [dbo].[Category] WITH CHECK ADD CONSTRAINT [FK_Category_CategorySchemas] FOREIGN KEY([IdCategorySchema])
REFERENCES [dbo].[CategorySchemas] ([idCategorySchema])

ALTER TABLE [dbo].[Category] CHECK CONSTRAINT [FK_Category_CategorySchemas]
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
PRINT 'Aggiungo la colonna [StartDate] nella tabella [Category]';
GO

ALTER TABLE [dbo].[Category] ADD [StartDate] datetimeoffset(7) NULL
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
PRINT 'Inserisco in StartDate la RegistrationDate del primo protocollo creato in DB';
GO

UPDATE [dbo].[Category] SET [StartDate] = (SELECT TOP (1) [StartDate]
			FROM Protocollo.[dbo].[CategorySchemas])
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
PRINT 'Setto come NOT NULL la colonna StartDate della tabella Category';
GO

ALTER TABLE [dbo].[Category] ALTER COLUMN [StartDate] datetimeoffset(7) NOT NULL
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

PRINT 'Aggiungo la colonna [EndDate] nella tabella [Category]';
GO

ALTER TABLE [dbo].[Category] ADD [EndDate] datetimeoffset(7) null
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