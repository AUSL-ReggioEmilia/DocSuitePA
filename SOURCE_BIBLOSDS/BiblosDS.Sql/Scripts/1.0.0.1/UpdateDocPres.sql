declare @idArchive as uniqueidentifier

set @idArchive = '00000000-0000-0000-0000-000000000003';

update BiblosDS2010.dbo.document
set idpreservation = BiblosDS2010.dbo.Preservation.IdPreservation,
DocumentHash = [Hash],
DateMain = DataOggetto,
PrimaryKeyValue = ChiaveUnivoca,
PreservationIndex = Progressivo,
PreservationName = NomeFileInArchivio
from dbo.Oggetto_Conservazione inner join BiblosDS2010.dbo.Preservation on BiblosDS2010.dbo.Preservation.IdCompatibility = dbo.Oggetto_Conservazione.IdConservazione
where BiblosDS2010.dbo.document.idarchive = @idArchive and dbo.Oggetto_Conservazione.IdOggetto = BiblosDS2010.dbo.document.IdBiblos
and BiblosDS2010.dbo.Preservation.idarchive = @idArchive