using System;

namespace Paragon.Container.Core
{
    public interface IAppStylesService
    {
        void SetDefaultColors();
        void SetColor(string title, string value);
    }
}
