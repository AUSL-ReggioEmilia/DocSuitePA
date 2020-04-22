DECLARE @GroupName varchar(255)
DECLARE @FullIncrementalPath varchar(255)
DECLARE @RegistrationUser varchar(255)
DECLARE @idGroupFather int
DECLARE @LogDescription varchar(255)
DECLARE @RegistrationDate datetimeoffset(7)
DECLARE @LastChangedUser varchar(255)
DECLARE @LastChangedDate datetimeoffset(7)
DECLARE @UniqueId uniqueidentifier
DECLARE @AllUsers bit
DECLARE @TenantId uniqueidentifier
DECLARE @IdSecurityGroupTenant int
DECLARE @FK_SecurityGroupFather int

DECLARE security_group_cursor CURSOR FOR
	SELECT AUSLRE_SECURITY_GROUP.[GroupName]
      ,AUSLRE_SECURITY_GROUP.[FullIncrementalPath]
      ,AUSLRE_SECURITY_GROUP.[RegistrationUser]
      ,AUSLRE_SECURITY_GROUP.[idGroupFather]
      ,AUSLRE_SECURITY_GROUP.[LogDescription]
      ,AUSLRE_SECURITY_GROUP.[RegistrationDate]
      ,AUSLRE_SECURITY_GROUP.[LastChangedUser]
      ,AUSLRE_SECURITY_GROUP.[LastChangedDate]
      ,AUSLRE_SECURITY_GROUP.[UniqueId]
      ,AUSLRE_SECURITY_GROUP.[AllUsers]
      ,AUSLRE_SECURITY_GROUP.[TenantId]
      ,AUSLRE_SECURITY_GROUP.[IdSecurityGroupTenant]
	FROM [AUSLRE_Protocollo].dbo.[SecurityGroups] AUSLRE_SECURITY_GROUP
	LEFT JOIN [ASMN_Protocollo].dbo.[SecurityGroups] ASMN_SECURITY_GROUP ON AUSLRE_SECURITY_GROUP.UniqueId = ASMN_SECURITY_GROUP.UniqueId
	WHERE 
		AUSLRE_SECURITY_GROUP.TenantId = '1F761813-6086-44F1-A507-99954EA64343'
		AND ASMN_SECURITY_GROUP.UniqueId IS NULL
	ORDER BY AUSLRE_SECURITY_GROUP.FullIncrementalPath

OPEN security_group_cursor
FETCH NEXT FROM security_group_cursor INTO @GroupName, @FullIncrementalPath, @RegistrationUser, @idGroupFather, @LogDescription, @RegistrationDate,
										   @LastChangedUser, @LastChangedDate, @UniqueId, @AllUsers, @TenantId, @IdSecurityGroupTenant

WHILE @@FETCH_STATUS = 0
BEGIN
 SET @FK_SecurityGroupFather = null
 SELECT TOP 1 @FK_SecurityGroupFather = idGroup FROM [ASMN_Protocollo].[dbo].[SecurityGroups] WHERE TenantId = '1F761813-6086-44F1-A507-99954EA64343' AND IdSecurityGroupTenant = @idGroupFather

 INSERT INTO [ASMN_Protocollo].[dbo].[SecurityGroups]
           ([idGroup],[GroupName],[FullIncrementalPath],[RegistrationUser],[idGroupFather],[LogDescription]
			,[RegistrationDate],[LastChangedUser],[LastChangedDate],[UniqueId],[AllUsers],[TenantId]
			,[IdSecurityGroupTenant]) 
		   VALUES((select max([idGroup]) from [ASMN_Protocollo].[dbo].[SecurityGroups])+ 1,@GroupName, @FullIncrementalPath, @RegistrationUser, @FK_SecurityGroupFather, @LogDescription, @RegistrationDate,
										   @LastChangedUser, @LastChangedDate, @UniqueId, @AllUsers, @TenantId, @IdSecurityGroupTenant);
 
 INSERT INTO [ASMN_Pratiche].[dbo].[SecurityGroups]
           ([idGroup],[GroupName],[FullIncrementalPath],[RegistrationUser],[idGroupFather],[LogDescription]
			,[RegistrationDate],[LastChangedUser],[LastChangedDate],[UniqueId],[AllUsers],[TenantId]
			,[IdSecurityGroupTenant]) 
		   VALUES((select max([idGroup]) from [ASMN_Pratiche].[dbo].[SecurityGroups])+ 1,@GroupName, @FullIncrementalPath, @RegistrationUser, @FK_SecurityGroupFather, @LogDescription, @RegistrationDate,
										   @LastChangedUser, @LastChangedDate, @UniqueId, @AllUsers, @TenantId, @IdSecurityGroupTenant);

 INSERT INTO [ASMN_Atti].[dbo].[SecurityGroups]
           ([idGroup],[GroupName],[FullIncrementalPath],[RegistrationUser],[idGroupFather],[LogDescription]
			,[RegistrationDate],[LastChangedUser],[LastChangedDate],[UniqueId],[AllUsers],[TenantId]
			,[IdSecurityGroupTenant]) 
		   VALUES((select max([idGroup]) from [ASMN_Atti].[dbo].[SecurityGroups])+ 1,@GroupName, @FullIncrementalPath, @RegistrationUser, @FK_SecurityGroupFather, @LogDescription, @RegistrationDate,
										   @LastChangedUser, @LastChangedDate, @UniqueId, @AllUsers, @TenantId, @IdSecurityGroupTenant);


 FETCH NEXT FROM security_group_cursor INTO @GroupName, @FullIncrementalPath, @RegistrationUser, @idGroupFather, @LogDescription, @RegistrationDate,
										   @LastChangedUser, @LastChangedDate, @UniqueId, @AllUsers, @TenantId, @IdSecurityGroupTenant

END 
CLOSE security_group_cursor
DEALLOCATE security_group_cursor
