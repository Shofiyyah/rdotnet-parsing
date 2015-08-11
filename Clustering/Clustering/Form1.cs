using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;


namespace Clustering
{
    public partial class Form1 : Form
    {
        private string Excel03ConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1}'";
        private string Excel07ConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1}'";
        KMeansForm frm2;
        PSOForm frm3;
        FaForm frm4;
        string dataPath0;
        string dataPath1;
        string dataPath2;

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string filePath = openFileDialog1.FileName;
            dataPath1 = filePath.Replace("\\","/");
            dataPath0 = dataPath1;
            setShow(1);
            fill(filePath, dataGridView1);
        }
        public Form1()
        {
            frm2 = new KMeansForm();
            frm3 = new PSOForm();
            frm4 = new FaForm();
            InitializeComponent();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        private void kMeansToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            frm2.ShowDialog();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }
        private void rbHeaderYes(object sender, EventArgs e)
        {

        }
        private void pSOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm3.ShowDialog();
        }
        private void factorAnalysisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frm4.ShowDialog();
        }
        private void tabChange(object sender, EventArgs e) {
            if (tabControl1.SelectedIndex == 0) dataPath0 = dataPath1;
            else dataPath0 = dataPath2;
        }

        public void fill(string filePath, DataGridView dataGridView)
        {
            string extension = Path.GetExtension(filePath);
            string header = "YES";
            string conStr, sheetName;

            conStr = string.Empty;
            switch (extension)
            {

                case ".xls": //Excel 97-03
                    conStr = string.Format(Excel03ConString, filePath, header);
                    break;

                case ".xlsx": //Excel 07
                    conStr = string.Format(Excel07ConString, filePath, header);
                    break;
            }

            //Get the name of the First Sheet.
            using (OleDbConnection con = new OleDbConnection(conStr))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    cmd.Connection = con;
                    con.Open();
                    DataTable dtExcelSchema = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                    con.Close();
                }
            }

            //Read Data from the First Sheet.
            using (OleDbConnection con = new OleDbConnection(conStr))
            {
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    using (OleDbDataAdapter oda = new OleDbDataAdapter())
                    {
                        DataTable dt = new DataTable();
                        cmd.CommandText = "SELECT * From [" + sheetName + "]";
                        cmd.Connection = con;
                        con.Open();
                        oda.SelectCommand = cmd;
                        oda.Fill(dt);
                        con.Close();

                        //Populate DataGridView.
                        dataGridView.DataSource = dt;
                    }
                }
            }
        }
        public KMeansForm   getForm2() { return frm2; }
        public PSOForm      getForm3() { return frm3; }
        public FaForm       getForm4() { return frm4; }
        public string       getDataPath() { return dataPath0; }
        public DataGridView getScoreGrid() { return dataGridView2; }
        public void setDataPath2(String ss) { dataPath2 = ss; }
        public void         setShow1(int i) {
            switch (i)
            {
                case 1:
                    tabControl2.SelectedTab = tabPage3;
                    break;
                case 2:
                    tabControl2.SelectedTab = tabPage4;
                    break;
                default:
                    tabControl2.SelectedTab = tabPage5;
                    break;
            }
        }
        public void         setShow(int i)
        {
            switch (i)
            {
                case 1:
                    tabControl1.SelectedTab = tabPage1;
                    break;
                default:
                    tabControl1.SelectedTab = tabPage2;
                    break;
            }
        } 

        public PictureBox getFaDiagram() { return faDiagram; }
        public RichTextBox getFAMSA() { return richTextBox1; }
        public PictureBox getNumberF() { return numberFactorDiagram; }
        public DataGridView getComunalityGrid() { return communGrid; }

        public TextBox getKQuanzErr() { return textBox1; }
        public TextBox getKIntercls() { return textBox2; }
        public TextBox getKIntracls() { return textBox3; }
        public PictureBox getKClusterPlot() { return clusterKPlot; }
        public PictureBox getKConvergence() { return convergenceKPlot; }
        public DataGridView getKClusterGrid() { return KClusterGrid; }

        public TextBox getPQuanzErr() { return textBox6; }
        public TextBox getPIntercls() { return textBox4; }
        public TextBox getPIntracls() { return textBox5; }
        public PictureBox getPClusterPlot() { return clusterPPlot; }
        public PictureBox getPConvergence() { return convPPlot; }
        public DataGridView getPClusterGrid() { return PClusterGrid; }
    }
}
