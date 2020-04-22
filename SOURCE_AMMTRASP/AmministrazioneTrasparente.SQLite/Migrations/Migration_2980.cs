using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmministrazioneTrasparente.SQLite.Migrations
{
    [Migration(2980)]
    public class Migration_2980 : Migration
    {
        public override void Down()
        {
            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "SeriesPreviewBehaviours",
                KeyValue = string.Empty,
                Note = "Indica le varie tipologie di visualizzazione delle serie documentali nella pagina di ricerca. Es. { \"PriorityEnabled\": true, \"LatestSeriesEnabled\": false }",
                ItemGroup = "Ricerca"
            });

            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "LatestSeriesGridElement",
                KeyValue = 10,
                Note = "Indica il numero di elementi da visualizzare nella griglia delle \"Informazioni recenti\"",
                ItemGroup = "Ricerca"
            });

            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "ConstraintPanelCollapsedOnOpen",
                KeyValue = false,
                Note = "Definisce se i pannelli relativi agli obblighi di trasparenza devono essere chiusi in apertura pagina di visualizzazione risultati",
                ItemGroup = "Ricerca"
            });

            Delete.FromTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "SeriesResultsByConstraintEnabled",
                KeyValue = false,
                Note = "Abilita la visualizzazione per obbligo di trasparenza dei risultati di ricerca",
                ItemGroup = "Ricerca"
            });
        }

        public override void Up()
        {
            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "SeriesPreviewBehaviours",
                KeyValue = string.Empty,
                Note = "Indica le varie tipologie di visualizzazione delle serie documentali nella pagina di ricerca. Es. { \"PriorityEnabled\": true, \"LatestSeriesEnabled\": false }",
                ItemGroup = "Ricerca"
            });

            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "LatestSeriesGridElement",
                KeyValue = 10,
                Note = "Indica il numero di elementi da visualizzare nella griglia delle \"Informazioni recenti\"",
                ItemGroup = "Ricerca"
            });

            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "ConstraintPanelCollapsedOnOpen",
                KeyValue = false,
                Note = "Definisce se i pannelli relativi agli obblighi di trasparenza devono essere chiusi in apertura pagina di visualizzazione risultati",
                ItemGroup = "Ricerca"
            });

            Insert.IntoTable("Parameters").Row(new
            {
                Active = true,
                KeyName = "SeriesResultsByConstraintEnabled",
                KeyValue = false,
                Note = "Abilita la visualizzazione per obbligo di trasparenza dei risultati di ricerca",
                ItemGroup = "Ricerca"
            });
        }
    }
}
