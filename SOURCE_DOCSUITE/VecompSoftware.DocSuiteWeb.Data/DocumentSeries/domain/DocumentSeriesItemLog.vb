Imports VecompSoftware.Helpers.ExtensionMethods

<Serializable()>
Public Class DocumentSeriesItemLog
    Inherits DomainObject(Of Int32)

#Region " Properties "
    Public Overridable Property DocumentSeriesItem() As DocumentSeriesItem
    Public Overridable Property LogDate As DateTime
    Public Overridable Property SystemComputer As String
    Public Overridable Property SystemUser As String
    Public Overridable Property Program As String
    Public Overridable Property LogType As DocumentSeriesItemLogType
    Public Overridable Property Severity As Short?
    Public Overridable Property Hash As String
    Public Overridable ReadOnly Property LogTypeDescription As String
        Get
            Return LogType.GetDescription()
        End Get
    End Property

    Public Overridable Property UniqueIdDocumentSeriesItem As Guid

    Public Overridable Property LogDescription As String
#End Region

#Region " Constructor "

    Public Sub New()
        UniqueId = Guid.NewGuid()
    End Sub

#End Region

End Class