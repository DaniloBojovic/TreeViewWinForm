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
using System.Xml;
using System.Xml.Linq;

namespace WindowsFormsApplication4
{
    public partial class FormTreeView : Form
    {
        string directoryPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\TreeView.xml";
        XDocument document = null;
        public FormTreeView()
        {
            InitializeComponent();
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            txtFolderName.Text = folderBrowserDialog1.SelectedPath;
        }

        private void btnCreateXml_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(txtFolderName.Text))
            {
                string header = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><Directory></Directory> ";
                document = XDocument.Parse(header);
                XElement root = document.Root;
                CreateXmlRecursion(txtFolderName.Text, root);                
                document.Save(directoryPath);
                MessageBox.Show("Xml file successfully created.");
                if (document != null)
                {
                    treeView1.Nodes.Clear();
                    TreeNode treeNode = new TreeNode(document.Root.FirstNode.ToString());
                    AddNode(document.Root, treeNode);
                    treeView1.Nodes.Add(treeNode);
                }
            }
            else
            {
                MessageBox.Show("Browse directory.");
            }
        }

        private float CreateXmlRecursion(string path, XElement element)
        {
            element.SetValue(path);
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            float folderSize = 0;
            int numOfFiles = 0;
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                try
                {
                    float fileSize = file.Length;
                    folderSize += fileSize;
                    element.Add(new XElement("File", new object[] {
                        new XAttribute("FileName",file.Name),
                        new XAttribute("FileSize",fileSize+"(bytes)"),
                        new XAttribute("FileCreationTime",file.CreationTime),
                        new XAttribute("FileLastAccessTime",file.LastAccessTime),
                        new XAttribute("FileLastModifiedTime",file.LastWriteTime)}));
                    numOfFiles++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            foreach (string directory in Directory.GetDirectories(path))
            {
                DirectoryInfo info = new DirectoryInfo(directory.ToString());
                XElement subDirectory = new XElement("Directory");
                element.Add(subDirectory);
                float sudDirSize = CreateXmlRecursion(directory, subDirectory);
                folderSize += sudDirSize;
            }
            element.Add(new XAttribute[]
            {
                new XAttribute("FolderName",directoryInfo.Name),
                new XAttribute("FolderSize",folderSize+"(bytes)"),
                new XAttribute("NumberOfFiles",numOfFiles)
            });
            return folderSize;
        }

        private void AddNode(XElement xElement, TreeNode treeNode)
        {
            treeView1.Nodes.Clear();
            foreach (var att in xElement.Attributes())
            {
                treeNode.Text = treeNode.Text;
                treeNode.Tag = xElement;
            }
            foreach (XElement childElement in xElement.Elements())
            {
                TreeNode tNode = treeNode.Nodes.Add(childElement.FirstAttribute.Value);
                AddNode(childElement, tNode);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var xElement = treeView1.SelectedNode.Tag as XElement;

            if (xElement != null)
            {
                lblInfo.Text = "";
                foreach (var a in xElement.Attributes())
                {
                    lblInfo.Text += a.Name.ToString() + ": " + a.Value + "\r\n" + "\r\n";
                }                
            }
            else
            {
                lblInfo.Text = "";
            }
        }
        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
