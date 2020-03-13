using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GTOresMayHere {
  public static class UIConfig {
    public static Color MainBackgroundColor { get; set; } = Colors.Black;
    public static SolidColorBrush MainBackgroundBrush { get; set; } = new SolidColorBrush(Colors.Black);

    //public static Color AxeXColor { get; set; }
    //public static Color AxeXArrowColor { get; set; }
    //public static Color AxeYColor { get; set; }
    //public static Color AxeYArrowColor { get; set; }

    //public static Color AxeOriginColor { get; set; }
    //public static Color FaceToArrowColor { get; set; }

    public static Color DetectedPointColor { get; set; } = Colors.Blue;
    public static SolidColorBrush DetectedPointColorBrush { get => new SolidColorBrush(DetectedPointColor); }
    public static Color UndetectedPointColor { get; set; } = Colors.Wheat;
    public static SolidColorBrush UndetectedPointColorBrush { get => new SolidColorBrush(UndetectedPointColor); }


  }

  public static class UserConfig {
    private static Vector? _CurrentGTIndex;
    public static Vector InitGTIndex, InitWorldIndex;
    private static Vector? _LastGTIndex;
    public static Vector? CurrentGTIndex {
      get => _CurrentGTIndex;
      set {
        _LastGTIndex = _CurrentGTIndex;
        _CurrentGTIndex = value;
      }
    }
    public static Vector? LastGTIndex { get => _LastGTIndex; set => _LastGTIndex = value; }
    public static Vector? CurrentWorldIndex { get { return CurrentGTIndex?.GTChunkIndexToMCWorldXZ(); } }
    public static ToDirector CurrentDirection { get; set; } = ToDirector.Origin;

    public static Key NextPointKey;
    public static ModifierKeys NextPointKeyFrontKey1;
    public static ModifierKeys NextPointKeyFrontKey2;
    public static void FillShortcutsForNextPoint(in Dictionary<string, string> Args) {
      if (Args.TryGetValue("Key", out var Val1)) {
        Enum.TryParse<Key>(Val1, out NextPointKey);
      }
      else
        NextPointKey = Key.None;

      if (Args.TryGetValue("FKey1", out var Val2)) {
        Enum.TryParse<ModifierKeys>(Val2, out NextPointKeyFrontKey1);
      }
      else
        NextPointKeyFrontKey1 = ModifierKeys.None;
      if (Args.TryGetValue("FKey2", out var Val3)) {
        Enum.TryParse<ModifierKeys>(Val3, out NextPointKeyFrontKey2);
      }
      else
        NextPointKeyFrontKey2 = ModifierKeys.None;
    }

    public static Dictionary<ToDirector, WorldDir> MappedAxeToDirection;
    public static void FillMappedAxeToDirection(in Dictionary<string, string> Args) {
      MappedAxeToDirection = new Dictionary<ToDirector, WorldDir>(4);
      if (Args.TryGetValue(Enum.GetName(typeof(ToDirector), ToDirector.XNeg).Replace("Neg", "-"), out var Val1)) {
        if (Enum.TryParse<WorldDir>(Val1, out var CVal))
          MappedAxeToDirection[ToDirector.XNeg] = CVal;
        else
          MappedAxeToDirection[ToDirector.XNeg] = WorldDir.Undefined;
      }
      else {
        MappedAxeToDirection[ToDirector.XNeg] = WorldDir.Undefined;
      }

      if (Args.TryGetValue(Enum.GetName(typeof(ToDirector), ToDirector.XPos).Replace("Pos", "+"), out var Val2)) {
        if (Enum.TryParse<WorldDir>(Val2, out var CVal))
          MappedAxeToDirection[ToDirector.XPos] = CVal;
        else
          MappedAxeToDirection[ToDirector.XPos] = WorldDir.Undefined;
      }
      else {
        MappedAxeToDirection[ToDirector.XPos] = WorldDir.Undefined;
      }

      if (Args.TryGetValue(Enum.GetName(typeof(ToDirector), ToDirector.ZPos).Replace("Pos", "+"), out var Val3)) {
        if (Enum.TryParse<WorldDir>(Val3, out var CVal))
          MappedAxeToDirection[ToDirector.ZPos] = CVal;
        else
          MappedAxeToDirection[ToDirector.ZPos] = WorldDir.Undefined;
      }
      else {
        MappedAxeToDirection[ToDirector.ZPos] = WorldDir.Undefined;
      }

      if (Args.TryGetValue(Enum.GetName(typeof(ToDirector), ToDirector.ZNeg).Replace("Neg", "-"), out var Val4)) {
        if (Enum.TryParse<WorldDir>(Val4, out var CVal))
          MappedAxeToDirection[ToDirector.ZNeg] = CVal;
        else
          MappedAxeToDirection[ToDirector.ZNeg] = WorldDir.Undefined;
      }
      else {
        MappedAxeToDirection[ToDirector.ZNeg] = WorldDir.Undefined;
      }

    }

    public static string GameBuiltinMapWayPointsDirectory;
    public static string SelfMapWayPointsDirectory = @".\Points\";
    public static void FillPointsSaveToPath(in Dictionary<string, string> Args) {
      if (Args.TryGetValue("ExtSave", out var Val1)) {
        SelfMapWayPointsDirectory = Val1;
      }
      if (!Directory.Exists(SelfMapWayPointsDirectory)) Directory.CreateDirectory(SelfMapWayPointsDirectory);
      if (Args.TryGetValue("MapSave", out var Val2)) {
        GameBuiltinMapWayPointsDirectory = Val2;
      }
      if (!Directory.Exists(GameBuiltinMapWayPointsDirectory)) { MessageBox.Show($"MapSave参数 {Val2} 不存在"); Application.Current.Shutdown(); }
    }
    public static string AssembleGTIndexToFullFileName(in IndexDetail Detail) {
      return Detail.Index.ToShortestString() + "," + Detail.OreType.ToString() + ".gtp";
    }
    public static string AssembleGTIndexToIndexFileFilter(in Vector GTIndex) {
      return GTIndex.ToShortestString() + ",*" + ".gtp";
    }
    public static string AssembleGTIndexToOreTypeFileFilter(in IndexDetail Detail) {
      return "*,*" + "," + Detail.OreType.ToString() + ".gtp";
    }
    private static DirectoryInfo _GameBuiltinMapWayPointsDirectoryInfo;
    public static DirectoryInfo GameBuiltinMapWayPointsDirectoryInfo {
      get {
        if (GameBuiltinMapWayPointsDirectory == null) _GameBuiltinMapWayPointsDirectoryInfo = null;
        else {
          if (_GameBuiltinMapWayPointsDirectoryInfo == null) {
            _GameBuiltinMapWayPointsDirectoryInfo = new DirectoryInfo(GameBuiltinMapWayPointsDirectory);
          }
        }
        return _GameBuiltinMapWayPointsDirectoryInfo;
      }
    }

    private static DirectoryInfo _SelfMapWayPointsDirectoryInfo;
    public static DirectoryInfo SelfMapWayPointsDirectoryInfo {
      get {
        if (SelfMapWayPointsDirectory == null) _SelfMapWayPointsDirectoryInfo = null;
        else {
          if (_SelfMapWayPointsDirectoryInfo == null) {
            _SelfMapWayPointsDirectoryInfo = new DirectoryInfo(SelfMapWayPointsDirectory);
          }
        }
        return _SelfMapWayPointsDirectoryInfo;
      }
    }
    public static bool IsPointDetected(in Vector GTIndex, out IndexDetail Detail) {
      var FileFilter = AssembleGTIndexToIndexFileFilter(GTIndex);
      List<FileInfo> _Files = new List<FileInfo>();
      var _BuiltinFiles = GameBuiltinMapWayPointsDirectoryInfo?.GetFiles(FileFilter);
      if (_BuiltinFiles != null && _BuiltinFiles.Length > 0) {
        string[] Parts = Path.GetFileNameWithoutExtension(_BuiltinFiles[0].FullName).Split(',');
        Detail = IndexDetail.FromFileNameParts(Parts);
        return true;
      }
      var _SelfFiles = SelfMapWayPointsDirectoryInfo.GetFiles(FileFilter);
      if (_SelfFiles != null && _SelfFiles.Length > 0) {
        string[] Parts = Path.GetFileNameWithoutExtension(_SelfFiles[0].FullName).Split(',');
        Detail = IndexDetail.FromFileNameParts(Parts);
        return true;
      }
      Detail = IndexDetail.UndetectedPoint(GTIndex);
      return false;
    }
    public static void PointDetected(in IndexDetail Detail) {
      if (GameBuiltinMapWayPointsDirectoryInfo != null) {
        File.Create(Path.Combine(GameBuiltinMapWayPointsDirectory, AssembleGTIndexToFullFileName(Detail)), 1024, FileOptions.SequentialScan).Close();
        return;
      }
      File.Create(Path.Combine(SelfMapWayPointsDirectory, AssembleGTIndexToFullFileName(Detail)), 1024, FileOptions.SequentialScan).Close();
    }

    public static int AttachPID;
    public static string AttachProcessName;
    public static Process AttachedProcess;
    public static IntPtr AttachedWindow;
    public static void FillAttachProcess(in Dictionary<string, string> Args) {
      if (Args.TryGetValue("APid", out var Val1)) {
        AttachPID = Convert.ToInt32(Val1);
      }
      else if (Args.TryGetValue("APName", out var Val2)) {
        AttachProcessName = Val2;
      }

    }


  }


}
