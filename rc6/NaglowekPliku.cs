using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace rc6
{
    [XmlRoot("EnctyptedFileHeader")]
    public class NaglowekPliku
    {
        [XmlElement("Algorithm")]
        public string Algorytm { get; set; }
        [XmlElement("CipherMode")]
        public string Tryb { get; set; }
        [XmlElement("IV")]
        public byte[] Iv { get; set; }
        [XmlElement("KeySize")]
        public int WielkoscKlucza { get; set; }
        [XmlElement("SubBlockSize")]
        public string WielkoscPodBlokuString { get; set; }
        [XmlArray("ApprovedUsers"), XmlArrayItem(typeof(Odbiorcy), ElementName = "User")]
        public List<Odbiorcy> ListaOdbiorcow { get; set; }
        [XmlIgnore] 
        private const string ZakonczenieNagłowka = "</EnctyptedFileHeader>";
        [XmlIgnore]
        public int WielkoscBloku { get; set; }
        [XmlIgnore]
        public int WielkoscPodBloku { get; set; }


        public NaglowekPliku(int wielkoscBloku, string tryb, byte[] iv, int wielkoscKlucza, int wielkoscPodBloku, List<Odbiorcy> listaOdbiorcow)
        {
            this.Algorytm = "RC6";
            this.WielkoscBloku = wielkoscBloku;
            this.Tryb = tryb;
            this.Iv = iv;
            this.WielkoscPodBloku = wielkoscPodBloku;
            this.WielkoscPodBlokuString = wielkoscPodBloku.ToString();
            this.WielkoscKlucza = wielkoscKlucza;
            this.ListaOdbiorcow = listaOdbiorcow;
            if (tryb.Equals("ECB"))
            {
                this.Iv = null;
                this.WielkoscPodBlokuString = null;
            }
            else if (tryb.Equals("CBC"))
            {
                this.WielkoscPodBlokuString = null;
            }
      

        }

        public NaglowekPliku()
        {
        }

        public static void Serjalizacja(NaglowekPliku obj, string wyjscieSciezka)
        {
            var serjalizator = new XmlSerializer(typeof(NaglowekPliku));
            using ( TextWriter writer = new StreamWriter(wyjscieSciezka))
            {
                serjalizator.Serialize(writer, obj);
            }  
        }

        public static NaglowekPliku Deserjalizacja(string wejsceSciezka)
        {
            var deserializer = new XmlSerializer(typeof(NaglowekPliku));
            TextReader reader = new StreamReader(wejsceSciezka);
            var tmp ="";
            var xml ="";
            while (!tmp.Equals(ZakonczenieNagłowka))
            {
                tmp = reader.ReadLine();
                xml += tmp;
            }     
            reader.Close();
            TextReader reader2 = new StringReader(xml);
            var obj = deserializer.Deserialize(reader2);
            var dane = (NaglowekPliku)obj;
            if (dane.WielkoscPodBlokuString != null)
                dane.WielkoscPodBloku = int.Parse(dane.WielkoscPodBlokuString);
            reader2.Close();
            return dane;
        }
    }
}