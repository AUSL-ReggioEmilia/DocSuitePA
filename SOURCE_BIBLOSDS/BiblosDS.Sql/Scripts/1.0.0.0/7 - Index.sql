-- =============================================
-- Script Template
-- =============================================
USE [BiblosDS2010]
GO

USE [BiblosDS2010]
GO

/****** Object:  Index [IX_Document]    Script Date: 10/18/2011 19:05:48 ******/
CREATE NONCLUSTERED INDEX [IX_Document] ON [dbo].[AttributesValue] 
(
	[IdDocument] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [BiblosDS2010]
GO

/****** Object:  Index [IX_IdAttribute]    Script Date: 10/18/2011 19:06:08 ******/
CREATE NONCLUSTERED INDEX [IX_IdAttribute] ON [dbo].[AttributesValue] 
(
	[IdAttribute] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


USE [BiblosDS2010]
GO

/****** Object:  Index [IX_Archive_Storage]    Script Date: 10/18/2011 19:06:25 ******/
CREATE NONCLUSTERED INDEX [IX_Archive_Storage] ON [dbo].[Document] 
(
	[IdStorage] ASC,
	[IdArchive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


USE [BiblosDS2010]
GO

/****** Object:  Index [IX_IdArchive]    Script Date: 10/18/2011 19:07:06 ******/
CREATE NONCLUSTERED INDEX [IX_IdArchive] ON [dbo].[Document] 
(
	[IdArchive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

USE [BiblosDS2010]
GO

/****** Object:  Index [IX_IdBiblos_IdArchive]    Script Date: 10/18/2011 19:07:21 ******/
CREATE NONCLUSTERED INDEX [IX_IdBiblos_IdArchive] ON [dbo].[Document] 
(
	[IdBiblos] ASC,
	[IdArchive] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO



CREATE NONCLUSTERED INDEX [_dta_index_Document_K3] ON [dbo].[Document] 
(
	[IdParentBiblos] ASC,
	[IdDocument] ASC,
	[IdArchive] ASC,
	[IdParentVersion] ASC,
	[IdStorageArea] ASC,
	[IdStorage] ASC
)
INCLUDE ( [IdBiblos],
[ChainOrder],
[StorageVersion],
[Version],
[IdDocumentLink],
[IdCertificate],
[SignHeader],
[FullSign],
[DocumentHash],
[IsLinked],
[IsVisible],
[IsConservated],
[DateExpire],
[DateCreated],
[Name],
[Size],
[IdNodeType],
[IsConfirmed],
[IdDocumentStatus],
[IsCheckOut],
[DateMain],
[IdPreservation],
[IsDetached],
[IdUserCheckOut],
[PrimaryKeyValue],
[IdPreservationException],
[PreservationIndex],
[IdThumbnail],
[IdPdf]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]