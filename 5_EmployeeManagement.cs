using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyStore
{
    public partial class Employee : Form
    {
        string connection = "Data Source=localhost\\MSSQLSERVER01;Initial Catalog=MyStore;Integrated Security=True";
        public Employee()
        {
            InitializeComponent();
        }   

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string empName = txtEmployeeName.Text;
            string empCode = txtEmployeeID.Text;
            string posID = txtPositionID.Text;
            string phone = txtPhone.Text;
            string accountID = txtAccountID.Text;

            SqlConnection con1 = new SqlConnection(connection);

            if (string.IsNullOrWhiteSpace(empName) || string.IsNullOrWhiteSpace(empCode) || string.IsNullOrWhiteSpace(posID) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(accountID))
            {
                MessageBox.Show("Please input full info", "Announcement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "insert into Employee (EmployeeID, EmployeeName,PositionID,Phone,AccountID)" +
                "values (@EmployeeID, @EmployeeName,@PositionID,@Phone,@AccountID)";
            try
            {
                if (con1.State != ConnectionState.Open)
                {
                    con1.Open();
                }

                using (SqlCommand cmd = new SqlCommand(query, con1))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", empCode);
                    cmd.Parameters.AddWithValue("@EmployeeName", empName);
                    cmd.Parameters.AddWithValue("@PositionID", posID);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@AccountID", accountID);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Product added successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearText();
                        LoadDataE();
                    }
                    else
                    {
                        MessageBox.Show("Unable to add product. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (con1.State == ConnectionState.Open)
                {
                    con1.Close();
                }
            }

        }

        private void Employee_Load(object sender, EventArgs e)
        {
            LoadDataE();
        }

        private void LoadDataE() 
        {
            SqlConnection con2 = new SqlConnection(connection);
            string query = @" select EmployeeID, EmployeeName, PositionID, Phone, AccountID from Employee";

            try
            {
                con2.Open();
                SqlDataAdapter adapter5 = new SqlDataAdapter(query, con2);
                DataTable dtb5 = new DataTable();
                adapter5.Fill(dtb5);
                dgvEmployee.DataSource = dtb5;
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Error! " + ex.Message);
            }
            finally
            {
                con2.Close();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Admin mainForm = new Admin();
            mainForm.Show();
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDataE();
        }

        private void ClearText()
        {
            txtEmployeeID.Clear();
            txtEmployeeName.Clear();
            txtAccountID.Clear();
            txtPhone.Clear();
            txtPositionID.Clear();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string empName = txtEmployeeName.Text;
            string empCode = txtEmployeeID.Text;
            string posID = txtPositionID.Text;
            string phone = txtPhone.Text;
            string accountID = txtAccountID.Text;

            if (string.IsNullOrWhiteSpace(empName) || string.IsNullOrWhiteSpace(empCode) || string.IsNullOrWhiteSpace(posID) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(accountID))
            {
                MessageBox.Show("Please input full info", "Announcement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "update Employee set EmployeeID = @EmployeeID, EmployeeName = @EmployeeName, Phone = @Phone, PositionID = @PositionID, AccountID = @AccountID";
            SqlConnection con3 = new SqlConnection(connection);

            try
            {
                if (con3.State != ConnectionState.Open)
                {
                    con3.Open();
                }
                using (SqlCommand cmd = new SqlCommand(query, con3))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", empCode);
                    cmd.Parameters.AddWithValue("@EmployeeName", empName);
                    cmd.Parameters.AddWithValue("@PositionID", posID);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@AccountID", accountID);

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Update successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearText();
                        LoadDataE();
                    }
                    else
                    {
                        MessageBox.Show("Unable to update. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con3.Close();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            SqlConnection con4 = new SqlConnection(connection);
            string empID = txtEmployeeID.Text;

            if (string.IsNullOrWhiteSpace(empID))
            {
                MessageBox.Show("Please enter the employee ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "DELETE FROM Employee WHERE EmployeeID = @EmployeeID";

            try
            {
                con4.Open();
                using (SqlCommand command = new SqlCommand(query, con4))
                {
                    command.Parameters.AddWithValue("@EmployeeID", empID);

                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Employee deleted successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataE();
                        ClearText();
                    }
                    else
                    {
                        MessageBox.Show("No matching employee found to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con4.Close();
            }
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            string search = txtSearchEmployee.Text;
            SqlConnection con5 = new SqlConnection(connection);
            string query = @" select EmployeeID, EmployeeName, PositionID, Phone, AccountID from Employee";

            try
            {
                con5.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, con5);
                adapter.SelectCommand.Parameters.AddWithValue("@searchValue", "%" + search + "%");
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvEmployee.DataSource = dt;
               
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No product found matching the search criteria.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured: " + ex.Message, "Error",MessageBoxButtons.OK ,MessageBoxIcon.Error);
            }
            finally 
            {
                con5.Close(); 
            }
        }
    }
}
