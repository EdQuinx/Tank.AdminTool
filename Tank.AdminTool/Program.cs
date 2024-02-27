using System.Security.Cryptography;
using System.Text;

namespace Tank.AdminTool
{
    internal class Program
    {

        static string GetMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2")); // Chuyển byte thành chuỗi Hex
                }
                return sb.ToString();
            }
        }

        static bool VerifyMD5(string input, string hash)
        {
            string inputHash = GetMD5(input);
            return StringComparer.OrdinalIgnoreCase.Compare(inputHash, hash) == 0;
        }
        static void MD5Tool()
        {
            string path = "swf/";
            string fileName;
            string fullPath;
            Console.WriteLine("Enter file text (*.txt)");
            fileName = Console.ReadLine();
            fullPath = $"{path}{fileName}.txt";

            if (!File.Exists(fullPath)) 
            {
                Console.WriteLine($"File {fileName}.txt not found!");
                return;
            }

            string[] lines = File.ReadAllLines(fullPath);
            string nameSWF = "namefiles";
            string symbols = "";
            bool md5Check = false;
            for (int i = 0; i < lines.Length; i++)
            {
                //get symbols
                if (lines[i].Contains("string symbols"))
                {
                    symbols = lines[i].Split(':')[1].ToLower().Trim();
                }

                //md5
                if (md5Check)
                {
                    string[] md5Strs = lines[i].Split("-");
                    if (md5Strs.Length < 2)
                        break;
                    string symbol = symbols.Replace(nameSWF, md5Strs[1].Trim());
                    string md5Code = GetMD5(symbol.Replace(".swf", ""));
                    lines[i] = $"{md5Code} - {md5Strs[1].Trim()}";

                    //change file
                    string fileSWFName = $"{path}{md5Strs[1].Trim()}.swf";
                    if (!File.Exists(fileSWFName))
                    {
                        Console.WriteLine($"Can't change file name {md5Strs[1].Trim()}.swf");
                    }
                    else
                    {
                        File.Move(fileSWFName, $"{path}{md5Code}.swf");
                        Console.WriteLine($"Change file name {md5Strs[1].Trim()}.swf to {md5Code}.swf successfully!");
                    }
                }
                if (lines[i].Contains("MD5 files") && md5Check)
                {
                    md5Check = false;
                }
                if (lines[i].Contains("MD5 files") && !md5Check)
                {
                    md5Check = true;
                }
            }
            //File.Create($"{path}{fileName}_encrypt.txt");
            File.WriteAllLines($"{path}{fileName}_encrypt.txt", lines);
        }
        static void Main(string[] args)
        {
            int option;
            Console.WriteLine("Welcome to Tank Tool! Please select option");
            Console.WriteLine("1. MD5 Tool");

            option = int.Parse(Console.ReadLine());
            switch (option) 
            {
                case 1:
                    MD5Tool();
                    break;
            }
        }
    }
}