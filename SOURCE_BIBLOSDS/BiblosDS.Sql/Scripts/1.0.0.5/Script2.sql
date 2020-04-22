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
ALTER TABLE dbo.Preservation SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Archive SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE ext.Customer SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE ext.PreservationVerify
	(
	IdPreservationVerify uniqueidentifier NOT NULL,
	IdCustomer varchar(255) NULL,
	IdArchive uniqueidentifier NULL,
	IdPreservation uniqueidentifier NULL,
	DataInizio datetime NULL,
	DataFine datetime NULL,
	PathVerifica varchar(500) NULL,
	Warning text NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE ext.PreservationVerify ADD CONSTRAINT
	PK_PreservationVerify PRIMARY KEY CLUSTERED 
	(
	IdPreservationVerify
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE ext.PreservationVerify ADD CONSTRAINT
	FK_PreservationVerify_Customer FOREIGN KEY
	(
	IdCustomer
	) REFERENCES ext.Customer
	(
	IdCustomer
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE ext.PreservationVerify ADD CONSTRAINT
	FK_PreservationVerify_Archive FOREIGN KEY
	(
	IdArchive
	) REFERENCES dbo.Archive
	(
	IdArchive
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE ext.PreservationVerify ADD CONSTRAINT
	FK_PreservationVerify_Preservation FOREIGN KEY
	(
	IdPreservation
	) REFERENCES dbo.Preservation
	(
	IdPreservation
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE ext.PreservationVerify SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
