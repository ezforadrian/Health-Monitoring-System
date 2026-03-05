using _VEHRSv1.Helper;
using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using _VEHRSv1.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _VEHRSv1.Controllers
{
    public class EnrollmentController : Controller
    {

        private readonly IAccountManagementRepository _accountManagementRepository;
        public EnrollmentController(IAccountManagementRepository enrollmentRepository)
        {
            _accountManagementRepository = enrollmentRepository;
        }

        //VIEW(S)
        [Authorize(Roles = "SYSADMIN,APPADMIN")]
        public IActionResult Index()
        {
            var UserRole = User.Claims.Where(a => a.Type == ClaimTypes.Role).Select(a => a.Value).FirstOrDefault();
            var empID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roles = _accountManagementRepository.GetRoles(UserRole);
            
            return View(roles);
        }
   
        //GET DATA 
        [ValidateAntiForgeryToken]
        public IActionResult GetEmployeeInfo()
        {
            var EmployeeID = Request.Form["UserId"];

            //var GetEmployeeInfo = _accountManagementRepository.GetEmployeeInfo(EmployeeID);
           var GetEmployeeInfo = _accountManagementRepository.GetInternalUserInfo(EmployeeID);

            if (GetEmployeeInfo.Count() > 0)
            {
                return Json(GetEmployeeInfo.First());
            }
            else
            {
                return Json("");
            }
        }

        //GET USER IN DATATABLE
       
        [Authorize(Roles = "SYSADMIN,APPADMIN,USER")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetUsersList(int start, int length, string? search)
        {
            try
            {
                
                var UsersList = new List<vm_UserDetails>();
                string CurrentUser = User.Claims.First().Value;
                var UserRole = User.Claims.Where(a => a.Type == ClaimTypes.Role).Select(a => a.Value).FirstOrDefault();
                var best = UserRole;
                //SHOW ALL USER EXCEPT SYSADMIN AND SAME ROLE
                UsersList = _accountManagementRepository.GetUsersList().Where(x => x.UserRole != "SYSADMIN" && x.UserRole != UserRole && x.UserName != CurrentUser).ToList();

                var edata = UsersList;

                var result = edata
                        .Skip(start)
                        .Take(length);
                return Json(new { data = result });
                //return Json(UsersList);
            }
            catch
            {
                return Json("No Data");
            }
        }

        //ENROLL USERS
        [Authorize(Roles = "SYSADMIN,APPADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnrollNewUser([FromBody] vm_UserDetails newUserData)
        {
            var ProcessStatus = 0;
            string ProcessMessage = "";
            string logDescription = "";
            string invalidInputs = "";
            string blankInputs = "";
            try
            {
                //Check if UserName(specifically) is Empty
                if (string.IsNullOrEmpty(newUserData.UserName.Trim()))
                {
                    throw new Exception("3");
                }

                //Check if UserName already exists
                bool isSystemUser = _accountManagementRepository.GetUsersList().Where(a => a.UserName == newUserData.UserName).Count() > 0;
                if (isSystemUser)
                {
                    throw new Exception("6");
                }

                //Set New Enrollee Details
                var NewEnrollment = new AspNetUser();
                bool isPAGCORID = false;

                string? RoleName = newUserData.UserRole;
                if (RoleName == "USER" || RoleName == "APPADMIN")
                {
                    var PAGCOR_EmployeeInfo = _accountManagementRepository.GetInternalUserInfo(newUserData.UserName);

                    //Check if PAGCOR User AND PAGCOR UserName is not Empty
                    if (PAGCOR_EmployeeInfo.Count > 0 && PAGCOR_EmployeeInfo.FirstOrDefault().UserName != "")
                    {
                        var VerifiedUser = PAGCOR_EmployeeInfo.FirstOrDefault();
                        isPAGCORID = true;

                        NewEnrollment.UserName = VerifiedUser.UserName;
                        NewEnrollment.FirstName = VerifiedUser.FirstName;
                        NewEnrollment.LastName = VerifiedUser.LastName;
                        NewEnrollment.MiddleName = VerifiedUser.MiddleName;
                        NewEnrollment.Department = VerifiedUser.Department;
                        NewEnrollment.Email = VerifiedUser.Email;
                        NewEnrollment.Position = VerifiedUser.PayClass;
                        NewEnrollment.PasswordHash = "Password01"; //Placeholder Value for now
                    }
                    else
                    {
                        throw new Exception("2");
                    }
                }

            
                _accountManagementRepository.EnrollUser(NewEnrollment);
                _accountManagementRepository.AssignRole(NewEnrollment, RoleName);

                //Generate Log
                logDescription = "Success | Enroll User | [" + User.Claims.First().Value + "] successfully enrolled new user: [" + newUserData.UserName + "] as [" + newUserData.UserRole + "] user type | Execution Date: " + DateTime.Now.ToString();

                //Set ProcessStatus
                ProcessStatus = 1; //User Enrollment Success
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An error occurred: {Message}, Source: {Source}", ex.Message, ex.Source);
                if (ex.Message.ToString() == "2")
                {
                    logDescription = "Error | Enroll User | [" + User.Claims.First().Value + "] failed to enroll [" + newUserData.UserName + "]. Invalid PAGCOR ID Number | Execution Date: " + DateTime.Now.ToString();
                    ProcessStatus = 2; //Invalid PAGCOR ID
                }
                else if (ex.Message.ToString() == "3")
                {
                    logDescription = "Error | Enroll User | [" + User.Claims.First().Value + "] cannot enroll UserName with empty or null value | Execution Date: " + DateTime.Now.ToString();
                    ProcessStatus = 3; //Empty UserName
                }
                else if (ex.Message.ToString() == "4")
                {
                    logDescription = "Error | Enroll User | [" + User.Claims.First().Value + "] attempted to enroll user with invalid field format | Execution Date: " + DateTime.Now.ToString();
                    ProcessStatus = 4; //An Input has invalid format
                }
                else if (ex.Message.ToString() == "5")
                {
                    logDescription = "Error | Enroll User | [" + User.Claims.First().Value + "] failed to enroll[" + newUserData.UserName + "]. A required field was empty or null | Execution Date: " + DateTime.Now.ToString();
                    ProcessStatus = 5; //An Input cannot be empty
                }
                else if (ex.Message.ToString() == "6")
                {
                    logDescription = "Error | Enroll User | [" + User.Claims.First().Value + "] failed to enroll[" + newUserData.UserName + "]. User with the same User already exists | Execution Date: " + DateTime.Now.ToString();
                    ProcessStatus = 6; //Existing UserName
                }
                else
                {
                    logDescription = "Error | Enroll User | [" + User.Claims.First().Value + "] failed to enroll[" + newUserData.UserName + "]. Unknown error has been encountered | Execution Date: " + DateTime.Now.ToString();
                    ProcessStatus = 0; //Unknown Error
                }
            }

            //Logs
            //_auditLogsRepository.SystemAuditLog("Controller", "EnrollmentController_EnrollNewUser", 0, logDescription, User.Claims.First().Value);

            return Json(new { ProcessStatus, ProcessMessage });
        }

        //DELETE USERS
        [Authorize]
        [Authorize(Roles = "SYSADMIN,APPADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser([FromBody] vm_UserDetails userDetails)
        {
            var returnText = "";
            string logDescription = "";
            try
            {
                var UserName = userDetails.UserName;
                bool isDeleted = _accountManagementRepository.DeleteUser(UserName);

                if (isDeleted)
                {
                    logDescription = "Success | Delete User | [" + User.Claims.First().Value + "] has deleted [" + userDetails.UserName + "] | Execution Date: " + DateTime.Now.ToString();
                    returnText = "User has been deleted";
                }
                else
                {
                    logDescription = "Error | Delete User | [" + User.Claims.First().Value + "] has failed to delete [" + userDetails.UserName + "] | Execution Date: " + DateTime.Now.ToString();
                    returnText = "Cannot delete this user";
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An error occurred: {Message}, Source: {Source}", ex.Message, ex.Source);
                logDescription = "Error | Delete User | [" + User.Claims.First().Value + "] has failed to delete [" + userDetails.UserName + "]. Unknown error encountered | Execution Date: " + DateTime.Now.ToString();
                returnText = ex.Message.ToString();
            }

            //Logs
            //_auditLogsRepository.SystemAuditLog("Controller", "EnrollmentController_DeleteUser", 0, logDescription, User.Claims.First().Value);

            return Json(returnText);
        }



        [Authorize]
        [Authorize(Roles = "SYSADMIN,APPADMIN,USER")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateUser([FromBody] vm_UserDetails userDetails)
        {
            var returnText = "";
            string logDescription = "";
            try
            {
                bool isUpdated = _accountManagementRepository.UpdateUser(userDetails);
                //var truee = isUpdated;
                if (isUpdated)
                {
                    returnText = "User has been updated";
                    logDescription = "Success | Update User | [" + User.Claims.First().Value + "] has updated [" + userDetails.UserName + "] | Execution Date: " + DateTime.Now.ToString();
                }
                else
                {
                    returnText = "Cannot update: the user's role already exists.";
                    logDescription = "Error | Update User | [" + User.Claims.First().Value + "] has failed to update [" + userDetails.UserName + "] | Execution Date: " + DateTime.Now.ToString();
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An error occurred: {Message}, Source: {Source}", ex.Message, ex.Source);

                returnText = ex.ToString();
                logDescription = "Error | Update User | [" + User.Claims.First().Value + "] has failed to update [" + userDetails.UserName + "]. Unknown error encountered | Execution Date: " + DateTime.Now.ToString();
            }

            //Logs
            //_auditLogsRepository.SystemAuditLog("Controller", "EnrollmentController_UpdateUser", 0, logDescription, User.Claims.First().Value);

            return Json(returnText);
        }


        [Authorize]
        [Authorize(Roles = "SYSADMIN,APPADMIN,USER")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetUserInfo()
        {
            var UserName = Request.Form["UserName"];
            var UserInfo = _accountManagementRepository.GetSystemUserInfo(UserName);
            if (UserInfo.Count() > 0)
            {
                return Json(UserInfo.First());
            }
            else
            {
                return Json("");
            }
        }






    }
}
