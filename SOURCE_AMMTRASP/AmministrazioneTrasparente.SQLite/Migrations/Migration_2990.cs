using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmministrazioneTrasparente.SQLite.Migrations
{
    [Migration(2990)]
    public class Migration_2990 : Migration
    {
        public override void Down()
        { }
        public override void Up()
        {
            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "DefaultViewSeries",
                KeyValue = string.Empty,
                Note = "Governa il comportamento di visualizzazione di default delle sezioni della trasparenza. (Priority;LatestSeries)  [ASL-TO= LatestSeries] ",
                ItemGroup = "Ricerca"
            });
            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "EnableHeaderComFixing",
                KeyValue = "True",
                Note = "Abilita il fix nello scroll del header-commands [ASL-TO= False]  ",
                ItemGroup = "Ricerca"
            });
        }
    }
}
