using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace BuildSystem
{
    public sealed class BuildConfig
    {
        public BuildConfig(string filePath)
        {
            DefaultLanguage = string.Empty;
            NeedInAppTestPackage = false;
            NeedReleasePackage = false;

            Parse(filePath);
            Validate();
        }

        public string DefaultLanguage { get; private set; }
        public bool NeedInAppTestPackage { get; private set; }
        public bool NeedReleasePackage { get; private set; }

        private void Parse(string filePath)
        {
            try
            {
                string file = File.ReadAllText(filePath);

                XmlSerializer serializer = new XmlSerializer(typeof(BuildConfigModel));
                using (TextReader reader = new StringReader(file))
                {
                    BuildConfigModel model = (BuildConfigModel)serializer.Deserialize(reader);
                    ParseModel(model);
                }
            }
            catch (Exception ex)
            {
                throw new BuildSystemException("Failed to read build config file.", ex);
            }
        }
        private void ParseModel(BuildConfigModel model)
        {
            DefaultLanguage = model.DefaultLanguage.Trim();

            string[] packageTypes = model.PackageType.Trim().Split('|');

            if (packageTypes.Contains("in_app_test"))
            {
                NeedInAppTestPackage = true;
            }

            if (packageTypes.Contains("release"))
            {
                NeedReleasePackage = true;
            }
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(DefaultLanguage))
            {
                throw new BuildSystemException("Build config error: default language is empty.");
            }

            if (!NeedInAppTestPackage && !NeedReleasePackage)
            {
                throw new BuildSystemException("Build config error: package type not selected.");
            }
        }
    }

    [XmlRoot("BuildConfig")]
    public class BuildConfigModel
    {
        public string DefaultLanguage { get; set; }
        public string PackageType { get; set; }
    }
}
