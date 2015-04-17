using System;

namespace Paragon.Container.Core
{
    internal interface IProductsDatabase
    {
        event Action<int> ProductAdded;
        event Action<int> ProductExpired;

        int ProductsCount { get; }

        void AddProduct(int productId);
        void AddProduct(int productId, DateTime Expire);
        bool HasProduct(int productId);
    }
}
