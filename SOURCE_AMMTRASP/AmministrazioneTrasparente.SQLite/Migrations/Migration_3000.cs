using FluentMigrator;

namespace AmministrazioneTrasparente.SQLite.Migrations
{
    [Migration(3000)]
    public class Migration_3000 : Migration
    {
        public override void Down()
        {
            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "ShowAllItems",
                KeyValue = "False",
                Note = "Se impostato a true,  permette di vedere tutte le serie documentali pubblicate senza la necessità di dover premere il pulsante 'visualizza dati pubblicati'",
                ItemGroup = "Layout"
            });
        }

        public override void Up()
        {
            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "ShowAllItems",
                KeyValue = "False",
                Note = "Se impostato a true,  permette di vedere tutte le serie documentali pubblicate senza la necessità di dover premere il pulsante 'visualizza dati pubblicati'",
                ItemGroup = "Layout"
            });
        }
    }
}
