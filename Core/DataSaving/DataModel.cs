using System;
using System.Collections.Generic;

namespace Paragon.Container.Core.DataSaving
{
    public struct DataItem
    {
        public string Title { get; set; }
        public int Language { get; set; }
    }

    public sealed class Data
    {
        public Data()
        {
            Verison = FormatVersion;
            Items = new List<DataItem>();
        }

        public int Verison { get; set; }
        public List<DataItem> Items { get; set; }

        public static int FormatVersion = 2;
    }
}
