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
    public partial class FaForm : Form
    {
        public FaForm()
        {
            InitializeComponent();
        }

        public ComboBox getCombo() { return comboBox1; }
        public Button getApply() { return button2; }
        public int getNumberFactor() { return Convert.ToInt32(textBox1.Text); }
        public Boolean getRotate() { return checkBox3.Checked; }
    }
}
