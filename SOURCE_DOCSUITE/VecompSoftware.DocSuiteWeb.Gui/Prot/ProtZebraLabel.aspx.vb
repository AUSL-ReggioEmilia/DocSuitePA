Imports System.Collections.Generic
Imports System.Linq
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Text
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.Biblos.Models

Partial Public Class ProtZebraLabel
    Inherits ProtBasePage

    Public Enum ChainType
        Document
        Attachment
    End Enum

#Region " Fields "

    Private Const LabelSeparator As String = "%ENDOFPRINTSECTION%"

    Private _currentZebraTemplate As String

    Private _selectedProtocols As IList(Of Protocol)

#End Region

#Region " Properties "

    Private ReadOnly Property CurrentChainType As ChainType
        Get
            Dim chainType As ChainType
            If Not [Enum].TryParse(Request.QueryString("ChainType"), True, chainType) Then
                Throw New DocSuiteException("Stampa etichette Zebra", "Catena documentale non prevista.")
            End If
            Return chainType
        End Get
    End Property

    Private ReadOnly Property CurrentZebraTemplate As String
        Get
            If _currentZebraTemplate Is Nothing Then
                Dim templateName As String
                Select Case CurrentChainType
                    Case ChainType.Document
                        templateName = Facade.ComputerLogFacade.GetCurrent.ZebraPrinter.DocumentTemplate
                    Case ChainType.Attachment
                        templateName = Facade.ComputerLogFacade.GetCurrent.ZebraPrinter.AttachmentTemplate
                End Select
                If String.IsNullOrEmpty(templateName) Then
                    Throw New DocSuiteException("Stampa etichette Zebra", "Template non previsto, controllare le configurazioni.")
                End If

                Dim templatePath As String = Server.MapPath("~/Prot/Stampe/Zebra/" & templateName)
                If Not File.Exists(templatePath) Then
                    Throw New DocSuiteException("Stampa etichette Zebra", String.Format("Percorso template [{0}] non valido.", templatePath))
                End If

                Using sr As New StreamReader(templatePath)
                    _currentZebraTemplate = sr.ReadToEnd()
                End Using
                _currentZebraTemplate = _currentZebraTemplate.Replace(vbCrLf, "\r\n")
                _currentZebraTemplate = _currentZebraTemplate.Replace("""", "\""")
            End If

            Return _currentZebraTemplate
        End Get
    End Property

    Private ReadOnly Property SelectedProtocols() As IList(Of Protocol)
        Get
            If _selectedProtocols Is Nothing Then
                ' Estraggo e pulisco i dati in sessione
                Dim protocolKeys As ICollection(Of Guid) = CommonShared.ZebraPrintData
                If protocolKeys IsNot Nothing Then
                    _selectedProtocols = Facade.ProtocolFacade.GetProtocols(protocolKeys).ToList()
                End If
                If _selectedProtocols.IsNullOrEmpty() Then
                    Throw New DocSuiteException("Stampa etichette Zebra", "Impossibile trovare protocolli per la stampa.")
                End If
            End If
            Return _selectedProtocols
        End Get
    End Property

#End Region

#Region " Methods "

    Protected Function GetPrinterName() As String
        If Facade.ComputerLogFacade.GetCurrent.ZebraPrinter IsNot Nothing AndAlso Not String.IsNullOrEmpty(Facade.ComputerLogFacade.GetCurrent.ZebraPrinter.Name) Then
            Return Facade.ComputerLogFacade.GetCurrent.ZebraPrinter.Name
        End If
        Return String.Empty
    End Function

    Protected Function GetYear() As String
        If SelectedProtocols.Count = 1 Then
            Return SelectedProtocols(0).Year.ToString()
        End If
        Return String.Empty
    End Function

    Protected Function GetNumber() As String
        If SelectedProtocols.Count = 1 Then
            Return SelectedProtocols(0).Number.ToString().PadLeft(8, "0"c)
        End If
        Return String.Empty
    End Function

    Protected Function GetDate() As String
        If SelectedProtocols.Count = 1 Then
            Return SelectedProtocols(0).RegistrationDate.ToLocalTime().ToString()
        End If
        Return String.Empty
    End Function

    Protected Function GetLabel() As String
        ' Creazione etichettte
        Dim label As New StringBuilder(CurrentZebraTemplate.Length * SelectedProtocols.Count)
        For Each protocol As Protocol In SelectedProtocols
            Select Case CurrentChainType
                Case ChainType.Document
                    PopulateZebraTemplate(label, CurrentZebraTemplate, protocol, ProtocolFacade.GetDocument(protocol), 1)
                Case ChainType.Attachment
                    Dim attachments As BiblosDocumentInfo() = ProtocolFacade.GetAttachments(protocol)
                    Dim documentNumber As Integer = 1
                    For Each attachment As BiblosDocumentInfo In attachments
                        PopulateZebraTemplate(label, CurrentZebraTemplate, protocol, attachment, documentNumber)
                        documentNumber += 1
                    Next
            End Select
        Next

        Return label.ToString()
    End Function

    ''' <summary> Popola il template corrente con i parametri specificati. </summary>
    ''' <param name="protocol">Protocollo di riferimento.</param>
    Private Shared Sub PopulateZebraTemplate(ByRef result As StringBuilder, ByRef template As String, protocol As Protocol, documentInfo As BiblosDocumentInfo, documentNumber As Integer?)
        FileLogger.Info(LoggerName, String.Format("Creazione etichetta Zebra protocollo [{0}].", protocol.ToString()))

        Dim label As String = template
        label = label.Replace("%YEAR%", protocol.Year.ToString())
        label = label.Replace("%NUMBER8%", String.Format("{0:00000000}", protocol.Number))
        label = label.Replace("%NUMBER7%", String.Format("{0:0000000}", protocol.Number))
        label = label.Replace("%DATE%", protocol.RegistrationDate.ToLocalTime().ToString())
        label = label.Replace("%CONTAINERNAME%", protocol.Container.Name)
        label = label.Replace("%DIRECTION%", protocol.Type.ShortDescription)
        label = label.Replace("%CATEGORY.CODE%", protocol.Category.FullCodeDotted)

        Dim dateOnly As String = String.Format("{0:dd/MM/yyyy}", protocol.RegistrationDate.ToLocalTime())
        label = label.Replace("%DATEONLY%", dateOnly)
        label = label.Replace("%COPIES%", "1")

        If label.Contains("%ATTACHMENTSCOUNT%") Then
            label = label.Replace("%ATTACHMENTSCOUNT%", GetAttachmentsCount(protocol).ToString())
        End If

        If documentNumber.HasValue Then
            label = label.Replace("%DOCUMENTNUMBER%", documentNumber.Value.ToString())
        End If

        If documentInfo IsNot Nothing Then
            label = label.Replace("%DOCUMENTINFONAME%", documentInfo.Name)

            If label.Contains("%DOCUMENTINFOPAGECOUNT%") Then
                label = label.Replace("%DOCUMENTINFOPAGECOUNT%", documentInfo.NumberOfPages.ToString())
            End If
        End If

        Dim protocolNote As String = String.Empty
        For Each found As Match In Regex.Matches(label, "%NOTE[(][0-9]{1,2}[)]%")
            ' Verifico se esiste un parametro di sottostringa valido.
            Dim lengthParameter As String = Regex.Match(found.ToString(), "[0-9]{1,2}").ToString()
            Dim finalLength As Integer = 0
            Integer.TryParse(lengthParameter, finalLength)

            If Not String.IsNullOrEmpty(protocol.Note) Then
                protocolNote = protocol.Note
            End If
            If finalLength > 0 AndAlso protocolNote.Length > finalLength Then
                protocolNote = protocolNote.Substring(0, finalLength)
            End If
            label = label.Replace(found.ToString(), protocolNote)
        Next


        Dim protocolCategory As String = String.Empty
        Dim lengthCategoryParameter As String = String.Empty
        Dim finalCategoryLength As Integer = 0

        For Each categoryFound As Match In Regex.Matches(label, "%CATEGORY.TXT[(][0-9]{1,2}[)]%")
            ' Verifico se esiste un parametro di sottostringa valido.
            lengthCategoryParameter = Regex.Match(categoryFound.ToString(), "[0-9]{1,2}").ToString()
            Integer.TryParse(lengthCategoryParameter, finalCategoryLength)

            If Not String.IsNullOrEmpty(protocol.Category.Name) Then
                protocolCategory = protocol.Category.Name
            End If
            If finalCategoryLength > 0 AndAlso protocolCategory.Length > finalCategoryLength Then
                protocolCategory = protocolCategory.Substring(0, finalCategoryLength)
            End If
            label = label.Replace(categoryFound.ToString(), protocolCategory)
        Next


        FileLogger.Debug(LoggerName, String.Format("Creata etichetta zebra [{0}].", label))

        If result.Length <> 0 Then
            result.Append(LabelSeparator)
        End If
        result.Append(label)
    End Sub

    Private Shared Function GetAttachmentsCount(protocol As Protocol) As Integer
        Dim identifier As Integer = protocol.IdAttachments.GetValueOrDefault(0)
        If identifier = 0 Then
            Return 0
        End If

        Dim attachments As New BiblosChainInfo(protocol.Location.ProtBiblosDSDB, protocol.IdAttachments.Value)
        If attachments IsNot Nothing AndAlso attachments.Documents IsNot Nothing Then
            Return attachments.Documents.Count
        End If
        Return 0
    End Function

#End Region

End Class