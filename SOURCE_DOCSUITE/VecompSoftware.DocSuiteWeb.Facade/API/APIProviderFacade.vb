Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

Public Class APIProviderFacade
    Inherits FacadeNHibernateBase(Of APIProvider, Guid, NHibernateAPIProviderDao)

#Region " Methods "

    Public Function GetMain() As APIProvider
        Dim mainProvider As APIProvider = GetAll().FirstOrDefault(Function(p) p.IsEnabled AndAlso p.IsMain)
        If mainProvider IsNot Nothing Then
            Return mainProvider
        End If

        Dim defaultProvider As New APIProvider() With {
            .Code = DocSuiteContext.Current.ProtocolEnv.CorporateAcronym,
            .Title = DocSuiteContext.Current.ProtocolEnv.CorporateName,
            .Description = DocSuiteContext.Current.ProtocolEnv.CorporateName,
            .Address = DocSuiteContext.Current.ProtocolEnv.APIDefaultProvider,
            .Main = True
        }
        Save(defaultProvider)
        Return defaultProvider
    End Function
    Public Function GetByCode(code As String) As APIProvider
        Return _dao.GetByCode(code)
    End Function

    Public Function GetDefault() As APIProvider
        If String.IsNullOrWhiteSpace(DocSuiteContext.Current.ProtocolEnv.APIDefaultProvider) Then
            Throw New DocSuiteException("Parametro APIDefaultProvider non configurato.")
        End If

        Dim defaultProvider As APIProvider = Me.GetAll().FirstOrDefault(Function(p) p.Address.Equals(DocSuiteContext.Current.ProtocolEnv.APIDefaultProvider))
        If defaultProvider Is Nothing Then
            Throw New DocSuiteException("Parametro APIDefaultProvider non valido.")
        End If

        Return defaultProvider
    End Function

    Public Sub Renew(providers As IEnumerable(Of APIProvider))
        _dao.Renew(providers)
    End Sub
    Public Sub Renew()
        Dim mainProvider As APIProvider = GetMain()
        If DocSuiteContext.Current.ProtocolEnv.APIDefaultProvider.Eq(mainProvider.Address) Then
            Throw New InvalidOperationException("Il provider principale coincide col provider locale.")
        End If

        Dim available As IAPIProviderDTO() = APIConnector.For(mainProvider.Address).GetAvailable()
        Dim domains As IEnumerable(Of APIProvider) = available.Select(Function(p) New APIProvider(p))
        Renew(domains)
    End Sub

#End Region

End Class
