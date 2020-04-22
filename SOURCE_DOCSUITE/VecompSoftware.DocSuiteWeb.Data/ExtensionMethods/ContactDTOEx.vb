Imports System.Runtime.CompilerServices
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.DocSuiteWeb.Data

Public Module ContactDTOEx

    <Extension()>
    Public Function ToPOLSender(source As API.ContactDTO) As POLRequestSender
        Dim result As New POLRequestSender()
        result.Id = Guid.NewGuid()
        result.Name = source.Description
        result.PhoneNumber = source.PhoneNumber
        result.Address = source.Address
        result.City = source.City
        result.CivicNumber = source.CivicNumber
        result.ZipCode = source.ZipCode
        result.Province = source.CityCode

        Return result
    End Function

    <Extension()>
    Public Function ToPOLRecipient(source As API.ContactDTO) As POLRequestRecipient
        Dim result As New POLRequestRecipient()
        result.Id = Guid.NewGuid()
        result.Name = source.Description
        result.PhoneNumber = source.PhoneNumber
        result.Address = source.Address
        result.City = source.City
        result.CivicNumber = source.CivicNumber
        result.ZipCode = source.ZipCode
        result.Province = source.CityCode

        result.Status = POLMessageContactEnum.Created
        result.StatusDescrition = "In Attesa di Invio"

        Return result
    End Function

End Module