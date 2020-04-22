Imports System.Linq

Public Class TopMediaParameters

#Region " Fields "

    Private _containerIdentifiers As IList(Of Integer)
    Private _userName As String
    Private _password As String
    Private _documentType As String
    Private _attachmentType As String
    Private _language As String
    Private _roleIdentifiers As IList(Of Integer)

#End Region

#Region " Properties "

    Public ReadOnly Property ContainerIdentifiers As IList(Of Integer)
        Get
            If _containerIdentifiers Is Nothing Then
                Dim splitted As String() = DocSuiteContext.Current.ProtocolEnv.WSTopMediaContainers.Split("|"c)

                Dim parsed As Integer = 0
                Dim valid As IEnumerable(Of String) = splitted.Where(Function(c) Integer.TryParse(c, parsed))
                _containerIdentifiers = valid.Select(Function(c) Integer.Parse(c)).ToList()
            End If
            Return _containerIdentifiers
        End Get
    End Property
    Public ReadOnly Property UserName As String
        Get
            If String.IsNullOrEmpty(_userName) Then
                Dim splitted As String() = DocSuiteContext.Current.ProtocolEnv.WSTopMediaParams.Split("|"c)
                If splitted.Length > 0 AndAlso Not String.IsNullOrEmpty(splitted(0)) Then
                    _userName = splitted(0)
                End If
            End If
            Return _userName
        End Get
    End Property
    Public ReadOnly Property Password As String
        Get
            If String.IsNullOrEmpty(_password) Then
                Dim splitted As String() = DocSuiteContext.Current.ProtocolEnv.WSTopMediaParams.Split("|"c)
                If splitted.Length > 1 AndAlso Not String.IsNullOrEmpty(splitted(1)) Then
                    _password = splitted(1)
                End If
            End If
            Return _password
        End Get
    End Property
    Public ReadOnly Property DocumentType As String
        Get
            If String.IsNullOrEmpty(_documentType) Then
                Dim splitted As String() = DocSuiteContext.Current.ProtocolEnv.WSTopMediaParams.Split("|"c)
                If splitted.Length > 2 AndAlso Not String.IsNullOrEmpty(splitted(2)) Then
                    _documentType = splitted(2)
                End If
            End If
            Return _documentType
        End Get
    End Property
    Public ReadOnly Property AttachmentType As String
        Get
            If String.IsNullOrEmpty(_attachmentType) Then
                Dim splitted As String() = DocSuiteContext.Current.ProtocolEnv.WSTopMediaParams.Split("|"c)
                If splitted.Length > 3 AndAlso Not String.IsNullOrEmpty(splitted(3)) Then
                    _attachmentType = splitted(3)
                End If
            End If
            Return _attachmentType
        End Get
    End Property
    Public ReadOnly Property Language As String
        Get
            If String.IsNullOrEmpty(_language) Then
                Dim splitted As String() = DocSuiteContext.Current.ProtocolEnv.WSTopMediaParams.Split("|"c)
                If splitted.Length > 4 AndAlso Not String.IsNullOrEmpty(splitted(4)) Then
                    _language = splitted(4)
                End If
            End If
            Return _language
        End Get
    End Property
    Public ReadOnly Property RoleIdentifiers As IList(Of Integer)
        Get
            If _roleIdentifiers Is Nothing Then
                Dim splitted As String() = DocSuiteContext.Current.ProtocolEnv.WSTopMediaRoles.Split("|"c)

                Dim parsed As Integer = 0
                Dim valid As IEnumerable(Of String) = splitted.Where(Function(c) Integer.TryParse(c, parsed))
                _roleIdentifiers = valid.Select(Function(c) Integer.Parse(c)).ToList()
            End If
            Return _roleIdentifiers
        End Get
    End Property

#End Region

End Class
