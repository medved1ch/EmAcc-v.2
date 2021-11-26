using System;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SQLite;
using uchet.Connection;
using Excel = Microsoft.Office.Interop.Excel;

namespace uchet
{
    /// <summary>
    /// Логика взаимодействия для Tracker.xaml
    /// </summary>
    public partial class Tracker : Window
    {
        DataTable dt1 = new DataTable("Employee");
       
        public Tracker()
        {
            InitializeComponent();
            CbFill();
            DisplayData();
        }

        /*private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }*/
        public void DisplayData()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {
                try
                {
                    
                    connection.Open();
                    string query3 = $@"SELECT Tracking.id, Tracking.TimeEntrance AS TimeEn, Tracking.TimeExit AS TimeEx, Tracking.DateEntrance AS DateEn, Tracking.DateExit AS DateEx, Employee.FirstName, Employee.SecondName FROM Tracking INNER JOIN Employee on Tracking.idEmployee = Employee.id";
                    SQLiteCommand cmd3 = new SQLiteCommand(query3, connection);
                    DataTable dt2 = new DataTable("Tracking");
                    SQLiteDataAdapter SDA3 = new SQLiteDataAdapter(cmd3);
                    SDA3.Fill(dt2);
                    DGTrack.ItemsSource = dt2.DefaultView;
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }


            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Entrance();
            DisplayData();
        }
        public void CbFill()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {
                try
                {
                    connection.Open();
                    string query2 = $@"SELECT id, SecondName || ' ' || FirstName AS FullName FROM Employee";
                    SQLiteCommand cmd2 = new SQLiteCommand(query2, connection);
                    SQLiteDataAdapter SDA2 = new SQLiteDataAdapter(cmd2);
                    SDA2.Fill(dt1);
                    CbEmp.ItemsSource = dt1.DefaultView;
                    CbEmp.DisplayMemberPath = "FullName";
                    CbEmp.SelectedValuePath = "id";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void Entrance()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {
                if (CbEmp.SelectedIndex == -1)
                {
                    MessageBox.Show("Выберите сотрудника", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    int IdEmp;
                    bool NameStat = int.TryParse(CbEmp.SelectedValue.ToString(), out IdEmp);
                    connection.Open();
                    string query = $@"INSERT INTO Tracking('idEmployee','TimeEntrance', 'DateEntrance') values ('{IdEmp}', @Time, @Date)";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    try
                    {
                        cmd.Parameters.AddWithValue("@Time", DateTime.Now.TimeOfDay.ToString("hh\\:mm"));
                        cmd.Parameters.AddWithValue("@Date", DateTime.Now.Date.ToString("dd/MM/yyyy"));
                        cmd.ExecuteNonQuery();

                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }
        public void Exit()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {
                if (CbEmp.SelectedIndex == -1)
                {
                    MessageBox.Show("Выберите сотрудника", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                int IdEmp;
                bool NameStat = int.TryParse(CbEmp.SelectedValue.ToString(), out IdEmp);
                connection.Open();
                string query = $@"INSERT INTO Tracking('idEmployee','TimeExit', 'DateExit') values ('{IdEmp}', @Time, @Date)";
                SQLiteCommand cmd = new SQLiteCommand(query, connection);
                try
                {
                    cmd.Parameters.AddWithValue("@Time", DateTime.Now.TimeOfDay.ToString("hh\\:mm"));
                    cmd.Parameters.AddWithValue("@Date", DateTime.Now.Date.ToString("dd/MM/yyyy"));
                    cmd.ExecuteNonQuery();

                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        }
        private void BtnEx_Click(object sender, RoutedEventArgs e)
        {
            Exit();
            DisplayData();
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
                        string query = $@"SELECT Tracking.id, Tracking.TimeEntrance AS TimeEn, Tracking.TimeExit AS TimeEx, Tracking.DateEntrance AS DateEn, 
                                        Tracking.DateExit AS DateEx, Employee.FirstName, Employee.SecondName FROM Tracking 
                                        INNER JOIN Employee on Tracking.idEmployee = Employee.id WHERE Employee.SecondName=@SN
	                                    ORDER BY SecondName";
                        SQLiteCommand cmd = new SQLiteCommand(query, connection);
                        cmd.Parameters.AddWithValue("@SN", TbSearch.Text);
                        DataTable DT = new DataTable("Tracking");
                        SQLiteDataAdapter SDA = new SQLiteDataAdapter(cmd);
                        SDA.Fill(DT);
                        DGTrack.ItemsSource = DT.DefaultView;


                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message);
                    }
                }
            }
        }

        private void BtnExp_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Excel.Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Excel.Worksheet sheet1 = (Excel.Worksheet)workbook.Sheets[1];
            sheet1.Name = "Отчет за " + DateTime.Now.Date.ToString("dd/MM/yyyy");
            Excel.Range myRange1 = sheet1.get_Range("A1", "F1");
            myRange1.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            myRange1.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            myRange1.Merge(Type.Missing);
            myRange1.Font.Name = "Times New Roman";
            myRange1.Font.Bold = true;
            myRange1.Cells.Font.Size = 16;
            sheet1.Range["A1"].Value = "Отчёт был создан в программе EMACC " + DateTime.Now.Date.ToString("dd/MM/yyyy");
            for (int j = 0; j < DGTrack.Columns.Count; j++)
            {
                Excel.Range myRange = (Excel.Range)sheet1.Cells[2, j + 1];
                myRange.Font.Bold = true;
                sheet1.Columns[j + 1].ColumnWidth = 15;
                myRange.Value2 = DGTrack.Columns[j].Header;
            }
            for (int i = 0; i < DGTrack.Columns.Count; i++)
            {
                for (int j = 0; j < DGTrack.Items.Count; j++)
                {
                    TextBlock b = DGTrack.Columns[i].GetCellContent(DGTrack.Items[j]) as TextBlock;
                    Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 3, i + 1];
                    myRange.Value2 = b.Text;
                }
            }
        }
    }
    }
