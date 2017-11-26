﻿using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using Codeplex.Data;
using System.Windows.Forms;
using System.Xml.Linq;
using AppSettings = ElectronicObserver.Properties.Settings;

namespace ElectronicObserver.Utility
{
	internal class SoftwareUpdater
	{
		public static bool UpdateRestart = false;
		
		internal static readonly string AppDataFolder =
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\\ElectronicObserver";
		internal static readonly string TranslationFolder = AppDataFolder + "\\Translations";
		private static readonly string UpdateFile = AppDataFolder + @"\\latest.zip";
		private static string _downloadHash = string.Empty;
		private static readonly Uri UpdateUrl =
			new Uri("http://raw.githubusercontent.com/silfumus/ryuukitsune.github.io/master/Translations/en-US/update.json");

		public static void UpdateSoftware()
		{
			if (!Directory.Exists(AppDataFolder))
				Directory.CreateDirectory(AppDataFolder);

			var jsonurl = "";
			try
			{
				using (var client = WebRequest.Create(UpdateUrl).GetResponse())
				{
					var updateData = client.GetResponseStream();
					var json = DynamicJson.Parse(updateData);
					jsonurl = json.url;
					_downloadHash = json.hash;
				}
			}
			catch (Exception e)
			{
				Logger.Add(3, "Failed to download update info. " + e);
			}
			var downloadUrl = new Uri(jsonurl);

			if (!File.Exists(UpdateFile))
				DownloadUpdate(downloadUrl);
			else if (GetFileHash(UpdateFile) != _downloadHash)
			{
				File.Delete(UpdateFile);
				DownloadUpdate(downloadUrl);
			}
			else
			{
				Logger.Add(2, "Close Electronic Observer to complete the update.");
			}

			var destPath =
				Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
			UpdateUpdater(UpdateFile, destPath);

			

			var updater = new Process
			{
				StartInfo =
				{
					FileName = Application.StartupPath + @"\EOUpdater.exe",
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};
			if (!UpdateRestart)
				updater.StartInfo.Arguments = "--restart";

			updater.Start();
		}

		private static string GetFileHash(string filename)
		{
			using (var sha256 = SHA256.Create())
			{
				using (var stream = File.OpenRead(filename))
				{
					var hash = sha256.ComputeHash(stream);
					return BitConverter.ToString(hash).Replace("-", "");
				}
			}
		}

		private static void DownloadUpdate(Uri url)
		{
			try
			{
				using (var client = new WebClient())
				{
					Logger.Add(2, "Downloading new version of Electronic Observer...");
					client.DownloadFileCompleted += DownloadComplete;
					client.DownloadFileAsync(url, UpdateFile);
				}
			}
			catch (Exception e)
			{
				Logger.Add(3, "Failed to download update file." + e);
			}

		}

		internal static void DownloadTranslation(TranslationFile filename, string latestVersion)
		{
			var url = AppSettings.Default.EOTranslations.AbsoluteUri + "en-US";
			try
			{
				var r2 = WebRequest.Create(url + $"/{filename}.xml");
				using (var resp = r2.GetResponse())
				{
					var doc = XDocument.Load(resp.GetResponseStream());
					doc.Save(TranslationFolder + $"\\{filename}.xml");
				}
				Logger.Add(2, $"Updated {filename} translations to v{latestVersion}.");
			}
			catch (Exception e)
			{
				Logger.Add(3, $"Failed to download {filename}.xml. " + e.Message);
			}

		}

		private static void UpdateUpdater(string zipPath, string extractPath)
		{
			var localPath = new Uri(extractPath).LocalPath;
			using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Read))
			{
				foreach (var file in archive.Entries)
				{
					var fullname = file.FullName.Replace(@"ElectronicObserver/", "");
					var completeFileName = Path.Combine(localPath, fullname);

					if (file.Name != "EOUpdater.exe") continue;

					file.ExtractToFile(completeFileName, true);
				}
			}
		}

		private static void DownloadComplete(object sender, EventArgs e)
		{
			Logger.Add(2, "Download complete. Close Electronic Observer to complete the update.");
		}
	}
}
