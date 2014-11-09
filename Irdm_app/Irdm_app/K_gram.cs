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
    public partial class K_gram : Form
    {
        public K_gram()
        {
            InitializeComponent();
        }
        Dictionary<string, SortedSet<string>> h;
        string path;
        BinaryFormatter binaryFormatter;
        private void Form2_Load(object sender, EventArgs e)
        {
            path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\irdm\\" + "K_gram.txt";

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

        void enablegui()
        {
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            comboBox1.Enabled = true;
            listBox1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            label3.Enabled = true;
            label4.Enabled = true;
            label5.Enabled = true;

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
            enablegui();
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            h = new Dictionary<string, SortedSet<string>>();
            SortedSet<string> l;
            BackgroundWorker worker = sender as BackgroundWorker;
            int progress = 0;
            XDocument doc = XDocument.Load(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\irdm\\Stack Exchange Data Dump - Sept 2013\\Content\\android.stackexchange.com\\posts.xml");
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
                        String gram;
                        if (key.Length > 2)
                        {
                            for (int len = 0; len < key.Length - 2; len++)
                            {
                                gram = key.Substring(len, 3);
                                if (h.ContainsKey(gram.ToLower()))
                                {
                                    h[gram.ToLower()].Union(l);
                                }
                                else
                                {
                                    h.Add(gram.ToLower(), l);
                                }
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

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text.Length > 2)
            {
                if (!listBox1.Items.Contains(comboBox1.Text.ToLower()))
                {
                    listBox1.Items.Add(comboBox1.Text.ToLower());

                }

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 2)
            {
                if (!listBox1.Items.Contains(textBox2.Text.ToLower()))
                {
                    listBox1.Items.Add(textBox2.Text.ToLower());

                }

            }
        }
        String search;
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            search = textBox1.Text.ToLower();
            //search.Replace(" ", "");
            search.Replace(".", "");
            search.Replace("?", "");
            search.Replace("\'", "");
            addsugestions();
        }
        SortedSet<string> sugg;
        void addsugestions()
        {
            sugg = new SortedSet<string>();
            string temp;
            for (int i = 0; i < search.Length - 2; i++)
            {
                temp = search.Substring(i, 3);
                if (temp.Contains(' '))
                    continue;
                if (h.ContainsKey(temp))
                {

                    sugg.UnionWith(h[temp]);
                }

            }
            comboBox1.DataSource = sugg.ToArray();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Remove(listBox1.SelectedItem);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s = "";
            foreach (string str in listBox1.Items)
            {
                s += str + " , ";
            }
            if (s.Length > 3)
                s = s.Substring(0, s.Length - 3);
            Post testDialog = new Post(textBox1.Text, s);
            testDialog.ShowDialog(this);

            testDialog.Dispose();
        }
    }
}
