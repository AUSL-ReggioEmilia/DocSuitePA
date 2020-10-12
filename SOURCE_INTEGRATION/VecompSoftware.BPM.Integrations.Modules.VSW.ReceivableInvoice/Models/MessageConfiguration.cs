namespace VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice.Models
{
   public class MessageConfiguration
    {
        /// <summary>
        /// Each string contains one email. Multiple emails should be setup in multiple items within the array
        /// </summary>
        public string[] EmailRecipients { get; set; }
        public string Subject { get; set; }
    }
}
