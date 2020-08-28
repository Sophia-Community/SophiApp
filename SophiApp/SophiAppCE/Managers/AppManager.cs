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
        internal static Point GetParentElementRelativePoint(FrameworkElement childrenElement) => childrenElement.TranslatePoint(new Point(0, 0), childrenElement.Parent as UIElement);

        internal static IEnumerable<JsonData> ParseJsonData()
        {
            return Regex.Matches(Encoding.UTF8.GetString(Properties.Resources.SettingsCE), @"\{(.*?)\}", RegexOptions.Compiled | RegexOptions.Singleline)
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

        internal static IEnumerable<T> GetControlByType<T>(IEnumerable<JsonData> controlsCollections, ControlType controlType)
        {
            foreach (JsonData json in controlsCollections)
            {
                dynamic control = new object();

                switch (controlType.ToString())
                {
                    case nameof(ControlType.Switch):
                        control = new SwitchBarModel();
                        break;
                }

                control.Id = json.Id;
                control.Path = json.Path;
                control.HeaderEn = json.HeaderEn;
                control.HeaderRu = json.HeaderRu;
                control.DescriptionEn = json.DescriptionEn;
                control.DescriptionRu = json.DescriptionRu;
                control.Type = json.Type;
                control.Sha256 = json.Sha256;
                control.Tag = json.Tag;
                yield return control;
            }

        }
    }
}
