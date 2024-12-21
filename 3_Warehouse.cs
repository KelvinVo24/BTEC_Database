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
    public partial class frmWareHouse : Form
    {
        string connection = "Data Source=localhost\\MSSQLSERVER01;Initial Catalog=MyStore;Integrated Security=True";
        public frmWareHouse()
        {
            InitializeComponent();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            Import import = new Import();   
            import.Show();
            this.Close();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();     
            
            Session.ClearToLogin();
        }

        private void frmWareHouse_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            SqlConnection con = new SqlConnection(connection);
            string query = @"
        SELECT p.ProductID,
               p.ProductName,
               p.SellingPrice,
               p.CategoryID,
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
                dgvInventory.DataSource = dataTable;
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
