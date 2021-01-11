using System.Collections.Generic;

namespace VecompSoftware.Commons.Interfaces.DocumentGenerator.Models
{
    public interface IDocumentGeneratorModel
    {
        ICollection<IDocumentGeneratorParameter> DocumentGeneratorParameters { get; set; }
    }
}
