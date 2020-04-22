using FluentMigrator;

namespace AmministrazioneTrasparente.SQLite.Migrations
{
    [Migration(2910)]
    public class Migration_2910 : Migration
    {
        public override void Down()
        {
            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "HistoryEnable",
                KeyValue = "False",
                Note = "Per AUSL-RE, value = true. Se impostato a true, viene abilitata la funzionalità di ricerca in archivio storico.",
                ItemGroup = "Ricerca"
            });
        }

        public override void Up()
        {
            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "HistoryEnable",
                KeyValue = "False",
                Note = "Per AUSL-RE, value = true. Se impostato a true, viene abilitata la funzionalità di ricerca in archivio storico.",
                ItemGroup = "Ricerca"
            });
        }
    }
}
