-- =============================================
-- Script Template
-- =============================================
USE [BiblosDS2010]
GO
/****** Object:  View [dbo].[DataBaseVersion]    Script Date: 10/17/2011 10:57:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[DataBaseVersion]
AS
SELECT     '1.100' AS LocalPath
GO
/****** Object:  Table [dbo].[Component]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Component](
	[IdComponent] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NULL,
	[Server] [varchar](50) NULL,
	[InstallPath] [varchar](250) NULL,
	[Enable] [smallint] NOT NULL,
 CONSTRAINT [PK_Component] PRIMARY KEY CLUSTERED 
(
	[IdComponent] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CertificateStore]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CertificateStore](
	[IdCertificate] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Name] [varchar](255) NULL,
	[IsDefault] [smallint] NOT NULL,
 CONSTRAINT [PK_CertificateStore] PRIMARY KEY CLUSTERED 
(
	[IdCertificate] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OperationType]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OperationType](
	[Type] [varchar](255) NULL,
	[IdOperationType] [smallint] NOT NULL,
 CONSTRAINT [PK_OperationType_1] PRIMARY KEY CLUSTERED 
(
	[IdOperationType] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Log]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Log](
	[IdEntry] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[IdOperationType] [smallint] NOT NULL,
	[IdArchive] [uniqueidentifier] NOT NULL,
	[TimeStamp] [datetime] NULL,
	[IdStorage] [uniqueidentifier] NOT NULL,
	[IdCorrelation] [uniqueidentifier] NULL,
	[Message] [text] NULL,
	[Server] [varchar](255) NOT NULL,
	[Client] [varchar](255) NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[IdEntry] ASC
) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[KeyValue]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[KeyValue](
	[KeyValue] [varchar](255) NULL,
	[IdDocument] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
 CONSTRAINT [PK_KeyValue] PRIMARY KEY CLUSTERED 
(
	[IdDocument] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Journal]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Journal](
	[IdEntry] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[IdOperationType] [smallint] NOT NULL,
	[IdArchive] [uniqueidentifier] NOT NULL,
	[UserAgent] [varchar](255) NULL,
	[TimeStamp] [datetime] NULL,
	[IdStorage] [uniqueidentifier] NOT NULL,
	[IdCorrelation] [uniqueidentifier] NULL,
	[Message] [text] NULL,
 CONSTRAINT [PK_Journal] PRIMARY KEY CLUSTERED 
(
	[IdEntry] ASC
) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DocumentStatus]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DocumentStatus](
	[IdDocumentStatus] [smallint] NOT NULL,
	[Description] [varchar](255) NULL,
 CONSTRAINT [PK_DocumentStatus] PRIMARY KEY CLUSTERED 
(
	[IdDocumentStatus] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DocumentNodeType]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DocumentNodeType](
	[Id] [smallint] NOT NULL,
	[Description] [varchar](255) NULL,
 CONSTRAINT [PK_DocumentNodeType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DocumentCache]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DocumentCache](
	[IdDocument] [uniqueidentifier] NOT NULL,
	[ServerName] [varchar](50) NOT NULL,
	[FileName] [varchar](250) NOT NULL,
	[Signature] [varchar](250) NULL,
	[DateCreated] [datetime] NULL,
	[Size] [float] NULL,
 CONSTRAINT [PK_DocumentCache] PRIMARY KEY CLUSTERED 
(
	[IdDocument] ASC,
	[ServerName] ASC,
	[FileName] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AttributesMode]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AttributesMode](
	[IdMode] [int] NOT NULL,
	[Description] [varchar](255) NULL,
 CONSTRAINT [PK_AttributesStatus] PRIMARY KEY CLUSTERED 
(
	[IdMode] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AttributesGroup]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AttributesGroup](
	[IdAttributeGroup] [uniqueidentifier] NOT NULL,
	[IdArchive] [uniqueidentifier] NOT NULL,
	[IdAttributeGroupType] [int] NOT NULL,
	[Description] [varchar](250) NULL,
	[IsVisible] [smallint] NULL,
 CONSTRAINT [PK_AttributeGroup_1] PRIMARY KEY CLUSTERED 
(
	[IdAttributeGroup] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Archive]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Archive](
	[IdArchive] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[IsLegal] [smallint] NOT NULL,
	[MaxCache] [bigint] NULL,
	[UpperCache] [bigint] NULL,
	[LowerCache] [bigint] NULL,
	[LastIdBiblos] [int] NOT NULL,
	[AutoVersion] [smallint] NULL,
	[AuthorizationAssembly] [varchar](255) NULL,
	[AuthorizationClassName] [varchar](50) NULL,
	[EnableSecurity] [smallint] NULL,
	[PathTransito] [text] NULL,
	[PathCache] [text] NULL,
	[PathPreservation] [text] NULL,
	[LastAutoIncValue] [bigint] NULL,
	[ThumbnailEnabled] [bit] NULL,
	[PdfConversionEnabled] [bit] NULL,
	[FullSignEnabled] [bit] NULL,
	[VerifyPreservationDateEnabled] [bit] NULL,
	[VerifyPreservationIncrementalEnabled] [bit] NULL,
	[TransitoEnabled] [bit] NULL,
 CONSTRAINT [PK_Archive] PRIMARY KEY CLUSTERED 
(
	[IdArchive] ASC
) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationHolidays]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationHolidays](
	[IdPreservationHolidays] [uniqueidentifier] NOT NULL,
	[HolidayDate] [smalldatetime] NOT NULL,
	[Description] [varchar](50) NULL,
 CONSTRAINT [PK_PreservationHolidays] PRIMARY KEY CLUSTERED 
(
	[IdPreservationHolidays] ASC
) ON [PRIMARY],
 CONSTRAINT [UQ_PreservationHolidays] UNIQUE NONCLUSTERED 
(
	[HolidayDate] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationExceptionType]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationExceptionType](
	[IdPreservationExceptionType] [uniqueidentifier] NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[IsFail] [bit] NOT NULL,
 CONSTRAINT [PK_ExceptionType] PRIMARY KEY CLUSTERED 
(
	[IdPreservationExceptionType] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PermissionMode]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PermissionMode](
	[IdMode] [smallint] NOT NULL,
	[Description] [varchar](50) NULL,
 CONSTRAINT [PK_PermissionMode] PRIMARY KEY CLUSTERED 
(
	[IdMode] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PermissionAllowed]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PermissionAllowed](
	[IdPermission] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NULL,
	[IdMode] [smallint] NULL,
 CONSTRAINT [PK_PermissionAllowed] PRIMARY KEY CLUSTERED 
(
	[IdPermission] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationJournalingActivity]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationJournalingActivity](
	[IdPreservationJournalingActivity] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[KeyCode] [varchar](50) NOT NULL,
	[Description] [varchar](250) NOT NULL,
	[IsUserActivity] [bit] NULL,
	[IsUserDeleteEnable] [bit] NULL,
 CONSTRAINT [PK_PreservationJournalingActivity] PRIMARY KEY CLUSTERED 
(
	[IdPreservationJournalingActivity] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationSchedule]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationSchedule](
	[IdPreservationSchedule] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Period] [varchar](255) NULL,
	[ValidWeekDays] [varchar](255) NULL,
	[FrequencyType] [smallint] NOT NULL,
	[Active] [tinyint] NOT NULL,
 CONSTRAINT [PK_PreservationSchedule_1] PRIMARY KEY CLUSTERED 
(
	[IdPreservationSchedule] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
EXEC dbo.sp_addextendedproperty @name=N'MS_Description', @value=N'es. 5|15|30' , @level0type=N'USER',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PreservationSchedule', @level2type=N'COLUMN',@level2name=N'Period'
GO
EXEC dbo.sp_addextendedproperty @name=N'MS_Description', @value=N'es. 1|2|3|4|5' , @level0type=N'USER',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PreservationSchedule', @level2type=N'COLUMN',@level2name=N'ValidWeekDays'
GO
EXEC dbo.sp_addextendedproperty @name=N'MS_Description', @value=N'0 = Cadenzato, 1 = Giornaliero, 2 = Settimanale, 3 = Mensile' , @level0type=N'USER',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PreservationSchedule', @level2type=N'COLUMN',@level2name=N'FrequencyType'
GO
/****** Object:  Table [dbo].[PreservationRole]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationRole](
	[IdPreservationRole] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Enable] [bit] NOT NULL,
	[AlertEnable] [bit] NOT NULL,
	[KeyCode] [smallint] NOT NULL,
 CONSTRAINT [PK_RoleConservazione] PRIMARY KEY CLUSTERED 
(
	[IdPreservationRole] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationPeriod]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationPeriod](
	[IdPreservationPeriod] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Periods] [varchar](255) NOT NULL,
	[DayofWeek] [varchar](255) NOT NULL,
	[Enable] [smallint] NOT NULL,
 CONSTRAINT [PK_PreservationPeriod] PRIMARY KEY CLUSTERED 
(
	[IdPreservationPeriod] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StorageType]    Script Date: 10/17/2011 10:57:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StorageType](
	[Type] [varchar](20) NULL,
	[IdStorageType] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[StorageAssembly] [varchar](255) NOT NULL,
	[StorageClassName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_StorageType] PRIMARY KEY CLUSTERED 
(
	[IdStorageType] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StorageStatus]    Script Date: 10/17/2011 10:57:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StorageStatus](
	[Status] [varchar](255) NULL,
	[IdStorageStatus] [smallint] NOT NULL,
 CONSTRAINT [PK_StorageStatus] PRIMARY KEY CLUSTERED 
(
	[IdStorageStatus] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[_PreservationAlertRole]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[_PreservationAlertRole](
	[IdPreservationAlertRole] [uniqueidentifier] NOT NULL,
	[IdPreservationRole] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_AlertRole] PRIMARY KEY CLUSTERED 
(
	[IdPreservationAlertRole] ASC,
	[IdPreservationRole] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PreservationTaskStatus]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationTaskStatus](
	[IdPreservationTaskStatus] [uniqueidentifier] NOT NULL,
	[Status] [varchar](50) NOT NULL,
 CONSTRAINT [PK_PreservationTaskStatus] PRIMARY KEY CLUSTERED 
(
	[IdPreservationTaskStatus] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationStorageDeviceStatus]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationStorageDeviceStatus](
	[IdPreservationStorageDeviceStatus] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[KeyCode] [varchar](100) NOT NULL,
	[Value] [varchar](255) NOT NULL,
 CONSTRAINT [PK_PreservationStorageDeviceStatus] PRIMARY KEY CLUSTERED 
(
	[IdPreservationStorageDeviceStatus] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationTaskGroupType]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationTaskGroupType](
	[IdPreservationTaskGroupType] [uniqueidentifier] NOT NULL,
	[Description] [varchar](255) NOT NULL,
 CONSTRAINT [PK_PreservationTaskGroupType_1] PRIMARY KEY CLUSTERED 
(
	[IdPreservationTaskGroupType] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationUser]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationUser](
	[IdPreservationUser] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Surname] [varchar](255) NOT NULL,
	[FiscalId] [char](16) NOT NULL,
	[Address] [varchar](255) NOT NULL,
	[Email] [varchar](255) NOT NULL,
	[Enable] [bit] NOT NULL,
	[DomainUser] [varchar](255) NOT NULL,
 CONSTRAINT [PK_PreservationUser] PRIMARY KEY CLUSTERED 
(
	[IdPreservationUser] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RuleOperator]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RuleOperator](
	[IdRuleOperator] [int] NOT NULL,
	[Descrizione] [varchar](50) NULL,
 CONSTRAINT [PK_RuleOperator] PRIMARY KEY CLUSTERED 
(
	[IdRuleOperator] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Storage]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Storage](
	[IdStorage] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[IdStorageType] [uniqueidentifier] NOT NULL,
	[MainPath] [varchar](255) NULL,
	[Name] [varchar](255) NULL,
	[StorageRuleAssembly] [varchar](255) NULL,
	[StorageRuleClassName] [varchar](255) NULL,
	[Priority] [int] NULL,
	[EnableFulText] [smallint] NOT NULL,
	[AuthenticationKey] [varchar](250) NULL,
	[AuthenticationPassword] [varchar](250) NULL,
	[IsVisible] [smallint] NULL,
 CONSTRAINT [PK_Storage] PRIMARY KEY CLUSTERED 
(
	[IdStorage] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationUserRole]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreservationUserRole](
	[IdPreservationUserRole] [uniqueidentifier] NOT NULL,
	[IdPreservationRole] [uniqueidentifier] NOT NULL,
	[IdPreservationUser] [uniqueidentifier] NOT NULL,
	[IdArchive] [uniqueidentifier] NULL,
 CONSTRAINT [PK_PreservationUserRole] PRIMARY KEY CLUSTERED 
(
	[IdPreservationUserRole] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PreservationTaskType]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationTaskType](
	[IdPreservationTaskType] [uniqueidentifier] NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[Period] [smallint] NOT NULL,
	[IdPreservationPeriod] [uniqueidentifier] NULL,
 CONSTRAINT [PK_PreservationTaskType] PRIMARY KEY CLUSTERED 
(
	[IdPreservationTaskType] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationTaskGroup]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationTaskGroup](
	[IdPreservationTaskGroup] [uniqueidentifier] NOT NULL,
	[IdPreservationTaskGroupType] [uniqueidentifier] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[IdPreservationUser] [uniqueidentifier] NOT NULL,
	[IdPreservationSchedule] [uniqueidentifier] NOT NULL,
	[Expiry] [datetime] NOT NULL,
	[EstimatedExpiry] [datetime] NULL,
	[Closed] [datetime] NULL,
	[IdCompatibility] [int] NULL,
 CONSTRAINT [PK_PreservationTaskGroup] PRIMARY KEY CLUSTERED 
(
	[IdPreservationTaskGroup] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationStorageDevice]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationStorageDevice](
	[IdPreservationStorageDevice] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[IdPreservationStorageDeviceOriginal] [uniqueidentifier] NULL,
	[IdPreservationStorageDeviceStatus] [uniqueidentifier] NOT NULL,
	[IdArchive] [uniqueidentifier] NOT NULL,
	[Label] [varchar](255) NOT NULL,
	[Location] [varchar](255) NULL,
	[DateStorageDevice] [datetime] NULL,
	[DateCreated] [datetime] NULL,
	[DomainUser] [varchar](255) NOT NULL,
	[LastVerifyDate] [datetime] NULL,
 CONSTRAINT [PK_PreservationStorageDevice] PRIMARY KEY CLUSTERED 
(
	[IdPreservationStorageDevice] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationParameters]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationParameters](
	[IdArchive] [uniqueidentifier] NOT NULL,
	[Label] [varchar](50) NOT NULL,
	[Value] [varchar](255) NOT NULL,
 CONSTRAINT [PK_PreservationParameters] PRIMARY KEY CLUSTERED 
(
	[IdArchive] ASC,
	[Label] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationException]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationException](
	[IdPreservationException] [uniqueidentifier] NOT NULL,
	[IdPreservationExceptionType] [uniqueidentifier] NULL,
	[IdPreservationExceptionCorrelated] [uniqueidentifier] NULL,
	[KeyName] [varchar](50) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[IsBlocked] [bit] NULL,
	[IdCompatibility] [int] NULL,
 CONSTRAINT [PK_PreservationException] PRIMARY KEY CLUSTERED 
(
	[IdPreservationException] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationCompany]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationCompany](
	[IdArchive] [uniqueidentifier] NOT NULL,
	[RagioneSociale] [varchar](255) NOT NULL,
	[Indirizzo] [varchar](255) NOT NULL,
	[PIVA] [varchar](50) NOT NULL,
	[NomeEsteso] [varchar](255) NOT NULL,
	[TemplateFileChiusura] [varchar](255) NOT NULL,
	[TipoArchivio] [varchar](255) NOT NULL,
	[CF] [varchar](255) NULL,
	[PostaCertificata] [varchar](255) NULL,
 CONSTRAINT [PK_ParameterConservazione] PRIMARY KEY CLUSTERED 
(
	[IdArchive] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationAlertType]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationAlertType](
	[IdPreservationAlertType] [uniqueidentifier] NOT NULL,
	[IdPreservationRole] [uniqueidentifier] NOT NULL,
	[IdPreservationConsole] [int] NULL,
	[AlertText] [varchar](255) NOT NULL,
	[Offset] [smallint] NOT NULL,
 CONSTRAINT [PK_AlertType] PRIMARY KEY CLUSTERED 
(
	[IdPreservationAlertType] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[_PreservationObjectType]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[_PreservationObjectType](
	[IdPreservationObjectType] [_Id_] NOT NULL,
	[Description] [_Stringa_] NULL,
 CONSTRAINT [PK_PreservationObjectType] PRIMARY KEY CLUSTERED 
(
	[IdPreservationObjectType] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Attributes]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Attributes](
	[IdAttribute] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[IdArchive] [uniqueidentifier] NOT NULL,
	[IsRequired] [smallint] NOT NULL,
	[KeyOrder] [smallint] NULL,
	[IdMode] [int] NOT NULL,
	[IsMainDate] [smallint] NULL,
	[IsEnumerator] [smallint] NULL,
	[IsAutoInc] [smallint] NULL,
	[IsUnique] [smallint] NULL,
	[AttributeType] [varchar](255) NOT NULL,
	[ConservationPosition] [smallint] NULL,
	[DefaultValue] [varchar](255) NULL,
	[MaxLenght] [int] NULL,
	[KeyFilter] [varchar](255) NULL,
	[KeyFormat] [varchar](255) NULL,
	[Validation] [varchar](255) NULL,
	[Format] [varchar](255) NULL,
	[IsChainAttribute] [smallint] NULL,
	[IdAttributeGroup] [uniqueidentifier] NULL,
	[IsVisible] [smallint] NULL,
 CONSTRAINT [PK_Attributes_1] PRIMARY KEY CLUSTERED 
(
	[IdAttribute] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ArchiveStorage]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ArchiveStorage](
	[IdArchive] [uniqueidentifier] NOT NULL,
	[IdStorage] [uniqueidentifier] NOT NULL,
	[Active] [smallint] NOT NULL,
 CONSTRAINT [PK_ArchiveStorage] PRIMARY KEY CLUSTERED 
(
	[IdArchive] ASC,
	[IdStorage] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PreservationAlertTask]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreservationAlertTask](
	[IdPreservationAlertType] [uniqueidentifier] NOT NULL,
	[IdPreservationTaskType] [uniqueidentifier] NOT NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_AlertTask] PRIMARY KEY CLUSTERED 
(
	[IdPreservationAlertType] ASC,
	[IdPreservationTaskType] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PreservationTaskRole]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreservationTaskRole](
	[IdPreservationTaskType] [uniqueidentifier] NOT NULL,
	[IdPreservationRole] [uniqueidentifier] NOT NULL,
	[CreationDate] [datetime] NULL,
 CONSTRAINT [PK_TaskRole] PRIMARY KEY CLUSTERED 
(
	[IdPreservationTaskType] ASC,
	[IdPreservationRole] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PreservationSchedule_TaskType]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreservationSchedule_TaskType](
	[IdPreservationSchedule] [uniqueidentifier] NOT NULL,
	[IdPreservationTaskType] [uniqueidentifier] NOT NULL,
	[Offset] [smallint] NULL,
 CONSTRAINT [PK_PreservationSchedule_TaskType] PRIMARY KEY CLUSTERED 
(
	[IdPreservationSchedule] ASC,
	[IdPreservationTaskType] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PreservationTask]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationTask](
	[IdPreservationTask] [uniqueidentifier] NOT NULL,
	[IdPreservationConsole] [int] NULL,
	[EstimatedDate] [datetime] NOT NULL,
	[ExecutedDate] [datetime] NULL,
	[StartDocumentDate] [datetime] NULL,
	[EndDocumentDate] [datetime] NULL,
	[IdPreservationTaskType] [uniqueidentifier] NOT NULL,
	[IdArchive] [uniqueidentifier] NOT NULL,
	[IdPreservationTaskStatus] [uniqueidentifier] NULL,
	[IdPreservationUser] [uniqueidentifier] NULL,
	[IdPreservationTaskGroup] [uniqueidentifier] NOT NULL,
	[Name] [varchar](250) NULL,
 CONSTRAINT [PK_TaskConservazione] PRIMARY KEY CLUSTERED 
(
	[IdPreservationTask] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StorageArea]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StorageArea](
	[IdStorageArea] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[IdStorage] [uniqueidentifier] NOT NULL,
	[Path] [varchar](255) NULL,
	[Name] [varchar](255) NOT NULL,
	[Priority] [int] NULL,
	[IdStorageStatus] [smallint] NULL,
	[MaxSize] [bigint] NOT NULL,
	[CurrentSize] [bigint] NULL,
	[MaxFileNumber] [bigint] NOT NULL,
	[CurrentFileNumber] [bigint] NULL,
	[Enable] [smallint] NOT NULL,
 CONSTRAINT [PK_StorageArea] PRIMARY KEY CLUSTERED 
(
	[IdStorageArea] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StorageRule]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StorageRule](
	[IdStorage] [uniqueidentifier] NOT NULL,
	[IdAttribute] [uniqueidentifier] NOT NULL,
	[RuleOrder] [smallint] NOT NULL,
	[RuleFormat] [varchar](255) NULL,
	[RuleFilter] [varchar](255) NOT NULL,
	[IdRuleOperator] [int] NULL,
 CONSTRAINT [PK_StorageRule] PRIMARY KEY CLUSTERED 
(
	[IdStorage] ASC,
	[IdAttribute] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StorageAreaRule]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StorageAreaRule](
	[IdAttribute] [uniqueidentifier] NOT NULL,
	[RuleOrder] [smallint] NOT NULL,
	[RuleFormat] [varchar](255) NULL,
	[IdRuleOperator] [int] NULL,
	[RuleFilter] [varchar](255) NULL,
	[IdStorageArea] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_StorageAreaRule_1] PRIMARY KEY CLUSTERED 
(
	[IdAttribute] ASC,
	[IdStorageArea] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationAlert]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PreservationAlert](
	[IdPreservationAlert] [uniqueidentifier] NOT NULL,
	[IdPreservationAlertType] [uniqueidentifier] NOT NULL,
	[IdPreservationTask] [uniqueidentifier] NOT NULL,
	[MadeDate] [datetime] NULL,
	[AlertDate] [datetime] NULL,
	[ForwardFrequency] [tinyint] NULL,
 CONSTRAINT [PK_PreservationAlert] PRIMARY KEY CLUSTERED 
(
	[IdPreservationAlert] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
EXEC dbo.sp_addextendedproperty @name=N'MS_Description', @value=N'Data in cui è stato effettuata l''operazione' , @level0type=N'USER',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PreservationAlert', @level2type=N'COLUMN',@level2name=N'MadeDate'
GO
/****** Object:  Table [dbo].[Preservation]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Preservation](
	[IdPreservation] [uniqueidentifier] NOT NULL,
	[IdArchive] [uniqueidentifier] NOT NULL,
	[IdPreservationTaskGroup] [uniqueidentifier] NOT NULL,
	[IdPreservationTask] [uniqueidentifier] NULL,
	[Path] [varchar](255) NULL,
	[Label] [varchar](255) NULL,
	[PreservationDate] [datetime] NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[CloseDate] [datetime] NULL,
	[IndexHash] [varchar](255) NULL,
	[CloseContent] [image] NULL,
	[LastVerifiedDate] [datetime] NULL,
	[IdPreservationUser] [uniqueidentifier] NULL,
	[IdCompatibility] [int] NULL,
 CONSTRAINT [PK_Conservazione_1] PRIMARY KEY CLUSTERED 
(
	[IdPreservation] ASC
) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Document]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Document](
	[IdBiblos] [int] NOT NULL,
	[IdDocument] [uniqueidentifier] NOT NULL,
	[IdParentBiblos] [uniqueidentifier] NULL,
	[IdStorageArea] [uniqueidentifier] NULL,
	[IdStorage] [uniqueidentifier] NULL,
	[IdArchive] [uniqueidentifier] NOT NULL,
	[ChainOrder] [int] NOT NULL,
	[StorageVersion] [decimal](18, 6) NULL,
	[Version] [decimal](18, 6) NOT NULL,
	[IdParentVersion] [uniqueidentifier] NULL,
	[IdDocumentLink] [uniqueidentifier] NULL,
	[IdCertificate] [uniqueidentifier] NULL,
	[SignHeader] [varchar](255) NULL,
	[FullSign] [varchar](255) NULL,
	[DocumentHash] [varchar](255) NULL,
	[IsLinked] [smallint] NULL,
	[IsVisible] [smallint] NOT NULL,
	[IsConservated] [smallint] NULL,
	[DateExpire] [datetime] NULL,
	[DateCreated] [datetime] NULL,
	[Name] [varchar](255) NULL,
	[Size] [bigint] NULL,
	[IdNodeType] [smallint] NULL,
	[IsConfirmed] [smallint] NULL,
	[IdDocumentStatus] [smallint] NOT NULL,
	[IsCheckOut] [smallint] NOT NULL,
	[DateMain] [datetime] NULL,
	[IdPreservation] [uniqueidentifier] NULL,
	[IsDetached] [bit] NULL,
	[IdUserCheckOut] [varchar](250) NULL,
	[PrimaryKeyValue] [varchar](250) NULL,
	[PrimaryKeyValueIndex]  AS (case when [PrimaryKeyValue] IS NULL OR [PrimaryKeyValue]='' then CONVERT([nvarchar](250),[IdDocument],(0)) else [PrimaryKeyValue] end),
	[IdPreservationException] [uniqueidentifier] NULL,
	[PreservationIndex] [bigint] NULL,
	[IdThumbnail] [varchar](250) NULL,
	[IdPdf] [varchar](250) NULL,
 CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED 
(
	[IdDocument] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationJournaling]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationJournaling](
	[IdPreservationJournaling] [uniqueidentifier] NOT NULL,
	[IdPreservation] [uniqueidentifier] NULL,
	[IdPreservationJournalingActivity] [uniqueidentifier] NOT NULL,
	[DateCreated] [datetime] NULL,
	[DateActivity] [datetime] NULL,
	[IdPreservationUser] [uniqueidentifier] NOT NULL,
	[Notes] [text] NULL,
	[DomainUser] [varchar](250) NOT NULL,
 CONSTRAINT [PK_PreservationJournaling] PRIMARY KEY CLUSTERED 
(
	[IdPreservationJournaling] ASC
) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PreservationInStorageDevice]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PreservationInStorageDevice](
	[IdPreservation] [uniqueidentifier] NOT NULL,
	[IdPreservationStorageDevice] [uniqueidentifier] NOT NULL,
	[Path] [varchar](250) NULL,
 CONSTRAINT [PK_PreservationInStorageDevice] PRIMARY KEY CLUSTERED 
(
	[IdPreservation] ASC,
	[IdPreservationStorageDevice] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Transito]    Script Date: 10/17/2011 10:57:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Transito](
	[LocalPath] [varchar](255) NULL,
	[IdDocument] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Status] [smallint] NULL,
	[Retry] [int] NOT NULL,
	[DateRetry] [datetime] NULL,
	[DateCreated] [datetime] NULL,
 CONSTRAINT [PK_Transito] PRIMARY KEY CLUSTERED 
(
	[IdDocument] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Permission]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Permission](
	[PermissionName] [varchar](255) NOT NULL,
	[IdDocument] [uniqueidentifier] NOT NULL,
	[IdMode] [smallint] NOT NULL,
	[IsGroup] [smallint] NULL,
 CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED 
(
	[PermissionName] ASC,
	[IdDocument] ASC,
	[IdMode] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExceptionConservazione]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExceptionConservazione](
	[IdExceptionType] [uniqueidentifier] NOT NULL,
	[IdConservazione] [uniqueidentifier] NOT NULL,
	[IdDocument] [uniqueidentifier] NOT NULL,
	[DateException] [datetime] NULL,
 CONSTRAINT [PK_ExceptionConservazione_1] PRIMARY KEY CLUSTERED 
(
	[IdExceptionType] ASC,
	[IdConservazione] ASC,
	[IdDocument] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cache]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Cache](
	[LocalPath] [varchar](255) NULL,
	[Score] [int] NULL,
	[IdDocument] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
 CONSTRAINT [PK_Cache] PRIMARY KEY CLUSTERED 
(
	[IdDocument] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AttributesValue]    Script Date: 10/17/2011 10:57:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AttributesValue](
	[IdDocument] [uniqueidentifier] NOT NULL,
	[IdAttribute] [uniqueidentifier] NOT NULL,
	[ValueInt] [bigint] NULL,
	[ValueFloat] [float] NULL,
	[ValueDateTime] [datetime] NULL,
	[ValueString] [varchar](8000) NULL,
 CONSTRAINT [PK_AttributesValue_1] PRIMARY KEY NONCLUSTERED 
(
	[IdDocument] ASC,
	[IdAttribute] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Default [DF_PreservationAlertTask_Enabled]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationAlertTask] ADD  CONSTRAINT [DF_PreservationAlertTask_Enabled]  DEFAULT ((1)) FOR [Enabled]
GO
/****** Object:  Default [DF_PreservationJournalingActivity_IdPreservationJournalingActivity]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationJournalingActivity] ADD  CONSTRAINT [DF_PreservationJournalingActivity_IdPreservationJournalingActivity]  DEFAULT (newid()) FOR [IdPreservationJournalingActivity]
GO
/****** Object:  Default [DF_PreservationSchedule_FrequencyType]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationSchedule] ADD  CONSTRAINT [DF_PreservationSchedule_FrequencyType]  DEFAULT ((1)) FOR [FrequencyType]
GO
/****** Object:  Default [DF_PreservationSchedule_Active]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationSchedule] ADD  CONSTRAINT [DF_PreservationSchedule_Active]  DEFAULT ((1)) FOR [Active]
GO
/****** Object:  Default [DF_PreservationStorageDevice_IdPreservationStorageDevice]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationStorageDevice] ADD  CONSTRAINT [DF_PreservationStorageDevice_IdPreservationStorageDevice]  DEFAULT (newid()) FOR [IdPreservationStorageDevice]
GO
/****** Object:  Default [DF_PreservationStorageDeviceStatus_IdPreservationStorageDeviceStatus]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationStorageDeviceStatus] ADD  CONSTRAINT [DF_PreservationStorageDeviceStatus_IdPreservationStorageDeviceStatus]  DEFAULT (newid()) FOR [IdPreservationStorageDeviceStatus]
GO
/****** Object:  Default [DF_Transito_DateCreated]    Script Date: 10/17/2011 10:57:49 ******/
ALTER TABLE [dbo].[Transito] ADD  CONSTRAINT [DF_Transito_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  ForeignKey [FK_ArchiveStorage_Archive]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[ArchiveStorage]  WITH CHECK ADD  CONSTRAINT [FK_ArchiveStorage_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[ArchiveStorage] CHECK CONSTRAINT [FK_ArchiveStorage_Archive]
GO
/****** Object:  ForeignKey [FK_ArchiveStorage_Storage]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[ArchiveStorage]  WITH CHECK ADD  CONSTRAINT [FK_ArchiveStorage_Storage] FOREIGN KEY([IdStorage])
REFERENCES [dbo].[Storage] ([IdStorage])
GO
ALTER TABLE [dbo].[ArchiveStorage] CHECK CONSTRAINT [FK_ArchiveStorage_Storage]
GO
/****** Object:  ForeignKey [FK_Attributes_Archive]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Attributes]  WITH CHECK ADD  CONSTRAINT [FK_Attributes_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[Attributes] CHECK CONSTRAINT [FK_Attributes_Archive]
GO
/****** Object:  ForeignKey [FK_Attributes_AttributeGroup]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Attributes]  WITH CHECK ADD  CONSTRAINT [FK_Attributes_AttributeGroup] FOREIGN KEY([IdAttributeGroup])
REFERENCES [dbo].[AttributesGroup] ([IdAttributeGroup])
GO
ALTER TABLE [dbo].[Attributes] CHECK CONSTRAINT [FK_Attributes_AttributeGroup]
GO
/****** Object:  ForeignKey [FK_Attributes_AttributesMode]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Attributes]  WITH CHECK ADD  CONSTRAINT [FK_Attributes_AttributesMode] FOREIGN KEY([IdMode])
REFERENCES [dbo].[AttributesMode] ([IdMode])
GO
ALTER TABLE [dbo].[Attributes] CHECK CONSTRAINT [FK_Attributes_AttributesMode]
GO
/****** Object:  ForeignKey [FK_AttributesValue_Attributes]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[AttributesValue]  WITH CHECK ADD  CONSTRAINT [FK_AttributesValue_Attributes] FOREIGN KEY([IdAttribute])
REFERENCES [dbo].[Attributes] ([IdAttribute])
GO
ALTER TABLE [dbo].[AttributesValue] CHECK CONSTRAINT [FK_AttributesValue_Attributes]
GO
/****** Object:  ForeignKey [FK_AttributesValue_Document]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[AttributesValue]  WITH CHECK ADD  CONSTRAINT [FK_AttributesValue_Document] FOREIGN KEY([IdDocument])
REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[AttributesValue] CHECK CONSTRAINT [FK_AttributesValue_Document]
GO
/****** Object:  ForeignKey [FK_Cache_Document]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Cache]  WITH CHECK ADD  CONSTRAINT [FK_Cache_Document] FOREIGN KEY([IdDocument])
REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[Cache] CHECK CONSTRAINT [FK_Cache_Document]
GO
/****** Object:  ForeignKey [FK_Document_Archive]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_Archive]
GO
/****** Object:  ForeignKey [FK_Document_CertificateStore]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_CertificateStore] FOREIGN KEY([IdCertificate])
REFERENCES [dbo].[CertificateStore] ([IdCertificate])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_CertificateStore]
GO
/****** Object:  ForeignKey [FK_Document_Document]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_Document] FOREIGN KEY([IdParentBiblos])
REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_Document]
GO
/****** Object:  ForeignKey [FK_Document_Document1]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_Document1] FOREIGN KEY([IdDocumentLink])
REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_Document1]
GO
/****** Object:  ForeignKey [FK_Document_DocumentNodeType]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_DocumentNodeType] FOREIGN KEY([IdNodeType])
REFERENCES [dbo].[DocumentNodeType] ([Id])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_DocumentNodeType]
GO
/****** Object:  ForeignKey [FK_Document_DocumentParentVersion]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_DocumentParentVersion] FOREIGN KEY([IdParentVersion])
REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_DocumentParentVersion]
GO
/****** Object:  ForeignKey [FK_Document_DocumentStatus]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_DocumentStatus] FOREIGN KEY([IdDocumentStatus])
REFERENCES [dbo].[DocumentStatus] ([IdDocumentStatus])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_DocumentStatus]
GO
/****** Object:  ForeignKey [FK_Document_Preservation]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_Preservation] FOREIGN KEY([IdPreservation])
REFERENCES [dbo].[Preservation] ([IdPreservation])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_Preservation]
GO
/****** Object:  ForeignKey [FK_Document_PreservationException]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_PreservationException] FOREIGN KEY([IdPreservationException])
REFERENCES [dbo].[PreservationException] ([IdPreservationException])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_PreservationException]
GO
/****** Object:  ForeignKey [FK_Document_Storage]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_Storage] FOREIGN KEY([IdStorage])
REFERENCES [dbo].[Storage] ([IdStorage])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_Storage]
GO
/****** Object:  ForeignKey [FK_Document_StorageArea]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Document]  WITH CHECK ADD  CONSTRAINT [FK_Document_StorageArea] FOREIGN KEY([IdStorageArea])
REFERENCES [dbo].[StorageArea] ([IdStorageArea])
GO
ALTER TABLE [dbo].[Document] CHECK CONSTRAINT [FK_Document_StorageArea]
GO
/****** Object:  ForeignKey [FK_ExceptionConservazione_Conservazione]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[ExceptionConservazione]  WITH CHECK ADD  CONSTRAINT [FK_ExceptionConservazione_Conservazione] FOREIGN KEY([IdConservazione])
REFERENCES [dbo].[Preservation] ([IdPreservation])
GO
ALTER TABLE [dbo].[ExceptionConservazione] CHECK CONSTRAINT [FK_ExceptionConservazione_Conservazione]
GO
/****** Object:  ForeignKey [FK_ExceptionConservazione_Document]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[ExceptionConservazione]  WITH CHECK ADD  CONSTRAINT [FK_ExceptionConservazione_Document] FOREIGN KEY([IdDocument])
REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[ExceptionConservazione] CHECK CONSTRAINT [FK_ExceptionConservazione_Document]
GO
/****** Object:  ForeignKey [FK_ExceptionConservazione_ExceptionType]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[ExceptionConservazione]  WITH CHECK ADD  CONSTRAINT [FK_ExceptionConservazione_ExceptionType] FOREIGN KEY([IdExceptionType])
REFERENCES [dbo].[PreservationExceptionType] ([IdPreservationExceptionType])
GO
ALTER TABLE [dbo].[ExceptionConservazione] CHECK CONSTRAINT [FK_ExceptionConservazione_ExceptionType]
GO
/****** Object:  ForeignKey [FK_Permission_Document]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Permission]  WITH CHECK ADD  CONSTRAINT [FK_Permission_Document] FOREIGN KEY([IdDocument])
REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[Permission] CHECK CONSTRAINT [FK_Permission_Document]
GO
/****** Object:  ForeignKey [FK_Permission_PermissionMode]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Permission]  WITH CHECK ADD  CONSTRAINT [FK_Permission_PermissionMode] FOREIGN KEY([IdMode])
REFERENCES [dbo].[PermissionMode] ([IdMode])
GO
ALTER TABLE [dbo].[Permission] CHECK CONSTRAINT [FK_Permission_PermissionMode]
GO
/****** Object:  ForeignKey [FK_Conservazione_Archive]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Preservation]  WITH CHECK ADD  CONSTRAINT [FK_Conservazione_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[Preservation] CHECK CONSTRAINT [FK_Conservazione_Archive]
GO
/****** Object:  ForeignKey [FK_Preservation_PreservationTask]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Preservation]  WITH CHECK ADD  CONSTRAINT [FK_Preservation_PreservationTask] FOREIGN KEY([IdPreservationTask])
REFERENCES [dbo].[PreservationTask] ([IdPreservationTask])
GO
ALTER TABLE [dbo].[Preservation] CHECK CONSTRAINT [FK_Preservation_PreservationTask]
GO
/****** Object:  ForeignKey [FK_Preservation_PreservationTaskGroup]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Preservation]  WITH CHECK ADD  CONSTRAINT [FK_Preservation_PreservationTaskGroup] FOREIGN KEY([IdPreservationTaskGroup])
REFERENCES [dbo].[PreservationTaskGroup] ([IdPreservationTaskGroup])
GO
ALTER TABLE [dbo].[Preservation] CHECK CONSTRAINT [FK_Preservation_PreservationTaskGroup]
GO
/****** Object:  ForeignKey [FK_Preservation_PreservationUser]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Preservation]  WITH CHECK ADD  CONSTRAINT [FK_Preservation_PreservationUser] FOREIGN KEY([IdPreservationUser])
REFERENCES [dbo].[PreservationUser] ([IdPreservationUser])
GO
ALTER TABLE [dbo].[Preservation] CHECK CONSTRAINT [FK_Preservation_PreservationUser]
GO
/****** Object:  ForeignKey [FK_PreservationAlert_PreservationAlertType]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationAlert]  WITH CHECK ADD  CONSTRAINT [FK_PreservationAlert_PreservationAlertType] FOREIGN KEY([IdPreservationAlertType])
REFERENCES [dbo].[PreservationAlertType] ([IdPreservationAlertType])
GO
ALTER TABLE [dbo].[PreservationAlert] CHECK CONSTRAINT [FK_PreservationAlert_PreservationAlertType]
GO
/****** Object:  ForeignKey [FK_PreservationAlert_PreservationTask]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationAlert]  WITH CHECK ADD  CONSTRAINT [FK_PreservationAlert_PreservationTask] FOREIGN KEY([IdPreservationTask])
REFERENCES [dbo].[PreservationTask] ([IdPreservationTask])
GO
ALTER TABLE [dbo].[PreservationAlert] CHECK CONSTRAINT [FK_PreservationAlert_PreservationTask]
GO
/****** Object:  ForeignKey [FK_AlertTask_AlertType]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationAlertTask]  WITH CHECK ADD  CONSTRAINT [FK_AlertTask_AlertType] FOREIGN KEY([IdPreservationAlertType])
REFERENCES [dbo].[PreservationAlertType] ([IdPreservationAlertType])
GO
ALTER TABLE [dbo].[PreservationAlertTask] CHECK CONSTRAINT [FK_AlertTask_AlertType]
GO
/****** Object:  ForeignKey [FK_PreservationAlertTask_PreservationTaskType]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationAlertTask]  WITH CHECK ADD  CONSTRAINT [FK_PreservationAlertTask_PreservationTaskType] FOREIGN KEY([IdPreservationTaskType])
REFERENCES [dbo].[PreservationTaskType] ([IdPreservationTaskType])
GO
ALTER TABLE [dbo].[PreservationAlertTask] CHECK CONSTRAINT [FK_PreservationAlertTask_PreservationTaskType]
GO
/****** Object:  ForeignKey [FK_PreservationAlertType_PreservationRole]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationAlertType]  WITH CHECK ADD  CONSTRAINT [FK_PreservationAlertType_PreservationRole] FOREIGN KEY([IdPreservationRole])
REFERENCES [dbo].[PreservationRole] ([IdPreservationRole])
GO
ALTER TABLE [dbo].[PreservationAlertType] CHECK CONSTRAINT [FK_PreservationAlertType_PreservationRole]
GO
/****** Object:  ForeignKey [FK_AziendaConservazione_Archive]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationCompany]  WITH CHECK ADD  CONSTRAINT [FK_AziendaConservazione_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[PreservationCompany] CHECK CONSTRAINT [FK_AziendaConservazione_Archive]
GO
/****** Object:  ForeignKey [FK_PreservationException_PreservationExceptionType]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationException]  WITH CHECK ADD  CONSTRAINT [FK_PreservationException_PreservationExceptionType] FOREIGN KEY([IdPreservationExceptionType])
REFERENCES [dbo].[PreservationExceptionType] ([IdPreservationExceptionType])
GO
ALTER TABLE [dbo].[PreservationException] CHECK CONSTRAINT [FK_PreservationException_PreservationExceptionType]
GO
/****** Object:  ForeignKey [FK_PreservationInStorageDevice_Preservation]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationInStorageDevice]  WITH CHECK ADD  CONSTRAINT [FK_PreservationInStorageDevice_Preservation] FOREIGN KEY([IdPreservation])
REFERENCES [dbo].[Preservation] ([IdPreservation])
GO
ALTER TABLE [dbo].[PreservationInStorageDevice] CHECK CONSTRAINT [FK_PreservationInStorageDevice_Preservation]
GO
/****** Object:  ForeignKey [FK_PreservationInStorageDevice_PreservationStorageDevice]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationInStorageDevice]  WITH CHECK ADD  CONSTRAINT [FK_PreservationInStorageDevice_PreservationStorageDevice] FOREIGN KEY([IdPreservationStorageDevice])
REFERENCES [dbo].[PreservationStorageDevice] ([IdPreservationStorageDevice])
GO
ALTER TABLE [dbo].[PreservationInStorageDevice] CHECK CONSTRAINT [FK_PreservationInStorageDevice_PreservationStorageDevice]
GO
/****** Object:  ForeignKey [FK_PreservationJournaling_Preservation]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationJournaling]  WITH CHECK ADD  CONSTRAINT [FK_PreservationJournaling_Preservation] FOREIGN KEY([IdPreservation])
REFERENCES [dbo].[Preservation] ([IdPreservation])
GO
ALTER TABLE [dbo].[PreservationJournaling] CHECK CONSTRAINT [FK_PreservationJournaling_Preservation]
GO
/****** Object:  ForeignKey [FK_PreservationJournaling_PreservationJournalingActivity]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationJournaling]  WITH CHECK ADD  CONSTRAINT [FK_PreservationJournaling_PreservationJournalingActivity] FOREIGN KEY([IdPreservationJournalingActivity])
REFERENCES [dbo].[PreservationJournalingActivity] ([IdPreservationJournalingActivity])
GO
ALTER TABLE [dbo].[PreservationJournaling] CHECK CONSTRAINT [FK_PreservationJournaling_PreservationJournalingActivity]
GO
/****** Object:  ForeignKey [FK_PreservationJournaling_PreservationUser]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationJournaling]  WITH CHECK ADD  CONSTRAINT [FK_PreservationJournaling_PreservationUser] FOREIGN KEY([IdPreservationUser])
REFERENCES [dbo].[PreservationUser] ([IdPreservationUser])
GO
ALTER TABLE [dbo].[PreservationJournaling] CHECK CONSTRAINT [FK_PreservationJournaling_PreservationUser]
GO
/****** Object:  ForeignKey [FK_ParameterConservazione_Archive]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationParameters]  WITH CHECK ADD  CONSTRAINT [FK_ParameterConservazione_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[PreservationParameters] CHECK CONSTRAINT [FK_ParameterConservazione_Archive]
GO
/****** Object:  ForeignKey [FK_PreservationSchedule_TaskType_Schedule]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationSchedule_TaskType]  WITH NOCHECK ADD  CONSTRAINT [FK_PreservationSchedule_TaskType_Schedule] FOREIGN KEY([IdPreservationSchedule])
REFERENCES [dbo].[PreservationSchedule] ([IdPreservationSchedule])
GO
ALTER TABLE [dbo].[PreservationSchedule_TaskType] CHECK CONSTRAINT [FK_PreservationSchedule_TaskType_Schedule]
GO
/****** Object:  ForeignKey [FK_PreservationSchedule_TaskType_TaskType]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationSchedule_TaskType]  WITH NOCHECK ADD  CONSTRAINT [FK_PreservationSchedule_TaskType_TaskType] FOREIGN KEY([IdPreservationTaskType])
REFERENCES [dbo].[PreservationTaskType] ([IdPreservationTaskType])
GO
ALTER TABLE [dbo].[PreservationSchedule_TaskType] CHECK CONSTRAINT [FK_PreservationSchedule_TaskType_TaskType]
GO
/****** Object:  ForeignKey [FK_PreservationStorageDevice_Archive]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationStorageDevice]  WITH CHECK ADD  CONSTRAINT [FK_PreservationStorageDevice_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[PreservationStorageDevice] CHECK CONSTRAINT [FK_PreservationStorageDevice_Archive]
GO
/****** Object:  ForeignKey [FK_PreservationStorageDevice_PreservationStorageDevice]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationStorageDevice]  WITH CHECK ADD  CONSTRAINT [FK_PreservationStorageDevice_PreservationStorageDevice] FOREIGN KEY([IdPreservationStorageDeviceOriginal])
REFERENCES [dbo].[PreservationStorageDevice] ([IdPreservationStorageDevice])
GO
ALTER TABLE [dbo].[PreservationStorageDevice] CHECK CONSTRAINT [FK_PreservationStorageDevice_PreservationStorageDevice]
GO
/****** Object:  ForeignKey [FK_PreservationStorageDevice_PreservationStorageDeviceStatus]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationStorageDevice]  WITH CHECK ADD  CONSTRAINT [FK_PreservationStorageDevice_PreservationStorageDeviceStatus] FOREIGN KEY([IdPreservationStorageDeviceStatus])
REFERENCES [dbo].[PreservationStorageDeviceStatus] ([IdPreservationStorageDeviceStatus])
GO
ALTER TABLE [dbo].[PreservationStorageDevice] CHECK CONSTRAINT [FK_PreservationStorageDevice_PreservationStorageDeviceStatus]
GO
/****** Object:  ForeignKey [FK_PreservationTask_Archive]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationTask]  WITH CHECK ADD  CONSTRAINT [FK_PreservationTask_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[PreservationTask] CHECK CONSTRAINT [FK_PreservationTask_Archive]
GO
/****** Object:  ForeignKey [FK_PreservationTask_PreservationTaskGroup]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationTask]  WITH CHECK ADD  CONSTRAINT [FK_PreservationTask_PreservationTaskGroup] FOREIGN KEY([IdPreservationTaskGroup])
REFERENCES [dbo].[PreservationTaskGroup] ([IdPreservationTaskGroup])
GO
ALTER TABLE [dbo].[PreservationTask] CHECK CONSTRAINT [FK_PreservationTask_PreservationTaskGroup]
GO
/****** Object:  ForeignKey [FK_PreservationTask_PreservationTaskStatus]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationTask]  WITH CHECK ADD  CONSTRAINT [FK_PreservationTask_PreservationTaskStatus] FOREIGN KEY([IdPreservationTaskStatus])
REFERENCES [dbo].[PreservationTaskStatus] ([IdPreservationTaskStatus])
GO
ALTER TABLE [dbo].[PreservationTask] CHECK CONSTRAINT [FK_PreservationTask_PreservationTaskStatus]
GO
/****** Object:  ForeignKey [FK_PreservationTask_PreservationTaskType]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationTask]  WITH CHECK ADD  CONSTRAINT [FK_PreservationTask_PreservationTaskType] FOREIGN KEY([IdPreservationTaskType])
REFERENCES [dbo].[PreservationTaskType] ([IdPreservationTaskType])
GO
ALTER TABLE [dbo].[PreservationTask] CHECK CONSTRAINT [FK_PreservationTask_PreservationTaskType]
GO
/****** Object:  ForeignKey [FK_PreservationTask_PreservationUser]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationTask]  WITH CHECK ADD  CONSTRAINT [FK_PreservationTask_PreservationUser] FOREIGN KEY([IdPreservationUser])
REFERENCES [dbo].[PreservationUser] ([IdPreservationUser])
GO
ALTER TABLE [dbo].[PreservationTask] CHECK CONSTRAINT [FK_PreservationTask_PreservationUser]
GO
/****** Object:  ForeignKey [FK_PreservationTaskGroup_PreservationSchedule]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationTaskGroup]  WITH NOCHECK ADD  CONSTRAINT [FK_PreservationTaskGroup_PreservationSchedule] FOREIGN KEY([IdPreservationSchedule])
REFERENCES [dbo].[PreservationSchedule] ([IdPreservationSchedule])
GO
ALTER TABLE [dbo].[PreservationTaskGroup] CHECK CONSTRAINT [FK_PreservationTaskGroup_PreservationSchedule]
GO
/****** Object:  ForeignKey [FK_PreservationTaskGroup_PreservationTaskGroupType]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationTaskGroup]  WITH NOCHECK ADD  CONSTRAINT [FK_PreservationTaskGroup_PreservationTaskGroupType] FOREIGN KEY([IdPreservationTaskGroupType])
REFERENCES [dbo].[PreservationTaskGroupType] ([IdPreservationTaskGroupType])
GO
ALTER TABLE [dbo].[PreservationTaskGroup] CHECK CONSTRAINT [FK_PreservationTaskGroup_PreservationTaskGroupType]
GO
/****** Object:  ForeignKey [FK_PreservationTaskGroup_PreservationUser]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationTaskGroup]  WITH NOCHECK ADD  CONSTRAINT [FK_PreservationTaskGroup_PreservationUser] FOREIGN KEY([IdPreservationUser])
REFERENCES [dbo].[PreservationUser] ([IdPreservationUser])
GO
ALTER TABLE [dbo].[PreservationTaskGroup] CHECK CONSTRAINT [FK_PreservationTaskGroup_PreservationUser]
GO
/****** Object:  ForeignKey [FK_PreservationTaskRole_PreservationRole]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationTaskRole]  WITH CHECK ADD  CONSTRAINT [FK_PreservationTaskRole_PreservationRole] FOREIGN KEY([IdPreservationRole])
REFERENCES [dbo].[PreservationRole] ([IdPreservationRole])
GO
ALTER TABLE [dbo].[PreservationTaskRole] CHECK CONSTRAINT [FK_PreservationTaskRole_PreservationRole]
GO
/****** Object:  ForeignKey [FK_PreservationTaskRole_PreservationTaskType]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationTaskRole]  WITH CHECK ADD  CONSTRAINT [FK_PreservationTaskRole_PreservationTaskType] FOREIGN KEY([IdPreservationTaskType])
REFERENCES [dbo].[PreservationTaskType] ([IdPreservationTaskType])
GO
ALTER TABLE [dbo].[PreservationTaskRole] CHECK CONSTRAINT [FK_PreservationTaskRole_PreservationTaskType]
GO
/****** Object:  ForeignKey [FK_PreservationTaskType_PreservationPeriod]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationTaskType]  WITH CHECK ADD  CONSTRAINT [FK_PreservationTaskType_PreservationPeriod] FOREIGN KEY([IdPreservationPeriod])
REFERENCES [dbo].[PreservationPeriod] ([IdPreservationPeriod])
GO
ALTER TABLE [dbo].[PreservationTaskType] CHECK CONSTRAINT [FK_PreservationTaskType_PreservationPeriod]
GO
/****** Object:  ForeignKey [FK_PreservationUserRole_PreservationRole]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationUserRole]  WITH CHECK ADD  CONSTRAINT [FK_PreservationUserRole_PreservationRole] FOREIGN KEY([IdPreservationRole])
REFERENCES [dbo].[PreservationRole] ([IdPreservationRole])
GO
ALTER TABLE [dbo].[PreservationUserRole] CHECK CONSTRAINT [FK_PreservationUserRole_PreservationRole]
GO
/****** Object:  ForeignKey [FK_PreservationUserRole_PreservationUser]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationUserRole]  WITH CHECK ADD  CONSTRAINT [FK_PreservationUserRole_PreservationUser] FOREIGN KEY([IdPreservationUser])
REFERENCES [dbo].[PreservationUser] ([IdPreservationUser])
GO
ALTER TABLE [dbo].[PreservationUserRole] CHECK CONSTRAINT [FK_PreservationUserRole_PreservationUser]
GO
/****** Object:  ForeignKey [FK_RoleUserConservazione_Archive]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[PreservationUserRole]  WITH CHECK ADD  CONSTRAINT [FK_RoleUserConservazione_Archive] FOREIGN KEY([IdArchive])
REFERENCES [dbo].[Archive] ([IdArchive])
GO
ALTER TABLE [dbo].[PreservationUserRole] CHECK CONSTRAINT [FK_RoleUserConservazione_Archive]
GO
/****** Object:  ForeignKey [FK_Storage_StorageType]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[Storage]  WITH CHECK ADD  CONSTRAINT [FK_Storage_StorageType] FOREIGN KEY([IdStorageType])
REFERENCES [dbo].[StorageType] ([IdStorageType])
GO
ALTER TABLE [dbo].[Storage] CHECK CONSTRAINT [FK_Storage_StorageType]
GO
/****** Object:  ForeignKey [FK_StorageArea_Storage]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[StorageArea]  WITH CHECK ADD  CONSTRAINT [FK_StorageArea_Storage] FOREIGN KEY([IdStorage])
REFERENCES [dbo].[Storage] ([IdStorage])
GO
ALTER TABLE [dbo].[StorageArea] CHECK CONSTRAINT [FK_StorageArea_Storage]
GO
/****** Object:  ForeignKey [FK_StorageArea_StorageStatus]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[StorageArea]  WITH CHECK ADD  CONSTRAINT [FK_StorageArea_StorageStatus] FOREIGN KEY([IdStorageStatus])
REFERENCES [dbo].[StorageStatus] ([IdStorageStatus])
GO
ALTER TABLE [dbo].[StorageArea] CHECK CONSTRAINT [FK_StorageArea_StorageStatus]
GO
/****** Object:  ForeignKey [FK_StorageAreaRule_Attributes]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[StorageAreaRule]  WITH CHECK ADD  CONSTRAINT [FK_StorageAreaRule_Attributes] FOREIGN KEY([IdAttribute])
REFERENCES [dbo].[Attributes] ([IdAttribute])
GO
ALTER TABLE [dbo].[StorageAreaRule] CHECK CONSTRAINT [FK_StorageAreaRule_Attributes]
GO
/****** Object:  ForeignKey [FK_StorageAreaRule_RuleOperator]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[StorageAreaRule]  WITH CHECK ADD  CONSTRAINT [FK_StorageAreaRule_RuleOperator] FOREIGN KEY([IdRuleOperator])
REFERENCES [dbo].[RuleOperator] ([IdRuleOperator])
GO
ALTER TABLE [dbo].[StorageAreaRule] CHECK CONSTRAINT [FK_StorageAreaRule_RuleOperator]
GO
/****** Object:  ForeignKey [FK_StorageAreaRule_StorageArea]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[StorageAreaRule]  WITH CHECK ADD  CONSTRAINT [FK_StorageAreaRule_StorageArea] FOREIGN KEY([IdStorageArea])
REFERENCES [dbo].[StorageArea] ([IdStorageArea])
GO
ALTER TABLE [dbo].[StorageAreaRule] CHECK CONSTRAINT [FK_StorageAreaRule_StorageArea]
GO
/****** Object:  ForeignKey [FK_StorageRule_Attributes]    Script Date: 10/17/2011 10:57:48 ******/
ALTER TABLE [dbo].[StorageRule]  WITH CHECK ADD  CONSTRAINT [FK_StorageRule_Attributes] FOREIGN KEY([IdAttribute])
REFERENCES [dbo].[Attributes] ([IdAttribute])
GO
ALTER TABLE [dbo].[StorageRule] CHECK CONSTRAINT [FK_StorageRule_Attributes]
GO
/****** Object:  ForeignKey [FK_StorageRule_RuleOperator]    Script Date: 10/17/2011 10:57:49 ******/
ALTER TABLE [dbo].[StorageRule]  WITH CHECK ADD  CONSTRAINT [FK_StorageRule_RuleOperator] FOREIGN KEY([IdRuleOperator])
REFERENCES [dbo].[RuleOperator] ([IdRuleOperator])
GO
ALTER TABLE [dbo].[StorageRule] CHECK CONSTRAINT [FK_StorageRule_RuleOperator]
GO
/****** Object:  ForeignKey [FK_StorageRule_Storage]    Script Date: 10/17/2011 10:57:49 ******/
ALTER TABLE [dbo].[StorageRule]  WITH CHECK ADD  CONSTRAINT [FK_StorageRule_Storage] FOREIGN KEY([IdStorage])
REFERENCES [dbo].[Storage] ([IdStorage])
GO
ALTER TABLE [dbo].[StorageRule] CHECK CONSTRAINT [FK_StorageRule_Storage]
GO
/****** Object:  ForeignKey [FK_Transito_Document]    Script Date: 10/17/2011 10:57:49 ******/
ALTER TABLE [dbo].[Transito]  WITH CHECK ADD  CONSTRAINT [FK_Transito_Document] FOREIGN KEY([IdDocument])
REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[Transito] CHECK CONSTRAINT [FK_Transito_Document]
GO
