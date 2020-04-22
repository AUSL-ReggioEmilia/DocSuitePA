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
	SELECT ASMN_SECURITY_USER.[Account]
      ,ASMN_SECURITY_USER.[Description]
      ,ASMN_SECURITY_USER.[idGroup]
      ,ASMN_SECURITY_USER.[RegistrationDate]
      ,ASMN_SECURITY_USER.[LastChangedUser]
      ,ASMN_SECURITY_USER.[LastChangedDate]
      ,ASMN_SECURITY_USER.[RegistrationUser]
      ,ASMN_SECURITY_USER.[UserDomain]
      ,ASMN_SECURITY_USER.[UniqueId]
	FROM [ASMN_Protocollo].dbo.[SecurityUsers] ASMN_SECURITY_USER
	INNER JOIN [ASMN_Protocollo].dbo.[SecurityGroups] ASMN_SECURITY_GROUP ON ASMN_SECURITY_GROUP.idGroup = ASMN_SECURITY_USER.idGroup
	LEFT JOIN [AUSLRE_Protocollo].dbo.[SecurityUsers] AUSLRE_SECURITY_USER ON ASMN_SECURITY_USER.UniqueId = AUSLRE_SECURITY_USER.UniqueId
	WHERE 
		ASMN_SECURITY_GROUP.TenantId = '1F761813-6086-44F1-A507-99954EA64342'
		AND AUSLRE_SECURITY_USER.UniqueId IS NULL

OPEN security_user_cursor
FETCH NEXT FROM security_user_cursor INTO @Account, @Description, @idGroup, @RegistrationDate, @LastChangedUser, @LastChangedDate, @RegistrationUser
									 ,@UserDomain, @UniqueId

WHILE @@FETCH_STATUS = 0
BEGIN

 SELECT TOP 1 @FK_SecurityGroup = idGroup FROM [AUSLRE_Protocollo].[dbo].[SecurityGroups]
 WHERE TenantId = '1F761813-6086-44F1-A507-99954EA64342' AND IdSecurityGroupTenant = @idGroup

 INSERT INTO [AUSLRE_Protocollo].[dbo].[SecurityUsers]
           ([idUser],[Account],[Description],[idGroup],[RegistrationDate],[LastChangedUser],[LastChangedDate]
			,[RegistrationUser],[UserDomain],[UniqueId]) 
		   VALUES((select max([idUser]) from [AUSLRE_Protocollo].[dbo].[SecurityUsers])+ 1,@Account, @Description, @FK_SecurityGroup, @RegistrationDate, @LastChangedUser, @LastChangedDate, @RegistrationUser
									 ,@UserDomain, @UniqueId);
 
 INSERT INTO [AUSLRE_Pratiche].[dbo].[SecurityUsers]
           ([idUser],[Account],[Description],[idGroup],[RegistrationDate],[LastChangedUser],[LastChangedDate]
			,[RegistrationUser],[UserDomain],[UniqueId]) 
		   VALUES((select max([idUser]) from [AUSLRE_Pratiche].[dbo].[SecurityUsers])+ 1,@Account, @Description, @FK_SecurityGroup, @RegistrationDate, @LastChangedUser, @LastChangedDate, @RegistrationUser
									 ,@UserDomain, @UniqueId);

 INSERT INTO [AUSLRE_Atti].[dbo].[SecurityUsers]
           ([idUser],[Account],[Description],[idGroup],[RegistrationDate],[LastChangedUser],[LastChangedDate]
			,[RegistrationUser],[UserDomain],[UniqueId]) 
		   VALUES((select max([idUser]) from [AUSLRE_Atti].[dbo].[SecurityUsers])+ 1,@Account, @Description, @FK_SecurityGroup, @RegistrationDate, @LastChangedUser, @LastChangedDate, @RegistrationUser
									 ,@UserDomain, @UniqueId);


 FETCH NEXT FROM security_user_cursor INTO @Account, @Description, @idGroup, @RegistrationDate, @LastChangedUser, @LastChangedDate, @RegistrationUser
									 ,@UserDomain, @UniqueId

END 
CLOSE security_user_cursor
DEALLOCATE security_user_cursor
