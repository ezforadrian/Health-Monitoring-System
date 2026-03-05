using _VEHRSv1.Models;

namespace _VEHRSv1.Interface
{
    public interface IAppReferenceRepository
    {
        Task<List<AppReference>> GetAllActiveRecords();
        Task<IEnumerable<vmAppReference>> GetAllUploadExcelRecords(int referenceGroup = 1); 
    }
}
