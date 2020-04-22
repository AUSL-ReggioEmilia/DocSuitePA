using System.Threading.Tasks;
using VecompSoftware.BiblosDS.Model.CQRS;

namespace VecompSoftware.BiblosDS.WindowsService.WebAPI.Receivers
{
    public interface IReceiverMediator
    {
        Task Send(CommandModel commandModel);
    }
}