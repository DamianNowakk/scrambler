using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rc6
{
    class FileMy : TextReader
    {
        private readonly TextReader _baseReader;
        public int Position { get; private set; }

        public FileMy(TextReader baseReader)
        {
            _baseReader = baseReader;
        }

        public override int Read()
        {
            Position++;
            return _baseReader.Read();
        }

        public override int Peek()
        {
            return _baseReader.Peek();
        }

        public override void Close()
        {
            _baseReader.Close();
        }

        public static byte[] PobranieDanychZPliku(string odbiorca)
        {
            string wejscieSciezka = System.Windows.Forms.Application.StartupPath + "\\klucze\\prywatne\\" + odbiorca + ".private";
            byte[] dane = null;
            if (File.Exists(wejscieSciezka))
            {
                FileStream fs = new System.IO.FileStream(wejscieSciezka, FileMode.Open);
                dane = new byte[fs.Length];
                fs.Position = 0;
                fs.Read(dane, 0, (int)fs.Length);
                fs.Close();
            }
            return dane;
        }

        
    }
}
