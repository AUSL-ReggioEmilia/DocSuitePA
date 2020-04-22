Imports System.Diagnostics
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Services.Sharepoint

Public Class SharePointFacade
    Public Shared Sub Publish(resl As Resolution, pubblicationStartDate As DateTime, pubblicationEndDate As DateTime, signature As [String], xmlDoc As [String], xmlOther As String)
        Publish(resl, pubblicationStartDate, pubblicationEndDate, xmlDoc, xmlOther, 0, _
            0, False)
    End Sub

    Public Shared Sub Publish(resl As Resolution, pubblicationStartDate As DateTime, pubblicationEndDate As DateTime, xmlDoc As [String], xmlOther As String, chainId As Integer, _
        chainEnum As Integer, isPrivacy As Boolean)
        Dim obj As String = If(resl.ResolutionObjectPrivacy IsNot Nothing, resl.ResolutionObjectPrivacy.Replace("&", "&amp;"), resl.ResolutionObject.Replace("&", "&amp;"))
        Dim resolutionType As String = resl.Type.Description

        ' Se non carica il valore, ricarico l'oggetto
        If [String].IsNullOrEmpty(resolutionType) Then
            resolutionType = FacadeFactory.Instance.ResolutionTypeFacade.GetById(resl.Type.Id).Description
        End If
        Dim retVal As New ReturnValue()
        Try
            Debug.Assert(resl.AdoptionDate IsNot Nothing, "resl.AdoptionDate != null")
            retVal = ServiceSHP.InsertInPublicationArea(resl.Container.Name, resl.Year, resl.Number.ToString(), resl.AdoptionDate.Value, obj, DateTime.Now, _
                pubblicationStartDate, pubblicationEndDate, resolutionType, xmlOther, xmlDoc)
        Catch ex As Exception
            Throw New DocSuiteException("Errore pubblicazione", "Errore Pubblicazione internet: " & retVal.Guid, ex)
        End Try

        FacadeFactory.Instance.ResolutionLogFacade.Log(resl, ResolutionLogType.WP, "File pubblicato correttamente")

        Try
            resl.WebSPGuid = retVal.Guid
            resl.WebState = Resolution.WebStateEnum.Published
            resl.WebPublicationDate = pubblicationStartDate
            FacadeFactory.Instance.ResolutionFacade.Update(resl)

            ' Registro il numero di publicazione ritornato
            ' Da verificare non appena il servizio sarà disponibile
            Dim numeroPubblicazione As String = String.Empty
            Dim i As Integer = 0
            While numeroPubblicazione = String.Empty AndAlso i < retVal.ExtraMetaDatas.Length
                Dim data As ExtraMetaData = retVal.ExtraMetaDatas(i)
                If data.Name = "NumeroPubblicazione" Then
                    numeroPubblicazione = data.Value
                End If
                i += 1
            End While
            If numeroPubblicazione <> String.Empty Then
                FacadeFactory.Instance.ResolutionFacade.DoGenericWebPublication(resl, numeroPubblicazione, chainId, chainEnum, isPrivacy)
            Else
                Throw New DocSuiteException("Pubblicazione") With {
                    .Descrizione = [String].Format("Errore nella memorizzazione del numero di pubblicazione reslid = {0} Contattare l'amministratore del sistema.", resl.Id),
                    .User = DocSuiteContext.Current.User.FullUserName
                }

            End If
        Catch ex As Exception
            Throw New DocSuiteException("Pubblicazione", ex) With {
                .Descrizione = String.Format("Errore Pubblicazione internet: Impossibile aggiornare i dati sul database. IdResolution = {0}", resl.Id),
                .User = DocSuiteContext.Current.User.FullUserName
            }
        End Try
    End Sub

    Public Shared Sub Retire(resl As Resolution, revokeDate As DateTime, signature As [String], xmlDoc As [String], xmlOther As String)
        Dim obj As String = If(resl.ResolutionObjectPrivacy IsNot Nothing, resl.ResolutionObjectPrivacy.Replace("&", "&amp;"), resl.ResolutionObject.Replace("&", "&amp;"))
        Dim retVal As ReturnValue = New ReturnValue()
        Try
            Debug.Assert(resl.AdoptionDate IsNot Nothing, "resl.AdoptionDate != null")
            Debug.Assert(resl.WebPublicationDate IsNot Nothing, "resl.WebPublicationDate != null")
            retVal = ServiceSHP.InsertInRetireArea(resl.Container.Name, resl.Year, resl.Number.ToString(), resl.AdoptionDate.Value, obj, DateTime.Now, _
                resl.WebPublicationDate.Value, revokeDate, resl.Type.Description, xmlOther, xmlDoc, resl.WebSPGuid)
        Catch ex As Exception
            Throw New DocSuiteException("Ritiro", "Errore Ritiro internet: " + retVal.Guid, ex) With {
                .User = DocSuiteContext.Current.User.FullUserName
            }
        End Try

        FacadeFactory.Instance.ResolutionLogFacade.Log(resl, ResolutionLogType.WP, "File ritirato correttamente")

        Try
            resl.WebSPGuid = retVal.Guid
            resl.WebState = Resolution.WebStateEnum.Revoked
            resl.WebRevokeDate = revokeDate
            FacadeFactory.Instance.ResolutionFacade.Update(resl)
        Catch ex As Exception
            Throw New DocSuiteException("Ritiro", ex) With {
                .Descrizione = "Errore Ritiro internet: Impossibile aggiornare i dati sul database",
                .User = DocSuiteContext.Current.User.FullUserName
            }
        End Try
    End Sub
End Class