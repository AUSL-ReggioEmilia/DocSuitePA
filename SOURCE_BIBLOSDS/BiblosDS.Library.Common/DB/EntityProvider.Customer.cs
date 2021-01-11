using System;
using System.Collections.Generic;
using System.Linq;
using BiblosDS.Library.Common.Objects;
using Model = BiblosDS.Library.Common.Model;
using System.ComponentModel;
using System.Configuration;
using BiblosDS.Library.Common.Services;
using BiblosDS.Library.Common.Enums;
using System.Data;
using log4net;
using BiblosDS.Library.Common.Model;

namespace BiblosDS.Library.Common.DB
{
    public partial class EntityProvider
    {
        #region [ Methods ]

        public BindingList<DocumentArchive> GetCustomerArchives(string idCustomer, string userName)
        {
            var retval = new BindingList<DocumentArchive>();

            try
            {
                if (!string.IsNullOrWhiteSpace(idCustomer) || !string.IsNullOrWhiteSpace(userName))
                {
                    IQueryable<Model.CustomerKey> query = db.CustomerKey
                        .Include(x => x.Archive);

                    if (string.IsNullOrWhiteSpace(idCustomer))
                    {
                        idCustomer = db.CustomerLogin
                            .Where(x => userName.Equals(x.UserName, StringComparison.InvariantCultureIgnoreCase))
                            .Select(x => x.IdCustomer)
                            .FirstOrDefault<string>();
                    }

                    var archives = query
                        .Where(x => idCustomer.Equals(x.IdCustomer, StringComparison.InvariantCultureIgnoreCase))
                        .Select(x => x.Archive);

                    foreach (var a in archives)
                    {
                        retval.Add(a.Convert(DEFAULT_LEVEL, DEFAULT_DEEP_LEVEL));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                Dispose();
            }

            return retval;
        }

        public string GetCustomerIdByLogin(string userName, string encryptedPassword)
        {
            try
            {
                return db.CustomerLogin
                    .Where(x => x.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase) && x.Password.Equals(encryptedPassword))
                    .Select(s => s.IdCustomer)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                Dispose();
            }
        }

        public string GetCustomerIdByName(string username)
        {
            try
            {
                CustomerLogin customer = db.CustomerLogin.Where(x => x.UserName.Equals(username)).FirstOrDefault();
                if (customer == null)
                {
                    throw new Exception($"Customer not found!");
                }

                return customer.IdCustomer;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            finally
            {
                Dispose();
            }
        }

        public string GetCustomerOrCompanySignInfo(Guid idCompany, string username)
        {
            try
            {
                string signInfoByCompany = db.Company
                    .Where(x => x.IdCompany == idCompany)
                    .Select(x => x.SignInfo).FirstOrDefault();

                if (!string.IsNullOrEmpty(signInfoByCompany))
                {
                    return signInfoByCompany;
                }
                CustomerLogin customerByUsername = db.CustomerLogin
                        .Where(x => x.UserName.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault();

                return db.Customer.Where(x => x.IdCustomer == customerByUsername.IdCustomer).Select(x => x.SignInfo).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

        }
        public bool CustomerLoginExists(string userName, string encryptedPassword)
        {
            var ret = false;

            if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(encryptedPassword))
            {
                try
                {
                    ret = db.CustomerLogin.Any<CustomerLogin>(x => userName.Equals(x.UserName, StringComparison.InvariantCultureIgnoreCase)
                        && encryptedPassword.Equals(x.Password));
                }
                catch (Exception ex) { logger.Error(ex); throw; }
                finally { Dispose(); }
            }

            return ret;
        }

        public void UpdateCustomerPassword(string idCustomer, string newPassword)
        {
            try
            {
                CustomerLogin customerLogin = db.CustomerLogin.FirstOrDefault(x => x.IdCustomer == idCustomer);
                customerLogin.Password = newPassword;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
        #endregion
    }
}
