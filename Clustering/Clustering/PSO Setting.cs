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
    public partial class PSOForm : Form
    {
        public PSOForm()
        {
            InitializeComponent();
        }
        public String getWeight() { return textBox1.Text; }
        public String getC1() { return textBox2.Text; }
        public String getC2() { return textBox3.Text; }
        public String getCentro() { return textBox4.Text; }
        public String getItter() { return textBox5.Text; }
        public Button getApply() { return button1; }
    }
}
