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
    public partial class ForCustomer : Form
    {
        string connection = "Data Source=localhost\\MSSQLSERVER01;Initial Catalog=MyStore;Integrated Security=True";

        public ForCustomer()
        {
            InitializeComponent();
        }
        private void LoadPurchaseHistory(string customerID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            so.SalesOrderID AS 'Order ID',
                            so.OrderDate AS 'Order Date',
                            p.ProductName AS 'Product',
                            sod.Quantity AS 'Quantity',
                            sod.UnitPrice AS 'Unit Price',
                            (sod.Quantity * sod.UnitPrice) AS 'Total'
                        FROM 
                            SalesOrder so
                        INNER JOIN 
                            SalesOrderDetail sod ON so.SalesOrderID = sod.SalesOrderID
                        INNER JOIN 
                            Product p ON sod.ProductID = p.ProductID
                        WHERE 
                            so.CustomerID = @CustomerID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CustomerID", customerID);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        dgvPurchaseHistory.DataSource = dt;
                    }
                    else
                    {
                        dgvPurchaseHistory.DataSource = null;
                        lblCustomerName.Text = "No purchase history found.";
                        MessageBox.Show("No purchase history found for the selected customer.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching purchase history.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string customerID = txtSearch.Text;

            if (string.IsNullOrEmpty(customerID))
            {
                MessageBox.Show("Please enter a Customer ID to search.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();
                    string query = "SELECT CustomerName FROM Customer WHERE CustomerID = @CustomerID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CustomerID", customerID);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        lblCustomerName.Text = result.ToString();
                        LoadPurchaseHistory(customerID);
                    }
                    else
                    {
                        lblCustomerName.Text = "Customer not found.";
                        dgvPurchaseHistory.DataSource = null;
                        MessageBox.Show("Customer not found.", "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching customer data.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }
    }
}

