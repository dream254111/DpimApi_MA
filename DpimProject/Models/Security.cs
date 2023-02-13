using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;

namespace DpimProject.Security
{
    class AesEncyption
    {

        private string keyStr = "mango";

        public AesEncyption()
        {
            Initial();
        }

        public AesEncyption(string key)
        {
            if (key != "") { keyStr = key; }
            Initial();
        }

        private ICryptoTransform encryptor, decryptor;
        private UTF8Encoding encoder;

        private void Initial()
        {
            HashProvider hash = new HashProvider();
            //32bytes
            byte[] key = Encoding.ASCII.GetBytes(hash.Md5(keyStr));

            //16Bytes
            byte[] vector = Encoding.ASCII.GetBytes(hash.Md5(keyStr).Substring(0, 16));


            RijndaelManaged rm = new RijndaelManaged();
            encryptor = rm.CreateEncryptor(key, vector);
            decryptor = rm.CreateDecryptor(key, vector);
            encoder = new UTF8Encoding();
        }

        public string Encrypt(string unencrypted)
        {

            return Convert.ToBase64String(Encrypt(encoder.GetBytes(unencrypted)));
        }

        public string Decrypt(string encrypted)
        {

            return encoder.GetString(Decrypt(Convert.FromBase64String(encrypted)));
        }

        public byte[] Encrypt(byte[] buffer)
        {
            return Transform(buffer, encryptor);
        }

        public byte[] Decrypt(byte[] buffer)
        {
            return Transform(buffer, decryptor);
        }

        protected byte[] Transform(byte[] buffer, ICryptoTransform transform)
        {
            MemoryStream stream = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
                cs.FlushFinalBlock();
                cs.Dispose();
            }
            return stream.ToArray();
        }
    }

    public class HashProvider
    {
        public HashProvider()
        {

        }

        public string Md5(string text)
        {
            MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hash = _md5.ComputeHash(buffer);
            return string.Join("", hash.Select(x => x.ToString("x2")));
            //return Convert.ToBase64String(hash);
        }

        public string Sha1(string text)
        {
            SHA1CryptoServiceProvider _sha1 = new SHA1CryptoServiceProvider();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hash = _sha1.ComputeHash(buffer);
            return string.Join("", hash.Select(x => x.ToString("x2")));
            //return Convert.ToBase64String(hash);
        }  

        //public string 
    }

    public class Token
    {
        private AesEncyption _aes;
        private HashProvider _hash;

        string salt = "I_LOVE_MANGO";


        public Token()
        {
            _aes = new AesEncyption(salt);
            _hash = new HashProvider();
        }

        public Token(string privateKey)
        {
            _aes = new AesEncyption(privateKey + salt);
            _hash = new HashProvider();
        }

        public string CreateToken(string text)
        {
            //return text;
            string data = _aes.Encrypt(text);
            return data + "." + _hash.Sha1(data);
        }public string CreateTokenStream(string text)
        {
            //return text;
            string data = _aes.Encrypt(text);
            return data + "." + _hash.Sha1(data);
        }

        public bool CheckToken(string token, out string data)
        {
            //data = token;
            //return true;
            data = "";

            if (string.IsNullOrEmpty(token)) return false;

            if (token.IndexOf(".") < 0) return false;

            string[] splitData = token.Split('.');

            if (splitData.Length != 2) return false;
            if (_hash.Sha1(splitData[0]) != splitData[1]) return false;

            //try
            //{
            data = _aes.Decrypt(splitData[0]);
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}

            return true;
        }
       public bool CheckTokenStream(string token, out string data)
        {
            //data = token;
            //return true;
            data = "";

            if (string.IsNullOrEmpty(token)) return false;

            if (token.IndexOf(".") < 0) return false;

            string[] splitData = token.Split('.');

            if (splitData.Length != 2) return false;
            if (_hash.Sha1(splitData[0]) != splitData[1]) return false;

            //try
            //{
            data = _aes.Decrypt(splitData[0]);
            //}
            string path_default = data.Substring(0, 8);
            string path_dir = data.Substring(0, data.IndexOf("_"));
            data = path_default + "\\" + path_dir + "\\" + data;
            //catch (Exception ex)
            //{
            //    return false;
            //}

            return true;
        }
    }

    public class Password
    {
        public Password() { }

        public string CreatePassword(int length)
        {
            const string AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz#@$^*()";
            Random rnd = new Random();
            string newPassword = "";

            int AllowedCharsLen = AllowedChars.Length;
            for (int i = 0; i < length; i++)
            {
                newPassword += AllowedChars.Substring(rnd.Next(0, AllowedCharsLen), 1);
            }

            return newPassword;
        }
    }

}