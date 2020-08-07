using SophiAppCE.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Diagnostics;

namespace SophiAppCE.Managers
{
    internal class AppManager
    {
        internal string BaseDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;
        private List<JsonData> JsonDataList { get; set; } = new List<JsonData>();
        public AppManager()
        {
            InitializeJsonDataList();
        }

        internal List<JsonData> GetJsonDataByTag(string tag) => JsonDataList.Where(j => j.Tag == tag).ToList(); //TODO: Задать вопрос на тостере!

        private void InitializeJsonDataList()
        {
            Regex.Matches(Encoding.UTF8.GetString(Properties.Resources.SettingsCE), @"\{(.*?)\}", RegexOptions.Compiled | RegexOptions.Singleline)
                 .Cast<Match>()
                 .ToList()
                 .ForEach(j =>
                 {
                     using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(j.Value)))
                     {
                         DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(JsonData));
                         JsonData jsonData = (JsonData)jsonSerializer.ReadObject(memoryStream);
                         string scriptPath = Path.Combine(BaseDirectory, jsonData.Path);

                         if (FileExistsAndHashed(filePath: Path.Combine(BaseDirectory, jsonData.Path), hashValue: jsonData.Sha256))
                             JsonDataList.Add(jsonData);
                     }
                 });
        }     

        private bool FileExistsAndHashed(string filePath, string hashValue)
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
