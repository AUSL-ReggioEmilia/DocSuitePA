Imports System.Xml.Serialization
Imports System.Xml

<XmlRootAttribute("ProtocolPecs")>
Public Class ProtocolPecsXML

    Private _year As Short
    Private _number As Integer
    Private _pecs As List(Of PECXML)

    <XmlAttribute("year")>
    Public Property Year() As Short
        Get
            Return _year
        End Get
        Set(value As Short)
            _year = value
        End Set
    End Property

    <XmlAttribute("number")>
    Public Property Number() As Integer
        Get
            Return _number
        End Get
        Set(value As Integer)
            _number = value
        End Set
    End Property

    <XmlElement("Pec")>
    Public Property Pecs() As List(Of PECXML)
        Get
            Return _pecs
        End Get
        Set(value As List(Of PECXML))
            _pecs = value
        End Set
    End Property
End Class

<XmlRoot("Pec")>
Public Class PECXML

    Private _id As Integer
    Private _mailbox As Mailbox
    Private _direction As Integer
    Private _subject As String
    Private _sender As String
    Private _recipients As List(Of RecipientXML)
    Private _timestamp As String
    Private _certificationData As CertificationData
    Private _body As String
    Private _receipts As List(Of Receipt)

    <XmlAttribute("id")>
    Public Property Id() As Integer
        Get
            Return _id
        End Get
        Set(value As Integer)
            _id = value
        End Set
    End Property

    <XmlAttribute("direction")>
    Public Property Direction() As Integer
        Get
            Return _direction
        End Get
        Set(value As Integer)
            _direction = value
        End Set
    End Property

    <XmlElement("Mailbox")>
    Public Property Mailbox() As Mailbox
        Get
            Return _mailbox
        End Get
        Set(value As Mailbox)
            _mailbox = value
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

    <XmlElement("Sender")>
    Public Property Sender() As String
        Get
            Return _sender
        End Get
        Set(value As String)
            _sender = value
        End Set
    End Property

    <XmlArray("Recipients"), XmlArrayItem("Recipient")>
    Public Property Recipients() As List(Of RecipientXML)
        Get
            Return _recipients
        End Get
        Set(value As List(Of RecipientXML))
            _recipients = value
        End Set
    End Property

    <XmlElement("Timestamp")>
    Public Property Timestamp() As String
        Get
            Return _timestamp
        End Get
        Set(value As String)
            _timestamp = value
        End Set
    End Property

    <XmlElement("CertificationData")>
    Public Property CertificationData() As CertificationData
        Get
            Return _certificationData
        End Get
        Set(value As CertificationData)
            _certificationData = value
        End Set
    End Property

    <XmlElement("Body")>
    Public Property Body() As String
        Get
            Dim xmld As New XmlDocument()
            Return xmld.CreateCDataSection(_body).OuterXml
        End Get
        Set(value As String)
            _body = value
        End Set
    End Property

    <XmlArray("Receipts"), XmlArrayItem("Receipt")>
    Public Property Receipts() As List(Of Receipt)
        Get
            Return _receipts
        End Get
        Set(value As List(Of Receipt))
            _receipts = value
        End Set
    End Property
End Class

<XmlRoot("Recipient")>
Public Class RecipientXML
    Private _name As String
    Private _type As String

    <XmlAttribute("name")>
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
        End Set
    End Property

    <XmlAttribute("type")>
    Public Property Type() As String
        Get
            Return _type
        End Get
        Set(value As String)
            _type = value
        End Set
    End Property
End Class

Public Class CertificationData

    Private _isCertified As Boolean
    Private _isInterop As Boolean

    <XmlAttribute("isCertified")>
    Public Property IsCertified() As Boolean
        Get
            Return _isCertified
        End Get
        Set(value As Boolean)
            _isCertified = value
        End Set
    End Property

    <XmlAttribute("isInterop")>
    Public Property IsInterop() As Boolean
        Get
            Return _isInterop
        End Get
        Set(value As Boolean)
            _isInterop = value
        End Set
    End Property
End Class

Public Class Mailbox

    Private _id As Integer
    Private _associatedMail As String

    <XmlAttribute("id")>
    Public Property Id() As Integer
        Get
            Return _id
        End Get
        Set(value As Integer)
            _id = value
        End Set
    End Property

    <XmlAttribute("associatedMail")>
    Public Property AssociatedMail() As String
        Get
            Return _associatedMail
        End Get
        Set(value As String)
            _associatedMail = value
        End Set
    End Property
End Class

<XmlRoot("Receipt")>
Public Class Receipt

    Private _timestamp As String
    Private _type As String
    Private _value As String

    <XmlAttribute("timestamp")>
    Public Property Timestamp() As String
        Get
            Return _timestamp
        End Get
        Set(value As String)
            _timestamp = value
        End Set
    End Property

    <XmlAttribute("type")>
    Public Property Type() As String
        Get
            Return _type
        End Get
        Set(value As String)
            _type = value
        End Set
    End Property

    <XmlText>
    Public Property Value() As String
        Get
            Return _value
        End Get
        Set(value As String)
            _value = value
        End Set
    End Property
End Class

'<?xml version="1.0" encoding="utf-8"?>
'<ProtocolPecs year="2013" number="42">
'    <pec id="42">
'        <mailbox id="2" associatedMail="mail3@server.it" />
'        <direction>0</direction>
'        <subject>Oggetto della PEC</subject>
'        <sender>mail@server.it</sender>
'        <recipients>
'            <recipient name="mail1@server.it" type="to" />
'            <recipient name="mail2@server.it" type="cc"/>
'            <recipient name="mail3@server.it" type="ccn"/>
'        </recipients >
'        <timestamp>2012-11-20 14:30</timestamp>
'        <certificationData isCertified="true" isInterop="false"/>
'        <body><![CDATA[]]></body>
'        <receipts>
'            <receipt timestamp="2012-11-20 15:30" type="accettazione"></receipt>
'            <receipt timestamp="2012-11-20 16:30" type="avvenuta-consegna"></receipt>
'            <receipt timestamp="2012-11-20 17:30" type="avvenuta-consegna">Dettagli errori</receipt>
'        </receipts>
'    </pec>
'    <pec id="88">
'        ...
'    </pec>
'</ProtocolPecs>
