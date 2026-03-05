using _VEHRSv1.Helper;
using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;


namespace _VEHRSv1.Repository
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly ILogger<MaintenanceRepository> _logger;
        private readonly HswmsContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;

        public MaintenanceRepository(ILogger<MaintenanceRepository> logger, HswmsContext hswmsContext, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _db = hswmsContext;
            _roleManager = roleManager;
        }
        public List<AspNetRole> GetRoles()
        {

            var RolesList = _db.AspNetRoles
                 .Select(role => new AspNetRole
                 {
                     Id = role.Id,
                     Name = role.Name,
                     NormalizedName = role.NormalizedName
                 })
                .ToList();


            return RolesList;

        }


        public bool AddRoles(AspNetRole newRole)
        {
            _db.AspNetRoles.Add(newRole);
            return Save();
        }

        public bool DeleteRole(string roleName)
        {
           
            var enrolledRole = _db.AspNetRoles.Where(x => x.Name == roleName).Select(x => x.Id).FirstOrDefault();
            bool UserRole = _db.AspNetUserRoles.Any(x => x.RoleId == enrolledRole);
            string[] DoNotDelete = { "SYSADMIN", "APPADMIN", "USER" }; //UserNames
            if (DoNotDelete.Contains(roleName) == false || UserRole == false)
            {
                
                var SelectedRole = _db.AspNetRoles.Where(a => a.Name == roleName).FirstOrDefault();
                _db.Remove(SelectedRole);       
                return Save();
            }
            else
            {
                return false;
            }
        }

        public bool Save()
        {
            return _db.SaveChanges() > 0;
        }
    }
}
