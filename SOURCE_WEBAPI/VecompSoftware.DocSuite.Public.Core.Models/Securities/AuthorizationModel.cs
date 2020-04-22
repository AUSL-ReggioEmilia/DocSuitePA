using System;
using System.ComponentModel.DataAnnotations;

namespace VecompSoftware.DocSuite.Public.Core.Models.Securities
{
    /// <summary>
    /// Modello astratto del contesto di sicurezza
    /// </summary>
    public abstract class AuthorizationModel : BaseModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Modello astratto del contesto di sicurezza
        /// </summary>
        /// <param name="authorizationType">Tiplogia di sicurezza <see cref="AuthorizationType"/></param>
        /// <param name="id">Chaive univooca</param>
        public AuthorizationModel(AuthorizationType authorizationType, Guid id)
            : base(id)
        {
            Authorization = authorizationType;
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Tipologia di sicurezza
        /// </summary>
        [Required]
        public AuthorizationType Authorization { get; private set; }

        #endregion
    }
}
