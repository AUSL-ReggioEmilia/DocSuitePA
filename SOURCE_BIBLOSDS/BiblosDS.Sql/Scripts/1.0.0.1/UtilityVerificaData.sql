-- =============================================
-- Script Template
-- =============================================
DECLARE filesupp_cursor CURSOR FOR 
select IdDocument, IdAttribute, ValueString
from AttributesValue
where (not ValueString is null and ValueString <> '')
and ValueDateTime is null
and idattribute in
(
select idattribute from attributes
where AttributeType = 'System.DateTime'
)

OPEN filesupp_cursor;

declare @doc as uniqueidentifier
declare @attr as uniqueidentifier
declare @str as varchar(255)

FETCH NEXT FROM filesupp_cursor 
INTO @doc, @attr, @str

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT @doc;
    PRINT @doc
    PRINT @str
    
	print CONVERT(datetime, @str, 103) 
		
    FETCH NEXT FROM filesupp_cursor 
    INTO @doc, @attr, @str
    END
CLOSE filesupp_cursor;
DEALLOCATE filesupp_cursor;

