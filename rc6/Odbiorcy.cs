using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace rc6
{
    public class Odbiorcy
    {
        [XmlElement("Name")]
        public string Nazwa;
        [XmlElement("SessionKey")]
        public byte[] ZaszyfrowanykluczSesyjny;
        [XmlIgnore]
        public byte[] KluczSesyjny;

        public Odbiorcy(string nazwa, byte[] kluczSesyjny)
        {
            this.Nazwa = nazwa;
            this.KluczSesyjny = kluczSesyjny;
            var kluczSciezka = System.Windows.Forms.Application.StartupPath + "\\klucze\\publiczne\\" + nazwa;
            var klucz = File.ReadAllText(kluczSciezka);
            klucz = klucz.Replace("\r\n\t", string.Empty);
            klucz = klucz.Replace("\r\n", string.Empty);
            this.ZaszyfrowanykluczSesyjny = Klucze.SzyfrowanieRsa(kluczSesyjny, klucz);
            
        }

        public Odbiorcy()
        {

        }

        public void OdszyfrowanieKlucza(string password)
        {
            var kluczPrywatnyOdbiorcy = Klucze.DecryptPassword(password, this.Nazwa);
            if (kluczPrywatnyOdbiorcy != null)
            {
                KluczSesyjny = Klucze.OdszyfrowywanieRsa(ZaszyfrowanykluczSesyjny, kluczPrywatnyOdbiorcy);
            }
            else
            {
                KluczSesyjny = null;
            }
            
        }
    }
}
