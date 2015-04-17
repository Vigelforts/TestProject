using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paragon.Container.Core.DataSaving.DataAccessors;

namespace Paragon.Container.Core.DataSaving
{

    public class LanguagesFilterData
    {
        public LanguagesFilterData(string l1, string l2)
        {
            Language1 = l1;
            Language2 = l2;
        }

        public LanguagesFilterData()
        {
            
        }
        
        public string Language1 { get; set; }
        public string Language2 { get; set; }
    }

    public class LanguageFilterDataSavingService:ILanguagesFilterDataSavingService
    {
        private DataAccessorGeneric<LanguagesFilterData> _dataAccessor;

        public LanguageFilterDataSavingService()
        {
            var fileUri = new Uri("lang.dat", UriKind.Relative);
            _dataAccessor = new DataAccessorGeneric<LanguagesFilterData>(fileUri);
        }

        public async void SaveCurrentPair(LanguagesFilterData data)
        {
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    await _dataAccessor.WriteData(data);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                }
            });
        }


        public Task<LanguagesFilterData> GetSavedPair()
        {

               var result = _dataAccessor.ReadData();
                return result;


            
        }


    }
}
