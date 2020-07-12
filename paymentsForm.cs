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
    public partial class paymentsForm : Form
    {
        private String conString;
        private NpgsqlConnection connection;
        private bool userrole;
        public paymentsForm(bool userrole)
        {
            InitializeComponent();
            this.userrole = userrole;
            deletePayment.Visible = userrole;
            editPayment.Visible = userrole;
            update();
        }


        private void editPayment_Click(object sender, EventArgs e)
        {
            connect();
            try
            {
                NpgsqlCommand com = new NpgsqlCommand("update payments set (rentcode, paymentdate, paymentamount) = (@rcode, @pdate, @pamount) WHERE paymentCode=@pcode", connection);
                com.Parameters.Add("rcode", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse((string)dataGridView1.CurrentRow.Cells[1].Value);
                com.Parameters.Add("pcode", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse((string)dataGridView1.CurrentRow.Cells[0].Value);
                com.Parameters.Add("pdate", NpgsqlTypes.NpgsqlDbType.Date).Value = DateTime.Parse((string)dataGridView1.CurrentRow.Cells[5].Value);
                com.Parameters.Add("pamount", NpgsqlTypes.NpgsqlDbType.Money).Value = decimal.Parse((string)dataGridView1.CurrentRow.Cells[4].Value);


                com.ExecuteNonQuery();
                MessageBox.Show("В таблице обновлена запись");
                
                update();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Ошибка при обновлении");
            }
            connection.Close();
        }

        private void deletePayment_Click(object sender, EventArgs e)
        {
            connect();
            try
            {
                NpgsqlCommand com = new NpgsqlCommand("delete from payments where paymentcode=@pcode", connection);

                com.Parameters.Add("pcode", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse((string)dataGridView1.CurrentRow.Cells[0].Value);
                com.ExecuteNonQuery();
                MessageBox.Show("В таблице удалена запись");
                update();
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка при удалении");
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
            NpgsqlCommand selectPayments = new NpgsqlCommand("SELECT paymentcode, r.rentcode, c.clientname, s.spacename, p.paymentamount, p.paymentdate, s.rentprice, r.findate-r.startdate, (r.findate-r.startdate)*s.rentprice, r.startdate, r.findate FROM clients c, spaces s, rentedspaces r, payments p WHERE r.rentcode=p.rentcode and r.clientcode=c.clientcode and r.spacecode=s.spacecode", connection);
            NpgsqlDataReader reader = selectPayments.ExecuteReader();

            DataTable rTable = new DataTable();
            rTable.Columns.Add("ID оплаты");
            rTable.Columns.Add("ID аренды");
            rTable.Columns.Add("Арендатор");
            rTable.Columns.Add("Название площади");
            rTable.Columns.Add("Размер платежа");
            rTable.Columns.Add("Дата платежа");
            rTable.Columns.Add("Суточная стоимость");
            rTable.Columns.Add("Срок аренды(дней)");
            rTable.Columns.Add("Полная стоимость");
            rTable.Columns.Add("Начало аренды");
            rTable.Columns.Add("Конец аренды");

            while (reader.Read())
            {
                rTable.Rows.Add(new object[] { reader.GetValue(0), reader.GetValue(1), reader.GetValue(2), reader.GetValue(3), reader.GetValue(4), reader.GetValue(5), reader.GetValue(6), reader.GetValue(7), reader.GetValue(8), reader.GetValue(9), reader.GetValue(10) });
            }

            dataGridView1.DataSource = rTable;
            connection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            newPayment payment = new newPayment();
            payment.ShowDialog();
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            update();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            connect();
            NpgsqlCommand selectPayments = new NpgsqlCommand("SELECT paymentcode, r.rentcode, c.clientname, s.spacename, p.paymentamount, p.paymentdate, s.rentprice, r.findate-r.startdate, (r.findate-r.startdate)*s.rentprice, r.startdate, r.findate FROM clients c, spaces s, rentedspaces r, payments p WHERE r.rentcode=p.rentcode and r.clientcode=c.clientcode and r.spacecode=s.spacecode and (c.clientname LIKE @cname) and @startp <= p.paymentamount and p.paymentamount <= @finp and @startd <= p.paymentdate and p.paymentdate <= @find ", connection);
            try
            {
                selectPayments.Parameters.AddWithValue("@cname", "%" + textBox1.Text.Trim() + "%");
                selectPayments.Parameters.AddWithValue("@startd", dateTimePicker1.Value);
                selectPayments.Parameters.AddWithValue("@find", dateTimePicker2.Value);
                selectPayments.Parameters.Add("@startp", NpgsqlTypes.NpgsqlDbType.Numeric).Value = double.Parse(textBox4.Text.Trim());
                selectPayments.Parameters.Add("@finp", NpgsqlTypes.NpgsqlDbType.Numeric).Value = double.Parse(textBox7.Text.Trim());
                
                NpgsqlDataReader reader = selectPayments.ExecuteReader();

                DataTable rTable = new DataTable();
                rTable.Columns.Add("ID оплаты");
                rTable.Columns.Add("ID аренды");
                rTable.Columns.Add("Арендатор");
                rTable.Columns.Add("Название площади");
                rTable.Columns.Add("Размер платежа");
                rTable.Columns.Add("Дата платежа");
                rTable.Columns.Add("Суточная стоимость");
                rTable.Columns.Add("Срок аренды(дней)");
                rTable.Columns.Add("Полная стоимость");
                rTable.Columns.Add("Начало аренды");
                rTable.Columns.Add("Конец аренды");

                while (reader.Read())
                {
                    rTable.Rows.Add(new object[] { reader.GetValue(0), reader.GetValue(1), reader.GetValue(2), reader.GetValue(3), reader.GetValue(4), reader.GetValue(5), reader.GetValue(6), reader.GetValue(7), reader.GetValue(8), reader.GetValue(9), reader.GetValue(10) });
                }

                dataGridView1.DataSource = rTable;
                connection.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка поиска");
                Console.WriteLine(ex);
            }
            connection.Close();
        }
    }
}
