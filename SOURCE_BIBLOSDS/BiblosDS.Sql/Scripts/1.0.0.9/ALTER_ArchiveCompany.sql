ALTER TABLE [dbo].[ArchiveCompany]  WITH CHECK ADD  CONSTRAINT [FK_ArchiveCompany_Company] FOREIGN KEY([Company])
REFERENCES [dbo].[Company] ([IdCompany])
GO

ALTER TABLE [dbo].[ArchiveCompany] CHECK CONSTRAINT [FK_ArchiveCompany_Company]
GO
