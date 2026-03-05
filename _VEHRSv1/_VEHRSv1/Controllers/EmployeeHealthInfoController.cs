using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using _VEHRSv1.Repository;
using _VEHRSv1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace _VEHRSv1.Controllers
{
    public class EmployeeHealthInfoController : Controller
    {
        private readonly EncryptionService _encryptionService;
        private readonly IAS400PlantillaRepository _aS400PlantillaRepository;
        private readonly IECURepository _eCURepository;
        private readonly HswmsContext _hswmsContext;
        private readonly IPEMERepository _pEMERepository;
        private const string _key = "thequickbrownfox";

        public EmployeeHealthInfoController(EncryptionService encryptionService, 
            IAS400PlantillaRepository aS400PlantillaRepository, 
            IPEMERepository pEMERepository, IECURepository eCURepository, HswmsContext hswmsContext)
        {
            _encryptionService = encryptionService;
            _aS400PlantillaRepository = aS400PlantillaRepository;
            _eCURepository = eCURepository;
            _hswmsContext = hswmsContext;
            _pEMERepository = pEMERepository;
        }

        public IActionResult Index()
        {

            return View();
        }

        public async Task<IActionResult> AEMView(string id)
        {
            try
            {
                // Decrypt Id Number
                var idnumberDec = _encryptionService.Decrypt(_key, id);

                // Check id number if exists in AS400 Plantilla Employee List
                var empRecord = _aS400PlantillaRepository.GetEmployeeInfoUsingIdNumber(idnumberDec);

                if (empRecord == null)
                {
                    // Redirect to the data table with message Invalid Id Number
                    TempData["ErrorMessage"] = "Invalid Parameter";
                    return RedirectToAction("EmployeeList", "SearchRecord");
                }


                var aew = new vmEmployeeHealthRecord
                {
                    vmEmployeeHealthInfo = empRecord,
                    EmpIdNumberEnc = id,
                    VmEcuHeaders = _eCURepository.GetEcuByEmployeeIdNumber(empRecord.EmployeeDetailAS400.IdNumber),
                    VmTestDetails = _eCURepository.GetEcuActiveTest(),



                };
                //var departments = await _repository.GetAllDepartments();

                var branch = await _aS400PlantillaRepository.GetAllBranches();
                ViewBag.Branch = JsonConvert.SerializeObject(branch.Select(p => new { id = p.BranchCodeEncrypt, text = p.BranchDescription }));

                return View(aew);
            }
            catch (Exception ex)
            {

                // Redirect to the data table with message Invalid Id Number
                TempData["ErrorMessage"] = "Invalid Parameter";
                return RedirectToAction("EmployeeList", "SearchRecord");
            }





        }

        


        //private string DecryptModelValueString(string modelValue)
        //{
        //    try
        //    {
        //        // Decrypt the value
        //        var decValue = _encryptionService.DecryptConstant(_key, modelValue);

        //        // Check if the decrypted value is null
        //        if (decValue == null)
        //        {
        //            throw new Exception("Decryption failed: Decrypted value is null.");
        //        }

        //        return decValue;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception as needed
        //        throw new Exception("An error occurred while decrypting the status.", ex);
        //    }
        //}



        //public async Task<IActionResult> GetView(string viewName, string idNumberEnc, string searchQuery = "", int page = 1, int pageSize = 10)
        //{
        //    if (string.IsNullOrEmpty(viewName) || string.IsNullOrEmpty(idNumberEnc))
        //    {
        //        return BadRequest("View name and ID number are required.");
        //    }

        //    try
        //    {
        //        var idNumberDec = _encryptionService.Decrypt(_key, idNumberEnc);
        //        var empRecord = _aS400PlantillaRepository.GetEmployeeInfoUsingIdNumber(idNumberDec);
        //        if (empRecord == null)
        //        {
        //            return NotFound("Employee record not found.");
        //        }

        //        var vmEcus = new List<vmEmployeeHealthRecord>
        //        {
        //            new vmEmployeeHealthRecord
        //            {
        //                EmpIdNumberEnc = idNumberEnc,
        //                VmEcuHeaders = _eCURepository.GetEcuByEmployeeIdNumber(idNumberDec),
        //                VmTestDetails = _eCURepository.GetEcuActiveTest()
        //            }
        //        };


        //        //switch (viewName)
        //        //{
        //        //    case "nav-peme-tab":
        //        //        var pemeResult = await _pEMERepository.SearchPEMEAsync(searchQuery, page, pageSize);
        //        //        ViewData["searchQuery"] = searchQuery;
        //        //        return PartialView("_PemeRecord", pemeResult);
        //        //    case "nav-ame-tab":
        //        //        return PartialView("_AmeRecord");
        //        //    case "nav-ecu-tab":
        //        //        var vmEcus = new List<vmEcu>
        //        //        {
        //        //            new vmEcu
        //        //            {
        //        //                EmpIdNumberEnc = idNumberEnc,
        //        //                VmEcuHeaders = _eCURepository.GetEcuByEmployeeIdNumber(idNumberDec),
        //        //                VmTestDetails = _eCURepository.GetEcuActiveTest()
        //        //            }
        //        //        };



        //        //        var branch = _aS400PlantillaRepository.GetAllBranches();
        //        //        ViewBag.Branch = JsonConvert.SerializeObject(branch);

        //        //        return PartialView("_EcuRecord", vmEcus);
        //        //    case "nav-mwr-tab":
        //        //        return PartialView("_MwrRecord");
        //        //    default:
        //        //        return BadRequest("View not found.");
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.ToString());
        //    }
        //}



    }
}
