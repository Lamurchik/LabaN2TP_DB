using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabaN2TP_DB
{
    public partial class Form2 : Form
    {
        static string connectionString = "Server=localhost;Database=library;Trusted_Connection=True;";
        SqlConnection connection = new SqlConnection(connectionString);
        SqlDataAdapter adapter;



        DataSet ds;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string sql = "SELECT * FROM Book";

            connection.Open();

            adapter = new SqlDataAdapter(sql, connection);

            ds = new DataSet();

            adapter.Fill(ds, "Book");

            connection.Close();


            dataGridView1.DataSource = ds.Tables["Book"];

            dataGridView1.Columns["ID"].Visible = false;
        }

        private void button1_Click(object sender, EventArgs e) //сохранить
        {
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);
            adapter.Update(ds, "Book");
        }
    }
}
