DECLARE @IdRoleGroup uniqueidentifier
DECLARE @GroupName varchar(255)
DECLARE @idRole smallint
DECLARE @ProtocolRights char(10)
DECLARE @ResolutionRights char(10)
DECLARE @DocumentRights char(10)
DECLARE @RegistrationUser nvarchar(256)
DECLARE @RegistrationDate datetimeoffset(7)
DECLARE @LastChangedUser nvarchar(256)
DECLARE @LastChangedDate datetimeoffset(7)
DECLARE @DocumentSeriesRights varchar(10)
DECLARE @idGroup int
DECLARE @FK_idRole smallint
DECLARE @FK_SecurityGroup int

DECLARE role_cursor CURSOR FOR
	SELECT ASMN_ROLEGROUP.[IdRoleGroup],ASMN_ROLEGROUP.[GroupName],ASMN_ROLEGROUP.[idRole],ASMN_ROLEGROUP.[ProtocolRights],
	ASMN_ROLEGROUP.[ResolutionRights],ASMN_ROLEGROUP.[DocumentRights],ASMN_ROLEGROUP.[RegistrationUser],
	ASMN_ROLEGROUP.[RegistrationDate],ASMN_ROLEGROUP.[LastChangedUser],ASMN_ROLEGROUP.[LastChangedDate],
	ASMN_ROLEGROUP.[DocumentSeriesRights], ASMN_ROLEGROUP.[idGroup]
	FROM [ASMN_Protocollo].dbo.[RoleGroup] ASMN_ROLEGROUP
	INNER JOIN [ASMN_Protocollo].dbo.[Role] ASMN_ROLE ON ASMN_ROLEGROUP.idRole = ASMN_ROLE.IdRole
	LEFT JOIN [AUSLRE_Protocollo].dbo.[RoleGroup] AUSLRE_ROLEGROUP ON ASMN_ROLEGROUP.IdRoleGroup = AUSLRE_ROLEGROUP.IdRoleGroup
	WHERE 
			ASMN_ROLE.[isActive] = 1 
		AND ASMN_ROLE.TenantId = '1F761813-6086-44F1-A507-99954EA64342'
		AND AUSLRE_ROLEGROUP.IdRoleGroup IS NULL

OPEN role_cursor
FETCH NEXT FROM role_cursor INTO @IdRoleGroup,@GroupName,@idRole,@ProtocolRights,@ResolutionRights,@DocumentRights,
								 @RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate,@DocumentSeriesRights,
								 @idGroup

WHILE @@FETCH_STATUS = 0
BEGIN
 SET @FK_idRole = null
 SET @FK_SecurityGroup = null

 SELECT TOP 1 @FK_idRole = idRole 
 FROM [AUSLRE_Protocollo].[dbo].[Role] WHERE IdRoleTenant = @idRole AND TenantId = '1F761813-6086-44F1-A507-99954EA64342'

 SELECT TOP 1 @FK_SecurityGroup = idGroup 
 FROM [AUSLRE_Protocollo].[dbo].[SecurityGroups] WHERE IdSecurityGroupTenant = @idGroup AND TenantId = '1F761813-6086-44F1-A507-99954EA64342'

 INSERT INTO [AUSLRE_Protocollo].[dbo].[RoleGroup]
           ([IdRoleGroup],[GroupName],[idRole],[ProtocolRights],[ResolutionRights],[DocumentRights],[RegistrationUser],[RegistrationDate]
           ,[LastChangedUser],[LastChangedDate],[DocumentSeriesRights],[idGroup]) 
		   VALUES(@IdRoleGroup,@GroupName,@FK_idRole,@ProtocolRights,@ResolutionRights,@DocumentRights,@RegistrationUser,@RegistrationDate,
		   @LastChangedUser,@LastChangedDate,@DocumentSeriesRights,@FK_SecurityGroup);
 
 INSERT INTO [AUSLRE_Pratiche].[dbo].[RoleGroup]
           ([IdRoleGroup],[GroupName],[idRole],[ProtocolRights],[ResolutionRights],[DocumentRights],[RegistrationUser],[RegistrationDate]
           ,[LastChangedUser],[LastChangedDate],[DocumentSeriesRights],[idGroup]) 
		   VALUES(@IdRoleGroup,@GroupName,@FK_idRole,@ProtocolRights,@ResolutionRights,@DocumentRights,@RegistrationUser,@RegistrationDate,
		   @LastChangedUser,@LastChangedDate,@DocumentSeriesRights,@FK_SecurityGroup);

 INSERT INTO [AUSLRE_Atti].[dbo].[RoleGroup]
           ([IdRoleGroup],[GroupName],[idRole],[ProtocolRights],[ResolutionRights],[DocumentRights],[RegistrationUser],[RegistrationDate]
           ,[LastChangedUser],[LastChangedDate],[DocumentSeriesRights],[idGroup]) 
		   VALUES(@IdRoleGroup,@GroupName,@FK_idRole,@ProtocolRights,@ResolutionRights,@DocumentRights,@RegistrationUser,@RegistrationDate,
		   @LastChangedUser,@LastChangedDate,@DocumentSeriesRights,@FK_SecurityGroup);


 FETCH NEXT FROM role_cursor INTO  @IdRoleGroup,@GroupName,@idRole,@ProtocolRights,@ResolutionRights,@DocumentRights,
								 @RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate,@DocumentSeriesRights,
								 @idGroup

END 
CLOSE role_cursor
DEALLOCATE role_cursor
