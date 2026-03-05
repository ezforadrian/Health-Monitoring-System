using _VEHRSv1.Models;

namespace _VEHRSv1.Interface
{
    public interface IMaintenanceRepository
    {
        List<AspNetRole> GetRoles();
        bool AddRoles(AspNetRole NewRoles);
        bool DeleteRole(string roleName);
        bool Save();
    }
}
