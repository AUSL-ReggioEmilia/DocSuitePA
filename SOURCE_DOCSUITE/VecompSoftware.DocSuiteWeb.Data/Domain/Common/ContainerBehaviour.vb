<Serializable()> _
Public Class ContainerBehaviour
    Inherits DomainObject(Of Int32)

    Public Overridable Property Container As Container
    Public Overridable Property AttributeName As String
    Public Overridable Property AttributeValue As String
    Public Overridable Property KeepValue As Boolean
    Public Overridable Property Action As ContainerBehaviourAction

End Class