using System.Threading.Tasks;
using static PotentHelper.DbText;

namespace PotentHelper
{
    public interface IDb
    {
        bool Initial(string name, string defaultFolder);
        Task<bool> AddAsync(string msg);
        void Add(string msg);
        Task<string[]> GetAllAsync();
        string[] GetAll();
        void ReplayAll();

        event DbNewDataEventHandler OnDbNewDataEvent;
    }
}