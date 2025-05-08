using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
            Console.WriteLine("4. Encrypted File Script.i3Pack");
            Console.WriteLine("5. Decrypt UserFileList.dat");
            Console.WriteLine("6. Create UserFileList.dat File from Pack Folder");
            Console.WriteLine("7. Create MD5 UserFileList tanpa enkripsi");
            Console.WriteLine(" --------------------------------------------------------");

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
                case 5:
                    DecryptUserFileList().Wait();
                    break;
                case 6:
                    CreateUserFileListFromPack();
                    break;
                case 7:
                    CreateUserFileListNoEncryption();
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
                Console.Write("Enter encryption password: ");
                string password = Console.ReadLine();

                string directoryPath = Directory.GetCurrentDirectory();
                string hashList = "";

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

        public static async void CreateUserFileListFromPack()
        {
            try
            {
                Console.Write("Enter encryption password: ");
                string password = Console.ReadLine();

                string packFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Pack");
                if (!Directory.Exists(packFolderPath))
                {
                    Console.WriteLine("Folder 'Pack' tidak ditemukan.");
                    return;
                }

                string hashList = "";
                // Build XML header
                hashList += "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
                hashList += "<list>\n";

                foreach (string filePath in Directory.EnumerateFiles(packFolderPath, "*.*", SearchOption.AllDirectories))
                {
                    string localPath = filePath.Replace(Directory.GetCurrentDirectory(), "");
                    using (FileStream stream = File.OpenRead(filePath))
                    {
                        MD5 md5 = MD5.Create();
                        byte[] hash = md5.ComputeHash(stream);
                        string hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();

                        // Encrypt hashString using Chipper
                        string encryptedHashString = ChipperEncryption.Encrypt(hashString, password);

                        // Append file element to XML
                        hashList += $" <file local=\"{localPath}\" hash=\"{encryptedHashString}\" />\n";
                    }
                }

                // Close XML list element
                hashList += "</list>\n";

                // Encrypt the entire XML content using Chipper
                string encryptedHashList = ChipperEncryption.Encrypt(hashList, password);

                // Save encrypted content to UserFileList.dat
                string hashListFilePath = Path.Combine(Directory.GetCurrentDirectory(), "UserFileList.dat");
                using (StreamWriter writer = new StreamWriter(hashListFilePath))
                {
                    await writer.WriteAsync(encryptedHashList);
                }

                Console.WriteLine("UserFileList.dat untuk folder 'Pack' telah disimpan.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Kesalahan: " + ex.Message);
            }
        }

        public static async void CreateUserFileListNoEncryption()
        {
            try
            {
                string directoryPath = Directory.GetCurrentDirectory();
                string hashList = "";

                // Build XML header
                hashList += "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
                hashList += "<list>\n";

                foreach (string filePath in Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories))
                {
                    string localPath = filePath.Replace(directoryPath, "");
                    using (FileStream stream = File.OpenRead(filePath))
                    {
                        MD5 md5 = MD5.Create();
                        byte[] hash = md5.ComputeHash(stream);
                        string hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();

                        // Tanpa enkripsi, langsung tambahkan ke XML
                        hashList += $" <file local=\"{localPath}\" hash=\"{hashString}\" />\n";
                    }
                }

                // Close XML list element
                hashList += "</list>\n";

                // Save plain XML hash list ke file
                string hashListFilePath = Path.Combine(directoryPath, "UserFileList_NoEncryption.dat");
                using (StreamWriter writer = new StreamWriter(hashListFilePath))
                {
                    await writer.WriteAsync(hashList);
                }

                Console.WriteLine("UserFileList tanpa enkripsi telah disimpan.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Kesalahan: " + ex.Message);
            }
        }

        public static async Task DecryptUserFileList()
        {
            try
            {
                Console.Write("Enter decryption password: ");
                string password = Console.ReadLine();
                string directoryPath = Directory.GetCurrentDirectory();
                string hashListFilePath = Path.Combine(directoryPath, "UserFileList.dat");

                if (!File.Exists(hashListFilePath))
                {
                    Console.WriteLine("File UserFileList.dat tidak ditemukan.");
                    return;
                }

                string encryptedHashList = File.ReadAllText(hashListFilePath);
                string decryptedHashList = ChipperEncryption.Decrypt(encryptedHashList, password);

                Console.WriteLine("Isi file setelah dekripsi:");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine(decryptedHashList);
                Console.WriteLine("--------------------------------------------------");
                await Task.Delay(3000);
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