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
ALTER TABLE dbo.[Document]
	DROP CONSTRAINT FK_Document_Preservation
GO
ALTER TABLE dbo.Preservation SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.[Document]
	DROP CONSTRAINT FK_Document_PreservationException
GO
ALTER TABLE dbo.PreservationException SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.[Document]
	DROP CONSTRAINT FK_Document_DocumentNodeType
GO
ALTER TABLE dbo.DocumentNodeType SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.[Document]
	DROP CONSTRAINT FK_Document_CertificateStore
GO
ALTER TABLE dbo.CertificateStore SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.[Document]
	DROP CONSTRAINT FK_Document_DocumentStatus
GO
ALTER TABLE dbo.DocumentStatus SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.[Document]
	DROP CONSTRAINT FK_Document_Storage
GO
ALTER TABLE dbo.Storage SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.[Document]
	DROP CONSTRAINT FK_Document_StorageArea
GO
ALTER TABLE dbo.StorageArea SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.[Document]
	DROP CONSTRAINT FK_Document_Archive
GO
ALTER TABLE dbo.Archive SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER VIEW [dbo]._dta_mv_0   AS SELECT  [dbo].[Document].[IdParentBiblos] as _col_1,  [dbo].[Document].[IdArchive] as _col_2,  count_big(*) as _col_3 FROM  [dbo].[Document]   GROUP BY  [dbo].[Document].[IdParentBiblos],  [dbo].[Document].[IdArchive] GO
ALTER TABLE dbo.[Document]
	DROP CONSTRAINT DF_Document_IsLatestVersion
