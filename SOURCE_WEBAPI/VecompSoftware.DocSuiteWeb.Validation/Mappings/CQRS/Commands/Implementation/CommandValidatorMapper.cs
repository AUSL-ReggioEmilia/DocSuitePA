using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Commands;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.CQRS.Commands
{
    public class CommandValidatorMapper : BaseMapper<ICommand, CommandValidator>, ICommandValidatorMapper
    {
        public override CommandValidator Map(ICommand entity, CommandValidator entityTransformed)
        {
            entityTransformed.CommandId = entity.Id;
            entityTransformed.CommandName = entity.CommandName;
            entityTransformed.CreationTime = entity.CreationTime;
            entityTransformed.TenantId = entity.TenantId;
            entityTransformed.TenantName = entity.TenantName;

            return entityTransformed;
        }

    }
}
