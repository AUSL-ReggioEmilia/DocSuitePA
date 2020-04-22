using System.Collections.Generic;
using System.Linq;
using BiblosDS.Library.Common.Objects;
using BiblosDS.API.Exceptions;

namespace BiblosDS.API
{
    public static class RequestConverter
    {
        public static List<DocumentAttributeValue> Convert(this IEnumerable<MetadatoItem> metadati, IEnumerable<DocumentAttribute> biblosDSMetadata)
        {
            var items = new List<DocumentAttributeValue>();
            foreach (var item in metadati)
            {
                var attributo = biblosDSMetadata.Where(x => x.Name == item.Name).SingleOrDefault();
                if (attributo != null)
                    items.Add(item.Convert(attributo));
                else
                    throw new AttributoNonTrovato("Attributo non definito in BiblosDS: " + item.Name);
            }
            return items;
        }

        private static DocumentAttributeValue Convert(this MetadatoItem metadato, DocumentAttribute biblosDSMetadato)
        {
            return new DocumentAttributeValue
            {
                IdAttribute = biblosDSMetadato.IdAttribute,
                Attribute = biblosDSMetadato,
                Value = metadato.Value
            };
        }
    }
}
