﻿Imports VecompSoftware.Helpers.NHibernate

Public Class TemplateProtocolRoleCompositeKey
    Inherits Duplet(Of Integer, Integer)

    Public Overridable Property IdTemplateProtocol As Integer
        Get
            Return First
        End Get
        Set(value As Integer)
            First = value
        End Set
    End Property

    Public Overridable Property IdRole As Integer
        Get
            Return Second
        End Get
        Set(value As Integer)
            Second = value
        End Set
    End Property
End Class
