using System;
using System.IO;

namespace BuildSystem
{
    internal static class Config
    {
        public readonly static string DataPathFormat = "data/{0}/";
        public readonly static string BuildConfigPathFormat = DataPathFormat + "build_config.xml";

        public readonly static string TargetFolder = "build_result";
        public readonly static string PdahpcCatalogsFolder = Path.Combine(TargetFolder, "pdahpc_catalogs");
        public readonly static string BasesFolder = Path.Combine(TargetFolder, "bases");
        public readonly static string CatalogFile = Path.Combine(TargetFolder, "catalog.xml");
        public readonly static string InAppTestConfigFile = Path.Combine(TargetFolder, "WindowsStoreProxy.xml");
        public readonly static string PackagesFolder = Path.Combine(TargetFolder, "packages");
        
        public readonly static string ProjectFolder = Path.Combine(TargetFolder, "project");
        public readonly static string ProjectCatalogFile = Path.Combine(ProjectFolder, "Win81App", "catalog.xml");
        public readonly static string ProjectBasesFolder = Path.Combine(ProjectFolder, "Win81App", "Bases");
        public readonly static string ProjectFile = Path.Combine(ProjectFolder, "Win81App", "Win81App.csproj");
        public readonly static string ProjectResourcesFolder = Path.Combine(ProjectFolder, "Win81App", "Resources");
        public readonly static string ProjectSetupFile = Path.Combine(ProjectFolder, "Win81App", "Setup.cs");

        public readonly static string SourcesPath = "/../..";

        public readonly static string[] IgnoreList = { ".svn", "bin", "obj", "Container.v12.suo", "BuildSystem" };

        public readonly static string CatalogV6UrlFormat = "http://sou.penreader.com/fcgid/sou.fcg?catalog_id={0}&catalog_ver=0&compress=0&protocol=6&shell_id=3733&development=1&rebuild_cache=true";
        public readonly static string CatalogV8UrlFormat = "http://sou.penreader.com/?protocol=8&catalog_id={0}&rebuild_cache=true";

        public const string CatalogBaseFileFormat = "ms-appx:///Bases/{0}.sdc";
        public const int CatalogVersion = 1;
        
        public readonly static string TargetSrcPathFormat = "/data/{0}/src/";
        public readonly static string TargetStringsPathFormat = "/data/{0}/strings/";

        public readonly static string MsBuildPath = @"C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe";
    }
}
