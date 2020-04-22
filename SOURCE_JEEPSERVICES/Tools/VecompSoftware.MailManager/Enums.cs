using Limilabs.Client.IMAP;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.MailManager
{
    public enum Direction
    {
        In,
        Out,
        InOut
    }

    public static class Enums
    {
        public static Flag Flag(this ImapFlag source)
        {
            return new Flag(string.Format("\\{0}", source));
        }
    }
}
