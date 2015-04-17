using Paragon.Container.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

namespace BuildSystem
{
    public class InAppTestConfigGenerator
    {
        public static void CreateInAppTestConfig(ProductsCatalog catalog)
        {
            ServiceLocator.Logger.Log(LogLevel.Info, "In app test config generation...");

            try
            {
                XNamespace xml = XNamespace.Xml;

                XElement rootElement = new XElement("CurrentApp");

                XElement listingInformationElement = new XElement("ListingInformation",
                    new XElement("App",
                        new XElement("AppId", "00000000-0000-0000-0000-000000000000"),
                        new XElement("LinkUri", "http://apps.microsoft.com/webpdp/app/00000000-0000-0000-0000-000000000000"),
                        new XElement("CurrentMarket", "en-US"),
                        new XElement("AgeRating", "3"),
                            new XElement("MarketData", new XAttribute(xml + "lang", "en-us"),
                                new XElement("Name", "AppName"),
                                new XElement("Description", "AppDescription"),
                                new XElement("Price", "42.00"),
                                new XElement("CurrencySymbol", "$"),
                                new XElement("CurrencyCode", "USD"))));

                foreach (Product product in catalog.Products)
                {
                    listingInformationElement.Add(new XElement("Product",
                        new XAttribute("ProductId", product.Id),
                        new XAttribute("LicenseDuration", "0"),
                        new XAttribute("ProductType", "Durable"),
                        new XElement("MarketData", new XAttribute(xml + "lang", "en-us"),
                            new XElement("Name", product.Id),
                            new XElement("Price", "42.00"),
                            new XElement("CurrencySymbol", "$"),
                            new XElement("CurrencyCode", "USD"))));
                }

                XElement licenseInformationElement = new XElement("LicenseInformation",
                    new XElement("App",
                        new XElement("IsActive", "true"),
                        new XElement("IsTrial", "false")));

                rootElement.Add(listingInformationElement);
                rootElement.Add(licenseInformationElement);

                File.WriteAllText(Config.InAppTestConfigFile, rootElement.ToString());

                ServiceLocator.Logger.Log(LogLevel.Info, "Done.");
            }
            catch(Exception ex)
            {
                ServiceLocator.Logger.Log(LogLevel.Warning, string.Format("Failed to generate InApp test config.\n{0}", ex));
            }
        }
    }
}
