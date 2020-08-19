using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Text.RegularExpressions;
using SophiAppCE.Models;
using System.Collections.ObjectModel;
using System.Linq;
using SophiAppCE.Classes;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;

namespace SophiAppCE.Managers
{
    internal class GuiManager
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
        internal static Point GetParentRelativePoint(FrameworkElement childrenElement, FrameworkElement parentElement)
        {
            return childrenElement.TranslatePoint(new Point(0, 0), parentElement);
        }
        
        internal enum AccentState
        {
            ACCENT_DISABLED = 1,
            ACCENT_ENABLE_GRADIENT = 0,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }        

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            // ...
            WCA_ACCENT_POLICY = 19
            // ...
        }

        internal static void SetWindowBlur()
        {
            WindowInteropHelper windowHelper = new WindowInteropHelper(Application.Current.MainWindow);

            AccentPolicy accent = new AccentPolicy
            {
                AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND
            };

            int accentStructSize = Marshal.SizeOf(accent);
            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);
            WindowCompositionAttributeData data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };
            SetWindowCompositionAttribute(windowHelper.Handle, ref data);
            Marshal.FreeHGlobal(accentPtr);
        }

        internal static ObservableCollection<SwitchBarModel> ParseJsonData()
        {
            ObservableCollection<SwitchBarModel> switchBarModelsCollection = new ObservableCollection<SwitchBarModel>();

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

            jsons.Where(j => FileExistsAndHashed(filePath: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, j.Path), hashValue: j.Sha256) == true)
                 .ToList()
                 .ForEach(j => switchBarModelsCollection.Add(GetControlByType(j)));

            return switchBarModelsCollection;
        }

        private static dynamic GetControlByType(JsonData jsonData)
        {
            dynamic control = new object();

            switch (jsonData.Type)
            {
                case "SwitchBar":
                    control = new SwitchBarModel();
                    break;
            }

            control.Id = jsonData.Id;
            control.Path = jsonData.Path;
            control.HeaderEn = jsonData.HeaderEn;
            control.HeaderRu = jsonData.HeaderRu;
            control.DescriptionEn = jsonData.DescriptionEn;
            control.DescriptionRu = jsonData.DescriptionRu;
            control.Type = jsonData.Type;
            control.Sha256 = jsonData.Sha256;
            control.Tag = jsonData.Tag;

            return control;
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
