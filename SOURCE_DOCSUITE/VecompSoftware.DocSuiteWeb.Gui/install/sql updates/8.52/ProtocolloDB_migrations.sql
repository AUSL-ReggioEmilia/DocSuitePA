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
PRINT 'Versionamento database alla 8.52'
GO

EXEC dbo.VersioningDatabase N'8.52'
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

PRINT N'Aggiunto unique indice [IX_Protocol_UniqueId] in Protocol';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Protocol_UniqueId]
    ON [dbo].[Protocol]([UniqueId] ASC);
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

EXEC sp_rename 'Fascicle', 'Fascicles';
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

--############################################################################

PRINT 'Elimino la chiave esterna a Fascicle in FascicleContact'
GO

IF EXISTS(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
	WHERE TABLE_NAME = 'FascicleContact' 
	AND TABLE_SCHEMA ='dbo'
	AND CONSTRAINT_NAME = 'FK_FascicleContact_Fascicle')
BEGIN
	ALTER TABLE [dbo].[FascicleContact] DROP CONSTRAINT [FK_FascicleContact_Fascicle]
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

PRINT 'Elimino la chiave esterna a Fascicle in FascicleProtocols'
GO

IF EXISTS(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
	WHERE TABLE_NAME = 'FascicleProtocols' 
	AND TABLE_SCHEMA ='dbo'
	AND CONSTRAINT_NAME = 'FK_FasciclesProtocols_Fascicle')
BEGIN
ALTER TABLE [dbo].[FascicleProtocols] DROP CONSTRAINT [FK_FasciclesProtocols_Fascicle]
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

PRINT 'DROP Primary Key Fascicles'
GO

declare @table_name nvarchar(256)
declare @Command  nvarchar(1000)

set @table_name = N'Fascicles'

select TOP 1 @Command = 'ALTER TABLE ' + @table_name + ' DROP CONSTRAINT ' + i.name 
 FROM    sys.indexes AS i INNER JOIN 
        sys.index_columns AS ic ON  i.OBJECT_ID = ic.OBJECT_ID
                                AND i.index_id = ic.index_id
WHERE   i.is_primary_key = 1 and OBJECT_NAME(ic.OBJECT_ID) = @table_name

PRINT @Command

execute (@Command)

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

PRINT N'Aggiunta colonna [IdFascicle] e impostata come chiave primaria in Fascicles';
GO

ALTER TABLE [dbo].[Fascicles] ADD [IdFascicle] UNIQUEIDENTIFIER  DEFAULT NEWSEQUENTIALID() NOT NULL
GO
ALTER TABLE [dbo].[Fascicles] ADD CONSTRAINT [PK_Fascicles] PRIMARY KEY NONCLUSTERED ([IdFascicle] ASC)
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

EXEC sp_rename 'Fascicles.idSubCategory', 'IdCategory', 'COLUMN';
GO

EXEC sp_rename 'Fascicles.idStatus', 'IdStatus', 'COLUMN';
GO

EXEC sp_rename 'Fascicles.Incremental', 'Number', 'COLUMN';
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

--############################################################################

PRINT N'Aggiunto clustered indice [IX_Fascicles_RegistationDate] in Fascicles';
GO

CREATE CLUSTERED INDEX [IX_Fascicles_RegistationDate]
    ON [dbo].[Fascicles]([RegistrationDate] ASC);
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

PRINT N'Modificata colonna [Number] in Fascicles';
GO

ALTER TABLE [dbo].[Fascicles] ALTER COLUMN [Number] INTEGER NOT NULL
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

PRINT N'Aggiunto clustered indice [IX_Fascicles_Year_IdCategory_Number] in Fascicles';
GO

CREATE	UNIQUE NONCLUSTERED INDEX [IX_Fascicles_Year_IdCategory_Number]
    ON [dbo].[Fascicles]([Year] ASC, [IdCategory] ASC, [Number] ASC);
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

PRINT N'Aggiunta foreign key [FK_Fascicles_Category] a Category in Fascicles';
GO

ALTER TABLE [dbo].[Fascicles] WITH CHECK ADD CONSTRAINT [FK_Fascicles_Category] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[Category] ([idCategory])

ALTER TABLE [dbo].[Fascicles] CHECK CONSTRAINT [FK_Fascicles_Category]
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

PRINT N'Drop colonna [IdStatus] in Fascicles';
GO

ALTER TABLE [dbo].[Fascicles] DROP COLUMN [IdStatus]
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

PRINT N'Modificata colonna [StartDate] in Fascicles';
GO

ALTER TABLE [dbo].[Fascicles] ALTER COLUMN [StartDate] DATETIMEOFFSET (7) NOT NULL
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

PRINT N'Modificata colonna [EndDate] in Fascicles';
GO

ALTER TABLE [dbo].[Fascicles] ALTER COLUMN [EndDate] DATETIMEOFFSET (7) NULL
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

PRINT N'Modificata colonna [RegistrationUser] in Fascicles';
GO

ALTER TABLE [dbo].[Fascicles] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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


PRINT N'Modificata colonna [LastChangedUser] in Fascicles';
GO

ALTER TABLE [dbo].[Fascicles] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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


PRINT N'Aggiunta colonna [FascicleType] in Fascicles';
GO

ALTER TABLE [dbo].[Fascicles] ADD [FascicleType] SMALLINT NULL
GO

UPDATE [dbo].[Fascicles] SET [FascicleType] = -1
GO

ALTER TABLE [dbo].[Fascicles] ALTER COLUMN [FascicleType] SMALLINT NOT NULL
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

PRINT N'Aggiunta colonna [Timestamp] in Fascicles';
GO

ALTER TABLE [dbo].[Fascicles] ADD [Timestamp] TIMESTAMP NOT NULL
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

PRINT N'Aggiunta colonna [Title] in Fascicles';
GO

ALTER TABLE [dbo].[Fascicles] ADD [Title] NVARCHAR(256) NULL
GO

