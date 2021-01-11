Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Biblos.Models

Public Class TestWatermark
    Inherits SuperAdminPage


    Protected Sub cmdWatermark_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdWatermark.Click

        Dim currentResolution As Resolution = Facade.ResolutionFacade.GetById(txtIdResolution.Text)
        Dim documents As FileResolution = Facade.FileResolutionFacade.GetByResolution(currentResolution)(0)

        Dim ext As String = String.Empty

        Dim lbl As String = ParseString(DocSuiteContext.Current.ResolutionEnv.WebPublishSign, currentResolution)
        Dim label As String = String.Format(DocSuiteContext.Current.ResolutionEnv.WebPublishSignTag, lbl)

        Dim doc As New BiblosDocumentInfo(currentResolution.Location.ReslBiblosDSDB, documents.IdResolutionFile)

        Dim data As Byte() = doc.GetPdfLocked(doc.Signature, label)

        Dim webpath As String = String.Format("/temp/WebPubTemp_{0}.pdf", Guid.NewGuid().ToString("N"))
        Dim tempFile As String = Server.MapPath(webpath) ' String.Format("{0}WebPubTemp_{1}.pdf", CommonUtil.GetInstance().AppTempPath, Guid.NewGuid().ToString("N"))
        Try
            Dim oFileStream As System.IO.FileStream
            oFileStream = New System.IO.FileStream(tempFile, System.IO.FileMode.Create)
            oFileStream.Write(data, 0, data.Length)
            oFileStream.Close()
        Catch ex As Exception
            Throw New Exception("Errore in fase di salvataggio documento: " & tempFile, ex)
        End Try

        Server.Transfer(webpath, False)
    End Sub


    Private Function ParseString(ByVal str As String, ByVal doc As Resolution) As String

        str = str.Replace("{year}", doc.Year)
        str = str.Replace("{number}", doc.Number.ToString().PadLeft(5, "0"c))
        str = str.Replace("{fullnumber}", Facade.ResolutionFacade.CalculateFullNumber(doc, doc.Type.Id, False))
        str = str.Replace("{type}", doc.Type.Description)

        If doc.AdoptionDate.HasValue Then
            str = str.Replace("{AdoptionDate}", doc.AdoptionDate.Value.ToString("dd/MM/yyyy"))
        Else
            str = str.Replace("{AdoptionDate}", String.Empty)
        End If
        If doc.PublishingDate.HasValue Then
            str = str.Replace("{PublishingDate}", doc.PublishingDate.Value.ToString("dd/MM/yyyy"))
        Else
            str = str.Replace("{PublishingDate}", String.Empty)
        End If
        If doc.EffectivenessDate.HasValue Then
            str = str.Replace("{EffectivenessDate}", doc.EffectivenessDate.Value.ToString("dd/MM/yyyy"))
        Else
            str = str.Replace("{EffectivenessDate}", String.Empty)
        End If
        str = str.Replace("{object}", doc.ResolutionObject)

        Return str

    End Function
End Class