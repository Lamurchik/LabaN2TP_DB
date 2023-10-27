using System.ComponentModel;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;
using System.Security.Cryptography;
using Microsoft.VisualBasic;
using System.Data;
using System;

namespace LabaN2TP_DB
{
    public partial class Form1 : Form
    {

        static string connectionString = "Server=localhost;Database=library;Trusted_Connection=True;";
        static SqlConnection connection = new SqlConnection(connectionString);
        static BindingList<Reader> readers;

        static BindingList<Reader> readersCopy;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            connection.Open();
            readers = ReaderCreateDB(connection);
            dataGridView1.DataSource = readers;
            Copy();
            connection.Close();


            #region конекстное меню

            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

            ToolStripMenuItem contextAddUser = new ToolStripMenuItem("добавить пользователя");
            contextAddUser.Click += Insert_Click;
            ToolStripMenuItem contextEdit = new ToolStripMenuItem("редактировать");
            contextEdit.Click += Update_Click;
            ToolStripMenuItem contextDelate = new ToolStripMenuItem("удалить");
            contextDelate.Click += Delate_Click;
            ToolStripMenuItem contextProc = new ToolStripMenuItem("сколько книг?");
            contextProc.Click += HowBook_Click;
            ToolStripMenuItem contextAddBook = new ToolStripMenuItem("выдать книгу");
            contextAddBook.Click += InsertBook_Click;


            contextMenuStrip.Items.Add(contextAddUser);
            contextMenuStrip.Items.Add(contextEdit);
            contextMenuStrip.Items.Add(contextDelate);
            contextMenuStrip.Items.Add(contextProc);
            contextMenuStrip.Items.Add(contextAddBook);

            dataGridView1.ContextMenuStrip = contextMenuStrip;


            #endregion
        }

