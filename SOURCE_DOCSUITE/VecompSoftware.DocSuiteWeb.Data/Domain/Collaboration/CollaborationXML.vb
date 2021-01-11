Imports System.Xml.Serialization

#Region " XMLRoot - CollaborationXML"

''' <summary>
''' Classe che mappa xml con parametri della Collaborazione per WSColl
''' </summary>
''' <remarks></remarks>
<XmlRootAttribute("Collaboration")>
Public Class CollaborationXML

    Private _modality As String
    Private _type As String
    Private _priority As String
    Private _signers As List(Of Signer)
    Private _workers As List(Of Worker)
    Private _reminderDate As String
    Private _subject As String
    Private _notes As String
    Private _attributes As List(Of CollaborationXmlData)

    <XmlAttribute("modality")>
    Public Property Modality() As String
        Get
            Return _modality
        End Get
        Set(value As String)
            _modality = value
        End Set
    End Property

    <XmlElement("Type")>
    Public Property [Type]() As String
        Get
            Return _type
        End Get
        Set(value As String)
            _type = value
        End Set
    End Property

    <XmlElement("Priority")>
    Public Property Priority() As String
        Get
            Return _priority
        End Get
        Set(value As String)
            _priority = value
        End Set
    End Property

    <XmlArray("Signers"), XmlArrayItem("Signer")>
    Public Property Signers() As List(Of Signer)
        Get
            Return _signers
        End Get
        Set(value As List(Of Signer))
            _signers = value
        End Set
    End Property

    <XmlArray("Workers"), XmlArrayItem("Worker")>
    Public Property Workers() As List(Of Worker)
        Get
            Return _workers
        End Get
        Set(value As List(Of Worker))
            _workers = value
        End Set
    End Property

    <XmlElement("ReminderDate")>
    Public Property ReminderDate() As String
        Get
            Return _reminderDate
        End Get
        Set(value As String)
            _reminderDate = value
        End Set
    End Property

    <XmlElement("Subject")>
    Public Property Subject() As String
        Get
            Return _subject
        End Get
        Set(value As String)
            _subject = value
        End Set
    End Property

    <XmlElement("Notes")>
    Public Property Notes() As String
        Get
            Return _notes
        End Get
        Set(value As String)
            _notes = value
        End Set
    End Property

    <XmlArray("Attributes"), XmlArrayItem("Protocol", GetType(ProtocolXML)), XmlArrayItem("Resolution", GetType(ResolutionXML))>
    Public Property Attributes() As List(Of CollaborationXmlData)
        Get
            Return _attributes
        End Get
        Set(value As List(Of CollaborationXmlData))
            _attributes = value
        End Set
    End Property

    <XmlIgnore>
    Public Property TemplateName As String

End Class

#End Region

#Region " Worker "

''' <summary>
''' Utenti abilitati alla protocollazione
''' </summary>
''' <remarks></remarks>
<XmlRoot("Worker")>
Public Class Worker
    Private _userName As String
    Private _type As String

    <XmlAttribute("userName")>
    Public Property UserName() As String
        Get
            Return _userName
        End Get
        Set(value As String)
            _userName = value
        End Set
    End Property

    <XmlAttribute("type")>
    Public Property [Type]() As String
        Get
            Return _type
        End Get
        Set(value As String)
            _type = value
        End Set
    End Property
End Class

#End Region

#Region " Signer "

''' <summary>
''' Classe per i destinatari di firma
''' </summary>
''' <remarks></remarks>
<XmlRoot("Signer")>
Public Class Signer
    Private _role As Int32
    Private _userName As String
    Private _type As String
    Private _signRequired As Boolean

    <XmlAttribute("role")>
    Public Property Role() As Int32
        Get
            Return _role
        End Get
        Set(value As Int32)
            _role = value
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

    <XmlAttribute("type")>
    Public Property [Type]() As String
        Get
            Return _type
        End Get
        Set(value As String)
            _type = value
        End Set
    End Property

    <XmlAttribute("signRequired")>
    Public Property SignRequired() As Boolean
        Get
            Return _signRequired
        End Get
        Set(value As Boolean)
            _signRequired = value
        End Set
    End Property
End Class

#End Region