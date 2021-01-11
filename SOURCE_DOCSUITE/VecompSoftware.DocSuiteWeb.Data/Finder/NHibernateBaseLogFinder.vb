Imports NHibernate
Imports NHibernate.Criterion
Imports VecompSoftware.NHibernateManager

''' <summary>
''' Classe che identifica un generico LogFinder implementando le proprietà comuni 
''' di TableLog, ProtocolLog,DocumentLog, ResolutionLog
''' </summary>
''' <typeparam name="T"></typeparam>
''' <remarks></remarks>
<Serializable()>
Public MustInherit Class NHibernateBaseLogFinder(Of T)
    Inherits NHibernateBaseFinder(Of T, T)

#Region " Fields"
    ' common fields
    Private _id As Integer
    Private _logDateStart As Nullable(Of Date)
    Private _logDateEnd As Nullable(Of Date)
    Private _systemComputer As String
    Private _systemUser As String
    Private _logType As String
    Private _logDescription As String

#End Region

#Region " Common Properties"
    ''' <summary>
    ''' IdLog
    ''' </summary>
    ''' <value>Integer</value>
    ''' <returns>Integer</returns>
    ''' <remarks></remarks>
    Public Property Id() As Integer
        Get
            Return Me._id
        End Get
        Set(ByVal value As Integer)
            Me._id = value
        End Set
    End Property
    ''' <summary>
    ''' Limite inferiore della data di inserimento dei log da estrarre 
    ''' </summary>
    ''' <value>System.Nullable(Of Date)</value>
    ''' <returns>System.Nullable(Of Date)</returns>
    ''' <remarks></remarks>
    Public Property LogDateStart() As System.Nullable(Of Date)
        Get
            Return Me._logDateStart
        End Get
        Set(ByVal value As System.Nullable(Of Date))
            Me._logDateStart = value
        End Set
    End Property

    ''' <summary>
    ''' Limite superiore della data di inserimento dei log da estrarre 
    ''' </summary>
    ''' <value>System.Nullable(Of Date)</value>
    ''' <returns>System.Nullable(Of Date)</returns>
    ''' <remarks></remarks>
    Public Property LogDateEnd() As System.Nullable(Of Date)
        Get
            Return Me._logDateEnd
        End Get
        Set(ByVal value As System.Nullable(Of Date))
            Me._logDateEnd = value
        End Set
    End Property

    ''' <summary>
    ''' Descrizione del log
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Property LogDescription() As String
        Get
            Return Me._logDescription
        End Get
        Set(ByVal value As String)
            Me._logDescription = value
        End Set
    End Property
    ''' <summary>
    ''' Tipo di log
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Property LogType() As String
        Get
            Return Me._logType
        End Get
        Set(ByVal value As String)
            Me._logType = value
        End Set
    End Property
    ''' <summary>
    ''' Nome computer
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Property SystemComputer() As String
        Get
            Return Me._systemComputer
        End Get
        Set(ByVal value As String)
            Me._systemComputer = value
        End Set
    End Property
    ''' <summary>
    ''' Restringe la ricerca ai log di uno specifico Nome utente
    ''' </summary>
    ''' <value>String</value>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Property SystemUser() As String
        Get
            Return _systemUser
        End Get
        Set(ByVal value As String)
            _systemUser = value
        End Set
    End Property

#End Region

#Region " NHibernate Properties "

    Protected ReadOnly Property NHibernateSession() As ISession
        Get
            Return NHibernateSessionManager.Instance.GetSessionFrom(SessionFactoryName)
        End Get
    End Property

#End Region

#Region " Virtual Functions "
    Public MustOverride Overrides Function DoSearch() As System.Collections.Generic.IList(Of T)
#End Region

#Region " IFinder Implementation "

    ''' <summary>
    ''' Creazione dei criteri di ricerca comuni per TableLog, ProtocolLog,DocumentLog, ResolutionLog
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function CreateCriteria() As ICriteria

        Dim criteria As ICriteria = NHibernateSession.CreateCriteria(persistentType, "D")

        If LogDateStart.HasValue Then
            criteria.Add(Restrictions.Ge("D.LogDate", LogDateStart))
        End If

        If LogDateEnd.HasValue Then
            criteria.Add(Restrictions.Le("D.LogDate", DateAdd(DateInterval.Day, 1, LogDateEnd.Value)))
        End If

        If SystemComputer <> String.Empty Then
            criteria.Add(Restrictions.Like("D.SystemComputer", SystemComputer))
        End If

        If SystemUser <> String.Empty Then
            criteria.Add(Restrictions.Like("D.SystemUser", SystemUser))
        End If

        If LogType <> String.Empty Then
            criteria.Add(Restrictions.Like("D.LogType", LogType))
        End If

        AttachFilterExpressions(criteria)

        Return criteria
    End Function

#End Region

#Region " Convert Functions "
    ''' <summary>
    ''' Converte una Stringa in Short
    ''' </summary>
    ''' <param name="value">Stringa da convertire</param>
    ''' <returns>Valore Short</returns>
    ''' <remarks></remarks>
    Protected Overloads Function ConvertToShort(ByVal value As String) As Nullable(Of Short)
        If Not String.IsNullOrEmpty(value) Then
            Return Convert.ToInt16(value)
        Else
            Return Nothing
        End If
    End Function
    ''' <summary>
    ''' Converte una stringa in Intero
    ''' </summary>
    ''' <param name="value">Stringa da convertire</param>
    ''' <returns>Valore Integer</returns>
    ''' <remarks></remarks>
    Protected Overloads Function ConvertToInteger(ByVal value As String) As Integer?
        If Not String.IsNullOrEmpty(value) Then
            Return Convert.ToInt32(value)
        Else
            Return Nothing
        End If
    End Function
    ''' <summary>
    ''' Converte una Stringa in una Data
    ''' </summary>
    ''' <param name="value">String  da convertire</param>
    ''' <returns>Valore DateTime</returns>
    ''' <remarks></remarks>
    Protected Overloads Function ConvertToDateTime(ByVal value As String) As Date?
        If Not String.IsNullOrEmpty(value) Then
            Return Convert.ToDateTime(value)
        Else
            Return Nothing
        End If
    End Function
#End Region

End Class
