using System.IO;
using System.Threading.Tasks;

namespace PotentHelper
{
    public class DbText : IDb
    {
        public bool Initial(string name, string defaultFolder = "DB")
        {

            textname = Path.Combine(defaultFolder, name);
            CreateFileIfNotExists(textname);
            return true;
        }
        string textname;

        public async Task<bool> AddAsync(string msg)
        {
            using StreamWriter file = new(textname, append: true);
            await file.WriteLineAsync(msg);
            RaiseNewDataEvent(msg);
            return true;
        }

        public bool Add(string msg)
        {
            using StreamWriter file = new(textname, append: true);
            file.WriteLine(msg);
            RaiseNewDataEvent(msg);
            return true;
        }

        static void CreateFileIfNotExists(string name)
        {
            if (!File.Exists(name))
            {
                File.CreateText(name).Dispose();
            }
            else if (name.Substring(0, 4) == "Test")
            {
                File.Delete(name);
                File.CreateText(name).Dispose();
            }
        }

        public async Task<string[]> GetAllAsync()
        {
            return await File.ReadAllLinesAsync(textname);
        }

        public string[] GetAll()
        {
            return File.ReadAllLines(textname);
        }

        public delegate void DbNewDataEventHandler(object sender, DbNewDataEventArgs e);

        public event DbNewDataEventHandler OnDbNewDataEvent;

        void RaiseNewDataEvent(string msg)
        {
            OnDbNewDataEvent?.Invoke(this, new DbNewDataEventArgs(msg));
        }

        public void ReplayAll()
        {
            foreach (var item in GetAll())
            {
                RaiseNewDataEvent(item);
            }
        }
    }

    public class DbNewDataEventArgs
    {
        public DbNewDataEventArgs(string text) { Text = text; }
        public string Text { get; } // readonly
    }


}
