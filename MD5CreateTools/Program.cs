using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
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

                        // Tambahkan elemen file ke XML
                        hashList += $"  <file local=\"{localPath}\" hash=\"{hashString}\" />\n";
                    }
                }

                // Tutup elemen list
                hashList += "</list>\n";

                // Simpan hash list ke file
                string hashListFilePath = Path.Combine(directoryPath, "UserFileList.dat");
                using (StreamWriter writer = new StreamWriter(hashListFilePath))
                {
                    await writer.WriteAsync(hashList);
                }

                Console.WriteLine("Hash list telah disimpan ke file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Kesalahan: " + ex.Message);
            }
        }

    }
}