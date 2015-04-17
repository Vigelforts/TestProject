using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paragon.Container.Core
{
    public interface IUserInteraction
    {
        Task<bool> ClearHistoryRequest();
        Task<bool> ClearFavoritesRequest();
        Task RestorePurchaseMessage(List<string> products);
        Task ErrorMessage(string message);
        Task InformationMessage(string message);
    }
}
