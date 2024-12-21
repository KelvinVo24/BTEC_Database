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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MyStore
{
    public partial class SalesForm : Form
    {
        string connection = "Data Source=localhost\\MSSQLSERVER01;Initial Catalog=MyStore;Integrated Security=True";

        public SalesForm()
        {
            InitializeComponent();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void btnSelling_Click(object sender, EventArgs e)
        {
            Sales sales = new Sales();
            sales.Show();
            this.Close();
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            CustomerForm customerForm = new CustomerForm();
            customerForm.Show();
            this.Close();
        }

        private void SalesForm_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadEmpName();
        }

        private void LoadEmpName()
        {
            using (SqlConnection con = new SqlConnection(connection))
            {
                string employeeID = Session.EmployeeID;

                if (string.IsNullOrEmpty(employeeID))
                {
                    MessageBox.Show("EmployeeID is null or empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string query = "SELECT EmployeeName FROM Employee WHERE EmployeeID = @EmployeeID";

                try
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblEmpName.Text = reader["EmployeeName"].ToString();
                            }
                            else
                            {
                                lblEmpName.Text = "Employee not found.";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void LoadData()
        {
            SqlConnection con = new SqlConnection(connection);
            string query = @"
        SELECT p.ProductID,
               p.ProductName,
               p.SellingPrice,
               c.CategoryName,
               p.InventoryQuantity
        FROM Product as p
        JOIN Category as c ON p.CategoryID = c.CategoryID";

            try
            {
                con.Open();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, con);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                dgvProductSF.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! " + ex.Message);
            } 
            finally
            {
                con.Close();
            }
        }
    } 
}
