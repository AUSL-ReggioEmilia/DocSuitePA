-- =============================================
-- Script Template
-- =============================================
Create view ActiveConfiguration
as
SELECT     Archive.Name, Storage.Name AS Storage, Storage.MainPath, StorageArea.Path
FROM         ArchiveStorage INNER JOIN
                      Storage ON ArchiveStorage.IdStorage = Storage.IdStorage INNER JOIN
                      Archive ON ArchiveStorage.IdArchive = Archive.IdArchive INNER JOIN
                      StorageArea ON Storage.IdStorage = StorageArea.IdStorage
WHERE     (Storage.IsVisible = 1) AND (StorageArea.Enable = 1)