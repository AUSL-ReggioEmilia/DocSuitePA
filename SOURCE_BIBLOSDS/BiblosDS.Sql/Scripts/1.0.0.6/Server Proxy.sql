-- =============================================
-- Script Template
-- =============================================

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Server
	(
	IdServer uniqueidentifier NOT NULL,
	ServerName varchar(250) NULL,
	ServerRole varchar(50) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Server ADD CONSTRAINT
	PK_Server PRIMARY KEY CLUSTERED 
	(
	IdServer
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Server SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.ServerRelation
	(
	IdServerRelation uniqueidentifier NOT NULL,
	IdServerSource uniqueidentifier NOT NULL,
	IdServerDestination uniqueidentifier NOT NULL,
	Enabled bit NULL,
	DateCreated datetime NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.ServerRelation ADD CONSTRAINT
	PK_ServerRelation PRIMARY KEY CLUSTERED 
	(
	IdServerRelation
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.ServerRelation ADD CONSTRAINT
	FK_ServerRelation_Server FOREIGN KEY
	(
	IdServerSource
	) REFERENCES dbo.Server
	(
	IdServer
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ServerRelation ADD CONSTRAINT
	FK_ServerRelation_Server1 FOREIGN KEY
	(
	IdServerDestination
	) REFERENCES dbo.Server
	(
	IdServer
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ServerRelation SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.[Document] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.DocumentServer
	(
	IdDocument uniqueidentifier NOT NULL,
	IdServer uniqueidentifier NOT NULL,
	IdStorage uniqueidentifier NULL,
	IdStorageArea uniqueidentifier NULL,
	DateCreated datetime NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.DocumentServer ADD CONSTRAINT
	PK_DocumentServer PRIMARY KEY CLUSTERED 
	(
	IdDocument,
	IdServer
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.DocumentServer ADD CONSTRAINT
	FK_DocumentServer_Document FOREIGN KEY
	(
	IdDocument
	) REFERENCES dbo.[Document]
	(
	IdDocument
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.DocumentServer ADD CONSTRAINT
	FK_DocumentServer_Server FOREIGN KEY
	(
	IdServer
	) REFERENCES dbo.Server
	(
	IdServer
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.DocumentServer SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Storage ADD
	IdServer uniqueidentifier NULL
GO
ALTER TABLE dbo.Storage ADD CONSTRAINT
	FK_Storage_Server FOREIGN KEY
	(
	IdServer
	) REFERENCES dbo.Server
	(
	IdServer
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Storage SET (LOCK_ESCALATION = TABLE)
GO
COMMIT

