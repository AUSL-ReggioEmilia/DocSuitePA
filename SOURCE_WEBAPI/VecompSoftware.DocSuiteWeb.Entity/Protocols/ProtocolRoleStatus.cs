namespace VecompSoftware.DocSuiteWeb.Entity.Protocols
{
    public enum ProtocolRoleStatus
    {
        /// <summary>
        /// 'Si tratta di uno status presente SOLO in ProtocolRejectedRole
        /// </summary>
        Refused = -1,
        ToEvaluate = 0,
        Accepted = 1,
        /// <summary>
        ///     'Fixed: Se un'autorizzazione è stata rifiutata e il protocollo è stato nuovamente autorizzato ad un altro settore
        ///     'Si tratta di uno status presente SOLO in ProtocolRejectedRole
        /// </summary>
        Fixed = 2,
        Deactivated = 3,
    }
}