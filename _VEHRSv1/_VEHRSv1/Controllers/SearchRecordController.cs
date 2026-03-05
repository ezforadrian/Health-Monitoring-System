using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using _VEHRSv1.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;

namespace _VEHRSv1.Controllers
{
    public class SearchRecordController : Controller
    {
        private readonly IPEMERepository _pEMERepository;
        private readonly IAS400PlantillaRepository _aS400PlantillaRepository;

        public SearchRecordController(IPEMERepository pEMERepository, IAS400PlantillaRepository aS400PlantillaRepository)
        {
            _pEMERepository = pEMERepository;
            _aS400PlantillaRepository = aS400PlantillaRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PemeSearchRecord(string searchQuery = "", int start = 0, int length = 10)
        {
            try
            {
                int page = (start / length) + 1;
                var result = await _pEMERepository.SearchPEMEAsync(searchQuery, page, length);

                return Json(new
                {
                    draw = Request.Query["draw"].FirstOrDefault(),
                    recordsTotal = result.TotalRecords,
                    recordsFiltered = result.TotalRecords,
                    data = result.Results.Select((record, index) => new
                    {
                        RowNumber = start + index + 1,
                        record.Idnumber,
                        FullName = $"{record.FirstName} {record.MiddleName} {record.LastName}",
                        Birthdate = record.Birthdate?.ToString("MM/dd/yyyy"),
                        record.PositionDescription,
                        record.StatusDescription,
                        Actions = $"<button class='btn btn-primary update-btn btn-sm' data-id='{record.PemeidString}'>Update</button>"
                    }).ToList()  // Add .ToList() to avoid deferred execution issues
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // This action is used for the initial view rendering
        public IActionResult SearchPemeRecord()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeListAS400([FromQuery] int start = 0, [FromQuery] int length = 10, [FromQuery] string searchValue = "")
        {
            var (empList, totalRecords) = await _aS400PlantillaRepository.GetEmployeeListAS400(start, length, searchValue);

            var pagedEmpList = empList.Select(record => new
            {
                idNumber = record.IdNumber,
                fullName = $"{record.FirstName} {record.MiddleName} {record.LastName}",
                datehired = record.DateHired.ToString("MM/dd/yyyy"),
                position = record.Position,
                birthDate = record.BirthDate.ToString("MM/dd/yyyy"),
                actions = $"<button class='btn btn-primary view-btn btn-sm' data-id='{record.IdEnc}'>View</button>"
            }).ToList();

            var result = new
            {
                draw = Request.Query["draw"],
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords, // Adjust if needed
                data = pagedEmpList
            };

            return Json(result);
        }


        public IActionResult EmployeeList()
        {
            return View("AS400EmployeeList");
        }

        





    }
}
