using Microsoft.AspNetCore.Mvc;
using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using Newtonsoft.Json;
using _VEHRSv1.Services;
using _VEHRSv1.Helper;
using System.Reflection.PortableExecutable;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _VEHRSv1.Controllers
{
    public class PEMEController : Controller
    {
        private readonly IAS400PlantillaRepository _repository;
        private readonly IPEMERepository _pEMERepository;
        private readonly EncryptionService _encryptionService;
        private readonly HswmsContext _hswmsContext;
        private const string _key = "thequickbrownfox";


        public PEMEController(IAS400PlantillaRepository repository, IPEMERepository pEMERepository, EncryptionService encryptionService, HswmsContext hswmsContext)
        {
            _repository = repository;
            _pEMERepository = pEMERepository;
            _encryptionService = encryptionService;
            _hswmsContext = hswmsContext;
        }

        public async Task<IActionResult> AddRecordPeme()
        {

            string viewName = "AddRecordPeme";
            try
            {
                var model = new vmSavePemeRecord(); // Create an instance
                return await PrepareViewBagAndReturn(viewName, model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePemeRecord(vmSavePemeRecord model)
        {
            TempData.Remove("SuccessMessage");
            string viewName = "AddRecordPeme";

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please correct field(s) with error(s).");
                return await PrepareViewBagAndReturn(viewName, model);
            }

            try
            {
                if (_pEMERepository.CheckRecordExist(model.LastName, model.FirstName, model.MiddleName, DateOnly.Parse(model.Birthdate.ToString())))
                {
                    ModelState.AddModelError("", "Record already exists.");
                    return await PrepareViewBagAndReturn(viewName, model);
                }


                using (var transaction = await _hswmsContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        DecryptModelData(model);

                        var pemeHeader = MapToPemeHeader(model);
                        await _pEMERepository.AddPemeheader(pemeHeader);

                        var pemeDetail = MapToPemeDetail(model, pemeHeader.Pemeid);
                        if (_pEMERepository.AddPemedetail(pemeDetail))
                        {
                            await transaction.CommitAsync();

                            // Set a success message in TempData
                            TempData["SuccessMessage"] = "Record saved successfully!";

                            // Redirect to the same view with a cleared model
                            return RedirectToAction("AddRecordPeme");

                            //return await PrepareViewBagAndReturn(new vmSavePemeRecord());
                            //return RedirectToAction("Success"); // Adjust this action as necessary
                        }
                        else
                        {
                            ModelState.AddModelError("", "An error occurred while saving the record. Please try again.");
                            await transaction.RollbackAsync();
                            return await PrepareViewBagAndReturn(viewName, model);
                        }



                    }
                    catch
                    {
                        ModelState.AddModelError("", "An error occurred while saving the record. Please try again.");
                        await transaction.RollbackAsync();
                        return await PrepareViewBagAndReturn(viewName, model);
                    }
                }
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while saving the record. Please try again.");
                return await PrepareViewBagAndReturn(viewName, model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateRecord(string id)
        {
            TempData.Remove("SuccessMessage");

            string viewName = "EmployeePemeRecord";

            int OutId;

            if (string.IsNullOrEmpty(id))
            {
                var error = GenericErrorList.GetError(ErrorType.ParameterNull);
                return View("_error", error);
            }

            try
            {
                id = _encryptionService.Decrypt(_key, id);
                if (!int.TryParse(id, out OutId))
                {
                    var error = GenericErrorList.GetError(ErrorType.ParameterNull);
                    return View("_error", error);
                }
            }
            catch (Exception ex)
            {
                var error = GenericErrorList.GetError(ErrorType.ParameterNull);
                return View("_error", error);
            }

            try
            {
                var record = _pEMERepository.GetEmployeePemeDetails(OutId);
                if (record == null)
                {
                    var error = GenericErrorList.GetError(ErrorType.ParameterNull);
                    return View("_error", error);
                }

                return await PrepareViewBagAndReturn(viewName, record);
                //return View("EmployeePemeRecord", record);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"Error loading record for update: {id}");
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePemeHeaderRecord(vmPemeEmployeeRecord model)
        {
            TempData.Remove("SuccessMessage");

            string viewName = "EmployeePemeRecord";
            try
            {
                DecryptModelPositionData(model);
                DecryptModelPemeStringData(model);
                DecryptModelStatusData(model);

                if (!ModelState.IsValid)
                {
                    var record = _pEMERepository.GetEmployeePemeDetails(model.Pemeid);
                    model.PemeDetails = record.PemeDetails;
                    EncryptConstPositionData(model);
                    EncryptConstStatusData(model);
                    // Return errors as a dictionary
                    var errors = ModelState.ToDictionary(
                        k => k.Key,
                        v => v.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
                    );
                    return Json(new { success = false, errors });
                }

                using (var transaction = await _hswmsContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var existingRecord = _pEMERepository.GetEmployeePemeDetails(model.Pemeid);
                        if (existingRecord == null)
                        {
                            return Json(new { success = false, message = "Record not found." });
                        }

                        existingRecord.LastName = model.LastName.Trim().ToUpper();
                        existingRecord.FirstName = model.FirstName.Trim().ToUpper();
                        existingRecord.MiddleName = model.MiddleName.Trim().ToUpper();
                        existingRecord.Birthdate = model.Birthdate;
                        existingRecord.Position = model.Position.Trim().ToUpper();
                        existingRecord.Status = model.Status;

                        var saveRecord = _pEMERepository.UpdatePemeheader(existingRecord);

                        if (saveRecord)
                        {
                            await transaction.CommitAsync();
                            //return Json(new { success = true, message = "Record updated successfully!" });
                            TempData["SuccessMessage"] = "Record updated successfully!";
                            return Json(new { success = true });
                        }
                        else
                        {
                            await transaction.RollbackAsync();
                            return Json(new { success = false, message = "An error occurred while saving the record. Please try again." });
                        }
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return Json(new { success = false, message = "An error occurred while saving the record. Please try again." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while saving the record. Please try again." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePemeDetail([FromBody] vmSavePemeDetail model)
        {

            TempData.Remove("SuccessMessage");

            var errors = new List<string>();

            if (!ModelState.IsValid)
            {
                var errorsModel = ModelState.ToDictionary(
                    k => k.Key,
                    v => v.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );

                foreach (var item in errorsModel)
                {
                    errors.Add(item.Value.FirstOrDefault());
                }

                //return Json(new { success = false });
            }

            int PemeId = 0;
            int MedicalEvaluatorId = 0;
            //int OutId;
            string decId;
            try
            {
                decId = _encryptionService.Decrypt(_key, model.PemeIdString);
                if (!int.TryParse(decId, out PemeId))
                {
                    errors.Add("Data manipulation detected. Please refresh the page or log in again.");
                    //return Json(new { success = false });
                }

            }
            catch (Exception)
            {
                errors.Add("Data manipulation detected. Please refresh the page or log in again.");
                //return Json(new { success = false });
            }

            try
            {
                decId = _encryptionService.DecryptConstant(_key, model.MedicalEvaluatorIdString);
                if (!int.TryParse(decId, out MedicalEvaluatorId))
                {
                    errors.Add("Data manipulation detected. Please refresh the page or log in again.");
                    //return Json(new { success = false });
                }
            }
            catch (Exception)
            {

                errors.Add("Data manipulation detected. Please refresh the page or log in again.");
                //return Json(new { success = false });
            }

            if (errors.Any())
            {
                return Json(new { success = false, errors });
            }
            else
            {
                try
                {
                    using (var transaction = _hswmsContext.Database.BeginTransaction())
                    {
                        try
                        {
                            model.MedicalEvaluatorIdString = MedicalEvaluatorId.ToString();
                            model.PemeDetailIdString = "0";
                            var pemeDetail = MapToPemeDetail(model, PemeId);

                            // Check if the pemeDetail already exists
                            if (!_pEMERepository.CheckPemeDetailsExistUsingPemeIdAndPemeExamDate(PemeId, model.ExamDate))
                            {
                                _pEMERepository.AddPemedetail(pemeDetail);
                                transaction.Commit();

                                // Set a success message in TempData
                                TempData["SuccessMessage"] = "Pre-employment detail saved successfully!";
                                return Json(new { success = true });
                            }
                            else
                            {
                                errors.Add("Record already exists for this pre-employment record.");
                                return Json(new { success = false, errors = errors });
                            }




                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            errors.Add("An error occurred while saving the record. Please try again.");
                            return Json(new { success = false });
                        }
                    }

                }
                catch (Exception ex)
                {
                    // Log the error (not shown here for brevity)
                    return Json(new { success = false, errors = new List<string> { "An error occurred while saving." } });
                }
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePemeDetail([FromBody] vmSavePemeDetail model)
        {

            TempData.Remove("SuccessMessage");

            var errors = new List<string>();

            if (!ModelState.IsValid)
            {
                var errorsModel = ModelState.ToDictionary(
                    k => k.Key,
                    v => v.Value.Errors.Select(e => e.ErrorMessage).ToList()
                );

                foreach (var item in errorsModel)
                {
                    errors.Add(item.Value.FirstOrDefault());
                }

                //return Json(new { success = false });
            }

            int PemeId = 0;
            int MedicalEvaluatorId = 0;
            int PemeDetailId = 0;
            //int OutId;
            string decId;
            try
            {
                decId = _encryptionService.Decrypt(_key, model.PemeIdString);
                if (!int.TryParse(decId, out PemeId))
                {
                    errors.Add("Data manipulation detected. Please refresh the page or log in again.");
                    //return Json(new { success = false });
                }

            }
            catch (Exception)
            {
                errors.Add("Data manipulation detected. Please refresh the page or log in again.");
                //return Json(new { success = false });
            }

            try
            {
                decId = _encryptionService.Decrypt(_key, model.PemeDetailIdString);
                if (!int.TryParse(decId, out PemeDetailId))
                {
                    errors.Add("Data manipulation detected. Please refresh the page or log in again.");
                    //return Json(new { success = false });
                }

            }
            catch (Exception)
            {
                errors.Add("Data manipulation detected. Please refresh the page or log in again.");
                //return Json(new { success = false });
            }

            try
            {
                decId = _encryptionService.DecryptConstant(_key, model.MedicalEvaluatorIdString);
                if (!int.TryParse(decId, out MedicalEvaluatorId))
                {
                    errors.Add("Data manipulation detected. Please refresh the page or log in again.");
                    //return Json(new { success = false });
                }
            }
            catch (Exception)
            {

                errors.Add("Data manipulation detected. Please refresh the page or log in again.");
                //return Json(new { success = false });
            }

            if (errors.Any())
            {
                return Json(new { success = false, errors });
            }
            else
            {
                try
                {
                    using (var transaction = _hswmsContext.Database.BeginTransaction())
                    {
                        try
                        {
                            model.MedicalEvaluatorIdString = MedicalEvaluatorId.ToString();
                            model.PemeDetailIdString = PemeDetailId.ToString();
                            var pemeDetail = MapToPemeDetail(model, PemeId);

                            // Check if the pemeDetail already exists
                            if (!_pEMERepository.CheckPemeDetailsExistUsingPemeIdAndPemeExamDate(PemeId, model.ExamDate, int.Parse(model.PemeDetailIdString)))
                            {
                                _pEMERepository.UpdatePemedetail(pemeDetail);
                                transaction.Commit();

                                // Set a success message in TempData
                                TempData["SuccessMessage"] = "Pre-employment detail saved successfully!";
                                return Json(new { success = true });
                            }
                            else
                            {
                                errors.Add("Record already exists for this pre-employment record.");
                                return Json(new { success = false, errors = errors });
                            }




                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            errors.Add("An error occurred while saving the record. Please try again.");
                            return Json(new { success = false });
                        }
                    }

                }
                catch (Exception ex)
                {
                    // Log the error (not shown here for brevity)
                    return Json(new { success = false, errors = new List<string> { "An error occurred while saving." } });
                }
            }


        }

        public IActionResult GetPemeDetails(string pemeId)
        {
            try
            {
                pemeId = _encryptionService.Decrypt(_key, pemeId);
                if (!int.TryParse(pemeId, out var pemeIdInt))
                {
                    return Json(new { success = false });
                }

                var details = _pEMERepository.GetEmployeePemeDetails(pemeIdInt);
                if (details == null)
                {
                    return Json(new { success = false, message = "Details not found" });
                }

                return Json(new { success = true, details });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, message = ex.Message });
            }
        }



        private async Task<IActionResult> PrepareViewBagAndReturn(string viewName, object model)
        {
            //var departments = await _repository.GetAllDepartments();
            var positions = await _repository.GetAllPositions();
            var pemeStatus = await _pEMERepository.GetAllActivePemeStatus();
            var medEval = await _pEMERepository.GetAllActiveMedicalEvaluator();

            //ViewBag.Department = JsonConvert.SerializeObject(departments.Select(d => new { id = d.DepartmentCodeEncrypt, text = d.DepartmentDescription }));
            ViewBag.Position = JsonConvert.SerializeObject(positions.Select(p => new { id = p.PositionCodeEncrypt, text = p.PositionDescription }));
            ViewBag.Status = JsonConvert.SerializeObject(pemeStatus.Select(p => new { id = p.IdEncrypt, text = p.Description }));
            ViewBag.MedicalEvaluator = JsonConvert.SerializeObject(medEval.Select(p => new { id = p.IdEncrypt, text = p.ReferenceName }));

            return View(viewName, model);
        }

        private void DecryptModelData(vmSavePemeRecord model)
        {
            model.Position = _encryptionService.DecryptConstant(_key, model.Position);
            model.Status = _encryptionService.DecryptConstant(_key, model.Status);
            model.MedicalEvaluator = _encryptionService.DecryptConstant(_key, model.MedicalEvaluator);
        }

        private Pemeheader MapToPemeHeader(vmSavePemeRecord model)
        {
            return new Pemeheader
            {
                LastName = model.LastName.Trim().ToUpper(),
                FirstName = model.FirstName.Trim().ToUpper(),
                MiddleName = model.MiddleName.Trim().ToUpper(),
                Birthdate = model.Birthdate,
                PositionRef = model.Position.Trim().ToUpper(),
                StatusId = 1, // Adjust based on your status mapping
                CreatedBy = "dbo", // User.Identity.Name
                CreatedDateTime = DateTime.UtcNow
            };
        }

        private Pemeheader MapToPemeHeaderWithId(vmPemeEmployeeRecord model)
        {
            int statusId;
            if (!int.TryParse(model.Status.ToString(), out statusId))
            {
                throw new FormatException("Status is not in a valid integer format.");
            }

            return new Pemeheader
            {
                Pemeid = model.Pemeid,
                LastName = model.LastName.Trim().ToUpper(),
                FirstName = model.FirstName.Trim().ToUpper(),
                MiddleName = model.MiddleName.Trim().ToUpper(),
                Birthdate = model.Birthdate,
                PositionRef = model.Position.Trim().ToUpper(),
                StatusId = statusId,
                CreatedBy = "dbo", // User.Identity.Name
                CreatedDateTime = DateTime.UtcNow
            };
        }




        private Pemedetail MapToPemeDetail(vmSavePemeRecord model, int pemeId)
        {
            return new Pemedetail
            {
                Pemeid = pemeId,
                ExamDate = (DateTime)model.MedicalExamDate,
                MedicalEvaluatorId = int.Parse(model.MedicalEvaluator),
                Remarks = model.Remarks,
                CreatedBy = "dbo", // User.Identity.Name
                CreatedDateTime = DateTime.UtcNow
            };
        }

        private Pemedetail MapToPemeDetail(vmSavePemeDetail model, int pemeId)
        {
            return new Pemedetail
            {
                Pemeid = pemeId,
                DetailId = int.Parse(model.PemeDetailIdString),
                ExamDate = (DateTime)model.ExamDate,
                MedicalEvaluatorId = int.Parse(model.MedicalEvaluatorIdString),
                Remarks = model.Remarks,
                CreatedBy = "dbo", // User.Identity.Name
                CreatedDateTime = DateTime.UtcNow
            };
        }



        private void EncryptConstPositionData(vmPemeEmployeeRecord model)
        {
            model.Position = _encryptionService.EncryptConstant(_key, model.Position);
        }

        private void EncryptConstStatusData(vmPemeEmployeeRecord model)
        {
            model.Status = _encryptionService.EncryptConstant(_key, model.Status);
        }

        private void DecryptModelPemeStringData(vmPemeEmployeeRecord model)
        {
            try
            {
                model.Pemeid = int.Parse(_encryptionService.Decrypt(_key, model.PemeidString));
            }
            catch (FormatException)
            {
                throw new FormatException("Pemeid is not in a valid integer format.");
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                throw new Exception("An error occurred while decrypting Pemeid.", ex);
            }
        }

        private void DecryptModelPositionData(vmPemeEmployeeRecord model)
        {
            //model.Position = _encryptionService.DecryptConstant(_key, model.Position);

            try
            {
                model.Position = _encryptionService.DecryptConstant(_key, model.Position);
            }
            catch (FormatException)
            {
                throw new FormatException("Position is not in a valid integer format.");
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                throw new Exception("An error occurred while decrypting Position.", ex);
            }
        }

        private void DecryptModelStatusData(vmPemeEmployeeRecord model)
        {
            //model.Status = _encryptionService.DecryptConstant(_key, model.Status);

            try
            {
                model.Status = _encryptionService.DecryptConstant(_key, model.Status);
            }
            catch (FormatException)
            {
                throw new FormatException("Status is not in a valid integer format.");
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                throw new Exception("An error occurred while decrypting Status.", ex);
            }
        }



    }
}
