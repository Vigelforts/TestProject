using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BuildSystem
{
    public class ProjectGenerator
    {
        public void CreateProject(int catalogId, string defaultLanguage)
        {
            _catalogId = catalogId;
            _defaultLanguage = defaultLanguage;

            ServiceLocator.Logger.Log(LogLevel.Info, "Start project generation...");

            CopySources();
            ReplaceCatalog();
            ReplaceSources();
            ReplaceStrings();
            ReplaceBases();
            UpdateProjectFile();

            ServiceLocator.Logger.Log(LogLevel.Info, "Project generation complited.");
        }

        private void CopySources()
        {
            Directory.CreateDirectory(Config.ProjectFolder);
            DirectoryInfo sourceDirectory = new DirectoryInfo(System.Environment.CurrentDirectory + Config.SourcesPath);
            CopyDirectory(sourceDirectory.FullName, Config.ProjectFolder);
        }
        private void ReplaceCatalog()
        {
            File.Copy(Config.CatalogFile, Config.ProjectCatalogFile, true);
        }
        private void ReplaceSources()
        {
            string targetSrcPath = string.Format(Config.TargetSrcPathFormat, _catalogId.ToString());
            DirectoryInfo srcDirectory = new DirectoryInfo(System.Environment.CurrentDirectory + targetSrcPath);
            if (srcDirectory.Exists)
            {
                CopyDirectory(srcDirectory.FullName, Path.Combine(Config.ProjectFolder, "Win81App"));
            }
        }
        
        private void ReplaceStrings()
        {
            string targetStringsPath = string.Format(Config.TargetStringsPathFormat, _catalogId.ToString());
            DirectoryInfo stringsDirectory = new DirectoryInfo(System.Environment.CurrentDirectory + targetStringsPath);
            if (stringsDirectory.Exists)
            {
                foreach (FileInfo file in stringsDirectory.EnumerateFiles())
                {
                    string fileName = Path.GetFileNameWithoutExtension(file.Name);

                    string[] parts = fileName.Split('_');
                    if (parts.Length == 2 && parts[0] == "Resources")
                    {
                        string lang = parts[1];
                        UpdateStrings(file, lang);
                    }
                }
            }
        }
        private void UpdateStrings(FileInfo file, string lang)
        {
            DirectoryInfo resourcesDirectory = new DirectoryInfo(Config.ProjectResourcesFolder);
            if (resourcesDirectory.Exists)
            {
                foreach (DirectoryInfo langDir in resourcesDirectory.EnumerateDirectories())
                {
                    if (langDir.Name.ToLower() == lang.ToLower())
                    {
                        FileInfo stringsFile = new FileInfo(Path.Combine(langDir.FullName, "Resources.resw"));
                        UpdateStringFile(stringsFile, file);
                        break;
                    }
                }
            }
        }
        private static void UpdateStringFile(FileInfo stringsFile, FileInfo updateFile)
        {
            XDocument stringsXml = XDocument.Load(stringsFile.FullName);

            XDocument updates = XDocument.Load(updateFile.FullName);
            foreach (XElement element in updates.Root.Elements("data"))
            {
                foreach (XElement srcElement in stringsXml.Root.Elements("data"))
                {
                    if (srcElement.Attribute("name").Value == element.Attribute("name").Value)
                    {
                        srcElement.Element("value").Value = element.Element("value").Value;
                    }
                }
            }

            File.WriteAllText(stringsFile.FullName, stringsXml.ToString());
        }
        
        private void ReplaceBases()
        {
            Directory.Delete(Config.ProjectBasesFolder, true);
            Directory.CreateDirectory(Config.ProjectBasesFolder);

            DirectoryInfo basesDirectory = new DirectoryInfo(Config.BasesFolder);
            FileInfo[] files = basesDirectory.GetFiles();
            foreach (FileInfo file in files)
            {
                string destPath = Path.Combine(Config.ProjectBasesFolder, file.Name);
                file.CopyTo(destPath, true);

                _basesNames.Add(file.Name);
            }
        }
        
        private void UpdateProjectFile()
        {
            string text = File.ReadAllText(Config.ProjectFile);
            XDocument project = XDocument.Parse(text);
            XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";

            UpdateProjectBases(project, ns);
            UpdateProjectDefaultLanguage(project, ns);

            project.Save(Config.ProjectFile, SaveOptions.None);
        }
        private void UpdateProjectBases(XDocument project, XNamespace ns)
        {
            project.Root
                .Elements(ns + "ItemGroup")
                .Elements(ns + "Content")
                .Attributes("Include")
                .Where(a => a.Value.StartsWith("Bases\\") && a.Value.EndsWith(".sdc"))
                .Remove();

            project.Root
                .Elements(ns + "ItemGroup")
                .Elements(ns + "Content")
                .Where(e => !e.HasAttributes)
                .Remove();

            foreach (string baseFile in _basesNames)
            {
                project.Root.Element(ns + "ItemGroup").Add(
                    new XElement(ns + "Content",
                        new XAttribute("Include", "Bases\\" + baseFile)));
            }
        }
        private void UpdateProjectDefaultLanguage(XDocument project, XNamespace ns)
        {
            project.Root
                .Elements(ns + "PropertyGroup")
                .Elements(ns + "DefaultLanguage")
                .First().Value = _defaultLanguage;
        }

        private static void CopyDirectory(string sourcePath, string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            DirectoryInfo sourceDirectory = new DirectoryInfo(sourcePath);

            FileInfo[] files = sourceDirectory.GetFiles();
            foreach (FileInfo file in files)
            {
                if (NeedIngore(file.FullName))
                {
                    continue;
                }

                string tempPath = Path.Combine(destPath, file.Name);
                file.CopyTo(tempPath, true);
            }

            DirectoryInfo[] subdirectories = sourceDirectory.GetDirectories();
            foreach (DirectoryInfo subdirectory in subdirectories)
            {
                if (NeedIngore(subdirectory.Name))
                {
                    continue;
                }

                string temppath = Path.Combine(destPath, subdirectory.Name);
                CopyDirectory(subdirectory.FullName, temppath);
            }
        }
        private static bool NeedIngore(string name)
        {
            foreach (string item in Config.IgnoreList)
            {
                if (item == name)
                {
                    return true;
                }
            }

            return false;
        }

        private int _catalogId;
        private string _defaultLanguage;
        private List<string> _basesNames = new List<string>();
    }
}
