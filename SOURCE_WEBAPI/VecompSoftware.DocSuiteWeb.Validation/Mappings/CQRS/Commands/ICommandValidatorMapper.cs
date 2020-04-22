using VecompSoftware.DocSuiteWeb.Validation.Objects.CQRS.Commands;
using VecompSoftware.Services.Command.CQRS.Commands;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.CQRS.Commands
{
    public interface ICommandValidatorMapper : IValidatorMapper<ICommand, CommandValidator>
    {
    }
}
