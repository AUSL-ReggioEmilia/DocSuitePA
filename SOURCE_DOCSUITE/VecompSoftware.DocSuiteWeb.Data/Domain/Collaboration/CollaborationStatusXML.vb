Imports System.Xml.Serialization

<XmlRootAttribute("CollaborationStatus")>
Public Class CollaborationStatusXML

    Private _type As String
    Private _collNumber As Int32
    Private _stato As Status
    Private _signers As List(Of Signatory)
    Private _refs As List(Of Ref)

    <XmlAttribute("type")>
    Public Property Type() As String
        Get
            Return _type
        End Get
        Set(value As String)
            _type = value
        End Set
    End Property

    <XmlAttribute("collNumber")>
    Public Property CollNumber() As Int32
        Get
            Return _collNumber
        End Get
        Set(value As Int32)
            _collNumber = value
        End Set
    End Property

    <XmlElement("Status")>
    Public Property Stato As Status
        Get
            Return _stato
        End Get
        Set(value As Status)
            _stato = value
        End Set
    End Property

    <XmlArray("Signers"), XmlArrayItem("Signer")>
    Public Property Signers() As List(Of Signatory)
        Get
            Return _signers
        End Get
        Set(value As List(Of Signatory))
            _signers = value
        End Set
    End Property

    <XmlArray("Refs"), XmlArrayItem("Ref")>
    Public Property Refs() As List(Of Ref)
        Get
            Return _refs
        End Get
        Set(value As List(Of Ref))
            _refs = value
        End Set
    End Property

End Class

Public Class Status

    Private _id As Integer
    Private _valore As String

    <XmlAttribute("id")>
    Public Property Id() As Integer
        Get
            Return _id
        End Get
        Set(value As Integer)
            _id = value
        End Set
    End Property

    <XmlAttribute("value")>
    Public Property Valore() As String
        Get
            Return _valore
        End Get
        Set(value As String)
            _valore = value
        End Set
    End Property

End Class

<XmlRoot("Signer")>
Public Class Signatory

    Private _incremental As Integer
    Private _userName As String
    Private _signDate As String
    Private _isActive As Boolean

    <XmlAttribute("incremental")>
    Public Property Incremental() As Integer
        Get
            Return _incremental
        End Get
        Set(value As Integer)
            _incremental = value
        End Set
    End Property

    <XmlAttribute("userName")>
    Public Property UserName() As String
        Get
            Return _userName
        End Get
        Set(value As String)
            _userName = value
        End Set
    End Property

    <XmlAttribute("signDate")>
    Public Property SignDate() As String
        Get
            Return _signDate
        End Get
        Set(value As String)
            _signDate = value
        End Set
    End Property

    <XmlAttribute("isActive")>
    Public Property IsActive() As Boolean
        Get
            Return _isActive
        End Get
        Set(value As Boolean)
            _isActive = value
        End Set
    End Property

End Class

<XmlRoot("Ref")>
Public Class Ref

    Private _type As String
    Private _id As String

    <XmlAttribute("type")>
    Public Property Type() As String
        Get
            Return _type
        End Get
        Set(value As String)
            _type = value
        End Set
    End Property

    <XmlAttribute("id")>
    Public Property Id() As String
        Get
            Return _id
        End Get
        Set(value As String)
            _id = value
        End Set
    End Property
End Class