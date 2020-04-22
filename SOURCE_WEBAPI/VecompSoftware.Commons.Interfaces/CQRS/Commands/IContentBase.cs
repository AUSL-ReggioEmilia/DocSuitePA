using System;

namespace VecompSoftware.Commons.Interfaces.CQRS.Commands
{
    public interface IContentBase
    {
        Guid UniqueId { get; set; }

        string RegistrationUser { get; set; }
    }
}