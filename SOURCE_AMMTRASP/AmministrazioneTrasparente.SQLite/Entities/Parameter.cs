using PetaPoco;

namespace AmministrazioneTrasparente.SQLite.Entities
{
    [ExplicitColumns]
    [TableName("Parameters")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class Parameter : IEntity
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public bool Active { get; set; }

        [Column]
        public string KeyName { get; set; }

        [Column]
        public string KeyValue { get; set; }

        [Column]
        public string Note { get; set; }

        [Column]
        public string ItemGroup { get; set; }
    }
}
