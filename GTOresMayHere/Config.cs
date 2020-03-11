using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GTOresMayHere {
  public static class UIConfig {
    public static Color MainBackgroundColor { get; set; } = Colors.Black;
    public static SolidColorBrush MainBackgroundBrush { get; set; } = new SolidColorBrush(Colors.Black);

    public static Color AxeXColor { get; set; }
    public static Color AxeXArrowColor { get; set; }
    public static Color AxeYColor { get; set; }
    public static Color AxeYArrowColor { get; set; }

    public static Color AxeOriginColor { get; set; }
    public static Color FaceToArrowColor { get; set; }

  }

  public static class UserConfig {
    public static Vector CurrentGTIndex, InitGTIndex, InitWorldIndex;
    public static Vector CurrentWorldIndex { get { return CurrentGTIndex.GTChunkIndexToMCWorldXZ(); } }


  }
  

}
