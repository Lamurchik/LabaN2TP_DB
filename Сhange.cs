using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabaN2TP_DB
{
    public partial class Сhange : Form
    {
        Reader reader;
        public Сhange(Reader r)
        {
            this.reader = r;
            InitializeComponent();
            dateTimePicker1.MaxDate = DateTime.Today;
            MaximizeBox = false;

            textBox1.DataBindings.Add(nameof(textBox1.Text), reader, nameof(reader.Name));
            numericUpDown1.DataBindings.Add(nameof(numericUpDown1.Value), reader, nameof(reader.Fine));
            dateTimePicker1.DataBindings.Add(nameof(dateTimePicker1.Value), reader, nameof(reader.Date));
            checkBox1.DataBindings.Add(nameof(checkBox1.Checked), reader, nameof(reader.IsBan));

            numericUpDown1.Value = 56m;
        }

        private void Сhange_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            {
                if (reader.Name == "")
                    MessageBox.Show("введите имя ");
                else
                {
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
                
            }
        }
    }
}
