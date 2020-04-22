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
	SELECT AUSLRE_ROLEGROUP.[IdRoleGroup],AUSLRE_ROLEGROUP.[GroupName],AUSLRE_ROLEGROUP.[idRole],AUSLRE_ROLEGROUP.[ProtocolRights],
	AUSLRE_ROLEGROUP.[ResolutionRights],AUSLRE_ROLEGROUP.[DocumentRights],AUSLRE_ROLEGROUP.[RegistrationUser],
	AUSLRE_ROLEGROUP.[RegistrationDate],AUSLRE_ROLEGROUP.[LastChangedUser],AUSLRE_ROLEGROUP.[LastChangedDate],
	AUSLRE_ROLEGROUP.[DocumentSeriesRights], AUSLRE_ROLEGROUP.[idGroup]
	FROM [AUSLRE_Protocollo].dbo.[RoleGroup] AUSLRE_ROLEGROUP
	INNER JOIN [AUSLRE_Protocollo].dbo.[Role] AUSLRE_ROLE ON AUSLRE_ROLEGROUP.idRole = AUSLRE_ROLE.IdRole
	LEFT JOIN [ASMN_Protocollo].dbo.[RoleGroup] ASMN_ROLEGROUP ON ASMN_ROLEGROUP.IdRoleGroup = AUSLRE_ROLEGROUP.IdRoleGroup
	WHERE 
			AUSLRE_ROLE.[isActive] = 1 
		AND AUSLRE_ROLE.TenantId = '1F761813-6086-44F1-A507-99954EA64343'
		AND ASMN_ROLEGROUP.IdRoleGroup IS NULL

OPEN role_cursor
FETCH NEXT FROM role_cursor INTO @IdRoleGroup,@GroupName,@idRole,@ProtocolRights,@ResolutionRights,@DocumentRights,
								 @RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate,@DocumentSeriesRights,
								 @idGroup

WHILE @@FETCH_STATUS = 0
BEGIN
 SET @FK_idRole = null
 SET @FK_SecurityGroup = null

 SELECT TOP 1 @FK_idRole = idRole 
 FROM [ASMN_Protocollo].[dbo].[Role] WHERE IdRoleTenant = @idRole AND TenantId = '1F761813-6086-44F1-A507-99954EA64343'

 SELECT TOP 1 @FK_SecurityGroup = idGroup 
 FROM [ASMN_Protocollo].[dbo].[SecurityGroups] WHERE IdSecurityGroupTenant = @idGroup AND TenantId = '1F761813-6086-44F1-A507-99954EA64343'

 INSERT INTO [ASMN_Protocollo].[dbo].[RoleGroup]
           ([IdRoleGroup],[GroupName],[idRole],[ProtocolRights],[ResolutionRights],[DocumentRights],[RegistrationUser],[RegistrationDate]
           ,[LastChangedUser],[LastChangedDate],[DocumentSeriesRights],[idGroup]) 
		   VALUES(@IdRoleGroup,@GroupName,@FK_idRole,@ProtocolRights,@ResolutionRights,@DocumentRights,@RegistrationUser,@RegistrationDate,
		   @LastChangedUser,@LastChangedDate,@DocumentSeriesRights,@FK_SecurityGroup);
 
 INSERT INTO [ASMN_Pratiche].[dbo].[RoleGroup]
           ([IdRoleGroup],[GroupName],[idRole],[ProtocolRights],[ResolutionRights],[DocumentRights],[RegistrationUser],[RegistrationDate]
           ,[LastChangedUser],[LastChangedDate],[DocumentSeriesRights],[idGroup]) 
		   VALUES(@IdRoleGroup,@GroupName,@FK_idRole,@ProtocolRights,@ResolutionRights,@DocumentRights,@RegistrationUser,@RegistrationDate,
		   @LastChangedUser,@LastChangedDate,@DocumentSeriesRights,@FK_SecurityGroup);

 INSERT INTO [ASMN_Atti].[dbo].[RoleGroup]
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
