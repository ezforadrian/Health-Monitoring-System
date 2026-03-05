using _VEHRSv1.Models;

namespace _VEHRSv1.Interface
{
    public interface IAccountManagementRepository
    {

        bool VerifyInternalUser(string UserName, string Password);
        List<vm_UserDetails> GetInternalUserInfo(string empID);
        List<vm_UserDetails> GetSystemUserInfo(string UserName);
        List<vm_UserDetails> GetUsersList();
        List<AspNetUser> GetEmployeeInfo(string empID);
        List<AspNetRole> GetRoles(string UserRole);
      
        bool EnrollUser(AspNetUser NewUser);
        bool AssignRole(AspNetUser NewUser, string UserRole);
        bool UpdateUser(vm_UserDetails UpdateUser);
        bool DeleteUser(string DeleteUser);
        bool Save();
    }
}
