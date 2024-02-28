using System.Windows.Forms;
using System.Drawing;

using Log2Window.Log;
using System;

namespace Log2Window.UI
{
    internal class TreeViewLoggerView : ILoggerView
    {
        public TreeViewLoggerView(TreeView treeView)
        {
            _treeView = treeView;
            _isRoot = true;
        }

        private TreeViewLoggerView(TreeView treeView, TreeNode node)
        {
            _treeView = treeView;
            _node = node;
            _isRoot = false;
        }


        #region ILoggerView Members

        /// <summary>
        /// Clears this view and all child views.
        /// </summary>
        public void Clear()
        {
            if (_isRoot)
            {
                try
                {
                    _treeView.BeginUpdate();
                    _treeView.Nodes.Clear();
                }
                finally
                {
                    _treeView.EndUpdate();
                }
            }
            else
            {
                try
                {
                    _node.TreeView.BeginUpdate();
                    _node.Nodes.Clear();
                }
                finally
                {
                    _node.TreeView.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Adds the new logger view as a child of the current view and returns the new view.
        /// </summary>
        /// <param name="text">The text to initialize the view with.</param>
        /// <param name="logger">The logger that this instance is a view of.</param>
        /// <returns></returns>
        public ILoggerView AddNew(string text, LoggerItem logger)
        {
            ILoggerView temp = null;
            if (this._treeView.InvokeRequired)
            {
                _treeView.Invoke(new Action(delegate ()
                {
                    temp = AddNewInner(text, logger);
                }));
            }
            else
            {
                temp = AddNewInner(text, logger);
            }
            return temp;
        } 

        //按 text 排序添加.
        TreeNode AddNode(TreeNodeCollection nodes, string key, string text)
        {
            int firstBiggerThanNewIndex = -1;

            for (int i = 0; i < nodes.Count; i++)
            {
                //忽略区域和字母大小写比较大小
                if (string.CompareOrdinal(nodes[i].Text, text) > 0)
                {
                    firstBiggerThanNewIndex = i;
                    break;
                }
            }

            var newNode = new TreeNode(text);
            newNode.Name = key;
            if (firstBiggerThanNewIndex == -1)
            {
                nodes.Add(newNode);
            }
            else
            {
                nodes.Insert(firstBiggerThanNewIndex, newNode);
            }

            return newNode;

        }

        public ILoggerView AddNewInner(string text, LoggerItem logger)
        {
            // Creating a new node.
            TreeNode node = _isRoot ? AddNode(_treeView.Nodes, text, text) : AddNode(_node.Nodes, text, text);
            node.Tag = logger;
            node.Checked = true;
            if (_node != null && _node.Level == 0)
            {
                _node.ExpandAll();
            }
            //  node.EnsureVisible();
            //if (_node != null)
            //{
            //    _node.Collapse(false);
            //}

            return new TreeViewLoggerView(_treeView, node);
        }

        public void Remove(string text)
        {
            if (_isRoot)
            {
                _treeView.Nodes.RemoveByKey(text);
            }
            else
            {
                _node.Nodes.RemoveByKey(text);
            }
        }

        public void Sync()
        {
            var node = _node;
            while(node != null)
            {
                node.Expand();
                node = node.Parent;
            }
        }

        /// <summary>
        /// Gets or sets the text of the view. The text is what is shown to the user.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return (_isRoot ? "(root)" : _node.Text);
            }
            set
            {
                if (!_isRoot)
                {
                    _node.Text = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ILoggerView"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled
        {
            get
            {
                return (_isRoot ? true : _node.Checked);
            }
            set
            {
                if (!_isRoot)
                {
                    if (_treeView.InvokeRequired)
                    {
                        _treeView.Invoke(new Action(delegate ()
                        {
                            _node.Checked = value;
                        }));
                    }
                    else
                    {
                        _node.Checked = value;
                    }

                }
            }
        }

        #endregion


        #region Private Members

        private TreeView _treeView;
        private TreeNode _node;
        private bool _isRoot = false;

        #endregion


    }
}
