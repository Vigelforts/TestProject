using Cirrious.MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paragon.Container.Core
{
    internal interface IProductsCatalog
    {
        event Action<int> ProductPurchased;
        event Action<DictionaryInfo> ProductLaunched;

        int Id { get; }

        Common.Mvvm.Command UpdatePurchasesStatusCommand { get; }

        IReadOnlyCollection<Product> GetProducts();        
        Task<bool> EnterCode(string key);
    }
}
