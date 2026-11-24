// <copyright file="CursorImage.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Microsoft.UI.Xaml.Media.Imaging;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.WindowsRuntime;

    /// <summary>
    /// Makes a picture of a cursor file (.cur or .ani).
    /// </summary>
    public static class CursorImage
    {
        private const uint ImageCursor = 2;
        private const uint LoadFromFile = 0x0010;
        private const uint DrawNormal = 0x0003;
        private const int BytesPerPixel = 4;
        private const int AlphaOffset = 3;

        /// <summary>
        /// Draws a cursor and cuts the empty margins off: a cursor takes only a part of its square.
        /// </summary>
        /// <param name="file">The path of the cursor file.</param>
        /// <param name="size">The side of the square the cursor is drawn in, in pixels. Windows takes the closest size the file has.</param>
        /// <returns>The picture, or null if the file cannot be drawn.</returns>
        public static WriteableBitmap? Load(string file, int size)
        {
            var cursor = LoadImage(0, file, ImageCursor, size, size, LoadFromFile);

            if (cursor == 0)
            {
                return null;
            }

            try
            {
                // An old cursor has a mask instead of transparency, so the transparency is found by drawing on black and on white
                var pixels = Draw(cursor, size, byte.MinValue);
                var onWhite = Draw(cursor, size, byte.MaxValue);
                var (left, top, right, bottom) = (size, size, -1, -1);

                for (var index = 0; index < size * size; index++)
                {
                    var pixel = index * BytesPerPixel;
                    var difference = Enumerable.Range(pixel, AlphaOffset).Max(channel => onWhite[channel] - pixels[channel]);
                    var alpha = (byte)Math.Clamp(byte.MaxValue - difference, byte.MinValue, byte.MaxValue);
                    pixels[pixel + AlphaOffset] = alpha;

                    if (alpha > 0)
                    {
                        var (x, y) = (index % size, index / size);
                        (left, top, right, bottom) = (Math.Min(left, x), Math.Min(top, y), Math.Max(right, x), Math.Max(bottom, y));
                    }
                }

                if (right < left)
                {
                    return null;
                }

                // What is drawn on black is already the color multiplied by the transparency, as the picture needs
                var width = right - left + 1;
                var picture = new WriteableBitmap(width, bottom - top + 1);
                using var stream = picture.PixelBuffer.AsStream();

                for (var y = top; y <= bottom; y++)
                {
                    stream.Write(pixels, ((y * size) + left) * BytesPerPixel, width * BytesPerPixel);
                }

                return picture;
            }
            finally
            {
                _ = DestroyCursor(cursor);
            }
        }

        private static byte[] Draw(nint cursor, int size, byte background)
        {
            var pixels = new byte[size * size * BytesPerPixel];
            Array.Fill(pixels, background);

            // A negative height: the rows go from the top
            var header = new BitmapHeader { Size = (uint)Marshal.SizeOf<BitmapHeader>(), Width = size, Height = -size, Planes = 1, BitCount = BytesPerPixel * 8 };
            var context = CreateCompatibleDC(0);
            var bitmap = CreateDIBSection(context, ref header, 0, out var bits, 0, 0);

            if (bitmap != 0)
            {
                var previous = SelectObject(context, bitmap);
                Marshal.Copy(pixels, 0, bits, pixels.Length);
                _ = DrawIconEx(context, 0, 0, cursor, size, size, 0, 0, DrawNormal);
                _ = GdiFlush();
                Marshal.Copy(bits, pixels, 0, pixels.Length);
                _ = SelectObject(context, previous);
                _ = DeleteObject(bitmap);
            }

            _ = DeleteDC(context);
            return pixels;
        }

        [DllImport("user32.dll", EntryPoint = "LoadImageW", CharSet = CharSet.Unicode)]
        private static extern nint LoadImage(nint instance, string name, uint type, int width, int height, uint load);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DrawIconEx(nint context, int left, int top, nint icon, int width, int height, uint step, nint brush, uint flags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyCursor(nint cursor);

        [DllImport("gdi32.dll")]
        private static extern nint CreateCompatibleDC(nint context);

        [DllImport("gdi32.dll")]
        private static extern nint CreateDIBSection(nint context, ref BitmapHeader header, uint usage, out nint bits, nint section, uint offset);

        [DllImport("gdi32.dll")]
        private static extern nint SelectObject(nint context, nint item);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(nint item);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteDC(nint context);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GdiFlush();

        // BITMAPINFOHEADER
        [StructLayout(LayoutKind.Sequential)]
        private struct BitmapHeader
        {
            public uint Size;
            public int Width;
            public int Height;
            public ushort Planes;
            public ushort BitCount;
            public uint Compression;
            public uint SizeImage;
            public int HorizontalResolution;
            public int VerticalResolution;
            public uint ColorsUsed;
            public uint ColorsImportant;
        }
    }
}
