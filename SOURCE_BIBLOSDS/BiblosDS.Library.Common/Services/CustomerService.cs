using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.Common.Objects.UtilityService;
using System.ComponentModel;

namespace BiblosDS.Library.Common.Services
{
    public class CustomerService : ServiceBase
    {
        static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(CustomerService));

        public static bool CustomerLoginExists(string userName, string plainTextPassword)
        {
            try
            {
                var encryptedPassword = PasswordService.GenerateHash(plainTextPassword ?? string.Empty);
                return DbProvider.CustomerLoginExists(userName, encryptedPassword);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                logger.Info("END");
            }
        }

        public static void ChangeCustomerPassword(string userName, string oldPassword, string newPassword)
        {
            try
            {
                string encryptedOldPassword = PasswordService.GenerateHash(oldPassword);
                string customerId = DbProvider.GetCustomerIdByLogin(userName, encryptedOldPassword);
                if (string.IsNullOrEmpty(customerId))
                {
                    throw new Exception(string.Concat("Username '", userName, "' with password '", oldPassword, "' not found"));
                }
                string encryptedNewPassword = PasswordService.GenerateHash(newPassword);
                DbProvider.UpdateCustomerPassword(customerId, encryptedNewPassword);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public static BindingList<DocumentArchive> GetCustomerArchivesByUsername(string userName)
        {
            logger.InfoFormat("username {0}", userName);
            try
            {
                return DbProvider.GetCustomerArchives(string.Empty, userName);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                logger.Info("END");
            }
        }
    }
}