CREATE FUNCTION [dbo].[_tmp.SplitString]
    (
        @List NVARCHAR(MAX),
        @Delim VARCHAR(255)
    )
    RETURNS TABLE
    AS
        RETURN ( SELECT [Value] FROM 
          ( 
            SELECT 
              [Value] = LTRIM(RTRIM(SUBSTRING(@List, [Number],
              CHARINDEX(@Delim, @List + @Delim, [Number]) - [Number])))
            FROM (SELECT Number = ROW_NUMBER() OVER (ORDER BY name)
              FROM sys.all_objects) AS x
              WHERE Number <= LEN(@List)
              AND SUBSTRING(@Delim + @List, [Number], LEN(@Delim)) = @Delim
          ) AS y
        );
GO

CREATE FUNCTION [dbo].[_tmp_updatefascicletitle](@IdCategory as smallint)
    RETURNS NVARCHAR(256) AS
BEGIN
	DECLARE @Names VARCHAR(256) 
	SELECT @Names = COALESCE(@Names + '.','') + CAST(Code AS NVARCHAR(5))
	FROM [_tmp.SplitString]((SELECT TOP 1 FullIncrementalPath FROM [dbo].[Category] WHERE IdCategory = @IdCategory),'|') inner join [dbo].[Category] on [Value] = IdCategory 
	order by [FullCode]

    RETURN @Names
END;
GO

UPDATE [dbo].[Fascicles] 
SET Title = (SELECT CAST(YEAR(StartDate) AS NVARCHAR(4))+ '.'+(select [dbo].[_tmp_updatefascicletitle] (IdCategory)) + '-' +RIGHT('0000000' + CAST(Number AS NVARCHAR(7)),7) ) 
WHERE TITLE IS NULL

GO
DROP FUNCTION [dbo].[_tmp_updatefascicletitle]

GO
DROP FUNCTION [dbo].[_tmp.SplitString]
GO

ALTER TABLE [dbo].[Fascicles] ALTER COLUMN [Title] NVARCHAR(256) NOT NULL
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

EXEC sp_rename 'FascicleContact', 'FascicleContacts';
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

--############################################################################

PRINT 'DROP Primary Key FascicleContacts'
GO

declare @table_name nvarchar(256)
declare @Command  nvarchar(1000)

set @table_name = N'FascicleContacts'

select TOP 1 @Command = 'ALTER TABLE ' + @table_name + ' DROP CONSTRAINT ' + i.name 
 FROM    sys.indexes AS i INNER JOIN 
        sys.index_columns AS ic ON  i.OBJECT_ID = ic.OBJECT_ID
                                AND i.index_id = ic.index_id
WHERE   i.is_primary_key = 1 and OBJECT_NAME(ic.OBJECT_ID) = @table_name

PRINT @Command

execute (@Command)

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

--############################################################################

PRINT N'Aggiunta colonna [IdFascicleContact] e impostata come chiave primaria in FascicleContacts';
GO

ALTER TABLE [dbo].[FascicleContacts] ADD [IdFascicleContact] UNIQUEIDENTIFIER  DEFAULT NEWSEQUENTIALID() NOT NULL
GO
ALTER TABLE [dbo].[FascicleContacts] ADD CONSTRAINT [PK_FascicleContacts] PRIMARY KEY NONCLUSTERED ([IdFascicleContact] ASC)
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
PRINT N'Aggiunta colonna [IdFascicle] in FascicleContacts';
GO

ALTER TABLE [dbo].[FascicleContacts] ADD [IdFascicle] UNIQUEIDENTIFIER NULL
GO

UPDATE [dbo].[FascicleContacts] SET IdFascicle = F.IdFascicle
FROM [dbo].[Fascicles] F
inner join [dbo].[FascicleContacts] FC ON FC.FYear = F.Year AND FC.FIncremental = F.Number AND FC.FidSubCategory = F.IdCategory
GO

ALTER TABLE [dbo].[FascicleContacts] ALTER COLUMN [IdFascicle] UNIQUEIDENTIFIER NOT NULL
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

PRINT N'Impostata [IdFascicle] chiave esterna a Fascicles';
GO

ALTER TABLE [dbo].[FascicleContacts] WITH CHECK ADD CONSTRAINT [FK_FascicleContacts_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])

ALTER TABLE [dbo].[FascicleContacts] CHECK CONSTRAINT [FK_FascicleContacts_Fascicles]
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

PRINT N'Modificata chiave esterna a Contact in FascicleContacts';
GO

IF EXISTS(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
	WHERE TABLE_NAME = 'FascicleContacts' 
	AND TABLE_SCHEMA ='dbo'
	AND CONSTRAINT_NAME = 'FK_FascicleContact_Contact')
BEGIN
	ALTER TABLE [dbo].[FascicleContacts] DROP CONSTRAINT [FK_FascicleContact_Contact]
END
GO


ALTER TABLE [dbo].[FascicleContacts] WITH CHECK ADD CONSTRAINT [FK_FascicleContacts_Contact] FOREIGN KEY([IdContact])
REFERENCES [dbo].[Contact] ([Incremental])

ALTER TABLE [dbo].[FascicleContacts] CHECK CONSTRAINT [FK_FascicleContacts_Contact]
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

PRINT N'Rese colonne [RegistrationDate] e [RegistrationUser] nullable in FascicleContacts';
GO

ALTER TABLE [dbo].[FascicleContacts] ALTER COLUMN [RegistrationDate] datetimeoffset(7) NULL
GO

ALTER TABLE [dbo].[FascicleContacts] ALTER COLUMN [RegistrationUser] nvarchar(256) NULL
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

PRINT N'Aggiunto clustered indice [IX_FascicleContacts_RegistationDate] in FascicleContacts';
GO

CREATE CLUSTERED INDEX [IX_FascicleContacts_RegistationDate]
    ON [dbo].[FascicleContacts]([RegistrationDate] ASC);
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

PRINT N'Drop column [FYear], [FidSubCategory], [FIncremental] in FascicleContacts';

ALTER TABLE [dbo].[FascicleContacts] DROP COLUMN [FYear], [FidSubCategory], [FIncremental]

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

PRINT 'DROP Primary Key FascicleProtocols'
GO

declare @table_name nvarchar(256)
declare @Command  nvarchar(1000)

set @table_name = N'FascicleProtocols'

select TOP 1 @Command = 'ALTER TABLE ' + @table_name + ' DROP CONSTRAINT ' + i.name 
 FROM    sys.indexes AS i INNER JOIN 
        sys.index_columns AS ic ON  i.OBJECT_ID = ic.OBJECT_ID
                                AND i.index_id = ic.index_id
