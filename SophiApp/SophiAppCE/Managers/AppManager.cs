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

                         if (File.Exists(scriptPath) && CompareFileHash(scriptPath, jsonData.Sha256))
                             JsonDataList.Add(jsonData);
                     }
                 });
        }

        private bool CompareFileHash(string filePath, string fileHash)
        {
            using (SHA256 sha = SHA256.Create())
            {
                try
                {
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                    {
                        fileStream.Position = 0;                        
                        return BitConverter.ToString(sha.ComputeHash(fileStream)).Replace("-", "") == fileHash ? true : false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }            
        }
    }
}
