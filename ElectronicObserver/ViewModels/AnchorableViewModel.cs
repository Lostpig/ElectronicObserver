﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace ElectronicObserver.ViewModels;


public class AnchorableViewModel : ObservableObject
{
	public string Title { get; set; }
	public virtual string ContentId { get; }
	public Visibility Visibility { get; set; } = Visibility.Collapsed;
	public bool IsSelected { get; set; }
	public bool IsActive { get; set; }
	public ImageSource? IconSource { get; set; }
	public bool IsClosable { get; set; }
	public bool IsMovable { get; set; }
	public bool IsFloatable { get; set; }
	public virtual ICommand CloseCommand { get; }

	protected AnchorableViewModel(string title, string contentId, ImageSource? icon = null)
	{
		Title = title;
		ContentId = contentId;
		IconSource = icon;
		CloseCommand = new RelayCommand(() => Visibility = Visibility.Collapsed);
	}
}
