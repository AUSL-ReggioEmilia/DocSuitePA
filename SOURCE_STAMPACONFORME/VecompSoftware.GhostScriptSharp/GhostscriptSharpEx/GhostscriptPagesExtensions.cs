using System;
using GhostscriptSharp.Settings;

namespace VecompSoftware.GhostscriptSharp
{
    public static class GhostscriptPagesExtensions
    {

        public static GhostscriptPages SetPages(this GhostscriptPages source, int start, int end)
        {
            if (source == null)
                source = new GhostscriptPages();

            source.Start = start;
            source.End = end;
            return source;
        }
        public static GhostscriptPages SinglePage(this GhostscriptPages source, int pageNumber)
        {
            if (pageNumber < 1)
                throw new InvalidOperationException("Il numero di pagina deve essere maggiore di 0.");

            return source.SetPages(pageNumber, pageNumber);
        }
        public static GhostscriptPages FirstPageOnly(this GhostscriptPages source)
        {
            return source.SinglePage(1);
        }
        public static GhostscriptPages AllPagesWorkaround(this GhostscriptPages source)
        {
            // FG20131016: Attenzione, questo è un workaround poichè la property "AllPages" non funziona correttamente.
            return source.SetPages(1, -1);
        }

    }
}
