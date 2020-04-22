using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiblosDS.LegalExtension.AdminPortal.Helpers
{
  public class UserArchive
  {
    public static List<DocumentArchive> GetUserArchivesPaged(string userName, int skip, int take, out int total)
    {
      var customerEnabledArchives = CustomerService.GetCustomerArchivesByUsername(userName);
      var archives = new List<DocumentArchive>();

      total = 0;

      if (customerEnabledArchives != null)
      {
        var ids = customerEnabledArchives
            .Select(x => x.IdArchive);
        archives.AddRange(ArchiveService.GetArchivesByIdPaged(ids, skip, take, out total));
      }

      return archives;
    }
  }
}