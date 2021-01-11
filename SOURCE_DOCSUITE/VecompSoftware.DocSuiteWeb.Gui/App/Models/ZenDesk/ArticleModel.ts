interface ArticleModel {
    AuthorId: number;
    Body: string;
    CommentsDisabled: boolean;
    CreatedAt: string;
    Draft: boolean;
    EditedAt: string;
    HtmlUrl: string;
    Id: number;
    LabelNames: string[];
    Locale: string;
    Name: string;
    Outdated: boolean;
    OutdatedLocales: string[];
    PermissionGroupId: number;
    Position: number;
    Promoted: boolean;
    SectionId: number;
    SourceLocale: string;
    Title: string;
    UpdatedAt: string;
    Url: string;
    UserSegmentId: number;
    VoteCount: number;
    VoteSum: number;
}

export = ArticleModel;