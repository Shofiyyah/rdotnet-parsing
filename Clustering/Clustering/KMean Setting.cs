using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Clustering
{
    public partial class KMeansForm : Form
    {
        public KMeansForm()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        public int getCentro() {
            return Convert.ToInt32(textBox1.Text);
        }
        public int getItter()
        {
            return Convert.ToInt32(textBox2.Text);
        }
        public Button getApply() {
            return button1;
        }
    }
}
