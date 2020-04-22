interface SectionModel {
    Id: number;
    Url: string;
    HtmlUrl: string;
    CategoryId: number;
    Position: number;
    Sorting: string;
    CreatedAt: string;
    UpdatedAt: string;
    Name: string;
    Description: string;
    Locale: string;
    SourceLocale: string;
    Outdated: boolean;
    ParentSectionId: number;
    ThemeTemplate: string;
}

export = SectionModel;