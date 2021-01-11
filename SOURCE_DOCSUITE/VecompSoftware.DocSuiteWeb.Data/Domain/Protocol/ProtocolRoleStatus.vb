Public Enum ProtocolRoleStatus
    'Si tratta di uno status presente SOLO in ProtocolRejectedRole
    Refused = -1
    ToEvaluate = 0
    Accepted = 1
    'Fixed: Se un'autorizzazione è stata rifiutata e il protocollo è stato nuovamente autorizzato ad un altro settore
    'Si tratta di uno status presente SOLO in ProtocolRejectedRole
    Fixed = 2
    Deactivated = 3
End Enum