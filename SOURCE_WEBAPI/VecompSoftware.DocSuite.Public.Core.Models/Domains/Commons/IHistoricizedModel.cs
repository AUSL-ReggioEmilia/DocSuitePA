using System;
namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons
{
    public interface IHistoricizedModel
    {
        DateTimeOffset StartDate { get; set; }

        DateTimeOffset? EndDate { get; set; }
    }
}
