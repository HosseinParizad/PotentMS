using System.Threading.Tasks;
using static PotentHelper.DbText;

namespace PotentHelper
{
    public interface IDb
    {
        Task<bool> AddAsync(string msg);
        bool Add(string msg);
        bool Initial(string name);
        Task<string[]> GetAllAsync();
        string[] GetAll();
        void ReplayAll();

        event DbNewDataEventHandler OnDbNewDataEvent;
    }
}