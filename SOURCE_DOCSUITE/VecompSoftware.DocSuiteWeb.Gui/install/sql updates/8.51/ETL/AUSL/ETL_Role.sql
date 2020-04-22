DECLARE @IdRoleTenant smallint
DECLARE @name varchar(256)
DECLARE @isActive tinyint
DECLARE @EMailAddress nvarchar(256)
DECLARE @idRoleFather smallint
DECLARE @FullIncrementalPath nvarchar(256)
DECLARE @DocmLocation smallint
DECLARE @ProtLocation smallint
DECLARE @ReslLocation smallint
DECLARE @RegistrationUser nvarchar(256)
DECLARE @RegistrationDate datetimeoffset(7)
DECLARE @LastChangedUser nvarchar(256)
DECLARE @LastChangedDate datetimeoffset(7)
DECLARE @ServiceCode varchar(256)
DECLARE @Collapsed tinyint
DECLARE @ActiveFrom datetime
DECLARE @ActiveTo datetime
DECLARE @isChanged smallint
DECLARE @UriSharepoint varchar(MAX)
DECLARE @UniqueId uniqueidentifier
DECLARE @TenantId uniqueidentifier
DECLARE @FK_RoleFather smallint

DECLARE role_cursor CURSOR FOR
	SELECT		ASMN_ROLE.IdRoleTenant
				,ASMN_ROLE.[Name],ASMN_ROLE.[isActive],ASMN_ROLE.[EMailAddress],ASMN_ROLE.[idRoleFather],ASMN_ROLE.[FullIncrementalPath]
				,ASMN_ROLE.[DocmLocation],ASMN_ROLE.[ProtLocation],ASMN_ROLE.[ReslLocation],ASMN_ROLE.[RegistrationUser]
				,ASMN_ROLE.[RegistrationDate],ASMN_ROLE.[LastChangedUser],ASMN_ROLE.[LastChangedDate],ASMN_ROLE.[ServiceCode]
				,ASMN_ROLE.[Collapsed],ASMN_ROLE.[ActiveFrom],ASMN_ROLE.[ActiveTo]
				,ASMN_ROLE.[isChanged],ASMN_ROLE.[UriSharepoint],ASMN_ROLE.[UniqueId],ASMN_ROLE.[TenantId]
	FROM [ASMN_Protocollo].dbo.[Role] ASMN_ROLE
	LEFT JOIN [AUSLRE_Protocollo].dbo.[Role] AUSLRE_ROLE ON ASMN_ROLE.UniqueId = AUSLRE_ROLE.UniqueId
	WHERE 
			ASMN_ROLE.[isActive] = 1 
		AND ASMN_ROLE.TenantId = '1F761813-6086-44F1-A507-99954EA64342'
		AND AUSLRE_ROLE.UniqueId IS NULL
	ORDER BY ASMN_ROLE.FullIncrementalPath

OPEN role_cursor
FETCH NEXT FROM role_cursor INTO @IdRoleTenant,@name,@isActive,@EMailAddress,@idRoleFather,@FullIncrementalPath,@DocmLocation,
@ProtLocation,@ReslLocation,@RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate,@ServiceCode,
@Collapsed,@ActiveFrom,@ActiveTo,@isChanged,@UriSharepoint,@UniqueId,@TenantId

WHILE @@FETCH_STATUS = 0
BEGIN
 SET @FK_RoleFather = null
 SELECT TOP 1 @FK_RoleFather = idRole FROM [AUSLRE_Protocollo].[dbo].[Role] WHERE TenantId = '1F761813-6086-44F1-A507-99954EA64342' AND IdRoleTenant = @IdRoleFather

 INSERT INTO [AUSLRE_Protocollo].[dbo].[Role]
           ([idRole],[IdRoleTenant],[Name],[isActive],[EMailAddress],[idRoleFather],[FullIncrementalPath],[DocmLocation],[ProtLocation],[ReslLocation],[RegistrationUser]
           ,[RegistrationDate],[LastChangedUser],[LastChangedDate],[ServiceCode],[Collapsed],[ActiveFrom],[ActiveTo]
           ,[isChanged],[UriSharepoint],[UniqueId],[TenantId]) 
		   VALUES((select max([idRole])  from [AUSLRE_Protocollo].[dbo].[Role])+1,@IdRoleTenant,@name,@isActive,@EMailAddress,@FK_RoleFather,@FullIncrementalPath,@DocmLocation,
			@ProtLocation,@ReslLocation,@RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate,@ServiceCode,
			@Collapsed,@ActiveFrom,@ActiveTo,@isChanged,@UriSharepoint,@UniqueId,@TenantId);
 
 INSERT INTO [AUSLRE_Pratiche].[dbo].[Role]
           ([idRole],[IdRoleTenant],[Name],[isActive],[EMailAddress],[idRoleFather],[FullIncrementalPath],[DocmLocation],[ProtLocation],[ReslLocation],[RegistrationUser]
           ,[RegistrationDate],[LastChangedUser],[LastChangedDate],[ServiceCode],[Collapsed],[ActiveFrom],[ActiveTo]
           ,[isChanged],[UriSharepoint],[UniqueId],[TenantId]) 
		   VALUES((select max([idRole])  from [AUSLRE_Pratiche].[dbo].[Role])+1,@IdRoleTenant,@name,@isActive,@EMailAddress,@FK_RoleFather,@FullIncrementalPath,@DocmLocation,
			@ProtLocation,@ReslLocation,@RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate,@ServiceCode,
			@Collapsed,@ActiveFrom,@ActiveTo,@isChanged,@UriSharepoint,@UniqueId,@TenantId);

 INSERT INTO [AUSLRE_Atti].[dbo].[Role]
           ([idRole],[IdRoleTenant],[Name],[isActive],[EMailAddress],[idRoleFather],[FullIncrementalPath],[DocmLocation],[ProtLocation],[ReslLocation],[RegistrationUser]
           ,[RegistrationDate],[LastChangedUser],[LastChangedDate],[ServiceCode],[Collapsed],[ActiveFrom],[ActiveTo]
           ,[isChanged],[UriSharepoint],[UniqueId],[TenantId]) 
		   VALUES((select max([idRole])  from [AUSLRE_Atti].[dbo].[Role])+1,@IdRoleTenant,@name,@isActive,@EMailAddress,@FK_RoleFather,@FullIncrementalPath,@DocmLocation,
			@ProtLocation,@ReslLocation,@RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate,@ServiceCode,
			@Collapsed,@ActiveFrom,@ActiveTo,@isChanged,@UriSharepoint,@UniqueId,@TenantId);


 FETCH NEXT FROM role_cursor INTO @IdRoleTenant,@name,@isActive,@EMailAddress,@idRoleFather,@FullIncrementalPath,@DocmLocation,
@ProtLocation,@ReslLocation,@RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate,@ServiceCode,
@Collapsed,@ActiveFrom,@ActiveTo,@isChanged,@UriSharepoint,@UniqueId,@TenantId

END 
CLOSE role_cursor
DEALLOCATE role_cursor

GO
UPDATE [AUSLRE_Protocollo].[dbo].[Parameter] SET [LastUsedIdRole]=(select max([idRole]) from [AUSLRE_Protocollo].[dbo].[Role])
GO
UPDATE [AUSLRE_Atti].[dbo].[Parameter] SET [LastUsedIdRole]=(select max([idRole]) from [AUSLRE_Protocollo].[dbo].[Role])
GO
UPDATE [AUSLRE_Pratiche].[dbo].[Parameter] SET [LastUsedIdRole]=(select max([idRole]) from [AUSLRE_Protocollo].[dbo].[Role])
GO
