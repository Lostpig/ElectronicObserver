﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Xml.Linq;
using Codeplex.Data;
using AppSettings = ElectronicObserver.Properties.Settings;

namespace ElectronicObserver.Utility
{
    internal class SoftwareUpdater
	{
		internal static readonly string AppDataFolder =
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\\ElectronicObserver";
		internal static readonly string TranslationFolder = AppDataFolder + "\\Translations";
		private static readonly Uri UpdateUrl =
			new Uri("https://raw.githubusercontent.com/silfumus/ryuukitsune.github.io/master/Translations/en-US/update.json");
		internal static string MaintDate { get; set; } = string.Empty;
		internal static int MaintState { get; set; }

		//internal static string ZipUrl = string.Empty;
		//internal static string DownloadHash = string.Empty;
		private static bool _isChecked = false;

		public static void UpdateSoftware()
		{
			if (!Directory.Exists(AppDataFolder))
				Directory.CreateDirectory(AppDataFolder);

			CheckVersion();
			DownloadUpdater();

			var updaterFile = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\EOUpdater.exe";
			if (!File.Exists(updaterFile))
			{
				var updater = new Process
				{
					StartInfo =
					{
						FileName = updaterFile,
						UseShellExecute = false,
						CreateNoWindow = false
					}
				};
				updater.StartInfo.Arguments = "--restart";
				updater.Start();
				Logger.Add(2, "Updater started. Close EO after it has finished downloading the update.");
			}
		}

		private static void DownloadUpdater()
		{
			try
			{
				using (var client = new WebClient())
				{
					ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
					var url = @"https://github.com/silfumus/ryuukitsune.github.io/raw/develop/Translations/en-US/EOUpdater.exe";
					var updaterFile = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\EOUpdater.exe";

					client.DownloadFile(url, updaterFile);
					Logger.Add(1, "Updater download finished.");
				}
			}
			catch (Exception e)
			{
				Logger.Add(3, "Failed to download updater. " + e);
			}
		}

		internal static void CheckVersion()
		{
		    if (_isChecked) return;
			try
			{

				using (var client = WebRequest.Create(UpdateUrl).GetResponse())
				{
					var updateData = client.GetResponseStream();
					var json = DynamicJson.Parse(updateData);

					//ZipUrl = json.url;
					//DownloadHash = json.hash;
					MaintDate = json.kancolle_mt;
					MaintState = (int)json.event_state;

					CheckDataVersion(TranslationFile.Equipment, json.tl_ver.equipment);
					CheckDataVersion(TranslationFile.EquipmentTypes, json.tl_ver.equipment_type);
					CheckDataVersion(TranslationFile.Expeditions, json.tl_ver.expedition);
					CheckDataVersion(TranslationFile.Operations, json.tl_ver.operation);
					CheckDataVersion(TranslationFile.Quests, json.tl_ver.quest);
					CheckDataVersion(TranslationFile.Ships, json.tl_ver.ship);
					CheckDataVersion(TranslationFile.ShipTypes, json.tl_ver.ship_type);
					CheckDataVersion("nodes.json", (int)json.tl_ver.nodes);
				}
				
			}
			catch (Exception e)
			{
				Logger.Add(3, "Failed to download update info. " + e);
			}

			_isChecked = true;
	    }

	    public static void CheckDataVersion(string filename, int latestVer)
	    {
	        var source = TranslationFolder + $"\\{filename}";
		    var currentVer = 0;
			if (!File.Exists(source))
	            DownloadData(filename);
		    try
		    {
			    using (var sr = new StreamReader(source))
			    {
				    var json = DynamicJson.Parse(sr.ReadToEnd());
				    currentVer = (int) json.Revision;
			    }
		    }
		    catch (Exception e)
		    {
			    Logger.Add(3, "Error while checking translation data. " + e);
		    }
		    if (latestVer != currentVer)
			    DownloadData(filename);
		}

	    public static void CheckDataVersion(TranslationFile filename, string latestVer)
	    {
	        var source = TranslationFolder + $"\\{filename}.xml";
		    var currentVer = "0.0.0";
			if (!File.Exists(source))
	            DownloadData(filename);
		    try
		    {
			    var xml = XDocument.Load(source);
			    currentVer = xml.Root.Attribute("Version").Value;
		    }
		    catch (Exception e)
			{
				Logger.Add(3, "Error while checking translation data. " + e);
			}
		    if (latestVer != currentVer)
			    DownloadData(filename);
		}

        private static string GetHash(string filename)
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

		internal static void DownloadData(TranslationFile filename)
		{
		    var url = AppSettings.Default.EOTranslations.AbsoluteUri + "en-US/" + $"{filename}.xml";
		    var dest = TranslationFolder + $"\\{filename}.xml";
		    try
		    {
		        using (var client = new WebClient())
		        {
		            client.DownloadFile(new Uri(url), dest);
		            Logger.Add(2, $"File {filename} updated.");
                }
		    }
		    catch (Exception e)
		    {
		        Logger.Add(3, $"Failed to update {filename} data. " + e.Message);
		    }
        }

	    internal static void DownloadData(string filename)
	    {
	        var url = AppSettings.Default.EOTranslations.AbsoluteUri + "en-US/" + $"{filename}";
	        var dest = TranslationFolder + $"\\{filename}";
            try
	        {
	            using (var client = new WebClient())
	            {
	                client.DownloadFile(new Uri(url), dest);
	                Logger.Add(2, $"File {filename} updated.");
	            }
            }
	        catch (Exception e)
	        {
	            Logger.Add(3, $"Failed to update {filename} data. " + e.Message);
	        }
	    }
    }
}
