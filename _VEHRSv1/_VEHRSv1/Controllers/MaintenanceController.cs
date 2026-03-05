using _VEHRSv1.Helper;
using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace _VEHRSv1.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly IMaintenanceRepository _maintenanceRepository;
        public MaintenanceController(IMaintenanceRepository maintenanceRepository)
        {
            _maintenanceRepository = maintenanceRepository;
        }

        [Authorize(Roles = "SYSADMIN")]
        public IActionResult UserRole()
        {
            return View();
        }

       
        [Authorize(Roles = "SYSADMIN")]
        [HttpGet]
        public IActionResult GetRoleList(int start, int length, string? search)
        {
            try
            {

                var RolesList = new List<AspNetRole>();
                
                RolesList = _maintenanceRepository.GetRoles().ToList();
                var edata = RolesList;

                var result = edata
                        .Skip(start)
                        .Take(length);
                return Json(new { data = result });


            }
            catch
            {
                return Json("No Data");
            }
        }

        
        [Authorize(Roles = "SYSADMIN")]
        [HttpPost] 
        [ValidateAntiForgeryToken]
        public IActionResult addRoles([FromBody] AspNetRole newRoles)
        {
            var processStatus = 0;
            string processMessage = "";
            
            if (newRoles == null || string.IsNullOrEmpty(newRoles.Name))
            {
                processMessage = "Role name is required.";
                //return Json(new { processStatus, processMessage });
            }
            bool isSystemUser = _maintenanceRepository.GetRoles().Where(a => string.Equals(a.Name, newRoles.Name, StringComparison.OrdinalIgnoreCase)).Select(a => a.Name).Count() > 0;
            bool try1 = isSystemUser;
            var newRole = new AspNetRole
            {
                Name = newRoles.Name.ToUpper(), 
                NormalizedName = newRoles.NormalizedName.ToUpper()
            };
            if (isSystemUser == true)
            {
                processStatus = 2;
                processMessage = "Role already existed.";
            }
            else
            {
                _maintenanceRepository.AddRoles(newRole);
                processStatus = 1;
                processMessage = "Role added successfully.";
            }
            return Json(new { processStatus, processMessage });
        }


      
        [Authorize(Roles = "SYSADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRoles([FromBody] AspNetRole roles)
        {

            var returnText = "";
            string logDescription = "";

            try
            {
                var RoleNames = roles.Name;
                bool isDeleted = _maintenanceRepository.DeleteRole(RoleNames);
                if (isDeleted)
                {
                    logDescription = "Success | Delete User";
                    returnText = "User has been deleted";
                }
                else 
                {
                    logDescription = "Error | Delete User";
                    returnText = "Cannot delete this user";
                }
            }
            catch (Exception ex) {
                returnText = ex.Message.ToString();
            }

            return Json(returnText);
        }





        //LastLine
    }
}
