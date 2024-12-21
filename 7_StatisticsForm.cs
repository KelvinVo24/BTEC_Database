using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MyStore
{
    public partial class StatisticsForm : Form
    {
        String connectionString = "Data Source=localhost\\MSSQLSERVER01;Initial Catalog=MyStore;Integrated Security=True";
        public StatisticsForm()
        {
            InitializeComponent();
            LoadStatisticsOptions();
        }

        private void LoadStatisticsOptions()
        {
            cboStatisticType.Items.Add("Revenue");
            cboStatisticType.Items.Add("Profit");
            cboStatisticType.Items.Add("Sales by Product");
            cboStatisticType.Items.Add("Sales by Employee");
            cboStatisticType.Items.Add("Import Statistics");
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            cboStatisticType.SelectedIndex = -1;
            dtpStartDate.Value = DateTime.Now;
            dtpEndDate.Value = DateTime.Now;
            dgvStatistics.DataSource = null;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Admin mainForm = new Admin();
            mainForm.Show();
            this.Close();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateStatistics();
        }

        private void GenerateStatistics()
        {
            string statisticsType = cboStatisticType.SelectedItem?.ToString();
            DateTime startDate = dtpStartDate.Value;
            DateTime endDate = dtpEndDate.Value;

            string query = "";

            switch (statisticsType)
            {
                case "Revenue":
                    query = @"SELECT SalesOrderID, OrderDate, TotalAmount 
              FROM SalesOrder 
              WHERE OrderDate BETWEEN @StartDate AND @EndDate";
                    break;

                case "Profit":
                    query = @"SELECT ProfitID, RecordDate, Profit 
              FROM ProfitStatistics 
              WHERE RecordDate BETWEEN @StartDate AND @EndDate";
                    break;

                case "Sales by Product":
                    query = @"SELECT ProductID, SUM(Quantity) AS TotalQuantity, SUM(UnitPrice * Quantity) AS TotalRevenue
              FROM SalesOrderDetail 
              WHERE SalesOrderID IN (SELECT SalesOrderID FROM SalesOrder WHERE OrderDate BETWEEN @StartDate AND @EndDate)
              GROUP BY ProductID";
                    break;

                case "Sales by Employee":
                    query = @"SELECT EmployeeID, COUNT(SalesOrderID) AS SalesCount, SUM(TotalAmount) AS TotalSales 
              FROM SalesOrder 
              WHERE OrderDate BETWEEN @StartDate AND @EndDate
              GROUP BY EmployeeID";
                    break;

                case "Import Statistics":
                    query = @"SELECT 
                        io.ImportOrderID AS [Order ID],
                        s.SupplierName AS [Supplier],
                        e.EmployeeName AS [Handled By],
                        io.ImportDate AS [Import Date],
                        iod.ProductID AS [Product ID],
                        p.ProductName AS [Product Name],
                        iod.Quantity AS [Quantity],
                        iod.UnitPrice AS [Unit Price],
                        iod.Quantity * iod.UnitPrice AS [Total Price]
                      FROM ImportOrder io
                      INNER JOIN ImportOrderDetail iod ON io.ImportOrderID = iod.ImportOrderID
                      INNER JOIN Product p ON iod.ProductID = p.ProductID
                      INNER JOIN Supplier s ON io.SupplierID = s.SupplierID
                      INNER JOIN Employee e ON io.EmployeeID = e.EmployeeID
                      WHERE io.ImportDate BETWEEN @StartDate AND @EndDate";
                    break;

                default:
                    MessageBox.Show("Please select a valid statistics type.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
            }

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dgvStatistics.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
