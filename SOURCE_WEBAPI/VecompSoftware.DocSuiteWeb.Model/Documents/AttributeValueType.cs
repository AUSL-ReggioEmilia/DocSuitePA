using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.DocSuiteWeb.Model.Documents
{
    public enum AttributeValueType : short
    {
        Undefined = 0,
        PropertyString = 1,
        PropertyInt = 2 * PropertyString,
        PropertyDate = 2 * PropertyInt,
    }
   
}
