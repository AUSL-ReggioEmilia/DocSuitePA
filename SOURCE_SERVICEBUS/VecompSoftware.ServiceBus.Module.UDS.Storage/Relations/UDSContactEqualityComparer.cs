using System.Collections.Generic;

namespace VecompSoftware.ServiceBus.Module.UDS.Storage.Relations
{
    public sealed class UDSContactEqualityComparer : IEqualityComparer<UDSContact>
    {

        public bool Equals(UDSContact x, UDSContact y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            //todo: se nell'xsd ci fossero anche i riferimenti ai GUID di persistenza basterebbe solo questo controllo
            //return x.UDSContactId == y.UDSContactId;

            bool primaryControl = x.ContactType == y.ContactType && x.UDSId == y.UDSId;
            switch ((UDSContactType)x.ContactType)
            {
                case UDSContactType.Normal:
                    return primaryControl && x.IdContact == y.IdContact;
                case UDSContactType.Manual:
                    return primaryControl && x.ContactManual.Equals(y.ContactManual);
                default:
                    return false;
            }
        }

        public int GetHashCode(UDSContact obj)
        {
            return obj.GetHashCode();
        }
    }
}
