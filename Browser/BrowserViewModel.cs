﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Browser.ExtraBrowser;
using BrowserLibCore;
using Grpc.Core;
using MagicOnion.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using ModernWpf;

namespace Browser;

public class ImageProvider
{
	public ImageSource? Screenshot { get; }
	public ImageSource? Zoom { get; }
	public ImageSource? ZoomIn { get; }
	public ImageSource? ZoomOut { get; }
	public ImageSource? Unmute { get; }
	public ImageSource? Mute { get; }
	public ImageSource? Refresh { get; }
	public ImageSource? Navigate { get; }
	public ImageSource? Other { get; }

	public ImageProvider(byte[][] images)
	{
		Screenshot = (ImageSource?)new ImageSourceConverter().ConvertFrom(images[0]);
		Zoom = (ImageSource?)new ImageSourceConverter().ConvertFrom(images[1]);
		ZoomIn = (ImageSource?)new ImageSourceConverter().ConvertFrom(images[2]);
		ZoomOut = (ImageSource?)new ImageSourceConverter().ConvertFrom(images[3]);
		Unmute = (ImageSource?)new ImageSourceConverter().ConvertFrom(images[4]);
		Mute = (ImageSource?)new ImageSourceConverter().ConvertFrom(images[5]);
		Refresh = (ImageSource?)new ImageSourceConverter().ConvertFrom(images[6]);
		Navigate = (ImageSource?)new ImageSourceConverter().ConvertFrom(images[7]);
		Other = (ImageSource?)new ImageSourceConverter().ConvertFrom(images[8]);
	}
}

public class BrowserViewModel : ObservableObject, BrowserLibCore.IBrowser
{
	public FormBrowserTranslationViewModel FormBrowser { get; }
	private BrowserConfiguration Configuration { get; set; }

	private string Host { get; }
	private int Port { get; }
	private string Culture { get; }
	private BrowserLibCore.IBrowserHost BrowserHost { get; set; }
	public string? ProxySettings { get; set; }

	public ImageProvider? Icons { get; set; }
	public WebView2? Browser { get; set; }
	public bool IsRefreshing { get; set; }
	private System.Drawing.Size KanColleSize { get; } = new(1200, 720);
	private string KanColleUrl => "http://www.dmm.com/netgame/social/-/gadgets/=/app_id=854854/";
	private string BrowserCachePath => BrowserConstants.CachePath;

	private string StyleClassID { get; } = Guid.NewGuid().ToString().Substring(0, 8);

	// user setting for stylesheet
	public bool StyleSheetEnabled { get; set; }
	// todo: flag to temporarily disable the stylesheet
	// seems to be used for when you navigate to something other than kancolle
	public bool ShouldStyleSheetApply { get; set; } = true;
	public bool StyleSheetApplied => StyleSheetEnabled && ShouldStyleSheetApply;

	public Dock ToolMenuDock { get; set; } = Dock.Top;
	public Orientation ToolMenuOrientation => ToolMenuDock switch
	{
		Dock.Left or Dock.Right => Orientation.Vertical,
		_ => Orientation.Horizontal
	};
	public Visibility ToolMenuVisibility { get; set; } = Visibility.Visible;

	public WindowsFormsHost BrowserWrapper { get; } = new();

	public double ActualWidth { get; set; }
	public double ActualHeight { get; set; }

	private System.Timers.Timer HeartbeatTimer { get; } = new();

	private bool IsKanColleLoaded { get; set; }

	public string? LastScreenShotPath { get; set; }

	private VolumeManager? VolumeManager { get; set; }
	public int Volume { get; set; }
	public ImageSource? MuteStateImage { get; set; }
	public CoreWebView2Frame? gameframe { get; private set; }
	public CoreWebView2Frame? kancolleframe { get; private set; }
	public bool ZoomFit { get; set; }
	public string CurrentZoom { get; set; } = "";

	public ICommand ScreenshotCommand { get; }
	public ICommand SetZoomCommand { get; }
	public ICommand ModifyZoomCommand { get; }
	public ICommand MuteCommand { get; }
	public ICommand RefreshCommand { get; }
	public ICommand GoToLoginPageCommand { get; }

