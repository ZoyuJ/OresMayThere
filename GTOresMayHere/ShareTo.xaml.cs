using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace GTOresMayHere {
  /// <summary>
  /// ShareTo.xaml 的交互逻辑
  /// </summary>
  public partial class ShareTo : Window {
    public ShareTo() {
      InitializeComponent();
    }

    #region TitleReact
    private void ShareWindowButton_Click(object sender, RoutedEventArgs e) {

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
}
