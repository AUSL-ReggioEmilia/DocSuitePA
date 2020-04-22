SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;
GO

IF (SELECT OBJECT_ID('tempdb..#tmpErrors')) IS NOT NULL DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
GO
BEGIN TRANSACTION
GO

--#############################################################################

PRINT 'Creazione della tabella DocumentUnits'
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'cqrs')
BEGIN
	EXEC('CREATE SCHEMA cqrs');
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cqrs].[DocumentUnits](
	[IdDocumentUnit] [uniqueidentifier] NOT NULL,
	[IdContainer] [smallint] NOT NULL,
	[IdCategory] [smallint] NOT NULL,
	[IdUDSRepository] [uniqueidentifier] NULL,
	[IdFascicle] [uniqueidentifier] NULL,
	[EntityId] [int] NOT NULL,
	[Year] [smallint] NOT NULL,
	[Number] [int] NOT NULL,
	[Title] [nvarchar](256) NOT NULL,
	[Subject] [nvarchar](1000) NULL,
	[DocumentUnitName] [nvarchar](256) NOT NULL,
	[Environment] [int] NOT NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_DocumentUnit] PRIMARY KEY NONCLUSTERED 
(
	[IdDocumentUnit] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [cqrs].[DocumentUnits]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnits_Container] FOREIGN KEY([IdContainer])
REFERENCES [dbo].[Container] ([idContainer])
GO
ALTER TABLE [cqrs].[DocumentUnits] CHECK CONSTRAINT [FK_DocumentUnits_Container]
GO

ALTER TABLE [cqrs].[DocumentUnits]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnits_Category] FOREIGN KEY([IdCategory])
REFERENCES [dbo].[Category] ([idCategory])
GO
ALTER TABLE [cqrs].[DocumentUnits] CHECK CONSTRAINT [FK_DocumentUnits_Category]
GO

ALTER TABLE [cqrs].[DocumentUnits]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnits_Fascicles] FOREIGN KEY([IdFascicle])
REFERENCES [dbo].[Fascicles] ([IdFascicle])
GO
ALTER TABLE [cqrs].[DocumentUnits] CHECK CONSTRAINT [FK_DocumentUnits_Fascicles]
GO

ALTER TABLE [cqrs].[DocumentUnits]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnits_UDSRespositories] FOREIGN KEY([IdUDSRepository])
REFERENCES [uds].[UDSRepositories] ([IdUDSRepository])
GO
ALTER TABLE [cqrs].[DocumentUnits] CHECK CONSTRAINT [FK_DocumentUnits_UDSRespositories]
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
	BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
	BEGIN
		INSERT  INTO #tmpErrors (Error) VALUES (1);
		BEGIN TRANSACTION;
	END
GO

--#############################################################################

PRINT N'Aggiunto clustered indice [IX_DocumentUnits_RegistationDate] in DocumentUnits';
GO

CREATE CLUSTERED INDEX [IX_DocumentUnits_RegistationDate]
	ON [cqrs].[DocumentUnits]([RegistrationDate] ASC);
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
	BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
	BEGIN
		INSERT  INTO #tmpErrors (Error) VALUES (1);
		BEGIN TRANSACTION;
	END
GO

--#############################################################################

PRINT 'Creazione della tabella DocumentUnitRoles'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cqrs].[DocumentUnitRoles](
	[IdDocumentUnitRole] [uniqueidentifier] NOT NULL,
	[UniqueIdRole] [uniqueidentifier] NOT NULL,
	[IdDocumentUnit] [uniqueidentifier] NOT NULL,
	[RoleLabel] [nvarchar](256) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_DocumentUnitRole] PRIMARY KEY NONCLUSTERED  
(
	[IdDocumentUnitRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [cqrs].[DocumentUnitRoles]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnitRoles_DocumentUnits] FOREIGN KEY([IdDocumentUnit])
REFERENCES [cqrs].[DocumentUnits] ([IdDocumentUnit])
GO
ALTER TABLE [cqrs].[DocumentUnitRoles] CHECK CONSTRAINT [FK_DocumentUnitRoles_DocumentUnits]
GO

ALTER TABLE [cqrs].[DocumentUnitRoles]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnitRoles_Role] FOREIGN KEY([UniqueIdRole])
REFERENCES [dbo].[Role] ([UniqueId])
GO
ALTER TABLE [cqrs].[DocumentUnitRoles] CHECK CONSTRAINT [FK_DocumentUnitRoles_Role]
GO


