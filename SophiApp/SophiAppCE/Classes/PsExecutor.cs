using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SophiAppCE.Models;

namespace SophiAppCE.Classes
{
    internal static class PsExecutor
    {
        internal static Task Execute(List<string> filesPath)
        {
            return Task.Run(() =>
            {
                filesPath.ForEach(s =>
                {                    
                    Process.Start(ProcessCreator(s)).WaitForExit();
                    Thread.Sleep(5000);
                });
            });
        }

        internal static ProcessStartInfo ProcessCreator(string filePath)
        {
            return new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoLogo -NoProfile -NonInteractive -WindowStyle Hidden -ExecutionPolicy Bypass -File \"{filePath}\" -On",
                LoadUserProfile = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                ErrorDialog = false
            };            
        }
    }
}
