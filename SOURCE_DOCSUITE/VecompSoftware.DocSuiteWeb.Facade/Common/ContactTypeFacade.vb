Imports System.ComponentModel
Imports VecompSoftware.DocSuiteWeb.Data

<DataObject()>
Public Class ContactTypeFacade
    Inherits CommonFacade(Of ContactType, Char, NHibernateContactTypeDao)

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub

#End Region

    ''' <summary> Dato id torna descrizione </summary>
    ''' <remarks> TODO: eliminare a favore di enumeratore serio o query secca, spostata qui per retrocompatibilità </remarks>
    Public Shared Function LegacyDescription(id As Char) As String
        Select Case id
            Case ContactType.Administration
                Return "Amministrazione"
            Case ContactType.Group
                Return "Gruppo"
            Case ContactType.Aoo
                Return "Area Organizzativa Omogenea"
            Case ContactType.OrganizationUnit
                Return "Unità Organizzativa"
            Case ContactType.Role
                Return "Ruolo"
            Case ContactType.Person
                Return "Persona"
            Case ContactType.Sector
                Return "Rubrica per Settore"
            Case ContactType.Ipa
                Return "Pubblica Amministrazione"
        End Select
        Return Nothing
    End Function

End Class