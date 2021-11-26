using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Data.SQLite;
using uchet.Connection;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Controls;
using System.Reflection;

namespace uchet
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window

    {
        public MainWindow()
        {
            InitializeComponent();
            DisplayData();
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddEmpl addEmpl = new AddEmpl();
            addEmpl.Owner = this;
            bool? result = addEmpl.ShowDialog();
            switch (result)
            {
                default:
                    DisplayData();
                    break;
            }


        }

        private void BtnTrack_Click(object sender, RoutedEventArgs e)
        {
            Tracker tracker = new Tracker();
            tracker.Owner = this;
            tracker.ShowDialog();
        }
        public void DisplayData()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {
                try
                {
                    connection.Open();
                    string query = $@"SELECT Employee.id, Employee.FirstName AS FN, Employee.SecondName AS SN, Employee.MiddleName AS MN, Employee.Dateofbirth AS DoB, Employee.Phone AS Phone, Position.Name AS Post, Stat.Status FROM Employee INNER JOIN Position on Employee.idPost = Position.id INNER JOIN Stat on Employee.idStatus = Stat.id";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    DataTable DT = new DataTable("Employee");
                    SQLiteDataAdapter SDA = new SQLiteDataAdapter(cmd);
                    SDA.Fill(DT);
                    DGAllEmp.ItemsSource = DT.DefaultView;
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }


            }
        }

        public void Delete()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {
                try
                {

                    foreach (var item in DGAllEmp.SelectedItems.Cast<DataRowView>())
                    {
                        string query1 = $@"DELETE FROM Employee WHERE id = " + item["ID"];
                        connection.Open();

                        SQLiteCommand cmd1 = new SQLiteCommand(query1, connection);
                        DataTable DT = new DataTable("Employee");
                        cmd1.ExecuteNonQuery();
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }


            }
        }
        private void BtnUpd_Click(object sender, RoutedEventArgs e)
        {
            DisplayData();
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            Delete();
            DisplayData();
        }

        private void BtnExp_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Excel.Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet sheet1 = (Excel.Worksheet)workbook.Sheets[1];
            sheet1.Name = "Отчет сотрудников " + DateTime.Now.Date.ToString("dd/MM/yyyy");
            Excel.Range myRange1 = sheet1.get_Range("A1", "G1");
            myRange1.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            myRange1.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            myRange1.Merge(Type.Missing);
            myRange1.Font.Name = "Times New Roman";
            myRange1.Font.Bold = true;
            myRange1.Cells.Font.Size = 16;
            sheet1.Range["A1"].Value = "Отчёт был создан в программе EMACC " + DateTime.Now.Date.ToString("dd/MM/yyyy");
            for (int j = 0; j < DGAllEmp.Columns.Count; j++)
            {
                Excel.Range myRange = (Excel.Range)sheet1.Cells[2, j + 1];
                myRange.Font.Bold = true;
                sheet1.Columns[j + 1].ColumnWidth = 15;
                myRange.Value2 = DGAllEmp.Columns[j].Header;
            }
            for (int i = 0; i < DGAllEmp.Columns.Count; i++)
            {
                for (int j = 0; j < DGAllEmp.Items.Count; j++)
                {
                    TextBlock b = DGAllEmp.Columns[i].GetCellContent(DGAllEmp.Items[j]) as TextBlock;
                    Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 3, i + 1];
                    myRange.Value2 = b.Text;
                }
            }
        }

        private void DGAllEmp_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DGAllEmp.SelectedIndex != -1)
            {
                EditEmpl editEmpl = new EditEmpl((DataRowView)DGAllEmp.SelectedItem);
                editEmpl.Owner = this;
                bool? result = editEmpl.ShowDialog();
                switch (result)
                {
                    default:
                        DisplayData();
                        break;
                }
            }
        }
        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TbSearch.Text == "")
            {
                DisplayData();
            }
            else
            {
                using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
                {
                    try
                    {
                        connection.Open();
                        string query = $@"SELECT Employee.id, Employee.FirstName AS FN, Employee.SecondName AS SN, Employee.MiddleName AS MN, Employee.Dateofbirth AS DoB, 
                                        Employee.Phone AS Phone, Position.Name AS Post, Stat.Status 
                                        FROM Employee INNER JOIN Position on Employee.idPost = Position.id 
                                        INNER JOIN Stat on Employee.idStatus = Stat.id WHERE SecondName LIKE @Empl
	                                    ORDER BY SecondName";
                        SQLiteCommand cmd = new SQLiteCommand(query, connection);
                        cmd.Parameters.AddWithValue("@Empl", TbSearch.Text);
                        DataTable DT = new DataTable("Employee");
                        SQLiteDataAdapter SDA = new SQLiteDataAdapter(cmd);
                        SDA.Fill(DT);
                        DGAllEmp.ItemsSource = DT.DefaultView;


                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                    }
                }
            }
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string relPath = @"EmACCHelper.chm";
                string fullPath = System.IO.Path.Combine(path, relPath);
                System.Diagnostics.Process.Start($@"{fullPath}");
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
    }
}