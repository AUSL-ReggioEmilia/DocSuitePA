Imports System.Xml

Public Class SegnaturaContactsBranch

#Region " Fields "

    Private _branchBase As XmlElement
    Private _leaf As XmlElement

#End Region

#Region " Properties "

    Public ReadOnly Property BranchBase() As XmlElement
        Get
            Return _branchBase
        End Get
    End Property

    Public ReadOnly Property Leaf() As XmlElement
        Get
            Return _leaf
        End Get
    End Property

#End Region

#Region " Constructors "

    Public Sub New(ByVal branchBase As XmlElement, ByVal leaf As XmlElement)
        Me._branchBase = branchBase
        Me._leaf = leaf
    End Sub

#End Region

End Class