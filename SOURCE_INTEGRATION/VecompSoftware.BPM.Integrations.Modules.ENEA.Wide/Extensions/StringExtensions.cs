using System.Linq;

namespace VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string @this, int maxLength)
        {
            @this = @this ?? "";
            return string.Join("", @this.Take(maxLength));
        }
    }
}
