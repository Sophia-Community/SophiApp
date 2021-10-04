using Newtonsoft.Json;
using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace SophiApp.Conditions
{
    internal class NoNewVersion : ICondition
    {
        public bool Result { get; set; }
        public string Tag { get; set; } = Tags.ConditionNoNewVersion;

        public bool Invoke()
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(DataHelper.GitHubApiReleases);
                request.UserAgent = DataHelper.UserAgent;
                var response = request.GetResponse();
                DebugHelper.HasUpdateResponse();
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    var serverResponse = reader.ReadToEnd();
                    var release = JsonConvert.DeserializeObject<List<ReleaseDto>>(serverResponse).FirstOrDefault();
                    DebugHelper.HasUpdateRelease(release);
                    var isNewVersion = new Version(release.tag_name) > DataHelper.Version
                                                                     && release.prerelease.Invert()
                                                                     && release.draft.Invert();

                    if (isNewVersion)
                    {
                        DebugHelper.IsNewRelease();
                        ToastHelper.ShowUpdateToast(currentVersion: $"{DataHelper.Version}", newVersion: release.tag_name);
                    }

                    DebugHelper.UpdateNotnecessary();
                    return Result = isNewVersion.Invert();
                }
            }
            catch (WebException e)
            {
                DebugHelper.HasException("An error occurred while checking for an update", e);
                return Result = true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.Replace(":", null));
            }
        }
    }
}