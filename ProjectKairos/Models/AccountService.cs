using System.Collections.Generic;
using System.Linq;
using ProjectKairos.ViewModel;
using System.Data.Entity;
using Newtonsoft.Json;
using ProjectKairos.Utilities;

namespace ProjectKairos.Models
{
    public class AccountService
    {
        private KAIROS_SHOPEntities db;

        public AccountService(KAIROS_SHOPEntities db)
        {
            this.db = db;
        }

        public bool AdminUpdateAccount(string username, bool isActive, int roleId)
        {
            Account account = db.Accounts.Find(username);

            db.Accounts.Attach(account);

            account.IsActive = isActive;
            account.RoleId = roleId;

            int result = db.SaveChanges();
            if (db.Entry(account).State == EntityState.Unchanged || result > 0)
            {
                return true;
            }
            return false;
        }

        public AccountInfoRoleViewModel ViewAccountInfo(string username)
        {
            var account = db.Accounts.Where(a => a.Username == username).Select(a => new AccountInfoRoleViewModel
            {
                Username = a.Username,
                FullName = a.LastName + " " + a.FirstName,
                Phone = a.Phone,
                Email = a.Email,
                DOB = a.DOB,
                Gender = a.Gender,
                StartDate = a.StartDate,
                IsActive = a.IsActive,
                RoleId = a.RoleId
            }).FirstOrDefault();
            return account;
        }

        public List<AccountTableViewModel> LoadAccountTable(string sortColumnDirection, string searchValue, ref int recordsTotal, int pageSize, int skip)
        {
            // Getting all Account data  
            var account = db.Accounts.Include(a => a.Role).Select(a => new AccountTableViewModel
            {
                Username = a.Username,
                FullName = a.LastName + " " + a.FirstName,
                RoleName = a.Role.RoleName,
                IsActive = a.IsActive

            });

            //sorting
            if (!string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumnDirection)
                {
                    case "asc":
                        account = account.OrderBy(a => a.FullName);
                        break;
                    case "desc":
                        account = account.OrderByDescending(a => a.FullName);
                        break;
                }
            }

            //Search  
            if (!string.IsNullOrEmpty(searchValue))
            {
                account = account.Where(a => a.Username.Contains(searchValue) || a.FullName.Contains(searchValue));
            }

            //total number of rows count   
            recordsTotal = account.Count();

            //Paging   
            return account.Skip(skip).Take(pageSize).ToList();
        }

        public bool IsDuplicatedUsername(string username)
        {
            string result = db.Accounts.Where(a => a.Username == username).Select(a => a.Username).FirstOrDefault();
            return result != null;
        }

        public bool IsDuplicatedEmail(string email)
        {
            string result = db.Accounts.Where(a => a.Email == email).Select(a => a.Email).FirstOrDefault();
            return result != null;
        }

        public bool AddNewAccount(Account account)
        {
            db.Accounts.Add(account);
            int result = db.SaveChanges();
            return result > 0;
        }

        public string CheckLogin(string username, string password)
        {
            var result = db.Accounts
                .Include(a => a.Role)
                .Where(a => a.Username == username && a.IsActive == true)
                .Select(a => new { a.Username, a.Password, a.PasswordSalt, a.Role.RoleName, a.RoleId })
                .FirstOrDefault();
            if (result == null)
            {
                return null;

            }

            string saltKey = result.PasswordSalt;
            string encryptPassword = EncryptPasswordUtil.EncryptPassword(password, saltKey);

            //compared with encrypt password stored in database
            if (encryptPassword == result.Password)
            {
                //password is correct
                //Manage session
                //serialize username and role name to add to session
                //return to admin dashboard if admin
                var loginInfo = new
                {
                    Username = username,
                    RoleId = result.RoleId,
                    RoleName = result.RoleName
                };
                return JsonConvert.SerializeObject(loginInfo, Formatting.Indented);
            }

            return null;
        }

        public string GetRoleName(string username)
        {
            return db.Accounts
                .Include(a => a.Role)
                .Where(a => a.Username == username)
                .Select(a => a.Role.RoleName).FirstOrDefault();
        }

        public AccountInfoViewModel ViewMyAccount(string username)
        {
            var account = db.Accounts.Where(a => a.Username == username)
                .Select(a => new AccountInfoViewModel
                {
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Email = a.Email,
                    Phone = a.Phone,
                    DOB = a.DOB,
                    StartedDate = a.StartDate,
                    Gender = a.Gender
                }).First();
            return account;
        }

        public bool UpdateAccountInfo(Account account, string username)
        {
            Account currentUser = db.Accounts.Find(username);

            db.Accounts.Attach(currentUser);

            currentUser.FirstName = account.FirstName;
            currentUser.LastName = account.LastName;
            currentUser.Phone = account.Phone;
            currentUser.DOB = account.DOB;
            currentUser.Gender = account.Gender;
            int result = db.SaveChanges();
            if (db.Entry(currentUser).State == EntityState.Unchanged || result > 0)
            {
                return true;
            }
            return false;
        }

        public bool UpdateAccountPassword(string username, string password, string newPassword)
        {
            Account account = db.Accounts.Find(username);

            string encryptedPassword = EncryptPasswordUtil.EncryptPassword(password, account.PasswordSalt);
            if (encryptedPassword != account.Password)
            {
                //incorrect password
                return false;
            }

            db.Accounts.Attach(account);
            account.Password = EncryptPasswordUtil.EncryptPassword(newPassword, out string key);
            account.PasswordSalt = key;
            int result = db.SaveChanges();
            if (db.Entry(account).State == EntityState.Unchanged || result > 0)
            {
                return true;
            }
            return false;
        }

    }
}