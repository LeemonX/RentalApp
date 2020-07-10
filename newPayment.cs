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
    public partial class newPayment : Form
    {
        public newPayment()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String conString = "Server=localhost;port=5432;username='postgres';password='root';database=RentalDB";
            NpgsqlConnection connection = new NpgsqlConnection(conString);
            connection.Open();
            try
            {
                NpgsqlCommand insertPayment = new NpgsqlCommand("INSERT INTO payments(rentcode, clientcode, paymentdate, paymentamount) VALUES (@rcode, (SELECT clientcode from rentedSpaces WHERE rentCode=@rcode), @pdate, @pamount)", connection);

                insertPayment.Parameters.Add("rcode", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse(textBox1.Text.Trim());
                insertPayment.Parameters.Add("pamount", NpgsqlTypes.NpgsqlDbType.Money).Value = int.Parse(textBox2.Text.Trim());
                insertPayment.Parameters.Add("pdate", NpgsqlTypes.NpgsqlDbType.Date).Value = DateTime.Parse((string)dateTimePicker1.Text);
            }

            catch (Exception)
            {
                MessageBox.Show("Проверьте введенные данные");
            }


            connection.Close();
        }
    }
}
