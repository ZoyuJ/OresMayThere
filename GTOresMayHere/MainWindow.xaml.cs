﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using static GTOresMayHere.UserConfig;

//https://www.mcmod.cn/post/298.html

namespace GTOresMayHere {
  /// <summary>
  /// MainWindow.xaml 的交互逻辑
  /// </summary>
  public partial class MainWindow : Window {
    public MainWindow() {
      InitializeComponent();
      DataContext = this;
      Loaded += (sender, args) => {
        WorldPosInput.GotFocus += (wpisender, wpiargs) => {
          if (WorldPosInput.Text.Trim() == "输入当前坐标") { WorldPosInput.Text = ""; }
          else { }
        };
      };
    }




    #region TitleReact
    bool IsCheckOutMapOrigin = false;
    private void MapOriginWindowButton_Click(object sender, RoutedEventArgs e) {
      if (IsCheckOutMapOrigin) {
        WorldPosInput.Visibility = Visibility.Visible;
        WorldPosOutput.Visibility = Visibility.Hidden;
        WorldPosInput.Text = "输入当前坐标";
        IsCheckOutMapOrigin = false;
      }
      else {
        string[] PosStringArray = WorldPosInput.Text.Trim().Split(' ');
        if (PosStringArray.Length < 2) {
          WorldPosOutput.Text = "无效坐标"; return;
        }
        try {
          InitWorldIndex = new Vector(Convert.ToInt64(PosStringArray[0]), Convert.ToInt64(PosStringArray[1]));
          CurrentGTIndex = InitGTIndex = InitWorldIndex.WorldXZToGTChunkIndex();
          IsCheckOutMapOrigin = true;
          WorldPosInput.Visibility = Visibility.Hidden;
          WorldPosOutput.Visibility = Visibility.Visible;
        }
        catch (Exception) {
          WorldPosOutput.Text = "无效坐标"; return;
        }

      }

    }
    private void SettingWindowButton_Click(object sender, RoutedEventArgs e) {

    }
    private void CloseWindowButton_Click(object sender, RoutedEventArgs e) {
      this.Close();
    }
    public (object, bool) DialogReturn { get; private set; }
    private void Grid_TouchMove(object sender, TouchEventArgs e) {
      if (RestoreIfMove) {
        RestoreIfMove = false;
        var MouseX = e.GetTouchPoint(this).Position.X;
        var Width = RestoreBounds.Width;
        var X = MouseX - Width / 2;
        if (X < 0) X = 0;
        else if (X + Width > SystemParameters.PrimaryScreenWidth) {
          X = SystemParameters.PrimaryScreenWidth - Width;
        }
        WindowState = WindowState.Normal;
        Left = X;
        Top = 0;
      }
      try {
        this.DragMove();
      }
      catch { }
    }
    private void Grid_MouseMove(object sender, MouseEventArgs e) {
      if (RestoreIfMove) {
        RestoreIfMove = false;
        var MouseX = e.GetPosition(this).X;
        var Width = RestoreBounds.Width;
        var X = MouseX - Width / 2;
        if (X < 0) X = 0;
        else if (X + Width > SystemParameters.PrimaryScreenWidth) {
          X = SystemParameters.PrimaryScreenWidth - Width;
        }
        WindowState = WindowState.Normal;
        Left = X;
        Top = 0;
      }
      try {
        this.DragMove();
      }
      catch { }
    }

    bool RestoreIfMove = false;
    private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
      if (e.ClickCount == 2) {
        if (this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip) {
          switch (WindowState) {
            case WindowState.Normal: {
              WindowState = WindowState.Maximized;
              break;
            }
            case WindowState.Maximized: {
              WindowState = WindowState.Normal;
              break;
            }
          }
        }
      }
      else {
        if (WindowState == WindowState.Maximized) RestoreIfMove = true;
      }
    }

    private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
      RestoreIfMove = false;
    }

    private void Grid_TouchDown(object sender, TouchEventArgs e) {
      if (WindowState == WindowState.Maximized) RestoreIfMove = true;
    }

    private void Grid_TouchUp(object sender, TouchEventArgs e) {
      RestoreIfMove = false;
    }
    #endregion


  }

  public sealed class ValueCorrectToBrush : IValueConverter {
    public static readonly SolidColorBrush UncorrectBrush = new SolidColorBrush(Colors.Red);
    public static readonly SolidColorBrush CorrectBrush = new SolidColorBrush(Colors.Black);
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      return Regex.IsMatch((string)value, @"^[0-9]+$") ? CorrectBrush : UncorrectBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }
  }
  public sealed class ConvertSysResizeBorderThicknessToCurrent : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      var Tks = SystemParameters.WindowResizeBorderThickness;
      return new Thickness(0.0, Tks.Top + Tks.Bottom, 0.0, 0.0);

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }
  }

}
