using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.IO;
using System.Reflection;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;

namespace rc6
{
    class RC6
    {

        private static byte[] Bytes(int length)
        {
            var key = new byte[length / 8];
            RandomNumberGenerator.Create().GetBytes(key);

            var x = Cursor.Position.X * 2156327;
            var xb = BitConverter.GetBytes(x);

            var y = Cursor.Position.Y * 4373745;
            var yb = BitConverter.GetBytes(y);

            var m = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) * 313;
            var mb = BitConverter.GetBytes(m);

            var rnd = new Random();
            for (var i = 0; i < key.Length; i++)
            {
                switch (rnd.Next(0, 4))
                {
                    case 0:
                        key[i] = xb[rnd.Next(0, xb.Length)];
                        break;
                    case 2:
                        key[i] = yb[rnd.Next(0, yb.Length)];
                        break;
                    case 4:
                        key[i] = mb[rnd.Next(0, mb.Length)];
                        break;
                    default:
                        key[i] = key[i];
                        break;
                }
            }
            return key;
        }

        public static byte[] GeneratorKluczaSesyjnego(int length)
        {
            return Bytes(length);
        }


        public static byte[] GeneratorIv(int length)
        {
            //var iv = new byte[length / 8];
            //RandomNumberGenerator.Create().GetBytes(iv);
            //return iv;
            return Bytes(length);
        }

        public static byte[] SzyfrowanieRc6Password(byte[] data, string password )
        {
            var pass = getKey(password);
            var cipher = CipherUtilities.GetCipher("RC6/ECB/PKCS7Padding");
            var keyParam = ParameterUtilities.CreateKeyParameter("RC6", pass);
            cipher.Init(true, keyParam);
            return cipher.DoFinal(data);
        }

        public static byte[] OdszyfrowywanieRc6Password(byte[] data, string password)
        {
            var pass = getKey(password);
            var cipher = CipherUtilities.GetCipher("RC6/ECB/PKCS7Padding");
            var keyParam = ParameterUtilities.CreateKeyParameter("RC6", pass);
            cipher.Init(false, keyParam);
            try
            {
                return cipher.DoFinal(data);
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        private static byte[] getKey(string password)
        {
            var pass = Encoding.Unicode.GetBytes(password);
            var mySha256 = SHA256.Create();
            var hash = mySha256.ComputeHash(pass);
            return hash;
        }

        //-------------

        public static void SzyfrowanieRc6(string wejscie, string wyjscie, byte[] kluczsesyjny, NaglowekPliku naglowek, MainWindow window)
        {
            //entry
            var entry = new FileStream(wejscie, FileMode.Open);

            //exit
            var enter = Environment.NewLine;
            var enterdata = Encoding.UTF8.GetBytes(enter);
            var exit = new FileStream(wyjscie, FileMode.Append);
            exit.Write(enterdata, 0, enterdata.Length); // dodanie entera po xmlu

            var blockCipher = CreateCipher(naglowek.Tryb, naglowek.WielkoscPodBloku);

            ICipherParameters parameters;
            if (naglowek.Tryb == "ECB")
                parameters = new KeyParameter(kluczsesyjny);
            else
                parameters = new ParametersWithIV(new KeyParameter(kluczsesyjny), naglowek.Iv);
            blockCipher.Init(true, parameters);

            //date for progresbar
            var lenghtFile = entry.Length;
            double progres = lenghtFile/1000;
            long actuallyLenght = 0;

            //date for encrypt
            var buffer = new byte[16];
            byte[] outputBytes = null;
            var length = 0;
            int bytesRead;

            outputBytes = new byte[blockCipher.GetOutputSize(buffer.Length)];
            while ((bytesRead = entry.Read(buffer, 0, buffer.Length)) > 0)
            {
                length = blockCipher.ProcessBytes(buffer, 0, bytesRead, outputBytes, 0);
                exit.Write(outputBytes, 0, length);
                //progresbar
                actuallyLenght += bytesRead;
                if (actuallyLenght > progres)
                {
                    window.progressBarSzyfrowanie.Dispatcher.Invoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(
                            delegate() 
                            {
                                window.progressBarSzyfrowanie.Value += 0.1;
                            }
                        ));
                    actuallyLenght -= (long) progres;
                }
            }           
            length = blockCipher.DoFinal(outputBytes, 0);
            exit.Write(outputBytes, 0, length);


            exit.Close();
            entry.Close();
        }

        public static void OdszyfrowywanieRc6(string wejscie, string wyjscie, Odbiorcy odbiorca, NaglowekPliku naglowek, MainWindow window)
        {
            //data
            var reader2 = new StreamReader(wejscie);
            var reader = new FileMy(reader2);
            while (true)
            {
                var searcher = reader.ReadLine();
                if (searcher != null && searcher.StartsWith("</EnctyptedFileHeader>"))
                    break;
            }
            var position = reader.Position;
            reader.Close();
            var entry = new FileStream(wejscie, FileMode.Open)
            {
                Position = position      
            };
            var exit = new System.IO.FileStream(wyjscie, FileMode.Create);



            //decrypt
            var blockCipher = CreateCipher(naglowek.Tryb, naglowek.WielkoscPodBloku);
            ICipherParameters parameters;
            if (naglowek.Tryb == "ECB")
                parameters = new KeyParameter(odbiorca.KluczSesyjny);
            else
                parameters = new ParametersWithIV(new KeyParameter(odbiorca.KluczSesyjny), naglowek.Iv);
            blockCipher.Init(false, parameters);

            //date for progresbar
            long lenghtFile = entry.Length;
            double progres = lenghtFile / 1000;
            long actuallyLenght = 0;

            //date for encrypt
            var buffer = new byte[16];
            var outputBytes = new byte[blockCipher.GetOutputSize(buffer.Length)];
            var length = 0;
            int bytesRead;

             while ((bytesRead = entry.Read(buffer, 0, buffer.Length)) > 0)
            {

                length = blockCipher.ProcessBytes(buffer, 0, bytesRead, outputBytes, 0);
                exit.Write(outputBytes, 0, length);
                //progresbar
                actuallyLenght += bytesRead;
                if (actuallyLenght > progres)
                {
                    window.progressBarOdszyfrowywanie.Dispatcher.Invoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(
                            delegate()
                            {
                                window.progressBarOdszyfrowywanie.Value += 0.1;
                            }
                            ));
                    actuallyLenght -= (long) progres;
                }
            }
            length = blockCipher.DoFinal(outputBytes, 0);
            exit.Write(outputBytes, 0, length);

            entry.Close();
            exit.Close();
        }

