using AutoMapper;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.PECMails;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.PECMails
{
    public class PECMailReceiptModelProfile : Profile
    {

        #region [ Constructor ]
        public PECMailReceiptModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            //
            CreateMap<PECMailReceipt, PECMailReceiptModel>()
                .AfterMap((src, dest) => dest.Step = src.ReceiptType)
                .AfterMap((src, dest) => dest.Date = src.ReceiptDate);
        }

        #endregion

    }
}