IF @@ERROR <> 0 AND @@TRANCOUNT > 0
	BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
	BEGIN
		INSERT  INTO #tmpErrors (Error) VALUES (1);
		BEGIN TRANSACTION;
	END
GO
--#############################################################################

PRINT N'Aggiunto clustered indice [IX_DocumentUnitRoles_RegistationDate] in DocumentUnitRoles';
GO

CREATE CLUSTERED INDEX [IX_DocumentUnitRoles_RegistationDate]
	ON [cqrs].[DocumentUnitRoles]([RegistrationDate] ASC);
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
	BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
	BEGIN
		INSERT  INTO #tmpErrors (Error) VALUES (1);
		BEGIN TRANSACTION;
	END
GO

--#############################################################################

PRINT 'Creazione della tabella DocumentUnitChains'
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cqrs].[DocumentUnitChains](
	[IdDocumentUnitChain] [uniqueidentifier] NOT NULL,
	[IdDocumentUnit] [uniqueidentifier] NOT NULL,
	[DocumentName] [nvarchar](256) NULL,
	[ArchiveName] [nvarchar](256) NOT NULL,
	[IdArchiveChain] [uniqueidentifier] NOT NULL,
	[ChainType] [smallint] NOT NULL,
	[DocumentLabel] [nvarchar](256) NULL,
	[RegistrationUser] [nvarchar](256) NOT NULL,
	[RegistrationDate] [datetimeoffset](7) NOT NULL,
	[LastChangedDate] [datetimeoffset](7) NULL,
	[LastChangedUser] [nvarchar](256) NULL,
	[Timestamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_DocumentUnitChain] PRIMARY KEY NONCLUSTERED  
(
	[IdDocumentUnitChain] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [cqrs].[DocumentUnitChains]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnitChains_DocumentUnits] FOREIGN KEY([IdDocumentUnit])
REFERENCES [cqrs].[DocumentUnits] ([IdDocumentUnit])
GO
ALTER TABLE [cqrs].[DocumentUnitChains] CHECK CONSTRAINT [FK_DocumentUnitChains_DocumentUnits]
GO


IF @@ERROR <> 0 AND @@TRANCOUNT > 0
	BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
	BEGIN
		INSERT  INTO #tmpErrors (Error) VALUES (1);
		BEGIN TRANSACTION;
	END
GO
--#############################################################################

PRINT N'Aggiunto clustered indice [IX_DocumentUnitChains_RegistationDate] in DocumentUnitChains';
GO

CREATE CLUSTERED INDEX [IX_DocumentUnitChains_RegistationDate]
	ON [cqrs].[DocumentUnitChains]([RegistrationDate] ASC);
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
	BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
	BEGIN
		INSERT  INTO #tmpErrors (Error) VALUES (1);
		BEGIN TRANSACTION;
	END
GO
--#############################################################################

PRINT N'Modifico la Function FascicleDocumentUnits';
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION [dbo].[FascicleDocumentUnits]
(
	@FascicleId uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(    

	SELECT DU.[IdDocumentUnit] as UniqueId
	  ,DU.[IdFascicle]
	  ,DU.[EntityId]
	  ,DU.[Year]
	  ,CAST(DU.[Number] as varchar) as Number
	  ,DU.[Title]
	  ,DU.[Subject]
	  ,DU.[DocumentUnitName]
	  ,DU.[Environment]
	  ,DU.[RegistrationUser]
	  ,DU.[RegistrationDate]
	  ,COALESCE(FP.ReferenceType, FR.ReferenceType, FD.ReferenceType) as ReferenceType
	  ,CT.idCategory as Category_IdCategory
	  ,CT.Name as Category_Name
	  ,C.idContainer as Container_IdContainer
	  ,C.Name as Container_Name
	FROM cqrs.DocumentUnits DU
	LEFT OUTER JOIN dbo.FascicleProtocols FP ON FP.UniqueIdProtocol = DU.IdDocumentUnit
	LEFT OUTER JOIN dbo.FascicleResolutions FR ON FR.UniqueIdResolution = DU.IdDocumentUnit
	LEFT OUTER JOIN dbo.FascicleDocumentSeriesItems FD ON FD.IdDocumentSeriesItem = DU.EntityId
	INNER JOIN [dbo].[Category] CT on CT.idCategory = DU.idCategory
	INNER JOIN [dbo].[Container] C on C.idContainer = DU.idContainer
	WHERE 
	(FP.UniqueIdProtocol IS NOT NULL AND FP.IdFascicle = @FascicleId)
	OR (FR.UniqueIdResolution IS NOT NULL AND FR.IdFascicle = @FascicleId)
	OR (FD.IdDocumentSeriesItem IS NOT NULL AND FD.IdFascicle = @FascicleId)

);
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
	BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
	BEGIN
		INSERT  INTO #tmpErrors (Error) VALUES (1);
		BEGIN TRANSACTION;
	END
GO
--#############################################################################

PRINT N'Modifico la Function AvailableFasciclesFromResolution';
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[AvailableFasciclesFromResolution]
(
	@UniqueIdResolution uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(
	SELECT 
		F.IdFascicle as UniqueId,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object as FascicleObject,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.RegistrationUser,
		F.RegistrationDate						
	FROM Fascicles F 
		LEFT JOIN FascicleResolutions FR on F.IdFascicle = FR.IdFascicle AND FR.UniqueIdResolution = @UniqueIdResolution
		LEFT JOIN Resolution R on R.UniqueId = @UniqueIdResolution
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 2 AND CF.FascicleType = 1 
		WHERE F.IdCategory = R.IdCategoryAPI AND FR.IdFascicleResolution IS NULL
		AND F.EndDate IS NULL
	GROUP BY F.IdFascicle,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object ,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.RegistrationUser,
		F.RegistrationDate
);
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
	BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
	BEGIN
		INSERT  INTO #tmpErrors (Error) VALUES (1);
		BEGIN TRANSACTION;
	END
GO
--#############################################################################

PRINT N'Modifico la Function AvailableFasciclesFromProtocol';
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[AvailableFasciclesFromProtocol]
(
	@UniqueIdProtocol uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(
	SELECT 
		F.IdFascicle as UniqueId,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object as FascicleObject,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.RegistrationUser,
		F.RegistrationDate						
	FROM Fascicles F 
		LEFT JOIN FascicleProtocols FP on F.IdFascicle = FP.IdFascicle AND FP.UniqueIdProtocol = @UniqueIdProtocol
		LEFT JOIN Protocol P on P.UniqueId = @UniqueIdProtocol
		INNER JOIN CategoryFascicles CF on CF.IdCategory = F.IdCategory AND CF.DSWEnvironment = 1 AND CF.FascicleType = 1 
		WHERE F.IdCategory = P.IdCategoryAPI AND FP.IdFascicleProtocol IS NULL
		AND F.EndDate IS NULL
	GROUP BY F.IdFascicle,
		F.Year,
		F.Number,
		F.Conservation,
		F.StartDate,
		F.EndDate,
		F.Title,
		F.Name,
		F.Object ,
		F.Manager,
		F.Rack,
		F.Note,
		F.FascicleType,
		F.RegistrationUser,
		F.RegistrationDate
);
GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
	BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
	BEGIN
		INSERT  INTO #tmpErrors (Error) VALUES (1);
		BEGIN TRANSACTION;
	END
GO

--#############################################################################

PRINT N'ALTER FUNCTION [dbo].[FascicolableDocumentUnitsSecurityUser]';
GO

ALTER FUNCTION [dbo].[FascicolableDocumentUnitsSecurityUser]
(
	@UserName nvarchar(256), 
	@Domain nvarchar(256),
	@DateFrom datetimeoffset,
	@DateTo datetimeoffset,
	@IncludeThreshold bit,
	@ThresholdFrom datetimeoffset
)
RETURNS TABLE
AS
RETURN
(
	WITH 
	
	MySecurityGroups AS (
		SELECT SG.IdGroup 
		FROM [dbo].[SecurityGroups] SG 
		INNER JOIN [dbo].[SecurityUsers] SU on SG.idGroup = SU.idGroup
		WHERE SU.Account = @UserName AND SU.UserDomain = @Domain
		GROUP BY SG.IdGroup
	)

   
	SELECT DU.DocumentUnitName,
		   DU.[Year],
		   DU.[Title] AS [Number],
		   DU.[EntityId],
		   DU.[idCategory],
		   DU.[idContainer],
		   DU.[RegistrationUser],
		   DU.[RegistrationDate],
		   DU.[Subject],
		   DU.[IdDocumentUnit] as [UniqueId],
		   CT.idCategory AS Category_IdCategory,
		   CT.Name AS Category_Name,
		   C.idContainer AS Container_IdContainer,
		   C.Name AS Container_Name
	FROM [cqrs].[DocumentUnits] DU
	INNER JOIN [dbo].[Category] CT on DU.idCategory = CT.idCategory
	INNER JOIN [dbo].[Container] C on DU.idContainer = C.idContainer
	INNER JOIN [dbo].[ContainerGroup] CG on CG.IdContainer = C.IdContainer
	LEFT OUTER JOIN MySecurityGroups C_MSG on CG.IdGroup = C_MSG.IdGroup
	
	LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DU.IdDocumentUnit = DUR.IdDocumentUnit
	LEFT OUTER JOIN [dbo].[Role] RL on DUR.UniqueIdRole = RL.UniqueId
	LEFT OUTER JOIN [dbo].[RoleGroup] RG on RL.idRole = RG.idRole
	LEFT OUTER JOIN MySecurityGroups MSG on RG.IdGroup = MSG.IdGroup

	WHERE ( (@IncludeThreshold = 0 AND DU.RegistrationDate BETWEEN @DateFrom AND @DateTo) OR
				( @IncludeThreshold = 1 AND ( DU.RegistrationDate BETWEEN @ThresholdFrom AND CAST(getdate()-60 AS DATE) OR 
											  DU.RegistrationDate BETWEEN @DateFrom AND @DateTo))
			   )
			   AND ( (C_MSG.IdGroup IS NOT NULL AND (CASE Environment 
													WHEN 1 THEN CG.Rights 
													WHEN 2 THEN CG.ResolutionRights 
													WHEN 3 THEN '0'
													WHEN 4 THEN CG.DocumentSeriesRights
													WHEN 5 THEN '0'
													ELSE CG.UDSRights
													END) like '1%') OR 
					 ( DUR.UniqueIdRole IS NULL OR 
					  (DUR.UniqueIdRole IS NOT NULL AND (CASE Environment 
													WHEN 1 THEN RG.ProtocolRights 
													WHEN 2 THEN RG.ResolutionRights 
													WHEN 3 THEN '0'
													WHEN 4 THEN RG.DocumentSeriesRights
													ELSE '0'
													END like '1%') AND MSG.IdGroup IS NOT NULL)
					 ) 
				   )
			   AND NOT (C_MSG.IdGroup IS NULL AND MSG.IdGroup IS NULL)
			   AND DU.[IdFascicle] IS NULL
			   AND DU.Environment in (1,2)
	GROUP BY DU.DocumentUnitName,
		   DU.[Year],
		   DU.[Title],
		   DU.[EntityId],
		   DU.[idCategory],
		   DU.[idContainer],
		   DU.[RegistrationUser],
		   DU.[RegistrationDate],
		   DU.[Subject],
		   DU.[IdDocumentUnit],
		   CT.idCategory,
		   CT.Name,
		   C.idContainer,
		   C.Name
);

GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
	BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
	BEGIN
		INSERT  INTO #tmpErrors (Error) VALUES (1);
		BEGIN TRANSACTION;
	END
GO
--#############################################################################

PRINT N'ALTER FUNCTION [dbo].[FascicolableDocumentUnits]';
GO

ALTER FUNCTION [dbo].[FascicolableDocumentUnits]
(
    @DateFrom datetimeoffset,
    @DateTo datetimeoffset,
    @IncludeThreshold bit,
    @ThresholdFrom datetimeoffset,
    @RoleNames xml,
    @TenantId as uniqueidentifier
)
RETURNS TABLE
AS
RETURN
(
    WITH 
    MyNames AS ( 
	SELECT x.y.value('.','varchar(255)') AS GroupName
		FROM @RoleNames.nodes('/ArrayOfDomainGroupModel/DomainGroupModel/Name/text()') as x ( y )
		),
    MyRoles AS (
        SELECT R.UniqueId, Environment = 1
			FROM [Role] R
			INNER JOIN [RoleGroup] RG ON R.IdRole = RG.IdRole
			INNER JOIN MyNames MM on RG.GroupName = MM.GroupName
			WHERE RG.ProtocolRights like '1%' and R.IsActive = 1
			GROUP BY R.UniqueId
		UNION ALL 
		SELECT R.UniqueId, Environment = 2
				FROM [Role] R
				INNER JOIN [RoleGroup] RG ON R.IdRole = RG.IdRole
				INNER JOIN MyNames MM on RG.GroupName = MM.GroupName
				WHERE RG.ResolutionRights like '1%' and R.IsActive = 1
				GROUP BY R.UniqueId
		--UNION ALL 
		--SELECT R.UniqueId, Environment = 4
		--		FROM [Role] R
		--		INNER JOIN [RoleGroup] RG ON R.IdRole = RG.IdRole
		--		INNER JOIN  MyNames MM on RG.GroupName = MM.GroupName
		--		WHERE RG.DocumentSeriesRights like '1%' and R.IsActive = 1
		--		GROUP BY R.UniqueId
    )
    
	SELECT DU.DocumentUnitName,
		   DU.[Year],
		   DU.[Title] AS [Number],
		   DU.[EntityId],
		   DU.[idCategory],
		   DU.[idContainer],
		   DU.[RegistrationUser],
		   DU.[RegistrationDate],
		   DU.[Subject],
		   DU.[IdDocumentUnit] as [UniqueId],
		   CT.idCategory AS Category_IdCategory,
		   CT.Name AS Category_Name,
		   C.idContainer AS Container_IdContainer,
		   C.Name AS Container_Name
	FROM [cqrs].[DocumentUnits] DU
	INNER JOIN [dbo].[Category] CT on DU.idCategory = CT.idCategory
	INNER JOIN [dbo].[Container] C on DU.idContainer = C.idContainer
	INNER JOIN [dbo].[ContainerGroup] CG on CG.IdContainer = C.IdContainer
	LEFT OUTER JOIN MyNames MM on CG.GroupName =  MM.GroupName

    LEFT OUTER JOIN [cqrs].[DocumentUnitRoles] DUR on DU.IdDocumentUnit = DUR.IdDocumentUnit
    LEFT OUTER JOIN MyRoles D_MSG on DUR.UniqueIdRole = D_MSG.UniqueId AND DU.Environment = D_MSG.Environment

	WHERE ( (@IncludeThreshold = 0 AND DU.RegistrationDate BETWEEN @DateFrom AND @DateTo) OR
                ( @IncludeThreshold = 1 AND ( DU.RegistrationDate BETWEEN @ThresholdFrom AND CAST(getdate()-60 AS DATE) OR 
                                              DU.RegistrationDate BETWEEN @DateFrom AND @DateTo))
               )
               AND ( (MM.GroupName IS NOT NULL AND (CASE DU.Environment 
													WHEN 1 THEN CG.Rights 
													WHEN 2 THEN CG.ResolutionRights 
													WHEN 3 THEN '0'
													WHEN 4 THEN CG.DocumentSeriesRights
													WHEN 5 THEN '0'
													ELSE CG.UDSRights
													END) like '1%' COLLATE DATABASE_DEFAULT) OR
                     (MM.GroupName IS NULL AND D_MSG.UniqueId IS NOT NULL) OR
                     (MM.GroupName IS NOT NULL AND D_MSG.UniqueId IS NOT NULL)
                   )
               AND NOT (D_MSG.UniqueId IS NULL AND MM.GroupName IS NULL)

               AND DU.[IdFascicle] IS NULL
			   AND DU.Environment in (1,2)
	GROUP BY DU.DocumentUnitName,
		   DU.[Year],
		   DU.[Title],
		   DU.[EntityId],
		   DU.[idCategory],
		   DU.[idContainer],
		   DU.[RegistrationUser],
		   DU.[RegistrationDate],
		   DU.[Subject],
		   DU.[IdDocumentUnit],
		   CT.idCategory,
		   CT.Name,
		   C.idContainer,
		   C.Name
);


GO

IF @@ERROR <> 0 AND @@TRANCOUNT > 0
	BEGIN ROLLBACK;
END

IF @@TRANCOUNT = 0
	BEGIN
		INSERT  INTO #tmpErrors (Error) VALUES (1);
		BEGIN TRANSACTION;
	END
GO
--#############################################################################


IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
	PRINT N'The transacted portion of the database update succeeded.'
COMMIT TRANSACTION
END
ELSE PRINT N'The transacted portion of the database update FAILED.'
GO
DROP TABLE #tmpErrors
GO