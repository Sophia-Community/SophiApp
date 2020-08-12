using SophiAppCE.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SophiAppCE.Managers
{
    internal static class AppManager
    {
        internal static List<JsonData> GetJsonDataByTag(string tag)
        {
            IEnumerable<JsonData> jsons = Regex.Matches(Encoding.UTF8.GetString(Properties.Resources.SettingsCE), @"\{(.*?)\}", RegexOptions.Compiled | RegexOptions.Singleline)
                 .Cast<Match>()
                 .Select(m =>
                 {
                     JsonData json = new JsonData();

                     using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(m.Value)))
                     {
                         DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(JsonData));
                         json = (JsonData)jsonSerializer.ReadObject(memoryStream);                         
                     }

                     return json;
                 });

            return jsons.Where(j => FileExistsAndHashed(filePath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, j.Path),
                                                        hashValue: j.Sha256) == true && j.Tag == tag).ToList();
        }

        private static bool FileExistsAndHashed(string filePath, string hashValue)
        {
            bool result = default(bool);

            if (File.Exists(filePath))
            {
                using (SHA256 sha = SHA256.Create())
                {
                    try
                    {
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                        {
                            fileStream.Position = 0;
                            result = BitConverter.ToString(sha.ComputeHash(fileStream)).Replace("-", "") == hashValue ? true : false;
                        }
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            return result;
        }
    }
}
