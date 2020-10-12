using AutoMapper;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuite.Public.Core.Models.Domains.PECMails;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Finder.PECMails;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Domains.Protocols
{
    [EnableQuery]
    public class PECMailsController : BaseODataController<PECMailModel, PECMail>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        #endregion

        #region [ Constructor ]

        public PECMailsController(IDataUnitOfWork unitOfWork, ILogger logger, IMapper mapper)
            : base(unitOfWork, logger, mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #endregion

        #region [ Methods ]

        [EnableQuery(MaxExpansionDepth = 10, PageSize = 100, AllowedQueryOptions = AllowedQueryOptions.All)]
        [HttpGet]
        public IHttpActionResult GetProtocolPECMails(Guid uniqueId, PECMailDirection direction)
        {
            return ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IList<PECMail> pecMails = _unitOfWork.Repository<PECMail>().GetByProtocol(uniqueId, direction).ToList();
                if (pecMails == null)
                {
                    throw new ArgumentNullException("PECMails not found");
                }
                IList<PECMailModel> pecList = new List<PECMailModel>();
                foreach (PECMail item in pecMails)
                {
                    PECMailModel model = _mapper.Map<PECMail, PECMailModel>(item);

                    if (item.Direction.Equals(PECMailDirection.Outgoing))
                    {
                        ICollection<PECMailReceipt> receipts = item.PECMailReceipts.Where(r => r.ReceiptType.Equals("avvenuta-consegna") || r.ReceiptType.Equals("accettazione") ||
                                                                         r.ReceiptType.Equals("non-accettazione") || r.ReceiptType.Equals("preavviso-errore-consegna") ||
                                                                         r.ReceiptType.Equals("errore-consegna")).OrderBy(t => t.ReceiptDate).ToList();
                        model.Receipts = receipts.Select(t => _mapper.Map<PECMailReceipt, PECMailReceiptModel>(t)).ToList();
                    }

                    pecList.Add(model);
                }
                return Ok(pecList);
            }, _logger, LogCategories);
        }


        #endregion
    }
}
