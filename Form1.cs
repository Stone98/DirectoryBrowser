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
        OpenFileDialog openFileDialog1 = null;
        string initialDirectory = "C:/";
        public Form1()
        {
            InitializeComponent();
            changeDirectory(initialDirectory);
            startup();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var fileName = openFileDialog1.FileName;
                if (!String.IsNullOrWhiteSpace(fileName))
                {
                    showTxtFile(fileName);
                }


            }
        }

        private void showTxtFile(String fileName)
        {
            if (File.Exists(fileName))
            {
                this.splitContainer1.Panel2.Controls.Clear();
                var fileContents = File.ReadAllText(fileName);
                this.toolStripStatusLabel1.Text = fileName;
                TextBox textBox = new TextBox();
                this.splitContainer1.Panel2.Controls.Add(textBox);
                textBox.Multiline = true;
                textBox.MaxLength = fileContents.Length + 1;
                textBox.Text = fileContents;
                textBox.Dock = DockStyle.Fill;
                //textBox.Anchor = AnchorStyles.Left;
            }
            else
            {
                MessageBox.Show($"The file {fileName} does not exist.");
            }
        }

        private void startup()
        {
            openFileDialog1 = new OpenFileDialog()
            {
                FileName = "Select a text file",
                Filter = "Text files (*.txt)|*.txt",
                Title = "Open text file"
            };
        }

        private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        
        private void changeDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                this.treeView1.Nodes.Clear();
                TreeNode node = new TreeNode();
                node.Text = directory;
                this.treeView1.Nodes.Add(node);
                DirectoryInfo di = new DirectoryInfo(directory);
                var directories = di.GetDirectories().Where(x => !String.IsNullOrWhiteSpace(x.Name) && !x.Name.StartsWith("$")).OrderBy(x => x.Name).ToList();
                foreach (var item in directories)
                {
                    TreeNode dir = new TreeNode();                   
                    dir.Text = "[] " + item.Name;
                    dir.Tag = item.FullName;
                    this.treeView1.Nodes.Add(dir);
                }
                var files = di.GetFiles().Where(x => !String.IsNullOrWhiteSpace(x.Name) && !x.Name.StartsWith("$")).OrderBy(x => x.Name).ToList();
                foreach (var item in files)
                {
                    TreeNode file = new TreeNode();
                    file.Text = item.Name;
                    file.Tag = item.FullName;
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
            var node = this.treeView1.SelectedNode;
            if(node != null && !String.IsNullOrEmpty(node.Tag as String))
            {
                var path = node.Tag as String;
                if (File.Exists(path))
                {
                    if(path.EndsWith("txt", StringComparison.OrdinalIgnoreCase))
                    {
                        showTxtFile(path);
                    }
                    else
                    {
                        this.splitContainer1.Panel2.Controls.Clear();
                    }
                }
                else if (Directory.Exists(path))
                {
                    changeDirectory(path);
                }
            }
        }
    }
}
