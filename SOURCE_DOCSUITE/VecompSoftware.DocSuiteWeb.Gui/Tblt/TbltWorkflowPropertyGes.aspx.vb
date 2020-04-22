Public Class TbltWorkflowPropertyGes
    Inherits CommonBasePage

#Region " Properties "
    Public ReadOnly Property PageAction As String
        Get
            Return GetKeyValue(Of String, TbltWorkflowPropertyGes)("Action")
        End Get
    End Property

    Public ReadOnly Property ArgumentType As String
        Get
            Return GetKeyValue(Of String, TbltWorkflowPropertyGes)("Argument")
        End Get
    End Property

    Public ReadOnly Property Validation As Boolean
        Get
            Return GetKeyValue(Of Boolean, TbltWorkflowPropertyGes)("Validation")
        End Get
    End Property
#End Region
#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MasterDocSuite.TitleVisible = False

    End Sub
#End Region

End Class