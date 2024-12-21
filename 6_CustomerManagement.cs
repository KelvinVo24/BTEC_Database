using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyStore
{
    public partial class CustomerForm : Form
    {
        string connection = "Data Source=localhost\\MSSQLSERVER01;Initial Catalog=MyStore;Integrated Security=True";

        public CustomerForm()
        {
            InitializeComponent();
        }

        private void CustomerForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            SqlConnection con = new SqlConnection(connection);
            string query = "SELECT CustomerID, CustomerName, Phone FROM Customer";

            try
            {
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvCustomer.DataSource = dt;
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string customerID = txtCustomerID.Text;
            string customerName = txtCustomerName.Text;
            string phone = txtPhone.Text;

            SqlConnection con = new SqlConnection(connection);

            if (string.IsNullOrWhiteSpace(customerID) || string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Please input full info", "Announcement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "INSERT INTO Customer (CustomerID, CustomerName, Phone) VALUES (@CustomerID, @CustomerName, @Phone)";

            try
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerID);
                    cmd.Parameters.AddWithValue("@CustomerName", customerName);
                    cmd.Parameters.AddWithValue("@Phone", phone);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Customer added successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearFields();
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Unable to add customer. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string customerID = txtCustomerID.Text;
            string customerName = txtCustomerName.Text;
            string phone = txtPhone.Text;

            if (string.IsNullOrWhiteSpace(customerID) || string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Please input full info", "Announcement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "UPDATE Customer SET CustomerName = @CustomerName,  Phone = @Phone WHERE CustomerID = @CustomerID";
            SqlConnection con = new SqlConnection(connection);

            try
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerID);
                    cmd.Parameters.AddWithValue("@CustomerName", customerName);
                    cmd.Parameters.AddWithValue("@Phone", phone);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Customer updated successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearFields();
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Unable to update customer. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string customerID = txtCustomerID.Text;

            if (string.IsNullOrWhiteSpace(customerID))
            {
                MessageBox.Show("Please enter the customer code.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "DELETE FROM Customer WHERE CustomerID = @CustomerID";
            SqlConnection con = new SqlConnection(connection);

            try
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerID);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Customer deleted successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearFields();
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("No matching customer found to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchValue = txtSearch.Text;
            string query = "SELECT CustomerID, CustomerName, Phone FROM Customer WHERE CustomerName LIKE @searchValue OR CustomerID LIKE @searchValue";
            SqlConnection con = new SqlConnection(connection);

            try
            {
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                adapter.SelectCommand.Parameters.AddWithValue("@searchValue", "%" + searchValue + "%");
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvCustomer.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No customer found matching the search criteria.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearFields();
            LoadData();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            string role = Session.UserRole;

            switch (role)
            {
                case "admin":
                    {
                        Admin mainForm = new Admin();
                        mainForm.Show();
                        this.Close();
                        break;
                    }
                case "sales":
                    {
                        SalesForm salesForm = new SalesForm();  
                        salesForm.Show();
                        this.Close();
                        break;
                    }
                default:
                    {
                        MessageBox.Show("Error", "Warning", MessageBoxButtons.OK);
                        break;
                    }
            }
        }

        private void ClearFields()
        {
            txtCustomerID.Clear();
            txtCustomerName.Clear();
            txtPhone.Clear();
        }
    }
}