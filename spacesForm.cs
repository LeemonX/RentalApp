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
    public partial class spacesForm : Form
    {
        private String conString;
        private NpgsqlConnection connection;
        public spacesForm()
        {
            InitializeComponent();
            update();
        }

        private void deleteSpace_Click(object sender, EventArgs e)
        {
            connect();
            try
            {
                NpgsqlCommand com = new NpgsqlCommand("delete from spaces where spacename=@sname", connection);

                com.Parameters.Add("sname", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[1].Value;
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

        private void editSpace_Click(object sender, EventArgs e)
        {
            connect();
            try
            {
                NpgsqlCommand com = new NpgsqlCommand("update spaces set (spacename, spacelvl, spacearea, ac, rentprice) = (@sname, @lvl, @area, @ac, @rentprice) where spacecode=@scode", connection);

                com.Parameters.Add("scode", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse((string)dataGridView1.CurrentRow.Cells[0].Value);

                com.Parameters.Add("sname", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[1].Value;
                com.Parameters.Add("lvl", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse((string)dataGridView1.CurrentRow.Cells[2].Value);
                com.Parameters.Add("area", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse((string)dataGridView1.CurrentRow.Cells[3].Value);
                com.Parameters.Add("ac", NpgsqlTypes.NpgsqlDbType.Boolean).Value = Boolean.Parse((string)dataGridView1.CurrentRow.Cells[4].Value);
                com.Parameters.Add("rentprice", NpgsqlTypes.NpgsqlDbType.Money).Value = int.Parse((string)dataGridView1.CurrentRow.Cells[5].Value);
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

        private void addSpace_Click(object sender, EventArgs e)
        {
            connect();
            try
            {
                NpgsqlCommand newSpace = new NpgsqlCommand("INSERT INTO spaces(spacename, spacelvl, spacearea, ac, rentprice) VALUES (@sname, @lvl, @area, @ac, @rentprice)", connection);


                newSpace.Parameters.Add("sname", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[1].Value;
                newSpace.Parameters.Add("lvl", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse((string)dataGridView1.CurrentRow.Cells[2].Value);
                newSpace.Parameters.Add("area", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse((string)dataGridView1.CurrentRow.Cells[3].Value);
                newSpace.Parameters.Add("ac", NpgsqlTypes.NpgsqlDbType.Boolean).Value = Boolean.Parse((string)dataGridView1.CurrentRow.Cells[4].Value);
                newSpace.Parameters.Add("rentprice", NpgsqlTypes.NpgsqlDbType.Money).Value = int.Parse((string)dataGridView1.CurrentRow.Cells[5].Value);
                newSpace.ExecuteNonQuery();

                MessageBox.Show("Добавлена новая торговая площадь");
            }
            catch (Exception)
            {
                MessageBox.Show("Проверьте введенные данные");
            }


            connection.Close();
            update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connect();
            NpgsqlCommand selectRented = new NpgsqlCommand("SELECT  spacecode, spacename, spacelvl, spacearea, ac, rentprice, rentstatus FROM spaces WHERE (spacename LIKE @sname) and @startp<rentprice and rentprice<@finp and spacelvl=@lvl and @starta<spacearea and spacearea<@fina and ac=@acval and rentstatus=@rentval", connection);

            
            selectRented.Parameters.Add("sname", NpgsqlTypes.NpgsqlDbType.Varchar).Value = "%" + textBox1.Text.Trim() + "%";
            selectRented.Parameters.Add("starta", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse(textBox2.Text.Trim());
            selectRented.Parameters.Add("fina", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse(textBox3.Text.Trim());
            selectRented.Parameters.Add("lvl", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse(lvl.Text.Trim());
            selectRented.Parameters.Add("startp", NpgsqlTypes.NpgsqlDbType.Money).Value = int.Parse(textBox5.Text.Trim());
            selectRented.Parameters.Add("finp", NpgsqlTypes.NpgsqlDbType.Money).Value = int.Parse(textBox6.Text.Trim());
            selectRented.Parameters.Add("acval", NpgsqlTypes.NpgsqlDbType.Boolean).Value = Boolean.Parse(checkBox1.Checked.ToString());
            selectRented.Parameters.Add("rentval", NpgsqlTypes.NpgsqlDbType.Boolean).Value = Boolean.Parse(checkBox2.Checked.ToString());




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

        public void connect()
        {
            conString = "Server=localhost;port=5432;username='postgres';password='root';database=RentalDB";
            connection = new NpgsqlConnection(conString);
            connection.Open();
        }
        public void update()
        {
            connect();
            NpgsqlCommand selectSpaces = new NpgsqlCommand("SELECT  spacecode, spacename, spacelvl, spacearea, ac, rentprice, rentstatus FROM spaces", connection);
            NpgsqlDataReader reader = selectSpaces.ExecuteReader();

            DataTable rTable = new DataTable();
            rTable.Columns.Add("Идентификатор");
            rTable.Columns.Add("Название площади");
            rTable.Columns.Add("Этаж");
            rTable.Columns.Add("Площадь");
            rTable.Columns.Add("Кондиционер");
            rTable.Columns.Add("Стоимость аренды(день)");
            rTable.Columns.Add(new DataColumn("Статус аренды", typeof(bool)));


            while (reader.Read())
            {
                rTable.Rows.Add(new object[] { reader.GetValue(0), reader.GetValue(1), reader.GetValue(2), reader.GetValue(3), reader.GetValue(4), reader.GetValue(5), reader.GetValue(6)});
            }

            dataGridView1.DataSource = rTable;
            connection.Close();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
