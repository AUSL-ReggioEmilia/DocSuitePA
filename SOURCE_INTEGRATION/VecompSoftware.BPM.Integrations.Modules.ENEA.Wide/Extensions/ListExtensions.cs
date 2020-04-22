using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.Extensions
{
    public static class ListExtensions
    {
        public static T SecondOrDefault<T>(this IList<T> @this)
        {
            if (@this.Count >= 2)
            {
                return @this[1];
            }

            return default(T);
        }

        public static T ThirdOrDefault<T>(this IList<T> @this)
        {
            if (@this.Count >= 3)
            {
                return @this[2];
            }

            return default(T);
        }

        public static T ForthOrDefault<T>(this IList<T> @this)
        {
            if (@this.Count >= 4)
            {
                return @this[3];
            }

            return default(T);
        }
    }
}
