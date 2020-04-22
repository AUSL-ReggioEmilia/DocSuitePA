using System.ComponentModel.DataAnnotations.Schema;

namespace VecompSoftware.DocSuiteWeb.Repository.Infrastructure
{
    public interface IObjectState
    {
        [NotMapped]
        ObjectState ObjectState { get; set; }
    }
}