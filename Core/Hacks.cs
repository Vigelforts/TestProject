using Cirrious.CrossCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Paragon.Container.Core
{
    internal static class Hacks
    {
        public static bool NeedSwitchShowVariants(string baseId)
        {
            return BasesWithWrongShowVariants.Contains(baseId);
        }
        public static bool NeedExcludeAddtionalList(int listTypeId)
        {
            foreach (int wrongListTypeId in WrongAddtionalLists)
            {
                if (listTypeId == wrongListTypeId)
                {
                    return true;
                }
            }

            return false;
        }
        public static bool NeedDisableColumns()
        {
            IProductsCatalog productsCatalog = Mvx.Resolve<IProductsCatalog>();
            if (productsCatalog.Id == 833)
            {
                IDictionaryService dictionaryService = Mvx.Resolve<IDictionaryService>();
                int productId = dictionaryService.ProductId;

                IReadOnlyCollection<Product> products = productsCatalog.GetProducts();
                Product product = products.Where(p => p.Id == productId).First();

                switch (product.Level)
                {
                    case ProductLevel.Advanced:
                    case ProductLevel.School:
                        return true;
                }
            }

            return false;
        }
        public static bool NeedDisableAutoSwitchDirection(string baseId)
        {
            return BasesWhichNeedDisableAutoSwitchDirection.Contains(baseId);
        }
        public static bool IsBaseWithDudenMorphomagic(string baseId)
        {
            return BasesWithDudenMorphomagic.Contains(baseId);
        }

        private static readonly string[] BasesWithWrongShowVariants = { "531B", "531A", "5319" };
        private static readonly int[] WrongAddtionalLists = { 771, 776 };
        private static readonly string[] BasesWhichNeedDisableAutoSwitchDirection = { "515E", "527E", "53E9", "53EA", "53EB", "53EC", "53ED", "52D4", "53B8", "53B9", "53C8", "53CF", "53D1", "53D2", "53D8", "53F1", "53F2", "53F6", "515A", "515B", "515C", "516A", "516B", "516C", "516D", "527F", "529B", "530F", "532B", "538C", "543D", "5000", "5001", "5002", "5003", "5158", "5159", "5160", "5161", "5163", "5164", "5165", "5167", "5168", "5169", "5250", "5252", "5278", "5280", "5324", "5357", "5381", "5383", "5384", "5385", "5404", "5405", "5421", "5422" };
        private static readonly string[] BasesWithDudenMorphomagic = { "52C4" };
    }
}
