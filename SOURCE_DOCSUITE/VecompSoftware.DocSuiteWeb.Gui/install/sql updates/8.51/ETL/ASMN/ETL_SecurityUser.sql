DECLARE @Account nvarchar(max)
DECLARE @Description nvarchar(max)
DECLARE @idGroup int
DECLARE @RegistrationDate datetimeoffset(7)
DEClARE @LastChangedUser varchar(255)
DECLARE @LastChangedDate datetimeoffset(7)
DECLARE @RegistrationUser varchar(255)
DECLARE @UserDomain nvarchar(256)
DECLARE @UniqueId uniqueidentifier
DECLARE @FK_SecurityGroup int


DECLARE security_user_cursor CURSOR FOR
	SELECT AUSLRE_SECURITY_USER.[Account]
      ,AUSLRE_SECURITY_USER.[Description]
      ,AUSLRE_SECURITY_USER.[idGroup]
      ,AUSLRE_SECURITY_USER.[RegistrationDate]
      ,AUSLRE_SECURITY_USER.[LastChangedUser]
      ,AUSLRE_SECURITY_USER.[LastChangedDate]
      ,AUSLRE_SECURITY_USER.[RegistrationUser]
      ,AUSLRE_SECURITY_USER.[UserDomain]
      ,AUSLRE_SECURITY_USER.[UniqueId]
	FROM [AUSLRE_Protocollo].dbo.[SecurityUsers] AUSLRE_SECURITY_USER
	INNER JOIN [AUSLRE_Protocollo].dbo.[SecurityGroups] AUSLRE_SECURITY_GROUP ON AUSLRE_SECURITY_GROUP.idGroup = AUSLRE_SECURITY_USER.idGroup
	LEFT JOIN [ASMN_Protocollo].dbo.[SecurityUsers] ASMN_SECURITY_USER ON AUSLRE_SECURITY_USER.UniqueId = ASMN_SECURITY_USER.UniqueId
	WHERE 
		AUSLRE_SECURITY_GROUP.TenantId = '1F761813-6086-44F1-A507-99954EA64343'
		AND ASMN_SECURITY_USER.UniqueId IS NULL

OPEN security_user_cursor
FETCH NEXT FROM security_user_cursor INTO @Account, @Description, @idGroup, @RegistrationDate, @LastChangedUser, @LastChangedDate, @RegistrationUser
									 ,@UserDomain, @UniqueId

WHILE @@FETCH_STATUS = 0
BEGIN

 SELECT TOP 1 @FK_SecurityGroup = idGroup FROM [ASMN_Protocollo].[dbo].[SecurityGroups]
 WHERE TenantId = '1F761813-6086-44F1-A507-99954EA64343' AND IdSecurityGroupTenant = @idGroup

 INSERT INTO [ASMN_Protocollo].[dbo].[SecurityUsers]
           ([idUser],[Account],[Description],[idGroup],[RegistrationDate],[LastChangedUser],[LastChangedDate]
			,[RegistrationUser],[UserDomain],[UniqueId]) 
		   VALUES((select max([idUser]) from [ASMN_Protocollo].[dbo].[SecurityUsers])+ 1,@Account, @Description, @FK_SecurityGroup, @RegistrationDate, @LastChangedUser, @LastChangedDate, @RegistrationUser
									 ,@UserDomain, @UniqueId);
 
 INSERT INTO [ASMN_Pratiche].[dbo].[SecurityUsers]
           ([idUser],[Account],[Description],[idGroup],[RegistrationDate],[LastChangedUser],[LastChangedDate]
			,[RegistrationUser],[UserDomain],[UniqueId]) 
		   VALUES((select max([idUser]) from [ASMN_Pratiche].[dbo].[SecurityUsers])+ 1,@Account, @Description, @FK_SecurityGroup, @RegistrationDate, @LastChangedUser, @LastChangedDate, @RegistrationUser
									 ,@UserDomain, @UniqueId);

 INSERT INTO [ASMN_Atti].[dbo].[SecurityUsers]
           ([idUser],[Account],[Description],[idGroup],[RegistrationDate],[LastChangedUser],[LastChangedDate]
			,[RegistrationUser],[UserDomain],[UniqueId]) 
		   VALUES((select max([idUser]) from [ASMN_Atti].[dbo].[SecurityUsers])+ 1,@Account, @Description, @FK_SecurityGroup, @RegistrationDate, @LastChangedUser, @LastChangedDate, @RegistrationUser
									 ,@UserDomain, @UniqueId);


 FETCH NEXT FROM security_user_cursor INTO @Account, @Description, @idGroup, @RegistrationDate, @LastChangedUser, @LastChangedDate, @RegistrationUser
									 ,@UserDomain, @UniqueId

END 
CLOSE security_user_cursor
DEALLOCATE security_user_cursor
