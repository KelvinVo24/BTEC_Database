using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyStore
{
    public partial class Import : Form
    {
        string connectionString = "Data Source=localhost\\MSSQLSERVER01;Initial Catalog=MyStore;Integrated Security=True;";

        public Import()
        {
            InitializeComponent();
            txtEmployeeID.Text = Session.EmployeeID;
        }

        private void Import_Load(object sender, EventArgs e)
        {
            InitializeImportDetailsTable();
            LoadProductIDs();
            LoadSupplierIDs();
        }

        private void LoadProductIDs()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string query = "SELECT ProductID FROM Product";

            using (connection)
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            cboProductID.Items.Add(reader["ProductID"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading Product IDs: " + ex.Message);
                }
            }
        }

        private void LoadSupplierIDs()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string query = "SELECT SupplierID FROM Supplier";

            using (connection)
            {
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            cboSupplierID.Items.Add(reader["SupplierID"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading Supplier IDs: " + ex.Message);
                }
            }
        }


        private DataTable importDetailsTable;
        
        private void InitializeImportDetailsTable()
        {
            importDetailsTable = new DataTable();
            importDetailsTable.Columns.Add("ProductID", typeof(string));
            importDetailsTable.Columns.Add("ProductName", typeof(string));
            importDetailsTable.Columns.Add("Quantity", typeof(int));
            importDetailsTable.Columns.Add("UnitPrice", typeof(decimal));
            importDetailsTable.Columns.Add("TotalPrice", typeof(decimal), "Quantity * UnitPrice");

            dgvImport.DataSource = importDetailsTable;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string productId = cboProductID.Text;
            string productName = txtProductName.Text;
            int quantity = 0;
            decimal unitPrice;

            if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(productName) ||
                !int.TryParse(txtQuantity.Text, out quantity) || !decimal.TryParse(txtUnitPrice.Text, out unitPrice))
            {
                MessageBox.Show("Please enter valid product details.");
                return;
            }

            DataRow row = importDetailsTable.NewRow();
            row["ProductID"] = productId;
            row["ProductName"] = productName;
            row["Quantity"] = quantity;
            row["UnitPrice"] = unitPrice;
            importDetailsTable.Rows.Add(row);

            ClearField_I();
        }
        private void ClearField_I()
        {

            cboProductID.SelectedIndex = -1;
            txtProductName.Clear();
            txtQuantity.Clear();
            txtUnitPrice.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string importOrderId = GenerateNextImportOrderID();
            string supplierId = cboSupplierID.Text;
            string employeeId = txtEmployeeID.Text;
            DateTime importDate = dtpImportDate.Value;

            if (string.IsNullOrWhiteSpace(importOrderId) || string.IsNullOrWhiteSpace(supplierId) ||
                string.IsNullOrWhiteSpace(employeeId))
            {
                MessageBox.Show("Please enter all required fields.");
                return;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                
                    string importOrderQuery = "INSERT INTO ImportOrder (ImportOrderID, SupplierID, EmployeeID, ImportDate, TotalAmount) " +
                                              "VALUES (@ImportOrderID, @SupplierID, @EmployeeID, @ImportDate, @TotalAmount)";
                    SqlCommand cmdOrder = new SqlCommand(importOrderQuery, connection, transaction);
                    cmdOrder.Parameters.AddWithValue("@ImportOrderID", importOrderId);
                    cmdOrder.Parameters.AddWithValue("@SupplierID", supplierId);
                    cmdOrder.Parameters.AddWithValue("@EmployeeID", employeeId);
                    cmdOrder.Parameters.AddWithValue("@ImportDate", importDate);
                    cmdOrder.Parameters.AddWithValue("@TotalAmount", CalculateTotalAmount());
                    cmdOrder.ExecuteNonQuery();

              
                    foreach (DataRow row in importDetailsTable.Rows)
                    {
                        string importDetailQuery = "INSERT INTO ImportOrderDetail (ImportOrderID, ProductID, Quantity, UnitPrice) " +
                                                   "VALUES (@ImportOrderID, @ProductID, @Quantity, @UnitPrice)";
                        SqlCommand cmdDetail = new SqlCommand(importDetailQuery, connection, transaction);
                        cmdDetail.Parameters.AddWithValue("@ImportOrderID", importOrderId);
                        cmdDetail.Parameters.AddWithValue("@ProductID", row["ProductID"]);
                        cmdDetail.Parameters.AddWithValue("@Quantity", row["Quantity"]);
                        cmdDetail.Parameters.AddWithValue("@UnitPrice", row["UnitPrice"]);
                        cmdDetail.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Import order saved successfully.");
                    ClearForm();
                    InitializeImportDetailsTable();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error saving import order: " + ex.Message);
                }
            }
        }

        private void ClearForm()
        {
            txtSupplierName.Clear();
            cboSupplierID.SelectedIndex = -1;
            txtEmployeeID.Clear();
            dtpImportDate.Value = DateTime.Now;
            importDetailsTable.Clear();
        }

        private decimal CalculateTotalAmount()
        {
            decimal total = 0;
            foreach (DataRow row in importDetailsTable.Rows)
            {
                total += (decimal)row["TotalPrice"];
            }
            return total;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {   string role = Session.UserRole;
            switch (role)
            {
                case "admin":
                    {
                        Admin mainForm = new Admin();
                        mainForm.Show();
                        this.Close();
                        break;
                    }
                case "warehouse":
                    {
                        frmWareHouse wareHouse = new frmWareHouse();
                        wareHouse.Show();
                        this.Close();
                        break;
                    }
                default:
                    {
                        MessageBox.Show("Error!","Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
            }
        }

        private void cboProductID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProductID.SelectedItem != null)
            {
                string productId = cboProductID.SelectedItem.ToString();
                FillProductDetails(productId);
            }
        }

        private void FillProductDetails(string productId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                p.ProductName,
                COALESCE(
                    (SELECT TOP 1 iod.UnitPrice 
                     FROM ImportOrderDetail iod
                     INNER JOIN ImportOrder io ON iod.ImportOrderID = io.ImportOrderID
                     WHERE iod.ProductID = @ProductID
                     ORDER BY io.ImportDate DESC), 
                     p.SellingPrice / (1 + p.MarkupPercentage / 100.0)
                ) AS UnitPrice
            FROM Product p
            WHERE p.ProductID = @ProductID";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ProductID", productId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtProductName.Text = reader["ProductName"].ToString();
                        txtUnitPrice.Text = Convert.ToDecimal(reader["UnitPrice"]).ToString("N2");
                    }
                    else
                    {
                        txtProductName.Clear();
                        txtUnitPrice.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading Product details: " + ex.Message);
                }
            }
        }


        private string GenerateNextImportOrderID()
        {
            string latestImportOrderID = null;
            string newImportOrderID = null;
            string query = "SELECT TOP 1 ImportOrderID FROM ImportOrder ORDER BY ImportOrderID DESC";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        latestImportOrderID = result.ToString();
                        string numericPart = latestImportOrderID.Substring(3);
                        int nextNumber = int.Parse(numericPart) + 1;
                        newImportOrderID = "IMP" + nextNumber.ToString("D3");
                    }
                    else
                    {
                        newImportOrderID = "IMP001";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error generating ImportOrderID: " + ex.Message);
                }
            }
            return newImportOrderID;
        }


        private void cboSupplierID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSupplierID.SelectedItem != null)
            {
                string supplierId = cboSupplierID.SelectedItem.ToString();
                FillSupplierName(supplierId);
            }
        }

        private void FillSupplierName(string supplierId)
        {
            string query = "SELECT SupplierName FROM Supplier WHERE SupplierID = @SupplierID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SupplierID", supplierId);
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            txtSupplierName.Text = result.ToString();
                        }
                        else
                        {
                            txtSupplierName.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading Supplier Name: " + ex.Message);
                }
            }
        }

    }
}
