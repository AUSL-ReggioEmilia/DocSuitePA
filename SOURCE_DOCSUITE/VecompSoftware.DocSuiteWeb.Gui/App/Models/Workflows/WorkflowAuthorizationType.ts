enum WorkflowAuthorizationType {
    None = 0,
    AllRoleUser = 1,
    AllSecretary = 2,
    AllSigner = AllSecretary * 2,
    AllManager = AllSigner * 2,
    AllOChartRoleUser = AllManager * 2,
    AllOChartManager = AllOChartRoleUser * 2,
    AllOChartHierarchyManager = AllOChartManager * 2,
    UserName = AllOChartHierarchyManager * 2,
    ADGroup = UserName * 2,
    MappingTags = ADGroup * 2,
    AllDematerialisationManager = MappingTags * 2,
    AllProtocolSecurityUsers = AllDematerialisationManager * 2,
    AllUDSSecurityUsers = AllProtocolSecurityUsers * 2,
    AllPECMailBoxRoleUser = AllUDSSecurityUsers * 2
}

export = WorkflowAuthorizationType;