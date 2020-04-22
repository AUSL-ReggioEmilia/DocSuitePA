using System;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.DocSuiteWeb.AVCP
{
    public class SetDataSetResultException : DocSuiteException
    {
        public SetDataSetResult PartialResult { get; set; }

        public SetDataSetResultException(SetDataSetResult res, string message, Exception inner)
            : base(message, inner)
        {
            PartialResult = res;
        }
    }
}
