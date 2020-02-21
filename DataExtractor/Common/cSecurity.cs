using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataExtractor.Common
{
    public partial class cSecurity
    {
        public cSecurity()
        {

        }

        public string strFilePath;
        public string Host;
        public string Port;
        public string Usr;
        public string BD;
        public string Pwd;
        public string Estatus;

        /// Esta función regresa  la cadena de conexión  
        public string getConnection(string strPath, string Ambiente)
        {
            string str = "";
            ArrayList arrayLists = new ArrayList();
            string str1 = "";
            string str2 = "";
            string str3 = "";
            string str4 = "";
            string str5 = "";
            string str6 = "";
            string str7 = "";
            string Connect = string.Empty;
            try
            {
                if (!File.Exists(strPath))
                {
                    File.Create(strPath).Close();
                }
                StreamReader streamReader = new StreamReader(strPath);


                do
                {
                    str = streamReader.ReadLine();
                    if (str == null)
                    {
                        continue;
                    }
                    string[] strArrays = str.Split(new char[] { '|' });
                    if (checked((int)strArrays.Length) == 7)
                    {
                        str1 = strArrays[2];
                        str2 = strArrays[0];
                        str3 = strArrays[1];
                        str4 = strArrays[3];
                        str5 = strArrays[4];
                        str6 = strArrays[5];
                        str7 = strArrays[6];

                        BD = this.getDecrypt(str3);
                        Estatus = this.getDecrypt(str7);

                        if (Estatus.ToUpper() == "ACTIVO")
                        {
                            if (BD.ToUpper() == "WEBSERVICE")
                            {
                                Usr = this.getDecrypt(str5);
                                Pwd = this.getDecrypt(str6);
                                Connect = Usr + "|" + Pwd;
                                break;
                            }
                            else
                            {

                                if (BD.ToUpper() == Ambiente.ToUpper())
                                {
                                    Host = this.getDecrypt(str1);
                                    Port = this.getDecrypt(str4);
                                    Usr = this.getDecrypt(str5);
                                    Pwd = this.getDecrypt(str6);

                                    Connect = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + Host + ")(PORT=" + Port + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + BD + ")));User Id=" + Usr + ";Password=" + Pwd;
                                    break;
                                }
                            }
                        }


                    }
                }
                while (str != null);
                streamReader.Close();
                return Connect;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// Esta función desencripta la cadena que le envíamos en el parámentro de entrada.
        public string getDecrypt(string cipherText)
        {
            try
            {
                string str = "KAaz20*50";
                string str1 = "SifitT_7";
                string str2 = "SHA1";
                int num = 2;
                string str3 = "@1B2c3D4e5F6g7H8";
                int num1 = 256;
                byte[] bytes = Encoding.ASCII.GetBytes(str3);
                byte[] numArray = Encoding.ASCII.GetBytes(str1);
                byte[] numArray1 = Convert.FromBase64String(cipherText);
                PasswordDeriveBytes passwordDeriveByte = new PasswordDeriveBytes(str, numArray, str2, num);
                byte[] bytes1 = passwordDeriveByte.GetBytes(num1 / 8);
                RijndaelManaged rijndaelManaged = new RijndaelManaged()
                {
                    Mode = CipherMode.CBC
                };
                ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor(bytes1, bytes);
                MemoryStream memoryStream = new MemoryStream(numArray1);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);
                byte[] numArray2 = new byte[checked(checked(checked((int)numArray1.Length) - 1) + 1)];
                int num2 = cryptoStream.Read(numArray2, 0, checked((int)numArray2.Length));
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(numArray2, 0, num2);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
