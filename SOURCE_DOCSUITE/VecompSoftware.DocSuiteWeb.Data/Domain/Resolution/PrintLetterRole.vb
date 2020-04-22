Imports VecompSoftware.Helpers.NHibernate

<Serializable()>
Public Class PrintLetterRoleCompositeKey
    Inherits Duplet(Of String, String)

#Region "Constructor"
    Public Sub New()
        MyBase.New()
    End Sub
#End Region

#Region "Properties"
    Public Overridable Property IdCode As String
        Get
            Return First
        End Get
        Set(value As String)
            First = value
        End Set
    End Property

    Public Overridable Property Description As String
        Get
            Return Second
        End Get
        Set(value As String)
            Second = value
        End Set
    End Property
#End Region

#Region "Methods"
    Public Overrides Function ToString() As String
        Return String.Format("IdCode: {0} , Description: {1}", IdCode, Description)
    End Function
#End Region

End Class

<Serializable()>
Public Class PrintLetterRole
    Inherits DomainObject(Of PrintLetterRoleCompositeKey)

#Region "Constructor"
    Public Sub New()
        Id = New PrintLetterRoleCompositeKey()
    End Sub
#End Region

End Class
