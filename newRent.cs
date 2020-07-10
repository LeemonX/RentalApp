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
    public partial class newRent : Form
    {
        public newRent()
        {
            InitializeComponent();
            String conString = "Server=localhost;port=5432;username='postgres';password='root';database=RentalDB";
            NpgsqlConnection connection = new NpgsqlConnection(conString);
            connection.Open();
            NpgsqlCommand selectClients = new NpgsqlCommand("SELECT clientcode, clientname from clients", connection);
            
            NpgsqlDataReader readerc = selectClients.ExecuteReader();
            

            var datac = new List<p1>();

            while (readerc.Read())
            {
                var mc = new p1
                {
                    code = readerc[0].ToString().Trim(),
                    name = readerc[1].ToString().Trim()
                };
                datac.Add(mc);
            }

            comboBox2.DataSource = datac;
            comboBox2.DisplayMember = "name";
            comboBox2.ValueMember = "code";
            connection.Close();

            
            connection.Open();
            NpgsqlCommand selectSpaces = new NpgsqlCommand("SELECT spacecode, spacename from spaces", connection);
            NpgsqlDataReader readers = selectSpaces.ExecuteReader();
            var datas = new List<p1>();
            while (readers.Read())
            {
                var mc = new p1
                {
                    code = readers[0].ToString().Trim(),
                    name = readers[1].ToString().Trim()
                };
                datas.Add(mc);
            }
            
            comboBox1.DataSource = datas;
            comboBox1.DisplayMember = "name";
            comboBox1.ValueMember = "code";
            connection.Close();
        }

        class p1
        {
            public string code { get; set; }
            public string name { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String conString = "Server=localhost;port=5432;username='postgres';password='root';database=RentalDB";
            NpgsqlConnection connection = new NpgsqlConnection(conString);
            connection.Open();

            try
            {
                NpgsqlCommand insertRent = new NpgsqlCommand("INSERT INTO rentedspaces (spacecode, clientcode, startdate, findate) VALUES (@scode, @ccode, @start, @fin)", connection);



                insertRent.Parameters.Add("scode", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse((string)comboBox1.SelectedValue);
                insertRent.Parameters.Add("ccode", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse((string)comboBox2.SelectedValue);
                insertRent.Parameters.Add("start", NpgsqlTypes.NpgsqlDbType.Date).Value = DateTime.Parse((string)dateTimePicker1.Text);
                insertRent.Parameters.Add("fin", NpgsqlTypes.NpgsqlDbType.Date).Value = DateTime.Parse((string)dateTimePicker2.Text);
                insertRent.ExecuteNonQuery();
            }
            catch(Exception)
            {
                MessageBox.Show("Проверьте введенные данные");
            }
            connection.Close();
        }
    }
}