WHERE   i.is_primary_key = 1 and OBJECT_NAME(ic.OBJECT_ID) = @table_name

PRINT @Command

execute (@Command)

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

--############################################################################

PRINT N'Aggiunta colonna [IdFascicleProtocol] e impostata come chiave primaria in FascicleProtocols';
GO

ALTER TABLE [dbo].[FascicleProtocols] ADD [IdFascicleProtocol] UNIQUEIDENTIFIER  DEFAULT NEWSEQUENTIALID() NOT NULL
GO
ALTER TABLE [dbo].[FascicleProtocols] ADD CONSTRAINT [PK_FascicleProtocols] PRIMARY KEY NONCLUSTERED ([IdFascicleProtocol] ASC)
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

PRINT N'Aggiunto clustered indice [IX_FascicleProtocols_RegistationDate] in FascicleProtocols';
GO

CREATE CLUSTERED INDEX [IX_FascicleProtocols_RegistationDate]
    ON [dbo].[FascicleProtocols]([RegistrationDate] ASC);
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

PRINT 'Elimino la chiave esterna a Protocol in FascicleProtocols'
GO

IF EXISTS(
	SELECT * 
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
	WHERE TABLE_NAME = 'FascicleProtocols' 
	AND TABLE_SCHEMA ='dbo'
	AND CONSTRAINT_NAME = 'FK_FasciclesProtocols_Protocol')
BEGIN
ALTER TABLE [dbo].[FascicleProtocols] DROP CONSTRAINT [FK_FasciclesProtocols_Protocol]
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

PRINT 'Elimino indice [IX_FasciclesProtocols] in FascicleProtocols'
GO

DROP INDEX [IX_FasciclesProtocols] ON [dbo].[FascicleProtocols]
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

PRINT N'Aggiunta colonna [IdFascicle] in FascicleProtocols';
GO

ALTER TABLE [dbo].[FascicleProtocols] ADD [IdFascicle] UNIQUEIDENTIFIER NULL
GO

UPDATE [dbo].[FascicleProtocols] SET IdFascicle = F.IdFascicle
FROM [dbo].[Fascicles] F
inner join [dbo].[FascicleProtocols] FP ON FP.FYear = F.Year AND FP.FIncremental = F.Number AND FP.FidSubCategory = F.IdCategory
GO

ALTER TABLE [dbo].[FascicleProtocols] ALTER COLUMN [IdFascicle] UNIQUEIDENTIFIER NOT NULL
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

PRINT N'Impostata [IdFascicle] chiave esterna a Fascicles in FascicleProtocols';
GO

ALTER TABLE [dbo].[FascicleProtocols] WITH CHECK ADD CONSTRAINT [FK_FascicleProtocols_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])

ALTER TABLE [dbo].[FascicleProtocols] CHECK CONSTRAINT [FK_FascicleProtocols_Fascicles]
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


PRINT N'Aggiunta colonna [UniqueIdProtocol] in FascicleProtocols';
GO

ALTER TABLE [dbo].[FascicleProtocols] ADD [UniqueIdProtocol] UNIQUEIDENTIFIER NULL
GO

UPDATE [dbo].[FascicleProtocols] SET UniqueIdProtocol = P.UniqueId
FROM [dbo].[Protocol] P
inner join [dbo].[FascicleProtocols] FP ON FP.PYear = P.Year AND FP.PNumber = P.Number
GO

ALTER TABLE [dbo].[FascicleProtocols] ALTER COLUMN [UniqueIdProtocol] UNIQUEIDENTIFIER NOT NULL
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

PRINT N'Impostata [UniqueIdProtocol] chiave esterna a Protocol in FascicleProtocols';
GO

ALTER TABLE [dbo].[FascicleProtocols] WITH CHECK ADD CONSTRAINT [FK_FascicleProtocols_Protocol] FOREIGN KEY([UniqueIdProtocol])
REFERENCES [dbo].[Protocol] ([UniqueId])

ALTER TABLE [dbo].[FascicleProtocols] CHECK CONSTRAINT [FK_FascicleProtocols_Protocol]
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

PRINT 'Aggiunta colonna [ReferenceType] in FascicleProtocols'
GO

ALTER TABLE [dbo].[FascicleProtocols] ADD [ReferenceType] SMALLINT DEFAULT 0 NOT NULL
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

PRINT N'Modificata colonna [RegistrationUser] in FascicleProtocols';
GO

ALTER TABLE [dbo].[FascicleProtocols] ALTER COLUMN [RegistrationUser] NVARCHAR(256) NOT NULL
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


PRINT N'Modificata colonna [LastChangedUser] in FascicleProtocols';
GO

ALTER TABLE [dbo].[FascicleProtocols] ALTER COLUMN [LastChangedUser] NVARCHAR(256) NULL
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

PRINT N'Drop column [FYear], [FidSubCategory], [FIncremental], [PYear], [PNumber] in FascicleProtocols';

ALTER TABLE [dbo].[FascicleProtocols] DROP COLUMN [FYear], [FidSubCategory], [FIncremental], [PYear], [PNumber]
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

PRINT N'Aggiunto unique index [IX_IdFascicle_UniqueIdProtocol_ReferenceType] in FascicleProtocols';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_IdFascicle_UniqueIdProtocol_ReferenceType] 
	ON [dbo].[FascicleProtocols]([IdFascicle] ASC, [UniqueIdProtocol] ASC, [ReferenceType] ASC)
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

PRINT N'Aggiunta colonna [Timestamp] in FascicleProtocols';
GO

ALTER TABLE [dbo].[FascicleProtocols] ADD [Timestamp] TIMESTAMP NOT NULL
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

PRINT 'Creata Tabella [FascicleDocumentSeriesItems]'
GO

