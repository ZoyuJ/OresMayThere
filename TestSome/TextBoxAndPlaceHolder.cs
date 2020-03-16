using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestSome {
  /// <summary>
  /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
  ///
  /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
  /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
  /// 元素中:
  ///
  ///     xmlns:MyNamespace="clr-namespace:TestSome"
  ///
  ///
  /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
  /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
  /// 元素中:
  ///
  ///     xmlns:MyNamespace="clr-namespace:TestSome;assembly=TestSome"
  ///
  /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
  /// 并重新生成以避免编译错误:
  ///
  ///     在解决方案资源管理器中右击目标项目，然后依次单击
  ///     “添加引用”->“项目”->[浏览查找并选择此项目]
  ///
  ///
  /// 步骤 2)
  /// 继续操作并在 XAML 文件中使用控件。
  ///
  ///     <MyNamespace:TextBoxAndPlaceHolder/>
  ///
  /// </summary>
  public class TextBoxAndPlaceHolder : TextBox {
    static TextBoxAndPlaceHolder() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxAndPlaceHolder), new FrameworkPropertyMetadata(typeof(TextBoxAndPlaceHolder)));
    }

    public TextBoxAndPlaceHolder() {
      DataContext = this;
      TextChanged += (sender, args) => {
       PlaceHolderPresenter.Visibility = Text.Length <= 0 ? Visibility.Visible : Visibility.Hidden;
      };
    }

    public FrameworkElement PlaceHolderPresenter { get => (FrameworkElement)this.Template.FindName("PlaceHolderPresenter", this); }

    // Using a DependencyProperty as the backing store for PlaceHolder.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PlaceHolderProperty = DependencyProperty.Register("PlaceHolder", typeof(object), typeof(TextBoxAndPlaceHolder), new PropertyMetadata(null));
    public object PlaceHolder {
      get { return (object)GetValue(PlaceHolderProperty); }
      set { SetValue(PlaceHolderProperty, value); }
    }

    // Using a DependencyProperty as the backing store for PlaceHolderMargin.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PlaceHolderMarginProperty = DependencyProperty.Register("PlaceHolderMargin", typeof(Thickness), typeof(TextBoxAndPlaceHolder), new PropertyMetadata(new Thickness(2, 0, 0, 0)));
    public Thickness PlaceHolderMargin {
      get { return (Thickness)GetValue(PlaceHolderMarginProperty); }
      set { SetValue(PlaceHolderMarginProperty, value); }
    }



  }
}
