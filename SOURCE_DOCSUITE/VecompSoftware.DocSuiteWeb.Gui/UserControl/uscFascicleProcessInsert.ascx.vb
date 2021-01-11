Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Model.Metadata

Public Class uscFascicleProcessInsert
    Inherits DocSuite2008BaseControl

#Region " Fields "

#End Region

#Region " Properties "
    Public ReadOnly Property PageContentDiv As Control
        Get
            Return pnlContent
        End Get
    End Property

    Public Property ValidationDisabled As Boolean
#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        uscContact.FilterByParentId = ProtocolEnv.FascicleContactId

        uscMetadataRepositorySel.FascicleInsertCommonIdEvent = PageContentDiv.ClientID

        uscDynamicMetadataRest.ValidationEnabled = Not ValidationDisabled

        uscCategory.ShowProcesses = ProtocolEnv.ProcessEnabled
    End Sub
#End Region

#Region " Methods "
#End Region

End Class