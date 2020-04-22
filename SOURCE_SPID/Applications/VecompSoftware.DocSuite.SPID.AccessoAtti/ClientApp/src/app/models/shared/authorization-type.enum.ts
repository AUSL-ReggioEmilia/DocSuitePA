

export enum AuthorizationType {
    Invalid = -1,
    External = 0,
    NTLM = 1,
    DocSuiteSecurity = 2,
    DocSuiteToken = DocSuiteSecurity * 2,
    OAuth = DocSuiteToken * 2,
    JWT = OAuth * 2,
    SPID = JWT * 2,
}
