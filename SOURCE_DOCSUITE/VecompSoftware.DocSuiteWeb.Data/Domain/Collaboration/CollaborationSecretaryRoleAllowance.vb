''' <summary> Tolleranza per l'estrazione dei settori da collaborazione. </summary>
Public Enum CollaborationSecretaryRoleAllowance
    ''' <summary> Tutti i settori legati alla collaborazione. </summary>
    EveryAssociatedRoles = 0
    ''' <summary> Tutti i settori di destinazione legati alla collaborazione. </summary>
    DestinationFirstRoles = 1
    ''' <summary> Tutti i settori di destinazione legati alla collaborazione con controllo operatore. </summary>
    UserDestinationFirstRoles = 2
    ''' <summary> Tutti i settori di destinazione legati alla collaborazione con operatore segretario. </summary>
    SecretaryDestinationFirstRoles = 3
End Enum
