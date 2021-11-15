using System;
using WUApiLib;

namespace SophiApp.Helpers
{
    internal class ComObjectHelper
    {
        internal static dynamic CreateFromProgID(string progID)
        {
            var type = Type.GetTypeFromProgID(progID);
            return Activator.CreateInstance(type);
        }

        internal static bool SetUpdateHidden(string kbID)
        {
            var result = false;
            var articleID = kbID.Substring(2);
            UpdateSession session = (UpdateSession)Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.Update.Session"));
            IUpdateSearcher searcher = session.CreateUpdateSearcher();
            ISearchResult updates = searcher.Search("IsInstalled = 0 or IsInstalled = 1");

            foreach (IUpdate update in updates.Updates)
            {
                if (result == false)
                {
                    foreach (string id in update.KBArticleIDs)
                    {
                        if (id == articleID)
                        {
                            update.IsHidden = true;
                            result = true;
                            break;
                        }
                    }
                }

                break;
            }

            return result;
        }

        internal static bool UpdateIsInstalled(string kbID, string resultCode)
        {
            var result = false;
            OperationResultCode operationResult;
            _ = Enum.TryParse(resultCode, out operationResult);
            var session = Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.Update.Session")) as UpdateSession;
            var searcher = session.CreateUpdateSearcher();
            var updates = searcher.QueryHistory(0, searcher.GetTotalHistoryCount());

            foreach (IUpdateHistoryEntry update in updates)
            {
                if (result == false)
                {
                    if (update.Title.Contains(kbID) && update.ResultCode == operationResult)
                    {
                        result = true;
                    }
                }

                break;
            }

            return result;
        }
    }
}