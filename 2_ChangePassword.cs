using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyStore
{
    public partial class ChangePasswordForm : Form
    {
        private SqlConnection connection;
        private string username;

        public ChangePasswordForm(string user)
        {
            InitializeComponent();
            username = user;
            connection = new SqlConnection("Data Source=localhost\\MSSQLSERVER01;Initial Catalog=MyStore;Integrated Security=True");
            txtNewPassword.PasswordChar = '*';
            txtConfirmPassword.PasswordChar = '*';
        }        

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (newPassword == confirmPassword)
            {
                string query = "UPDATE Account SET Password = @NewPassword, IsFirstLogin = 0 WHERE Username = @Username";

                try
                {
                    // Kiểm tra nếu connection chưa được khởi tạo
                    if (connection == null)
                    {
                        connection = new SqlConnection("Data Source=localhost\\MSSQLSERVER01;Initial Catalog=MyStore;Integrated Security=True");
                    }

                    connection.Open();

                    if (connection.State == ConnectionState.Open)
                    {
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@NewPassword", newPassword);
                            cmd.Parameters.AddWithValue("@Username", username);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Password has been successfully changed!");

                        this.Hide();
                        LoginForm loginForm = new LoginForm();
                        loginForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Connection to the database could not be established.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            else
            {
                lblMessage.Text = "Passwords do not match!";
            }
        }




    }
}
