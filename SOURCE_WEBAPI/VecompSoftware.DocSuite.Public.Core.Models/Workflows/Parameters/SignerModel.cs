using System;
using VecompSoftware.DocSuite.Public.Core.Models.Securities;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Modello del Firmatario di una Collaborazione 
    /// </summary>
    public sealed class SignerModel : IWorkflowModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Modello del Firmatario di una Collaborazione
        /// </summary>
        /// <param name="order">Ordine dei firmatari. Questo parametro serve per determinare la sequenza corretta approvativa nel processo di Collaborazione/Firma</param>
        /// <param name="required">Specifica se la firma è obblicatoria (firma digitale) o semplicemente un 'visto a procedere'</param>
        /// <param name="signerType">Rappresenta la tipologia di firmatario, es: Settore DocSuite o Utente di Dominio<see cref="SignerType"/></param>
        /// <param name="identity">Modello dell'utente di Dominio</param>
        /// <param name="role">Modello del settore della DocSuite (o associazione di tag per la trascodifica)</param>
        public SignerModel(ushort order, bool required, SignerType signerType,
            IdentityModel identity = null, RoleModel role = null)
        {
            Order = order;
            Required = required;
            SignerType = signerType;
            Identity = identity;
            Role = role;

            switch (signerType)
            {
                case SignerType.Invalid:
                    throw new ArgumentException("SignerType non è valido");
                case SignerType.AD:
                    {
                        if (identity == null || string.IsNullOrEmpty(identity.Account))
                        {
                            throw new ArgumentException("La proprietà Account(IdentityModel) non può essere vuota se si imposta SignerType.AD");
                        }
                        break;
                    }
                case SignerType.DSWRole:
                    {
                        if (role == null || !role.RoleUniqueId.HasValue)
                        {
                            throw new ArgumentException("La proprietà RoleUniqueId(RoleModel) non può essere vuota se si imposta SignerType.DSWRole");
                        }
                        break;
                    }
                case SignerType.Mapping:
                    {
                        if (role == null || string.IsNullOrEmpty(role.ExternalTagIdentifier))
                        {
                            throw new ArgumentException("La proprietà ExternalTagIdentifier(RoleModel) non può essere vuota se si imposta SignerType.Mapping");
                        }
                        break;
                    }
                default:
                    break;
            }
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Ordine dei firmatari. Questo parametro serve per determinare la sequenza corretta approvativa nel processo di Collaborazione/Firma
        /// </summary>
        public ushort Order { get; private set; }

        /// <summary>
        /// Specifica se la firma è obblicatoria (firma digitale) o semplicemente un 'visto a procedere'
        /// </summary>
        public bool Required { get; private set; }

        /// <summary>
        /// Modello di sicuezza dell'utente di dominio
        /// </summary>
        public IdentityModel Identity { get; private set; }

        /// <summary>
        /// Modello di sicurezza del settore della DocSuite (o associazione di tag per la trascodifica)
        /// </summary>
        public RoleModel Role { get; private set; }

        /// <summary>
        /// Rappresenta la tipologia di firmatario, es: Settore DocSuite o Utente di Dominio
        /// </summary>
        public SignerType SignerType { get; private set; }

        #endregion
    }
}