        private static PaddedBufferedBlockCipher CreateCipher(string encryptionMode, int segmentSize)
        {
            var engine = new RC6Engine();
            var padding = new Pkcs7Padding();
            IBlockCipher cipherWithoutPadding = null;
            switch (encryptionMode)
            {
                case "ECB":
                    cipherWithoutPadding = engine;
                    break;
                case "CBC":
                    cipherWithoutPadding = new CbcBlockCipher(engine);
                    break;
                case "CFB":
                    cipherWithoutPadding = new CfbBlockCipher(engine, segmentSize);
                    break;
                case "OFB":
                    cipherWithoutPadding = new OfbBlockCipher(engine, segmentSize);
                    break;
            }

            return new PaddedBufferedBlockCipher(cipherWithoutPadding, padding);
        }

        public static void FakeRc6(string wejscie, string wyjscie, string password, MainWindow window)
        {
            //entry
            var entry = new FileStream(wejscie, FileMode.Open);

            //exit
            var enter = Environment.NewLine;
            var enterdata = Encoding.UTF8.GetBytes(enter);
            var exit = new FileStream(wyjscie, FileMode.Append);
            exit.Write(enterdata, 0, enterdata.Length); // dodanie entera po xmlu

            var pass = getKey(password);
            var blockCipher = CipherUtilities.GetCipher("RC6/ECB/PKCS7Padding");
            var keyParam = ParameterUtilities.CreateKeyParameter("RC6", pass);
            blockCipher.Init(true, keyParam);


            //date for progresbar
            var lenghtFile = entry.Length;
            double progres = lenghtFile / 1000;
            long actuallyLenght = 0;

            //date for encrypt
            var buffer = new byte[16];
            byte[] outputBytes = null;
            var length = 0;
            int bytesRead;

            outputBytes = new byte[blockCipher.GetOutputSize(buffer.Length)];
            while ((bytesRead = entry.Read(buffer, 0, buffer.Length)) > 0)
            {
                length = blockCipher.ProcessBytes(buffer, 0, bytesRead, outputBytes, 0);
                exit.Write(outputBytes, 0, length);
                //progresbar
                actuallyLenght += bytesRead;
                if (actuallyLenght > progres)
                {
                    window.progressBarOdszyfrowywanie.Dispatcher.Invoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(
                            delegate()
                            {
                                window.progressBarOdszyfrowywanie.Value += 0.1;
                            }
                        ));
                    actuallyLenght -= (long)progres;
                }
            }
            length = blockCipher.DoFinal(outputBytes, 0);
            exit.Write(outputBytes, 0, length);


            exit.Close();
            entry.Close();
        }
    }
}