using System;
using System.IO;
using System.Threading.Tasks;
using Titanium.Web.Proxy.Models;
using Titanium.Web.Proxy.EventArguments;
using System.Diagnostics;

namespace ElectronicObserver.Observer;
internal sealed class LocalCache
{
	private static LocalCache? _instance;
	public static LocalCache Instance { 
		get {
			_instance ??= new();
			return _instance;
		} 
	}
	private bool _enable = false;
	private string _cachePath = "";
	private void UpdateCachePath ()
	{
		_enable = Utility.Configuration.Config.Connection.LocalCache;
		_cachePath = Utility.Configuration.Config.Connection.LocalCachePath.Replace("\\", "/");
		if (_cachePath.EndsWith("/")) _cachePath = _cachePath[0..^1];
	}

	private LocalCache()
	{
		UpdateCachePath();
		Utility.Configuration.Instance.ConfigurationChanged += HandleConfigurationChanged;
	}

	private void HandleConfigurationChanged()
	{
		UpdateCachePath();
	}

	private bool IsStaticResource(string pathname)
	{
		return pathname.StartsWith("/kcs/") || pathname.StartsWith("/kcs2/");
	}
	private bool GetCachePath(string pathname, out string resourcePath)
	{
		string cachePath = Path.Join(_cachePath, pathname);
		string ext = Path.GetExtension(pathname);
		string hackPath = Path.ChangeExtension(cachePath, $".hack{ext}");

		if (File.Exists(hackPath))
		{
			resourcePath = hackPath;
			return true;
		}
		if (File.Exists(cachePath))
		{
			resourcePath = cachePath;
			return true;
		}

		resourcePath = "";
		return false;
	}

	public async Task UseLocalCahce(SessionEventArgs e)
	{
		if (!_enable) return;

		string path = e.HttpClient.Request.RequestUri.AbsolutePath;
		if (!IsStaticResource(path)) return;

		if (GetCachePath(path, out string resourcePath))
		{
#if DEBUG
			Debug.WriteLine($"Load local cache file: {resourcePath}");
#endif

			var mtime = File.GetLastWriteTimeUtc(resourcePath);
			var since = e.HttpClient.Request.Headers.GetFirstHeader("if-modified-since");
			if (since is not null && DateTime.Parse(since.Value) > mtime)
			{
				HttpHeader[] headers =
				{
					new HttpHeader("Server", "nginx"),
					new HttpHeader("Last-Modified", mtime.ToString("r"))
				};
				e.GenericResponse(Array.Empty<byte>(), System.Net.HttpStatusCode.NotModified, headers);
			} 
			else
			{
				var bytes = await File.ReadAllBytesAsync(resourcePath);
				
				HttpHeader[] headers =
				{
					new HttpHeader("Server", "nginx"),
					new HttpHeader("Last-Modified", mtime.ToString("r")),
					new HttpHeader("Content-Length", bytes.Length + ""),
					new HttpHeader("Content-Type", Utility.MimeTypes.GetMimeType(resourcePath)),
					new HttpHeader("Cache-Control", "max-age=0"),
				};
				e.Ok(bytes, headers);
			}
		}
	}
}
