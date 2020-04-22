Imports System.Web.Optimization

Public Class BundleConfig
    ' For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254726
    Public Shared Sub RegisterBundles(ByVal bundles As BundleCollection)
        bundles.Add(New StyleBundle("~/bundles/css").Include(
                        "~/Content/reset-fonts-grids.css",
                        "~/Content/style.css",
                        "~/Content/bindGrid.css",
                        "~/Content/comm.css",
                        "~/Content/desk.css",
                        "~/Content/desk.documentgridskin.css",
                        "~/Content/docm.css",
                        "~/Content/pec.css",
                        "~/Content/print.css",
                        "~/Content/prot.css",
                        "~/Content/fasc.css",
                        "~/Content/resl.css",
                        "~/Content/series.css",
                        "~/Content/uds.css",
                        "~/Content/dossier.css",
                        "~/Content/notification.css"))
        '"~/Content/report-generator.css"))

        bundles.Add(New StyleBundle("~/bundles/browserConditions").Include(
                        "~/Content/browserCondition/chrome.css",
                        "~/Content/browserCondition/firefox.css",
                        "~/Content/browserCondition/ie9.css",
                        "~/Content/browserCondition/ie10.css",
                        "~/Content/browserCondition/ie11.css"))

        BundleTable.EnableOptimizations = True

    End Sub
End Class
