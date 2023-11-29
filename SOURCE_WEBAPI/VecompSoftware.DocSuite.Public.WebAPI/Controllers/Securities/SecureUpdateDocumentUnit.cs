using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VecompSoftware.DocSuite.Public.WebAPI.Controllers.Securities
{
    public class SecureUpdateDocumentUnit
    {
        public Guid AuthenticationId { get; set; }
        public Guid Token { get; set; }
        public string DocumentUnit { get; set; }
        public short Year { get; set; }
        public int Number { get; set; }
        public string Filename { get; set; }
        public byte[] Stream { get; set; }
    }
}