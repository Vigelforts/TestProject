using System;
using System.Collections.Generic;

namespace CopyIcons
{
    public static class ProductIcons
    {
        public static List<ProductIcon> GetProducstIconList()
        {
            List<ProductIcon> result = new List<ProductIcon>();

            result.Add(new ProductIcon("Logo.scale-80.png", 120, 120));
            result.Add(new ProductIcon("Logo.scale-100.png", 150, 150));
            result.Add(new ProductIcon("Logo.scale-140.png", 210, 210));
            result.Add(new ProductIcon("Logo.scale-180.png", 270, 270));
            result.Add(new ProductIcon("SmallLogo.scale-80.png", 24, 24));
            result.Add(new ProductIcon("SmallLogo.scale-100.png", 30, 30));
            result.Add(new ProductIcon("SmallLogo.scale-140.png", 42, 42));
            result.Add(new ProductIcon("SmallLogo.scale-180.png", 54, 54));
            result.Add(new ProductIcon("SplashScreen.scale-100.png", 620, 300));
            result.Add(new ProductIcon("SplashScreen.scale-140.png", 868, 420));
            result.Add(new ProductIcon("SplashScreen.scale-180.png", 1116, 540));
            result.Add(new ProductIcon("StoreLogo.scale-100.png", 50, 50));
            result.Add(new ProductIcon("StoreLogo.scale-140.png", 70, 70));
            result.Add(new ProductIcon("StoreLogo.scale-180.png", 90, 90));
            result.Add(new ProductIcon("WideLogo.scale-80.png", 248, 120));
            result.Add(new ProductIcon("WideLogo.scale-100.png", 310, 150));
            result.Add(new ProductIcon("WideLogo.scale-140.png", 434, 210));
            result.Add(new ProductIcon("WideLogo.scale-180.png", 558, 270));

            return result;
        }
    }

    public struct ProductIcon
    {
        public ProductIcon(string name, int width, int height)
            : this()
        {
            Name = name;
            Width = width;
            Height = height;
        }

        public string Name { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}
