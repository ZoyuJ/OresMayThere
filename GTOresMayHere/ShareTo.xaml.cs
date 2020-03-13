using System;
using System.Collections.Generic;
using System.IO;
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
using Newtonsoft.Json;

namespace GTOresMayHere {
  /// <summary>
  /// ShareTo.xaml 的交互逻辑
  /// </summary>
  public partial class ShareTo : Window {
    public ShareTo() {
      InitializeComponent();
      Loaded += (sender, args) => {
        //DialogResult = true;
        //WorldPosOutput.GotFocus += (sender1, args1) => { WorldPosInput.SelectAll(); };
      };
    }

    public ShareTo(IndexDetail? Detail) : this() {
      CurrentLocalDetail = Detail;
      if (CurrentLocalDetail != null) {
        WorldPosInput.Visibility = Visibility.Visible;
        WorldPosOutput.Visibility = Visibility.Hidden;
        WorldPosInput.Text = $"{CurrentLocalDetail.Value.Index.ToString()} is {CurrentLocalDetail.Value.OreType}";
      }
    }


    public object DialogReturn { get; private set; }
    public IndexDetail? CurrentLocalDetail { get; set; }

    #region TitleReact
    private void SaveWindowButton_Click(object sender, RoutedEventArgs e) {
      if (CurrentLocalDetail != null) {
        var WPos = CurrentLocalDetail.Value.Index.GTChunkIndexToMCWorldXZ();
        ExtenWayPoint EWPoint = new ExtenWayPoint(CurrentLocalDetail.Value.OreType, WPos.X, WPos.Z);
        using (var sw = new StreamWriter(File.Create(Path.Combine(UserConfig.GameBuiltinMapWayPointsDirectory, EWPoint.WayPointFileName())), new UTF8Encoding(false))) {
          sw.Write(JsonConvert.SerializeObject(EWPoint));
        }
        DialogResult = true;
        this.Close();
      }
    }
    private void ImportWindowButton_Click(object sender, RoutedEventArgs e) {
      if (GetDataFromClipboard(out var Detail)) {
        CurrentLocalDetail = Detail;
        WorldPosInput.Visibility = Visibility.Hidden;
        WorldPosOutput.Visibility = Visibility.Visible;
        WorldPosOutput.Text = $"{Detail.Index.GTChunkIndexToMCWorldXZ().ToString()} is {Detail.OreType}";
      }

    }
    private void ShareWindowButton_Click(object sender, RoutedEventArgs e) {
      int Len = WorldPosInput.Text.IndexOf(" is ");
      if (Len < 0) {
        DialogResult = false;
        MessageBox.Show("数据结构不完整");
        this.Close();
        return;
      }
      Len += " is ".Length;
      var OreType = WorldPosInput.Text.Substring(Len, WorldPosInput.Text.Length - Len);
      CurrentLocalDetail = new IndexDetail() { Index = CurrentLocalDetail.Value.Index, Detected = CurrentLocalDetail.Value.Detected, OreType = OreType };
      var SharedData = Convert.ToBase64String(new UTF8Encoding(false).GetBytes(JsonConvert.SerializeObject(CurrentLocalDetail.Value)));
      Clipboard.SetText(SharedData);
    }

    bool GetDataFromClipboard(out IndexDetail Detail) {
      Detail = IndexDetail.None;
      string PackedData = null;
      try {
        PackedData = Clipboard.GetText();
      }
      catch (Exception) {
        return false;
      }
      if (PackedData == null || PackedData.Length == 0) return false;

      try {
        Detail = JsonConvert.DeserializeObject<IndexDetail>(new UTF8Encoding(false).GetString(Convert.FromBase64String(PackedData)));
        return true;
      }
      catch (Exception) {
        return false;
      }
    }

    private void CloseWindowButton_Click(object sender, RoutedEventArgs e) {
      this.Close();
    }
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
