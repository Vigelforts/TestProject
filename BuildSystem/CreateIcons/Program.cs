using CopyIcons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace CreateIcons
{
    class Program
    {
        static void Main(string[] args)
        {
            ValidateArguments(args);

            using (Image srcImage = Image.FromFile(args[0]))
            {
                List<ProductIcon> icons = ProductIcons.GetProducstIconList();
                foreach (ProductIcon icon in icons)
                {
                    string outputFile = Path.Combine(args[1], icon.Name);
                    ResizeImage(srcImage, outputFile, icon.Width, icon.Height);
                }
            }

            Console.ReadKey();
        }

        private static void ResizeImage(Image srcImage, string outputFile, int width, int height)
        {
            using (Image newImage = new Bitmap(width, height))
            {
                using (Graphics graphics = Graphics.FromImage(newImage))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.DrawImage(srcImage, new Rectangle(0, 0, width, height));
                    newImage.Save(outputFile);
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

            if (!File.Exists(args[0]))
            {
                Console.WriteLine(string.Format("File {0} can not be found.", args[0]));
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
            Console.WriteLine("Copy icons for Windows 8.1 projects.\nUsage:\nCopyIcons.exe <source_file> <dest_folder>\nSource file width must be greater than 540");
        }
    }
}
