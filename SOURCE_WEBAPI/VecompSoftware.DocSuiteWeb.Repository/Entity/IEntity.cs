using System;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.DocSuiteWeb.Repository.Infrastructure;

namespace VecompSoftware.DocSuiteWeb.Repository.Entity
{
    public interface IEntity : IObjectState, IContentBase
    {
        int EntityId { get; set; }

        short EntityShortId { get; set; }

        DateTimeOffset RegistrationDate { get; set; }

        string LastChangedUser { get; set; }

        DateTimeOffset? LastChangedDate { get; set; }

        byte[] Timestamp { get; set; }
    }
}
