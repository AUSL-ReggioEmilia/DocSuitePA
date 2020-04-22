using VecompSoftware.DocSuiteWeb.Validation.Objects.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.CQRS.Events
{
    public interface IEventValidatorMapper : IValidatorMapper<IEvent, EventValidator>
    {
    }
}
