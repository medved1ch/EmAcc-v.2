using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        private string ConfirmCode;
        public Registration()
        {
            InitializeComponent();
        }
        private void DisableElements()
        {
            BtnNext.Visibility = Visibility.Hidden;
            BtnNext.IsEnabled = false;
        }
        private void VisibleElements()
        {
            BtnConfirm.Visibility = Visibility.Visible;
            TbCodeConfirm.Visibility = Visibility.Visible;
            BtnConfirm.IsEnabled = true;
            TbCodeConfirm.IsEnabled = true;
            TbLogin.IsReadOnly = true;
            PbPass.IsEnabled = false;
            TbMail.IsReadOnly = true;
            PbCheckPass.IsEnabled = false;
        }
        private static string RndStr(int len) //генератор пароля(временного)
        {
            string s = "", symb = "1234567890qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
            Random rnd = new Random();

            for (int i = 0; i < len; i++)
                s += symb[rnd.Next(0, symb.Length)];
            return s;

        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))
            {

                if (String.IsNullOrEmpty(TbLogin.Text) || String.IsNullOrEmpty(PbPass.Password) || String.IsNullOrEmpty(TbMail.Text) || String.IsNullOrEmpty(PbCheckPass.Password))
                {
                    MessageBox.Show("Заполните поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (TbLogin.Text.Length <= 3)
                {
                    MessageBox.Show(" Логин должен быть больше 3", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (PbPass.Password.Length <= 3)
                {
                    MessageBox.Show(" Пароль должен быть больше 3", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    connection.Open();
                    string query = $@"SELECT  COUNT(*) FROM User WHERE Login=@Login";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Login", TbLogin.Text.ToLower());
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    int pr = 0;
                    if (count == 1)
                    {
                        MessageBox.Show("Логин занят", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        pr = 1;
                    }
                    string query2 = $@"SELECT  COUNT(*) FROM User WHERE Mail=@Mail";
                    SQLiteCommand cmd2 = new SQLiteCommand(query2, connection);
                    cmd2.Parameters.AddWithValue("@Mail", TbMail.Text.ToLower());
                    int count2 = Convert.ToInt32(cmd2.ExecuteScalar());
                    int pr2 = 0;
                    if (count2 == 1)
                    {
                        MessageBox.Show("Пользователь с такой почтой уже зарегистрирован", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        pr2 = 1;
                    }
                    if (pr == 0 && pr2 == 0 && PbPass.Password == PbCheckPass.Password)
                    {
                        DisableElements();
                        VisibleElements();
                        try
                        {
                            ConfirmCode = "1";
                            ConfirmCode = RndStr(6);
                            SmtpClient Smtp = new SmtpClient("smtp.mail.ru");
                            Smtp.UseDefaultCredentials = true;
                            Smtp.EnableSsl = true;
                            Smtp.Credentials = new NetworkCredential("usotrudnikov@bk.ru", "fbNytLkEtd1fHVkJqCWJ");
                            MailMessage Message = new MailMessage();
                            try
                            {
                                Message.From = new MailAddress("usotrudnikov@bk.ru");
                                Message.To.Add(new MailAddress(TbMail.Text));
                                Message.Subject = "EmACC";
                                Message.Body = "Ваш код подтверждения: " + ConfirmCode + " . Для регистрации аккаунта: " + TbLogin.Text + ". На это сообщение не нужно отвечать.";
                                Smtp.Send(Message);
                                MessageBox.Show("На вашу почту выслан код для проверки, введите его, чтобы завершить регистрацию", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            catch
                            {
                                MessageBox.Show("Сообщение не может отправиться");
                            }
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("Введенная почта некорректна.");
                        }
                        catch (SmtpFailedRecipientsException)
                        {
                            MessageBox.Show("Введенная почта некорректна.");
                        }
                    }
                }
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrEmpty(TbCodeConfirm.Text))
            {
                MessageBox.Show("Заполните поле.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (TbCodeConfirm.Text == ConfirmCode)
            {
                using (SQLiteConnection connection = new SQLiteConnection(DBConnection.myConn))

                {
                    connection.Open();
                    string query = $@"INSERT INTO User ('Login','Mail','Password') VALUES (@Login, @Mail, @Pass)";
                    SQLiteCommand cmd = new SQLiteCommand(query, connection);
                    try
                    {
                        byte[] Pass;
                        Encryption f = new Encryption();
                        Pass = f.GetHashPassword(PbPass.Password);
                        cmd.Parameters.AddWithValue("@Login", TbLogin.Text.ToLower());
                        cmd.Parameters.AddWithValue("@Pass", Pass);
                        cmd.Parameters.AddWithValue("@Mail", TbMail.Text.ToLower());
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Проверка пройдена. Аккаунт зарегистрирован.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }

                    catch (SQLiteException ex)
                    {
                        MessageBox.Show("Ошибка" + ex);

                    }
                }
            }
            else
            {
                MessageBox.Show("Неправильные данные.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
