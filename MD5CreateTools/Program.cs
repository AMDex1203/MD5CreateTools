using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;

namespace Tester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Point Blank - System Server";
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(" --------------------------------------------------------");
            Console.ResetColor();
            Console.WriteLine("  Server Developed By : [DEV]AMDex");
            Console.WriteLine("  Copyright Server: MoMz GaMeS");
            Console.WriteLine("  Version Server : 2023.2.4.12");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" --------------------------------------------------------");
            Console.ResetColor();
            Console.WriteLine("  Machine Run Time : " + DateTime.Now.ToString("yyyy|MM|dd|HH:mm:ss"));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" --------------------------------------------------------");
            Console.ResetColor();
            Console.WriteLine("  Machine Region : " + TimeZoneInfo.Local.ToString());
            RegionInfo region = RegionInfo.CurrentRegion;
            Console.WriteLine("  Computer region : " + region.DisplayName.ToString());
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(" --------------------------------------------------------");
            Console.ResetColor();

            // Panggil fungsi MD5Checker setelah menampilkan informasi server
            MD5Checker(args);
        }

        public static void MD5Checker(string[] args)
        {
            // Ganti dengan path direktori yang berisi file .i3Pack
            string directoryPath = @"Pack";

            // Nama file teks untuk menyimpan hasil
            string outputFile = "hash_md5.txt";

            try
            {
                // Mendapatkan semua file .i3Pack dalam direktori
                string[] filePaths = Directory.GetFiles(directoryPath, "*.i3Pack");

                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    foreach (string filePath in filePaths)
                    {
                        using (FileStream stream = File.OpenRead(filePath))
                        {
                            MD5 md5 = MD5.Create();
                            byte[] hash = md5.ComputeHash(stream);

                            // Mengubah byte array menjadi string hexadecimal
                            string hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();

                            // Menulis hasil ke file teks
                            writer.WriteLine($"{filePath}:                                  {hashString}");
                        }
                    }
                }

                Console.WriteLine($"Hash MD5 berhasil disimpan ke {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Terjadi kesalahan: " + ex.Message);
            }
            Process.GetCurrentProcess().WaitForExit();
        }

    }
}