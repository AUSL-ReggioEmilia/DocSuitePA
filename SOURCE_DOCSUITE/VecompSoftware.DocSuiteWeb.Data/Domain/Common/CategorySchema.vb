Imports Newtonsoft.Json
Imports System.Text

<Serializable()>
Partial Public Class CategorySchema
    Inherits AuditableDomainObject(Of Guid)

    Public Sub New()
        MyBase.Id = Guid.NewGuid()
    End Sub

    Public Overridable Property Version As Short
    Public Overridable Property StartDate As DateTimeOffset
    Public Overridable Property EndDate As DateTimeOffset?
    Public Overridable Property Note As String
    Public Overridable Property Categories As IList(Of Category)


End Class

