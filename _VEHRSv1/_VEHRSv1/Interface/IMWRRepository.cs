using _VEHRSv1.Models;

namespace _VEHRSv1.Interface
{
    public interface IMwrRepository
    {

        Task<List<vm_Mwrlist>> GetMwrlist();
        Task<List<Mwrdate>> GetMwrDate(int MwrId);
        Task<List<Mwrdate>> getDateSelectedMwr(int MwrId);
       List<Mwractivity> mwrParticipantList();
        List<Mwrlist> Mwrlist();
        List<Mwrdate> MwrlistDate();
        
        Task<List<AppReference>> GetActivity();
        Task<List<Mwrdate>> GetActivityDate(int MwrId);

        Task<List<Mwractivity>> GetParticipant();
        bool AddMwrParticipant(Mwractivity NewMwrParticipant);
        bool AddMwrAct(Mwrlist NewMwr);

        bool AddMwrActDate(Mwrdate NewMwrDate);
        bool DeleteMwrAct(int MwrAct);
        bool DeleteMwrActDate(int MwrAct, DateTime Dmwrdate);
        bool UpdateMwrAct(string CUser,Mwrlist UpdateMwr);
        bool Save();
        List<vmMWRRecordPerEmployee> GetAll(string idNumber);
    }
}
