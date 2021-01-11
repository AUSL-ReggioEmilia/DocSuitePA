using System;
using GhostscriptSharp;
using GhostscriptSharp.Settings;

namespace VecompSoftware.GhostscriptSharp
{
    public static class GhostscriptSettingsExtensions
    {

        public static string GetDeviceExtension(this GhostscriptSettings source)
        {
            switch (source.Device)
            {
                case GhostscriptDevices.bmpgray:
                    return ".bmp";
                case GhostscriptDevices.jpeg:
                case GhostscriptDevices.jpeggray:
                    return ".jpg";
                case GhostscriptDevices.pnggray:
                case GhostscriptDevices.pngmono:
                case GhostscriptDevices.png256:
                    return ".png";
                default:
                    break;
            }

            throw new NotImplementedException("Estensione non implementata per il GhostscriptDevices specificato.");
        }

    }
}
