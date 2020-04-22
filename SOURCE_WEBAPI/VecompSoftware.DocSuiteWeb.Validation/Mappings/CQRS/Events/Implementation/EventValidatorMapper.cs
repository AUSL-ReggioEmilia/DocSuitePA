using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.CQRS.Events;
using VecompSoftware.Services.Command.CQRS.Events;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.CQRS.Events
{
    public class EventValidatorMapper : BaseMapper<IEvent, EventValidator>, IEventValidatorMapper
    {
        public override EventValidator Map(IEvent entity, EventValidator entityTransformed)
        {
            entityTransformed.EventId = entity.Id;
            entityTransformed.EventName = entity.EventName;
            entityTransformed.CreationTime = entity.CreationTime;
            entityTransformed.TenantId = entity.TenantId;
            entityTransformed.TenantName = entity.TenantName;
            entityTransformed.ExecutedTime = entity.ExecutedTime;

            return entityTransformed;
        }

    }
}
