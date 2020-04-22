using System.Drawing;
using GhostscriptSharp;
using GhostscriptSharp.Settings;

namespace VecompSoftware.GhostscriptSharp
{
    public static class GhostscriptSettingsFactory
    {

        public static GhostscriptSettings Default()
        {
            // FG20131016: Verificare come si comporta con documenti con formato di pagina non omogeneo.
            var settings = new GhostscriptSettings()
            {
                Device = GhostscriptDevices.png256,
                Resolution = new Size(300, 300),
                Size = new GhostscriptPageSize() { Native = GhostscriptPageSizes.a4 }
            };
            settings.Page.AllPagesWorkaround();
            return settings;
        }

        public static GhostscriptSettings Tesseract()
        {
            // FG20131016: Verificare come si comporta con documenti con formato di pagina non omogeneo.
            var settings = new GhostscriptSettings()
            {
                Device = GhostscriptDevices.jpeggray,
                Resolution = new Size(300, 300),
                Size = new GhostscriptPageSize() { Native = GhostscriptPageSizes.a4 }
            };
            settings.Page.AllPagesWorkaround();
            return settings;
        }
        public static GhostscriptSettings Thumbnail()
        {
            // FG20131016: Verificare come si comporta con documenti con formato di pagina non omogeneo.
            var settings = new GhostscriptSettings()
            {
                Device = GhostscriptDevices.pngmono,
                Resolution = new Size(72, 72),
                Size = new GhostscriptPageSize() { Native = GhostscriptPageSizes.a4 }
            };
            settings.Page.AllPagesWorkaround();
            return settings;
        }

    }
}
