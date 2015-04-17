using System;
using System.Collections.Generic;
using System.IO;

namespace CopyIcons
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!ValidateArguments(args))
            {
                return;
            }

            string srcDirPath = args[0];
            string destDirPath = args[1];

            List<ProductIcon> iconsList = ProductIcons.GetProducstIconList();

            foreach (string file in Directory.EnumerateFiles(srcDirPath))
            {
                Tuple<int, int> imageSize = GetImageSize(file);
                if (imageSize == null)
                {
                    continue;
                }

                int width = imageSize.Item1;
                int height = imageSize.Item2;

                foreach (ProductIcon icon in iconsList)
                {
                    if (width == icon.Width && height == icon.Height)
                    {
                        string destFile = Path.Combine(destDirPath, icon.Name);
                        File.Copy(file, destFile, true);

                        Console.WriteLine(string.Format("{0} -> {1}", file, destFile));
                    }
                }
            }
        }

        private static bool ValidateArguments(string[] args)
        {
            if (args.Length != 2)
            {
                PrintUsage();
                return false;
            }

            if (!Directory.Exists(args[0]))
            {
                Console.WriteLine(string.Format("Path {0} can not be found.", args[0]));
                return false;
            }

            if (!Directory.Exists(args[1]))
            {
                Console.WriteLine(string.Format("Path {0} can not be found.", args[1]));
                return false;
            }

            return true;
        }
        private static void PrintUsage()
        {
            Console.WriteLine("Copy icons for Windows 8.1 projects.\nUsage:\nCopyIcons.exe <source_folder> <dest_folder>");
        }

        private static Tuple<int, int> GetImageSize(string path)
        {
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(path);
                return new Tuple<int, int>(image.Width, image.Height);
            }
            catch
            {
                return null;
            }
        }
    }
}