CREATE TABLE [dbo].[FascicleDocumentSeriesItems](
	[IdFascicleDocumentSeriesItem] [uniqueidentifier] NOT NULL,
	[IdFascicle] [uniqueidentifier] NOT NULL,
	[IdDocumentSeriesItem] [int] NOT NULL,
	[ReferenceType] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,	
 CONSTRAINT [PK_FascicleDocumentSeriesItems] PRIMARY KEY NONCLUSTERED 
(
	[IdFascicleDocumentSeriesItem] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FascicleDocumentSeriesItems] WITH CHECK ADD CONSTRAINT [FK_FascicleDocumentSeriesItems_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
GO
ALTER TABLE [dbo].[FascicleDocumentSeriesItems] CHECK CONSTRAINT [FK_FascicleDocumentSeriesItems_Fascicles]
GO


ALTER TABLE [dbo].[FascicleDocumentSeriesItems] WITH CHECK ADD CONSTRAINT [FK_FascicleDocumentSeriesItems_DocumentSeriesItem] FOREIGN KEY([IdDocumentSeriesItem])
REFERENCES [dbo].[DocumentSeriesItem] ([Id])
GO
ALTER TABLE [dbo].[FascicleDocumentSeriesItems] CHECK CONSTRAINT [FK_FascicleDocumentSeriesItems_DocumentSeriesItem]
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

PRINT N'Aggiunto clustered indice [IX_FascicleDocumentSeriesItems_RegistationDate] in FascicleDocumentSeriesItems';
GO

CREATE CLUSTERED INDEX [IX_FascicleDocumentSeriesItems_RegistationDate]
    ON [dbo].[FascicleDocumentSeriesItems]([RegistrationDate] ASC);
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

PRINT N'Aggiunta colonna [Timestamp] in FascicleDocumentSeriesItems';
GO

ALTER TABLE [dbo].[FascicleDocumentSeriesItems] ADD [Timestamp] TIMESTAMP NOT NULL
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

PRINT 'Creata Tabella [FascicleResolutions]'
GO

CREATE TABLE [dbo].[FascicleResolutions](
	[IdFascicleResolution] [uniqueidentifier] NOT NULL,
    [IdFascicle] [uniqueidentifier] NOT NULL,
	[UniqueIdResolution] [uniqueidentifier] NOT NULL,
    [ReferenceType] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,	
 CONSTRAINT [PK_FascicleResolutions] PRIMARY KEY NONCLUSTERED 
(
	[IdFascicleResolution] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[FascicleResolutions] WITH CHECK ADD CONSTRAINT [FK_FascicleResolutions_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
GO
ALTER TABLE [dbo].[FascicleResolutions] CHECK CONSTRAINT [FK_FascicleResolutions_Fascicles]
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

PRINT N'Aggiunto clustered indice [IX_FascicleResolutions_RegistationDate] in FascicleResolutions';
GO

CREATE CLUSTERED INDEX [IX_FascicleResolutions_RegistationDate]
    ON [dbo].[FascicleResolutions]([RegistrationDate] ASC);
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

PRINT N'Aggiunto unique index [IX_IdFascicle_UniqueIdResolution_ReferenceType] in FascicleResolutions';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_IdFascicle_UniqueIdResolution_ReferenceType] 
	ON [dbo].[FascicleResolutions]([IdFascicle] ASC, [UniqueIdResolution] ASC, [ReferenceType] ASC)
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

PRINT N'Aggiunta colonna [Timestamp] in FascicleResolutions';
GO

ALTER TABLE [dbo].[FascicleResolutions] ADD [Timestamp] TIMESTAMP NOT NULL
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

PRINT 'Creata Tabella [FascicleUDS]'
GO

CREATE TABLE [dbo].[FascicleUDS](
	[IdFascicleUDS] [uniqueidentifier] NOT NULL,
    [IdFascicle] [uniqueidentifier] NOT NULL,
    [IdUDSRepository] [uniqueidentifier] NOT NULL,
	[IdUDS] [uniqueidentifier] NOT NULL,
    [ReferenceType] [smallint] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,	
 CONSTRAINT [PK_FascicleUDS] PRIMARY KEY NONCLUSTERED 
(
	[IdFascicleUDS] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[FascicleUDS] WITH CHECK ADD CONSTRAINT [FK_FascicleUDS_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
GO

ALTER TABLE [dbo].[FascicleUDS] CHECK CONSTRAINT [FK_FascicleUDS_Fascicles]
GO

ALTER TABLE [dbo].[FascicleUDS] WITH CHECK ADD CONSTRAINT [FK_FascicleUDS_UDSRepositories] FOREIGN KEY([IdUDSRepository])
REFERENCES [uds].[UDSRepositories] ([IdUDSRepository])
GO

ALTER TABLE [dbo].[FascicleUDS] CHECK CONSTRAINT [FK_FascicleUDS_UDSRepositories]
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

PRINT N'Aggiunto clustered indice [IX_FascicleUDS_RegistationDate] in FascicleUDS';
GO

CREATE CLUSTERED INDEX [IX_FascicleUDS_RegistationDate]
    ON [dbo].[FascicleUDS]([RegistrationDate] ASC);
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

PRINT N'Aggiunta colonna [Timestamp] in FascicleUDS';
GO

ALTER TABLE [dbo].[FascicleUDS] ADD [Timestamp] TIMESTAMP NOT NULL
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

PRINT N'Aggiunta colonna [Timestamp] a [WorkflowRepositories]';
GO

ALTER TABLE [dbo].[WorkflowRepositories] ADD [Timestamp] TIMESTAMP NOT NULL
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

PRINT N'Aggiunta colonna [Timestamp] a [WorkflowProperties]';
GO

ALTER TABLE [dbo].[WorkflowProperties] ADD [Timestamp] TIMESTAMP NOT NULL
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

PRINT N'Aggiunta colonna [Timestamp] a [WorkflowInstances]';
GO

ALTER TABLE [dbo].[WorkflowInstances] ADD [Timestamp] TIMESTAMP NOT NULL
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


PRINT N'Aggiunta colonna [Timestamp] a [WorkflowAuthorizations]';
GO

ALTER TABLE [dbo].[WorkflowAuthorizations] ADD [Timestamp] TIMESTAMP NOT NULL
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

PRINT N'Aggiunta colonna [Timestamp] a [WorkflowActivityLogs]';
GO

ALTER TABLE [dbo].[WorkflowActivityLogs] ADD [Timestamp] TIMESTAMP NOT NULL
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

PRINT N'Aggiunta colonna [Timestamp] a [WorkflowActivities]';
GO

ALTER TABLE [dbo].[WorkflowActivities] ADD [Timestamp] TIMESTAMP NOT NULL
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

PRINT N'Modificata chiave primaria nonclustered [PK_WorkflowAuthorizations]';
GO

ALTER TABLE [dbo].[WorkflowAuthorizations] DROP CONSTRAINT [PK_WorkflowAuthorizations]
GO

ALTER TABLE [dbo].[WorkflowAuthorizations] ADD CONSTRAINT [PK_WorkflowAuthorizations] PRIMARY KEY NONCLUSTERED([IdWorkflowAuthorization])
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

PRINT N'Aggiunto clustered indice [IX_WorkflowAuthorizations_RegistationDate]';
GO

CREATE CLUSTERED INDEX [IX_WorkflowAuthorizations_RegistationDate]
    ON [dbo].[WorkflowAuthorizations]([RegistrationDate] ASC);
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

PRINT N'Modificata chiave primaria nonclustered [PK_WorkflowActivityLogs]';
GO

ALTER TABLE [dbo].[WorkflowActivityLogs] DROP CONSTRAINT [PK_WorkflowActivityLogs]
GO

ALTER TABLE [dbo].[WorkflowActivityLogs] ADD CONSTRAINT [PK_WorkflowActivityLogs] PRIMARY KEY NONCLUSTERED([IdWorkflowActivityLog])
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

PRINT N'Aggiunto clustered indice [IX_WorkflowActivityLogs_RegistationDate]';
GO

CREATE CLUSTERED INDEX [IX_WorkflowActivityLogs_RegistationDate]
    ON [dbo].[WorkflowActivityLogs]([RegistrationDate] ASC);
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

PRINT N'Modificata chiave primaria nonclustered [PK_WorkflowProperties]';
GO

ALTER TABLE [dbo].[WorkflowProperties] DROP CONSTRAINT [PK_WorkflowProperties]
GO

ALTER TABLE [dbo].[WorkflowProperties] ADD CONSTRAINT [PK_WorkflowProperties] PRIMARY KEY NONCLUSTERED([IdWorkflowProperty])
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

PRINT N'Aggiunto clustered indice [IX_WorkflowProperties_RegistationDate]';
GO

CREATE CLUSTERED INDEX [IX_WorkflowProperties_RegistationDate]
    ON [dbo].[WorkflowProperties]([RegistrationDate] ASC);
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

PRINT N'Modificata chiave primaria nonclustered [PK_WorkflowRoles]';
GO

ALTER TABLE [dbo].[WorkflowRoles] DROP CONSTRAINT [PK_WorkflowRoles]
GO

ALTER TABLE [dbo].[WorkflowRoles] ADD CONSTRAINT [PK_WorkflowRoles] PRIMARY KEY NONCLUSTERED([IdWorkflowRole])
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

PRINT N'Aggiunto clustered indice [IX_WorkflowRoles_RegistationDate]';
GO

CREATE CLUSTERED INDEX [IX_WorkflowRoles_RegistationDate]
    ON [dbo].[WorkflowRoles]([RegistrationDate] ASC);
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

PRINT N'Modificata chiave primaria nonclustered [PK_WorkflowActivities]';
GO

ALTER TABLE [dbo].[WorkflowAuthorizations] DROP CONSTRAINT [FK_WorkflowAuthorizations_WorkflowActivities]
GO
ALTER TABLE [dbo].[WorkflowProperties] DROP CONSTRAINT [FK_WorkflowProperties_WorkflowActivities]
GO
ALTER TABLE [dbo].[WorkflowActivityLogs] DROP CONSTRAINT [FK_WorkflowActivityLogs_WorkflowActivities]
GO

ALTER TABLE [dbo].[WorkflowActivities] DROP CONSTRAINT [PK_WorkflowActivities]
GO
ALTER TABLE [dbo].[WorkflowActivities] ADD CONSTRAINT [PK_WorkflowActivities] PRIMARY KEY NONCLUSTERED([IdWorkflowActivity])
GO

ALTER TABLE [dbo].[WorkflowProperties]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowProperties_WorkflowActivities] FOREIGN KEY([IdWorkflowActivity])
REFERENCES [dbo].[WorkflowActivities] ([IdWorkflowActivity])
GO
ALTER TABLE [dbo].[WorkflowProperties] CHECK CONSTRAINT [FK_WorkflowProperties_WorkflowActivities]
GO

ALTER TABLE [dbo].[WorkflowAuthorizations]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowAuthorizations_WorkflowActivities] FOREIGN KEY([IdWorkflowActivity])
REFERENCES [dbo].[WorkflowActivities] ([IdWorkflowActivity])
GO
ALTER TABLE [dbo].[WorkflowAuthorizations] CHECK CONSTRAINT [FK_WorkflowAuthorizations_WorkflowActivities]
GO

ALTER TABLE [dbo].[WorkflowActivityLogs]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowActivityLogs_WorkflowActivities] FOREIGN KEY([IdWorkflowActivity])
REFERENCES [dbo].[WorkflowActivities] ([IdWorkflowActivity])
GO
ALTER TABLE [dbo].[WorkflowActivityLogs] CHECK CONSTRAINT [FK_WorkflowActivityLogs_WorkflowActivities]
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

PRINT N'Aggiunto clustered indice [IX_WorkflowActivities_RegistationDate]';
GO

CREATE CLUSTERED INDEX [IX_WorkflowActivities_RegistationDate]
    ON [dbo].[WorkflowActivities]([RegistrationDate] ASC);
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

PRINT N'Modificata chiave primaria nonclustered [PK_WorkflowInstances]';
GO

ALTER TABLE [dbo].[WorkflowActivities] DROP CONSTRAINT [FK_WorkflowActivities_WorkflowInstances]
GO
ALTER TABLE [dbo].[WorkflowProperties] DROP CONSTRAINT [FK_WorkflowProperties_WorkflowInstances]
GO


ALTER TABLE [dbo].[WorkflowInstances] DROP CONSTRAINT [PK_WorkflowInstance]
GO
ALTER TABLE [dbo].[WorkflowInstances] ADD CONSTRAINT [PK_WorkflowInstances] PRIMARY KEY NONCLUSTERED([IdWorkflowInstance])
GO

ALTER TABLE [dbo].[WorkflowActivities]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowActivities_WorkflowInstances] FOREIGN KEY([IdWorkflowInstance])
REFERENCES [dbo].[WorkflowInstances] ([IdWorkflowInstance])
GO
ALTER TABLE [dbo].[WorkflowActivities] CHECK CONSTRAINT [FK_WorkflowActivities_WorkflowInstances]
GO

ALTER TABLE [dbo].[WorkflowProperties]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowProperties_WorkflowInstances] FOREIGN KEY([IdWorkflowInstance])
REFERENCES [dbo].[WorkflowInstances] ([IdWorkflowInstance])
GO
ALTER TABLE [dbo].[WorkflowProperties] CHECK CONSTRAINT [FK_WorkflowProperties_WorkflowInstances]
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

PRINT N'Aggiunto clustered indice [IX_WorkflowInstances_RegistationDate]';
GO

CREATE CLUSTERED INDEX [IX_WorkflowInstances_RegistationDate]
    ON [dbo].[WorkflowInstances]([RegistrationDate] ASC);
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

PRINT N'Modificata chiave primaria nonclustered [PK_WorkflowRepositories]';
GO

ALTER TABLE [dbo].[WorkflowInstances] DROP CONSTRAINT [FK_WorkflowInstances_WorkflowRepositories]
GO

ALTER TABLE [dbo].[WorkflowRoles] DROP CONSTRAINT [FK_WorkflowRoles_WorkflowRepositories]
GO

ALTER TABLE [dbo].[OChartItemWorkflows] DROP CONSTRAINT [FK_OChartItemWorkflows_WorkflowRepositories]
GO

ALTER TABLE [dbo].[WorkflowRepositories] DROP CONSTRAINT [PK_WorkflowRepositories]
GO
ALTER TABLE [dbo].[WorkflowRepositories] ADD CONSTRAINT [PK_WorkflowRepositories] PRIMARY KEY NONCLUSTERED([IdWorkflowRepository])
GO

ALTER TABLE [dbo].[OChartItemWorkflows]  WITH CHECK ADD  CONSTRAINT [FK_OChartItemWorkflows_WorkflowRepositories] FOREIGN KEY([IdWorkflowRepository])
REFERENCES [dbo].[WorkflowRepositories] ([IdWorkflowRepository])
GO
ALTER TABLE [dbo].[OChartItemWorkflows] CHECK CONSTRAINT [FK_OChartItemWorkflows_WorkflowRepositories]
GO

ALTER TABLE [dbo].[WorkflowRoles]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowRoles_WorkflowRepositories] FOREIGN KEY([IdWorkflowRepository])
REFERENCES [dbo].[WorkflowRepositories] ([IdWorkflowRepository])
GO
ALTER TABLE [dbo].[WorkflowRoles] CHECK CONSTRAINT [FK_WorkflowRoles_WorkflowRepositories]
GO

ALTER TABLE [dbo].[WorkflowInstances]  WITH CHECK ADD  CONSTRAINT [FK_WorkflowInstances_WorkflowRepositories] FOREIGN KEY([IdWorkflowRepository])
REFERENCES [dbo].[WorkflowRepositories] ([IdWorkflowRepository])
GO
ALTER TABLE [dbo].[WorkflowInstances] CHECK CONSTRAINT [FK_WorkflowInstances_WorkflowRepositories]
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

PRINT N'Aggiunto clustered indice [IX_WorkflowRepositories_RegistationDate]';
GO

CREATE CLUSTERED INDEX [IX_WorkflowRepositories_RegistationDate]
    ON [dbo].[WorkflowRepositories]([RegistrationDate] ASC);
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

PRINT N'Creazione tabella UDSRepositories...';
GO

ALTER TABLE [uds].[UDSRepositories] ADD [DSWEnvironment] int NULL
GO

DECLARE @env AS INT
SET @env = 99
UPDATE [uds].[UDSRepositories] SET [DSWEnvironment] = @env,@env = @env+1
GO

ALTER TABLE [uds].[UDSRepositories] ALTER COLUMN [DSWEnvironment] int NOT NULL
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

PRINT N'Creazione tabella FasciclePeriods...';
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FasciclePeriods](
	[IdFasciclePeriod] [uniqueidentifier] NOT NULL,
	[PeriodName] [nvarchar](255) NOT NULL,
	[PeriodDays] [float] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) DEFAULT GETUTCDATE() NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_FasciclePeriods] PRIMARY KEY NONCLUSTERED 
