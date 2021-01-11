using AmministrazioneTrasparente.SQLite.Entities;
using AmministrazioneTrasparente.SQLite.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmministrazioneTrasparente.Services
{
    public class UserLogService
    {
        private readonly IRepository<UserLog> _repository;
        private readonly UserService _userService;

        public UserLogService()
        {
            this._repository = new Repository<UserLog>();
            this._userService = new UserService();
        }

        public IList<UserLog> GetLogs()
        {
            IEnumerable<UserLog> logs = this._repository.SelectQuery<User>(
                @"SELECT UserLogs.*, Users.* 
                    FROM UserLogs
                    INNER JOIN Users ON Users.Id = UserLogs.IdUser
                    ORDER BY UserLogs.LogDate DESC LIMIT 5");
            return logs.ToList();
        }

        public void AddLog(string username, string description)
        {
            User user = this._userService.GetUserByUsername(username);
            UserLog log = new UserLog()
            {
                IdUser = user.Id,
                Action = description,
                LogDate = DateTime.Now
            };

            this._repository.Insert(log);
        }
    }
}