using Limilabs.Mail;
using Limilabs.Mail.MIME;

namespace VecompSoftware.GenericMailSender.Extensions
{
    public static class LimilabsMailExtensions
    {
        public static void AddAttachment(this MailBuilder builder, IMail messageToAttach)
        {
            MimeRfc822 rfc822 = new MimeFactory().CreateMimeRfc822();
            rfc822.Data = messageToAttach.Render();
            builder.AddAttachment(rfc822);
        }

        public static IMail Clone(this IMail source)
        {
            var builder = new MailBuilder();
            return builder.CreateFromEml(source.Render());
        }
    }
}
