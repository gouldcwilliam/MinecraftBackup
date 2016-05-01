using System;
using System.IO;
using System.Diagnostics;

using MinecraftBackup.Properties;

namespace MinecraftBackup
{
	class MinecraftBackup
	{

		static void Main(string[] args)
		{
			string dateFormat = "yyy-MM-dd_HH-mm";

			string dateString = DateTime.Now.ToString(dateFormat);

			// create log file
			if (!File.Exists(Settings.Default._logFile))
			{
				File.Create(Settings.Default._logFile).Close();
			}

			// initial entry
			Log();
			Log(DateTime.Now, "Starting MinecraftBackup");

			// set mcrcon arguments
			string mcrconArgs = string.Format("-H {0} -p {1} save-off save-all save-on", Settings.Default._rconHost, Settings.Default._rconPassword);
			Log("{0} - Executing {1}", DateTime.Now.ToString(), Settings.Default._rconEXE);

			// run mcrcon to save the map
			ExecuteCommand(Settings.Default._rconEXE, mcrconArgs);

			// back up all the maps in the settings file
			foreach (string mapName in Settings.Default._mapNames)
			{
				// build path
				string mapPath = string.Format("{0}{1}",
					AddBackSlash(Settings.Default._mapDirectory), 
					mapName);

				// build zip name
				string zipName = string.Format("{0}{1}_{2}",
					AddBackSlash(Settings.Default._backupDirectory),
					mapName,
					DateTime.Now.ToString(dateFormat));

				// build 7z arguments
				string zipArgs = string.Format("a -tzip {0} {1}", zipName, mapPath);
				Log("{0} - Executing {1} {2}", DateTime.Now.ToString(), Settings.Default._7zipExe, zipArgs);

				// create zip with 7z
				ExecuteCommand(Settings.Default._7zipExe, zipArgs);
			}

			// add trailing new line
			Log();

			//if(Debugger.IsAttached) Console.Write("Press any key");Console.ReadKey();
		}

		/// <summary>
		/// Executes external subproceess
		/// </summary>
		/// <param name="ExecutableFile"></param>
		/// <param name="Arguments"></param>
		/// <returns></returns>
		public static int ExecuteCommand(string ExecutableFile, string Arguments)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.FileName = ExecutableFile;
			startInfo.Arguments = Arguments;
			startInfo.CreateNoWindow = true;
			startInfo.UseShellExecute = false;
			Process process = Process.Start(startInfo);
			process.WaitForExit();
			Log("{0} - Finished {1}", DateTime.Now.ToString(), startInfo.FileName);
			return process.ExitCode;
		}

		/// <summary>
		/// Copies directory
		/// </summary>
		/// <param name="sourceDirName"></param>
		/// <param name="destDirName"></param>
		/// <param name="copySubDirs"></param>
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

		/// <summary>
		/// adds a blank line to the log
		/// </summary>
		static void Log()
		{
			if (System.Diagnostics.Debugger.IsAttached)
			{
				Console.WriteLine();
			}
			using (StreamWriter w = File.AppendText(Settings.Default._logFile))
			{
				w.WriteLine();
			}
		}
		/// <summary>
		/// logs the given string
		/// </summary>
		/// <param name="logText"></param>
		static void Log(string logText)
		{
			if (System.Diagnostics.Debugger.IsAttached)
			{
				Console.WriteLine("  {0}", logText);
			}
			// open log stream
			using (StreamWriter w = File.AppendText(Settings.Default._logFile))
			{
				w.WriteLine("  {0}", logText);
				w.Close();
			}
		}
		/// <summary>
		/// logs the string with the time
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="logText"></param>
		static void Log(DateTime dt, string logText)
		{
			if (System.Diagnostics.Debugger.IsAttached)
			{
				Console.WriteLine("* {0} - {1}", DateTime.Now.ToString(), logText);
				//Console.WriteLine("* {0} {1} - {2}", dt.ToLongTimeString(), dt.ToLongDateString(), logText);
			}
			using (StreamWriter w = File.AppendText(Settings.Default._logFile))
			{
				w.WriteLine("* {0} - {1}", DateTime.Now.ToString(), logText);
				//w.WriteLine("* {0} {1} - {2}", dt.ToLongTimeString(), dt.ToLongDateString(), logText);
			}
		}
		/// <summary>
		/// Clone of string.Format
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		/// <param name="arg3"></param>
		/// <param name="arg4"></param>
		static void Log(string format, object arg0, object arg1, object arg2, object arg3, object arg4)
		{
			string logText = string.Format(format, arg0, arg1, arg2, arg3, arg4);
			Log(logText);
		}
		static void Log(string format, object arg0, object arg1, object arg2, object arg3)
		{
			string logText = string.Format(format, arg0, arg1, arg2, arg3);
			Log(logText);
		}
		static void Log(string format, object arg0, object arg1, object arg2)
		{
			string logText = string.Format(format, arg0, arg1, arg2);
			Log(logText);
		}
		static void Log(string format, object arg0, object arg1)
		{
			string logText = string.Format(format, arg0, arg1);
			Log(logText);
		}
		static void Log(string format, object arg0)
		{
			string logText = string.Format(format, arg0);
			Log(logText);
		}

		/// <summary>
		/// adds backslash to paths
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		static string AddBackSlash(string path)
		{
			string sep1 = Path.DirectorySeparatorChar.ToString();
			string sep2 = Path.AltDirectorySeparatorChar.ToString();

			if (path.EndsWith(sep1) || path.EndsWith(sep2)) { return path; }
			return path + sep1;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		static bool IsFileLocked(FileInfo file)
		{
			FileStream stream = null;

			try
			{
				stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
			}
			catch (IOException)
			{
				//the file is unavailable because it is:
				//still being written to
				//or being processed by another thread
				//or does not exist (has already been processed)
				return true;
			}
			finally
			{
				if (stream != null)
					stream.Close();
			}

			//file is not locked
			return false;
		}
	}
}

/*
 * M = full month and day
 * MM = month #
 * MMM = abbreviated month
 * MMMMM = full month
 * d = mm/dd/yyyy
 * dd = day #
 * ddd = abbreviated day
 * dddd = full day
 * yy = year ##
 * yyy = year ####
 * m = full month and day
 * mm = minute #
 * HH = hour #
 */
