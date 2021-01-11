Imports VecompSoftware.DocSuiteWeb.Data

Public Class ResolutionGridBarController
    Inherits BaseGridBarController

    Public Enum BarWorkflowStep As Short
        Adotta = 0
        AvvenutaAdozione = 1
        TrasmissioneServizi = 2
        TrasmissioneOc = 3
        Esecutiva = 6
        TrasmissioneCollegioSindacaleFirmaDigitale = 9
        Pubblicazione = 10
        StampaUltimaPagina = 11
        DaAffariGenerali = 12
    End Enum

#Region "Configurations"
    Private Const [DEFAULT] As String = ""
    Private Const ASL_TO2 As String = "ASL3-TO"
#End Region

#Region "Constructors"
    Public Sub New(ByRef gridBar As IGridBar)
        MyBase.New(gridBar)
        LoadConfiguration([DEFAULT])
    End Sub

    Public Sub New(ByRef gridBar As IGridBar, ByVal [default] As Boolean, ByVal region As Boolean, ByVal workflow As Boolean)
        MyBase.New(gridBar)
        EnableLeft = workflow
        EnableMiddle = region
        EnableRight = [default]
    End Sub
#End Region

#Region "Virtual Sub Implementation"
    Public Overrides Sub LoadConfiguration(ByVal config As String)
        If DocSuiteContext.Current.ResolutionEnv.ShowMassiveResolutionSearchPageEnabled Then
            EnableLeft = HasWorkflow 'viene abilitato solo nel caso sia da gestire il workflow
            EnableMiddle = True
            EnableRight = True
        Else
            EnableLeft = False
            EnableMiddle = False
            EnableRight = True
        End If
    End Sub
#End Region

End Class
