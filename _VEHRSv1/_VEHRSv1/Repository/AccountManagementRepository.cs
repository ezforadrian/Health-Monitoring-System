using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.DirectoryServices;
using System.Formats.Asn1;
using System.Linq;
using System.Security.Claims;
using System.DirectoryServices.ActiveDirectory;

namespace _VEHRSv1.Repository
{
    public class AccountManagementRepository : IAccountManagementRepository
    {
        private readonly ILogger<AccountManagementRepository> _logger;
        private readonly HswmsContext _db;
        

        public AccountManagementRepository(ILogger<AccountManagementRepository> logger, HswmsContext hswmsContext, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _db = hswmsContext;
            
        }

        public string DomainDefault = "192.168.100.21"; //LDAP Default Domain IP
        public string DomainName = "CORPORATE\\"; //DirectoryServices Default Domain Name

        public bool VerifyInternalUser(string UserName, string Password)
        {


            bool isValidEmployee = false;

            try
            {
                var ldapConn = new LdapConnection();
                ldapConn.Connect(DomainDefault, 389);
                ldapConn.Bind(DomainName + UserName, Password);
                isValidEmployee = true;
                ldapConn.Disconnect();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}, Source: {Source}", ex.Message, ex.Source);
                isValidEmployee = false;
            }

            return isValidEmployee;
        }
        public List<AspNetRole> GetRoles(string UserRole)
        {

            var isRoles = _db.AspNetRoles.Where(x => x.Name != UserRole && x.Name != "SYSADMIN").ToList();
            return isRoles;

        }

      

