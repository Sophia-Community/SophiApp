using SophiApp;
using SophiAppCE.Classes;
using SophiAppCE.General;
using SophiAppCE.Models;
using SophiAppCE.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SophiAppCE.Managers
{
    internal class AppManager
    {
        private Dictionary<string, ResourceDictionary> localizedDictionarys = new Dictionary<string, ResourceDictionary>()
        {
            { nameof(UiLanguage.EN), new ResourceDictionary() { Source = new Uri("pack://application:,,,/Localization/EN.xaml", UriKind.Absolute)} },
            { nameof(UiLanguage.RU), new ResourceDictionary() { Source = new Uri("pack://application:,,,/Localization/RU.xaml", UriKind.Absolute)} }
        };

        private string uiCulture;

        private ResourceDictionary UiCulture => localizedDictionarys[Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToUpper()];    

        public AppManager()
        {
            SetUiLanguage();
        }

        private void SetUiLanguage()
        {
            uiCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToUpper() == nameof(UiLanguage.EN) ? nameof(UiLanguage.EN) : nameof(UiLanguage.RU);
            Application.Current.MainWindow.Resources.MergedDictionaries.Add(UiCulture);
        }

        internal static Point GetParentElementRelativePoint(FrameworkElement childrenElement) => childrenElement.TranslatePoint(new Point(0, 0), childrenElement.Parent as UIElement);

        internal IEnumerable<JsonData> ParseJsonData()
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

        internal bool FileExistsAndHashed(string filePath, string hashValue)
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

        internal IEnumerable<T> CreateControlsByType<T>(IEnumerable<JsonData> controlsCollections, ControlType controlType)
        {
            foreach (JsonData json in controlsCollections)
            {
                dynamic control = new object();

                switch (controlType.ToString())
                {
                    case nameof(ControlType.Switch):
                        string resourceHeaderID = $"Header_{json.Id}";
                        string resourceDescriptionID = $"Description_{json.Id}";

                        control = new SwitchBarModel();
                        control.Id = json.Id;
                        control.Path = json.Path;
                        control.HeaderEn = json.HeaderEn;
                        control.HeaderRu = json.HeaderRu;
                        control.DescriptionEn = json.DescriptionEn;
                        control.DescriptionRu = json.DescriptionRu;
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
