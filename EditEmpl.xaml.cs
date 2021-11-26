using System;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using uchet.Connection;

namespace uchet
{
    /// <summary>
    /// Логика взаимодействия для EditEmpl.xaml
    /// </summary>
    public partial class EditEmpl : Window
    {
        DataTable dt1 = new DataTable("Position");
        DataTable dt2 = new DataTable("Stat");
        string id;
        public EditEmpl(DataRowView drv)
        {
            InitializeComponent();
            CbStatusFill();
            CbPostFill();
            TbFN.Text = drv["FN"].ToString();
            TbSN.Text = drv["SN"].ToString();
            TbMN.Text = drv["MN"].ToString();
            TbPhone.Text = drv["Phone"].ToString();
            DpB.Text = drv["DoB"].ToString();
            CbPost.Text = drv["Post"].ToString();
            CbStat.Text = drv["Status"].ToString();
            id = drv["id"].ToString();
            
        }
/*        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }*/
        public void CbStatusFill()
        {
            dt2.Clear();
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {
                try
                {
                    connection.Open();
                    string query2 = $@"SELECT * FROM Stat";
                    SQLiteCommand cmd2 = new SQLiteCommand(query2, connection);
                    SQLiteDataAdapter SDA2 = new SQLiteDataAdapter(cmd2);
                    SDA2.Fill(dt2);
                    CbStat.ItemsSource = dt2.DefaultView;
                    CbStat.DisplayMemberPath = "Status";
                    CbStat.SelectedValuePath = "id";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void CbPostFill()
        {
            dt1.Clear();
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {
                try
                {
                    connection.Open();
                    string query1 = $@"SELECT * FROM Position";
                    SQLiteCommand cmd1 = new SQLiteCommand(query1, connection);
                    SQLiteDataAdapter SDA1 = new SQLiteDataAdapter(cmd1);
                    SDA1.Fill(dt1);
                    CbPost.ItemsSource = dt1.DefaultView;
                    CbPost.DisplayMemberPath = "Name";
                    CbPost.SelectedValuePath = "id";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public void UpdateEmployee()
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {
                connection.Open();
                if (String.IsNullOrEmpty(TbSN.Text) || String.IsNullOrEmpty(TbFN.Text) || String.IsNullOrEmpty(DpB.Text) || String.IsNullOrEmpty(TbPhone.Text) || CbStat.SelectedIndex == -1 || CbPost.SelectedIndex == -1)
                {
                    MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    int IdStatus, IdPost;
                    bool NameStat = int.TryParse(CbStat.SelectedValue.ToString(), out IdStatus);
                    bool NamePost = int.TryParse(CbPost.SelectedValue.ToString(), out IdPost);
                    string query = $@"UPDATE Employee SET FirstName=@FN, SecondName=@SN, MiddleName=@MN, Dateofbirth=@Date, Phone=@Phone, idPost='{IdPost}', idStatus='{IdStatus}' WHERE id='{id}' ";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    try
                    {
                        cmd.Parameters.AddWithValue("@SN", TbSN.Text);
                        cmd.Parameters.AddWithValue("@FN", TbFN.Text);
                        cmd.Parameters.AddWithValue("@MN", TbMN.Text);
                        cmd.Parameters.AddWithValue("@Date", DpB.Text);
                        cmd.Parameters.AddWithValue("@Phone", TbPhone.Text);
                        cmd.ExecuteNonQuery();

                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            UpdateEmployee();
            this.Close();
        }

        private void PhoneMask(string Phone)
        {
            var newVal = Phone;
            Phone = string.Empty;
            switch (newVal.Length)
            {
                case 1:
                    Phone = Regex.Replace(newVal, @"(\d{1})", "+7(___)___-__-__");
                    break;
                case 2:
                    Phone = Regex.Replace(newVal, @"(\d{1})(\d{0,3})", "+7($2__)___-__-__");
                    break;
                case 3:
                    Phone = Regex.Replace(newVal, @"(\d{1})(\d{0,3})", "+7($2_)___-__-__");
                    break;
                case 4:
                    Phone = Regex.Replace(newVal, @"(\d{1})(\d{0,3})", "+7($2)___-__-__");
                    break;
                case 5:
                    Phone = Regex.Replace(newVal, @"(\d{1})(\d{3})(\d{0,3})", "+7($2)$3__-__-__");
                    break;
                case 6:
                    Phone = Regex.Replace(newVal, @"(\d{1})(\d{3})(\d{0,3})", "+7($2)$3_-__-__");
                    break;
                case 7:
                    Phone = Regex.Replace(newVal, @"(\d{1})(\d{3})(\d{0,3})", "+7($2)$3-__-__");
                    break;
                case 8:
                    Phone = Regex.Replace(newVal, @"(\d{1})(\d{3})(\d{0,3})(\d{0,2})", "+7($2)$3-$4_-__");
                    break;
                case 9:
                    Phone = Regex.Replace(newVal, @"(\d{1})(\d{3})(\d{0,3})(\d{0,2})", "+7($2)$3-$4-__");
                    break;
                case 10:
                    Phone = Regex.Replace(newVal, @"(\d{1})(\d{3})(\d{0,3})(\d{0,2})(\d{0,2})", "+7($2)$3-$4-$5_");
                    break;
                case 11:
                    Phone = Regex.Replace(newVal, @"(\d{1})(\d{3})(\d{0,3})(\d{0,2})(\d{0,2})", "+7($2)$3-$4-$5");
                    break;
            }
            TbPhone.Text = Phone;
        }
        private string replacenumber()
        {
            string num = Regex.Replace(TbPhone.Text, @"[^0-9]", "");
            return num;
        }
        private void changeCaretIndex(string Phone)
        {
            if (Phone.Length <= 11)
            {
                PhoneMask(Phone);
            }
            if (Phone.Length <= 4)
            {
                TbPhone.CaretIndex = Phone.Length + 2;
            }
            else if (Phone.Length <= 7)
            {
                TbPhone.CaretIndex = Phone.Length + 3;
            }
            else if (Phone.Length <= 9)
            {
                TbPhone.CaretIndex = Phone.Length + 4;
            }
            else if (Phone.Length <= 11)
            {
                TbPhone.CaretIndex = Phone.Length + 5;
            }
        }
        private void TbPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            changeCaretIndex(replacenumber());
        }

        private void TbPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            changeCaretIndex(replacenumber() + e.Text);
            e.Handled = true;
        }

        private void TbPhone_GotFocus(object sender, RoutedEventArgs e)
        {
            changeCaretIndex(replacenumber());
        }
    }
}