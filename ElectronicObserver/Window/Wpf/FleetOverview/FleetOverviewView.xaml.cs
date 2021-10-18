﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ElectronicObserver.Window.Wpf.FleetOverview;

/// <summary>
/// Interaction logic for FleetOverviewView.xaml
/// </summary>
public partial class FleetOverviewView : UserControl
{
	public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
		"ViewModel", typeof(FleetOverviewViewModel), typeof(FleetOverviewView), new PropertyMetadata(default(FleetOverviewViewModel)));

	public FleetOverviewViewModel ViewModel
	{
		get => (FleetOverviewViewModel)GetValue(ViewModelProperty);
		set => SetValue(ViewModelProperty, value);
	}

	public FleetOverviewView()
	{
		InitializeComponent();
	}
}
