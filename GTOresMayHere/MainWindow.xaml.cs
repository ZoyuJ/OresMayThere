using System;
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

//https://www.mcmod.cn/post/298.html

namespace GTOresMayHere {
  /// <summary>
  /// MainWindow.xaml 的交互逻辑
  /// </summary>
  public partial class MainWindow : Window {
    public MainWindow() {
      InitializeComponent();
      DataContext = this;
    }



    public string XValue {
      get { return (int)GetValue(XValueProperty); }
      set { SetValue(XValueProperty, value); }
    }

    // Using a DependencyProperty as the backing store for XValue.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty XValueProperty =
        DependencyProperty.Register("XValue", typeof(string), typeof(MainWindow), new PropertyMetadata("0"));



    public string YValue {
      get { return (int)GetValue(YValueProperty); }
      set { SetValue(YValueProperty, value); }
    }

    // Using a DependencyProperty as the backing store for YValue.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty YValueProperty =
        DependencyProperty.Register("YValue", typeof(string), typeof(MainWindow), new PropertyMetadata("0"));



    void NearestOrePoint() {

    }

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


}
