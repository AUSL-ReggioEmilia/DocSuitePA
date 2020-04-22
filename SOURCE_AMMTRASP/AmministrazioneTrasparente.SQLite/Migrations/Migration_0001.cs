using FluentMigrator;
using System;

namespace AmministrazioneTrasparente.SQLite.Migrations
{
    [Migration(0001)]
    public class Migration_0001 : Migration
    {
        public override void Up()
        {
            //Users
            Create.Table("Users")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Active").AsInt16()
                .WithColumn("Username").AsString()
                .WithColumn("Password").AsString()
                .WithColumn("Name").AsString().Nullable()
                .WithColumn("Surname").AsString().Nullable()
                .WithColumn("Email").AsString().Nullable();

            Create.Index("ix_Username").OnTable("Users").OnColumn("Username").Unique();

            //UserLogs
            Create.Table("UserLogs")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("IdUser").AsInt32().ReferencedBy("Users", "Id")
                .WithColumn("LogDate").AsDateTime()
                .WithColumn("Action").AsString();

            //Parameters
            Create.Table("Parameters")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Active").AsInt16()
                .WithColumn("KeyName").AsString()
                .WithColumn("KeyValue").AsString().Nullable()
                .WithColumn("Note").AsString().Nullable()
                .WithColumn("ItemGroup").AsString();

            Create.Index("ix_KeyName").OnTable("Parameters").OnColumn("KeyName").Unique();

            InsertData();
        }

        private void InsertData()
        {
            Insert.IntoTable("Users").Row(new { Username = "Administrator", Password = "d41e98d1eafa6d6011d3a70f1a5b92f0", Active = true });
            Insert.IntoTable("Parameters").Row(new 
            {
                Active = true,
                KeyName = "GoogleAnalyticsCode",
                Note = "Inserisci il codice di analisi GoogleAnalyticsCode in questo campo (es. UA-55129017-1).",
                ItemGroup = "Server"
            }).Row(new 
            {
                Active = true,
                KeyName = "SignatureEnable",
                KeyValue = "False",
                Note = "Abilita la gestione custom della segnatura per i documenti.",
                ItemGroup = "Documenti"
            }).Row(new 
            {
                Active = true,
                KeyName = "SignatureTemplate",
                KeyValue = "{0:container.name} n° {0:year}/{0:number} del {0:publishingdate|dd/MM/yyyy}",
                Note = "Template per la gestione della signature documenti.",
                ItemGroup = "Documenti"
            }).Row(new 
            {
                Active = true,
                KeyName = "ThemeConfiguration",
                KeyValue = "",
                Note = "Indicare il nome del tema (corrispondente alla directory creata sotto la voce css/Themes) da applicare al sito.",
                ItemGroup = "Layout"
            }).Row(new 
            {
                Active = true,
                KeyName = "CustomFamilyLink",
                KeyValue = "",
                Note = "Possibilità di inserire link personalizzati di navigazione nelle pagine Family. Struttura: {id family, testo da visualizzare, url}|...",
                ItemGroup = "Layout"
            }).Row(new 
            {
                Active = true,
                KeyName = "ViewDocumentFullName",
                KeyValue = "False",
                Note = "Se abilitato visualizza il nome originale dei documenti principali.",
                ItemGroup = "Documenti"
            }).Row(new 
            {
                Active = true,
                KeyName = "CsvFilesPath",
                Note = "Percorso dove sono salvati i CSV relativi alle serie documentali.",
                ItemGroup = "Documenti"
            });
        }

        public override void Down()
        {
            Delete.Table("Users");
            Delete.Table("UserLogs");
            Delete.Table("Parameters");
        }
    }
}
