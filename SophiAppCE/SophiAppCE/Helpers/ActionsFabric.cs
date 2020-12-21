using SophiAppCE.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.Helpers
{
    internal static class ActionsFabric
    {
        internal static IApplicable GetActionByName(string fullyQualifiedName)
        {
            Type type = Type.GetType(fullyQualifiedName);
            return Activator.CreateInstance(type) as IApplicable;
        }

        internal static bool ExecuteState(string className, string methodName, string methodArg)
        {            
            Type type = Type.GetType($"SophiAppCE.Actions.{className}");
            MethodInfo methodInfo = type.GetMethod(methodName);
            return (bool)methodInfo.Invoke(type, null);
        }
    }
}
