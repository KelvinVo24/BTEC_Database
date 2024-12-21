
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace MyStore
{
    public partial class LoginForm : Form
    {
        private SqlConnection connection;

        public LoginForm()
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';
            connection = new SqlConnection("Data Source=localhost\\MSSQLSERVER01;Initial Catalog=MyStore;Integrated Security=True");
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();           

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (AuthenticateUser(username, password))
            {
                MessageBox.Show($"Logged in successfully as {Session.UserRole}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (IsFirstLogin(username))
                {
                    MessageBox.Show("You need to change your password on the first login.");
                    ChangePasswordForm changePasswordForm = new ChangePasswordForm(username);
                    changePasswordForm.Show();
                    this.Hide();
                }
                else
                {
                    string role = Session.UserRole;

                    switch (role)
                    {
                        case "admin":
                            {
                                Admin mainForm = new Admin();
                                mainForm.Show();
                                this.Hide();
                                break;
                            }
                        case "sales":
                            {
                                SalesForm salesForm = new SalesForm();
                                salesForm.Show();
                                this.Hide();
                                break;
                            }
                        case "warehouse":
                            {
                                frmWareHouse wareHouse = new frmWareHouse();    
                                wareHouse.Show();
                                this.Hide();
                                break;
                            }
                        default:
                            {
                                MessageBox.Show("Error!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            }
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Incorrect username or password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            string query = @"SELECT A.Password, A.Role, E.EmployeeID 
                     FROM Account A 
                     LEFT JOIN Employee E ON A.AccountID = E.AccountID 
                     WHERE A.Username = @Username";
            bool isAuthenticated = false;

            try
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string storedPassword = reader["Password"].ToString();

                        if (storedPassword == password)
                        {
                            isAuthenticated = true;
                            Session.UserRole = reader["Role"].ToString();
                            Session.EmployeeID = reader["EmployeeID"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return isAuthenticated;
        }


        private bool IsFirstLogin(string username)
        {
            string query = "SELECT IsFirstLogin FROM Account WHERE Username = @Username";
            bool isFirstLogin = false;

            try
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        isFirstLogin = (bool)reader["IsFirstLogin"];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return isFirstLogin;
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            ForCustomer formForCustomer = new ForCustomer();
            formForCustomer.Show();
            this.Hide();
        }
    }
}
