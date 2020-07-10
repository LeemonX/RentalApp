using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace RentalApp
{
    public partial class authForm : Form
    {
        public authForm()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            String username = login.Text;
            String pass = password.Text;

            String conString = "Server=localhost;port=5432;username='postgres';password='root';database=RentalDB";
            NpgsqlConnection connection = new NpgsqlConnection(conString);


            connection.Open();

            NpgsqlCommand logonCommand = new NpgsqlCommand("SELECT username, pass, userrole FROM users WHERE username=@username and pass=@pass", connection);
            logonCommand.Parameters.Add("username", NpgsqlTypes.NpgsqlDbType.Varchar, 30).Value = username;
            logonCommand.Parameters.Add("pass", NpgsqlTypes.NpgsqlDbType.Varchar, 30).Value = pass;
            NpgsqlDataReader reader = logonCommand.ExecuteReader();

            int temp = 0;
            while (reader.Read())
            {
                temp++;
            }
            if(temp == 1)
            {
                this.Hide();
                mainMenu menu = new mainMenu();
                menu.ShowDialog();
            }
            else
            {
                loginFail.Text = "Неверный логин или пароль";
            }
        }

        private void authForm_Load(object sender, EventArgs e)
        {

        }
    }
}
