Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Data

Public Class UtltLocations
    Inherits SuperAdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            If DocSuiteContext.Current.IsDocumentEnabled Then
                environmentsRadioButtonList.Items.Add(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.DocmDB))
            End If
            If DocSuiteContext.Current.IsProtocolEnabled Then
                environmentsRadioButtonList.Items.Add(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB))
            End If
            If DocSuiteContext.Current.IsResolutionEnabled Then
                environmentsRadioButtonList.Items.Add(System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ReslDB))
            End If
        End If
    End Sub

    Protected Sub environmentSelection(ByVal sender As Object, ByVal e As EventArgs)
        ' ritiro la stringa per ritirare le location
        Dim locationName As String = environmentsRadioButtonList.SelectedValue
        ' lista delle locations
        Dim locations As IList(Of Location)

        locations = Facade.LocationFacade.GetAll(locationName)

        If locations.Count > 0 Then
            dgLocation.DataSource = locations
            dgLocation.DataBind()
        End If
    End Sub

    Protected Function DsArchive() As String
        Dim ris As EnvironmentDataCode = DirectCast(System.Enum.Parse(GetType(EnvironmentDataCode), environmentsRadioButtonList.SelectedValue), EnvironmentDataCode)
        Select Case ris
            Case EnvironmentDataCode.DocmDB
                Return "DocmBiblosDSDB"
            Case EnvironmentDataCode.ProtDB
                Return "ProtBiblosDSDB"
            Case EnvironmentDataCode.ReslDB
                Return "ReslBiblosDSDB"
        End Select
        Throw New DocSuiteException("Locazioni", "Caso non previsto.")
    End Function

End Class