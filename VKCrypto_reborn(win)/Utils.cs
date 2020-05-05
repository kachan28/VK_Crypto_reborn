using System;
using System.Xml;
using System.Text;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using VkNet;

namespace VKCrypto_reborn_win_
{

    public class Utils
    {
        public class Crypto
        {
            public class Sim
            {
                private static string Generate_Sim_Key()
                {
                    CipherKeyGenerator keygen = new CipherKeyGenerator();
                    keygen.Init(new KeyGenerationParameters(new SecureRandom(), 256));
                    string key = Convert.ToBase64String(keygen.GenerateKey());
                    return key;
                }
                private static string SimKeyforMes;
                private static string SimKeyforCom;
                
                public Sim()
                {
                    SimKeyforMes = Generate_Sim_Key();
                    SimKeyforCom = Generate_Sim_Key();
                }
                public string GetSimKeyforMes()
                {
                    return SimKeyforMes;
                }
                public void SetSimKeyforMes(string NewSimKey)
                {
                    SimKeyforMes = NewSimKey;
                }
                public string Encryption(string data)
                {
                    CamelliaEngine encryptor = new CamelliaEngine();
                    string strkey = SimKeyforMes;
                    ICipherParameters param = new KeyParameter(Convert.FromBase64String(strkey));
                    encryptor.Init(true, param);
                    int strlengthbytes = Encoding.UTF8.GetByteCount(data);

                    if (strlengthbytes > 16 && strlengthbytes % 16 != 0)
                    {
                        for (int i = 0; i < 16 - strlengthbytes % 16; i++)
                        {
                            data += " ";
                        }

                    }

                    byte[] encdata = Encoding.UTF8.GetBytes(data);
                    string decmes = "";
                    if (encdata.Length < 16)
                    {
                        for (int i = 0; i < 16 - encdata.Length; i++)
                        {
                            data += " ";
                        }

                        encdata = Encoding.UTF8.GetBytes(data);
                        byte[] decdata = new byte[encdata.Length];
                        encryptor.ProcessBlock(encdata, 0, decdata, 0);
                        decmes = Convert.ToBase64String(decdata);
                    }


                    if (encdata.Length > 16)
                    {

                        byte[] decdata = new byte[encdata.Length];
                        for (int i = 0; i < encdata.Length; i += 16)
                        {
                            encryptor.ProcessBlock(encdata, i, decdata, i);
                            if (i + 16 > encdata.Length)
                            {
                                break;
                            }
                        }

                        decmes = Convert.ToBase64String(decdata);
                    }

                    return decmes;
                }
                public string Decryption(string curmessage)
                {
                    CamelliaEngine decryptor = new CamelliaEngine();
                    string strkey = SimKeyforMes;
                    ICipherParameters param = new KeyParameter(Convert.FromBase64String(strkey));
                    decryptor.Init(false, param);

                    byte[] nbts = Convert.FromBase64String(curmessage);
                    byte[] ndbts = new byte[nbts.Length];
                    if (ndbts.Length <= 16)
                    {
                        decryptor.ProcessBlock(nbts, 0, ndbts, 0);
                        return Encoding.UTF8.GetString(ndbts);
                    }

                    for (int i = 0; i < ndbts.Length; i += 16)
                    {
                        decryptor.ProcessBlock(nbts, i, ndbts, i);
                    }

                    return Encoding.UTF8.GetString(ndbts);
                }
            }
            public class ASim
            {
                private static string ToXmlString(RSA rsa, bool includePrivateParameters)
                {
                    RSAParameters parameters = rsa.ExportParameters(includePrivateParameters);

                    return string.Format(
                        "<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                        parameters.Modulus != null ? Convert.ToBase64String(parameters.Modulus) : null,
                        parameters.Exponent != null ? Convert.ToBase64String(parameters.Exponent) : null,
                        parameters.P != null ? Convert.ToBase64String(parameters.P) : null,
                        parameters.Q != null ? Convert.ToBase64String(parameters.Q) : null,
                        parameters.DP != null ? Convert.ToBase64String(parameters.DP) : null,
                        parameters.DQ != null ? Convert.ToBase64String(parameters.DQ) : null,
                        parameters.InverseQ != null ? Convert.ToBase64String(parameters.InverseQ) : null,
                        parameters.D != null ? Convert.ToBase64String(parameters.D) : null);
                }
                private static void FromXmlString(RSA rsa, string xmlString)
                {
                    RSAParameters parameters = new RSAParameters();

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlString);

