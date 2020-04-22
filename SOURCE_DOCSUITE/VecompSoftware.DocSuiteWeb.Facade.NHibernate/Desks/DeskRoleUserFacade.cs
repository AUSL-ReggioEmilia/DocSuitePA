using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Data.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Desks;
using VecompSoftware.DocSuiteWeb.DTO.Desks;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks
{
    public class DeskRoleUserFacade : BaseProtocolFacade<DeskRoleUser, Guid, DeskRoleUserDao>
    {
        #region [ Fields ]
        private readonly string _userName;
        private DeskLogFacade _deskLogFacade;
        private const string ADD_USER_LOG_DESCRIPTION_FORMAT = "Inserito nuovo utente {0}";
        private const string REMOVED_USER_LOG_DESCRIPTION_FORMAT = "Rimosso utente {0}";
        #endregion [ Fields ]

        #region [ ErrorMessage ]
        private const string USER_NOT_FOUND = "L'utente {0} non è presente nel tavolo con ID {1}";
        #endregion

        #region [ Properties ]
        private DeskLogFacade CurrentDeskLogFacade
        {
            get { return _deskLogFacade ?? (_deskLogFacade = new DeskLogFacade(_userName)); }
        }
        #endregion [ Properties ]

        #region [ Constructor ]
        public DeskRoleUserFacade(string userName) : base()
        {
            _userName = userName;
        }

        #endregion [ Constructor ]

        #region [ Methods ]

        /// <summary>
        /// Aggiunge un nuovo utente al tavolo
        /// </summary>
        public DeskRoleUser AddUser(DeskRoleUserResult dto, Desk desk)
        {
            DeskRoleUser deskUser = new DeskRoleUser
            {
                Desk = desk,
                AccountName = dto.UserName,
                PermissionType = DeskPermissionType.Reader
            };

            if(dto.PermissionType.HasValue)
                deskUser.PermissionType = dto.PermissionType.Value;

            this.Save(ref deskUser);

            //Inserisco il log
            CurrentDeskLogFacade.InsertLog(DeskLogType.Modify, string.Format(ADD_USER_LOG_DESCRIPTION_FORMAT, dto.UserName), desk, SeverityLog.Info);

            return deskUser;
        }

        /// <summary>
        /// Aggiunge una collezione di utenti al tavolo
        /// </summary>
        public ICollection<DeskRoleUser> AddUsers(ICollection<DeskRoleUserResult> dtos, Desk desk)
        {
            ICollection<DeskRoleUser> userAdded = new Collection<DeskRoleUser>();
            foreach (DeskRoleUserResult userResult in dtos)
            {
                userAdded.Add(AddUser(userResult, desk));
            }
            return userAdded;
        }

        /// <summary>
        /// Permette di modificare un utente di un tavolo
        /// </summary>
        public DeskRoleUser UpdateUser(DeskRoleUserResult dto, Desk desk)
        {
            if (dto == null)
                throw new ArgumentNullException("VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks.UpdateUser dto");

            if (desk == null)
                throw new ArgumentNullException("VecompSoftware.DocSuiteWeb.Facade.NHibernate.Desks.UpdateUser desk");

            DeskRoleUser user = _dao.GetByAccountName(dto.UserName, desk.Id);
            if (user == null)
                throw new Exception(string.Format(USER_NOT_FOUND, dto.UserName, desk.Id));

            user.PermissionType = dto.PermissionType ?? DeskPermissionType.Reader;
            this.Update(ref user);
            return user;
        }

        /// <summary>
        /// Permette di modificare una collezione di utenti di un tavolo
        /// </summary>
        public ICollection<DeskRoleUser> UpdateUsers(ICollection<DeskRoleUserResult> dtos, Desk desk)
        {
            ICollection<DeskRoleUser> userChanged = new Collection<DeskRoleUser>();
            foreach (var userResult in dtos)
            {
                userChanged.Add(UpdateUser(userResult, desk));
            }
            return userChanged;
        }

        public void RemoveUsers(ICollection<DeskRoleUser> users)
        {
            foreach (DeskRoleUser roleUser in users)
            {
                DeskRoleUser beforeDelete = roleUser;
                this.Delete(ref beforeDelete);
            }
        }

        public void RemoveUser(DeskRoleUserResult userDto, Desk desk)
        {
            DeskRoleUser roleUser = _dao.GetByAccountName(userDto.UserName, desk.Id);
            if (roleUser == null)
                return;

            this.Delete(ref roleUser);
            //Inserisco il log
            CurrentDeskLogFacade.InsertLog(DeskLogType.Modify, string.Format(REMOVED_USER_LOG_DESCRIPTION_FORMAT, userDto.UserName), desk, SeverityLog.Info);            
        }

        public void RemoveUsers(ICollection<DeskRoleUserResult> dtos, Desk desk)
        {            
            foreach (DeskRoleUserResult userDto in dtos)
            {
                RemoveUser(userDto, desk);
            }
        }

        /// <summary>
        /// Verifica se esiste almeno un utente con diritti di approvazione
        /// escludendo l'utente che ha aperto il tavolo
        /// </summary>
        public bool HasUsersApprovers(Guid idDesk)
        {
            return _dao.HasUsersApprovers(idDesk);
        }        
        #endregion [ Methods ]
    }
}
