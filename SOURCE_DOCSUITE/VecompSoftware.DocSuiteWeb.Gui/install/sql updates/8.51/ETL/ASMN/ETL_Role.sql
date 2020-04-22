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
	SELECT		AUSLRE_ROLE.IdRoleTenant
				,AUSLRE_ROLE.[Name],AUSLRE_ROLE.[isActive],AUSLRE_ROLE.[EMailAddress],AUSLRE_ROLE.[idRoleFather],AUSLRE_ROLE.[FullIncrementalPath]
				,AUSLRE_ROLE.[DocmLocation],AUSLRE_ROLE.[ProtLocation],AUSLRE_ROLE.[ReslLocation],AUSLRE_ROLE.[RegistrationUser]
				,AUSLRE_ROLE.[RegistrationDate],AUSLRE_ROLE.[LastChangedUser],AUSLRE_ROLE.[LastChangedDate],AUSLRE_ROLE.[ServiceCode]
				,AUSLRE_ROLE.[Collapsed],AUSLRE_ROLE.[ActiveFrom],AUSLRE_ROLE.[ActiveTo]
				,AUSLRE_ROLE.[isChanged],AUSLRE_ROLE.[UriSharepoint],AUSLRE_ROLE.[UniqueId],AUSLRE_ROLE.[TenantId]
	FROM [AUSLRE_Protocollo].dbo.[Role] AUSLRE_ROLE
	LEFT JOIN [ASMN_Protocollo].dbo.[Role] ASMN_ROLE ON AUSLRE_ROLE.UniqueId = ASMN_ROLE.UniqueId
	WHERE 
			AUSLRE_ROLE.[isActive] = 1 
		AND AUSLRE_ROLE.TenantId = '1F761813-6086-44F1-A507-99954EA64343'
		AND ASMN_ROLE.UniqueId IS NULL
	ORDER BY AUSLRE_ROLE.FullIncrementalPath

OPEN role_cursor
FETCH NEXT FROM role_cursor INTO @IdRoleTenant,@name,@isActive,@EMailAddress,@idRoleFather,@FullIncrementalPath,@DocmLocation,
@ProtLocation,@ReslLocation,@RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate,@ServiceCode,
@Collapsed,@ActiveFrom,@ActiveTo,@isChanged,@UriSharepoint,@UniqueId,@TenantId

WHILE @@FETCH_STATUS = 0
BEGIN
 SET @FK_RoleFather = null
 SELECT TOP 1 @FK_RoleFather = idRole FROM [ASMN_Protocollo].[dbo].[Role] WHERE TenantId = '1F761813-6086-44F1-A507-99954EA64343' AND IdRoleTenant = @IdRoleFather

 INSERT INTO [ASMN_Protocollo].[dbo].[Role]
           ([idRole],[IdRoleTenant],[Name],[isActive],[EMailAddress],[idRoleFather],[FullIncrementalPath],[DocmLocation],[ProtLocation],[ReslLocation],[RegistrationUser]
           ,[RegistrationDate],[LastChangedUser],[LastChangedDate],[ServiceCode],[Collapsed],[ActiveFrom],[ActiveTo]
           ,[isChanged],[UriSharepoint],[UniqueId],[TenantId]) 
		   VALUES((select max([idRole])  from [ASMN_Protocollo].[dbo].[Role])+1,@IdRoleTenant,@name,@isActive,@EMailAddress,@FK_RoleFather,@FullIncrementalPath,@DocmLocation,
			@ProtLocation,@ReslLocation,@RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate,@ServiceCode,
			@Collapsed,@ActiveFrom,@ActiveTo,@isChanged,@UriSharepoint,@UniqueId,@TenantId);
 
 INSERT INTO [ASMN_Pratiche].[dbo].[Role]
           ([idRole],[IdRoleTenant],[Name],[isActive],[EMailAddress],[idRoleFather],[FullIncrementalPath],[DocmLocation],[ProtLocation],[ReslLocation],[RegistrationUser]
           ,[RegistrationDate],[LastChangedUser],[LastChangedDate],[ServiceCode],[Collapsed],[ActiveFrom],[ActiveTo]
           ,[isChanged],[UriSharepoint],[UniqueId],[TenantId]) 
		   VALUES((select max([idRole])  from [ASMN_Pratiche].[dbo].[Role])+1,@IdRoleTenant,@name,@isActive,@EMailAddress,@FK_RoleFather,@FullIncrementalPath,@DocmLocation,
			@ProtLocation,@ReslLocation,@RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate,@ServiceCode,
			@Collapsed,@ActiveFrom,@ActiveTo,@isChanged,@UriSharepoint,@UniqueId,@TenantId);

 INSERT INTO [ASMN_Atti].[dbo].[Role]
           ([idRole],[IdRoleTenant],[Name],[isActive],[EMailAddress],[idRoleFather],[FullIncrementalPath],[DocmLocation],[ProtLocation],[ReslLocation],[RegistrationUser]
           ,[RegistrationDate],[LastChangedUser],[LastChangedDate],[ServiceCode],[Collapsed],[ActiveFrom],[ActiveTo]
           ,[isChanged],[UriSharepoint],[UniqueId],[TenantId]) 
		   VALUES((select max([idRole])  from [ASMN_Atti].[dbo].[Role])+1,@IdRoleTenant,@name,@isActive,@EMailAddress,@FK_RoleFather,@FullIncrementalPath,@DocmLocation,
			@ProtLocation,@ReslLocation,@RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate,@ServiceCode,
			@Collapsed,@ActiveFrom,@ActiveTo,@isChanged,@UriSharepoint,@UniqueId,@TenantId);


 FETCH NEXT FROM role_cursor INTO @IdRoleTenant,@name,@isActive,@EMailAddress,@idRoleFather,@FullIncrementalPath,@DocmLocation,
@ProtLocation,@ReslLocation,@RegistrationUser,@RegistrationDate,@LastChangedUser,@LastChangedDate,@ServiceCode,
@Collapsed,@ActiveFrom,@ActiveTo,@isChanged,@UriSharepoint,@UniqueId,@TenantId

END 
CLOSE role_cursor
DEALLOCATE role_cursor

GO
UPDATE [ASMN_Protocollo].[dbo].[Parameter] SET [LastUsedIdRole]=(select max([idRole]) from [ASMN_Protocollo].[dbo].[Role])
GO
UPDATE [ASMN_Atti].[dbo].[Parameter] SET [LastUsedIdRole]=(select max([idRole]) from [ASMN_Protocollo].[dbo].[Role])
GO
UPDATE [ASMN_Pratiche].[dbo].[Parameter] SET [LastUsedIdRole]=(select max([idRole]) from [ASMN_Protocollo].[dbo].[Role])
GO
