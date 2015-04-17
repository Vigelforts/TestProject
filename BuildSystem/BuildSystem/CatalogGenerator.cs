using Paragon.Container.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace BuildSystem
{
    public sealed class CatalogGenerator
    {
        public ProductsCatalog CreateCatalog(int catalogId)
        {
            _catalogId = catalogId;

            ServiceLocator.Logger.Log(LogLevel.Info, "Start catalog generation...");

            RequestPdahpcCatalogs();
            SavePdahpcCatalogs();
            ProductsCatalog catalog = GenerateCatalog();

            ServiceLocator.Logger.Log(LogLevel.Info, string.Format("Catalog generation completed, {0} products.", catalog.Products.Count));

            return catalog;
        }

        private void RequestPdahpcCatalogs()
        {
            ServiceLocator.Logger.Log(LogLevel.Info, "Request catalogs from server...");

            try
            {
                _pdahpcCatalogV6 = Utils.ExecuteHttpRequest(string.Format(Config.CatalogV6UrlFormat, _catalogId));
                _pdahpcCatalogV8 = Utils.ExecuteHttpRequest(string.Format(Config.CatalogV8UrlFormat, _catalogId));
            }
            catch (Exception ex)
            {
                throw new BuildSystemException("Failed to request catalog from server.", ex);
            }

            ServiceLocator.Logger.Log(LogLevel.Info, "Request catalogs from server completed.");
        }
        private void SavePdahpcCatalogs()
        {
            try
            {
                Directory.CreateDirectory(Config.PdahpcCatalogsFolder);
                File.WriteAllText(string.Format("{0}/{1}_v6.xml", Config.PdahpcCatalogsFolder, _catalogId), _pdahpcCatalogV6);
                File.WriteAllText(string.Format("{0}/{1}_v8.xml", Config.PdahpcCatalogsFolder, _catalogId), _pdahpcCatalogV8);
            }
            catch (Exception ex)
            {
                ServiceLocator.Logger.Log(LogLevel.Warning, string.Format("Failed to save pdahpc catalogs.\n{0}", ex));
            }
        }

        private ProductsCatalog GenerateCatalog()
        {
            try
            {
                IEnumerable<XElement> addons = GetAddons();

                XmlCatalogCreator catalogCreator = new XmlCatalogCreator(_catalogId);
                catalogCreator.AddAddons(addons);

                XElement catalogV8root = XDocument.Parse(_pdahpcCatalogV8).Root.Element("catalog");
                foreach (XElement product in catalogV8root.Elements("product"))
                {
                    catalogCreator.AddProduct(product);
                }

                return catalogCreator.GetResult();
            }
            catch (Exception ex)
            {
                throw new BuildSystemException("Failed to generate catalog.", ex);
            }
        }
        private IEnumerable<XElement> GetAddons()
        {
            XElement catalogV6root = XDocument.Parse(_pdahpcCatalogV6).Root.Element("catalog");
            return catalogV6root.Elements("addon");
        }

        private int _catalogId;
        private string _pdahpcCatalogV6;
        private string _pdahpcCatalogV8;
    }

    internal class XmlCatalogCreator
    {
        public XmlCatalogCreator(int catalogId)
        {
            _catalog = new ProductsCatalog();
            _catalog.Id = catalogId;
            _catalog.Version = Config.CatalogVersion;
            _catalog.Products = new List<Product>();
        }

        public void AddAddons(IEnumerable<XElement> addons)
        {
            _addons = addons;
        }
        public void AddProduct(XElement productElement)
        {
            Product productModel = new Product();
            productModel.Id = int.Parse(productElement.Attribute("id").Value);
            productModel.Level = GetLevel(productElement);
            productModel.RemovedFromSale = GetRemovedFromSaleAttribute(productElement);
            productModel.Strings = GetStrings(productElement);
            productModel.Colors = GetColors(productElement);
            productModel.Icon = DownloadIcon(productModel.Id);
            productModel.Bases = GetBases(productElement);

            XAttribute lang1Attribute = productElement.Attribute("lang1");
            if (lang1Attribute != null)
            {
                productModel.Language1 = lang1Attribute.Value;
            }

            XAttribute lang2Attribute = productElement.Attribute("lang2");
            if (lang2Attribute != null)
            {
                productModel.Language2 = lang2Attribute.Value;
            }

            XAttribute priorityAttribute = productElement.Attribute("priority");
            if (priorityAttribute != null)
            {
                productModel.Priority = int.Parse(priorityAttribute.Value);
            }

            AddMorphoBases(ref productModel);

            _catalog.Products.Add(productModel);
        }

        public ProductsCatalog GetResult()
        {
            return _catalog;
        }

        private string GetLevel(XElement productElement)
        {
            try
            {
                return productElement.Attribute("level").Value;
            }
            catch
            {
                ServiceLocator.Logger.Log(LogLevel.Warning, "Level attribute not found");
            }

            return string.Empty;
        }
        private List<ProductStrings> GetStrings(XElement productElement)
        {
            List<ProductStrings> result = new List<ProductStrings>();

            foreach (XElement stringsElement in productElement.Elements("strings"))
            {
                ProductStrings strings = new ProductStrings();

                strings.Locale = stringsElement.Attribute("locale").Value;

                XElement nameElement = stringsElement.Element("name");
                if (nameElement != null)
                {
                    strings.Name = nameElement.Attribute("text").Value;
                }

                XElement descriptionElement = stringsElement.Element("description");
                if (descriptionElement != null)
                {
                    strings.Description = descriptionElement.Value;
                }

                result.Add(strings);
            }

            return result;
        }
        private List<ProductColor> GetColors(XElement productElement)
        {
            List<ProductColor> result = new List<ProductColor>();

            try
            {
                string genericDataElementValue = productElement.Element("generic_data").Value;
                string colors = genericDataElementValue.Replace("<![CDATA[", "").Replace("]]", "").Replace("\n", "");

                string[] colorsList = colors.Split(';');
                foreach (string color in colorsList)
                {
                    string[] colorParts = color.Split('=');

                    ProductColor productColor = new ProductColor();
                    productColor.Title = colorParts[0];
                    productColor.Value = colorParts[1];

                    result.Add(productColor);
                }

            }
            catch (Exception)
            {
            }

            return result;
        }
        private List<PBase> GetBases(XElement productElement)
        {
            List<PBase> result = new List<PBase>();

            foreach (XElement baseElement in productElement.Elements("component"))
            {
                PBase? baseModel = ParseBase(baseElement);
                if (baseModel.HasValue)
                {
                    result.Add(baseModel.Value);
                }
            }

            return result;
        }
        private bool GetRemovedFromSaleAttribute(XElement productElement)
        {
            try
            {
                XAttribute attribute = productElement.Attribute("removed_from_sale");
                if (attribute != null)
                {
                    return bool.Parse(attribute.Value);
                }
            }
            catch
            {
            }

            return false;
        }
        private PBase? ParseBase(XElement baseElement)
        {
            PBase baseModel = new PBase();

            XAttribute prcIdAttribute = baseElement.Attribute("prc_id");
            if (prcIdAttribute == null)
            {
                return null;
            }

            baseModel.Id = prcIdAttribute.Value;
            baseModel.Size = long.Parse(baseElement.Attribute("size").Value);
            baseModel.Type = baseElement.Attribute("type").Value;

            XAttribute amazonUrlAttribute = baseElement.Attribute("amazon_url");
            if (amazonUrlAttribute != null)
            {
                baseModel.Url = amazonUrlAttribute.Value;
            }

            XAttribute langFromAttribute = baseElement.Attribute("from_lang");
            if (langFromAttribute != null)
            {
                baseModel.LanguageFrom = langFromAttribute.Value;
            }

            XAttribute langToAttribute = baseElement.Attribute("to_lang");
            if (langToAttribute != null)
            {
                baseModel.LanguageTo = langToAttribute.Value;
            }

            XAttribute langAttribute = baseElement.Attribute("lang");
            if (langAttribute != null)
            {
                baseModel.LanguageTo = langAttribute.Value;
                baseModel.LanguageFrom = langAttribute.Value;
            }

            XAttribute majorVerAttribute = baseElement.Attribute("major_ver");
            if (majorVerAttribute != null)
            {
                baseModel.MajorVersion = int.Parse(majorVerAttribute.Value);
            }

            XAttribute minorVerAttribute = baseElement.Attribute("minor_ver");
            if (minorVerAttribute != null)
            {
                baseModel.MinorVersion = int.Parse(minorVerAttribute.Value);
            }

            if (baseModel.Type == "demo_dict")
            {
                XAttribute urlAttribute = baseElement.Attribute("url");
                if (urlAttribute == null)
                {
                    return null;
                }

                DownloadBase(baseModel.Id, urlAttribute.Value);
                baseModel.Url = string.Format(Config.CatalogBaseFileFormat, baseModel.Id);
            }

            return baseModel;
        }
        private PBase? ParseMorphoBase(XElement addonElement)
        {
            PBase baseModel = new PBase();

            XAttribute urlAttribute = addonElement.Attribute("url");
            if (urlAttribute == null)
            {
                return null;
            }

            Uri baseUri = new Uri(urlAttribute.Value);

            baseModel.Id = System.IO.Path.GetFileNameWithoutExtension(baseUri.LocalPath);
            baseModel.Url = string.Format(Config.CatalogBaseFileFormat, baseModel.Id);
            baseModel.Size = long.Parse(addonElement.Attribute("size").Value);
            baseModel.Type = addonElement.Attribute("type").Value;

            XAttribute langAttribute = addonElement.Attribute("lang");
            if (langAttribute != null)
            {
                baseModel.LanguageTo = langAttribute.Value;
                baseModel.LanguageFrom = langAttribute.Value;
            }

            DownloadBase(baseModel.Id, baseUri.OriginalString);

            return baseModel;
        }

        private void AddMorphoBases(ref Product productModel)
        {
            if (_addons != null)
            {
                List<string> languages = new List<string>();
                Dictionary<string, bool> languagesFlags = new Dictionary<string, bool>();
                languages.Add(productModel.Language1);
                languagesFlags.Add(productModel.Language1, false);
                if (productModel.Language1 != productModel.Language2)
                {
                    languages.Add(productModel.Language2);
                    languagesFlags.Add(productModel.Language2, false);
                }

                var morphoAddons = from e in _addons
                                   where e.Attribute("id").Value.Contains("Slovoed")
                                   where e.Attribute("type").Value == "morphology"
                                   select e;

                foreach (XElement addonElement in morphoAddons)
                {
                    foreach (string language in languages)
                    {
                        if (!languagesFlags[language] && language.ToLower() == addonElement.Attribute("lang").Value.ToLower())
                        {
                            PBase? baseModel = ParseMorphoBase(addonElement);
                            if (baseModel.HasValue)
                            {
                                productModel.Bases.Add(baseModel.Value);
                                languagesFlags[language] = true;
                            }
                        }
                    }
                }
            }
        }

        private string DownloadIcon(int productId)
        {
            ServiceLocator.Logger.Log(LogLevel.Info, string.Format("Downloading icon for product {0}...", productId));
            try
            {
                WebClient webClient = new WebClient();
                byte[] iconData = webClient.DownloadData(string.Format(IconUrl, productId, 512));
                return Convert.ToBase64String(iconData);
            }
            catch
            {
                ServiceLocator.Logger.Log(LogLevel.Warning, string.Format("Failed to download icon for product {0}.", productId));
                return null;
            }
        }

        private static void DownloadBase(string baseId, string url)
        {
            try
            {
                string path = string.Format(Config.BasesFolder);
                string fullPath = string.Format("{0}/{1}.sdc", path, baseId);

                if (File.Exists(fullPath))
                {
                    ServiceLocator.Logger.Log(LogLevel.Info, string.Format("Base {0} exist.", baseId));
                    return;
                }

                ServiceLocator.Logger.Log(LogLevel.Info, string.Format("Downloading base {0}...", baseId));

                Directory.CreateDirectory(path);

                WebClient webClient = new WebClient();
                webClient.DownloadFile(url, string.Format("{0}/{1}.sdc", path, baseId));
            }
            catch
            {
                ServiceLocator.Logger.Log(LogLevel.Warning, string.Format("Failed to download base {0}.", baseId));
            }
        }

        private ProductsCatalog _catalog;
        private IEnumerable<XElement> _addons;

        private const string IconUrl = "http://sou.penreader.com/dictionaries/{0}/icon/?size={1}";
    }
}
