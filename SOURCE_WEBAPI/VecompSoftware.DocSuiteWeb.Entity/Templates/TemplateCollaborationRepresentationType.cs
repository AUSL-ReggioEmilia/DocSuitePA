namespace VecompSoftware.DocSuiteWeb.Entity.Templates
{
    public enum TemplateCollaborationRepresentationType : short
    {
        /// <summary>
        /// This are special type of fixed records (types of tempalte collaboration) are are 
        /// at Level1, under the root and they can have folders and tempaltes as children
        /// </summary>
        FixedTemplates = 0,

        /// <summary>
        /// These cannot have children
        /// </summary>
        Template = 1,

        /// <summary>
        /// These are folders that can have only Folders and templates as children
        /// </summary>
        Folder = 2
    }
}
