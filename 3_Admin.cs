using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyStore
{
    public partial class Admin : Form
    {
        public Admin()
        {
            InitializeComponent();
            Authorize();
        }

        private void Authorize()
        {   string role = Session.UserRole;
            switch (role) 
            {
                case "admin":
                    // Admin can access everything
                    btnProductManagement.Enabled = true;
                    btnEmployeeManagement.Enabled = true;
                    btnCustomerManagement.Enabled = true;
                    btnStatistics.Enabled = true;
                    break;

                case "sales":
                    // Sales staff can access customer and product management, but not employee or statistics
                    btnProductManagement.Enabled = true;
                    btnEmployeeManagement.Enabled = false;
                    btnCustomerManagement.Enabled = true;
                    btnStatistics.Enabled = false;
                    break;

                case "warehouse":
                    // Warehouse staff can only access product management
                    btnProductManagement.Enabled = true;
                    btnEmployeeManagement.Enabled = false;
                    btnCustomerManagement.Enabled = false;
                    btnStatistics.Enabled = false;
                    break;

                default:
                    // In case the role is undefined, disable all buttons
                    btnProductManagement.Enabled = false;
                    btnEmployeeManagement.Enabled = false;
                    btnCustomerManagement.Enabled = false;
                    btnStatistics.Enabled = false;
                    break;
            }
        }

        private void btnProductManagement_Click(object sender, EventArgs e)
        {
            ProductManagement productManagement = new ProductManagement();
            productManagement.Show();
            this.Hide();
        }

        private void btnEmployeeManagement_Click(object sender, EventArgs e)
        {
            Employee employee = new Employee();
            employee.Show();
            this.Hide();
        }

        private void btnCustomerManagement_Click(object sender, EventArgs e)
        {
            CustomerForm customerForm = new CustomerForm();
            customerForm.Show();
            this.Hide();
        }

        private void btnStatistics_Click(object sender, EventArgs e)
        {
            StatisticsForm statisticsForm = new StatisticsForm();
            statisticsForm.Show();
            this.Hide();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            Import import = new Import();
            import.Show();
            this.Hide();
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            Sales sales = new Sales();
            sales.Show();
            this.Hide();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            Session.UserRole = "";
            Session.EmployeeID = "";

            LoginForm loginForm = new LoginForm();  
            loginForm.Show();
            this.Close();
        }
    }
}
