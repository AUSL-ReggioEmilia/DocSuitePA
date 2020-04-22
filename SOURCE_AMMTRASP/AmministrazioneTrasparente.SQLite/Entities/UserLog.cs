using PetaPoco;
using System;

namespace AmministrazioneTrasparente.SQLite.Entities
{
    [ExplicitColumns]
    [TableName("UserLogs")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class UserLog : IEntity
    {
        private DateTimeOffset _logDate;
        [Column]
        public int Id { get; set; }

        [Column]
        public int IdUser { get; set; }

        [Ignore]
        public User User { get; set; }

        [Column]
        public DateTime LogDate { get; set; }

        [Column]
        public string Action { get; set; }
    }
}
