Imports System.Xml.Serialization

''' <summary> interfaccia per il parsing XML dell'oggetto Attributes. </summary>
Public Class CollaborationXmlData
End Class

''' <summary>
''' Descrive in XML gli identificativi di storage biblos di un documento/allegato
''' </summary>
''' <remarks></remarks>
<XmlRoot("BiblosDocumentInfo")>
Public Class BiblosDocumentInfoXml

    Private _server As String
    Private _archive As String
    Private _id As Integer
    Private _enum As Integer
    Private _chainId As String
    Private _documentId As String

    <XmlAttribute("server")>
    Public Property Server() As String
        Get
            Return _server
        End Get
        Set(value As String)
            _server = value
        End Set
    End Property

    <XmlAttribute("archive")>
    Public Property Archive() As String
        Get
            Return _archive
        End Get
        Set(value As String)
            _archive = value
        End Set
    End Property

    <XmlAttribute("id")>
    Public Property Id() As Integer
        Get
            Return _id
        End Get
        Set(value As Integer)
            _id = value
        End Set
    End Property

    <XmlAttribute("enum")>
    Public Property [Enum]() As Integer
        Get
            Return _enum
        End Get
        Set(value As Integer)
            _enum = value
        End Set
    End Property

    <XmlAttribute("chainID")>
    Public Property ChainId() As String
        Get
            Return _chainId
        End Get
        Set(value As String)
            _chainId = value
        End Set
    End Property

    <XmlAttribute("documentID")>
    Public Property DocumentId() As String
        Get
            Return _documentId
        End Get
        Set(value As String)
            _documentId = value
        End Set
    End Property
End Class

<XmlRoot("Documents")>
Public Class DocumentsXml
    Private _mainDocument As List(Of BiblosDocumentInfoXml)
    Private _attachments As List(Of BiblosDocumentInfoXml)

    <XmlArray("MainDocument"), XmlArrayItem("BiblosDocumentInfo")>
    Public Property MainDocument As List(Of BiblosDocumentInfoXml)
        Get
            Return _mainDocument
        End Get
        Set(value As List(Of BiblosDocumentInfoXml))
            _mainDocument = value
        End Set
    End Property

    <XmlArray("Attachments"), XmlArrayItem("BiblosDocumentInfo")>
    Public Property Attachments As List(Of BiblosDocumentInfoXml)
        Get
            Return _attachments
        End Get
        Set(value As List(Of BiblosDocumentInfoXml))
            _attachments = value
        End Set
    End Property
End Class