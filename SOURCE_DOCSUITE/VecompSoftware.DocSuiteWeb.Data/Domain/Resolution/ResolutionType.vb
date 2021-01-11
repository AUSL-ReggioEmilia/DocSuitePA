<Serializable()> _
Public Class ResolutionType
    Inherits DomainObject(Of Short)

    Public Enum UploadDocumentType
        Allegato = 0
        AllegatoRiservato = 1
        Frontespizio = 2
        Pubblicazione = 3
    End Enum

#Region " Fields  "

    ''' <summary> Identificativo delle delibere. </summary>
    ''' <remarks> Codice hardcoded necessario per motivi legacy. Limitarne l'uso. </remarks>
    Public Const IdentifierDelibera As Short = 1S
    ''' <summary> Identificativo delle determine, atti o disposizioni. </summary>
    ''' <remarks> Codice hardcoded necessario per motivi legacy. Limitarne l'uso. </remarks>
    Public Const IdentifierDetermina As Short = 0S

    Private _description As String
    Private _tabMaster As IList(Of TabMaster) = Nothing

#End Region

#Region " Properties "

    Public Overridable Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Public Overridable Property TabMaster() As IList(Of TabMaster)
        Get
            Return _tabMaster
        End Get
        Set(ByVal value As IList(Of TabMaster))
            _tabMaster = value
        End Set
    End Property

#End Region

#Region " Constructor "

    Public Sub New()
    End Sub

#End Region

End Class

