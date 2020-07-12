using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RentalApp
{
    public partial class AdminPanel : Form
    {
        private String conString;
        private NpgsqlConnection connection;
        int userid;
        public AdminPanel(int userid)
        {
            InitializeComponent();
            update();
            this.userid = userid;
        }

        private void addUser_Click(object sender, EventArgs e)
        {
            connect();
            try
            {
                
                NpgsqlCommand newSpace = new NpgsqlCommand("INSERT INTO users(username, pass) VALUES (@uname, @password)", connection);


                newSpace.Parameters.Add("@uname", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[1].Value;
                newSpace.Parameters.Add("@password", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[2].Value;
                newSpace.ExecuteNonQuery();

                MessageBox.Show("Добавлен новый пользователь");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Проверьте введенные данные");
                Console.WriteLine(ex);
            }


            connection.Close();
            update();
        }

        private void editUser_Click(object sender, EventArgs e)
        {
            connect();
            try
            {
                NpgsqlCommand editUser = new NpgsqlCommand("update users set (username, pass) = (@uname, @password) where userid=@uid", connection);

                editUser.Parameters.Add("@scode", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse((string)dataGridView1.CurrentRow.Cells[0].Value);
                editUser.Parameters.Add("@uname", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[1].Value;
                editUser.Parameters.Add("@password", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[2].Value;
                

                editUser.ExecuteNonQuery();

                MessageBox.Show("Пользователь обновлен");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Проверьте введенные данные");
                Console.WriteLine(ex);
            }


            connection.Close();
            update();
        }

        private void deleteUser_Click(object sender, EventArgs e)
        {
            connect();
            if (int.Parse((string)dataGridView1.CurrentRow.Cells[0].Value) != userid)
            {
                try
                {

                    NpgsqlCommand com = new NpgsqlCommand("delete from users where userid=@uid", connection);

                    com.Parameters.Add("@uid", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse((string)dataGridView1.CurrentRow.Cells[0].Value);
                    com.ExecuteNonQuery();

                    MessageBox.Show("Пользователь удален");

                    update();
                }
                catch (Exception)
                {
                    MessageBox.Show("Удаление невозможно, имеются связанные записи");
                }
            }
            else
            {
                MessageBox.Show("Невозможно удалить текущего пользователя");
            }
                
            connection.Close();
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connect();
            NpgsqlCommand selectUsers = new NpgsqlCommand("SELECT userid, username, pass FROM users WHERE username LIKE @uname", connection);
            selectUsers.Parameters.AddWithValue("@uname", "%" + textBox1.Text.Trim() + "%");

            NpgsqlDataReader reader = selectUsers.ExecuteReader();

            DataTable rTable = new DataTable();
            rTable.Columns.Add("ID");
            rTable.Columns.Add("Логин");
            rTable.Columns.Add("Пароль");



            while (reader.Read())
            {
                rTable.Rows.Add(new object[] { reader.GetValue(0), reader.GetValue(1), reader.GetValue(2) });
            }

            dataGridView1.DataSource = rTable;
            connection.Close();
        }

        public void connect()
        {
            conString = "Server=localhost;port=5432;username='postgres';password='root';database=RentalDB";
            connection = new NpgsqlConnection(conString);
            connection.Open();
        }

        public void update()
        {
            connect();
            NpgsqlCommand selectUsers = new NpgsqlCommand("SELECT userid, username, pass FROM users", connection);
            NpgsqlDataReader reader = selectUsers.ExecuteReader();

            DataTable rTable = new DataTable();
            rTable.Columns.Add("ID");
            rTable.Columns.Add("Логин");
            rTable.Columns.Add("Пароль");
            
            

            while (reader.Read())
            {
                rTable.Rows.Add(new object[] { reader.GetValue(0), reader.GetValue(1), reader.GetValue(2)});
            }

            dataGridView1.DataSource = rTable;
            connection.Close();
        }
    }
}
