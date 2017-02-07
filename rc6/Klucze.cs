using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle;
namespace rc6
{
    class Klucze
    {
        public static void CreatNewKeys(string password, string odbiorca)
        {
            var rsa = new RSACryptoServiceProvider(2048);
            var publicKey = rsa.ToXmlString(false);
            var privateKey = rsa.ToXmlString(true);

            var encryptprivateKey = EncryptPassword(password, odbiorca, privateKey);
            publicKey = publicKey.Replace("<RSAKeyValue><Modulus>", "<RSAKeyValue>\r\n\t<Modulus>");
            publicKey = publicKey.Replace("</Modulus><Exponent>", "</Modulus>\r\n\t<Exponent>");
            publicKey = publicKey.Replace("</Exponent></RSAKeyValue>", "</Exponent>\r\n</RSAKeyValue>");
            File.WriteAllText(System.Windows.Forms.Application.StartupPath + "\\klucze\\publiczne\\" + odbiorca, publicKey);

            var path = System.Windows.Forms.Application.StartupPath + "\\klucze\\prywatne\\" + odbiorca + ".private";
            var fs = new System.IO.FileStream(path, FileMode.Create);
            fs.Write(encryptprivateKey, 0, encryptprivateKey.Length);
            fs.Close();
        }

        public static byte[] EncryptPassword(string password, string odbiorca, string privateKey)
        {
            var data = Encoding.ASCII.GetBytes(privateKey);
            var cryptoData = RC6.SzyfrowanieRc6Password(data, password);
            return cryptoData;
        }

        public static string DecryptPassword(string password, string odbiorca)
        {
            var data = FileMy.PobranieDanychZPliku(odbiorca);
            var cryptoData = RC6.OdszyfrowywanieRc6Password(data, password);
            try
            {
                return Encoding.ASCII.GetString(cryptoData);
            }
            catch (Exception)
            {
                return null;
            }
            
        }


        public static byte[] OdszyfrowywanieRsa(byte[] data, string privateKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            var decryptedByte = rsa.Decrypt(data, false);
            return decryptedByte;
        }

        public static byte[] SzyfrowanieRsa(byte[] data, string publicKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);
            var encryptedByteArray = rsa.Encrypt(data, false).ToArray();
            return encryptedByteArray;
        }
    }
}













        //byte[] data = null;
        //        string path = @"E:\SEMESTR 6\BEZPIECZENSTWO SYSTEMOW KUMPUTEROWYCH\projekt\RC6 wpf\rc6\rc6\bin\Debug\klucze\prywatne\Damian.private";
        //        if (File.Exists(path))
        //        {
        //            FileStream fs = new System.IO.FileStream(path, FileMode.Open);
        //            data = new byte[fs.Length];
        //            fs.Position = 0;
        //            fs.Read(data, 0, (int)fs.Lengteeh);
        //            fs.Close();
        //        }
        //        Klucze klucz = new Klucze();
        //        byte[] data2 = klucz.SzyfrowanieHaslem(data, "jestemdamian");
        //        string path2 = @"E:\SEMESTR 6\BEZPIECZENSTWO SYSTEMOW KUMPUTEROWYCH\projekt\RC6 wpf\rc6\rc6\bin\Debug\klucze\prywatne\Damian2.private";
        //        FileStream fs2 = new System.IO.FileStream(path2, FileMode.Create);
        //        fs2.Write(data2, 0, data2.Length);
        //        fs2.Close();
        //public byte[] SzyfrowanieHaslem(byte[] data, string password)
        //{
        //    GetKey(password);
        //    ICryptoTransform encryptor = _algorithm.CreateEncryptor();
        //    byte[] cryptoData = encryptor.TransformFinalBlock(data, 0, data.Length);
        //    return cryptoData;
        //}

        //public byte[] OdszyfrowywanieHaslem(byte[] cryptoData, string password)
        //{
        //    GetKey(password);
        //    ICryptoTransform decryptor = _algorithm.CreateDecryptor();
        //    byte[] data = decryptor.TransformFinalBlock(cryptoData, 0, cryptoData.Length);
        //    return data;
        //}

        //private void GetKey(string password)
        //{
        //    byte[] salt = new byte[8];
        //    byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
        //    int length = Math.Min(passwordBytes.Length, salt.Length);
        //    for (int i = 0; i < length; i++)
        //        salt[i] = passwordBytes[i];
        //    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt);
        //    _algorithm.Key = key.GetBytes(_algorithm.KeySize / 8);
        //    _algorithm.IV = key.GetBytes(_algorithm.BlockSize / 8);
        //}





        //static string _privateKey;
        //static string _publicKey;

        //public static void RSA()
        //{

        //    //System.Windows.Forms.Application.StartupPath + "\\klucze\\publiczne";

        //    var rsa = new RSACryptoServiceProvider();
        //    _privateKey = rsa.ToXmlString(true);
        //    _publicKey = rsa.ToXmlString(false);
        //    byte[] cos = {1,2,3,4,5,6,7,8,9};


        //    var enc = OdszyfrowywanieRSA(cos, _publicKey);

        //    var dec = SzyfrowanieRSA(enc, _privateKey);
          
        //}
        //private static readonly UnicodeEncoding Encoder = new UnicodeEncoding();
        //public static string Decrypt(string data, string privateKey)
        //{

        //    var rsa = new RSACryptoServiceProvider();
        //    var dataArray = data.Split(new char[] { ',' });
        //    byte[] dataByte = new byte[dataArray.Length];
        //    for (int i = 0; i < dataArray.Length; i++)
        //    {
        //        dataByte[i] = Convert.ToByte(dataArray[i]);
        //    }

        //    rsa.FromXmlString(privateKey);
        //    var decryptedByte = rsa.Decrypt(dataByte, false);
        //    return Encoder.GetString(decryptedByte);
        //}

        //public static string Encrypt(string data, string publicKey)
        //{
        //    var rsa = new RSACryptoServiceProvider();
        //    rsa.FromXmlString(publicKey);
        //    var dataToEncrypt = Encoder.GetBytes(data);
        //    var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
        //    var length = encryptedByteArray.Count();
        //    var item = 0;
        //    var sb = new StringBuilder();
        //    foreach (var x in encryptedByteArray)
        //    {
        //        item++;
        //        sb.Append(x);

        //        if (item < length)
        //            sb.Append(",");
        //    }

        //    return sb.ToString();
        //}


