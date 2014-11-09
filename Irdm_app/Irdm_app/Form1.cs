using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Threading.Tasks;

namespace Irdm_app
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string path;
        SortedSet<string> h;
        private void Form1_Load(object sender, EventArgs e)
        {
            path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\irdm\\" + "tags.txt";


            if (!File.Exists(path))
            {
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\irdm"))
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\irdm");
                }
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                    if (backgroundWorker1.IsBusy != true)
                    {
                        // Start the asynchronous operation.
                        label2.Text = "Scanning File...";
                        backgroundWorker1.RunWorkerAsync();
                    }
                }
            }
            else
            {
                //load file into memory
                loadfile();
                
            }


        }

        void loadfile()
        {
            if (h==null)
            {
                h = new SortedSet<string>(File.ReadAllLines(path));
            }
            if (h.Count < 1)
            {
                if (backgroundWorker1.IsBusy != true)
                {
                    // Start the asynchronous operation.
                    label2.Text = "Scanning File...";
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            label2.Text = "File Loaded";
            label1.Text = "";
        }

       

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != 100)
            {
                label1.Text = ("Progress =" + e.ProgressPercentage.ToString() + "%");
                
                label2.Text = "Scanning File.";
                for(int i=e.ProgressPercentage%5;i>0;i--)
                {
                    
                label2.Text += ".";
                }
            }
            else
            {
                label2.Text = "File Scan";
                label1.Text = "Completed";
            }
        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            h = new SortedSet<string>();
            BackgroundWorker worker = sender as BackgroundWorker;
            int progress = 0;
            XDocument doc = XDocument.Load("D:\\Stack Exchange Data Dump - Sept 2013\\Content\\android.stackexchange.com\\posts.xml");
            var nodes = doc.Element("posts").Elements("row");
            textBox1.Text = "";
            string x;
            int prog = nodes.Count();
            int count = prog/100;
            foreach (System.Xml.Linq.XElement item in nodes)
            {
                count--;
                try
                {
                    string level = Convert.ToString(item.Attribute("Tags").Value);
                    for (int i = 0; i < level.Length; i++)
                    {
                        if (level[i] == '<')
                        {
                            i++;
                            x = "";
                            do
                            {
                                x += level[i];
                                ++i;
                            } while (level[i] != '>');
                            h.Add(x);
                        }

                    }
                }
                catch
                {
                    continue;
                }
                if (count < 1)
                {
                    count = prog/100;
                    worker.ReportProgress(++progress);
                }
            }
            worker.ReportProgress(100);
            //h.ToList().Sort().ToArray();
            
            File.WriteAllLines(path, h.ToArray());
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loadfile();
        }

       
    }
}
