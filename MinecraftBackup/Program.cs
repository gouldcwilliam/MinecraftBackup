using System;
using System.IO;
using System.IO.Compression;

using MinecraftBackup.Properties;

namespace MinecraftBackup
{
	class Program
	{

		static void Main(string[] args)
		{
			string dateString =
				DateTime.Now.Year + "-" +
				DateTime.Now.Month + "-" +
				DateTime.Now.Day + "_" +
				DateTime.Now.Hour + "-" +
				DateTime.Now.Minute;

			Console.WriteLine(dateString);

			foreach (string mapName in Settings.Default._mapNames)
			{
				string mapDir = AddBackSlash(Settings.Default._mapDirectory);
				string mapPath = mapDir + mapName;
				Console.WriteLine(mapPath);

				string backupDir = AddBackSlash(Settings.Default._backupDirectory);
				string zipName = backupDir + mapName + "_" + dateString;
				Console.WriteLine(zipName);

				DirectoryCopy(mapPath, zipName, true);
			}

			if (System.Diagnostics.Debugger.IsAttached)
			{
				Console.Write("Press any key...."); Console.ReadKey();
			}
		}

		private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			DirectoryInfo[] dirs = dir.GetDirectories();
			// If the destination directory doesn't exist, create it.
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, false);
			}

			// If copying subdirectories, copy them and their contents to new location.
			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}
		static string AddBackSlash(string path)
		{
			string sep1 = Path.DirectorySeparatorChar.ToString();
			string sep2 = Path.AltDirectorySeparatorChar.ToString();

			if (path.EndsWith(sep1) || path.EndsWith(sep2)) { return path; }
			return path + sep1;

		}
	}
}
