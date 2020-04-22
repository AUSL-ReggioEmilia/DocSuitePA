using FluentMigrator;

namespace AmministrazioneTrasparente.SQLite.Migrations
{
    [Migration(2630)]
    public class Migration_2630 : Migration
    {
        public override void Up()
        {
            Insert.IntoTable("Parameters").Row(new 
            {
                Active = true,
                KeyName = "OneDocumentSeriesItemEnable",
                KeyValue = "False",
                Note = "Per AZOSP-VR, value = true. Se abilitato, quando la serie documentale contiene un solo elemento, permette la visualizzazione dell elemento in primo piano.",
                ItemGroup = "Ricerca"
            });
        }

        public override void Down()
        {
            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "OneDocumentSeriesItemEnable",
                KeyValue = "False",
                Note = "Per AZOSP-VR, value = true. Se abilitato, quando la serie documentale contiene un solo elemento, permette la visualizzazione dell elemento in primo piano.",
                ItemGroup = "Ricerca"
            });
        }
    }
}
