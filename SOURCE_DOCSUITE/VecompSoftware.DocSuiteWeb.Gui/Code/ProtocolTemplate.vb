Imports VecompSoftware.DocSuiteWeb.Data

Public Structure ProtocolTemplate
    Public Status As ProtocolStatus
    Public Type As ProtocolType
    Public DocumentType As DocumentType
    Public Location As Location
    Public AttachLocation As Location
    Public Container As Container
    Public Category As Category
    Public Note As String
End Structure