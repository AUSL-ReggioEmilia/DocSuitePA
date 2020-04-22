using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmministrazioneTrasparente.SQLite.Migrations
{
    [Migration(2940)]
    public class Migration_2940 : Migration
    {
        public override void Down()
        {
            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "AnalyticsStartDate",
                KeyValue = string.Empty,
                Note = "Per AUSL-RE (yyyy-MM-dd) -> Data da cui parte il calcolo delle visite alle pagine",
                ItemGroup = "Server"
            });
        }

        public override void Up()
        {
            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "AnalyticsStartDate",
                KeyValue = string.Empty,
                Note = "Per AUSL-RE (yyyy-MM-dd) -> Data da cui parte il calcolo delle visite alle pagine",
                ItemGroup = "Server"
            });
        }
    }
}
