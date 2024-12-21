using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;
using System.Diagnostics;

namespace MyStore
{
    public partial class ProductManagement : Form
    {
        string connection = "Data Source=localhost\\MSSQLSERVER01;Initial Catalog=MyStore;Integrated Security=True";
        public ProductManagement()
        {
            InitializeComponent();
        }

        private void ProductManagement_Load(object sender, EventArgs e)
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
                dgvProduct.DataSource = dataTable;
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


        private void ClearTextbox()
        {
            txtProductCode.Clear();
            txtProductName.Clear();
            txtPrice.Clear();
            txtCategory.Clear();
            txtQuantity.Clear();
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(connection); 
            String productId = txtProductCode.Text;
            String productName = txtProductName.Text;
            Decimal sellingPrice;
            String categoryId = txtCategory.Text;
            int inventoryQuantity;

            if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(categoryId) ||
                !decimal.TryParse(txtPrice.Text, out sellingPrice) || !int.TryParse(txtQuantity.Text, out inventoryQuantity))
            {
                MessageBox.Show("Please input full info", "Announcement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "INSERT INTO Product (ProductID, ProductName, SellingPrice, CategoryID, InventoryQuantity) " +
                           "VALUES (@ProductID, @ProductName, @SellingPrice, @CategoryID, @InventoryQuantity)";
            try
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@ProductID", productId);
                    command.Parameters.AddWithValue("@ProductName", productName);
                    command.Parameters.AddWithValue("@SellingPrice", sellingPrice);
                    command.Parameters.AddWithValue("@CategoryID", categoryId);
                    command.Parameters.AddWithValue("@InventoryQuantity", inventoryQuantity);

                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Product added successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearTextbox();
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Unable to add product. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }
        }




        private void btnEditProduct_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(connection);
            string productId = txtProductCode.Text;
            string productName = txtProductName.Text;
            decimal sellingPrice;
            string categoryId = txtCategory.Text;
            int inventoryQuantity;

            if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(productName) || string.IsNullOrWhiteSpace(categoryId) ||
                !decimal.TryParse(txtPrice.Text, out sellingPrice) || !int.TryParse(txtQuantity.Text, out inventoryQuantity))
            {
                MessageBox.Show("Please fill in all information and ensure valid data.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "UPDATE Product SET ProductName = @ProductName, SellingPrice = @SellingPrice, CategoryID = @CategoryID, InventoryQuantity = @InventoryQuantity WHERE ProductID = @ProductID";

            try
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@ProductID", productId);
                    command.Parameters.AddWithValue("@ProductName", productName);
                    command.Parameters.AddWithValue("@SellingPrice", sellingPrice);
                    command.Parameters.AddWithValue("@CategoryID", categoryId);
                    command.Parameters.AddWithValue("@InventoryQuantity", inventoryQuantity);

                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Product updated successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Unable to update product.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(connection);
            string productId = txtProductCode.Text;

            if (string.IsNullOrWhiteSpace(productId))
            {
                MessageBox.Show("Please enter the product ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "DELETE FROM Product WHERE ProductID = @ProductID";

            try
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@ProductID", productId);

                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Product deleted successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                        ClearTextbox();
                    }
                    else
                    {
                        MessageBox.Show("No matching product found to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void btnSearchProduct_Click(object sender, EventArgs e)
        {
            SqlConnection con5 = new SqlConnection(connection);
            string searchValue = txtSearch.Text;
            string query = "SELECT ProductID, ProductName, SellingPrice, CategoryID, InventoryQuantity FROM Product WHERE ProductName LIKE @searchValue OR ProductID LIKE @searchValue";

            try
            {
                con5.Open();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, con5);
                dataAdapter.SelectCommand.Parameters.AddWithValue("@searchValue", "%" + searchValue + "%");
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                dgvProduct.DataSource = dataTable;

                if (dataTable.Rows.Count == 0)
                {
                    MessageBox.Show("No product found matching the search criteria.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con5.Close();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearTextbox();
            LoadData();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {  
            Admin mainForm = new Admin();
            mainForm.Show();
            this.Close();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            Import import = new Import();
            import.Show();
            this.Close();
        }
    }
}