GO
CREATE TABLE dbo.Tmp_Document
	(
	IdBiblos int NOT NULL,
	IdDocument uniqueidentifier NOT NULL,
	IdParentBiblos uniqueidentifier NULL,
	IdStorageArea uniqueidentifier NULL,
	IdStorage uniqueidentifier NULL,
	IdArchive uniqueidentifier NOT NULL,
	ChainOrder int NOT NULL,
	StorageVersion decimal(18, 6) NULL,
	Version decimal(18, 6) NOT NULL,
	IdParentVersion uniqueidentifier NULL,
	IdDocumentLink uniqueidentifier NULL,
	IdCertificate uniqueidentifier NULL,
	SignHeader varchar(1000) NULL,
	FullSign varchar(1000) NULL,
	DocumentHash varchar(1000) NULL,
	IsLinked smallint NULL,
	IsVisible smallint NOT NULL,
	IsConservated smallint NULL,
	DateExpire datetime NULL,
	DateCreated datetime NULL,
	Name varchar(255) NULL,
	Size bigint NULL,
	IdNodeType smallint NULL,
	IsConfirmed smallint NULL,
	IdDocumentStatus smallint NOT NULL,
	IsCheckOut smallint NOT NULL,
	DateMain datetime NULL,
	IdPreservation uniqueidentifier NULL,
	IsDetached bit NULL,
	IdUserCheckOut varchar(250) NULL,
	PrimaryKeyValue varchar(250) NULL,
	PrimaryKeyValueIndex  AS (case when [PrimaryKeyValue] IS NULL OR [PrimaryKeyValue]='' then CONVERT([nvarchar](250),[IdDocument],(0)) else [PrimaryKeyValue] end),
	IdPreservationException uniqueidentifier NULL,
	PreservationIndex bigint NULL,
	IdThumbnail varchar(250) NULL,
	IdPdf varchar(250) NULL,
	PreservationName varchar(250) NULL,
	IsLatestVersion bit NOT NULL,
	IdArchiveCertificate uniqueidentifier NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Document SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_Document ADD CONSTRAINT
	DF_Document_IsLatestVersion DEFAULT ((0)) FOR IsLatestVersion
GO
IF EXISTS(SELECT * FROM dbo.[Document])
	 EXEC('INSERT INTO dbo.Tmp_Document (IdBiblos, IdDocument, IdParentBiblos, IdStorageArea, IdStorage, IdArchive, ChainOrder, StorageVersion, Version, IdParentVersion, IdDocumentLink, IdCertificate, SignHeader, FullSign, DocumentHash, IsLinked, IsVisible, IsConservated, DateExpire, DateCreated, Name, Size, IdNodeType, IsConfirmed, IdDocumentStatus, IsCheckOut, DateMain, IdPreservation, IsDetached, IdUserCheckOut, PrimaryKeyValue, IdPreservationException, PreservationIndex, IdThumbnail, IdPdf, PreservationName, IsLatestVersion, IdArchiveCertificate)
		SELECT IdBiblos, IdDocument, IdParentBiblos, IdStorageArea, IdStorage, IdArchive, ChainOrder, StorageVersion, Version, IdParentVersion, IdDocumentLink, IdCertificate, SignHeader, FullSign, DocumentHash, IsLinked, IsVisible, IsConservated, DateExpire, DateCreated, Name, Size, IdNodeType, IsConfirmed, IdDocumentStatus, IsCheckOut, DateMain, IdPreservation, IsDetached, IdUserCheckOut, PrimaryKeyValue, IdPreservationException, PreservationIndex, IdThumbnail, IdPdf, PreservationName, IsLatestVersion, IdArchiveCertificate FROM dbo.[Document] WITH (HOLDLOCK TABLOCKX)')
GO
ALTER TABLE dbo.DocumentAttach
	DROP CONSTRAINT FK_DocumentAttach_Document
GO
ALTER TABLE dbo.DocumentAttach
	DROP CONSTRAINT FK_DocumentAttach_Document1
GO
ALTER TABLE dbo.[Document]
	DROP CONSTRAINT FK_Document_Document
GO
ALTER TABLE dbo.[Document]
	DROP CONSTRAINT FK_Document_Document1
GO
ALTER TABLE dbo.[Document]
	DROP CONSTRAINT FK_Document_DocumentParentVersion
GO
ALTER TABLE dbo.Transito
	DROP CONSTRAINT FK_Transito_Document
GO
ALTER TABLE dbo.Permission
	DROP CONSTRAINT FK_Permission_Document
GO
ALTER TABLE dbo.Cache
	DROP CONSTRAINT FK_Cache_Document
GO
ALTER TABLE dbo.AttributesValue
	DROP CONSTRAINT FK_AttributesValue_Document
GO
ALTER TABLE dbo.DocumentServer
	DROP CONSTRAINT FK_DocumentServer_Document
GO
ALTER TABLE dbo.DocumentConnection
	DROP CONSTRAINT FK_DocumentConnection_Document
GO
ALTER TABLE dbo.DocumentConnection
	DROP CONSTRAINT FK_DocumentConnection_Document1
GO
ALTER TABLE dbo.Transit
	DROP CONSTRAINT FK_Transit_Document
GO
DROP TABLE dbo.[Document]
GO
EXECUTE sp_rename N'dbo.Tmp_Document', N'Document', 'OBJECT' 
GO
ALTER TABLE dbo.[Document] ADD CONSTRAINT
	PK_Document PRIMARY KEY CLUSTERED 
	(
	IdDocument
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_PrimaryKeyValue ON dbo.[Document]
	(
	IdArchive,
	PrimaryKeyValue
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_Archive_Storage ON dbo.[Document]
	(
	IdStorage,
	IdArchive
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_IdArchive ON dbo.[Document]
	(
	IdArchive
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_IdBiblos_IdArchive ON dbo.[Document]
	(
	IdBiblos,
	IdArchive
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX _dta_index_Document_K3 ON dbo.[Document]
	(
	IdParentBiblos,
	IdDocument,
	IdArchive,
	IdParentVersion,
	IdStorageArea,
	IdStorage
	) INCLUDE (IdBiblos, ChainOrder, StorageVersion, Version, IdDocumentLink, IdCertificate, SignHeader, FullSign, DocumentHash, IsLinked, IsVisible, IsConservated, DateExpire, DateCreated, Name, Size, IdNodeType, IsConfirmed, IdDocumentStatus, IsCheckOut, DateMain, IdPreservation, IsDetached, IdUserCheckOut, PrimaryKeyValue, IdPreservationException, PreservationIndex, IdThumbnail, IdPdf) 
 WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.[Document] ADD CONSTRAINT
	FK_Document_Archive FOREIGN KEY
	(
	IdArchive
	) REFERENCES dbo.Archive
	(
	IdArchive
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[Document] ADD CONSTRAINT
	FK_Document_StorageArea FOREIGN KEY
	(
	IdStorageArea
	) REFERENCES dbo.StorageArea
	(
	IdStorageArea
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[Document] ADD CONSTRAINT
	FK_Document_Document FOREIGN KEY
	(
	IdParentBiblos
	) REFERENCES dbo.[Document]
	(
	IdDocument
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[Document] ADD CONSTRAINT
	FK_Document_Storage FOREIGN KEY
	(
	IdStorage
	) REFERENCES dbo.Storage
	(
	IdStorage
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[Document] ADD CONSTRAINT
	FK_Document_Document1 FOREIGN KEY
	(
	IdDocumentLink
	) REFERENCES dbo.[Document]
	(
	IdDocument
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[Document] ADD CONSTRAINT
	FK_Document_DocumentStatus FOREIGN KEY
	(
	IdDocumentStatus
	) REFERENCES dbo.DocumentStatus
	(
	IdDocumentStatus
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[Document] ADD CONSTRAINT
	FK_Document_CertificateStore FOREIGN KEY
	(
	IdCertificate
	) REFERENCES dbo.CertificateStore
	(
	IdCertificate
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[Document] ADD CONSTRAINT
	FK_Document_DocumentNodeType FOREIGN KEY
	(
	IdNodeType
	) REFERENCES dbo.DocumentNodeType
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[Document] ADD CONSTRAINT
	FK_Document_DocumentParentVersion FOREIGN KEY
	(
	IdParentVersion
	) REFERENCES dbo.[Document]
	(
	IdDocument
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[Document] ADD CONSTRAINT
	FK_Document_PreservationException FOREIGN KEY
	(
	IdPreservationException
	) REFERENCES dbo.PreservationException
	(
	IdPreservationException
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[Document] ADD CONSTRAINT
	FK_Document_Preservation FOREIGN KEY
	(
	IdPreservation
	) REFERENCES dbo.Preservation
	(
	IdPreservation
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Transit ADD CONSTRAINT
	FK_Transit_Document FOREIGN KEY
	(
	IdDocument
	) REFERENCES dbo.[Document]
	(
	IdDocument
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Transit SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.DocumentConnection ADD CONSTRAINT
	FK_DocumentConnection_Document FOREIGN KEY
	(
	IdDocument
	) REFERENCES dbo.[Document]
	(
	IdDocument
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.DocumentConnection ADD CONSTRAINT
	FK_DocumentConnection_Document1 FOREIGN KEY
	(
	IdDocumentConnection
	) REFERENCES dbo.[Document]
	(
	IdDocument
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.DocumentConnection SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
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
ALTER TABLE dbo.DocumentServer SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.AttributesValue ADD CONSTRAINT
	FK_AttributesValue_Document FOREIGN KEY
	(
	IdDocument
	) REFERENCES dbo.[Document]
	(
	IdDocument
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.AttributesValue SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Cache ADD CONSTRAINT
	FK_Cache_Document FOREIGN KEY
	(
	IdDocument
	) REFERENCES dbo.[Document]
	(
	IdDocument
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Cache SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Permission ADD CONSTRAINT
	FK_Permission_Document FOREIGN KEY
	(
	IdDocument
	) REFERENCES dbo.[Document]
	(
	IdDocument
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Permission SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Transito ADD CONSTRAINT
	FK_Transito_Document FOREIGN KEY
	(
	IdDocument
	) REFERENCES dbo.[Document]
	(
	IdDocument
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.Transito SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.DocumentAttach ADD CONSTRAINT
	FK_DocumentAttach_Document FOREIGN KEY
	(
	IdDocument
	) REFERENCES dbo.[Document]
	(
	IdDocument
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.DocumentAttach ADD CONSTRAINT
	FK_DocumentAttach_Document1 FOREIGN KEY
	(
	IdDocument
	) REFERENCES dbo.[Document]
	(
	IdDocument
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.DocumentAttach SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
