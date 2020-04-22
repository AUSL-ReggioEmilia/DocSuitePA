namespace VecompSoftware.BPM.Integrations.Modules.FALCK.Data.Entities
{
    public class WorkflowAttachment
    {
        #region [ Constructor ]
        public WorkflowAttachment()
        {

        }
        #endregion

        #region [ Properties ]
        public int AttachmentId { get; set; }
        public string InternalFileName { get; set; }
        public string OriginalFileName { get; set; }
        public byte IsMainDocument { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual WorkflowMetadata WorkflowMetadata { get; set; }
        #endregion
    }
}
