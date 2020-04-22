using System;

namespace VecompSoftware.DocSuiteWeb.Model.Metadata
{
    [Serializable()]
    public class CommentFieldModel
    {
        public string Author { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
    }
}
