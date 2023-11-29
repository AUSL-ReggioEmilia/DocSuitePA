enum TemplateCollaborationRepresentationType {
    /** 
     * This are special type of fixed records (types of tempalte collaboration) are are
     * at Level1, under the root and they can have folders and tempaltes as children
     */
    FixedTemplates = 0,

    /**
     * Representing a template and the tree edge node. The node can have no more children.
     **/
    Template = 1,

    /**
     * These are folders that can have only Folders and templates as children
     **/
    Folder = 2
}

export = TemplateCollaborationRepresentationType;