using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DirectoryBrowser
{
    public partial class Form1 : Form
    {
        private ImageList imageList1;
        private ContextMenuStrip folderContextMenu;
        private ContextMenuStrip fileContextMenu;
        private bool isRightClickOperation = false;
        String[] browserFileTypes = new String[] { ".jpg", ".jpeg", ".png", ".gif", ".svg", ".bmp", ".html", ".htm", ".txt", ".md", ".ini", ".sql", ".json", ".js" };
        bool processEvents = false;
        Timer timer = new Timer();
        OpenFileDialog openFileDialog1 = null;
        FolderBrowserDialog folderBrowserDialog1 = null;
        string initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        
        public Form1()
        {
            InitializeComponent();
            imageList1 = new ImageList();
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageSize = new Size(16, 16);
            this.treeView1.ImageList = imageList1;
            
            // Initialize context menus
            InitializeContextMenus();
            
            timer.Tick += Timer_Tick;
            this.treeView1.Visible = false;
            this.treeView1.Enabled = false;
            string htmlPath = AppContext.BaseDirectory;
            htmlPath = Path.Combine(htmlPath, "index.html");
            this.webView21.Source = new Uri(htmlPath);
            //var task = waitForWebBrowser();
            //task.Wait();
            ChangeDirectory(initialDirectory, true);

            Startup();
            timer.Interval = 1000;
            timer.Start();
        }

        private void InitializeContextMenus()
        {
            // Create folder context menu
            folderContextMenu = new ContextMenuStrip();
            var copyFolderPathItem = new ToolStripMenuItem("Copy Folder Path");
            copyFolderPathItem.Click += CopyFolderPath_Click;
            folderContextMenu.Items.Add(copyFolderPathItem);

            // Create file context menu
            fileContextMenu = new ContextMenuStrip();
            var copyFilePathItem = new ToolStripMenuItem("Copy File Path");
            copyFilePathItem.Click += CopyFilePath_Click;
            fileContextMenu.Items.Add(copyFilePathItem);

            // Add mouse events to TreeView
            this.treeView1.MouseDown += TreeView1_MouseDown;
            this.treeView1.MouseUp += TreeView1_MouseUp;
        }

        private void TreeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                isRightClickOperation = true;
                
                // Get the node at the clicked location
                TreeNode clickedNode = treeView1.GetNodeAt(e.X, e.Y);
                if (clickedNode != null && !String.IsNullOrEmpty(clickedNode.Tag as String))
                {
                    // Select the node
                    treeView1.SelectedNode = clickedNode;
                    
                    string path = clickedNode.Tag as String;
                    
                    // Determine if it's a file or folder and show appropriate context menu
                    if (Directory.Exists(path))
                    {
                        folderContextMenu.Show(treeView1, e.Location);
                    }
                    else if (File.Exists(path))
                    {
                        fileContextMenu.Show(treeView1, e.Location);
                    }
                }
            }
        }

        private void TreeView1_MouseUp(object sender, MouseEventArgs e)
        {
            // Reset the flag after a short delay to allow context menu to show
            if (e.Button == MouseButtons.Right)
            {
                Timer resetTimer = new Timer();
                resetTimer.Interval = 100;
                resetTimer.Tick += (s, args) =>
                {
                    isRightClickOperation = false;
                    resetTimer.Stop();
                    resetTimer.Dispose();
                };
                resetTimer.Start();
            }
        }

        private void CopyFolderPath_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && !String.IsNullOrEmpty(treeView1.SelectedNode.Tag as String))
            {
                string folderPath = treeView1.SelectedNode.Tag as String;
                Clipboard.SetText(folderPath);
                this.toolStripStatusLabel1.Text = $"Copied folder path: {folderPath}";
            }
        }

        private void CopyFilePath_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && !String.IsNullOrEmpty(treeView1.SelectedNode.Tag as String))
            {
                string filePath = treeView1.SelectedNode.Tag as String;
                Clipboard.SetText(filePath);
                this.toolStripStatusLabel1.Text = $"Copied file path: {filePath}";
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            processEvents = true;
            this.treeView1.Enabled = true;
            this.treeView1.Visible = true;
        }

        //public async Task<bool> WaitForWebBrowser()
        //{
        //    await this.webView21.EnsureCoreWebView2Async();
        //    return true;
        //}
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var path = openFileDialog1.FileName;
                if (!String.IsNullOrWhiteSpace(path))
                {
                    FileInfo fi = new FileInfo(path);
                    string ext = fi.Extension;
                    if (!String.IsNullOrWhiteSpace(ext))
                    {
                        ext = ext.ToLower();
                        if (this.browserFileTypes.Where(x => x == ext).FirstOrDefault() != null)
                        {
                            ShowBrowserFileType(path);
                        }
                        else
                        {
                            this.webView21.CoreWebView2.NavigateToString("Unable to show contents.");
                        }
                    }
                }


            }
        }

        //private void showTxtFile(String fileName)
        //{
        //    if (File.Exists(fileName))
        //    {
        //        var fileContents = File.ReadAllText(fileName);
        //        this.toolStripStatusLabel1.Text = fileName;
        //        this.webView21.CoreWebView2.NavigateToString(fileContents);
        //        //textBox.Anchor = AnchorStyles.Left;
        //    }
        //    else
        //    {
        //        MessageBox.Show($"The file {fileName} does not exist.");
        //    }
        //}

        private void ShowBrowserFileType(String fileName)
        {
            if (File.Exists(fileName))
            {
                this.webView21.Source = new Uri(fileName);
            }
            else
            {
                MessageBox.Show($"The file {fileName} does not exist.");
            }
        }

        private void Startup()
        {
            openFileDialog1 = new OpenFileDialog()
            {
                FileName = "Select a browser file",
                Filter = "All files (*.*)|*.*",
                Title = "Open file"
            };
            folderBrowserDialog1 = new FolderBrowserDialog()
            {

            };
        }

        private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                var path = this.folderBrowserDialog1.SelectedPath;
                if (!String.IsNullOrWhiteSpace(path))
                {
                    if (Directory.Exists(path))
                    {
                        ChangeDirectory(path, false);
                    }
                }


            }
        }

        private void ChangeDirectory(string directory, bool force)
        {
            this.imageList1.Images.Clear();
            if (!processEvents && !force)
            {
                return;
            }
            this.toolStripStatusLabel1.Text = directory;
            if (Directory.Exists(directory))
            {
                DirectoryInfo di = new DirectoryInfo(directory);
                this.treeView1.Nodes.Clear();
                string parentDir = null;
                if (di.Parent != null)
                {
                    parentDir = di.Parent.FullName;
                };
                if (!String.IsNullOrWhiteSpace(parentDir))
                {
                    TreeNode rootNode = new TreeNode();
                    rootNode.Tag = parentDir;
                    rootNode.Text = "Go up";
                    this.treeView1.Nodes.Add(rootNode);
                }
                TreeNode node = new TreeNode();
                node.Text = directory;
                this.treeView1.Nodes.Add(node);
                List<DirectoryInfo> directories = null;
                try
                {
                    directories = di.GetDirectories().Where(x => !String.IsNullOrWhiteSpace(x.Name) && !x.Name.StartsWith("$")).OrderBy(x => x.Name).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    directories = new List<DirectoryInfo>();
                    this.toolStripStatusLabel1.Text = "Cannot show directory";
                }

                Icon folderIcon = FolderIconHelper.GetSmallFolderIcon();
                string folderKey = "folder";
                imageList1.Images.Add(folderKey, folderIcon); // Use file extension or path as key

                foreach (var item in directories)
                {
                    TreeNode dir = new TreeNode();
                    dir.Text = item.Name;
                    dir.Tag = item.FullName;
                    
                    if (folderIcon != null)
                    {
                        dir.ImageKey = folderKey;
                        dir.SelectedImageKey = folderKey;
                    }
                    this.treeView1.Nodes.Add(dir);
                }
                List<FileInfo> files = null;
                try
                {
                    files = di.GetFiles().Where(x => !String.IsNullOrWhiteSpace(x.Name) && !x.Name.StartsWith("$")).OrderBy(x => x.Name).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    files = new List<FileInfo>();
                    this.toolStripStatusLabel1.Text = "Cannot show directory";
                }

                foreach (var item in files)
                {
                    TreeNode file = new TreeNode();
                    file.Text = item.Name;
                    file.Tag = item.FullName;
                    string[] parts = item.FullName.Split('.');
                    file.ForeColor = Color.Gray;
                    if (parts.Length > 1)
                    {
                        string ext = ("." + parts[parts.Length - 1]).ToLower();
                        if (this.browserFileTypes.Where(x => x == ext).FirstOrDefault() != null)
                        {
                            file.ForeColor = Color.Black;
                        }


                    }
                    string filePath = item.FullName;
                    Icon icon = FileIconHelper.GetSmallIcon(filePath);
                    if (icon != null)
                    {
                        imageList1.Images.Add(filePath, icon); // Use file extension or path as key
                        file.ImageKey = filePath;
                        file.SelectedImageKey = filePath;
                    }
                    this.treeView1.Nodes.Add(file);
                }
            }
            else
            {
                MessageBox.Show($"{directory} does not exist.");
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Don't process selection if it's from a right-click operation
            if (isRightClickOperation)
                return;
                
            var node = this.treeView1.SelectedNode;
            if (node != null && !String.IsNullOrEmpty(node.Tag as String))
            {
                var path = node.Tag as String;
                if (File.Exists(path))
                {
                    FileInfo fi = new FileInfo(path);
                    string ext = fi.Extension;
                    if (!String.IsNullOrWhiteSpace(ext))
                    {
                        ext = ext.ToLower();
                        if (this.browserFileTypes.Where(x => x == ext).FirstOrDefault() != null)
                        {
                            ShowBrowserFileType(path);
                        }
                        else
                        {
                            this.webView21.CoreWebView2.NavigateToString("Unable to show contents.");
                        }
                    }
                }
                else if (Directory.Exists(path))
                {
                    ChangeDirectory(path, false);
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string title = "About Directory Browser";
            string message = "The Directory Browser will provide previews for the following file types: ";
            string fileTypes = String.Join(", ", this.browserFileTypes.OrderBy(x => x));
            message += Environment.NewLine;
            message += fileTypes;
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
