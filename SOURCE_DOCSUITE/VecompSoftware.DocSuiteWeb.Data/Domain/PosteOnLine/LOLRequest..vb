''' <summary> Richiesta lettera. </summary>
Public Class LOLRequest
    Inherits POLRequest

#Region " Fields "

    Private m_DocumentName As String
    Private m_DocumentMD5 As String
    Private m_DocumentPosteMD5 As String
    Private m_DocumentPosteFileType As String

#End Region

#Region " Properties "

    Public Overridable Property DocumentName() As String
        Get
            Return m_DocumentName
        End Get
        Set(ByVal value As String)
            m_DocumentName = value
        End Set
    End Property

    Public Overridable Property DocumentMD5() As String
        Get
            Return m_DocumentMD5
        End Get
        Set(ByVal value As String)
            m_DocumentMD5 = value
        End Set
    End Property

    Public Overridable Property DocumentPosteMD5() As String
        Get
            Return m_DocumentPosteMD5
        End Get
        Set(ByVal value As String)
            m_DocumentPosteMD5 = value
        End Set
    End Property

    Public Overridable Property DocumentPosteFileType() As String
        Get
            Return m_DocumentPosteFileType
        End Get
        Set(ByVal value As String)
            m_DocumentPosteFileType = value
        End Set
    End Property

    Public Overridable Property IdArchiveChain As Guid?
    Public Overridable Property IdArchiveChainPoste As Guid?

#End Region

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

End Class