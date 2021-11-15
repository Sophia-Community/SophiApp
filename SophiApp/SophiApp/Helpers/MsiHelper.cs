using Microsoft.Deployment.WindowsInstaller;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.Helpers
{
    internal class MsiHelper
    {
        internal static IEnumerable<Dictionary<string, string>> GetProperties(string[] paths)
        {
            foreach (var path in paths)
            {
                yield return GetProperties(path);
            }
        }

        internal static Dictionary<string, string> GetProperties(string path)
        {
            var result = new Dictionary<string, string>();

            using (var database = new Database(path, DatabaseOpenMode.ReadOnly))
            {
                using (var view = database.OpenView(database.Tables["Property"].SqlSelectString))
                {
                    view.Execute();
                    var s = view.ToList();
                    foreach (var rec in view)
                        result.Add(rec.GetString("Property"), rec.GetString("Value"));
                }
            }

            result.Add("Path", path);
            return result;
        }
    }
}