using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common.Exceptions
{
    public class Permission_Exception: Exception
    {
        public Permission_Exception()
            : base("Diritti non sufficienti per eseguire l'operazione richiesta.")
        {
        }
    }
}
