Imports System
Imports System.Collections.Generic
Imports System.Net.Mail
Imports System.Threading
Imports System.IO
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.Biblos.Models

<ComponentModel.DataObject()> _
Public Class UserErrorFacade
    Inherits BaseProtocolFacade(Of UserError, Integer, NHibernateUserErrorDao)

    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary>
    ''' Esegue un inserimento dentro alla tabella UserError
    ''' </summary>
    ''' <param name="ModuleName">Nome del modulo da salvare</param>
    ''' <param name="ErrorDescription">Descrizione dell'errore</param>
    ''' <param name="DBType">Nome della sessione NHibernate da utilizzare</param>
    Public Sub Insert(ByVal ModuleName As String, ByVal ErrorDescription As String, Optional ByVal DBType As String = "ProtDB")
        Dim userErrorEnabled As Boolean = False
        Select Case UCase(DBType)
            Case "PROTDB"
                userErrorEnabled = DocSuiteContext.Current.ProtocolEnv.IsUserErrorEnabled
            Case "RESLDB"
                userErrorEnabled = DocSuiteContext.Current.ResolutionEnv.IsUserErrorEnabled
        End Select
        If userErrorEnabled Then
            Dim userError As New UserError
            Dim i As Integer
            For i = 1 To 10
                userError.Id = _dao.GetMaxId(DBType) + 1
                userError.SystemUser = DocSuiteContext.Current.User.FullUserName
                userError.SystemComputer = DocSuiteContext.Current.UserComputer
                userError.SystemServer = CommonShared.MachineName
                userError.ErrorDate = _dao.GetServerDate()
                userError.ModuleName = Left(ModuleName, 50)
                userError.ErrorDescription = Left(ErrorDescription, 1000)
                Try
                    Me.Save(userError)
                    Exit For
                Catch ex As Exception
                    Thread.Sleep(50)
                    _unitOfWork.Clear()
                End Try
            Next i
        End If
        SmtpLogSendMail(ModuleName & "<BR>" & StringHelper.ReplaceCrLf(ErrorDescription, "<BR>"))
    End Sub

    Public Sub SmtpLogSendMail(ByVal Description As String)
        Try
            Dim mailSmtpServer As String = DocSuiteContext.Current.ProtocolEnv.MailSmtpServer
            If mailSmtpServer = "" Then
                Exit Sub
            End If
            Dim userErrorMailTo As String = DocSuiteContext.Current.ProtocolEnv.UserErrorMailTo
            If userErrorMailTo = "" Then
                Exit Sub
            End If
            Dim userErrorMailFrom As String = DocSuiteContext.Current.ProtocolEnv.UserErrorMailFrom
            Dim corporateAcronym As String = DocSuiteContext.Current.ProtocolEnv.CorporateAcronym
            Dim corporateName As String = DocSuiteContext.Current.ProtocolEnv.CorporateName
            Dim mm As New MailMessage

            mm.From = New MailAddress(If(userErrorMailFrom = "", "DocSuite@" & corporateAcronym, userErrorMailFrom))
            mm.To.Add(userErrorMailTo)
            mm.Subject = DocSuiteContext.ProductName & " Errore (" & corporateName & ")"
            mm.Body = Description
            Dim server As New SmtpClient(mailSmtpServer)
            server.UseDefaultCredentials = True
            server.Send(mm)
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
        End Try
    End Sub

    Public Sub SmtpLogSendMail(ByVal mailObject As String, ByVal mailDescription As String, _
        ByRef letteraFile As DocumentInfo, ByRef returnError As String, _
        ByVal mailTo As String, Optional ByVal mailCCn As String = "", Optional ByVal mailToCurrentUser As Boolean = False)
        ' TODO: c'è bisogno di passare una var di errore by ref?! ... blaw... da rifare con JS (dopo aver capito cosa deve effettivamente fare)
        Try
            Dim mailSmtpServer As String = DocSuiteContext.Current.ResolutionEnv.MailSmtpServer
            Dim sendMailFrom As String = DocSuiteContext.Current.ResolutionEnv.SendMailFrom
            If mailSmtpServer = "" Then Exit Sub

            If mailToCurrentUser Then
                If mailTo = "" Then
                    mailTo &= CommonUtil.GetInstance.UserMail
                Else
                    mailTo &= ";" & CommonUtil.GetInstance.UserMail
                End If
            End If

            If mailTo = "" Then Exit Sub
            If sendMailFrom = "" Then Exit Sub

            Using mm As New MailMessage
                mm.IsBodyHtml = True
                mm.From = New MailAddress(sendMailFrom)
                mm.To.Add(mailTo)
                If mailCCn <> "" Then
                    mm.Bcc.Add(mailCCn)
                End If
                mm.Subject = mailObject
                mm.Attachments.Add(New Attachment(New MemoryStream(letteraFile.Stream), letteraFile.Caption))
                mm.Body = mailDescription

                Dim server As New SmtpClient(mailSmtpServer)
                server.UseDefaultCredentials = True
                Try
                    server.Send(mm)
                Finally
                    mm.Dispose()
                End Try
            End Using
        Catch ex As Exception
            returnError = String.Format("UtlSmtpLogSendMail: {0} - {1}", ex.Message, If(ex.InnerException IsNot Nothing, ex.InnerException.Message, String.Empty))
            FileLogger.Error(LoggerName, returnError, ex)
        End Try

    End Sub

    Public Function SearchByModule(ByVal User As String, ByVal ModuleName As String, ByVal DateFrom As DateTime) As IList(Of UserError)
        Return _dao.SearchByModule(User, ModuleName, DateFrom)
    End Function
End Class