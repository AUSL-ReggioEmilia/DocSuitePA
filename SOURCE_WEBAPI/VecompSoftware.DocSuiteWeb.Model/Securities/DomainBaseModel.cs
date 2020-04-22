
using System;

namespace VecompSoftware.DocSuiteWeb.Model.Securities
{
    public abstract class DomainBaseModel
    {
        //
        // Summary:
        //     Gets or sets the description of the principal.
        //
        // Returns:
        //     The description text for this principal or null if there is no description.
        public string Description { get; set; }
        //
        // Summary:
        //     Gets or sets the display name for this principal.
        //
        // Returns:
        //     The display name for this principal or null if there is no display name.
        public string DisplayName { get; set; }
        //
        // Summary:
        //     Gets the distinguished name (DN) for this principal.
        //
        // Returns:
        //     The DN for this principal or null if there is no DN.
        public string DistinguishedName { get; set; }
        //
        // Summary:
        //     Gets the GUID associated with this principal.
        //
        // Returns:
        //     The Nullable System.Guid associated with this principal or null if there is no
        //     GUID.
        public Guid? GUID { get; set; }
        //
        // Summary:
        //     Gets or sets the SAM account name for this principal.
        //
        // Returns:
        //     The SAM account name for this principal or null if no name has been set.
        //
        public string Name { get; set; }
        //
        // Summary:
        //     Returns an uppercase Security Descriptor Definition Language (SDDL) string for
        //     the security identifier (SID) represented by this System.Security.Principal.SecurityIdentifier
        //     object.
        //
        // Returns:
        //     An uppercase SDDL string for the SID represented by the System.Security.Principal.SecurityIdentifier
        //     object.
        public string SDDL_SID { get; set; }
    }
}