(
	[IdFasciclePeriod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE CLUSTERED INDEX [IX_FasciclePeriods_RegistationDate]
    ON [dbo].[FasciclePeriods]([RegistrationDate] ASC);
GO

INSERT INTO [dbo].[FasciclePeriods]([IdFasciclePeriod], [PeriodName], [PeriodDays], [IsActive], [RegistrationUser], [RegistrationDate]) VALUES (NEWID(), 'Anno', 365, 1, 'docsuite', GETUTCDATE())
GO

INSERT INTO [dbo].[FasciclePeriods]([IdFasciclePeriod], [PeriodName], [PeriodDays], [IsActive], [RegistrationUser], [RegistrationDate]) VALUES (NEWID(), 'Semestre', 180, 1, 'docsuite', GETUTCDATE())
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

PRINT N'Creazione tabella CategoryFascicles...';
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CategoryFascicles](
	[IdCategoryFascicle] [uniqueidentifier] NOT NULL,
	[IdCategory] [smallint] NOT NULL,
	[IdFasciclePeriod] [uniqueidentifier] NULL,
	[FascicleType] [smallint] NOT NULL,
	[Manager] [int] NULL,
	[DSWEnvironment] [int] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) DEFAULT GETUTCDATE() NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_CategoryFascicles] PRIMARY KEY NONCLUSTERED 
