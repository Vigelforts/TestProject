using System.Threading.Tasks;
using Paragon.Container.Core.Annotations;

namespace Paragon.Container.Core.DataSaving
{
    public interface ILanguagesFilterDataSavingService
    {
        void SaveCurrentPair(LanguagesFilterData data);
        Task<LanguagesFilterData> GetSavedPair();
    }
}