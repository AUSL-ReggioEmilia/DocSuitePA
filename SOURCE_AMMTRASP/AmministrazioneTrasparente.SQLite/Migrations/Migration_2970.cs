using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmministrazioneTrasparente.SQLite.Migrations
{
    [Migration(2970)]
    public class Migration_2970: Migration
    {
        public override void Down()
        {
            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "AnalyticsIDView",
                KeyValue = string.Empty,
                Note = "Per AUSL-RE VIEW_ID per la vista impostata sul portale di analytics",
                ItemGroup = "Server"
            });
        }

        public override void Up()
        {
            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "AnalyticsIDView",
                KeyValue = string.Empty,
                Note = "Per AUSL-RE VIEW_ID per la vista impostata sul portale di analytics",
                ItemGroup = "Server"
            });
        }
    }
}