(
	[IdCategoryFascicle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

CREATE CLUSTERED INDEX [IX_CategoryFascicles_RegistationDate]
    ON [dbo].[CategoryFascicles]([RegistrationDate] ASC);
GO

ALTER TABLE [dbo].[CategoryFascicles]  WITH CHECK ADD  CONSTRAINT [FK_CategoryFascicles_Category] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[Category] ([idCategory])
GO

ALTER TABLE [dbo].[CategoryFascicles] CHECK CONSTRAINT [FK_CategoryFascicles_Category]
GO

ALTER TABLE [dbo].[CategoryFascicles]  WITH CHECK ADD  CONSTRAINT [FK_CategoryFascicles_Contact] FOREIGN KEY([Manager])
REFERENCES [dbo].[Contact] ([Incremental])
GO

ALTER TABLE [dbo].[CategoryFascicles] CHECK CONSTRAINT [FK_CategoryFascicles_Contact]
GO

ALTER TABLE [dbo].[CategoryFascicles]  WITH CHECK ADD  CONSTRAINT [FK_CategoryFascicles_FasciclePeriods] FOREIGN KEY([IdFasciclePeriod])
REFERENCES [dbo].[FasciclePeriods] ([IdFasciclePeriod])
GO

ALTER TABLE [dbo].[CategoryFascicles] CHECK CONSTRAINT [FK_CategoryFascicles_FasciclePeriods]
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

PRINT N'Aggiunto nonclustered indice [IX_WorkflowProperties_IdWorkflowActivity_IdWorkflowInstance_Name] in WorkflowProperties';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_WorkflowProperties_IdWorkflowActivity_IdWorkflowInstance_Name] 
	ON [dbo].[WorkflowProperties]([IdWorkflowActivity] ASC, [IdWorkflowInstance] ASC, [Name] ASC)
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

PRINT N'Aggiunto nonclustered indice [IX_WorkflowAuthorizations_IdWorkflowActivity_Account] in WorkflowAuthorizations';
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_WorkflowAuthorizations_IdWorkflowActivity_Account] 
	ON [dbo].[WorkflowAuthorizations]([IdWorkflowActivity] ASC, [Account] ASC)
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

