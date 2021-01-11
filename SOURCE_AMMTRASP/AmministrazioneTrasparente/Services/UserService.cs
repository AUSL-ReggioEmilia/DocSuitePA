using AmministrazioneTrasparente.Code;
using AmministrazioneTrasparente.SQLite.Entities;
using AmministrazioneTrasparente.SQLite.Repositories;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.Helpers.ExtensionMethods;

namespace AmministrazioneTrasparente.Services
{
    public class UserService
    {
        private readonly IRepository<User> _repository;

        public UserService()
        {
            this._repository = new Repository<User>();
        }

        public bool Authenticate(string username, string password)
        {
            var pwdEncrypted = Security.GetMd5Hash(password);
            IEnumerable<User> users = this._repository.SelectQuery("WHERE Username =@0 COLLATE NOCASE AND Password =@1", username, pwdEncrypted);
            return !users.IsNullOrEmpty();
        }

        public User GetUserByUsername(string username)
        {            
            IEnumerable<User> users = this._repository.SelectQuery("WHERE Username =@0 COLLATE NOCASE", username);
            if (users.IsNullOrEmpty()) return null;
            return users.First();
        }

        public User GetUserLogged()
        {
            return GetUserByUsername(Security.Username);
        }

        public User GetUserById(int id)
        {
            return this._repository.Find(id);
        }

        public void UpdateUser(string name, string surname, string email)
        {
            User user = GetUserByUsername(Security.Username);
            user.Name = name;
            user.Surname = surname;
            user.Email = email;
            this._repository.Update(user);
        }

        public void ChangePassword(string newPassword)
        {
            User user = GetUserByUsername(Security.Username);
            var pwdEncrypted = Security.GetMd5Hash(newPassword);
            user.Password = pwdEncrypted;
            this._repository.Update(user);
        }
    }
}