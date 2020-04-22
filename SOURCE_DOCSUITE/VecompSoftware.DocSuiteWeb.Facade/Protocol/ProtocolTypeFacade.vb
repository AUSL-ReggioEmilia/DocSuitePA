Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()> _
Public Class ProtocolTypeFacade
    Inherits BaseProtocolFacade(Of ProtocolType, Integer, NHibernateProtocolTypeDao)

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    ''' <summary>
    ''' Restituisce tutti le tipologie di un protocollo eliminando la tipologia "ingresso e uscita"
    ''' </summary>
    ''' <returns>Lista di ProtocolType escluso "ingresso e uscita"</returns>
    Public Function GetAllSearch() As IList(Of ProtocolType)
        Dim types As IList(Of ProtocolType) = GetAll().Where(Function(f) f.Id <> 0).ToList()

        If DocSuiteContext.Current.ProtocolEnv.IsInterOfficeEnabled Then
            types.Add(New ProtocolType(0, "Tra Uffici"))
        End If

        Return types.ToList()
    End Function

    Public Function GetTypes(Optional includeInOut As Boolean = False, Optional includeEmpty As Boolean = False) As IList(Of ProtocolType)
        Dim types As IList(Of ProtocolType) = New List(Of ProtocolType)
        If includeEmpty Then
            types.Add(New ProtocolType(Integer.MaxValue, String.Empty))
        End If
        If includeInOut Then
            types.Add(New ProtocolType(Integer.MaxValue, "ingresso e uscita"))
        End If
        types.Add(New ProtocolType(-1, "Ingresso"))
        If DocSuiteContext.Current.ProtocolEnv.IsInterOfficeEnabled Then
            types.Add(New ProtocolType(0, "Tra Uffici"))
        End If
        types.Add(New ProtocolType(1, "Uscita"))
        Return types
    End Function

    Public Shared Function CalcolaTipoProtocollo(ByVal type As Integer) As String
        Select Case type
            Case -1
                Return "Ingresso"
            Case 1
                Return "Uscita"
            Case 0
                Return "Tra Uffici"
            Case Else
                Return "Ingresso e Uscita"
        End Select
    End Function

    Public Shared Function CalcolaTipoProtocolloChar(ByVal type As Integer) As String
        Select Case type
            Case -1
                Return "I"
            Case 1
                Return "U"
            Case Else
                Return "T"
        End Select
    End Function

#End Region
End Class