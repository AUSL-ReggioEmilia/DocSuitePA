Imports System
Imports System.DirectoryServices
Imports System.Runtime.InteropServices
Imports System.Linq
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data

Public Class IPARetriever

    Public Shared Function GetIpaEntities(ByVal ldapConnection As String, ByVal descriptionToFind As String, Optional ByVal objectClass As List(Of String) = Nothing) As List(Of IPA)
        FileLogger.Info(LogName.DirectoryServiceLog, String.Format("GetIpaEntities [{0}] [{1}]", ldapConnection, descriptionToFind))
        Dim listIpaEntities As New List(Of IPA)
        'Dim directoryEntry As New DirectoryEntry(ldapConnection, "", "", AuthenticationTypes.Anonymous)
        ' Piccoli : dal Maggio 2012 l'autenticazione ai servizi ldap di DigitPA avviene viene utente e password 
        ' DN:uid=assistenza@vecompsoftware.it,o=utentildap  password: ZVACPNSDGQ()
        Dim objectClassfilter As String = "(|(objectClass=amministrazione)(objectClass=organizationalUnit)(objectClass=aoo))"
        If Not objectClass Is Nothing Then
            Dim objectClassfilterTmp As String = ""
            For Each objectClasstype As String In objectClass
                objectClassfilterTmp = String.Format("{0}(objectClass={1})", objectClassfilterTmp, objectClasstype)
            Next
            objectClassfilter = objectClassfilterTmp
            If objectClass.Count > 1 Then
                objectClassfilter = String.Format("(|{0})", objectClassfilterTmp)
            End If
        End If
        Dim directorySearcherFilter As String = String.Format("(&(description=*{0}*){1})", descriptionToFind, objectClassfilter)
        Try
            Using directoryEntry As New DirectoryEntry(ldapConnection, "uid=assistenza@vecompsoftware.it,o=utentildap", "ZVACPNSDGQ", AuthenticationTypes.None)
                Using directorySearcher As New DirectorySearcher(directoryEntry)
                    If Not String.IsNullOrEmpty(descriptionToFind) Then
                        directorySearcher.Filter = directorySearcherFilter
                    End If
                    Using searchResults As SearchResultCollection = directorySearcher.FindAll()
                        If searchResults IsNot Nothing Then
                            For Each sr As SearchResult In searchResults
                                listIpaEntities.Add(SetIPAProperty(sr))
                            Next
                        End If
                    End Using
                End Using
            End Using
        Catch exception As COMException
            FileLogger.Warn(LogName.DirectoryServiceLog, "Il server non risponde o non è disponibile", exception)
        End Try

        Return listIpaEntities.OrderBy(Function(ipa) ipa.Description).ToList()
    End Function

    Private Shared Function SetIPAProperty(sr As SearchResult) As IPA
        Dim ipaEnt As New IPA
        ipaEnt.Aoo = GetIPAProperty(sr, "Aoo")
        ipaEnt.Description = GetIPAProperty(sr, "Description")
        ipaEnt.Mail = GetIPAProperty(sr, "Mail")
        ipaEnt.PostalCode = GetIPAProperty(sr, "PostalCode")
        ipaEnt.Provincia = GetIPAProperty(sr, "Provincia")
        ipaEnt.Regione = GetIPAProperty(sr, "Regione")
        ipaEnt.TelephoneNumber = GetIPAProperty(sr, "TelephoneNumber")
        ipaEnt.LogoAmministrazione = GetIPAProperty(sr, "logoamm")
        ipaEnt.SitoIstituzionale = GetIPAProperty(sr, "sitoistituzionale")
        ipaEnt.CognomeResponsabile = GetIPAProperty(sr, "cognomeresp")
        ipaEnt.CodiceAmministrazione = GetIPAProperty(sr, "codiceamm")
        ipaEnt.StatoAmministrazione = GetIPAProperty(sr, "statoamm")
        ipaEnt.O = GetIPAProperty(sr, "o")
        ipaEnt.TitoloResponsabile = GetIPAProperty(sr, "titoloresp")
        ipaEnt.L = GetIPAProperty(sr, "l")
        ipaEnt.Indirizzo = GetIPAProperty(sr, "street")
        ipaEnt.NomeResponsabile = GetIPAProperty(sr, "nomeresp")
        ipaEnt.TipoAmministazione = GetIPAProperty(sr, "tipoamm")
        ipaEnt.St = GetIPAProperty(sr, "st")
        ipaEnt.ObjectClass = GetIPAProperty(sr, "objectclass")
        ipaEnt.ADSPath = GetIPAProperty(sr, "adspath")
        ipaEnt.AooRef = GetIPAProperty(sr, "aooRef")
        Return ipaEnt
    End Function

    Public Shared Function GetIpaChildEntities(ByVal ldappath As String) As List(Of IPA)
        Dim listIpaEntities As New List(Of IPA)
        Using directoryEntry As New DirectoryEntry(ldappath, "uid=assistenza@vecompsoftware.it,o=utentildap", "ZVACPNSDGQ", AuthenticationTypes.None)
            Using directorySearcher As New DirectorySearcher(directoryEntry)
                directorySearcher.SearchScope = SearchScope.OneLevel
                Using searchResults As SearchResultCollection = directorySearcher.FindAll()
                    If searchResults IsNot Nothing Then
                        For Each sr As SearchResult In searchResults
                            listIpaEntities.Add(SetIPAProperty(sr))
                        Next
                    End If
                End Using
            End Using
        End Using
        Return listIpaEntities
    End Function

    Public Shared Function GetIpaEntitieByPath(ByVal ldapConnection As String, ByVal path As String) As IPA
        FileLogger.Info(LogName.DirectoryServiceLog, String.Format("GetIpaEntities [{0}] [{1}]", ldapConnection, path))
        Dim ipaEnt As New IPA
        Try
            Using directoryEntry As New DirectoryEntry(ldapConnection, "uid=assistenza@vecompsoftware.it,o=utentildap", "ZVACPNSDGQ", AuthenticationTypes.None)
                directoryEntry.Path = path
                Using directorySearcher As New DirectorySearcher(directoryEntry)
                    Dim searchResults As SearchResult = directorySearcher.FindOne()
                    If searchResults IsNot Nothing Then
                        ipaEnt = SetIPAProperty(searchResults)
                    End If
                End Using
            End Using
        Catch exception As COMException
            FileLogger.Warn(LogName.DirectoryServiceLog, "Il server non risponde o non è disponibile", exception)
        End Try
        Return ipaEnt
    End Function


    Private Shared Function GetIPAProperty(ByVal sr As SearchResult, ByVal propertyName As String) As String
        Try
            If sr IsNot Nothing AndAlso sr.Properties IsNot Nothing AndAlso sr.Properties.Contains(propertyName) AndAlso sr.Properties(propertyName).Count > 0 Then
                Return CType(sr.Properties(propertyName).Item(0), String)
            End If
        Catch ex As Exception
            FileLogger.Warn(LogName.DirectoryServiceLog, "Errore nel ritiro di proprietà IPA", ex)
        End Try

        Return String.Empty
    End Function

End Class