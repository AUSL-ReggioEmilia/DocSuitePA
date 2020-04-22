using FluentMigrator;

namespace AmministrazioneTrasparente.SQLite.Migrations
{
    [Migration(2930)]
    public class Migration_2930 : Migration
    {
        public override void Down()
        {
            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "DocumentsHeaderLabel",
                KeyValue = "{ \"MainChain\": \"Documenti\" }",
                Note = "Per ***REMOVED***, value = { \"MainChain\": \"Procedure\", \"AnnexedChain\": \"Linee Guida\" }. Se non impostato, verranno utilizzate le etichette di Default.",
                ItemGroup = "Documenti"
            });

            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "ArchiveRestriction",
                KeyValue = string.Empty,
                Note = "Per ***REMOVED***, value = (Id dell'archivio da utilizzare per filtrare le serie documentali). Se non impostato, non saranno impostate restrizioni (comportamento di default).",
                ItemGroup = "Ricerca"
            });
        }

        public override void Up()
        {
            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "DocumentsHeaderLabel",
                KeyValue = "{ \"MainChain\": \"Documenti\" }",
                Note = "Per ***REMOVED***, value = { \"MainChain\": \"Procedure\", \"AnnexedChain\": \"Linee Guida\" }. Se non impostato, verranno utilizzate le etichette di Default.",
                ItemGroup = "Documenti"
            });

            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "ArchiveRestriction",
                KeyValue = string.Empty,
                Note = "Per ***REMOVED***, value = (Id dell'archivio da utilizzare per filtrare le serie documentali). Se non impostato, non saranno impostate restrizioni (comportamento di default).",
                ItemGroup = "Ricerca"
            });
        }
    }
}
