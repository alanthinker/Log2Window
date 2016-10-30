using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Log2Window.UI
{
    //fix bug of treeview with checkbox.
    //http://stackoverflow.com/questions/17356976/treeview-with-checkboxes-not-processing-clicks-correctly
    class TreeViewWithoutDoubleClick : TreeView
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x203) // identified double click
            {
                var localPos = PointToClient(Cursor.Position);
                var hitTestInfo = HitTest(localPos);
                if (hitTestInfo.Location == TreeViewHitTestLocations.StateImage)
                    m.Result = IntPtr.Zero;
                else
                    base.WndProc(ref m);
            }
            else base.WndProc(ref m);
        }
    }
}

