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
	SELECT ASMN_SECURITY_GROUP.[GroupName]
      ,ASMN_SECURITY_GROUP.[FullIncrementalPath]
      ,ASMN_SECURITY_GROUP.[RegistrationUser]
      ,ASMN_SECURITY_GROUP.[idGroupFather]
      ,ASMN_SECURITY_GROUP.[LogDescription]
      ,ASMN_SECURITY_GROUP.[RegistrationDate]
      ,ASMN_SECURITY_GROUP.[LastChangedUser]
      ,ASMN_SECURITY_GROUP.[LastChangedDate]
      ,ASMN_SECURITY_GROUP.[UniqueId]
      ,ASMN_SECURITY_GROUP.[AllUsers]
      ,ASMN_SECURITY_GROUP.[TenantId]
      ,ASMN_SECURITY_GROUP.[IdSecurityGroupTenant]
	FROM [ASMN_Protocollo].dbo.[SecurityGroups] ASMN_SECURITY_GROUP
	LEFT JOIN [AUSLRE_Protocollo].dbo.[SecurityGroups] AUSLRE_SECURITY_GROUP ON ASMN_SECURITY_GROUP.UniqueId = AUSLRE_SECURITY_GROUP.UniqueId
	WHERE 
		ASMN_SECURITY_GROUP.TenantId = '1F761813-6086-44F1-A507-99954EA64342'
		AND AUSLRE_SECURITY_GROUP.UniqueId IS NULL
	ORDER BY ASMN_SECURITY_GROUP.FullIncrementalPath

OPEN security_group_cursor
FETCH NEXT FROM security_group_cursor INTO @GroupName, @FullIncrementalPath, @RegistrationUser, @idGroupFather, @LogDescription, @RegistrationDate,
										   @LastChangedUser, @LastChangedDate, @UniqueId, @AllUsers, @TenantId, @IdSecurityGroupTenant

WHILE @@FETCH_STATUS = 0
BEGIN
 SET @FK_SecurityGroupFather = null
 SELECT TOP 1 @FK_SecurityGroupFather = idGroup FROM [AUSLRE_Protocollo].[dbo].[SecurityGroups] WHERE TenantId = '1F761813-6086-44F1-A507-99954EA64342' AND IdSecurityGroupTenant = @idGroupFather

 INSERT INTO [AUSLRE_Protocollo].[dbo].[SecurityGroups]
           ([idGroup],[GroupName],[FullIncrementalPath],[RegistrationUser],[idGroupFather],[LogDescription]
			,[RegistrationDate],[LastChangedUser],[LastChangedDate],[UniqueId],[AllUsers],[TenantId]
			,[IdSecurityGroupTenant]) 
		   VALUES((select max([idGroup]) from [AUSLRE_Protocollo].[dbo].[SecurityGroups])+ 1,@GroupName, @FullIncrementalPath, @RegistrationUser, @FK_SecurityGroupFather, @LogDescription, @RegistrationDate,
										   @LastChangedUser, @LastChangedDate, @UniqueId, @AllUsers, @TenantId, @IdSecurityGroupTenant);
 
 INSERT INTO [AUSLRE_Pratiche].[dbo].[SecurityGroups]
           ([idGroup],[GroupName],[FullIncrementalPath],[RegistrationUser],[idGroupFather],[LogDescription]
			,[RegistrationDate],[LastChangedUser],[LastChangedDate],[UniqueId],[AllUsers],[TenantId]
			,[IdSecurityGroupTenant]) 
		   VALUES((select max([idGroup]) from [AUSLRE_Pratiche].[dbo].[SecurityGroups])+ 1,@GroupName, @FullIncrementalPath, @RegistrationUser, @FK_SecurityGroupFather, @LogDescription, @RegistrationDate,
										   @LastChangedUser, @LastChangedDate, @UniqueId, @AllUsers, @TenantId, @IdSecurityGroupTenant);

 INSERT INTO [AUSLRE_Atti].[dbo].[SecurityGroups]
           ([idGroup],[GroupName],[FullIncrementalPath],[RegistrationUser],[idGroupFather],[LogDescription]
			,[RegistrationDate],[LastChangedUser],[LastChangedDate],[UniqueId],[AllUsers],[TenantId]
			,[IdSecurityGroupTenant]) 
		   VALUES((select max([idGroup]) from [AUSLRE_Atti].[dbo].[SecurityGroups])+ 1,@GroupName, @FullIncrementalPath, @RegistrationUser, @FK_SecurityGroupFather, @LogDescription, @RegistrationDate,
										   @LastChangedUser, @LastChangedDate, @UniqueId, @AllUsers, @TenantId, @IdSecurityGroupTenant);


 FETCH NEXT FROM security_group_cursor INTO @GroupName, @FullIncrementalPath, @RegistrationUser, @idGroupFather, @LogDescription, @RegistrationDate,
										   @LastChangedUser, @LastChangedDate, @UniqueId, @AllUsers, @TenantId, @IdSecurityGroupTenant

END 
CLOSE security_group_cursor
DEALLOCATE security_group_cursor
