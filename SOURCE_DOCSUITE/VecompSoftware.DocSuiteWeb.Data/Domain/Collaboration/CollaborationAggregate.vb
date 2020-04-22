

<Serializable()>
Public Class CollaborationAggregate
    Inherits DomainObject(Of Guid)


#Region " Properties "

    Private _collaborationFather As Collaboration
    Private _collaborationChild As Collaboration
    Private _collaborationDocumentType As String

    Public Overridable Property CollaborationFather As Collaboration
        Get
            Return _collaborationFather
        End Get
        Set(ByVal value As Collaboration)
            _collaborationFather = value
        End Set
    End Property

    Public Overridable Property CollaborationChild As Collaboration
        Get
            Return _collaborationChild
        End Get
        Set(ByVal value As Collaboration)
            _collaborationChild = value
        End Set
    End Property

    Public Overridable Property CollaborationDocumentType As String
        Get
            Return _collaborationDocumentType
        End Get
        Set(ByVal value As String)
            _collaborationDocumentType = value
        End Set
    End Property



#End Region

#Region " Constructors "

    Public Sub New()
    End Sub



#End Region

End Class
