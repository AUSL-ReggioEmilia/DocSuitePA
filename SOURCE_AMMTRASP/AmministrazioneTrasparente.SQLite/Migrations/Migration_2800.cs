using FluentMigrator;

namespace AmministrazioneTrasparente.SQLite.Migrations
{
    [Migration(2800)]
    public class Migration_2800 : Migration
    {
        public override void Down()
        {
            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "AVCPCheckIsPublish",
                KeyValue = "True",
                Note = "Per AUSL-PC, value = false. Se disabilitato, non verifica che la serie documentale di AVCP sia abilitata alla pubblicazione e che i relativi item siano effettivamente pubblicati.",
                ItemGroup = "AVCP"
            });
        }

        public override void Up()
        {
            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "AVCPCheckIsPublish",
                KeyValue = "True",
                Note = "Per AUSL-PC, value = false. Se disabilitato, non verifica che la serie documentale di AVCP sia abilitata alla pubblicazione e che i relativi item siano effettivamente pubblicati.",
                ItemGroup = "AVCP"
            });
        }
    }
}
