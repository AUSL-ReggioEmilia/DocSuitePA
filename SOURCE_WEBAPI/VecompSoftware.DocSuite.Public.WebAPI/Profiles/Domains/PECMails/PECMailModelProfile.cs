using AutoMapper;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.PECMails;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI.Profiles.Domains.PECMails
{
    public class PECMailModelProfile : Profile
    {

        #region [ Constructor ]
        public PECMailModelProfile(IDataUnitOfWork unitOfWork, Security.ISecurity security)
        {
            //
            CreateMap<PECMail, PECMailModel>()
                .ForCtorParam("id", opt => opt.MapFrom(src => src.UniqueId))
                .AfterMap((src, dest) => dest.Subject = src.MailSubject)
                .AfterMap((src, dest) => dest.Date = src.MailDate)
                .AfterMap((src, dest) => dest.Body = src.MailContent)
                .AfterMap((src, dest) => dest.Senders = src.MailSenders)
                .AfterMap((src, dest) => dest.Recipients = src.MailRecipients)
                .AfterMap((src, dest) => dest.Step = src.MailType);
        }

        #endregion

    }
}