using Limilabs.Mail;
using Limilabs.Mail.Headers;
using Limilabs.Mail.MIME;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.Helpers.LimilabsMail.LimilabsMailExtensions;
using VecompSoftware.StampaConforme.Interfaces.Office;
using VecompSoftware.StampaConforme.Models.Office.Outlook;
using VecompSoftware.StampaConforme.Models.Office;
using VecompSoftware.StampConforme.Models.Commons;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VecompSoftware.StampaConforme.Services.Office
{
    [LogCategory(LogCategoryName.SERVICEOFFICE)]
    public class OutlookToPdfService : OfficeToPdfService, IOutlookToPdfService
    {
        #region [ Fields ]
        private bool _disposed;
        private readonly ILogger _logger;
        private dynamic _outlookInstance;
        private dynamic _outlookMail;
        private static ICollection<LogCategory> _logCategories;
        private const string APPLICATION_PROJ_ID = "Outlook.Application";
        private readonly ICollection<ContentType> _visualAllowedContentTypes = new List<ContentType>()
        {
            ContentType.ImageBmp,
            ContentType.ImageGif,
            ContentType.ImageJpeg,
            ContentType.ImagePng,
            ContentType.ImageTiff
        };
        private const string SCHEMANAME_CONTENTID = "http://schemas.microsoft.com/mapi/proptag/0x3712001E";
        private readonly List<string> ExtensionToFilterBySize = new List<string>(new string[] { ".PDF", ".P7M", ".TSD", ".M7M" });
        private const int SizeToFilter = 1000000;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(OutlookToPdfService));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Dispose ]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_outlookMail != null)
                {
                    try
                    {
                        _outlookMail.Close(MailInspectorClose.Discard);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteWarning(new LogMessage("Error on dispose mailitem instance. The exception will be ignore."), ex, LogCategories);
                    }
                }

                if (_outlookInstance != null)
                {
                    try
                    {
                        _outlookInstance.Quit();
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteWarning(new LogMessage("Error on dispose outlook instance. The exception will be ignore."), ex, LogCategories);
                    }
                }
            }

            _outlookMail = null;
            _outlookInstance = null;
            _disposed = true;
        }
        #endregion

        #region [ Constructor ]
        public OutlookToPdfService(ILogger logger)
            : base(APPLICATION_PROJ_ID)
        {
            _logger = logger;
        }
        #endregion

        #region [ Methods ]  
        public bool SaveTo(SaveMailToPdfRequest model)
        {
            try
            {
                return RetryingPolicyAction<bool>(() =>
                {
                    _logger.WriteInfo(new LogMessage(string.Concat("SaveTo -> process item ", model.SourceFilePath, ". Message type: ", Path.GetExtension(model.SourceFilePath))), LogCategories);
                OpenMail(model.SourceFilePath, model);
                
                string wordFileName = string.Concat(Path.GetTempFileName(), ".doc");
                _logger.WriteInfo(new LogMessage("SaveTo -> save loaded message as .doc for pdf conversion"), LogCategories);
                _outlookMail.SaveAs(wordFileName, SaveAsType.Doc);

                _logger.WriteInfo(new LogMessage("SaveTo -> convert .doc to pdf"), LogCategories);
                byte[] pdfDocument = ConvertWordDocumentToPdf(wordFileName);
                File.WriteAllBytes(model.DestinationFilePath, pdfDocument);
                _logger.WriteInfo(new LogMessage("SaveTo -> item saved to pdf correctly"), LogCategories);
                return true;
                }, "OUTLOOK", _logger, LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("SaveTo -> error on convert item ", model.SourceFilePath, " to pdf")), ex, LogCategories);
                throw;
            }
        }

        private void OpenMail(string mailPath, SaveMailToPdfRequest model)
        {
            InstantiateOutlook();

            string mailExt = Path.GetExtension(mailPath);
            if (mailExt.Equals(".msg", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.WriteInfo(new LogMessage(string.Concat("OpenMail -> item ", mailPath, " is .msg file. Open item with outlook directly.")), LogCategories);
                _outlookMail = OpenSharedItem(mailPath);
                return;
            }

            _logger.WriteInfo(new LogMessage(string.Concat("OpenMail -> item ", mailPath, " is not .msg file. Try build message manually.")), LogCategories);
            MailBuilder builder = new MailBuilder();
            IMail mail = builder.CreateFromEmlFile(mailPath);
            _outlookMail = ConvertToMsg(mail, model);

            if (_outlookMail == null)
            {
                _logger.WriteError(new LogMessage(string.Concat("OpenMail -> item ", mailPath, " not open. Check if item is a correct email message.")), LogCategories);
                throw new Exception("Mail not open. Check if item is a correct email message.");
            }
        }

        private void InstantiateOutlook()
        {
            if (_outlookInstance != null)
                return;

            _logger.WriteDebug(new LogMessage("InstantiateOutlook -> get or create new outlook instance"), LogCategories);
            _outlookInstance = GetRuntimeInstance();
            dynamic outlookInstanceNamespace = _outlookInstance.GetNamespace("MAPI");
            outlookInstanceNamespace.GetDefaultFolder(DefaultFolders.FolderInbox);
            outlookInstanceNamespace = null;
            _logger.WriteDebug(new LogMessage(string.Format("InstantiateOutlook -> outlook instance created: {0}", _outlookInstance != null)), LogCategories);
        }

        private dynamic OpenSharedItem(string mailPath)
        {
            return _outlookInstance.Session.OpenSharedItem(mailPath);
        }

        private dynamic ConvertToMsg(IMail email, SaveMailToPdfRequest model)
        {
            ConversionMode noHeaders = model.ConversionMode & ConversionMode.NoHeaders;
            ConversionMode noBody = model.ConversionMode & ConversionMode.NoBody;
            dynamic msg = _outlookInstance.CreateItem(OutlookItemType.MailItem);

            if (noHeaders != ConversionMode.NoHeaders)
            {
                msg.Subject = email.Subject;
                msg.BodyFormat = MailBodyFormat.FormatHTML;

                msg.SentOnBehalfOfName = email.From[0].Address;
                IEnumerable<MailBox> toMailBoxes = email.To.SelectMany(s => s.GetMailboxes());
                dynamic recipient;
                foreach (MailBox toMailBox in toMailBoxes)
                {
                    recipient = msg.Recipients.Add(toMailBox.Address);
                    recipient.Type = (int)MailRecipientType.To;
                }

                IEnumerable<MailBox> ccMailBoxes = email.Cc.SelectMany(s => s.GetMailboxes());
                dynamic ccRecipient;
                foreach (MailBox ccMailBox in ccMailBoxes)
                {
                    ccRecipient = msg.Recipients.Add(ccMailBox.Address);
                    ccRecipient.Type = (int)MailRecipientType.CC;
                }

                IEnumerable<MailBox> bccMailBoxes = email.Bcc.SelectMany(s => s.GetMailboxes());
                dynamic bccRecipient;
                foreach (MailBox bccMailBox in bccMailBoxes)
                {
                    bccRecipient = msg.Recipients.Add(bccMailBox.Address);
                    bccRecipient.Type = (int)MailRecipientType.BCC;
                }
            }

            if (noBody != ConversionMode.NoBody)
            {
                string bodyHtml = email.GetBodyAsHtml();
                msg.HTMLBody = System.Net.WebUtility.HtmlDecode(bodyHtml) ;
                string temporaryItem;
                dynamic attachmentAdded;
                foreach (MimeData visualItem in email.Visuals.Where(x => x.Size < SizeToFilter && !ExtensionToFilterBySize.Contains(Path.GetExtension(x.FileName))))
                {
                    temporaryItem = visualItem.SaveToTemp();
                    attachmentAdded = msg.Attachments.Add(temporaryItem, MailAttachmentType.Embeddeditem, null, visualItem.FileNameOrDefault());

                    if (_visualAllowedContentTypes.Contains(visualItem.ContentType) && !string.IsNullOrEmpty(visualItem.ContentId))
                    {
                        attachmentAdded.PropertyAccessor.SetProperty(SCHEMANAME_CONTENTID, visualItem.ContentId);
                    }
                };
            }
            return msg;
        }

        private byte[] ConvertWordDocumentToPdf(string wordFileFullName)
        {
            using (StampaConformeWS.BiblosDSConvSoapClient client = new StampaConformeWS.BiblosDSConvSoapClient())
            {
                StampaConformeWS.stDoc documentToConvert = new StampaConformeWS.stDoc()
                {
                    Blob = Convert.ToBase64String(File.ReadAllBytes(wordFileFullName)),
                    FileExtension = Path.GetFileName(wordFileFullName)
                };
                StampaConformeWS.stDoc documentConverted = client.ToRasterFormatEx(documentToConvert, "PDF", string.Empty);
                return Convert.FromBase64String(documentConverted.Blob);
            }
        }
        #endregion
    }
}
