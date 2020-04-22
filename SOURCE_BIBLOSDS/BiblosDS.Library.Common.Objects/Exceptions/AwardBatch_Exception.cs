using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
  public class AwardBatch_Exception : Exception
  {
    public AwardBatch_Exception()
      : base("Nessun AwardBatch trovato con i parametri passati")
    {
    }

    public AwardBatch_Exception(string msg)
      : base(msg)
    {
    }
  }

}
