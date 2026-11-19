// <copyright file="AdmxPolicies.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// The registry values that the policy templates (ADMX files) describe.
    /// </summary>
    public sealed class AdmxPolicies
    {
        // The value names of a list policy are set by the user, so any value in its key belongs to the policy
        private const string AnyName = "*";

        // Scope -> registry key -> value names. Key and value names are not case-sensitive in the registry
        private readonly Dictionary<LGPOScope, Dictionary<string, HashSet<string>>> values = new()
        {
            [LGPOScope.Computer] = new(StringComparer.OrdinalIgnoreCase),
            [LGPOScope.User] = new(StringComparer.OrdinalIgnoreCase),
        };

        private AdmxPolicies()
        {
        }

        /// <summary>
        /// Reads all policy templates of a folder.
        /// </summary>
        /// <param name="folder">The folder with ADMX files.</param>
        public static AdmxPolicies Read(string folder)
        {
            var policies = new AdmxPolicies();

            foreach (var file in Directory.EnumerateFiles(folder, "*.admx"))
            {
                try
                {
                    policies.Add(XDocument.Load(file));
                }
                catch (Exception ex) when (ex is XmlException or IOException or UnauthorizedAccessException)
                {
                    // A template that cannot be read describes nothing
                }
            }

            return policies;
        }

        /// <summary>
        /// Gets a value indicating whether any template describes a value in the registry key.
        /// </summary>
        /// <param name="scope">Policy scope.</param>
        /// <param name="key">Registry key path without the root key.</param>
        public bool HasKey(LGPOScope scope, string key) => values[scope].ContainsKey(key);

        /// <summary>
        /// Gets a value indicating whether the registry value belongs to a policy.
        /// </summary>
        /// <param name="scope">Policy scope.</param>
        /// <param name="key">Registry key path without the root key.</param>
        /// <param name="name">Registry value name.</param>
        public bool Contains(LGPOScope scope, string key, string name)
            => values[scope].TryGetValue(key, out var names) && (names.Contains(name) || names.Contains(AnyName));

        // The templates of different years have different XML namespaces, so the elements are found by their local names
        private static IEnumerable<XElement> Children(IEnumerable<XElement> parents, params string[] names)
            => parents.Elements().Where(element => names.Contains(element.Name.LocalName));

        private static LGPOScope[] GetScopes(string? policyClass) => policyClass switch
        {
            "Machine" => [LGPOScope.Computer],
            "User" => [LGPOScope.User],
            "Both" => [LGPOScope.Computer, LGPOScope.User],
            _ => [],
        };

        private void Add(XDocument template)
        {
            if (template.Root is null)
            {
                return;
            }

            foreach (var policy in Children(Children(new[] { template.Root }, "policies"), "policy"))
            {
                var policyKey = (string?)policy.Attribute("key");
                var found = new List<(string? Key, string Name)>();

                // The value of the policy itself
                if ((string?)policy.Attribute("valueName") is { Length: > 0 } policyValue)
                {
                    found.Add((policyKey, policyValue));
                }

                var elements = Children(new[] { policy }, "elements").ToList();

                // An element may have a key of its own
                foreach (var element in Children(elements, "decimal", "text", "multiText", "enum", "boolean"))
                {
                    if ((string?)element.Attribute("valueName") is { Length: > 0 } elementValue)
                    {
                        found.Add(((string?)element.Attribute("key") ?? policyKey, elementValue));
                    }
                }

                foreach (var list in Children(elements, "list"))
                {
                    found.Add(((string?)list.Attribute("key") ?? policyKey, AnyName));
                }

                // The items of the enabled and disabled lists and of the enumeration values. They always have keys of their own
                var items = Children(Children(new[] { policy }, "enabledList", "disabledList"), "item")
                    .Concat(Children(Children(Children(Children(elements, "enum"), "item"), "valueList"), "item"));

                foreach (var item in items)
                {
                    if ((string?)item.Attribute("valueName") is { Length: > 0 } itemValue)
                    {
                        found.Add(((string?)item.Attribute("key"), itemValue));
                    }
                }

                foreach (var scope in GetScopes((string?)policy.Attribute("class")))
                {
                    foreach (var (key, name) in found)
                    {
                        if (string.IsNullOrEmpty(key))
                        {
                            continue;
                        }

                        if (!values[scope].TryGetValue(key, out var names))
                        {
                            names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            values[scope][key] = names;
                        }

                        names.Add(name);
                    }
                }
            }
        }
    }
}
