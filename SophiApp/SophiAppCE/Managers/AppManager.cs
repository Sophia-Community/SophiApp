using SophiAppCE.Classes;
using SophiAppCE.General;
using SophiAppCE.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace SophiAppCE.Managers
{
    internal static class AppManager
    {
        internal static Point GetParentElementRelativePoints(FrameworkElement childrenElement) => childrenElement.TranslatePoint(new Point(0, 0), childrenElement.Parent as UIElement);

        internal static IEnumerable<JsonData> ParseJsonData()
        {
            return Regex.Matches(Encoding.UTF8.GetString(Properties.Resources.SettingsCE), @"\{(.*?)\},", RegexOptions.Compiled | RegexOptions.Singleline)
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
        }

        internal static bool FileExistsAndHashed(string filePath, string hashValue)
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

        internal static IEnumerable<T> CreateControlsByType<T>(IEnumerable<JsonData> controlsCollections, ControlType controlType)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            foreach (JsonData json in controlsCollections)
            {
                dynamic control = new object();

                switch (controlType)
                {
                    case ControlType.Switch:
                        control = new SwitchBarModel();
                        control.Id = json.Id;
                        control.Path = Path.Combine(appDirectory, json.Path);
                        control.LocalizedHeader = new Dictionary<UiLanguage, string> { { UiLanguage.EN, json.HeaderEn }, { UiLanguage.RU, json.HeaderRu } };
                        control.LocalizedDescription = new Dictionary<UiLanguage, string> { { UiLanguage.EN, json.DescriptionEn }, { UiLanguage.RU, json.DescriptionRu } };
                        control.Type = json.Type;
                        control.Sha256 = json.Sha256;
                        control.Tag = json.Tag;
                        break;
                }

                yield return control;
            }
        }
    }
}
