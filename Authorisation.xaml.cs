using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using uchet.Connection;


namespace uchet
{
    /// <summary>
    /// Логика взаимодействия для Authorisation.xaml
    /// </summary>
    public partial class Authorisation : Window
    {
        public Authorisation()
        {
            InitializeComponent();
        }

        private void BtnSign_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(TbLogin.Text) || String.IsNullOrEmpty(PbPass.Password))
            {
                MessageBox.Show("Заполните поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
                    try
                    {
                        byte[] Pass;
                        Encryption f = new Encryption();
                        Pass = f.GetHashPassword(PbPass.Password);
                        connection.Open();
                        string query = $@"SELECT  COUNT(*) FROM User WHERE Login=@Login AND Password=@Pass";
                        SQLiteCommand cmd = new SQLiteCommand(query, connection);
                        string LoginLower = TbLogin.Text.ToLower();
                        cmd.Parameters.AddWithValue("@Login", TbLogin.Text.ToLower());
                        cmd.Parameters.AddWithValue("@Pass", Pass);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count == 1)
                        {
                            string query2 = $@"SELECT id FROM User WHERE Login=@Login";
                            SQLiteCommand cmd2 = new SQLiteCommand(query2, connection);
                            cmd2.Parameters.AddWithValue("@Login", TbLogin.Text.ToLower());
                            int countID = Convert.ToInt32(cmd2.ExecuteScalar());
                            connection.Close();
                            MessageBox.Show("Добро пожаловать!");
                            MainWindow menu = new MainWindow();
                            menu.Show();
                            this.Close();

                        }
                        else
                        {
                            MessageBox.Show("Неверное имя пользователя или пароль");
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
            }
        }

        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            registration.Owner = this;
            registration.ShowDialog();
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
