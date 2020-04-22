Imports Newtonsoft.Json

<Serializable()> _
Public Class ContactType
    Inherits DomainObject(Of Char)

#Region " Constants "

    ''' <summary>Amministrazione</summary>
    Public Const Administration As Char = "M"c

    ''' <summary>Area Organizzativa Omogenea o Azienda</summary>
    Public Const Aoo As Char = "A"c

    ''' <summary>Unità Organizzativa (AO)</summary>
    Public Const OrganizationUnit As Char = "U"c

    ''' <summary>Ruolo</summary>
    Public Const Role As Char = "R"c

    ''' <summary>Persona</summary>
    Public Const Person As Char = "P"c

    ''' <summary>Gruppo</summary>
    Public Const Group As Char = "G"c

    ''' <summary>Pubblica amministrazione da IPA</summary>
    Public Const Ipa As Char = "I"c

    ''' <summary>Settore</summary>
    Public Const Sector As Char = "S"c

    ''' <summary>Persona AdAm</summary>
    Public Const AdAmPerson As Char = "D"c

    ''' <summary> Valore Dummy per contatti che non sono contatti. </summary>
    ''' <remarks> Usato in collaborazione per visualizzare le segreterie <see cref="Role"/> </remarks>
    Public Const Mistery As Char = "X"c

#End Region

#Region " Fields "
    Private _description As String
#End Region

#Region " Properties "

    ''' <summary>
    ''' Carattere rappresentativo del tipo di contatto
    ''' </summary>
    ''' <value>
    ''' <list>
    ''' <item><term>M</term><description>Amministrazione</description></item>
    ''' <item><term>A</term><description>Area Organizzativa Omogenea (AOO)</description></item>
    ''' <item><term>U</term><description>Unità Organizzativa (AO)</description></item>
    ''' <item><term>R</term><description>Ruolo</description></item>
    ''' <item><term>P</term><description>Persona</description></item>
    ''' <item><term>G</term><description>Gruppo</description></item>
    ''' <item><term>I</term><description>Pubblica amministrazione da IPA</description></item>
    ''' <item><term>S</term><description>Settore</description></item>
    ''' <item><term>D</term><description>Persona AdAm</description></item>
    ''' ''' <item><term>H</term><description>Storicizzazione</description></item>
    ''' </list>
    ''' </value>
    <JsonProperty("ContactTypeId")> _
    Public Overrides Property Id() As Char
        Get
            Return MyBase.Id
        End Get
        Set(ByVal value As Char)
            MyBase.Id = value
        End Set
    End Property

    <JsonProperty("ContactTypeDescription")> _
    Public Overridable Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim objContactType As ContactType = TryCast(obj, ContactType)
        Return objContactType IsNot Nothing AndAlso objContactType.Id() = Id()
    End Function
#End Region

#Region " Constructors "

    Public Sub New()
        _description = ""
    End Sub

    Public Sub New(ByVal type As Char)
        Id = type
    End Sub

#End Region

End Class

