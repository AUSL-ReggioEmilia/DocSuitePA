<Serializable()>
Public Class UnifiedDiary
    Inherits DomainObject(Of Int32)

#Region "Constructor"
    Public Sub New()

    End Sub
#End Region

#Region "Properties"

    'La proprietà Type era di tipo LogKindEnum, è stata modificata a Short poichè oltre ai valori presenti nell'enum, nella tabella LogKind sono presenti anche
    'i valori dell'environment delle singole UDS (valori dinamici che non possono essere inseriti staticamente nell'enum)
    Public Overridable Property Type As Integer

    Public Overridable Property LogDate As DateTime

    Public Overridable Property LogType As String

    Public Overridable Property LogDescription As String

    Public Overridable Property User As String

    Public Overridable Property Severity As Short?

    Public Overridable Property UDSId As Guid?

    Public Overridable Property IdUDSRepository As Guid?
#End Region

#Region "Navigation Properties"
    Public Overridable Property Collaboration As Collaboration

    Public Overridable Property Document As Document

    Public Overridable Property DocumentSeriesItem As DocumentSeriesItem

    Public Overridable Property Message As DSWMessage

    Public Overridable Property PecMail As PECMail

    Public Overridable Property Protocol As Protocol

    Public Overridable Property Resolution As Resolution
#End Region

End Class
