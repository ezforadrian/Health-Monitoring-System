using _VEHRSv1.Models;
using System.Drawing;
using System.Reflection.PortableExecutable;

namespace _VEHRSv1.Interface
{
    public interface IPEMERepository
    {
        //Employee Peme status
        Task<List<vmPemeStatus>> GetAllActivePemeStatus();
        Task<List<vmMedicalEvaluator>> GetAllActiveMedicalEvaluator();
        bool CheckRecordExist(string lastname, string firstname, string middlename, DateOnly birthdate);
        bool CheckRecordExistUsingPemeId(int id);
        bool CheckPemeDetailsExistUsingPemeIdAndPemeExamDate(int pemeid, DateTime examdate);
        bool CheckPemeDetailsExistUsingPemeIdAndPemeExamDate(int pemeid, DateTime examdate, int pemedetailid);
        Task AddPemeheader(Pemeheader pemeheader);
        bool AddPemedetail(Pemedetail pemedetail);
        bool UpdatePemeheader(vmPemeEmployeeRecord pemeheader);
        bool UpdatePemedetail(Pemedetail pemedetail);
        string GetStatusDescriptionUsingStatusId(int statusId);
        vmPemeEmployeeRecord GetEmployeePemeDetails(int id);
        vmPemeEmployeeRecord GetEmployeePemeDetails(string idNumber);
        //int GetMaxStatusId();





        //search
        Task<PagedResult<vmSearchPeme>> SearchPEMEAsync(string query, int page, int pageSize);
    }
}
