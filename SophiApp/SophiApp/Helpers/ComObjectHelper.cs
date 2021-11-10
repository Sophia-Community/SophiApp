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

        internal static bool UpdateInstalled(string kbID)
        {
            var result = false;
            var articleID = kbID.Substring(2);
            UpdateSession session = (UpdateSession)Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.Update.Session"));
            IUpdateSearcher searcher = session.CreateUpdateSearcher();
            ISearchResult updates = searcher.Search("IsHidden = 0");

            foreach (IUpdate update in updates.Updates)
            {
                if (result == false)
                {
                    foreach (string id in update.KBArticleIDs)
                    {
                        if (id == articleID)
                        {
                            result = true;
                            break;
                        }
                    }
                }

                break;
            }

            return result;
        }
    }
}