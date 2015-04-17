using System;

namespace BuildSystem
{
    public sealed class ArgumentsManager
    {
        public ArgumentsManager(string[] args)
        {
            FillArguments(args);
        }

        public bool IsValid { get; private set; }

        public int CatalogId { get; private set; }

        private void FillArguments(string[] args)
        {
            if (args.Length == 1)
            {
                int catalogId;
                if (int.TryParse(args[0], out catalogId))
                {
                    CatalogId = catalogId;
                    IsValid = true;
                    return;
                }
            }

            IsValid = false;
        }
    }
}
