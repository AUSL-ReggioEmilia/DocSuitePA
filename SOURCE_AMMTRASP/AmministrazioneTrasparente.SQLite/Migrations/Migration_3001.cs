using FluentMigrator;

namespace AmministrazioneTrasparente.SQLite.Migrations
{
    [Migration(3001)]
    public class Migration_3001 : Migration
    {
        public override void Down()
        { 
            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "CustomerSatisfactionEnabled",
                KeyValue = "False",
                Note = "Per ASL-TO, value = true. Se impostato a true, nella home page verrà visualizzato il link per la compilazione del questionario di gradimento.",
                ItemGroup = "Layout"
            });

            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "CustomerSatisfactionSeriedId",
                KeyValue = "False",
                Note = "Per ASL-TO, value = true. Se impostato a true, nella home page verrà visualizzato il link per la compilazione del questionario di gradimento.",
                ItemGroup = "Layout"
            });
        }
        public override void Up()
        {
            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "CustomerSatisfactionEnabled",
                KeyValue = "True",
                Note = "Per ASL-TO, value = true. Se impostato a true, nella home page verrà visualizzato il link per la compilazione del questionario di gradimento.",
                ItemGroup = "Layout"
            });

            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "CustomerSatisfactionSeriedId",
                KeyValue = "-1",
                Note = "Per ASL-TO, value = <IdSeries>. Se impostato il parametro a CustomerSatisfactionEnabled è true, è necessario inserire l'Id DocSuite della DocumentSeries speciale che ospiterà i risultati della CustomerSutisfaction",
                ItemGroup = "Documenti"
            });
        }
    }
}