	public ICommand OpenLastScreenshotCommand { get; }
	public ICommand OpenScreenshotFolderCommand { get; }
	public ICommand CopyLastScreenshotCommand { get; }

	public ICommand HardRefreshCommand { get; }
	public ICommand GoToCommand { get; }
	public ICommand ClearCacheCommand { get; }

	public ICommand SetToolMenuAlignmentCommand { get; }
	public ICommand SetToolMenuVisibilityCommand { get; }

	public ICommand OpenDevtoolsCommand { get; }

	/// <summary>
	/// </summary>
	/// <param name="serverUri">ホストプロセスとの通信用URL</param>
	public BrowserViewModel(string host, int port, string culture)
	{
		//Debugger.Launch();

		FormBrowser = App.Current.Services.GetService<FormBrowserTranslationViewModel>()!;
		ScreenshotCommand = new RelayCommand(ToolMenu_Other_ScreenShot_Click);
		SetZoomCommand = new RelayCommand<string>(SetZoom);
		ModifyZoomCommand = new RelayCommand<string>(ModifyZoom);
		MuteCommand = new RelayCommand(ToolMenu_Other_Mute_Click);
		RefreshCommand = new RelayCommand(ToolMenu_Other_Refresh_Click);
		GoToLoginPageCommand = new RelayCommand(ToolMenu_Other_NavigateToLogInPage_Click);

		OpenLastScreenshotCommand = new RelayCommand(ToolMenu_Other_LastScreenShot_ImageHost_Click);
		OpenScreenshotFolderCommand = new RelayCommand(ToolMenu_Other_LastScreenShot_OpenScreenShotFolder_Click);
		CopyLastScreenshotCommand = new RelayCommand(ToolMenu_Other_LastScreenShot_CopyToClipboard_Click);

		HardRefreshCommand = new RelayCommand(ToolMenu_Other_RefreshIgnoreCache_Click);
		GoToCommand = new RelayCommand(ToolMenu_Other_Navigate_Click);
		ClearCacheCommand = new RelayCommand(ToolMenu_Other_ClearCache_Click);

		SetToolMenuAlignmentCommand = new RelayCommand<Dock>(SetToolMenuAlignment);
		SetToolMenuVisibilityCommand = new RelayCommand<Visibility>(SetToolMenuVisibility);

		OpenDevtoolsCommand = new RelayCommand(ToolMenu_Other_OpenDevTool_Click);

		Host = host;
		Port = port;
		Culture = culture;
		CultureInfo c = new(culture);

		Thread.CurrentThread.CurrentCulture = c;
		Thread.CurrentThread.CurrentUICulture = c;

		PropertyChanged += (sender, args) =>
		{
			if (Configuration is null) return;
			if (args.PropertyName is not nameof(ZoomFit)) return;

			Configuration.ZoomFit = ZoomFit;
			ApplyZoom();
			ConfigurationUpdated();
		};

		PropertyChanged += (sender, args) =>
		{
			if (Configuration is null) return;
			if (args.PropertyName is not nameof(StyleSheetEnabled)) return;

			Configuration.AppliesStyleSheet = StyleSheetEnabled;

			ApplyStyleSheet();
			ApplyZoom();
			ConfigurationUpdated();
		};

		PropertyChanged += (sender, args) =>
		{
			if (args.PropertyName is not (nameof(ActualWidth) or nameof(ActualHeight))) return;

			ApplyZoom();
		};

		PropertyChanged += (sender, args) =>
		{
			if (args.PropertyName is not nameof(Volume)) return;

			ToolMenu_Other_Volume_ValueChanged();
		};
	}

