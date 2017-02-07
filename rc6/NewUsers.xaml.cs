using System;
using System.Collections.Generic;
using System.Linq;
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

namespace rc6
{
    /// <summary>
    /// Interaction logic for NewUsers.xaml
    /// </summary>
    public partial class NewUsers : Window
    {
        private MainWindow Mainwindow { get; set; }


        public NewUsers(MainWindow mainwindow)
        {
            InitializeComponent();
            this.Mainwindow = mainwindow;
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            var work = true;
            if (newUserNameTextbox.Text == "")
            {
                Mainwindow.listboxSzyfrowanieLog.Items.Add("nowy użytkownik: Nie podano nazwy użytkownika");
                MessageBox.Show("Nie podano nazwy urzytkownika", "błąd");
                work = false;
            }
            else if (newUserPasswordTextbox.Password == "")
            {
                Mainwindow.listboxSzyfrowanieLog.Items.Add("nowy użytkownik: Nie podano hasła");
                MessageBox.Show("Nie podano hasła", "błąd");
                work = false;
            }
            else if (newUserPasswordRepeatTextbox.Password == "")
            {
                Mainwindow.listboxSzyfrowanieLog.Items.Add("nowy użytkownik: Nie powtórzono hasła");
                MessageBox.Show("Nie powtórzono hasła", "błąd");
                work = false;
            }
            else if (newUserPasswordRepeatTextbox.Password != newUserPasswordTextbox.Password)
            {
                Mainwindow.listboxSzyfrowanieLog.Items.Add("nowy użytkownik: Hasła nie są takie same");
                MessageBox.Show("Hasła nie są takie same", "błąd");
                work = false;
            }
            if (work)
            {
                Klucze.CreatNewKeys(newUserPasswordTextbox.Password, newUserNameTextbox.Text);
                this.Close();
                MessageBox.Show("Dodano nowego użytkownika", "info");
                Mainwindow.listboxSzyfrowanieLog.Items.Add("nowy użytkownik: Dodano nowego użytkownika: " + newUserNameTextbox.Text);
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
