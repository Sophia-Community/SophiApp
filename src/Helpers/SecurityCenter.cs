// <copyright file="SecurityCenter.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Reads the antiviruses registered in Windows Security Center.
    /// </summary>
    public static class SecurityCenter
    {
        // Microsoft Defender Antivirus
        private const string DefenderGuid = "{D68DDC3A-831F-4fae-9E44-DA132C1ACF46}";

        // WSC_SECURITY_PROVIDER_ANTIVIRUS
        private const uint AntivirusProvider = 4;

        // WSC_SECURITY_PRODUCT_STATE_ON
        private const int ProductStateOn = 0;

        // The WSCProductList class of wscapi.dll
        private static readonly Guid ProductListClass = new("17072F7B-9ABE-4A74-A261-1EB76B55107A");

        /// <summary>
        /// IWscProduct: a product registered in Windows Security Center. The members go in the order of the interface.
        /// </summary>
        [ComImport]
        [Guid("8C38232E-3A45-4A27-92B0-1A16A975F669")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        private interface IWscProduct
        {
            /// <summary>
            /// Gets the name of the product.
            /// </summary>
            string ProductName
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            /// <summary>
            /// Gets WSC_SECURITY_PRODUCT_STATE: on, off, snoozed or expired.
            /// </summary>
            int ProductState { get; }

            /// <summary>
            /// Gets whether the signatures are up to date.
            /// </summary>
            int SignatureStatus { get; }

            /// <summary>
            /// Gets the path of the program that fixes the state of the product.
            /// </summary>
            string RemediationPath
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            /// <summary>
            /// Gets the time the state was reported.
            /// </summary>
            string ProductStateTimestamp
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }

            /// <summary>
            /// Gets the id of the product.
            /// </summary>
            string ProductGuid
            {
                [return: MarshalAs(UnmanagedType.BStr)]
                get;
            }
        }

        /// <summary>
        /// IWSCProductList: the products of one kind. The members go in the order of the interface.
        /// </summary>
        [ComImport]
        [Guid("722A338C-6E8E-4E72-AC27-1417FB0C81C2")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        private interface IWSCProductList
        {
            /// <summary>
            /// Reads the products of a kind.
            /// </summary>
            /// <param name="provider">WSC_SECURITY_PROVIDER.</param>
            void Initialize(uint provider);

            /// <summary>
            /// Gets the number of the products.
            /// </summary>
            /// <returns>The number of the products.</returns>
            int GetCount();

            /// <summary>
            /// Gets a product.
            /// </summary>
            /// <param name="index">The number of the product, from zero.</param>
            /// <returns>The product.</returns>
            IWscProduct GetItem(uint index);
        }

        /// <summary>
        /// Gets every antivirus that is not Microsoft Defender, with its name as Windows Security shows it.
        /// </summary>
        /// <returns>The names, and whether each antivirus is turned on.</returns>
        public static IReadOnlyList<(string Name, bool IsOn)> GetThirdPartyAntiviruses()
        {
            var antiviruses = new List<(string Name, bool IsOn)>();
            Exception? error = null;

            // The list of Security Center lives in a single-threaded apartment, and the checks run on the thread pool
            var thread = new Thread(() =>
            {
                try
                {
                    var list = (IWSCProductList)Activator.CreateInstance(Type.GetTypeFromCLSID(ProductListClass, throwOnError: true)!)!;

                    try
                    {
                        list.Initialize(AntivirusProvider);

                        for (var index = 0u; index < (uint)list.GetCount(); index++)
                        {
                            var product = list.GetItem(index);

                            try
                            {
                                if (!DefenderGuid.Equals(product.ProductGuid, StringComparison.OrdinalIgnoreCase) && product.ProductName is { Length: > 0 } name)
                                {
                                    antiviruses.Add((name.Trim(), product.ProductState == ProductStateOn));
                                }
                            }
                            finally
                            {
                                _ = Marshal.ReleaseComObject(product);
                            }
                        }
                    }
                    finally
                    {
                        _ = Marshal.ReleaseComObject(list);
                    }
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
            thread.Join();

            return error is null ? antiviruses : throw new InvalidOperationException("Windows Security Center did not give the list of antiviruses", error);
        }
    }
}
