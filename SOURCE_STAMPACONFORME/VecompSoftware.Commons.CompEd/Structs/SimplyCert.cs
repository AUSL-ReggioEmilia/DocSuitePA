using System;

namespace VecompSoftware.Commons.CompEd.Structs
{
    public struct SimplyCert
    {
        public int Level;

        public string Type,
                    Name,
                    FiscalCode,
                    Role,
                    Description,
                    Issuer,
                    eMail,
                    SerialNumber,
                    Version,
                    Id;

        public DateTime Expiry,
                    ValidFrom;

        public string HeaderInfo { get; set; }
    }
}
