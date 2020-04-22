using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Objects.Exceptions
{
  public class DocumentUnitException
  {
    public static ApplicationException NotFound()
    {
      return new ApplicationException("DocumentUnit non trovata");
    }

    public static ApplicationException ReadOnly()
    {
      return new ApplicationException("La DocumentUnit non può essere modificata. Aprire prima la DocumentUnit o gli evenutali DocumentUnitAggregate a cui appartiene.");
    }

    public static ApplicationException InvalidDocuments(string[] docs)
    {
      return new ApplicationException(
        String.Format("I seguenti documenti della DocumentUnitChain non sono stati trovati o non sono dei capo-catena: '{0}'", String.Join("','", docs))
      );
    }
  }
}