                    if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
                    {
                        foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                        {
                            switch (node.Name)
                            {
                                case "Modulus":
                                    parameters.Modulus = (string.IsNullOrEmpty(node.InnerText)
                                        ? null
                                        : Convert.FromBase64String(node.InnerText));
                                    break;
                                case "Exponent":
                                    parameters.Exponent = (string.IsNullOrEmpty(node.InnerText)
                                        ? null
                                        : Convert.FromBase64String(node.InnerText));
                                    break;
                                case "P":
                                    parameters.P = (string.IsNullOrEmpty(node.InnerText)
                                        ? null
                                        : Convert.FromBase64String(node.InnerText));
                                    break;
                                case "Q":
                                    parameters.Q = (string.IsNullOrEmpty(node.InnerText)
                                        ? null
                                        : Convert.FromBase64String(node.InnerText));
                                    break;
                                case "DP":
                                    parameters.DP = (string.IsNullOrEmpty(node.InnerText)
                                        ? null
                                        : Convert.FromBase64String(node.InnerText));
                                    break;
                                case "DQ":
                                    parameters.DQ = (string.IsNullOrEmpty(node.InnerText)
                                        ? null
                                        : Convert.FromBase64String(node.InnerText));
                                    break;
                                case "InverseQ":
                                    parameters.InverseQ = (string.IsNullOrEmpty(node.InnerText)
                                        ? null
                                        : Convert.FromBase64String(node.InnerText));
                                    break;
                                case "D":
                                    parameters.D = (string.IsNullOrEmpty(node.InnerText)
                                        ? null
                                        : Convert.FromBase64String(node.InnerText));
                                    break;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid XML RSA key.");
                    }

                    rsa.ImportParameters(parameters);
                }

                private static RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(4096);
                public class Encryptor
                {

                    private string pubkey;
                    public Encryptor()
                    {
                        pubkey = ToXmlString(RSA, false);
                    }
                    public string RSAEncryption(string strText)
                    {
                        string publicKey = pubkey;

                        byte[] testData = Encoding.UTF8.GetBytes(strText);

                        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(4096))
                        {
                            try
                            {
                                // client encrypting data with public key issued by server                    
                                FromXmlString(rsa, publicKey);

                                var encryptedData = rsa.Encrypt(testData, true);

                                var base64Encrypted = Convert.ToBase64String(encryptedData);

                                return base64Encrypted;
                            }
                            finally
                            {
                                rsa.PersistKeyInCsp = false;
                            }
                        }
                    }
                    public string GetPubKey()
                    {
                        return pubkey;
                    }
                    public void SetPubKey(string newpub)
                    {
                        pubkey = newpub;
                    }
                }
                public class Decryptor
                {
                    private string privkey;
                    
                    public Decryptor()
                    {
                        privkey = ToXmlString(RSA, true);
                    }
                    public string RSADecryption(string strText)
                    {
                        string privateKey = privkey;

                        byte[] testData = Encoding.UTF8.GetBytes(strText);

                        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(4096))
                        {
                            try
                            {
                                string base64Encrypted = strText;

                                // server decrypting data with private key                    
                                FromXmlString(rsa, privateKey);

                                string npriv = privateKey;

                                Console.WriteLine(ToXmlString(rsa, true));

                                byte[] resultBytes = Convert.FromBase64String(base64Encrypted);
                                byte[] decryptedBytes = rsa.Decrypt(resultBytes, true);
                                string decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                                return decryptedData.ToString();
                            }
                            finally
                            {
                                rsa.PersistKeyInCsp = false;
                            }
                        }
                    }

                    public string GetPrivKey()
                    {
                        return privkey;
                    }
                }
            }
        }

        /*public class VKManage
        {
            public class VKSignal
            {
                public void SendSignalToLive()
                {
                    while (true)
                    {

                        int totalSeconds = (int)(DateTime.Now - DateTime.Today).TotalSeconds;
                        if (totalSeconds % 30 == 0)
                        {
                            Random random = new Random();
                            int randid = random.Next(999999);
                            MainWindow.api.Messages.Send(new MessagesSendParams
                            {
                                UserId = MainWindow.api.UserId,
                                RandomId = randid,
                                Message = "Ya zdes",
                            });
                            Thread.Sleep(1000);
                        }
                    }
                }
                public VKSignal()
                {
                    Action SendAction = new Action(SendSignalToLive);
                    Task SendTask = new Task(SendAction);
                    SendTask.Start();
                }
            }
        } */

        public static Crypto.ASim.Decryptor AsimDecryptor = new Crypto.ASim.Decryptor();
        public static Crypto.ASim.Encryptor AsimEncryptor = new Crypto.ASim.Encryptor();
        public static Crypto.Sim SimCrypto = new Crypto.Sim();
        public static VkApi Userapi = new VkApi();
    }

}
