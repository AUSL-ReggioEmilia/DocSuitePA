using FluentMigrator;

namespace AmministrazioneTrasparente.SQLite.Migrations
{
    [Migration(2920)]
    public class Migration_2920 : Migration
    {
        public override void Down()
        {
            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "ShowSeriesCountDetails",
                KeyValue = "True",
                Note = "Per AUSL-RE, value = false. Se impostato a false, viene nascosta l'etichetta con il conteggio degli elementi nella serie e relativo primo piano.",
                ItemGroup = "Layout"
            });
        }

        public override void Up()
        {
            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "ShowSeriesCountDetails",
                KeyValue = "True",
                Note = "Per AUSL-RE, value = false. Se impostato a false, viene nascosta l'etichetta con il conteggio degli elementi nella serie e relativo primo piano.",
                ItemGroup = "Layout"
            });
        }
    }
}