PRINT 'Creazione tabella [FascicleLogs]'
GO

CREATE TABLE [dbo].[FascicleLogs](
	[IdFascicleLog] [uniqueidentifier],
	[IdFascicle] [uniqueidentifier] NOT NULL,
	[LogDate] [DateTimeOffset](7) NOT NULL,
	[SystemComputer] [nvarchar](256) NOT NULL,
	[SystemUser] [nvarchar](256) NOT NULL,
	[LogType] [smallint] NOT NULL,
	[LogDescription] [nvarchar](MAX) NOT NULL,
	[Severity] [smallint] NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_FascicleLogs] PRIMARY KEY NONCLUSTERED 
(
	[IdFascicleLog] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FascicleLogs] WITH CHECK ADD CONSTRAINT [FK_FascicleLogs_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
GO
ALTER TABLE [dbo].[FascicleLogs] CHECK CONSTRAINT [FK_FascicleLogs_Fascicles]
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

PRINT 'Creazione tabella [FascicleLinks]'
GO

CREATE TABLE [dbo].[FascicleLinks](
	[IdFascicleLink] [uniqueidentifier],
	[IdFascicleParent] [uniqueidentifier] NOT NULL,
    [IdFascicleSon] [uniqueidentifier] NOT NULL,
	[RegistrationDate] [DateTimeOffset](7) DEFAULT GETUTCDATE() NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[Timestamp] [timestamp] NOT NULL,
	[FascicleLinkType] [smallint] NOT NULL,
 CONSTRAINT [PK_FascicleLinks] PRIMARY KEY NONCLUSTERED 
(
	[IdFascicleLink] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FascicleLinks] WITH CHECK ADD CONSTRAINT [FK_FascicleLinks_Fascicles_Parent] FOREIGN KEY([IdFascicleParent])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
GO
ALTER TABLE [dbo].[FascicleLinks] CHECK CONSTRAINT [FK_FascicleLinks_Fascicles_Parent]
GO

ALTER TABLE [dbo].[FascicleLinks] WITH CHECK ADD CONSTRAINT [FK_FascicleLinks_Fascicles_Son] FOREIGN KEY([IdFascicleSon])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
GO
ALTER TABLE [dbo].[FascicleLinks] CHECK CONSTRAINT [FK_FascicleLinks_Fascicles_Son]
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

PRINT 'Aggiunto unique index [IX_FascicleLinks_IdFascicleParent_IdFascicleSon] in FascicleLinks'

CREATE UNIQUE NONCLUSTERED INDEX [IX_FascicleLinks_IdFascicleParent_IdFascicleSon] 
	ON [dbo].[FascicleLinks]([IdFascicleParent] ASC, [IdFascicleSon] ASC)
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

PRINT N'Aggiunto clustered indice [IX_FascicleLinks_RegistationDate] in FascicleLinks';
GO

CREATE CLUSTERED INDEX [IX_FascicleLinks_RegistationDate]
    ON [dbo].[FascicleLinks]([RegistrationDate] ASC);
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

PRINT N'Modificata colonna [PublicationUser] in Collaboration';
GO

ALTER TABLE [dbo].[Collaboration] ALTER COLUMN [PublicationUser] nvarchar(256) null
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

PRINT N'Modificata colonna [RegistrationName] in Collaboration';
GO

ALTER TABLE [dbo].[Collaboration] ALTER COLUMN [RegistrationName] nvarchar(256) null
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

PRINT N'DROP VIEW [dbo].[ResolutionEmpty] / CREATE VIEW [dbo].[ResolutionEmpty]';
GO

DROP VIEW [dbo].[ResolutionEmpty]
GO
CREATE VIEW [dbo].[ResolutionEmpty] AS 
                    SELECT TOP 0 
                           CAST( NULL AS [int]) [idResolution],
                           CAST( NULL AS [uniqueidentifier]) [UniqueId],
                           CAST( NULL AS [tinyint]) [idType],
                           CAST( NULL AS [smallint]) [idContainer],
                           CAST( NULL AS [smallint]) [Year],
                           CAST( NULL AS [int]) [Number],
                           CAST( NULL AS [char](3)) [WorkflowType],
                           CAST( NULL AS [smallint]) [idStatus],
                           CAST( NULL AS [datetime]) [AdoptionDate],
                           CAST( NULL AS [nvarchar](256)) [AdoptionUser] ,
                           CAST( NULL AS [datetime]) [PublishingDate],
                           CAST( NULL AS [nvarchar](256)) [PublishingUser] ,
                           CAST( NULL AS [datetime]) [EffectivenessDate],
                           CAST( NULL AS [nvarchar](256)) [EffectivenessUser] ,
                           CAST( NULL AS [datetime]) [LeaveDate],
                           CAST( NULL AS [nvarchar](256)) [LeaveUser] ,
                           CAST( NULL AS [datetime]) [WarningDate],
                           CAST( NULL AS [datetime]) [ConfirmDate],
                           CAST( NULL AS [nvarchar](600)) [Object],
                           CAST( NULL AS [nvarchar](256)) [Note] ,
                           CAST( NULL AS [nvarchar](255)) [ServiceNumber],
                           CAST( NULL AS [smallint]) [idProposer],
                           CAST( NULL AS [datetime]) [LastChangedDate],
                           CAST( NULL AS [smallint]) [idManager],
                           CAST( NULL AS [smallint]) [idAssignee],
                           CAST( NULL AS [nvarchar](256)) [LastChangedUser] ,
                           CAST( NULL AS [nvarchar](256)) [LastChangedReason] ,
                           CAST( NULL AS [nvarchar](256)) [ControllerOpinion] ,
                           CAST( NULL AS [nvarchar](256)) [ConfirmUser] ,
                           CAST( NULL AS [nvarchar](256)) [WarningUser] ,
                           CAST( NULL AS [smallint]) [idLocation],
                           CAST( NULL AS [nvarchar](256)) [AlternativeProposer] ,
                           CAST( NULL AS [nvarchar](256)) [AlternativeManager] ,
                           CAST( NULL AS [nvarchar](256)) [AlternativeAssignee] ,
                           CAST( NULL AS [nvarchar](500)) [AlternativeRecipient],
                           CAST( NULL AS [nvarchar](256)) [FrontispieceObject] ,
                           CAST( NULL AS [smallint]) [idControllerStatus],
                           CAST( NULL AS [datetime]) [ProposeDate],
                           CAST( NULL AS [nvarchar](256)) [ProposeUser] ,
                           CAST( NULL AS [datetime]) [WaitDate],
                           CAST( NULL AS [nvarchar](256)) [WaitUser] ,
                           CAST( NULL AS [nvarchar](256)) [WarningProtocol] ,
                           CAST( NULL AS [nvarchar](256)) [ConfirmProtocol] ,
                           CAST( NULL AS [datetime]) [ResponseDate],
                           CAST( NULL AS [nvarchar](256)) [ResponseProtocol] ,
                           CAST( NULL AS [nvarchar](256)) [ResponseUser] ,
                           CAST( NULL AS [bit]) [Send],
                           CAST( NULL AS [nvarchar](256)) [Position] ,
                           CAST( NULL AS [datetime]) [ValidityDateFrom],
                           CAST( NULL AS [datetime]) [ValidityDateTo],
                           CAST( NULL AS [int]) [idBidType],
                           CAST( NULL AS [char](10)) [SupplierCode],
                           CAST( NULL AS [nvarchar](256)) [SupplierDescription] ,
                           CAST( NULL AS [smallint]) [idCategory],
                           CAST( NULL AS [smallint]) [idSubCategory],
                           CAST( NULL AS [char](1)) [CheckPublication],
                           CAST( NULL AS [char](1)) [ConservationStatus],
                           CAST( NULL AS [datetime]) [LastConservationDate],
                           CAST( NULL AS [bit]) [HasConservatedDocs],
                           CAST( NULL AS [int]) [WebState],
                           CAST( NULL AS [datetime]) [WebPublicationDate],
                           CAST( NULL AS [datetime]) [WebRevokeDate],
                           CAST( NULL AS [nvarchar](256)) [WebSPGuid] ,
                           CAST( NULL AS [bit]) [IsChecked],
                           CAST( NULL AS [nvarchar](256)) [WebPublicatedDocuments] ,
                           CAST( NULL AS [bit]) [ImmediatelyExecutive],
                           CAST( NULL AS [nvarchar](256)) [InclusiveNumber] ,
                           CAST( NULL AS [int]) [ProposerProtocolCollaboration],
                           CAST( NULL AS [int]) [SupervisoryBoardProtocolCollaboration],
                           CAST( NULL AS [smallint]) [AdoptionRoleId],
                           CAST( NULL AS [bit]) [OCSupervisoryBoard],
                           CAST( NULL AS [bit]) [OCRegion],
                           CAST( NULL AS [bit]) [OCManagement],
                           CAST( NULL AS [bit]) [OCOther],
                           CAST( NULL AS [bit]) [OCCorteConti],
                           CAST( NULL AS [nvarchar](256)) [OtherDescription] ,
                           CAST( NULL AS [datetime]) [ProposerWarningDate],
                           CAST( NULL AS [nvarchar](256)) [ProposerWarningUser] ,
                           CAST( NULL AS [nvarchar](256)) [ProposerProtocolLink] ,
                           CAST( NULL AS [nvarchar](256)) [PublishingProtocolLink] ,
                           CAST( NULL AS [nvarchar](256)) [RegionProtocolLink] ,
                           CAST( NULL AS [nvarchar](256)) [EffectivenessProtocolLink] ,
                           CAST( NULL AS [nvarchar](256)) [SupervisoryBoardProtocolLink] ,
                           CAST( NULL AS [nvarchar](256)) [SupervisoryBoardOpinion] ,
                           CAST( NULL AS [datetime]) [SupervisoryBoardWarningDate],
                           CAST( NULL AS [nvarchar](256)) [SupervisoryBoardWarningUser] ,
                           CAST( NULL AS [datetime]) [CorteDeiContiWarningDate],
                           CAST( NULL AS [nvarchar](256)) [CorteDeiContiProtocolLink] ,
                           CAST( NULL AS [nvarchar](256)) [DGR] ,
                           CAST( NULL AS [datetime]) [ManagementWarningDate],
                           CAST( NULL AS [nvarchar](256)) [ManagementWarningUser] ,
                           CAST( NULL AS [nvarchar](256)) [ManagementProtocolLink] ,
                           CAST( NULL AS [nvarchar](1500)) [ObjectPrivacy],
                           CAST( NULL AS [nvarchar](256)) [DeclineNote] ,
                           CAST( NULL AS [nvarchar](256)) [ApprovalNote]

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