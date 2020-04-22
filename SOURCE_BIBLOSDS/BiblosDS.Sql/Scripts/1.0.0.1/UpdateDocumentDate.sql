-- =============================================
-- Script Template
-- =============================================
update document
set DateMain = ValueDateTime
from AttributesValue
where AttributesValue.IdDocument = document.iddocument
and IdAttribute = 'A09E7467-2609-4823-87CC-15C9ADEC5B10'
and DateMain is null and IdArchive = '0cccf40e-4d05-4097-aa39-c3078da117e6'

--Aggiorna i documenti sgaffi
update dbo.Document
set [IsDetached] = 1
where iddocument not in(
select iddocument from AttributesValue
)
and     (IdArchive = 'd6e79b92-0efe-4196-a9e9-88842f9ef817') AND (IdPreservation IS NULL) AND (NOT (IdParentBiblos IS NULL)) AND (IsVisible = 1)



update Document
set IsConservated = 1
where not IdPreservation is null


update document 
set datemain = null,
primarykeyValue = null
where idarchive = 'd6e79b92-0efe-4196-a9e9-88842f9ef817'
and IdPreservation is null