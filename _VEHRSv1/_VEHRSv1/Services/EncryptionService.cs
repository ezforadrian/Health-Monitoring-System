using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace _VEHRSv1.Services
{
    public class EncryptionService
    {
        private static readonly byte[] ConstantIV = Encoding.UTF8.GetBytes("thequickbrownfox"); // Must be 16 bytes for AES

        public string EncryptConstant(string encryptionKey, string rawData)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(encryptionKey);
                    aesAlg.IV = ConstantIV;

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    byte[] encryptedData;

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cs))
                            {
                                sw.Write(rawData);
                            }
                        }
                        encryptedData = ms.ToArray();
                    }

                    // Convert the byte array to a hexadecimal string
                    return BitConverter.ToString(encryptedData).Replace("-", "");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Encryption failed: " + ex.Message);
            }
        }

        public string DecryptConstant(string encryptionKey, string encryptedData)
        {
            try
            {
                // Convert the hexadecimal string to a byte array
                byte[] encryptedBytes = new byte[encryptedData.Length / 2];
                for (int i = 0; i < encryptedData.Length; i += 2)
                {
                    encryptedBytes[i / 2] = Convert.ToByte(encryptedData.Substring(i, 2), 16);
                }

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(encryptionKey);
                    aesAlg.IV = ConstantIV;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (var ms = new MemoryStream(encryptedBytes))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Decryption failed: " + ex.Message);
            }
        }

        public string Encrypt(string encryptionKey, string rawData)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(encryptionKey);
                    aesAlg.GenerateIV();

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    byte[] encryptedData;

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cs))
                            {
                                sw.Write(rawData);
                            }
                        }
                        encryptedData = ms.ToArray();
                    }

                    var ivAndEncryptedData = new byte[aesAlg.IV.Length + encryptedData.Length];
                    Array.Copy(aesAlg.IV, ivAndEncryptedData, aesAlg.IV.Length);
                    Array.Copy(encryptedData, 0, ivAndEncryptedData, aesAlg.IV.Length, encryptedData.Length);

                    // Convert the byte array to a hexadecimal string
                    return BitConverter.ToString(ivAndEncryptedData).Replace("-", "");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Encryption failed: " + ex.Message);
            }
        }

        public string Decrypt(string encryptionKey, string rawData)
        {
            try
            {
                // Convert the hexadecimal string to a byte array
                byte[] ivAndEncryptedData = new byte[rawData.Length / 2];
                for (int i = 0; i < rawData.Length; i += 2)
                {
                    ivAndEncryptedData[i / 2] = Convert.ToByte(rawData.Substring(i, 2), 16);
                }

                // Extract the IV from the data
                byte[] iv = new byte[16]; // The IV size is typically 16 bytes for AES
                Array.Copy(ivAndEncryptedData, iv, iv.Length);

                // Create an AES instance with the same key and IV
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(encryptionKey);
                    aesAlg.IV = iv;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Decrypt the data
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(ivAndEncryptedData, iv.Length, ivAndEncryptedData.Length - iv.Length);
                        }

                        byte[] decryptedData = ms.ToArray();

                        // Convert the decrypted bytes to a string (assuming it's text data)
                        return Encoding.UTF8.GetString(decryptedData);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Decryption failed: " + ex.Message);
            }
        }
    }
}
