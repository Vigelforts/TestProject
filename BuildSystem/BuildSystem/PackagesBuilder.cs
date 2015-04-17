using System;
using System.IO;

namespace BuildSystem
{
    public class PackagesBuilder
    {
        public void BuildInAppTestPackage()
        {
            ServiceLocator.Logger.Log(LogLevel.Info, "Building in app test package...");

            string setupFile = File.ReadAllText(Config.ProjectSetupFile);
            setupFile = setupFile.Replace(
                "Mvx.RegisterSingleton<Common.IInAppService>(new Common.InAppService());",
                "Mvx.RegisterSingleton<Common.IInAppService>(new Common.TestInAppService());");

            File.WriteAllText(Config.ProjectSetupFile, setupFile);

            string outputDir = Path.Combine(Environment.CurrentDirectory, Config.PackagesFolder, "InAppTest");
            Build(outputDir);            

            ServiceLocator.Logger.Log(LogLevel.Info, "Done.");
        }

        public void BuildReleasePackage()
        {
            ServiceLocator.Logger.Log(LogLevel.Info, "Building release package...");

            string setupFile = File.ReadAllText(Config.ProjectSetupFile);
            setupFile = setupFile.Replace(
                "Mvx.RegisterSingleton<Common.IInAppService>(new Common.TestInAppService());",
                "Mvx.RegisterSingleton<Common.IInAppService>(new Common.InAppService());");

            File.WriteAllText(Config.ProjectSetupFile, setupFile);

            string outputDir = Path.Combine(Environment.CurrentDirectory, Config.PackagesFolder, "Release");
            Build(outputDir);

            ServiceLocator.Logger.Log(LogLevel.Info, "Done.");
        }

        private void Build(string outputDir)
        {
            string parameters = string.Format("{0}/Container.sln /p:Configuration=Release /t:Clean;Rebuild",
                Config.ProjectFolder);

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = Config.MsBuildPath;
            process.StartInfo.Arguments = parameters;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                ServiceLocator.Logger.Log(LogLevel.Error, output);
            }

            CopyDirectory(Path.Combine(Config.ProjectFolder, "Win81App", "AppPackages"), outputDir);
        }

        private void CopyDirectory(string sourceDirName, string destDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                CopyDirectory(subdir.FullName, temppath);
            }
        }
    }
}
