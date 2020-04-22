
export interface AppConfig {
    apiOdataAddress: string;
    apiAuthAddress: string;
    apiRestAddress: string;
    apiRestControllers: [string, boolean];
    apiOdataControllers: [string, boolean];
    documentHandler: string;
    toastLife: number;
    applicationName: string;
    gridItemsNumber: number;
    titleIntroduction: string;
    introduction: string;
    oauthUsername: [string, string];
    title: string;
    appLogo: string;
}