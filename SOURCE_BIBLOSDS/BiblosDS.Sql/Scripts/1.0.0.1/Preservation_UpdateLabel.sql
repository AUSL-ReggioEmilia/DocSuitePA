-- =============================================
-- Script Template
-- =============================================
Update Preservation 
set Label = REVERSE(substring(REVERSE([Path]), 0, CHARINDEX('\', REVERSE([Path]))))
where label is null