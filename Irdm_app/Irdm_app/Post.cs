using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Irdm_app
{
    public partial class Post : Form
    {
        String q, t;
        public Post(String ques, string tags)
        {
            InitializeComponent();
            q = ques;
            t = tags;
        }

        private void Post_Load(object sender, EventArgs e)
        {
            label1.Text = "Question is : " + q;
            label2.Text = "Tags selected are:" + t;
        }
    }
}