	public void OnLoaded(object sender, RoutedEventArgs e)
	{
		if (sender is not System.Windows.Window window) return;

		IntPtr handle = new WindowInteropHelper(window).Handle;
		SetWindowLong(handle, GWL_STYLE, WS_CHILD);

		// ホストプロセスに接続
		Channel grpChannel = new(Host, Port, ChannelCredentials.Insecure);
		BrowserHost = StreamingHubClient.Connect<BrowserLibCore.IBrowserHost, BrowserLibCore.IBrowser>(grpChannel, this);

		ConfigurationChanged();

		// ウィンドウの親子設定＆ホストプロセスから接続してもらう
		Task.Run(async () => await BrowserHost.ConnectToBrowser((long)handle)).Wait();

		// 親ウィンドウが生きているか確認
		HeartbeatTimer.Elapsed += async (sender2, e2) =>
		{
			try
			{
				bool alive = await BrowserHost.IsServerAlive();
			}
			catch (Exception)
			{
				Debug.WriteLine("host died");
				Exit();
			}
		};
		HeartbeatTimer.Interval = 2000; // 2秒ごと　
		HeartbeatTimer.Start();

		SetIconResource();
		Browser = new WebView2();
		InitializeAsync();
	}

	/// <summary>
	/// ブラウザを初期化します。
	/// 最初の呼び出しのみ有効です。二回目以降は何もしません。
	/// </summary>
	public async void InitializeAsync()
	{
		if (Browser.CoreWebView2 != null) return;
		if (ProxySettings == null) return;
		string colorArgs = "";
		if (Configuration.ForceColorProfile)
		{
			colorArgs = @"--force-color-profile=""sRGB""";
		}
		var corewebviewoptions = new CoreWebView2EnvironmentOptions() { AdditionalBrowserArguments = $"--proxy-server=\"{ProxySettings}\" --disable-features=\"HardwareMediaKeyHandling\" --lang=\"ja\" " + colorArgs };
		var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder: BrowserCachePath, options: corewebviewoptions);
		//Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", $"--proxy-server=\"{ProxySettings}\" --disable-features=\"HardwareMediaKeyHandling\" " + colorArgs);
		//Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", BrowserCachePath);
		await Browser.EnsureCoreWebView2Async(env);
		Browser.Source = new Uri("about:blank");
		Browser.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
		Browser.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Script);
		Browser.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.Media);
		Browser.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
		Browser.CoreWebView2.FrameCreated += CoreWebView2_FrameCreated;
		Browser.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarted;
		Browser.CoreWebView2.ProcessFailed += CoreWebView2_ProcessFailed;
		Browser.CoreWebView2.ContextMenuRequested += CoreWebView2_ContextMenuRequested;
		Browser.PreviewKeyDown += Browser_PreviewKeyDown;
		SetCookie();
		Browser.CoreWebView2.Navigate(KanColleUrl);
	}

	private void CoreWebView2_ContextMenuRequested(object? sender, CoreWebView2ContextMenuRequestedEventArgs e)
	{
		if (Browser.CoreWebView2 == null) return;
		if (gameframe != null)
		{
			//e.Handled = true;
		}
	}

	private void Browser_PreviewKeyDown(object sender, KeyEventArgs e)
	{
		if (Browser.CoreWebView2 == null) return;
		switch (e.Key)
		{
			case Key.F5:
				RefreshCommand.Execute(null);
				break;
			case Key.F12:
				OpenDevtoolsCommand.Execute(null);
				break;
			case Key.F2:
				ScreenshotCommand.Execute(null);
				break;
			case Key.F7:
				MuteCommand.Execute(null);
				break;
		}

	}

	private void CoreWebView2_ProcessFailed(object? sender, CoreWebView2ProcessFailedEventArgs e)
	{
		switch (e.ProcessFailedKind)
		{
			case CoreWebView2ProcessFailedKind.BrowserProcessExited:
				AddLog(2, "Browser Process Exited");
				break;
			case CoreWebView2ProcessFailedKind.GpuProcessExited:
				AddLog(2, "GPU Process Exited");
				break;
			case CoreWebView2ProcessFailedKind.RenderProcessUnresponsive:
				AddLog(2, "Render Process Unresponsive");
				break;
			case CoreWebView2ProcessFailedKind.UnknownProcessExited:
				AddLog(2, "Unknown Process Exited");
				break;
			default:
				AddLog(2, "Proccess Failed");
				break;
		}
	}


	private void CoreWebView2_NavigationStarted(object? sender, CoreWebView2NavigationStartingEventArgs e)
	{
		if (Browser.CoreWebView2 == null) return;

		if (e.Uri.Contains(@"/rt.gsspat.jp/"))
		{
			e.Cancel = true;
		}
		if (new Uri(e.Uri).Host.Contains("accounts.google.com"))
		{
			var settings = Browser.CoreWebView2.Settings;
			settings.UserAgent = "Chrome";
		}

		if (gameframe != null && !IsRefreshing)
		{
			e.Cancel = true;
		}
	}
	private void CoreWebView2_FrameCreated(object? sender, CoreWebView2FrameCreatedEventArgs e)
	{
		if (e.Frame.Name.Contains(@"game_frame"))
		{
			gameframe = e.Frame;
			IsRefreshing = false;
		}
		if (e.Frame.Name.Contains(@"htmlWrap"))
		{
			kancolleframe = e.Frame;
		}
	}
	private void CoreWebView2_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
	{
		if (e.Request.Uri.Contains(@"gadget_html5") && Configuration.UseGadgetRedirect)
		{
			e.Request.Uri = e.Request.Uri.Replace("http://203.104.209.7/gadget_html5/", "https://kcwiki.github.io/cache/gadget_html5/");
		}
		if (e.Request.Uri.Contains("/kcs2/resources/bgm/"))
		{
			//not working in webview2
			//e.Request.Headers.RemoveHeader("Range");
		}
	}
	private void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
	{
		if (e.IsSuccess)
		{
			ApplyStyleSheet();
			ApplyZoom();
			SetCookie();
		}
	}


	private void Exit()
	{
		// if (!BrowserHost.Closed)
		{
			// BrowserHost.Close();
			App.Current.Dispatcher.Invoke(() =>
			{
				HeartbeatTimer.Stop();
				Task.Run(async () => await BrowserHost.DisposeAsync()).Wait();
				Browser.Dispose();
				App.Current.Shutdown();
			});
		}
	}

	public void CloseBrowser()
	{
		HeartbeatTimer.Stop();
		// リモートコールでClose()呼ぶのばヤバそうなので非同期にしておく
		App.Current.Dispatcher.BeginInvoke((Action)(() => Exit()));
	}

	public async void ConfigurationChanged()
	{
		Configuration = await BrowserHost.Configuration();

		ZoomFit = Configuration.ZoomFit;
		ApplyZoom();
		StyleSheetEnabled = Configuration.AppliesStyleSheet;

		ToolMenuDock = Configuration.ToolMenuDockStyle switch
		{
			1 => Dock.Top,
			2 => Dock.Bottom,
			3 => Dock.Left,
			4 => Dock.Right,
			_ => Dock.Top
		};

		ToolMenuVisibility = Configuration.IsToolMenuVisible switch
		{
			true => Visibility.Visible,
			_ => Visibility.Collapsed
		};

		ThemeManager.Current.ApplicationTheme = (ApplicationTheme)await BrowserHost.GetTheme();

		// SizeAdjuster.BackColor = System.Drawing.Color.FromArgb(unchecked((int)Configuration.BackColor));
		// ToolMenu.BackColor = System.Drawing.Color.FromArgb(unchecked((int)Configuration.BackColor));
		// ToolMenu_Other_ClearCache.Visible = conf.EnableDebugMenu;
	}

	private void ConfigurationUpdated()
	{
		BrowserHost.ConfigurationUpdated(Configuration);
	}

	private void AddLog(int priority, string message)
	{
		BrowserHost.AddLog(priority, message);
	}

	private void SendErrorReport(string exceptionName, string message)
	{
		BrowserHost.SendErrorReport(exceptionName, message);
	}

	public void InitialAPIReceived()
	{
		IsKanColleLoaded = true;

		//ロード直後の適用ではレイアウトがなぜか崩れるのでこのタイミングでも適用
		ApplyStyleSheet();
		ApplyZoom();
		DestroyDMMreloadDialog();

		//起動直後はまだ音声が鳴っていないのでミュートできないため、この時点で有効化
		SetVolumeState();
	}

	// hack: it makes an infinite loop in the wpf version for some reason
	private int Counter { get; set; }


	private void ApplyStyleSheet()
	{
		if (Browser is not { IsInitialized: true }) return;
		try
		{

			if (gameframe == null)
				return;

			if (!StyleSheetApplied)
			{
				Browser.ExecuteScriptAsync(string.Format(Properties.Resources.RestoreScript, StyleClassID));
				gameframe.ExecuteScriptAsync(string.Format(Properties.Resources.RestoreScript, StyleClassID));
				gameframe.ExecuteScriptAsync("document.body.style.backgroundColor = \"#000000\";");
			}
			else
			{
				Browser.ExecuteScriptAsync(String.Format(Properties.Resources.PageScript, StyleClassID));
				gameframe.ExecuteScriptAsync(String.Format(Properties.Resources.FrameScript, StyleClassID));
				gameframe.ExecuteScriptAsync("document.body.style.backgroundColor = \"#000000\";");
			}
		}
		catch (Exception ex)
		{
			SendErrorReport(ex.ToString(), FormBrowser.FailedToApplyStylesheet);
		}
	}

	/// <summary>
	/// DMMによるページ更新ダイアログを非表示にします。
	/// </summary>
	private void DestroyDMMreloadDialog()
	{
		if (Browser is not { IsInitialized: true }) return;
		if (!Configuration.IsDMMreloadDialogDestroyable) return;

		try
		{
			Browser?.CoreWebView2.ExecuteScriptAsync(Properties.Resources.DMMScript);
		}
		catch (Exception ex)
		{
			SendErrorReport(ex.ToString(), FormBrowser.FailedToHideDmmRefreshDialog);
		}
	}


	// タイミングによっては(特に起動時)、ブラウザの初期化が完了する前に Navigate() が呼ばれることがある
	// その場合ロードに失敗してブラウザが白画面でスタートしてしまう（手動でログインページを開けば続行は可能だが）
	// 応急処置として失敗したとき後で再試行するようにしてみる
	private string? NavigateCache { get; set; }



	/// <summary>
	/// 指定した URL のページを開きます。
	/// </summary>
	public void Navigate(string url)
	{
		Browser.CoreWebView2?.Navigate(url);
	}
	/// <summary>
	/// ブラウザを再読み込みします。
	/// </summary>
	private void RefreshBrowser() => RefreshBrowser(false);

	/// <summary>
	/// ブラウザを再読み込みします。
	/// </summary>
	/// <param name="ignoreCache">キャッシュを無視するか。</param>
	private void RefreshBrowser(bool ignoreCache)
	{
		//Browser.Reload(ignoreCache);
		Browser.CoreWebView2.Reload();
		IsRefreshing = true;
	}

	/// <summary>
	/// ズームを適用します。
	/// </summary>
	private void ApplyZoom()
	{
		if (Browser is not { IsInitialized: true }) return;
		double zoomRate = Configuration.ZoomRate;
		bool fit = Configuration.ZoomFit && StyleSheetApplied;

		double zoomFactor;

		if (fit)
		{
			double rateX = ActualWidth / KanColleSize.Width;
			double rateY = ActualHeight / KanColleSize.Height;

			zoomFactor = Math.Min(rateX, rateY);
		}
		else
		{
			zoomFactor = Math.Clamp(zoomRate, 0.1, 10);
		}

		// DpiScaleX and DpiScaleY should always be the same so it doesn't matter which one you use
		Browser.ZoomFactor = zoomFactor;

		if (StyleSheetApplied && gameframe != null)
		{
			int newWidth = (int)(KanColleSize.Width * zoomFactor);
			int newHeight = (int)(KanColleSize.Height * zoomFactor);

			Browser.Width = newWidth;
			Browser.Height = newHeight;

			// Browser.MinWidth = newWidth;
			// Browser.MinHeight = newHeight;
		}
		else
		{
			Browser.Width = double.NaN;
			Browser.Height = double.NaN;
		}

		CurrentZoom = fit switch
		{
			true => FormBrowser.Other_Zoom_Current_Fit,
			_ => FormBrowser.Other_Zoom_Current + $" {zoomRate:p1}"
		};
	}
	/// <summary>
	/// スクリーンショットを撮影し、設定で指定された保存先に保存します。
	/// </summary>
	private async Task SaveScreenShot()
	{
		int savemode = Configuration.ScreenShotSaveMode;
		int format = Configuration.ScreenShotFormat;
		string folderPath = Configuration.ScreenShotPath;
		bool is32bpp = format != 1 && Configuration.AvoidTwitterDeterioration;

		// to file
		try
		{
			if (!Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);

			ImageFormat imageFormat = null;
			CoreWebView2CapturePreviewImageFormat browserImageFormat;
			string ext;
			switch (format)
			{
				case 1:
					imageFormat = ImageFormat.Jpeg;
					browserImageFormat = CoreWebView2CapturePreviewImageFormat.Jpeg;
					ext = "jpg";
					break;
				case 2:
				default:
					imageFormat = ImageFormat.Png;
					browserImageFormat = CoreWebView2CapturePreviewImageFormat.Png;
					ext = "png";
					break;
			}

			string path = $"{folderPath}\\{DateTime.Now:yyyyMMdd_HHmmssff}.{ext}";

			await using MemoryStream memoryStream = new();
			await Browser.CoreWebView2.CapturePreviewAsync(browserImageFormat, memoryStream).ConfigureAwait(false);

			Bitmap image = (Bitmap)Bitmap.FromStream(memoryStream, true);

			if (savemode is 1 or 3)
			{
				image.Save(path, imageFormat);
				AddLog(2, string.Format(FormBrowser.ScreenshotSavedTo, path));
				LastScreenShotPath = Path.GetFullPath(path);
			}

			if ((savemode & 2) != 0)
			{
				App.Current.Dispatcher.Invoke(() => Clipboard.SetImage(image.ToBitmapSource()));
				AddLog(2, string.Format(FormBrowser.ScreenshotCopiedToClipboard));
			}
		}
		catch (Exception ex)
		{
			SendErrorReport(ex.ToString(), FormBrowser.FailedToSaveScreenshot);
		}
	}

	public void SetProxy(string proxy)
	{
		if (ushort.TryParse(proxy, out ushort port))
		{
			// WinInetUtil.SetProxyInProcessForNekoxy(port);
			ProxySettings = "http=127.0.0.1:" + port; // todo: 動くには動くが正しいかわからない
		}
		else
		{
			// WinInetUtil.SetProxyInProcess(proxy, "local");
			ProxySettings = proxy;
		}
		InitializeAsync();
		BrowserHost.SetProxyCompleted();
	}

	private void SetCookie()
	{
		var gamesCookies = Browser.CoreWebView2.CookieManager.CreateCookie("ckcy", "1", "games.dmm.com", "/");
		gamesCookies.Expires = DateTime.Now.AddYears(6);
		gamesCookies.IsSecure = true;
		Browser.CoreWebView2.CookieManager.AddOrUpdateCookie(gamesCookies);
		var dmmCookie = Browser.CoreWebView2.CookieManager.CreateCookie("ckcy", "1", ".dmm.com", "/");
		dmmCookie.Expires = DateTime.Now.AddYears(6);
		dmmCookie.IsSecure = true;
		Browser.CoreWebView2.CookieManager.AddOrUpdateCookie(dmmCookie);
		var acccountsCookie = Browser.CoreWebView2.CookieManager.CreateCookie("ckcy", "1", "accounts.dmm.com", "/");
		acccountsCookie.Expires = DateTime.Now.AddYears(6);
		acccountsCookie.IsSecure = true;
		Browser.CoreWebView2.CookieManager.AddOrUpdateCookie(acccountsCookie);
		var osapiCookie = Browser.CoreWebView2.CookieManager.CreateCookie("ckcy", "1", "osapi.dmm.com", "/");
		acccountsCookie.Expires = DateTime.Now.AddYears(6);
		acccountsCookie.IsSecure = true;
		Browser.CoreWebView2.CookieManager.AddOrUpdateCookie(osapiCookie);
		var gameserverCookie = Browser.CoreWebView2.CookieManager.CreateCookie("ckcy", "1", "203.104.209.7", "/");
		acccountsCookie.Expires = DateTime.Now.AddYears(6);
		Browser.CoreWebView2.CookieManager.AddOrUpdateCookie(gameserverCookie);
		var gamepathCookie = Browser.CoreWebView2.CookieManager.CreateCookie("ckcy", "1", "www.dmm.com", "/netgame/");
		acccountsCookie.Expires = DateTime.Now.AddYears(6);
		Browser.CoreWebView2.CookieManager.AddOrUpdateCookie(gamepathCookie);
	}

	private async void SetIconResource()
	{
		byte[][] canvas = await BrowserHost.GetIconResource();

		Icons = new(canvas);

		SetVolumeState();
	}

	private void TryGetVolumeManager()
	{
		VolumeManager = VolumeManager.CreateInstanceByProcessName("msedgewebview2");
	}

	private void SetVolumeState()
	{
		bool mute;
		float volume;

		try
		{
			if (VolumeManager == null)
			{
				TryGetVolumeManager();
			}

			mute = VolumeManager?.IsMute ?? false;
			volume = (VolumeManager?.Volume ?? 1) * 100;
		}
		catch (Exception)
		{
			// 音量データ取得不能時
			VolumeManager = null;
			mute = false;
			volume = 100;
		}

		MuteStateImage = mute switch
		{
			true => Icons?.Mute,
			_ => Icons?.Unmute,
		};

		Volume = (int)volume;
		Configuration.Volume = volume;
		Configuration.IsMute = mute;
		ConfigurationUpdated();
	}

	private async void ToolMenu_Other_ScreenShot_Click()
	{
		await SaveScreenShot();
	}

	private void SetZoom(string? zoomParameter)
	{
		if (!double.TryParse(zoomParameter, out double zoom)) return;

		Configuration.ZoomRate = zoom;
		Configuration.ZoomFit = ZoomFit = false;
		ApplyZoom();
		ConfigurationUpdated();
	}

	private void ModifyZoom(string? zoomParameter)
	{
		if (!double.TryParse(zoomParameter, out double zoom)) return;

		Configuration.ZoomRate = Math.Clamp(Configuration.ZoomRate + zoom, 0.1, 10);
		Configuration.ZoomFit = ZoomFit = false;
		ApplyZoom();
		ConfigurationUpdated();
	}

	private void SetToolMenuAlignment(Dock dock)
	{
		ToolMenuDock = dock;

		Configuration.ToolMenuDockStyle = dock switch
		{
			Dock.Top => 1,
			Dock.Bottom => 2,
			Dock.Left => 3,
			Dock.Right => 4,
			_ => 1
		};

		ConfigurationUpdated();
	}

	private void SetToolMenuVisibility(Visibility visibility)
	{
		ToolMenuVisibility = visibility;

		Configuration.IsToolMenuVisible = visibility == Visibility.Visible;
		ConfigurationUpdated();
	}

	private void ToolMenu_Other_Mute_Click()
	{
		if (VolumeManager == null)
		{
			TryGetVolumeManager();
		}

		try
		{
			if (VolumeManager is null)
			{
				System.Media.SystemSounds.Beep.Play();
			}
			else
			{
				VolumeManager.ToggleMute();
			}
		}
		catch (Exception)
		{

		}

		SetVolumeState();
	}

	private void ToolMenu_Other_Volume_ValueChanged()
	{
		if (VolumeManager is null)
		{
			TryGetVolumeManager();
		}

		try
		{
			if (VolumeManager is not null)
			{
				VolumeManager.Volume = (float)Volume / 100;
				// control.BackColor = System.Drawing.SystemColors.Window;
			}
			else
			{
				// todo: add red color to indicate volume manager isn't active?
				// control.BackColor = System.Drawing.Color.MistyRose;
			}

		}
		catch (Exception)
		{
		}
	}

	private void ToolMenu_Other_Refresh_Click()
	{
		if (!Configuration.ConfirmAtRefresh ||
			MessageBox.Show(FormBrowser.ReloadDialog, FormBrowser.Confirmation,
				MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.Cancel)
			== MessageBoxResult.OK)
		{
			RefreshBrowser();
		}
	}

	private void ToolMenu_Other_RefreshIgnoreCache_Click()
	{
		if (!Configuration.ConfirmAtRefresh ||
			MessageBox.Show(FormBrowser.ReloadHardDialog, FormBrowser.Confirmation,
				MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.Cancel)
			== MessageBoxResult.OK)
		{
			RefreshBrowser(true);
		}
	}

	private void ToolMenu_Other_NavigateToLogInPage_Click()
	{
		if (MessageBox.Show(FormBrowser.LoginDialog, FormBrowser.Confirmation,
				MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel)
			== MessageBoxResult.OK)
		{
			Navigate(Configuration.LogInPageURL);
		}
	}

	private void ToolMenu_Other_Navigate_Click()
	{
		BrowserHost.RequestNavigation(Browser.CoreWebView2?.Source ?? "");
	}

	private void ToolMenu_Other_AppliesStyleSheet_Click()
	{
		/*
		Configuration.AppliesStyleSheet = ToolMenu.Other_AppliesStyleSheet.Checked;
		if (!Configuration.AppliesStyleSheet)
			RestoreStyleSheet = true;
		ApplyStyleSheet();
		ApplyZoom();
		ConfigurationUpdated();
		*/
	}



	private void ContextMenuTool_ShowToolMenu_Click(object sender, EventArgs e)
	{
		/*
		ToolMenu.Visible = Configuration.IsToolMenuVisible = true;
		ConfigurationUpdated();
		*/
	}

	private void FormBrowser_Activated(object sender, EventArgs e)
	{
		//Browser?.Focus();
	}

	void ToolMenu_Other_LastScreenShot_ImageHost_Click()
	{
		if (LastScreenShotPath is null || !File.Exists(LastScreenShotPath)) return;

		ProcessStartInfo psi = new()
		{
			FileName = LastScreenShotPath,
			UseShellExecute = true
		};
		Process.Start(psi);
	}

	private void ToolMenu_Other_LastScreenShot_OpenScreenShotFolder_Click()
	{
		if (!Directory.Exists(Configuration.ScreenShotPath)) return;

		ProcessStartInfo psi = new()
		{
			FileName = Configuration.ScreenShotPath,
			UseShellExecute = true
		};
		Process.Start(psi);
	}

	private void ToolMenu_Other_LastScreenShot_CopyToClipboard_Click()
	{
		if (LastScreenShotPath is null || !File.Exists(LastScreenShotPath)) return;

		try
		{
			using var img = new System.Drawing.Bitmap(LastScreenShotPath);
			Clipboard.SetImage(img.ToBitmapSource());
			AddLog(2, string.Format(FormBrowser.LastScreenshotCopiedToClipboard, LastScreenShotPath));
		}
		catch (Exception ex)
		{
			SendErrorReport(ex.Message, FormBrowser.FailedToCopyScreenshotToClipboard);
		}
	}

	private void ToolMenu_Other_OpenDevTool_Click()
	{
		if (Browser is not { IsInitialized: true }) return;
		if (Browser.CoreWebView2 is null) return;
		Browser.CoreWebView2.OpenDevToolsWindow();
	}

	private void ToolMenu_Other_ClearCache_Click()
	{
		if (MessageBox.Show(FormBrowser.ClearCacheMessage, FormBrowser.ClearCacheTitle,
			MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
		{
			BrowserHost.ClearCache();
		}
	}

	public void OpenExtraBrowser()
	{
		new ExtraBrowserWindow().Show();
	}

	/*
	protected override void WndProc(ref Message m)
	{
		if (m.Msg == WM_ERASEBKGND)
			// ignore this message
			return;

		base.WndProc(ref m);
	}

	*/

	#region 呪文

	[DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
	public static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

	[DllImport("user32.dll", EntryPoint = "SetWindowLongA", SetLastError = true)]
	public static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

	public const int GWL_STYLE = (-16);
	public const uint WS_CHILD = 0x40000000;
	public const uint WS_VISIBLE = 0x10000000;
	public const int WM_ERASEBKGND = 0x14;

	#endregion
}
