using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

[ToolboxBitmap(typeof(ToolStripTextBox))]
public class MyToolStripTextBox : ToolStripTextBox
{
    private const int EM_SETCUEBANNER = 0x1501;
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern Int32 SendMessage(IntPtr hWnd, int msg,
        int wParam, string lParam);
    public MyToolStripTextBox()
    {
        this.Control.HandleCreated += Control_HandleCreated;
    }
    private void Control_HandleCreated(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(cueBanner))
            UpdateCueBanner();
    }
    string cueBanner;
    public string CueBanner
    {
        get { return cueBanner; }
        set
        {
            cueBanner = value;
            UpdateCueBanner();
        }
    }
    private void UpdateCueBanner()
    {
        SendMessage(this.Control.Handle, EM_SETCUEBANNER, 0, cueBanner);
    }
}