        public List<vm_UserDetails> GetInternalUserInfo(string empID)
        {
            
            var LoggedInUser = new vm_UserCredentials
            {
                UserName = "78-0001",
                Password = "Password01"
            };

            var UserInfo = new List<vm_UserDetails>();
            var UserDetails = new vm_UserDetails();

            if (VerifyInternalUser(LoggedInUser.UserName, LoggedInUser.Password) == true)
            {

                var ldapConn = new LdapConnection();
                ldapConn.Connect(DomainDefault, 389);
                ldapConn.Bind(DomainName + LoggedInUser.UserName, LoggedInUser.Password);
                try
                {
                    var searcher = new DirectorySearcher();
                    searcher.Filter = ("sAMAccountName=" + empID);
                    //var searchResult = searcher.FindOne().Properties["sAMAccountName"].ToString();
                    SearchResult searchResult = searcher.FindOne();

                    if (searchResult != null)
                    {
                        ResultPropertyCollection resultProperties = searchResult.Properties; //

                        UserDetails.UserName = resultProperties["sAMAccountName"][0].ToString().ToUpper();
                        UserDetails.FirstName = resultProperties["givenName"][0].ToString().ToUpper();
                        UserDetails.LastName = resultProperties["sn"][0].ToString().ToUpper();
                        UserDetails.MiddleName = resultProperties["initials"][0].ToString().ToUpper();
                        UserDetails.Department = resultProperties["Department"][0].ToString().ToUpper();
                        UserDetails.PayClass = resultProperties["Title"][0].ToString().ToUpper();
                        UserDetails.Email = resultProperties["userPrincipalName"][0].ToString().ToUpper();
                    }
                    else
                    {
                        throw new Exception("");
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred: {Message}, Source: {Source}", ex.Message, ex.Source);
                }

                ldapConn.Disconnect();
                UserInfo.Add(UserDetails);
            }

            return UserInfo;
        }

        public List<vm_UserDetails> GetSystemUserInfo(string UserName)
        {
            var UserDetails = _db.AspNetUsers.Where(a => a.UserName == UserName).FirstOrDefault();
            var UserRoleID = _db.AspNetUserRoles.Where(a => a.UserId == UserDetails.Id).FirstOrDefault();
            var UserRoleName = _db.AspNetRoles.Where(a => a.Id == UserRoleID.RoleId).FirstOrDefault();

            var UserInfo = new vm_UserDetails
            {
                UserRole = UserRoleName.Name,
                UserName = UserDetails.UserName,
                LastName = UserDetails.LastName,
                FirstName = UserDetails.FirstName,
                MiddleName = UserDetails.MiddleName,
                Department = UserDetails.Department,
                PayClass = UserDetails.Position,
                Email = UserDetails.Email,
                Password = UserDetails.PasswordHash
            };

            var ReturnInfo = new List<vm_UserDetails> { UserInfo };

            return ReturnInfo;
        }



        public List<vm_UserDetails> GetUsersList()
        {
            var ActiveUsersList = _db.AspNetUsers.ToList();

            var UsersList = _db.AspNetUsers;
            var RolesList = _db.AspNetRoles;
            var UserRoles = _db.AspNetUserRoles;
            var CompleteList = (
                    from a in UserRoles
                    join b in UsersList on a.UserId equals b.Id
                    join c in RolesList on a.RoleId equals c.Id
                    select new vm_UserDetails()
                    {
                        UserRole = c.Name,
                        UserName = b.UserName,
                        FirstName = b.FirstName,
                        LastName = b.LastName,
                        MiddleName = b.MiddleName,
                        Department = b.Department,
                        PayClass = b.PayClass,
                        Position = b.Position,
                        Email = b.Email,
                    }
                ).ToList();


            return CompleteList;
        }

        //Get Info from Active Directory (PAGCOR Employees)
        public List<AspNetUser> GetEmployeeInfo(string empID)
        {
            var UserInformation = new List<AspNetUser>().Where(a => a.Id == "").ToList();

            //Check UserID validity from AD

            return UserInformation;
        }

        public bool EnrollUser(AspNetUser NewUser)
        {
            var passwordHasher = new PasswordHasher<AspNetUser>();
            NewUser.NormalizedUserName = NewUser.UserName.ToString().ToUpper();
            NewUser.NormalizedEmail = NewUser.Email.ToString().ToUpper();
            NewUser.PasswordHash = passwordHasher.HashPassword(NewUser, NewUser.PasswordHash.ToString());

            _db.Add(NewUser);
            return Save();
        }

        public bool AssignRole(AspNetUser NewUser, string UserRole)
        {
            var RoleId = _db.AspNetRoles.Where(a => a.Name == UserRole.Trim().ToUpper()).FirstOrDefault().Id;
            var AddRole = new AspNetUserRole
            {
                UserId = NewUser.Id,
                RoleId = RoleId
            };
            _db.Add(AddRole);
            return Save();
        }

            public bool UpdateUser(vm_UserDetails UpdateUser)
            {
          
                    var SelectedUser = _db.AspNetUsers.Where(a => a.UserName == UpdateUser.UserName).FirstOrDefault();
                    var UserRole = _db.AspNetRoles.Where(a => a.Name == UpdateUser.UserRole).FirstOrDefault();
                    var SelectedUserRole = _db.AspNetUserRoles.Where(a => a.UserId == SelectedUser.Id).FirstOrDefault();

                    bool isExist = _db.AspNetUserRoles.Any(x => x.UserId == SelectedUser.Id);



            if (isExist)
            {
                if (SelectedUserRole.RoleId == UserRole.Id)
                {
                    return Save();
                }
                else
                {
                    _db.Remove(SelectedUserRole);
                }
              
            }
           
                var updateRole = new AspNetUserRole
                {
                    UserId = SelectedUser.Id,
                    RoleId = UserRole.Id // Assign the new role
                };
            _db.Add(updateRole);
            return Save();
                //return true;
            }

        public bool DeleteUser(string DeleteUser)
        {

                var SelectedUser = _db.AspNetUsers.Where(a => a.UserName == DeleteUser).FirstOrDefault();
                var SelectedUserRole = _db.AspNetUserRoles.Where(a => a.UserId == SelectedUser.Id).FirstOrDefault();
                _db.Remove(SelectedUser);
                _db.Remove(SelectedUserRole);
                return Save();    
        }

        public bool Save()
        {
            return _db.SaveChanges() > 0;
        }

    }
}
