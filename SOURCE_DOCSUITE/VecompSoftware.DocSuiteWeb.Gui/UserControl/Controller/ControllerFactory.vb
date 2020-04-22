Imports VecompSoftware.DocSuiteWeb.Data

Public Class ControllerFactory

    Public Shared Function CreateResolutionChangerController(ByRef control As uscResolutionChange) As IChangerController(Of Resolution)
        If DocSuiteContext.Current.ResolutionEnv.CustomControllerFactoryEnabled Then
            Select Case DocSuiteContext.Current.ResolutionEnv.Configuration
                Case "ASL3-TO"
                    Return New ResolutionChangeControllerTO(control)
                Case "AUSL-PC"
                    Return New ResolutionChangeControllerPC(control)
                Case Else
                    Return New ResolutionChangeController(control)
            End Select
        End If
        Return New ResolutionChangeController(control)
    End Function

    Public Shared Function CreateResolutionDisplayController(ByRef control As uscResolution, ByRef uscBar As uscResolutionBar) As IDisplayController

        If DocSuiteContext.Current.ResolutionEnv.CustomControllerFactoryEnabled Then
            Select Case DocSuiteContext.Current.ResolutionEnv.Configuration
                Case "ASL3-TO"
                    Return New ResolutionDisplayControllerTO(control, uscBar)
                Case "AUSL-PC"
                    Return New ResolutionDisplayControllerPC(control, uscBar)
                Case Else
                    Return New ResolutionDisplayController(control, uscBar)
            End Select
        End If

        Return New ResolutionDisplayController(control, uscBar)
    End Function

    Public Shared Function CreateResolutionFinderController(ByRef control As uscResolutionFinder) As BaseResolutionFinderController

        If DocSuiteContext.Current.ResolutionEnv.CustomControllerFactoryEnabled Then
            Select Case DocSuiteContext.Current.ResolutionEnv.Configuration
                Case "ASL3-TO"
                    Return New ResolutionFinderControllerTO(control)
                Case "AUSL-PC"
                    Return New ResolutionFinderControllerPC(control)
                Case Else
                    Return New ResolutionFinderController(control)
            End Select
        End If

        Return New ResolutionFinderController(control)
    End Function

End Class
