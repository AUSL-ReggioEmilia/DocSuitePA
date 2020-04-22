using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using VecompSoftware.Helpers;

namespace VecompSoftware.GenericMailSender.Extensions
{
    public static class MailMessageExtensions
    {
        public static string ToEml(this MailMessage message, DirectoryInfo tempDir)
        {
            return Encoding.ASCII.GetString(message.ToArray(tempDir));
        }

        public static MemoryStream ToStream(this MailMessage message, DirectoryInfo tempFolder)
        {
            return new MemoryStream(message.ToArray(tempFolder));
        }

        public static byte[] ToArray(this MailMessage message, DirectoryInfo tempFolder)
        {
            var temp = new DirectoryInfo(Path.Combine(tempFolder.FullName,
                FileHelper.UniqueFileNameFormat("debug_attach_mail.eml", "")));
            temp.Create();
            var client = new SmtpClient("mysmtphost")
                         {
                             DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                             PickupDirectoryLocation = temp.FullName
                         };
            client.Send(message);

            var bytes = File.ReadAllBytes(temp.GetFiles()[0].FullName);
            temp.Delete(true);
            return bytes;
        }

        public static Attachment ToAttachment(this MailMessage message, DirectoryInfo tempDir)
        {
            return new Attachment(message.ToStream(tempDir), "mail.eml", MediaTypeNames.Application.Octet);
        }
    }
}
