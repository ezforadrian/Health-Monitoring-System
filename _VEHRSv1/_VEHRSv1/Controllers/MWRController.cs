using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using _VEHRSv1.Repository;
using _VEHRSv1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace _VEHRSv1.Controllers
{
    public class MWRController : Controller
    {
        private readonly IMwrRepository _mwrRepository;
        private readonly EncryptionService _encryptionService;
        private const string _key = "thequickbrownfox";
        private readonly IAS400PlantillaRepository _aS400PlantillaRepository;
        private readonly IAccountManagementRepository _accountManagementRepository;
        public MWRController(IMwrRepository imwrRepository, EncryptionService encryptionService)
        {
            _mwrRepository = imwrRepository;
            _encryptionService = encryptionService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ViewTesting()
        {
            string[] items = { "Item1", "Item2", "Item3" };
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Items", typeof(string));

            foreach (var item in items)
            {
                dataTable.Rows.Add(item);
            }

            return View(dataTable);
        }
        //public ActionResult Index()
        //{
        //    string[] items = { "Item1", "Item2", "Item3" };
        //    return View(items);
        //}

        public IActionResult AddRecordMwr()
        {
            return View();
        }

    

        public async Task<IActionResult> GetActivity()
        {
            try
            {
                var appReference = new List<AppReference>();
                appReference = await _mwrRepository.GetActivity();
                var result = appReference;

             
                return Json(new { data = result });
                //return Json(edata);
            }
            catch (Exception ex)
            {
                return Json("No Data");
            }
        }

        public async Task<IActionResult> GetActivityDate(int mwrlistId)
        {
            try
            {
                
               
                var dateAct = new List<Mwrdate>();
                dateAct = await _mwrRepository.GetActivityDate(mwrlistId);
                var result = dateAct;


                return Json(new { data = result });
                //return Json(edata);
            }
            catch (Exception ex)
            {
                return Json("No Data");
            }
        }


        public async Task<IActionResult> getDateSelectedMwr(int mwrlistId)
        {
            try
            {
                var datelist = new List<Mwrdate>();
                datelist = await _mwrRepository.getDateSelectedMwr(mwrlistId);
                var result = datelist;


                return Json(new { data = result });
                //return Json(edata);
            }
            catch (Exception ex)
            {
                return Json("No schedule created for HSWD Activity");
            }
        }

        public async Task<IActionResult> GetMwrlistDate(int mwrlistId, int start, int length, string? search)
        {
            try
            {
                var mwrList = new List<Mwrdate>();
                mwrList = await _mwrRepository.GetMwrDate(mwrlistId);
                var edata = mwrList;

                var result = edata
                        .Skip(start)
                        .Take(length);
                return Json(new { data = result });
                //return Json(edata);
            }
            catch (Exception ex)
            {
                return Json("No Data");
            }
        }




        public async Task<IActionResult> GetMwrlist(int start, int length, string? search)
        {
            try
            {
                var mwrList = new List<vm_Mwrlist>();
                mwrList = await _mwrRepository.GetMwrlist();
                var edata = mwrList;

                var result = edata
                        .Skip(start)
                        .Take(length);
                return Json(new { data = result });
                //return Json(edata);
            }
            catch (Exception ex)
            {
                return Json("No Data");
            }
        }

        [HttpGet]
        public IActionResult GetMwrList_DataTable(string idEncMwr)
        {
            try
            {
                var idDec = _encryptionService.Decrypt(_key, idEncMwr);


                var details = _mwrRepository.GetAll(idDec);
                if (details == null)
                {
                    return Json(new { success = false, message = "Details not found" });
                }

                return Json(new { success = true, details });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMwr([FromBody] Mwrlist addActivity)
        {
            var processStatus = 0;
            string processMessage = "";

            try
            {
                // Validate the input
                if (addActivity == null || string.IsNullOrEmpty(addActivity.ActivityName))
                {
                    processMessage = "Activity name is required.";
                    return Json(new { processStatus, processMessage });
                }

                if (addActivity.ActivityType == "0")
                {
                    processMessage = "Please select an activity type.";
                    return Json(new { processStatus, processMessage });
                }

                // Check if the activity already exists
                bool isSystemUser = _mwrRepository.Mwrlist()
                    .Any(a => string.Equals(a.ActivityName, addActivity.ActivityName, StringComparison.OrdinalIgnoreCase));

                if (isSystemUser)
                {
                    processStatus = 2;
                    processMessage = "MWR Activity already exists.";
                }
                else
                {
                    string CurrentUser = User.Claims.First().Value;
                    // Create a new activity
                    var newActivity = new Mwrlist
                    {
                        ActivityName = addActivity.ActivityName.ToUpper(),
                        ActivityType = addActivity.ActivityType,
                        CreatedDateTime = DateTime.Now,
                        CreatedBy = CurrentUser,
                    };

                    // Add the new activity
                    _mwrRepository.AddMwrAct(newActivity);
                    processStatus = 1;
                    processMessage = "MWR Activity added successfully.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.Error.WriteLine(ex);
                processMessage = "An error occurred while processing your request.";
                return Json(new { processStatus, processMessage });
            }

            return Json(new { processStatus, processMessage });
        }

        //add mwrdateactivity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMwrDate([FromBody] Mwrdate addActivityDate)
        {
            var processStatus = 0;
            string processMessage = "";

            try
            {
                // Validate the input
                if (addActivityDate == null || string.IsNullOrEmpty(addActivityDate.MwractDate.ToString()))
                {
                    processMessage = "Activity Date is required.";
                    return Json(new { processStatus, processMessage });
                }

                if (addActivityDate.MwrlistId == 0)
                {
                    processMessage = "Please select an activity name.";
                    return Json(new { processStatus, processMessage });
                }

                // Check if the activity already exists
                bool isSystemDate = _mwrRepository.MwrlistDate()
                    .Where(a => a.MwrlistId == addActivityDate.MwrlistId)
                    .Any(a => a.MwractDate == addActivityDate.MwractDate);
               

                if (isSystemDate)
                {
                    processStatus = 2;
                    processMessage = "MWR Activity Date already exists.";
                }
                else
                {
                    string CurrentUser = User.Claims.First().Value;
                    // Create a new activity
                    var newActivityDate = new Mwrdate
                    {
                        MwrlistId = addActivityDate.MwrlistId,
                        MwractDate = addActivityDate.MwractDate,
                        CreatedDate = DateTime.Now,
                        CreatedBy = CurrentUser,
                    };

                    // Add the new activity
                    _mwrRepository.AddMwrActDate(newActivityDate);
                    processStatus = 1;
                    processMessage = "MWR Activity added successfully.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.Error.WriteLine(ex);
                processMessage = "An error occurred while processing your request.";
                return Json(new { processStatus, processMessage });
            }

            return Json(new { processStatus, processMessage });
        }





        public IActionResult DeleteMwr([FromBody] Mwrlist mwrId)
        {

            var returnText = "";
            string logDescription = "";

            try
            {
                var id = mwrId.MwrlistId;
                bool isDeleted = _mwrRepository.DeleteMwrAct(id);
                if (isDeleted)
                {
                    logDescription = "Success | Delete HSWD Activity";
                    returnText = "HSWD Activity has been deleted";
                }
                else
                {
                    logDescription = "Error | Delete HSWD Activity";
                    returnText = "Cannot delete this HSWD Activity";
                }
            }
            catch (Exception ex)
            {
                returnText = ex.Message.ToString();
            }

            return Json(returnText);
        }


        public IActionResult DeleteMwrDate([FromBody] Mwrdate mwrdated)
        {

            var returnText = "";
            string logDescription = "";

            try
            {
                int id = mwrdated.MwrlistId; 
                DateTime ActDate = mwrdated.MwractDate;
                bool isDeleted = _mwrRepository.DeleteMwrActDate(id, ActDate);
                if (isDeleted)
                {
                    logDescription = "Success | Delete HSWD Activity Date";
                    returnText = "HSWD Activity Date has been deleted";
                }
                else
                {
                    logDescription = "Error | Delete HSWD Activity Date";
                    returnText = "The HSWD activity date cannot be deleted as there are already participants registered.";
                }
            }
            catch (Exception ex)
            {
                returnText = ex.Message.ToString();
            }

            return Json(returnText);
        }

        public IActionResult UpdateMwr([FromBody] Mwrlist mwrDetails)
        {
            var returnText = "";
            string CurrentUser = User.Claims.First().Value;
            try
            {
                bool isUpdated = _mwrRepository.UpdateMwrAct(CurrentUser, mwrDetails);
                //var truee = isUpdated;
                if (isUpdated)
                {
                    returnText = "HSWD Activity has been updated";
                   
                }
                else
                {
                    returnText = "Cannot update: the HSWD Activity already exists.";
                    
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An error occurred: {Message}, Source: {Source}", ex.Message, ex.Source);

                returnText = ex.ToString();
               
            }

            //Logs
            //_auditLogsRepository.SystemAuditLog("Controller", "EnrollmentController_UpdateUser", 0, logDescription, User.Claims.First().Value);

            return Json(returnText);
        }



        public IActionResult AddMwrParticipant([FromBody] Mwractivity addActivityPaticipant)
        {
            var processStatus = 0;
            string processMessage = "";

            try
            {
                // Validate the input
                if (addActivityPaticipant == null || string.IsNullOrEmpty(addActivityPaticipant.Idnumber))
                {
                    processMessage = "Participant Id Number is required.";
                    return Json(new { processStatus, processMessage });
                }

                if (addActivityPaticipant.ActDate == null)
                {
                    processMessage = "Please select an activity date.";
                    return Json(new { processStatus, processMessage });
                }

                // Check if the activity already exists
                bool isSystemDate = _mwrRepository.mwrParticipantList()
                    .Where(x => x.Idnumber == addActivityPaticipant.Idnumber 
                           && x.MwrlistId == addActivityPaticipant.MwrlistId
                           && x.ActDate == addActivityPaticipant.ActDate)
                    .Count() > 0;

                //var PAGCOR_EmployeeInfo = _accountManagementRepository.GetInternalUserInfo(addActivityPaticipant?.Idnumber);
                if (isSystemDate)
                {
                    processStatus = 2;
                    processMessage = "Participant already exists.";
                }
                
                else
                {
                    string CurrentUser = User.Claims.First().Value;
                    // Create a new activity
                    var newActivityDate = new Mwractivity
                    {
                        MwrlistId = addActivityPaticipant.MwrlistId,
                        ActDate = addActivityPaticipant.ActDate,
                        Idnumber = addActivityPaticipant.Idnumber,
                        CreatedBy = CurrentUser,
                        CreatedDateTime = DateTime.Now,
                    };

                    // Add the new activity
                    _mwrRepository.AddMwrParticipant(newActivityDate);
                    processStatus = 1;
                    processMessage = "Participant added successfully.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.Error.WriteLine(ex);
                processMessage = "An error occurred while processing your request.";
                return Json(new { processStatus, processMessage });
            }

            return Json(new { processStatus, processMessage });
        }


        //LastLine

    }
}
