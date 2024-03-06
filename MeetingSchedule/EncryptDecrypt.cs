using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;


    public  class EncryptDecrypt
    {
        private static string strEncrKey = "Progience";

        [DebuggerNonUserCode]
        public EncryptDecrypt()
        {
        }

        public static string Encrypt(string strIn)
        {
            byte[] rgbIV = new byte[8] { 18, 52, 86, 120, 144, 171, 205, 239 };
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(Strings.Left(strEncrKey, 8));
                byte[] bytes2 = Encoding.UTF8.GetBytes(strIn);
                DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(bytes, rgbIV), CryptoStreamMode.Write);
                cryptoStream.Write(bytes2, 0, bytes2.Length);
                cryptoStream.FlushFinalBlock();
                return Convert.ToBase64String(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception ex2 = ex;
                string result;
                if (Operators.CompareString(strIn, "", TextCompare: false) == 0)
                {
                    result = "";
                    ProjectData.ClearProjectError();
                    return result;
                }

                result = ex2.Message;
                ProjectData.ClearProjectError();
                return result;
            }
        }

        public static string Decrypt(string strIn)
        {
            if (!string.IsNullOrEmpty(strIn))
            {
                strIn = strIn.Replace(" ", "+");
                byte[] rgbIV = new byte[8] { 18, 52, 86, 120, 144, 171, 205, 239 };
                byte[] array = new byte[checked(strIn.Length + 1)];
                try
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(Strings.Left(strEncrKey, 8));
                    DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
                    array = Convert.FromBase64String(strIn);
                    MemoryStream memoryStream = new MemoryStream();
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(bytes, rgbIV), CryptoStreamMode.Write);
                    cryptoStream.Write(array, 0, array.Length);
                    cryptoStream.FlushFinalBlock();
                    Encoding uTF = Encoding.UTF8;
                    return uTF.GetString(memoryStream.ToArray());
                }
                catch (Exception ex)
                {
                    ProjectData.SetProjectError(ex);
                    Exception ex2 = ex;
                    string message = ex2.Message;
                    ProjectData.ClearProjectError();
                    return message;
                }
            }

            return strIn;
        }
    }

