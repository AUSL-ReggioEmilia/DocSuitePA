Imports Microsoft.AspNet.SignalR
Imports Microsoft.Owin
Imports Microsoft.Owin.Cors
Imports Owin

<Assembly: OwinStartup(GetType(VecompSoftware.DocSuiteWeb.Gui.Startup))>
Public Class Startup
    Public Sub Configuration(app As IAppBuilder)
        app.Map("/signalr", Sub(map)
                                map.UseCors(CorsOptions.AllowAll)
                                Dim hubConfiguration As HubConfiguration = New HubConfiguration() With {.EnableJSONP = True}
#If DEBUG Then
                                hubConfiguration.EnableDetailedErrors = True
#End If
                                map.RunSignalR(hubConfiguration)
                            End Sub)
    End Sub
End Class
