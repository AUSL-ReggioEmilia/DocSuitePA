using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace VecompSoftware.DocSuite.SPID.Logging.Extensions
{
    internal static class HeaderDictionaryExtension
    {
        internal static IDictionary<string, IEnumerable<string>> ConvertHeadersToDictionary(this IHeaderDictionary headers)
        {
            return headers.Select(s => new { s.Key, s.Value })
                .ToDictionary(d => d.Key, d => d.Value.AsEnumerable<string>());
        }
    }
}
