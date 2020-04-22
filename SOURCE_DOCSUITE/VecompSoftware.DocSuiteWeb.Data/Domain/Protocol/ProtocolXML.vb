Imports System.Xml.Serialization

''' <summary> classe per il parsing XML dell'oggetto Protocol. </summary>
<XmlRootAttribute("Protocol")>
Public Class ProtocolXML
    Inherits CollaborationXmlData

    Private _data As String
    Private _senders As List(Of ContactBag)
    Private _recipients As List(Of ContactBag)
    Private _object As String
    Private _authorizations As List(Of Int32)
    Private _category As Int32
    Private _notes As String
    Private _assignee As String
    Private _serviceCode As String
    Private _idStatus As Int32
    Private _document As DocumentsXml
    Private _protocolPecs As ProtocolPecsXML

    <XmlAttribute("Year")>
    Public Property Year() As Integer

    <XmlAttribute("Number")>
    Public Property Number() As Integer

    <XmlElement("Type")>
    Public Property Type() As Int32

    <XmlElement("Container")>
    Public Property Container() As Int32

    <XmlElement("DocumentType")>
    Public Property DocumentType() As Int32

    <XmlElement("RegistrationDate")>
    Public Property RegistrationDate As DateTimeOffset

    <XmlElement("Data")>
    Public Property Data As String
        Get
            Return _data
        End Get
        Set(value As String)
            _data = value
        End Set
    End Property

    <XmlArray("Senders"), XmlArrayItem("ContactBag")>
    Public Property Senders() As List(Of ContactBag)
        Get
            Return _senders
        End Get
        Set(value As List(Of ContactBag))
            _senders = value
        End Set
    End Property

    <XmlArray("Recipients"), XmlArrayItem("ContactBag")>
    Public Property Recipients() As List(Of ContactBag)
        Get
            Return _recipients
        End Get
        Set(value As List(Of ContactBag))
            _recipients = value
        End Set
    End Property

    <XmlElement("Object")>
    Public Property [Object]() As String
        Get
            Return _object
        End Get
        Set(value As String)
            _object = value
        End Set
    End Property

    <XmlArray("Authorizations"), XmlArrayItem("RoleId")>
    Public Property Authorizations() As List(Of Int32)
        Get
            Return _authorizations
        End Get
        Set(value As List(Of Int32))
            _authorizations = value
        End Set
    End Property

    <XmlElement("Category")>
    Public Property Category() As Int32
        Get
            Return _category
        End Get
        Set(value As Int32)
            _category = value
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

    <XmlElement("Assignee")>
    Public Property Assignee() As String
        Get
            Return _assignee
        End Get
        Set(value As String)
            _assignee = value
        End Set
    End Property

    <XmlElement("ServiceCode")>
    Public Property ServiceCode() As String
        Get
            Return _serviceCode
        End Get
        Set(value As String)
            _serviceCode = value
        End Set
    End Property

    <XmlElement("Status")>
    Public Property Status As String


    <XmlElement("IdStatus")>
    Public Property IdStatus() As Int32
        Get
            Return _idStatus
        End Get
        Set(value As Int32)
            _idStatus = value
        End Set
    End Property

    <XmlElement("Documents")>
    Public Property Document() As DocumentsXml
        Get
            Return _document
        End Get
        Set(value As DocumentsXml)
            _document = value
        End Set
    End Property

    <XmlElement("ProtocolPecs")>
    Public Property ProtocolPecs As ProtocolPecsXML
        Get
            Return _protocolPecs
        End Get
        Set(value As ProtocolPecsXML)
            _protocolPecs = value
        End Set
    End Property

    <XmlArray("WorkflowMetadatas")>
    Public Property WorkflowMetadatas() As List(Of WorkflowMetadataXml)

    Public Function GetProtocol() As Protocol
        Dim prot As New Protocol()

        prot.ProtocolObject = Me.Object
        prot.Note = Notes
        prot.Container = New Container() With {.Id = Container}

        Return prot
    End Function

End Class

<XmlRoot("ContactBag")>
Public Class ContactBag
    <XmlAttribute("sourceType")>
    Public Property SourceType() As Int32

    <XmlElement("Contact")>
    Public Property Contacts() As List(Of ContactXML)
End Class

<XmlRoot("Contact")>
Public Class ContactXML

    Private _id As Int32
    Private _type As String
    Private _cc As Boolean
    Private _surname As String
    Private _name As String
    Private _description As String
    Private _standardMail As String
    Private _certifiedMail As String
    Private _fiscalCode As String
    Private _address As AddressXML
    Private _telephone As String
    Private _fax As String
    Private _notes As String
    Private _birthDate As String
    Private _birthPlace As String


    <XmlAttribute("type")>
    Public Property Type() As String
        Get
            Return _type
        End Get
        Set(value As String)
            _type = value
        End Set
    End Property

    <XmlAttribute("cc")>
    Public Property Cc() As Boolean
        Get
            Return _cc
        End Get
        Set(value As Boolean)
            _cc = value
        End Set
    End Property

    <XmlElement("Id")>
    Public Property Id() As Int32
        Get
            Return _id
        End Get
        Set(value As Int32)
            _id = value
        End Set
    End Property

    <XmlElement("Surname")>
    Public Property Surname() As String
        Get
            Return _surname
        End Get
        Set(value As String)
            _surname = value
        End Set
    End Property

    <XmlElement("Name")>
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
        End Set
    End Property

    <XmlElement("Description")>
    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(value As String)
            _description = value
        End Set
    End Property

    <XmlElement("StandardMail")>
    Public Property StandardMail() As String
        Get
            Return _standardMail
        End Get
        Set(value As String)
            _standardMail = value
        End Set
    End Property

    <XmlElement("CertifiedMail")>
    Public Property CertifiedMail() As String
        Get
            Return _certifiedMail
        End Get
        Set(value As String)
            _certifiedMail = value
        End Set
    End Property

    <XmlElement("FiscalCode")>
    Public Property FiscalCode() As String
        Get
            Return _fiscalCode
        End Get
        Set(value As String)
            _fiscalCode = value
        End Set
    End Property

    <XmlElement("Address")>
    Public Property Address() As AddressXML
        Get
            Return _address
        End Get
        Set(value As AddressXML)
            _address = value
        End Set
    End Property

    <XmlElement("Telephone")>
    Public Property Telephone() As String
        Get
            Return _telephone
        End Get
        Set(value As String)
            _telephone = value
        End Set
    End Property

    <XmlElement("Fax")>
    Public Property Fax() As String
        Get
            Return _fax
        End Get
        Set(value As String)
            _fax = value
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

    <XmlElement("BirthDate")>
    Public Property BirthDate() As String
        Get
            Return _birthDate
        End Get
        Set(value As String)
            _birthDate = value
        End Set
    End Property

    <XmlElement("BirthPlace")>
    Public Property BirthPlace() As String
        Get
            Return _birthPlace
        End Get
        Set(value As String)
            _birthPlace = value
        End Set
    End Property
End Class

<XmlRoot("Address")>
Public Class AddressXML

    Private _type As String
    Private _name As String
    Private _number As String
    Private _cap As String
    Private _city As String
    Private _prov As String

    <XmlAttribute("type")>
    Public Property Type As String
        Get
            Return _type
        End Get
        Set(value As String)
            _type = value
        End Set
    End Property

    <XmlAttribute("name")>
    Public Property Name As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
        End Set
    End Property

    <XmlAttribute("number")>
    Public Property Number As String
        Get
            Return _number
        End Get
        Set(value As String)
            _number = value
        End Set
    End Property

    <XmlAttribute("cap")>
    Public Property Cap As String
        Get
            Return _cap
        End Get
        Set(value As String)
            _cap = value
        End Set
    End Property

    <XmlAttribute("city")>
    Public Property City As String
        Get
            Return _city
        End Get
        Set(value As String)
            _city = value
        End Set
    End Property

    <XmlAttribute("prov")>
    Public Property Prov As String
        Get
            Return _prov
        End Get
        Set(value As String)
            _prov = value
        End Set
    End Property
End Class
