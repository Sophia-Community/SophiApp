using Newtonsoft.Json;
using SophiApp.Commons;
using SophiApp.Dto;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System;
using System.IO;
using System.Net;

namespace SophiApp.Conditions
{
    internal class NewVersionCondition : IStartupCondition
    {
        public bool HasProblem { get; set; } = default;
        public ConditionsTag Tag { get; set; } = ConditionsTag.NewVersion;

        public bool Invoke()
        {
            DebugHelper.IsOnline();

            try
            {
                if (HttpHelper.IsOnline)
                {
                    HttpWebRequest request = WebRequest.CreateHttp(AppHelper.SophiAppVersionsJson);
                    request.UserAgent = AppHelper.UserAgent;
                    var response = request.GetResponse();

                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        var serverResponse = reader.ReadToEnd();
                        var release = JsonConvert.DeserializeObject<ReleaseDto>(serverResponse);
                        DebugHelper.HasUpdateRelease(release);
                        var releasedVersion = new Version(AppHelper.IsRelease ? release.SophiApp_release : release.SophiApp_pre_release);

                        if (releasedVersion > AppHelper.Version)
                        {
                            DebugHelper.IsNewRelease();
                            ToastHelper.ShowUpdateToast(currentVersion: $"{AppHelper.Version}", newVersion: $"{releasedVersion}");
                        }
                        else
                        {
                            DebugHelper.UpdateNotNecessary();
                        }

                        return HasProblem;
                    }
                }

                return HasProblem;
            }
            catch (WebException e)
            {
                DebugHelper.HasException("An error occurred while checking for an update", e);
                return HasProblem;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.Replace(":", null));
            }
        }
    }
}