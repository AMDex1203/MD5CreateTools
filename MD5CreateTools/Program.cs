using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Tester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "MD5 Create Tools";
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(" --------------------------------------------------------");
            Console.ResetColor();
            Console.WriteLine(" Tools Developed By : [DEV]AMDex");
            Console.WriteLine(" Copyright Tools: AMDex 2025");
            Console.WriteLine(" Version Tools : 2024.12.8");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" --------------------------------------------------------");
            Console.ResetColor();
            Console.WriteLine(" Machine Run Time : " + DateTime.Now.ToString("yyyy|MM|dd|HH:mm:ss"));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" --------------------------------------------------------");
            Console.ResetColor();
            Console.WriteLine(" Machine Region : " + TimeZoneInfo.Local.ToString());
            RegionInfo region = RegionInfo.CurrentRegion;
            Console.WriteLine(" Computer region : " + region.DisplayName.ToString());
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(" --------------------------------------------------------");
            Console.ResetColor();

            Console.WriteLine("Pilih opsi:");
            Console.WriteLine("1. Scan MD5 di dalam folder PACK");
            Console.WriteLine("2. Scanning MD5 semua file termasuk program");
            Console.WriteLine("3. Create UserFileList.dat File");
            Console.WriteLine("4. Encrypted UserFileList.dat");

            int pilihan = Convert.ToInt32(Console.ReadLine());

            switch (pilihan)
            {
                case 1:
                    ScanMD5Pack();
                    break;
                case 2:
                    ScanMD5Semua();
                    break;
                case 3:
                    CreateUserFileList();
                    break;
                case 4:
                    EncryptionFileScript();
                    break;
                default:
                    Console.WriteLine("Pilihan tidak valid.");
                    break;
            }
        }

        // Metode Scan MD5
        public static void ScanMD5Pack()
        {
            string directoryPath = @"Pack";
            string outputFile = "hash_md5_pack.txt";

            try
            {
                string[] filePaths = Directory.GetFiles(directoryPath, "*.i3Pack");
                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    foreach (string filePath in filePaths)
                    {
                        using (FileStream stream = File.OpenRead(filePath))
                        {
                            MD5 md5 = MD5.Create();
                            byte[] hash = md5.ComputeHash(stream);
                            string hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();
                            writer.WriteLine($"{hashString} {filePath}");
                        }
                    }
                }
                Console.WriteLine($"Hash MD5 berhasil disimpan ke {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Terjadi kesalahan: " + ex.Message);
            }
        }

        public static async void ScanMD5Semua()
        {
            string directoryPath = ".";
            string outputFile = "hash_md5_semua.txt";

            try
            {
                string[] filePaths = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    foreach (string filePath in filePaths)
                    {
                        using (FileStream stream = File.OpenRead(filePath))
                        {
                            MD5 md5 = MD5.Create();
                            byte[] hash = md5.ComputeHash(stream);
                            string hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();
                            writer.WriteLine($"{hashString} {filePath}");
                        }
                    }
                }
                await Task.Delay(3000);
                Console.WriteLine("Tunggu Sebentar......");
                await Task.Delay(3000);
                Console.WriteLine($"Hash MD5 berhasil disimpan ke {outputFile}");
                await Task.Delay(3000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Terjadi kesalahan: " + ex.Message);
            }
        }
        public static async void CreateUserFileList()
        {
            try
            {
                string directoryPath = Directory.GetCurrentDirectory();
                string hashList = "";
                string password = "your_password"; // Ganti dengan password yang Anda inginkan

                // Buat header XML
                hashList += "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
                hashList += "<list>\n";

                foreach (string filePath in Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories))
                {
                    string fileName = Path.GetFileName(filePath);
                    string fileExtension = Path.GetExtension(filePath);
                    string localPath = filePath.Replace(directoryPath, "");

                    using (FileStream stream = File.OpenRead(filePath))
                    {
                        MD5 md5 = MD5.Create();
                        byte[] hash = md5.ComputeHash(stream);
                        string hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();

                        // Enkripsi hashString menggunakan Chipper
                        string encryptedHashString = ChipperEncryption.Encrypt(hashString, password);

                        // Tambahkan elemen file ke XML
                        hashList += $" <file local=\"{localPath}\" hash=\"{encryptedHashString}\" />\n";
                    }
                }

                // Tutup elemen list
                hashList += "</list>\n";

                // Enkripsi hashList menggunakan Chipper
                string encryptedHashList = ChipperEncryption.Encrypt(hashList, password);

                // Simpan hash list ke file
                string hashListFilePath = Path.Combine(directoryPath, "UserFileList.dat");
                using (StreamWriter writer = new StreamWriter(hashListFilePath))
                {
                    await writer.WriteAsync(encryptedHashList);
                }

                Console.WriteLine("Hash list telah disimpan ke file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Kesalahan: " + ex.Message);
            }
        }

        public static class ChipperEncryption
        {
            public static string Encrypt(string plaintext, string password)
            {
                string encryptedText = "";
                int key = 0;

                foreach (char c in plaintext)
                {
                    key = password[key % password.Length];
                    encryptedText += (char)(c + key);
                }

                return encryptedText;
            }

            public static string Decrypt(string ciphertext, string password)
            {
                string decryptedText = "";
                int key = 0;

                foreach (char c in ciphertext)
                {
                    key = password[key % password.Length];
                    decryptedText += (char)(c - key);
                }

                return decryptedText;
            }
        }

        private static void EncryptionFileScript()
        {
            string filePath = "Script(2).i3Pack";
            string password = "your_password"; // Ganti dengan password yang Anda inginkan

            EncryptFile(filePath, password);
            Console.WriteLine("File telah dienkripsi.");

            // Untuk mendekripsi file
            // ChipperEncryption.DecryptFile(filePath, password);
            // Console.WriteLine("File telah didekripsi.");
        }

        public static void EncryptFile(string filePath, string password)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            byte[] encryptedBytes = Encrypt(fileBytes, password);
            File.WriteAllBytes(filePath, encryptedBytes);
        }

        public static void DecryptFile(string filePath, string password)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            byte[] decryptedBytes = Decrypt(fileBytes, password);
            File.WriteAllBytes(filePath, decryptedBytes);
        }

        private static byte[] Encrypt(byte[] plaintextBytes, string password)
        {
            byte[] encryptedBytes = new byte[plaintextBytes.Length];
            int key = 0;

            for (int i = 0; i < plaintextBytes.Length; i++)
            {
                key = password[key % password.Length];
                encryptedBytes[i] = (byte)(plaintextBytes[i] + key);
            }

            return encryptedBytes;
        }

        private static byte[] Decrypt(byte[] ciphertextBytes, string password)
        {
            byte[] decryptedBytes = new byte[ciphertextBytes.Length];
            int key = 0;

            for (int i = 0; i < ciphertextBytes.Length; i++)
            {
                key = password[key % password.Length];
                decryptedBytes[i] = (byte)(ciphertextBytes[i] - key);
            }

            return decryptedBytes;
        }
    }
}