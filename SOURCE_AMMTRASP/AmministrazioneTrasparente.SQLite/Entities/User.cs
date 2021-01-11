using PetaPoco;
using System;

namespace AmministrazioneTrasparente.SQLite.Entities
{
    [ExplicitColumns]
    [TableName("Users")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class User : IEntity
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public bool Active { get; set; }

        [Column]
        public string Username { get; set; }

        [Column]
        public string Password { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public string Surname { get; set; }

        [Column]
        public string Email { get; set; }
    }
}
