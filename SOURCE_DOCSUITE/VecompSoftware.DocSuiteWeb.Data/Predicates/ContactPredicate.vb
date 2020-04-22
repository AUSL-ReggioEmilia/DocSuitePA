Public Class ContactPredicate

#Region " Fields "

    Private ReadOnly _predContact As Contact
    Private ReadOnly _predContactDto As ContactDTO

#End Region

#Region " Constructors "

    Public Sub New(ByVal predicateContact As Contact)
        _predContact = predicateContact
    End Sub

    Public Sub New(ByVal predicateContactDto As ContactDTO)
        _predContact = predicateContactDto.Contact
        _predContactDto = predicateContactDto
    End Sub

#End Region

#Region " Methods "

    Public Function CompareParentId(ByVal contact As Contact) As Boolean
        Return _predContact.Parent.Id = contact.Id
    End Function

    Public Function CompareId(ByVal contact As Contact) As Boolean
        Return _predContact.Id = contact.Id
    End Function

    Public Function CompareContactDtoId(ByVal contactDto As ContactDTO) As Boolean
        If contactDto.Type = contactDto.ContactType.Manual Then
            If _predContactDto.IdManualContact Is Nothing Then
                Return (contactDto.IdManualContact Is Nothing)
            End If
            Return _predContactDto.IdManualContact.Equals(contactDto.IdManualContact)
        End If

        Return CompareId(contactDto.Contact)
    End Function

#End Region

End Class
