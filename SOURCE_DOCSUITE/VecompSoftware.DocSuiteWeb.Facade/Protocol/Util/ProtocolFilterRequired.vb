
Imports System.Xml
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Text
Imports VecompSoftware.Helpers.ExtensionMethods

''' <summary>
''' Classe per la verfica dei filtri mini applicati in fase di ricerca Protocollo
''' </summary>
''' <remarks></remarks>
Public Class ProtocolFilterRequired
    Public Shared Function CheckFilter(ByVal _finder As NHibernateProtocolFinder, ByVal _config As XmlDocument, ByRef strMessage As String, Optional ByVal VisibleDocTypeSearch As Boolean = False, Optional ByVal VisibleInteropSearch As Boolean = False, Optional ByVal VisiblePackageSearch As Boolean = False, Optional ByVal VisibleStatusSearch As Boolean = False) As Boolean
        Dim fieldsNotEmpty As New List(Of String)

        ' Verifico quali campi hanno un valore
        'Anno
        If _finder.Year.HasValue Then
            fieldsNotEmpty.Add("Year")
        End If
        'Numero
        If _finder.Number.HasValue Then
            fieldsNotEmpty.Add("Number")
        End If
        'Data Registrazione da
        If _finder.RegistrationDateFrom.HasValue Then
            fieldsNotEmpty.Add("RegistrationDateFrom")
        End If
        'Data Registrazione a
        If _finder.RegistrationDateTo.HasValue Then
            fieldsNotEmpty.Add("RegistrationDateTo")
        End If
        'Tipo
        If Not _finder.IdTypes.IsNullOrEmpty() Then
            fieldsNotEmpty.Add("IdTypes")
        End If
        'Locazione
        If Not String.IsNullOrEmpty(_finder.IdLocation) Then
            fieldsNotEmpty.Add("IdLocation")
        End If
        'Contenitore
        If Not String.IsNullOrEmpty(_finder.IdContainer) Then
            fieldsNotEmpty.Add("IdContainer")
        End If
        'Mittente Data Documento da
        If _finder.DocumentDateFrom.HasValue Then
            fieldsNotEmpty.Add("DocumentDateFrom")
        End If
        'Mittente Data Documento a
        If _finder.DocumentDateTo.HasValue Then
            fieldsNotEmpty.Add("DocumentDateTo")
        End If
        'Mittente Protocollo (?)
        If Not String.IsNullOrEmpty(_finder.DocumentProtocol) Then
            fieldsNotEmpty.Add("DocumentProtocol")
        End If
        'Mittente nome documento
        If Not String.IsNullOrEmpty(_finder.DocumentName) Then
            fieldsNotEmpty.Add("DocumentName")
        End If
        'Oggetto
        If Not String.IsNullOrEmpty(_finder.ProtocolObject) Then
            fieldsNotEmpty.Add("ProtocolObject")
        End If
        'Note
        If Not String.IsNullOrEmpty(_finder.Note) Then
            fieldsNotEmpty.Add("Note")
        End If
        'Mittente/Destinatario
        If Not String.IsNullOrEmpty(_finder.Recipient) Then
            fieldsNotEmpty.Add("Recipient")
        End If
        'Soggetto
        If Not String.IsNullOrEmpty(_finder.Subject) Then
            fieldsNotEmpty.Add("Subject")
        End If
        'Categoria
        If Not String.IsNullOrEmpty(_finder.ServiceCategory) Then
            fieldsNotEmpty.Add("ServiceCategory")
        End If
        'Classificazione
        If Not String.IsNullOrEmpty(_finder.Classifications) Then
            fieldsNotEmpty.Add("Classifications")
        End If

        ' Ricerca opzionale
        If VisibleDocTypeSearch AndAlso Not String.IsNullOrEmpty(_finder.IdDocType) Then
            fieldsNotEmpty.Add("IdDocType")
        End If

        ''INVOICE
        If VisibleInteropSearch Then
            If Not String.IsNullOrEmpty(_finder.InvoiceNumber) Then
                fieldsNotEmpty.Add("InvoiceNumber")
            End If
            If _finder.InvoiceDateFrom.HasValue Then
                fieldsNotEmpty.Add("InvoiceDateFrom")
            End If
            If _finder.InvoiceDateTo.HasValue Then
                fieldsNotEmpty.Add("InvoiceDateTo")
            End If
            If Not String.IsNullOrEmpty(_finder.AccountingSectional) Then
                fieldsNotEmpty.Add("AccountingSectional")
            End If
            If _finder.AccountingYear.HasValue Then
                fieldsNotEmpty.Add("AccountingYear")
            End If
            If _finder.AccountingNumber.HasValue Then
                fieldsNotEmpty.Add("AccountingNumber")
            End If
        End If


        ''INTEROP
        If VisibleInteropSearch Then
            If Not String.IsNullOrEmpty(_finder.Contacts) Then
                fieldsNotEmpty.Add("Contacts")
            End If
        End If

        ''PACKAGE
        If VisiblePackageSearch Then
            If Not String.IsNullOrEmpty(_finder.PackageOrigin) Then
                fieldsNotEmpty.Add("PackageOrigin")
            End If
            If Not String.IsNullOrEmpty(_finder.Package) Then
                fieldsNotEmpty.Add("Package")
            End If
            If Not String.IsNullOrEmpty(_finder.PackageLot) Then
                fieldsNotEmpty.Add("PackageLot")
            End If
            If Not String.IsNullOrEmpty(_finder.PackageIncremental) Then
                fieldsNotEmpty.Add("PackageIncremental")
            End If
        End If

        ''STATUS
        If VisibleStatusSearch AndAlso Not String.IsNullOrEmpty(_finder.AdvancedStatus) Then
            fieldsNotEmpty.Add("AdvancedStatus")
        End If

        ' Verifico che sia valorizzato il campo di verifica
        Dim retval As Boolean = True
        Dim errorMessage As New StringBuilder()

        Dim nodes As XmlNodeList = _config.SelectNodes("//filedtocheck/filter")
        Dim nodes1 As XmlNodeList = _config.SelectNodes("//filtertoadd/filter")
        For Each n As XmlNode In nodes
            If fieldsNotEmpty.Contains(n.Attributes("field").Value) Then
                For Each n1 As XmlNode In nodes1
                    errorMessage.AppendFormat("{0} ", n1.Attributes("description").Value)
                    retval = fieldsNotEmpty.Contains(n1.Attributes("field").Value)
                Next
            End If
        Next
        strMessage = errorMessage.ToString()

        ' Verifico che non sia già stato impostato un'altro campo in caso di esito negativo
        If Not retval AndAlso (nodes1.Count + nodes.Count) <= fieldsNotEmpty.Count Then
            retval = True
        End If

        Return retval

    End Function
End Class
