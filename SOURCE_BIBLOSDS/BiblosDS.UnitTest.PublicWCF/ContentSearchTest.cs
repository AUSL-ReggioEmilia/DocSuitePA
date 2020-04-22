using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BiblosDS.UnitTest.PublicWCF.ServiceReferenceContentSearch;

namespace BiblosDS.UnitTest.PublicWCF
{
    [TestClass]
    public class ContentSearchTest
    {
        [TestMethod]
        public void SearchDocumentVisibility()
        {
            ServiceReferenceContentSearch.ContentSearchClient client = new ServiceReferenceContentSearch.ContentSearchClient();
            int documentsInArchive = 0;
            var result = client.GetAllDocuments(out documentsInArchive, "BIBLPROT", true, 0, 10);
            
        }

        [TestMethod]
        public void SearchByAttribute()
        {
            using (var client = new ServiceReferenceContentSearch.ContentSearchClient())
            {
                var doc = client.SearchQuery(new[] { new Condition { Name = "Descrizione", Value = "qwwq", Operator = FilterOperator.IsEqualTo } }, null, null);
            }
        }
    }
}
