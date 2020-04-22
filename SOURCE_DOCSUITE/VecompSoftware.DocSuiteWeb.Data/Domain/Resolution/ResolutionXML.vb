Imports System.Xml.Serialization

''' <summary> classe per il parsing XML dell'oggetto Protocol. </summary>
<XmlRootAttribute("Resolution")>
Public Class ResolutionXML
    Inherits CollaborationXmlData

    <XmlAttribute("Id")>
    Public Property Id() As Integer

    <XmlAttribute("Year")>
    Public Property Year() As Integer

    <XmlAttribute("Number")>
    Public Property Number() As Integer

    <XmlElement("Type")>
    Public Property Type() As Int32

    <XmlElement("Container")>
    Public Property Container() As Int32

    <XmlElement("Data")>
    Public Property Data As String

    <XmlElement("Object")>
    Public Property [Object]() As String

    <XmlArray("Authorizations"), XmlArrayItem("RoleId")>
    Public Property Authorizations() As List(Of Int32)

    <XmlElement("Category")>
    Public Property Category() As Int32

    <XmlElement("Notes")>
    Public Property Notes() As String

    <XmlElement("Assignee")>
    Public Property Assignee() As String

    <XmlElement("ServiceCode")>
    Public Property ServiceCode() As String

    <XmlElement("IdStatus")>
    Public Property IdStatus() As Int32

    <XmlElement("Documents")>
    Public Property Document() As DocumentsXml

    ''' <summary>
    ''' Definisce se deve essere aggiornato il protocollo dell'invio ai servizi
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlElement("UpdateProposerProtocolLink")>
    Public Property UpdateProposerProtocolLink() As Boolean

    ''' <summary>
    ''' Valore del protocollo di invio ai servizi per effettuare l'aggiornamento degli atti
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlElement("ProposerProtocolLink")>
    Public Property ProposerProtocolLink() As String

    ''' <summary>
    ''' Definisce se deve essere aggiornato protocollo e data dell'invio al collegio sindacale
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlElement("UpdateCollegioSindacaleProtocolLink")>
    Public Property UpdateCollegioSindacaleProtocolLink() As Boolean

    ''' <summary>
    ''' Valore del protocollo di invio al collegio sindacale per effettuare l'aggiornamento degli atti
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlElement("CollegioSindacaleProtocolLink")>
    Public Property CollegioSindacaleProtocolLink() As String

    ''' <summary>
    ''' Valore della data di invio al collegio sindacale per effettuare l'aggiornamento degli atti
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlElement("CollegioSindacaleDate")>
    Public Property CollegioSindacaleDate() As Date?

    ''' <summary>
    ''' Definisce se deve essere aggiornato protocollo dell'invio pubblicazione e lettera firma digitale
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlElement("UpdateInvioPubbLetteraFirmaDigitaleProtocolLink")>
    Public Property UpdateInvioPubbLetteraFirmaDigitaleProtocolLink() As Boolean
    ''' <summary>
    ''' Valore del protocollo di invio pubblicazione e lettera firma digitale per effettuare l'aggiornamento degli atti
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlElement("InvioPubbLetteraFirmaDigitaleProtocolLink")>
    Public Property InvioPubbLetteraFirmaDigitaleProtocolLink() As String

    ''' <summary>
    ''' Valore della data di invio pubblicazione e lettera firma digitale per effettuare l'aggiornamento degli atti
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlElement("InvioPubbLetteraFirmaDigitaleDate")>
    Public Property InvioPubbLetteraFirmaDigitaleDate() As Date?

    <XmlElement("ResolutionsToUpdate")>
    Public Property ResolutionsToUpdate() As List(Of Int32)

    Public Function GetResolution() As Resolution
        Dim resl As New Resolution()

        resl.ResolutionObject = Me.Object
        resl.Note = Notes
        resl.Container = New Container() With {.Id = Container}

        Return resl
    End Function

End Class
