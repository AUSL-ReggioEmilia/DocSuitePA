

-- Units --

CREATE TABLE [dbo].[DocumentUnit](
	[IdDocumentUnit] [uniqueidentifier] NOT NULL,
	[Identifier] [varchar](900) NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[CloseDate] [datetime] NULL,
	[Subject] [varchar](1024) NOT NULL,
	[Classification] [varchar](1024) NOT NULL,
	[UriFascicle] [varchar](1024) NULL,
	[XmlDoc] [varchar](max) NULL,
 CONSTRAINT [PK_DocumentUnit] PRIMARY KEY CLUSTERED 
(
	[IdDocumentUnit] ASC
) ON [PRIMARY],
 CONSTRAINT [UNQ_DocumentUnit_Identifier] UNIQUE NONCLUSTERED 
(
	[Identifier] ASC
) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[DocumentUnitChain](
	[IdDocumentUnit] [uniqueidentifier] NOT NULL,
	[IdParentBiblos] [uniqueidentifier] NOT NULL,
	[Name] [varchar](max) NOT NULL,
 CONSTRAINT [PK_DocumentUnitChain] PRIMARY KEY CLUSTERED 
(
	[IdDocumentUnit] ASC,
	[IdParentBiblos] ASC
) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[DocumentUnitChain]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnitChain_DocumentUnit] FOREIGN KEY([IdDocumentUnit]) REFERENCES [dbo].[DocumentUnit] ([IdDocumentUnit])
GO
ALTER TABLE [dbo].[DocumentUnitChain] CHECK CONSTRAINT [FK_DocumentUnitChain_DocumentUnit]
GO


ALTER TABLE [dbo].[DocumentUnitChain]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnitChain_Document] FOREIGN KEY([IdParentBiblos]) REFERENCES [dbo].[Document] ([IdDocument])
GO
ALTER TABLE [dbo].[DocumentUnitChain] CHECK CONSTRAINT [FK_DocumentUnitChain_Document]
GO


-- Aggregate --

CREATE TABLE [dbo].[DocumentUnitAggregate](
	[IdAggregate] [uniqueidentifier] NOT NULL,
	[XmlFascicle] [varchar](max) NULL,
	[CloseDate] [datetime] NULL,
	[AggregationType] [smallint] NOT NULL,
	[PreservationDate] [datetime] NULL,
 CONSTRAINT [PK_DocumentUnitAggregate] PRIMARY KEY CLUSTERED 
(
	[IdAggregate] ASC
) ON [PRIMARY],
) ON [PRIMARY]


CREATE TABLE [dbo].[DocumentUnitAggregateChain](
	[IdAggregate] [uniqueidentifier] NOT NULL,
	[IdDocumentUnit] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_DocumentUnitAggregateChain] PRIMARY KEY CLUSTERED 
(
	[IdAggregate] ASC,
	[IdDocumentUnit] ASC
) ON [PRIMARY]
) ON [PRIMARY]  


ALTER TABLE [dbo].[DocumentUnitAggregateChain]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnitAggregateChain_DocumentUnit] FOREIGN KEY([IdDocumentUnit]) REFERENCES [dbo].[DocumentUnit] ([IdDocumentUnit])
GO
ALTER TABLE [dbo].[DocumentUnitAggregateChain] CHECK CONSTRAINT [FK_DocumentUnitAggregateChain_DocumentUnit]
GO


ALTER TABLE [dbo].[DocumentUnitAggregateChain]  WITH CHECK ADD  CONSTRAINT [FK_DocumentUnitAggregateChain_DocumentUnitAggregate] FOREIGN KEY([IdAggregate]) REFERENCES [dbo].[DocumentUnitAggregate] ([IdAggregate])
GO
ALTER TABLE [dbo].[DocumentUnitAggregateChain] CHECK CONSTRAINT [FK_DocumentUnitAggregateChain_DocumentUnitAggregate]
GO
