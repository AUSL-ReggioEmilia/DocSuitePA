using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.DocumentUnits
{
    public class DocumentUnitChainModelMapper : BaseModelMapper<DocumentUnitChain, UDSDocumentModel>, IDocumentUnitChainModelMapper
    {

        private readonly IMapperUnitOfWork _mapperUnitOfWork;

        public DocumentUnitChainModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        public static UDSDocumentType ChainTypeConverter(ChainType chainType)
        {
            switch (chainType)
            {
                case ChainType.MainChain:
                    return UDSDocumentType.Document;
                case ChainType.AttachmentsChain:
                    return UDSDocumentType.DocumentAttachment;
                case ChainType.AnnexedChain:
                    return UDSDocumentType.DocumentAnnexed;
                default:
                    throw new Exception("DocumentUnitChainModelMapper - Errore di conversione tra ChainType e DocumentType.");

            }
        }

        public override UDSDocumentModel Map(DocumentUnitChain model, UDSDocumentModel modelTransformed)
        {
            modelTransformed.IdChain = model.IdArchiveChain;
            modelTransformed.DocumentType = ChainTypeConverter(model.ChainType);

            return modelTransformed;
        }

        public override ICollection<UDSDocumentModel> MapCollection(ICollection<DocumentUnitChain> entities)
        {
            if (entities == null)
            {
                return new List<UDSDocumentModel>();
            }

            List<UDSDocumentModel> entitiesTransformed = new List<UDSDocumentModel>();
            UDSDocumentModel entityTransformed = null;
            foreach (IGrouping<Guid, DocumentUnitChain> udLookup in entities.ToLookup(x => x.UniqueId))
            {
                entityTransformed = Map(udLookup.First(), new UDSDocumentModel());
                entitiesTransformed.Add(entityTransformed);
            }

            return entitiesTransformed;
        }

    }
}
