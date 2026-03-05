using _VEHRSv1.Models;

namespace _VEHRSv1.Interface
{
    public interface IAS400PlantillaRepository
    {
        Task<List<vmDepartment>> GetAllDepartments();
        Task<List<vmPosition>> GetAllPositions();
        Task<vmDepartment> GetDepartmentByCode(string code);
        Task<vmPosition> GetPositionByCode(string code);
        string GetDepartmentDescriptionByCode(string code);
        string GetPositionDescriptionByCode(string code);
        Task<List<vmAS400EmployeeList>> GetEmployeeListAS400();
        Task<List<vmBranch>> GetAllBranches();

        Task<(List<vmAS400EmployeeList> empList, int totalRecords)> GetEmployeeListAS400(int start, int length, string searchValue);
        vmEmployeeHealthInfo GetEmployeeInfoUsingIdNumber(string idnumber);

    }
}
