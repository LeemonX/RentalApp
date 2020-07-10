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
    public partial class clientsForm : Form
    {
        private String conString;
        private NpgsqlConnection connection;
        public clientsForm()
        {
            InitializeComponent();
            update();
        }

        private void editClient_Click(object sender, EventArgs e)
        {
            connect();
            try
            {
                NpgsqlCommand com = new NpgsqlCommand("update clients set (clientname, manager, bankid, officeaddress, email) = (@cname, @mng, @bank, @address, @email) where clientcode=@ccode", connection);

                com.Parameters.Add("ccode", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse((string)dataGridView1.CurrentRow.Cells[0].Value);

                com.Parameters.Add("cname", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[1].Value;
                com.Parameters.Add("mng", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[2].Value;
                com.Parameters.Add("bank", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[3].Value;
                com.Parameters.Add("address", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[4].Value;
                com.Parameters.Add("email", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[5].Value;
                com.ExecuteNonQuery();

                MessageBox.Show("В таблице обновлена запись");
            }
            catch (Exception)
            {
                MessageBox.Show("Проверьте введенные данные");
            }
            
            update();

            connection.Close();
        }

        private void addClient_Click(object sender, EventArgs e)
        {
            connect();
            try
            {
                NpgsqlCommand newClient = new NpgsqlCommand("INSERT INTO clients(clientname, manager, bankid, officeaddress, email) VALUES (@cname, @mng, @bank, @address, @email)", connection);


                newClient.Parameters.Add("cname", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[1].Value;
                newClient.Parameters.Add("mng", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[2].Value;
                newClient.Parameters.Add("bank", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[3].Value;
                newClient.Parameters.Add("address", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[4].Value;
                newClient.Parameters.Add("email", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[5].Value;
                newClient.ExecuteNonQuery();

                MessageBox.Show("Добавлен новый клиент");
            }
            catch(Exception)
            {
                MessageBox.Show("Проверьте введенные данные");
            }
                

            connection.Close();
            update();
        }

        private void deleteClient_Click(object sender, EventArgs e)
        {
            connect();
            try
            {
                NpgsqlCommand com = new NpgsqlCommand("delete from clients where clientname=@cname", connection);

                com.Parameters.Add("cname", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[1].Value;
                com.ExecuteNonQuery();

                MessageBox.Show("В таблице удалена запись");
               
                update();
            }
            catch (Exception)
            {
                MessageBox.Show("Удаление невозможно, имеются связанные записи");
            }
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
            NpgsqlCommand selectClients = new NpgsqlCommand("SELECT clientcode, clientname, manager, bankid, officeaddress, email FROM clients c", connection);
            NpgsqlDataReader reader = selectClients.ExecuteReader();

            DataTable rTable = new DataTable();
            rTable.Columns.Add("Идентификатор");
            rTable.Columns.Add("Название компании");
            rTable.Columns.Add("Менеджер");
            rTable.Columns.Add("Номер счёта");
            rTable.Columns.Add("Адрес офиса");
            rTable.Columns.Add("E-mail");

            while (reader.Read())
            {
                rTable.Rows.Add(new object[] { reader.GetValue(0), reader.GetValue(1), reader.GetValue(2), reader.GetValue(3), reader.GetValue(4), reader.GetValue(5) });
            }

            dataGridView1.DataSource = rTable;
            connection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connect();
            NpgsqlCommand selectRented = new NpgsqlCommand("SELECT  clientcode, clientname, manager, bankid, officeaddress, email FROM clients WHERE (clientname LIKE @cname) and (manager LIKE @mng) and (officeaddress like @address) and (email like @email)", connection);

            selectRented.Parameters.AddWithValue("@cname", "%" + textBox1.Text.Trim() + "%");
            selectRented.Parameters.AddWithValue("@mng", "%" + textBox2.Text.Trim() + "%");
            selectRented.Parameters.AddWithValue("@address", "%" + textBox3.Text.Trim() + "%");
            selectRented.Parameters.AddWithValue("@email", "%" + textBox4.Text.Trim() + "%");



            NpgsqlDataReader reader = selectRented.ExecuteReader();

            DataTable rTable = new DataTable();
            rTable.Columns.Add("Идентификатор");
            rTable.Columns.Add("Название компании");
            rTable.Columns.Add("Менеджер");
            rTable.Columns.Add("Номер счёта");
            rTable.Columns.Add("Адрес офиса");
            rTable.Columns.Add("E-mail");

            while (reader.Read())
            {
                rTable.Rows.Add(new object[] { reader.GetValue(0), reader.GetValue(1), reader.GetValue(2), reader.GetValue(3), reader.GetValue(4), reader.GetValue(5) });
            }

            dataGridView1.DataSource = rTable;
            connection.Close();
        }
    }
}
