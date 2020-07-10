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
using XL = Microsoft.Office.Interop.Excel;

namespace RentalApp
{
    public partial class mainMenu : Form
    {
        private String conString;
        private NpgsqlConnection connection;

        public mainMenu()
        {
            InitializeComponent();
            update();
        }

        private void clientMenu_Click(object sender, EventArgs e)
        {
            clientsForm clients = new clientsForm();
            clients.ShowDialog();
        }

        private void spacesMenu_Click(object sender, EventArgs e)
        {
            spacesForm spaces = new spacesForm();
            spaces.ShowDialog();
        }

        private void rentMenu_Click(object sender, EventArgs e)
        {
            newRent createRent = new newRent();
            createRent.ShowDialog();
        }

        private void paymentsMenu_Click(object sender, EventArgs e)
        {
            paymentsForm payments = new paymentsForm();
            payments.ShowDialog();
        }

        private void editData_Click(object sender, EventArgs e)
        {
            connect();
            
                NpgsqlCommand com = new NpgsqlCommand("update rentedspaces set (spacecode, clientcode, startdate, findate) = ( (SELECT spacecode FROM spaces WHERE spacename=@sname), (SELECT clientcode FROM clients WHERE clientname=@cname), @start, @fin) WHERE rentCode=@rcode", connection);

                com.Parameters.Add("rcode", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse((string)dataGridView1.CurrentRow.Cells[0].Value);

                com.Parameters.Add("sname", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[2].Value;
                com.Parameters.Add("cname", NpgsqlTypes.NpgsqlDbType.Varchar).Value = (string)dataGridView1.CurrentRow.Cells[1].Value;
                com.Parameters.Add("start", NpgsqlTypes.NpgsqlDbType.Date).Value = DateTime.Parse((string)dataGridView1.CurrentRow.Cells[4].Value);
                com.Parameters.Add("fin", NpgsqlTypes.NpgsqlDbType.Date).Value = DateTime.Parse((string)dataGridView1.CurrentRow.Cells[5].Value);


                MessageBox.Show("В таблице обновлена запись");
                update();
            
            connection.Close();
        }

        private void deleteData_Click(object sender, EventArgs e)
        {
            connect();
            try
            {
                NpgsqlCommand com = new NpgsqlCommand("delete from rentedspaces where rentcode=@rcode", connection);

                com.Parameters.Add("rcode", NpgsqlTypes.NpgsqlDbType.Integer).Value = int.Parse((string)dataGridView1.CurrentRow.Cells[0].Value);


                com.ExecuteNonQuery();
                MessageBox.Show("В таблице удалена запись");
                connection.Close();
                update();
            }
            catch (Exception)
            {
                MessageBox.Show("Ес");
            }
            
        }

        public void update()
        {
            connect();
            NpgsqlCommand selectRented = new NpgsqlCommand("SELECT r.rentcode, c.clientname, s.spacename, s.rentprice, r.startdate, r.findate FROM clients c, spaces s, rentedspaces r WHERE r.spacecode=s.spacecode and r.clientcode=c.clientcode", connection);

            NpgsqlDataReader reader = selectRented.ExecuteReader();

            DataTable rTable = new DataTable();
            rTable.Columns.Add("Код аренды");
            rTable.Columns.Add("Арендатор");
            rTable.Columns.Add("Название площади");
            rTable.Columns.Add("Стоимость аренды(мес)");
            rTable.Columns.Add("Начало аренды");
            rTable.Columns.Add("Конец аренды");

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

        private void refresh_Click(object sender, EventArgs e)
        {
            update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connect();
            NpgsqlCommand selectRented = new NpgsqlCommand("SELECT r.rentcode, c.clientname, s.spacename, s.rentprice, r.startdate, r.findate FROM clients c, spaces s, rentedspaces r WHERE r.spacecode=s.spacecode and r.clientcode=c.clientcode and clientname LIKE @cname and r.startdate<=@start and @start<=r.findate", connection);

            selectRented.Parameters.AddWithValue("@cname", "%" + textBox1.Text.Trim() + "%");
            selectRented.Parameters.Add("start", NpgsqlTypes.NpgsqlDbType.Date).Value = dateTimePicker1.Value;


            NpgsqlDataReader reader = selectRented.ExecuteReader();

            DataTable rTable = new DataTable();
            rTable.Columns.Add("Код аренды");
            rTable.Columns.Add("Арендатор");
            rTable.Columns.Add("Название площади");
            rTable.Columns.Add("Стоимость аренды(мес)");
            rTable.Columns.Add("Начало аренды");
            rTable.Columns.Add("Конец аренды");

            while (reader.Read())
            {
                rTable.Rows.Add(new object[] { reader.GetValue(0), reader.GetValue(1), reader.GetValue(2), reader.GetValue(3), reader.GetValue(4), reader.GetValue(5) });
            }

            dataGridView1.DataSource = rTable;
            connection.Close();
        }

        private void word_Click(object sender, EventArgs e)
        {
            XL.Application application = new XL.Application();
            application.Workbooks.Add(Type.Missing);
            XL.Worksheet sheet = (XL.Worksheet)application.Sheets[1];
            sheet.Cells[1, 1] = "Май";
            sheet.Cells[1, 2] = 100000;
            XL.ChartObjects xlCharts = (XL.ChartObjects)sheet.ChartObjects(Type.Missing);
            XL.ChartObject myChart = (XL.ChartObject)xlCharts.Add(110, 0, 350, 250);
            XL.Chart chart = myChart.Chart;
            XL.SeriesCollection seriesCollection = (XL.SeriesCollection)chart.SeriesCollection(Type.Missing);
            XL.Series series = seriesCollection.NewSeries();
            chart.ChartType = XL.XlChartType.xlColumnClustered;
            series.XValues = sheet.get_Range("A1", "A10");
            series.Values = sheet.get_Range("B1", "B10");
            chart.Axes(XL.XlAxisType.xlValue).HasTitle = true;
            chart.Axes(XL.XlAxisType.xlValue).AxisTitle.
            Characters.Text = "Уровни продаж";
            chart.Axes(XL.XlAxisType.xlCategory).HasTitle = true;
            chart.Axes(XL.XlAxisType.xlCategory).AxisTitle.
            Characters.Text = "Месяцы";
            chart.HasLegend = false;
            chart.HasTitle = true;
            chart.ChartTitle.Characters.Text = "ПРОДАЖИ ЗА ПЯТЬ МЕСЯЦЕВ";
            application.Visible = true;
        }
    }
}
