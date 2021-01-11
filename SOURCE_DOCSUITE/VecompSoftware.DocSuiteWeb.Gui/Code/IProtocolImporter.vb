Imports System.IO
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Xml

Public Interface IProtocolImporter

    Function Import(ByVal xml As XmlDocument, ByVal pdf As FileInfo, ByVal template As ProtocolTemplate) As Protocol

    ReadOnly Property ClassificationManaged() As Boolean

End Interface