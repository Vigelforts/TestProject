using System;

namespace Paragon.Container.Core
{
    public interface ITopItemsController
    {
        bool CanAddTopItem(bool hasSubtitle);
        void Reset();
    }
}
