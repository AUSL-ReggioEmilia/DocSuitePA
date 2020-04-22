using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Objects.Exceptions
{
  public class DocumentUnitAggregateException
  {
    public static ApplicationException NotFound()
    {
      return new ApplicationException("DocumentUnitAggregate non trovata");
    }

    public static ApplicationException ReadOnly()
    {
      return new ApplicationException("La DocumentUnitAggregate è in sola lettura. Aprire prima la DocumentUnitAggregate. Se è già stata preservata non è possibile riaprirla.");
    }

    public static ApplicationException Preserved()
    {
      return new ApplicationException("La DocumentUnitAggregate è già stata preservata e non può essere in alcun modo modificata.");
    }
  }
}
