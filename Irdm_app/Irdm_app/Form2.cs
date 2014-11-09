using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Irdm_app
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        Dictionary<string, SortedSet<string>> h;
        string path;
        BinaryFormatter binaryFormatter;
        private void Form2_Load(object sender, EventArgs e)
        {
            path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\irdm\\" + "dictionary.txt";

            binaryFormatter = new BinaryFormatter();

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

        void loaddict()
        {
            try
            {
                FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                h = (Dictionary<string, SortedSet<string>>)binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
            }
            catch { }
        }
        void loadfile()
        {
            if (h == null)
            {
                h = new Dictionary<string, SortedSet<string>>();
                loaddict();
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
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            h = new Dictionary<string, SortedSet<string>>();
            SortedSet<string> l;
            BackgroundWorker worker = sender as BackgroundWorker;
            int progress = 0;
            XDocument doc = XDocument.Load("D:\\Stack Exchange Data Dump - Sept 2013\\Content\\android.stackexchange.com\\posts.xml");
            var nodes = doc.Element("posts").Elements("row");
            string x;
            int prog = nodes.Count();
            int count = prog / 100;
            foreach (System.Xml.Linq.XElement item in nodes)
            {
                count--;
                l = new SortedSet<string>();
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
                            l.Add(x);
                        }

                    }
                    level = Convert.ToString(item.Attribute("Title").Value);
                    char[] t = { ' ', '.', '?', '\'' };
                    String[] temp = level.Split(t);
                    foreach (string key in temp)
                    {
                        if (key.Length > 2)
                        {
                            if (h.ContainsKey(key.ToLower()))
                            {
                                h[key.ToLower()].Union(l);
                            }
                            else
                            {
                                h.Add(key.ToLower(), l);
                            }
                        }
                    }
                }
                catch
                {
                    continue;
                }
                if (count < 1)
                {
                    count = prog / 100;
                    worker.ReportProgress(++progress);
                }
            }
            worker.ReportProgress(100);
            //h.ToList().Sort().ToArray();
            FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
            binaryFormatter.Serialize(fileStream, h);
            fileStream.Close();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != 100)
            {
                label1.Text = ("Progress =" + e.ProgressPercentage.ToString() + "%");

                label2.Text = "Scanning File.";
                for (int i = e.ProgressPercentage % 5; i > 0; i--)
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

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            loadfile();
        }
    }
}