        private void Copy()
        {
            BindingList<Reader> r = new BindingList<Reader>();
            foreach (Reader red in readers)
            {
                r.Add(red.DeepCopy());
            }
            readersCopy= r;

        }
        private void Restart()
        {
            connection.Open();
            readers = ReaderCreateDB(connection);
            dataGridView1.DataSource = readers;
            Copy();
            connection.Close();
        }
        static BindingList<Reader> ReaderCreateDB(SqlConnection connection)
        {
            string sqlExpression = "SELECT * FROM Reader";
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataReader reader = command.ExecuteReader();


            BindingList<Reader> ret = new BindingList<Reader>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {

                    Reader r = new Reader
                        (
                        (int)reader["ReaderID"],
                        (string)reader["Name"],
                        (bool)reader["IsBanned"],
                        (DateTime)reader["RegistrationDate"],
                        (float)(double)reader["Fine"]
                        );
                    // try { r.Fine = (float)reader["Fine"]; }
                    // catch { r.Fine = 0f; }
                    ret.Add(r);
                }

            }
            reader.Close();
            return ret;
        }

        #region методы для контекстного меню

        private void Insert_Click(object sender, EventArgs e)
        {
            Reader ins = new Reader();
            Сhange insert = new Сhange(ins);

            insert.ShowDialog();
            if (insert.DialogResult == DialogResult.OK)
            {
                readers.Add(ins);
                Insert(ins.Name, ins.IsBan, ins.Date, ins.Fine);
            }
            Restart();
        }

        private void InsertBook_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
                if (selectedRowCount != 1)
                {
                    MessageBox.Show("выделите читаля каторму выдаёте книгу", "неопределеность", MessageBoxButtons.OK);
                    return;
                }

                string name = Interaction.InputBox("input Book name");
                int id = readers[dataGridView1.SelectedRows[0].Index].Id;

                InsertBook(name, id);
                
            }
            catch { MessageBox.Show("неизвестная ошибка"); }





        }
        private void Update_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
                if (selectedRowCount != 1)
                {
                    MessageBox.Show("выделите строку которую хотите изменить", "неопределеность", MessageBoxButtons.OK);
                    return;
                }

                Reader up = readers[dataGridView1.SelectedRows[0].Index].DeepCopy();
                Сhange update = new Сhange(up);

                update.ShowDialog();

                if (update.DialogResult == DialogResult.OK)
                {
                    readers[dataGridView1.SelectedRows[0].Index] = up;
                    Update(up.Id, up.Name, up.IsBan, up.Date, up.Fine);
                }
                Copy();



            }
            catch { MessageBox.Show("неизвестная ошибка"); }
        }

        private void Delate_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
                if (selectedRowCount != 1)
                {
                    MessageBox.Show("выделите строку которую хотите изменить", "неопределеность", MessageBoxButtons.OK);
                    return;
                }
                if (Delete(readers[dataGridView1.SelectedRows[0].Index].Id))
                    readers.RemoveAt(dataGridView1.SelectedRows[0].Index);
                Copy();
            }
            catch { MessageBox.Show("неизвестная ошибка"); }
        }

        private void HowBook_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
                if (selectedRowCount != 1)
                {
                    MessageBox.Show("выделите строку которую хотите изменить", "неопределеность", MessageBoxButtons.OK);
                    return;
                }

                int id = readers[dataGridView1.SelectedRows[0].Index].Id;
                string name = readers[dataGridView1.SelectedRows[0].Index].Name;
                string sqlExpression = "HowBook";

                connection.Open();

                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter idParam = new SqlParameter
                {
                    ParameterName = "@id",
                    Value = id
                };
                command.Parameters.Add(idParam);

                SqlParameter resultParam = new SqlParameter
                {
                    ParameterName = "@result",
                    SqlDbType = SqlDbType.Int,


                    Direction = ParameterDirection.Output
                };

                command.Parameters.Add(resultParam);

                command.ExecuteNonQuery();


                MessageBox.Show($"число книг у {name} = {(int)command.Parameters["@result"].Value}");

                connection.Close();
            }
            catch { MessageBox.Show("неизвестная ошибка"); }
        }

        #endregion



        #region вспомагательные методы 

        private static bool Delete(int id)
        {
            connection.Open();


            string sqlExpression = "HowBook";
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter idParam = new SqlParameter
            {
                ParameterName = "@id",
                Value = id
            };
            command.Parameters.Add(idParam);
            SqlParameter resultParam = new SqlParameter
            {
                ParameterName = "@result",
                SqlDbType = SqlDbType.Int,


                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(resultParam);
            command.ExecuteNonQuery();

            if ((int)command.Parameters["@result"].Value > 0)
            {
                MessageBox.Show("за данным пользователем закреплена книга, \n прежде чем удалить пользователя он должен вернуть книгу");
                return false;
            }
            else
            {
                SqlCommand commandDelete = new SqlCommand("delete from Reader where ReaderID=@id");
                commandDelete.Parameters.AddWithValue("@id", id);
                commandDelete.Connection = connection;
                commandDelete.ExecuteNonQuery();
            }

            connection.Close();
            return true;
        }

        private void Insert(string name, bool isBan, DateTime dateTime, float? fine)
        {

            SqlCommand commandInsert = new SqlCommand("INSERT INTO Reader ( Name, IsBanned, RegistrationDate, Fine) VALUES (@name, @IsBanned ,@RegistrationDate,@Fine)");
            commandInsert.Parameters.AddWithValue("@name", name);
            commandInsert.Parameters.AddWithValue("@IsBanned", isBan);
            commandInsert.Parameters.AddWithValue("@RegistrationDate", dateTime);
            commandInsert.Parameters.AddWithValue("@Fine", fine);


            connection.Open();
            commandInsert.Connection = connection;
            commandInsert.ExecuteNonQuery();
            connection.Close();
        }


        private void InsertBook(string bookName, int readerId)
        {
            SqlCommand commandInsert = new SqlCommand("INSERT INTO Book ( BookName, DateTaken, ReaderID) VALUES (@name, @DateTaken ,@ReaderID)");
            commandInsert.Parameters.AddWithValue("@name", bookName);
            commandInsert.Parameters.AddWithValue("@DateTaken", DateTime.Today);
            commandInsert.Parameters.AddWithValue("@ReaderID", readerId);


            connection.Open();
            commandInsert.Connection = connection;
            commandInsert.ExecuteNonQuery();
            connection.Close();
        }

        public void Update(int id, string name, bool isBan, DateTime dateTime, float? fine)
        {
            SqlCommand commandUpdate = new SqlCommand($"update Reader set Name=@name, IsBanned=@IsBanned, RegistrationDate=@RegistrationDate,Fine=@Fine where ReaderID={id}");
            commandUpdate.Parameters.AddWithValue("@name", name);
            commandUpdate.Parameters.AddWithValue("@IsBanned", isBan);
            commandUpdate.Parameters.AddWithValue("@RegistrationDate", dateTime);
            commandUpdate.Parameters.AddWithValue("@Fine", fine);


            connection.Open();
            commandUpdate.Connection = connection;
            commandUpdate.ExecuteNonQuery();
            connection.Close();
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)  //просмотреть таблицу с книгами 
        {
            Form2 books = new Form2();

            books.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e) //поиск по имени
        {
            string name = Interaction.InputBox("input name");
            int index = -1;
            for (int i = 0; i < readers.Count; i++)
            {
                if (readers[i].Name == name)
                    index = i;

            }
            if (index != -1)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[index].Selected = true;

                //dataGridView1.Rows[index].Cells[0].Selected = true;


                dataGridView1.CurrentCell = dataGridView1.Rows[index].Cells[2];
            }

        }


        private void Text_Changet(object sender, EventArgs e)
        {
            
            string name = textBox1.Text;
            if (name == "")
                Restart();
            else
            {
                for (int i = 0; i < readers.Count; i++)
                {
                    if (!readers[i].Name.Contains(name))
                    {
                        readers.Remove(readers[i]);
                    }
                }
            }
        }
    }
}