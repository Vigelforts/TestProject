using Cirrious.CrossCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Paragon.Container.Core
{
    internal sealed class ProductsDatabase : IProductsDatabase
    {
        public ProductsDatabase()
        {
            Common.IFileAccessorFactory fileAccessorFactory = Mvx.Resolve<Common.IFileAccessorFactory>();
            _fileAccessor = fileAccessorFactory.GetLocalFileAccessor();

            ReadDatabase();
        }

        public event Action<int> ProductAdded;
        public event Action<int> ProductExpired;

        public int ProductsCount
        {
            get
            {
                _locker.WaitOne();
                return _model.Products.Count;
            }
        }

        public void AddProduct(int productId)
        {
            _locker.WaitOne();
            lock (_locker)
            {
                DatabaseProductModel newProduct = new DatabaseProductModel();
                newProduct.Id = productId;
                newProduct.Date = DateTime.UtcNow.ToBinary();

                AddProductToDatabase(newProduct);
            }
        }
        public void AddProduct(int productId, DateTime expire)
        {
            _locker.WaitOne();
            lock (_locker)
            {
                DatabaseProductModel newProduct = new DatabaseProductModel();
                newProduct.Id = productId;
                newProduct.Date = DateTime.UtcNow.ToBinary();
                newProduct.Expire = expire.ToBinary();

                AddProductToDatabase(newProduct);
            }
        }
        public bool HasProduct(int productId)
        {
            _locker.WaitOne();
            lock (_locker)
            {
                List<DatabaseProductModel> products = _model.Products;
                foreach (DatabaseProductModel product in products)
                {
                    if (productId == product.Id)
                    {
                        if (!ValidateProduct(product))
                        {
                            return false;
                        }

                        return true;
                    }
                }

                return false;
            }
        }

        private void AddProductToDatabase(DatabaseProductModel model)
        {
            List<DatabaseProductModel> products = _model.Products.Where(p => p.Id == model.Id).ToList();
            if (products.Count != 0)
            {
                DatabaseProductModel existingProduct = products[0];
                if (existingProduct.Expire == 0)
                {
                    return;
                }
                
                DateTime existingProductExpire = DateTime.FromBinary(existingProduct.Expire);
                DateTime newProductExpire = DateTime.FromBinary(model.Expire);
                if (existingProductExpire > newProductExpire)
                {
                    return;
                }                
            }        

            _model.Products.Add(model);
            WriteDatabase();

            Common.Delegate.Call<int>(ProductAdded, model.Id);
        }

        private bool ValidateProduct(DatabaseProductModel model)
        {
            if (model.Expire != 0)
            {
                DateTime productExpire = DateTime.FromBinary(model.Expire);
                if (productExpire < DateTime.UtcNow)
                {
                    _model.Products.Remove(model);
                    WriteDatabase();
                    Common.Delegate.Call(ProductExpired, model.Id);
                    return false;
                }
            }

            return true;
        }

        private async void ReadDatabase()
        {
            _locker.Reset();
            await Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        string text = await _fileAccessor.ReadFile(DatabaseFileUri);
                        if (string.IsNullOrEmpty(text))
                        {
                            CreateModel();
                            return;
                        }

                        text = ProductsDatabaseEncrypter.Decrypt(text);

                        XmlSerializer serializer = new XmlSerializer(typeof(ProductsDatabaseModel));
                        using (TextReader reader = new StringReader(text))
                        {
                            _model = (ProductsDatabaseModel)serializer.Deserialize(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        CreateModel();
                    }
                    finally
                    {
                        _locker.Set();
                    }
                });
        }
        private async void WriteDatabase()
        {
            _locker.Reset();
            await Task.Factory.StartNew(async () =>
                {
                    _model.Version = DatabaseVersion;

                    XmlSerializer serializer = new XmlSerializer(typeof(ProductsDatabaseModel));
                    StringBuilder builder = new StringBuilder();
                    using (TextWriter writer = new StringWriter(builder))
                    {
                        serializer.Serialize(writer, _model);
                    }

                    string encodedText = ProductsDatabaseEncrypter.Encrypt(builder.ToString());
                    await _fileAccessor.WriteFile(DatabaseFileUri, encodedText);

                    _locker.Set();
                });
        }

        private void CreateModel()
        {
            _model = new ProductsDatabaseModel();
            _model.Products = new List<DatabaseProductModel>();
            _model.Version = DatabaseVersion;
        }

        private ProductsDatabaseModel _model;
        private readonly Common.IFileAccessor _fileAccessor;
        private readonly ManualResetEvent _locker = new ManualResetEvent(true);

        private const int DatabaseVersion = 2;
        private readonly Uri DatabaseFileUri = new Uri("products_data.bin", UriKind.Relative);
    }
}
