using FluentMigrator;

namespace AmministrazioneTrasparente.SQLite.Migrations
{
    [Migration(2900)]
    public class Migration_2900 : Migration
    {
        public override void Down()
        {
            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "HeaderExternalUrl",
                KeyValue = "",
                Note = "Per AUSL-RE, value = http://www.ausl.re.it/inc_header.php. Se impostato verrà aggiunta una nuova sezione header al sito con il contenuto dell'URL impostato.",
                ItemGroup = "Layout"
            });

            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "FooterExternalUrl",
                KeyValue = "",
                Note = "Per AUSL-RE, value = http://www.ausl.re.it/inc_footer.php. Se impostato verrà aggiunta una nuova sezione footer al sito con il contenuto dell'URL impostato.",
                ItemGroup = "Layout"
            });

            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "SimpleSearchEnable",
                KeyValue = "False",
                Note = "Per AUSL-RE, value = true. Se impostato a true, la pagina di ricerca visualizzerà di default solamente Oggetto e Data Dal/Al con un nuovo pulsante per la visualizzazione della ricerca completa.",
                ItemGroup = "Ricerca"
            });
        }

        public override void Up()
        {
            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "HeaderExternalUrl",
                KeyValue = "",
                Note = "Per AUSL-RE, value = http://www.ausl.re.it/inc_header.php. Se impostato verrà aggiunta una nuova sezione header al sito con il contenuto dell'URL impostato.",
                ItemGroup = "Layout"
            });

            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "FooterExternalUrl",
                KeyValue = "",
                Note = "Per AUSL-RE, value = http://www.ausl.re.it/inc_footer.php. Se impostato verrà aggiunta una nuova sezione footer al sito con il contenuto dell'URL impostato.",
                ItemGroup = "Layout"
            });

            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "SimpleSearchEnable",
                KeyValue = "False",
                Note = "Per AUSL-RE, value = true. Se impostato a true, la pagina di ricerca visualizzerà di default solamente Oggetto e Data Dal/Al con un nuovo pulsante per la visualizzazione della ricerca completa.",
                ItemGroup = "Ricerca"
            });
        }
    }
}
