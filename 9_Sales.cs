using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MyStore
{
    public partial class Sales : Form
    {
        private string connectionString = "Data Source=localhost\\MSSQLSERVER01;Initial Catalog=MyStore;Integrated Security=True";

        public Sales()
        {
            InitializeComponent();
            LoadCustomerIDs();
            LoadProductIDs();
            AddConlumnDgv();
        }

        private void AddConlumnDgv()
        {
            dgvSalesProduct.Columns.Add("ProductID", "Product ID");
            dgvSalesProduct.Columns.Add("ProductName", "Product Name");
            dgvSalesProduct.Columns.Add("Price", "Price");
            dgvSalesProduct.Columns.Add("Quantity", "Quantity");
            dgvSalesProduct.Columns.Add("TotalPrice", "Total Price");
        }

        private void LoadCustomerIDs()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT CustomerID FROM Customer";
                try
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            cboCustomerID.Items.Add(reader["CustomerID"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading Customer IDs: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadProductIDs()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ProductID FROM Product";
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            string productId = cboProductID.Text;
            string productName = txtProductName.Text;
            decimal price = decimal.Parse(txtUnitPrice.Text);
            int quantity = int.Parse(cboQuantity.Text);

            // Thêm sản phẩm vào DataGridView
            int rowIndex = dgvSalesProduct.Rows.Add();
            DataGridViewRow row = dgvSalesProduct.Rows[rowIndex];
            row.Cells["ProductID"].Value = productId;
            row.Cells["ProductName"].Value = productName;
            row.Cells["Price"].Value = price;
            row.Cells["Quantity"].Value = quantity;
            row.Cells["TotalPrice"].Value = price * quantity;

            UpdateTotal();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(cboProductID.Text))
            {
                MessageBox.Show("Please select a Product ID.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(txtProductName.Text))
            {
                MessageBox.Show("Product Name cannot be empty.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out _))
            {
                MessageBox.Show("Price must be a valid decimal number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(cboQuantity.Text) || !int.TryParse(cboQuantity.Text, out _))
            {
                MessageBox.Show("Quantity must be a valid integer.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void UpdateTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dgvSalesProduct.Rows)
            {
                if (row.Cells["TotalPrice"].Value != null)
                {
                    total += Convert.ToDecimal(row.Cells["TotalPrice"].Value);
                }
            }
            txtTotal.Text = total.ToString("N0");
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
                string query = "SELECT ProductName, SellingPrice FROM Product WHERE ProductID = @ProductID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ProductID", productId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtProductName.Text = reader["ProductName"].ToString();
                        txtUnitPrice.Text = reader["SellingPrice"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading product details: " + ex.Message);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvSalesProduct.SelectedRows.Count > 0)
            {
                dgvSalesProduct.Rows.RemoveAt(dgvSalesProduct.SelectedRows[0].Index);
                UpdateTotal();
            }
            else
            {
                MessageBox.Show("Please select a product to delete.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnPurchase_Click(object sender, EventArgs e)
        {
            if (dgvSalesProduct.Rows.Count == 0)
            {
                MessageBox.Show("The cart is empty. Please add products.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to complete the purchase?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                SaveOrderToDatabase();
                MessageBox.Show("Purchase completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dgvSalesProduct.Rows.Clear();
                txtTotal.Text = "0";
            }
        }

        // Save the order details into the database using a transaction
        private void SaveOrderToDatabase()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Insert into SalesOrder table
                    string salesOrderId = GenerateNextSalesOrderID();
                    string customerId = cboCustomerID.Text; // Retrieve CustomerID from ComboBox
                    string employeeId = Session.EmployeeID;
                    decimal totalAmount = decimal.Parse(txtTotal.Text);

                    string salesOrderQuery = @"INSERT INTO SalesOrder (SalesOrderID, CustomerID, EmployeeID, OrderDate, TotalAmount) 
                                               VALUES (@SalesOrderID, @CustomerID, @EmployeeID, @OrderDate, @TotalAmount)";
                    using (SqlCommand cmdOrder = new SqlCommand(salesOrderQuery, connection, transaction))
                    {
                        cmdOrder.Parameters.AddWithValue("@SalesOrderID", salesOrderId);
                        cmdOrder.Parameters.AddWithValue("@CustomerID", customerId);
                        cmdOrder.Parameters.AddWithValue("@EmployeeID", employeeId);
                        cmdOrder.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                        cmdOrder.Parameters.AddWithValue("@TotalAmount", totalAmount);
                        cmdOrder.ExecuteNonQuery();
                    }

                    // Insert into SalesOrderDetail table
                    foreach (DataGridViewRow row in dgvSalesProduct.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            string productId = row.Cells["ProductID"].Value.ToString();
                            int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                            decimal unitPrice = Convert.ToDecimal(row.Cells["Price"].Value);

                            string salesOrderDetailQuery = @"INSERT INTO SalesOrderDetail (SalesOrderID, ProductID, Quantity, UnitPrice) 
                                                             VALUES (@SalesOrderID, @ProductID, @Quantity, @UnitPrice)";
                            using (SqlCommand cmdDetail = new SqlCommand(salesOrderDetailQuery, connection, transaction))
                            {
                                cmdDetail.Parameters.AddWithValue("@SalesOrderID", salesOrderId);
                                cmdDetail.Parameters.AddWithValue("@ProductID", productId);
                                cmdDetail.Parameters.AddWithValue("@Quantity", quantity);
                                cmdDetail.Parameters.AddWithValue("@UnitPrice", unitPrice);
                                cmdDetail.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error saving order: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cboCustomerID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCustomerID.SelectedItem != null)
            {
                string customerId = cboCustomerID.SelectedItem.ToString();
                FillCustomerName(customerId);
            }
        }

        private void FillCustomerName(string customerId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT CustomerName FROM Customer WHERE CustomerID = @CustomerID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@CustomerID", customerId);

                try
                {
                    connection.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        txtCustomerName.Text = result.ToString();
                    }
                    else
                    {
                        txtCustomerName.Clear();
                        MessageBox.Show("Customer not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading Customer Name: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string GenerateNextSalesOrderID()
        {
            string latestOrderID = null;
            string newOrderID = null;
            string query = "SELECT TOP 1 SalesOrderID FROM SalesOrder ORDER BY SalesOrderID DESC";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        latestOrderID = result.ToString();

                        string numericPart = latestOrderID.Substring(3);
                        int nextNumber = int.Parse(numericPart) + 1;
                        newOrderID = "SAL" + nextNumber.ToString("D3");
                    }
                    else
                    {
                        newOrderID = "SAL001";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error generating SalesOrderID: " + ex.Message);
                }
            }
            return newOrderID;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            string role = Session.UserRole;
            if (role == "admin")
            {
                Admin mainForm = new Admin();
                mainForm.Show();
                this.Close();
            }
            else if (role == "sales")
            {
                SalesForm salesForm = new SalesForm();
                salesForm.Show();
                this.Close();
            }
            else 
            {
                MessageBox.Show("Error","Warning", MessageBoxButtons.OK);
            }

        }
    }
}

