-- =============================================
-- Script Template
-- =============================================
create PROCEDURE [InizializeBiblosDS]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert StorageTypes 
	
	INSERT INTO StorageType (Type, IdStorageType, StorageAssembly, StorageClassName)
	VALUES ('Window Azure','9b8abf7e-0cc3-42bf-a9b6-1e9d60abe205','BiblosDS.Library.Storage.Azure','AzureStorage')		
	INSERT INTO StorageType (Type, IdStorageType, StorageAssembly, StorageClassName)
	VALUES ('SharePoint2010DS','c90c3c96-4887-4128-8129-26f19571dd00','BiblosDS.Library.Storage.SharePoint2010DS','SharePointStorage2010DS')
	INSERT INTO StorageType (Type, IdStorageType, StorageAssembly, StorageClassName)
	VALUES ('SharePoint','c90c3c96-4887-4128-8129-26f19571dd87','BiblosDS.Library.Storage.SharePoint','SharePointStorage')
	INSERT INTO StorageType (Type, IdStorageType, StorageAssembly, StorageClassName)
	VALUES ('SharePoint2010','c90c3c96-4887-4128-8129-26f19571dd88','BiblosDS.Library.Storage.SharePoint2010','SharePointStorage2010')
	INSERT INTO StorageType (Type, IdStorageType, StorageAssembly, StorageClassName)
	VALUES ('Ftp','1f80a15a-e29a-4fe2-bae4-40015a2e0900','BiblosDS.Library.Storage.Ftp','FtpStorage')
	INSERT INTO StorageType (Type, IdStorageType, StorageAssembly, StorageClassName)
	VALUES ('SqlServer','74800da8-13d0-4f79-b8df-484967a0bc40','BiblosDS.Library.Storage.SQL','SQL2008Storage')
	INSERT INTO StorageType (Type, IdStorageType, StorageAssembly, StorageClassName)
	VALUES ('LegacyBiblosDS','1f80a15a-e29a-4fe2-bae4-4cc15a2e097e','BiblosDS.Library.Storage.FileSystem','FileSystem')
	INSERT INTO StorageType (Type, IdStorageType, StorageAssembly, StorageClassName)
	VALUES ('BDSCom','abae5490-a430-4301-b835-ae0a939fffd2','BiblosDS.Library.Storage.BDSCom','BDSComStorage')

	INSERT INTO AttributesMode (IdMode, Description) 
		VALUES (0, 'ReadOnly after insert') 

	INSERT INTO AttributesMode (IdMode, Description) 
		VALUES (1, 'Modify if Empty') 

	INSERT INTO AttributesMode (IdMode, Description) 
		VALUES (2, 'Modify if not Archived')
		
	INSERT INTO AttributesMode (IdMode, Description) 
		VALUES (3, 'Modify Always')
				 
  insert into dbo.DocumentStatus(IdDocumentStatus, Description) values(1, 'Undefined')
  insert into dbo.DocumentStatus(IdDocumentStatus, Description) values(2, 'In Transito')
  insert into dbo.DocumentStatus(IdDocumentStatus, Description) values(3, 'In Storage')
  insert into dbo.DocumentStatus(IdDocumentStatus, Description) values(4, 'In Cache')
  insert into dbo.DocumentStatus(IdDocumentStatus, Description) values(5, 'Profile')
  
  
  insert into dbo.PermissionMode(IdMode, [Description]) values(-1, 'Full contrl')
  insert into dbo.PermissionMode(IdMode, [Description]) values(1, 'Read')
  insert into dbo.PermissionMode(IdMode, [Description]) values(2, 'Write')
  insert into dbo.PermissionMode(IdMode, [Description]) values(3, 'Modify')
  
  insert CertificateStore( IdCertificate, Name, IsDefault) values(NEWID(), 'BiblosDS', 1)
  
  
  INSERT INTO [RuleOperator]([IdRuleOperator],[Descrizione]) VALUES(0 ,'Is Equal')
  INSERT INTO [RuleOperator]([IdRuleOperator],[Descrizione]) VALUES(1 ,'Is Greather Than')
  INSERT INTO [RuleOperator]([IdRuleOperator],[Descrizione]) VALUES(2 ,'Is Greather Or Equal Than')
  INSERT INTO [RuleOperator]([IdRuleOperator],[Descrizione]) VALUES(3 ,'Is Less Than')
  INSERT INTO [RuleOperator]([IdRuleOperator],[Descrizione]) VALUES(4 ,'Is Less Or Equal Than')   	
 	
END

