using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Log2Window.UI
{
    /// <summary>
    /// Provides Font Awesome 6 Free Solid glyphs for toolbar buttons,
    /// rendered as high-quality bitmaps at any target size (vector-perfect, DPI-aware).
    /// </summary>
    public static class IconProvider
    {
        /// <summary>
        /// Maps ToolStripItem.Name to its Font Awesome 6 Free Solid Unicode glyph character.
        /// </summary>
        private static readonly Dictionary<string, string> GlyphMap = new Dictionary<string, string>
        {
            // ── Main Toolbar (mainToolStrip) ──
            { "zoomOutLogListBtn",           "\uF010" },   // magnifying-glass-minus
            { "zoomInLogListBtn",            "\uF00E" },   // magnifying-glass-plus
            { "goToFirstLogBtn",             "\uF049" },   // backward (fast backward)
            { "pauseRefreshNewMessagesBtn",  "\uF04C" },   // pause
            { "goToLastLogBtn",              "\uF04E" },   // forward (fast forward)
            { "clearBtn",                    "\uF2ED" },   // trash-can
            { "toolStripLabel3",             "\uF1DE" },   // sliders (filter)
            { "lblSearch",                   "\uF002" },   // magnifying-glass
            { "settingsBtn",                 "\uF013" },   // gear
            { "receiversBtn",                "\uF0E0" },   // envelope (receive messages)
            { "ddbEventLog",                 "\uF1DA" },   // clock-rotate-left (history)
            { "ddbOpenLogFileBtn",           "\uF07C" },   // folder-open
            { "ddbExportBtn",                "\uF56D" },   // file-export
            { "aboutBtn",                    "\uF05A" },   // circle-info
            { "pinOnTopBtn",                 "\uF276" },   // thumbtack

            // ── Loggers Toolbar (loggersToolStrip) ──
            { "clearLoggersBtn",             "\uF1F8" },   // trash
            { "collapseAllBtn",              "\uF066" },   // compress / collapse

            // ── Log Detail Toolbar (logDetailToolStrip) ──
            { "zoomOutLogDetailsBtn",        "\uF010" },   // magnifying-glass-minus
            { "zoomInLogDetailsBtn",         "\uF00E" },   // magnifying-glass-plus
            { "copyLogDetailBtn",            "\uF0C5" },   // copy

            // ── Source Code Toolbar (toolStrip1) ──
            { "btnOpenFileInVS",             "\uF35D" },   // up-right-from-square
        };

        /// <summary>
        /// Maps ToolStripItem.Name to its icon color. Each action gets a semantic color
        /// that looks natural on a light toolbar background.
        /// </summary>
        private static readonly Dictionary<string, Color> ColorMap = new Dictionary<string, Color>
        {
            // ── Main Toolbar ──
            { "zoomOutLogListBtn",           Color.FromArgb(0, 150, 200) },    // 青蓝
            { "zoomInLogListBtn",            Color.FromArgb(0, 150, 200) },    // 青蓝
            { "goToFirstLogBtn",             Color.FromArgb(60, 120, 210) },   // medium blue
            { "pauseRefreshNewMessagesBtn",  Color.FromArgb(220, 140, 0) },    // amber
            { "goToLastLogBtn",              Color.FromArgb(60, 120, 210) },   // medium blue
            { "clearBtn",                    Color.FromArgb(210, 60, 60) },    // red
            { "toolStripLabel3",             Color.FromArgb(0, 160, 150) },    // teal
            { "lblSearch",                   Color.FromArgb(0, 150, 200) },    // 青蓝
            { "settingsBtn",                 Color.FromArgb(100, 100, 100) },  // neutral gray
            { "receiversBtn",                Color.FromArgb(220, 100, 20) },   // orange
            { "ddbEventLog",                 Color.FromArgb(160, 120, 60) },   // brown
            { "ddbOpenLogFileBtn",           Color.FromArgb(200, 160, 40) },   // gold
            { "ddbExportBtn",                Color.FromArgb(60, 160, 80) },    // green
            { "aboutBtn",                    Color.FromArgb(50, 120, 210) },   // info blue
            { "pinOnTopBtn",                 Color.FromArgb(180, 50, 50) },    // dark red

            // ── Loggers Toolbar ──
            { "clearLoggersBtn",             Color.FromArgb(210, 60, 60) },    // red
            { "collapseAllBtn",              Color.FromArgb(120, 120, 120) },  // gray

            // ── Log Detail Toolbar ──
            { "zoomOutLogDetailsBtn",        Color.FromArgb(0, 150, 200) },    // 青蓝
            { "zoomInLogDetailsBtn",         Color.FromArgb(0, 150, 200) },    // 青蓝
            { "copyLogDetailBtn",            Color.FromArgb(0, 150, 140) },    // teal

            // ── Source Code Toolbar ──
            { "btnOpenFileInVS",             Color.FromArgb(130, 80, 180) },   // purple
        };

        private static readonly PrivateFontCollection _fonts = new PrivateFontCollection();
        private static readonly FontFamily IconFontFamily;

        /// <summary>
        /// Static constructor: loads the Font Awesome 6 Free Solid TTF from embedded resource.
        /// </summary>
        static IconProvider()
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "Log2Window.res.fa-solid-900.ttf";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new InvalidOperationException(
                        $"Embedded resource '{resourceName}' not found. " +
                        "Ensure fa-solid-900.ttf is added as an EmbeddedResource in the .csproj.");

                byte[] fontBytes = new byte[stream.Length];
                stream.Read(fontBytes, 0, fontBytes.Length);

                IntPtr fontPtr = Marshal.AllocCoTaskMem(fontBytes.Length);
                try
                {
                    Marshal.Copy(fontBytes, 0, fontPtr, fontBytes.Length);
                    _fonts.AddMemoryFont(fontPtr, fontBytes.Length);
                }
                finally
                {
                    Marshal.FreeCoTaskMem(fontPtr);
                }
            }

            // Find the Font Awesome family in the collection (by name search for robustness)
            FontFamily found = null;
            foreach (var family in _fonts.Families)
            {
                if (family.Name.IndexOf("Font Awesome", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    found = family;
                    break;
                }
            }
            IconFontFamily = found ?? _fonts.Families[_fonts.Families.Length - 1];
        }

        /// <summary>
        /// Returns the glyph character for the given ToolStripItem, or null if no mapping exists.
        /// </summary>
        public static bool TryGetGlyph(ToolStripItem item, out string glyph)
        {
            return GlyphMap.TryGetValue(item.Name, out glyph);
        }

        /// <summary>
        /// Returns the semantic color for the given ToolStripItem, or <see cref="SystemColors.ControlText"/> as fallback.
        /// </summary>
        public static Color GetGlyphColor(ToolStripItem item)
        {
            Color color;
            if (ColorMap.TryGetValue(item.Name, out color))
                return color;
            return SystemColors.ControlText;
        }

        /// <summary>
        /// Renders a Font Awesome glyph at the requested <paramref name="size"/>.
        /// The bitmap is created with generous padding so the glyph — including any
        /// font-internal leading overflow — is fully contained. The ToolStrip's
        /// <c>SizeToFit</c> image scaling then cleanly downscales it to
        /// <c>ImageScalingSize</c> for display, preserving all content.
        /// </summary>
        public static Bitmap RenderGlyph(string glyph, int size, Color color)
        {
            // Font size matches the visual fullness the user confirmed as correct
            // (equivalent to the original Segoe MDL2 Assets rendering).
            float fontSize = size * 1.15f;

            // Modest padding prevents edge clipping without wasting pixels.
            // The ToolStrip downscales the bitmap to ImageScalingSize, so large
            // padding reduces effective glyph size — keep it small.
            int padding = Math.Max(2, (int)Math.Round(size / 8f));
            int bmpSize = (int)Math.Round(fontSize) + padding * 2;

            var bmp = new Bitmap(bmpSize, bmpSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                using (var font = new Font(IconFontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel))
                using (var brush = new SolidBrush(color))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                    };
                    g.DrawString(glyph, font, brush, new RectangleF(0, 0, bmpSize, bmpSize), sf);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Replaces all mapped bitmap images on the given <paramref name="toolStrip"/>
        /// with high-quality font glyphs rendered at <paramref name="targetSize"/> pixels.
        /// </summary>
        public static void ApplyGlyphIcons(ToolStrip toolStrip, int targetSize)
        {
            foreach (ToolStripItem item in toolStrip.Items)
            {
                if (item is ToolStripSeparator)
                    continue;

                string glyph;
                if (!TryGetGlyph(item, out glyph))
                    continue;

                var bmp = RenderGlyph(glyph, targetSize, GetGlyphColor(item));

                if (item is ToolStripButton btn)
                {
                    btn.Image = bmp;
                    btn.ImageTransparentColor = Color.Empty;
                }
                else if (item is ToolStripDropDownButton ddb)
                {
                    ddb.Image = bmp;
                    ddb.ImageTransparentColor = Color.Empty;
                }
                else if (item is ToolStripLabel lbl)
                {
                    lbl.Image = bmp;
                    lbl.ImageTransparentColor = Color.Empty;
                }
            }
        }
    }
}
