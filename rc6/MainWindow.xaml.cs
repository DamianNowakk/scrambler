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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading;
using Org.BouncyCastle;
using Org.BouncyCastle.Crypto.Engines;
using System.Xml.Serialization;
using MessageBox = System.Windows.Forms.MessageBox;


namespace rc6
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Wejscie { set; get; }
        public string Wyjscie { set; get; }
        public string Tryb { set; get; }
        public int Blok { set; get; }
        public int DlKlucza { set; get; }
        public int Podblok { set; get; }
        private List<string> _odbiorcy = new List<string>();
        public string Haslo { set; get; }

        public MainWindow()
        {
            InitializeComponent();
            Tryb = comboboxSzyfrowanieTryb.Text;
            DlKlucza = int.Parse(comboboxSzyfrowanieDlKlucza.Text);
            Podblok = int.Parse(comboboxSzyfrowaniePodblok.Text);
            labelSzyfrowaniePodblok.Visibility = Visibility.Hidden;
            comboboxSzyfrowaniePodblok.Visibility = Visibility.Hidden;
            folderExists();
        }

        private void ResetSzyfrowanie()
        {
            textboxSzyfrowanieWejscie.Text = "";
            textboxSzyfrowanieWyjscie.Text = "";
            listboxSzyfrowanieOdbiorcy.Items.Clear();  
            progressBarSzyfrowanie.Value = 0;
        }

        private void ResetOdszyfrowywanie()
        {
            textboxOdszyfrowywanieWejscie.Text = "";
            textboxOdszyfrowywanieWyjscie.Text = "";
            textboxOdszyfrowywanieHaslo.Password = "";
            labelOdszyfrowywanieOdbiorca.Content = "";
            listboxOdszyfrowywanieOdbiorcy.Items.Clear();
            progressBarOdszyfrowywanie.Value = 0;
        }

        private void folderExists()
        {
            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\klucze"))
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\klucze");
            }
            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\klucze\\publiczne"))
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\klucze\\publiczne");
            }
            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\klucze\\prywatne"))
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\klucze\\prywatne");
            }
        }
        
        //Szyfrator 
        readonly OpenFileDialog _sciezkaWejsciaSzyfrowanie = new OpenFileDialog();
        readonly SaveFileDialog _sciezkaWyjsciaSzyfrowanie = new SaveFileDialog();

        private void buttonSzyfrowanieWczytaj_Click(object sender, RoutedEventArgs e)
        {
            _sciezkaWejsciaSzyfrowanie.ShowDialog();
            textboxSzyfrowanieWejscie.Text = _sciezkaWejsciaSzyfrowanie.FileName;
            listboxSzyfrowanieLog.Items.Add("wyznaczono plik do szyfrowania");
        }

        private void buttonSzyfrowanieZapisz_Click(object sender, RoutedEventArgs e)
        {
            _sciezkaWyjsciaSzyfrowanie.ShowDialog();
            textboxSzyfrowanieWyjscie.Text = _sciezkaWyjsciaSzyfrowanie.FileName;
            listboxSzyfrowanieLog.Items.Add("wyznaczono miejsce zapisu pliku");
        }

        private void comboboxSzyfrowanieTryb_DropDownClosed(object sender, EventArgs e)
        {

            if (comboboxSzyfrowanieTryb.Text != "")
            {
                Tryb = comboboxSzyfrowanieTryb.Text;
                if (comboboxSzyfrowanieTryb.Text == "CFB" || comboboxSzyfrowanieTryb.Text == "OFB")
                {
                    labelSzyfrowaniePodblok.Visibility = Visibility.Visible;
                    comboboxSzyfrowaniePodblok.Visibility = Visibility.Visible;
                }
                else
                {
                    labelSzyfrowaniePodblok.Visibility = Visibility.Hidden;
                    comboboxSzyfrowaniePodblok.Visibility = Visibility.Hidden;
                }
            }
            listboxSzyfrowanieLog.Items.Add("zmieniono tryb szyfrowania na " + comboboxSzyfrowanieTryb.Text);
        }

        private void comboboxSzyfrowanieDlKlucza_DropDownClosed(object sender, EventArgs e)
        {
            listboxSzyfrowanieLog.Items.Add("zmieniono dlugosc klucza szyfrowania na " + comboboxSzyfrowanieDlKlucza.Text);
        }

        private void comboboxSzyfrowaniePodblok_DropDownClosed(object sender, EventArgs e)
        {
            listboxSzyfrowanieLog.Items.Add("zmieniono dlugosc podbloku szyfrowania na " + comboboxSzyfrowaniePodblok.Text);
        }

        private void buttonSzyfrowanieNowy_Click(object sender, RoutedEventArgs e)
        {
            var newuser = new NewUsers(this);
            newuser.Show();
        }

        private void buttonSzyfrowanieDodaj_Click(object sender, RoutedEventArgs e)
        {
            var sciezka = new OpenFileDialog
            {
                Multiselect = true,
                InitialDirectory = System.Windows.Forms.Application.StartupPath + "\\klucze\\publiczne"
            };
            sciezka.ShowDialog();
            foreach(var odbiorca in sciezka.SafeFileNames)
            {
                _odbiorcy.Add(odbiorca);
                listboxSzyfrowanieLog.Items.Add("dodano " + odbiorca);
            }
            _odbiorcy = _odbiorcy.Select(x => x).Distinct().ToList();
            listboxSzyfrowanieOdbiorcy.Items.Clear();

            foreach (var odbiorca in _odbiorcy)
            {
                listboxSzyfrowanieOdbiorcy.Items.Add(odbiorca);        
            }
            
            
        }

        private void buttonSzyfrowanieUsun_Click(object sender, RoutedEventArgs e)
        {
            if (listboxSzyfrowanieOdbiorcy.SelectedIndex != -1)
            {
                _odbiorcy.Remove(listboxSzyfrowanieOdbiorcy.SelectedValue.ToString());
                listboxSzyfrowanieLog.Items.Add("usunieto " + listboxSzyfrowanieOdbiorcy.SelectedValue.ToString());
                listboxSzyfrowanieOdbiorcy.Items.Remove(listboxSzyfrowanieOdbiorcy.SelectedItem);
            }
        }

        private void buttonSzyfrowanieSzyfruj_Click(object sender, RoutedEventArgs e)
        {

            
            Wejscie = textboxSzyfrowanieWejscie.Text;
            Wyjscie = textboxSzyfrowanieWyjscie.Text;
            DlKlucza = int.Parse(comboboxSzyfrowanieDlKlucza.Text);
            Podblok = int.Parse(comboboxSzyfrowaniePodblok.Text);
            Blok = 128;
            //-------------------------------------------------------//
            var kluczSesyjny = RC6.GeneratorKluczaSesyjnego(DlKlucza);
            var iv = RC6.GeneratorIv(Blok); //dlugosc bloku
            var listaOdbiorcow = new List<Odbiorcy>();
            foreach (var x in _odbiorcy)
            {
                listaOdbiorcow.Add(new Odbiorcy(x, kluczSesyjny));
            }

            bool fine = true;
            if (Wejscie == "")
            {
                listboxSzyfrowanieLog.Items.Add("Nie podano pliku do zaszyfrowania");
                MessageBox.Show("Nie podano pliku do zaszyfrowania", "błąd");
                fine = false;
            }
            else if (Wyjscie == "")
            {
                listboxSzyfrowanieLog.Items.Add("Nie podano miejsca zapisu");
                MessageBox.Show("Nie podano miejsca zapisu", "błąd");
                fine = false;
            }
            else if (listaOdbiorcow.Count == 0)
            {
                listboxSzyfrowanieLog.Items.Add("Nie dodano zadnego odbiorcy");
                MessageBox.Show("Nie dodano zadnego odbiorcy", "błąd");
                fine = false;
            }
            if (!fine) 
                return;

            listboxSzyfrowanieLog.Items.Add("rozpoczeto szyfrowanie");
            NaglowekPliku naglowek = new NaglowekPliku(Blok, Tryb, iv, DlKlucza, Podblok, listaOdbiorcow);
            NaglowekPliku.Serjalizacja(naglowek, Wyjscie);
            new Thread(
                delegate()
                {
                    RC6.SzyfrowanieRc6(Wejscie, Wyjscie, kluczSesyjny, naglowek, this);
                    Dispatcher.Invoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(
                            delegate()
                            {
                                listboxSzyfrowanieLog.Items.Add("zakonczono szyfrowanie");
                                progressBarSzyfrowanie.Value = 100;
                                MessageBox.Show("zakonczono szyfrowanie", "Info");
                                ResetSzyfrowanie();
                            }
                            ));
                       
                }
           ).Start();
        }

        //Deszyfrator----------------------------------------------------------------------------------------------------------------------Deszyfrator
        
        readonly OpenFileDialog _sciezkaWejsciaOdszyfrowywanie = new OpenFileDialog();
        readonly SaveFileDialog _sciezkaWyjsciaOdszyfrowywanie = new SaveFileDialog();
        NaglowekPliku _wczytaneDane;
        private Odbiorcy _odbiorca;

        private void buttonOdszyfrowywanieWczytaj_Click(object sender, RoutedEventArgs e)
        {
            _sciezkaWejsciaOdszyfrowywanie.ShowDialog();
            textboxOdszyfrowywanieWejscie.Text = _sciezkaWejsciaOdszyfrowywanie.FileName;
            if (textboxOdszyfrowywanieWejscie.Text == "") 
                return;

            listboxOdszyfrowywanieOdbiorcy.Items.Clear();
            Wejscie = textboxOdszyfrowywanieWejscie.Text;
            try
            {
                _wczytaneDane = NaglowekPliku.Deserjalizacja(Wejscie);
                foreach (var odbiorca in _wczytaneDane.ListaOdbiorcow)
                {
                    listboxOdszyfrowywanieOdbiorcy.Items.Add(odbiorca.Nazwa);
                }
                listboxOdszyfrowywanieLog.Items.Add("wyznaczono plik do odszyfrowania");
            }
            catch (Exception)
            {
                textboxOdszyfrowywanieWejscie.Text = "";
                Wejscie = null;
                MessageBox.Show("Niepoprawny plik do odszyfrowania", "błąd");
                listboxOdszyfrowywanieLog.Items.Add("Niepoprawny plik do odszyfrowania");
            }
        }

        private void buttonOdszyfrowywanieZapisz_Click_1(object sender, RoutedEventArgs e)
        {
            _sciezkaWyjsciaOdszyfrowywanie.ShowDialog();
            textboxOdszyfrowywanieWyjscie.Text = _sciezkaWyjsciaOdszyfrowywanie.FileName;
            listboxOdszyfrowywanieLog.Items.Add("wyznaczono miejsce zapisu pliku");
        }

        private void buttonOdszyfrowywanieWybierz_Click(object sender, RoutedEventArgs e)
        {
            if (listboxOdszyfrowywanieOdbiorcy.SelectedIndex != -1)
            {
                var nazwa = listboxOdszyfrowywanieOdbiorcy.SelectedValue.ToString();
                labelOdszyfrowywanieOdbiorca.Content = nazwa;
                _odbiorca = _wczytaneDane.ListaOdbiorcow.FirstOrDefault(n => n.Nazwa == nazwa);
                listboxOdszyfrowywanieLog.Items.Add("wyznaczono odbiorce: " + _odbiorca.Nazwa);
            }
        }

        private void buttonOdszyfrowywanieOdszyfruj_Click(object sender, RoutedEventArgs e)
        {
            Wyjscie = textboxOdszyfrowywanieWyjscie.Text;
            Haslo = textboxOdszyfrowywanieHaslo.Password;

            var fine = true;
            if (Wejscie == "")
            {
                listboxOdszyfrowywanieLog.Items.Add("Nie podano pliku do odszyfrowania");
                MessageBox.Show("Nie podano pliku do odszyfrowania", "błąd");
                fine = false;
            }
            else if (Wyjscie == "")
            {
                listboxOdszyfrowywanieLog.Items.Add("Nie podano miejsca zapisu");
                MessageBox.Show("Nie podano miejsca zapisu", "błąd");
                fine = false;
            }
            else if (Haslo == "")
            {
                listboxOdszyfrowywanieLog.Items.Add("Nie podano hasła");
                MessageBox.Show("Nie podano hasła", "błąd");
                fine = false;
            }
            if (fine)
            {
                listboxSzyfrowanieLog.Items.Add("rozpoczeto odszyfrowywanie");
                _odbiorca.OdszyfrowanieKlucza(Haslo);
                
                new Thread(
                    delegate()
                    {
                        if (_odbiorca.KluczSesyjny != null)
                        {
                            RC6.OdszyfrowywanieRc6(Wejscie, Wyjscie, _odbiorca, _wczytaneDane, this);
                        }
                        else
                        {

                            RC6.FakeRc6(Wejscie, Wyjscie, Haslo, this);
                        }                       
                        Dispatcher.Invoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(
                            delegate()
                            {
                                progressBarOdszyfrowywanie.Value = 100;
                                listboxSzyfrowanieLog.Items.Add("zakonczono odszyfrowywanie");
                                MessageBox.Show("zakonczono odszyfrowywanie", "Info");
                                ResetOdszyfrowywanie();
                            }
                        ));
                        
                    }
                ).Start();
                
            }
        }    
    }
}
