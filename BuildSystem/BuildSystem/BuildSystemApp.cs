using Paragon.Container.Models;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace BuildSystem
{
    public sealed class BuildSystemApp
    {
        public BuildSystemApp()
        {
            Setup();
        }

        public int Start(string[] args)
        {
            try
            {
                _argumentsManager = new ArgumentsManager(args);
                if (!_argumentsManager.IsValid)
                {
                    ServiceLocator.Logger.Log(LogLevel.Warning, "Incorrect arguments.");
                    return 1;
                }

                ReadBuildConfig();
                Clean();
                CreateCatalog();
                SaveCatalog();
                CreateProject();
                BuildPackages();

                ServiceLocator.Logger.Log(LogLevel.Info, "Completed.");
            }
            catch
            {
                return 2;
            }

            return 0;
        }

        private void Setup()
        {
            ServiceLocator.Logger = new ConsoleLogger();
        }

        private void ReadBuildConfig()
        {
            _buildConfig = new BuildConfig(string.Format(Config.BuildConfigPathFormat, _argumentsManager.CatalogId));
        }
        private void Clean()
        {
            if (Directory.Exists(Config.TargetFolder))
            {
                Directory.Delete(Config.TargetFolder, true);
            }

            Directory.CreateDirectory(Config.TargetFolder);
        }
        private void CreateCatalog()
        {
            CatalogGenerator catalogGenerator = new CatalogGenerator();
            _catalog = catalogGenerator.CreateCatalog(_argumentsManager.CatalogId);
        }
        private void SaveCatalog()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ProductsCatalog));
                StringBuilder builder = new StringBuilder();
                using (TextWriter writer = new StringWriter(builder))
                {
                    serializer.Serialize(writer, _catalog);
                }

                string serializedCatalog = builder.ToString();

                File.WriteAllText(Config.CatalogFile, serializedCatalog);
            }
            catch (Exception ex)
            {
                throw new BuildSystemException("Failed to save catalog.", ex);
            }
        }
        private void CreateProject()
        {
            ProjectGenerator projectGenerator = new ProjectGenerator();
            projectGenerator.CreateProject(_argumentsManager.CatalogId, _buildConfig.DefaultLanguage);
        }
        private void BuildPackages()
        {
            PackagesBuilder packagesBuilder = new PackagesBuilder();

            if (_buildConfig.NeedInAppTestPackage)
            {
                packagesBuilder.BuildInAppTestPackage();
                InAppTestConfigGenerator.CreateInAppTestConfig(_catalog);
            }

            if (_buildConfig.NeedReleasePackage)
            {
                packagesBuilder.BuildReleasePackage();
            }
        }

        private ArgumentsManager _argumentsManager;
        private BuildConfig _buildConfig;
        private ProductsCatalog _catalog;
    }
}
