﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GTOresMayHere_Console {
  using u64 = System.Int64;
  using static Program;
  public class Program {
    public const u64 MCChunkLen = 16;
    public const u64 GTChunkLen = MCChunkLen * 3;
    public static readonly Vector SecondArea = new Vector(-8, 24);
    public static readonly Vector FirstArea = new Vector(24, 24);
    public static readonly Vector ThridArea = new Vector(-8, -8);
    public static readonly Vector FourthArea = new Vector(24, -8);

    public static readonly Vector ChunkSize = new Vector(MCChunkLen, MCChunkLen);

    public static Vector CurrentGTIndex, InitGTIndex, InitWorldIndex;
    public static Vector CurrentWorldIndex { get { return CurrentGTIndex.GTChunkIndexToMCWorldXZ(); } }
    public static TemperaStorage Storage;

    public static int AutoFlushFileCount = 15, CurrentAutoFlushCount = 0;

    static void Main(string[] args) {
      _ = new Settings();
      Storage = new TemperaStorage();
      try {
        Storage.LoadFromFile();
      }
      catch (Exception) {
        Console.WriteLine("未加载之前的信息");
      }


      Console.WriteLine($"/////////////////原型机///////////////////");
      Console.WriteLine($"//////////////////////////////////////////");
      Console.WriteLine($"");
      Console.WriteLine($"");
      Console.WriteLine($"//////////////////////////////////////////");
      Console.WriteLine($"向东：{Settings.This.InputEast}");
      Console.WriteLine($"向西：{Settings.This.InputWest}");
      Console.WriteLine($"向南：{Settings.This.InputSouth}");
      Console.WriteLine($"向北：{Settings.This.InputNorth}");
      Console.WriteLine($"//////////////////////////////////////////");
      Console.WriteLine($"{Storage.LocalPoints?.Count} 已从本地导入");
      Console.WriteLine($"//////////////////////////////////////////");


      Console.WriteLine("输入当前坐标: like : -5110 -256");
      var InpCurrentWorld = Console.ReadLine().Trim();
      string[] WorldINDEXsTR = InpCurrentWorld.Split(' ');
      InitWorldIndex = new Vector(u64.Parse(WorldINDEXsTR[0]), u64.Parse(WorldINDEXsTR[1]));
      CurrentGTIndex = InitGTIndex = InitWorldIndex.WorldXZToGTChunkIndex();

      Console.WriteLine($"从{CurrentWorldIndex} @{CurrentGTIndex} 开始：↓↓↓↓");



      while (true) {
        Console.WriteLine("输入方向： 或 L键 显示已保存列表");
        var InputKey = Console.ReadKey();
        try {

          if (InputKey.Key == ConsoleKey.L) {
            DisplayList();
            continue;
          }

          var Dir = Settings.This.InputToWorldDir(InputKey.Key);
          Console.WriteLine($"\n{Enum.GetName(typeof(WorldDir), Dir)}");
          CurrentGTIndex = CurrentGTIndex.GTChunkIndexTranslateOne(Dir);
          if (Storage.AlreadyDetected(CurrentGTIndex, out (string, Vector) Detail)) {
            Console.WriteLine($"已探查区块：\n\t{Detail.Item1} @{Detail.Item2}");
          }
          else {
            var WI = CurrentWorldIndex;
            Console.WriteLine($"For LV采矿机 \n\t{WI + (Vector.NW * ChunkSize)},{WI + (Vector.N * ChunkSize)},{WI + (Vector.NE * ChunkSize)}\n\t{WI + (Vector.W * ChunkSize)},{WI},{WI + (Vector.E * ChunkSize)}\n\t{WI + (Vector.SW * ChunkSize)},{WI + (Vector.S * ChunkSize)},{WI + (Vector.SE * ChunkSize)}");
            Console.WriteLine($"新区块@{CurrentGTIndex}：输入区块信息,Enter 跳过(填充NONE)");
            var IndexInfo = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(IndexInfo)) IndexInfo = "NONE";
            Storage.AddNewPoints((IndexInfo, CurrentGTIndex));
            Storage.FlushToFile();
          }
        }
        catch (Exception) {
          Console.WriteLine($"Dont' know Key:{Enum.GetName(typeof(ConsoleKey), InputKey.Key)}");
        }
      }

      void DisplayList() {
        int LastIndex = 0;
        int Perpage = 10;
        do {
          Console.WriteLine($"当前页至：{LastIndex+Perpage}/{Storage.LocalPoints.Count}");
          Console.WriteLine($"|{"Index".Padding(5)}|{"GT Index".Padding(12)}|{"World Index".Padding(16)}| Info...");
          int i = LastIndex;
          for (; i < LastIndex+Perpage && i<Storage.LocalPoints.Count; i++) {
            Console.WriteLine($"{i.ToString().Padding(5)}|{Storage.LocalPoints[i].Item2.ToString().Padding(12)}|{Storage.LocalPoints[i].Item2.GTChunkIndexToMCWorldXZ().ToString().Padding(16)}|{Storage.LocalPoints[i].Item1}");
          }
          LastIndex = i;
          Console.WriteLine("输入 E 退出|输入 N 显示下一页");
          var InputKey = Console.ReadKey();
          if (InputKey.Key == ConsoleKey.E) return;
          else if (InputKey.Key == ConsoleKey.N) continue;
        } while (true);

      }

      while (true) {
        var Inp = Console.ReadKey();
        if (Inp.Key == ConsoleKey.M) {

          continue;
        }
        else {
          var K = Settings.This.InputToWorldDir(Inp.Key);
        J1:
          if (CurrentAutoFlushCount >= AutoFlushFileCount) {
            Storage.FlushToFileAsync();
          }
          Console.WriteLine("\t\tNext:");
          if (K == WorldDir.O) Console.WriteLine(CurrentWorldIndex);
          else {

            CurrentGTIndex = CurrentGTIndex.GTChunkIndexTranslateOne(K);
            (string, Vector) Detail;
            if (Storage.AlreadyDetected(CurrentGTIndex, out Detail)) {
              Console.WriteLine($"已查阅区块 —— {Detail.Item1} @{Detail.Item2}");
              Console.WriteLine("修改区块信息？Enter键取消");
              var ResetPointInp = Console.ReadLine().Trim();
              if (string.IsNullOrEmpty(ResetPointInp)) {
                continue;
              }
              else {
                ResetPointInp.Replace(' ', '_');
                Detail.Item1 = ResetPointInp;
              }
              continue;
            }
            Console.WriteLine(CurrentGTIndex.GTChunkIndexToMCWorldXZ());
            CurrentAutoFlushCount++;
            var SetPointInp = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(SetPointInp)) {
              Storage.AddNewPoints(("NONE", CurrentGTIndex));
              goto J1;
            }
            else if (Settings.This.InputIsDirector(SetPointInp, ref K)) {
              goto J1;
            }
            else {
              Storage.AddNewPoints((SetPointInp.Replace(' ', '_'), CurrentGTIndex));
              var NK = Console.ReadKey();
              if (NK.Key == ConsoleKey.Enter) goto J1;
              else {
                K = Settings.This.InputToWorldDir(NK.Key);
                goto J1;
              }
            }

          }
        }
      }



      void DisplayMarks() {
        int Index = 0;
        Settings.This.OrePointMarks?.ForEach(E => {

        });
      }


      string DisplayAndSearch(string KeyWord, List<string> Source) {
        int Index = 0;
        while (true) {
          if (string.IsNullOrEmpty(KeyWord)) {
            Source.ForEach(E => { Console.Write($"{Index}-{E}"); Console.Write("   "); Index++; });
            Index = 0;
            Console.WriteLine();
          }
          else {
            Source.ForEach(E => {
              if (E.Contains(KeyWord)) {
                Console.Write($"{Index}-{E}"); Console.Write("   ");
              }
              Index++;
            });
            Index = 0;
            Console.WriteLine();
          }
          Console.WriteLine("选择");

        }
      }

    }

    static void GetNearest4GTChunk(Vector Current, Vector[] Chunks) {

    }



    public enum ToDirector : sbyte {
      /*    
       *           Z+
       *           ↑
       *           |
       *           |
       * X- -------|---------->X+
       *           |
       *           |
       *           Z-
       *          
       *           S
       *           ↑
       *           |
       *           |
       * W---------|---------->E
       *           |
       *           |
       *           N
       *          
       **/
      Origin = 0x00,//0
      West = 0x01,//1
      East = 0x02,//2
      North = 0x10,//16
      South = 0x20,//32
      NorthEast = 0x12,//18
      NorthWest = 0x11,//17
      SouthEast = 0x22,//34
      SouthWest = 0x21,//33
    }

    public enum WorldDir {
      O = 0,
      W = 1,
      E = 2,
      S = 3,
      N = 4,
    }


  }


  public partial class Settings {

    public static Settings This;

    public ConsoleKey InputEast = ConsoleKey.D, InputWest = ConsoleKey.A, InputNorth = ConsoleKey.W, InputSouth = ConsoleKey.S;
    public WorldDir InputToWorldDir(ConsoleKey Key) {
      if (Key == InputEast) return WorldDir.E;
      if (Key == InputWest) return WorldDir.W;
      if (Key == InputNorth) return WorldDir.N;
      if (Key == InputSouth) return WorldDir.S;
      return WorldDir.O;
    }

    private string[] _InputDirStrs = null;
    public string[] InputDirStrs {
      get {
        if (_InputDirStrs == null)
          _InputDirStrs = new string[] { Enum.GetName(typeof(ConsoleKey), InputEast), Enum.GetName(typeof(ConsoleKey), InputWest), Enum.GetName(typeof(ConsoleKey), InputNorth), Enum.GetName(typeof(ConsoleKey), InputSouth) };
        return _InputDirStrs;
      }
    }

    public bool InputIsDirector(string Inp, ref WorldDir Dir) {
      if (Inp == InputDirStrs[0]) {
        Dir = WorldDir.E;
        return true;
      }
      else if (Inp == InputDirStrs[1]) {
        Dir = WorldDir.W;
        return true;
      }
      else if (Inp == InputDirStrs[2]) {
        Dir = WorldDir.N;
        return true;
      }
      else if (Inp == InputDirStrs[3]) {
        Dir = WorldDir.S;
        return true;
      }
      return false;
    }

    public string PointsFileLoadDirectoryPath, PointsFileLoadFileName, PointMarksFileName;
    public string LocalPointsFilePath { get { return Path.Combine(PointsFileLoadDirectoryPath, PointsFileLoadFileName); } }
    public string LocalPointMarksFilePath { get { return Path.Combine(PointsFileLoadDirectoryPath, PointMarksFileName); } }

    public static readonly char[] TrimChars = new char[] { '\n', '\r', ' ' };

    public Settings() {
      This = this;
      PointsFileLoadDirectoryPath = System.Environment.CurrentDirectory;
      PointsFileLoadFileName = "OrePoints.txt";
      try {
        if (File.Exists(LocalPointMarksFilePath)) {
          OrePointMarks = new List<string>(LocalPointMarksFilePath.FileReadLine().Select(E => E.Trim(TrimChars)));
        }
      }
      catch { }
    }

    public void CheckPointsFile() {
      if (!File.Exists(LocalPointsFilePath)) File.Create(LocalPointsFilePath).Dispose();
    }

    public List<string> OrePointMarks;


  }

  public class TemperaStorage {

    public List<(string, Vector)> LocalPoints = new List<(string, Vector)>();
    //public List<(string, Vector)> NewPoints = new List<(string, Vector)>();

    public void LoadFromFile() {
      foreach (var item in Settings.This.LocalPointsFilePath.FileReadLine()) {
        try {
          string[] Parts = item.Trim(Settings.TrimChars).Split(' ');
          LocalPoints.Add((Parts[0], new Vector(u64.Parse(Parts[1]), u64.Parse(Parts[2]))));
        }
        catch (Exception) {
          Console.WriteLine($"Cant Load {item}");
        }
      }
    }

    public void AddNewPoints((string, Vector) Point) {
      LocalPoints.Add(Point);
    }

    public void AppendToFile() {
      Settings.This.LocalPointsFilePath.FlushToFile(new List<string>(LocalPoints.Select(E => E.EToString())));
    }
    public void FlushToFile() {
      //if (File.Exists(Settings.This.LocalPointsFilePath)) {
      //  FileInfo FI = new FileInfo(Settings.This.LocalPointsFilePath);
      //  FI.MoveTo(Path.Combine(Path.GetDirectoryName(Settings.This.LocalPointsFilePath), $"backup {DateTime.Now.ToFileTime()}.txt"));
      //}
      Settings.This.LocalPointsFilePath.FlushToFile(new List<string>(LocalPoints.Select(E => E.EToString())));
    }

    public void FlushToFileAsync() {
      Task.Run(() => {
        FlushToFile();
      });
    }

    public void LoadFromString(string Data) {
      Data = Data.Trim(Settings.TrimChars);
      if (Data.Contains('\n')) {
        string[] Eles = Data.Split('\n');
        Array.ForEach(Eles, E => LoadFromString(E));
      }
      else {
        string[] Parts = Data.Trim(Settings.TrimChars).Split(' ');
        LocalPoints.Add((Parts[0], new Vector(u64.Parse(Parts[1]), u64.Parse(Parts[2]))));
      }
    }

    public void RemovePoint(Vector Point) {
      LocalPoints.RemoveSelectLastOne(E => E.Item2 == Point);
    }

    public bool AlreadyDetected(Vector Point, out (string, Vector) Detail) {
      for (int i = 0; i < LocalPoints.Count; i++) {
        if (LocalPoints[i].Item2 == Point) {
          Detail = LocalPoints[i];
          return true;
        }
      }
      Detail = default;
      return false;
    }

    public List<(string, Vector, u64)> SelectNearest(Vector Point) {
      List<(string, Vector, u64)> Selected = new List<(string, Vector, u64)>();
      LocalPoints.ForEach(E => Selected.Add((E.Item1, E.Item2, E.Item2.XAndZDistanceTotal(Point))));
      Selected.Sort((L, R) => L.Item3 - R.Item3);
      return Selected;
    }

    public List<(string, Vector)> SelectedMark(string Mark) => new List<(string, Vector)>(LocalPoints.Where(E => E.Item1 == Mark));


  }


  [Serializable]
  public class GTChunkMapNode {
    public Vector Index;
    public Vector World;
    public GTChunkMapNode West, East, North, South;


    public GTChunkMapNode(GTChunkMapNode From, ToDirector Dir) {
      switch (Dir) {
        case ToDirector.Origin:
          break;
        case ToDirector.West:
          East = From;
          break;
        case ToDirector.East:
          West = From;
          break;
        case ToDirector.North:
          South = From;
          break;
        case ToDirector.South:
          North = From;
          break;
        default:
          break;
      }
    }
    public GTChunkMapNode(GTChunkMapNode From, ToDirector FromDir, Vector Index) : this(From, FromDir) {
      World = Index.GTChunkIndexToMCWorldXZ();
    }



    public void ExpandEachDir() {
      if (West) {
        West = new GTChunkMapNode(this, ToDirector.West, Index.GTChunkIndexTranslateOne(ToDirector.West));
      }
      if (East) {
        East = new GTChunkMapNode(this, ToDirector.East, Index.GTChunkIndexTranslateOne(ToDirector.East));
      }
      if (North) {
        North = new GTChunkMapNode(this, ToDirector.North, Index.GTChunkIndexTranslateOne(ToDirector.North));
      }
      if (South) {
        South = new GTChunkMapNode(this, ToDirector.South, Index.GTChunkIndexTranslateOne(ToDirector.South));
      }
    }

    public void ExpandDir(ToDirector NextDir) {
      switch (NextDir) {
        case ToDirector.Origin:
          break;
        case ToDirector.West:
          West = new GTChunkMapNode(this, ToDirector.West, Index.GTChunkIndexTranslateOne(ToDirector.West));
          break;
        case ToDirector.East:
          East = new GTChunkMapNode(this, ToDirector.East, Index.GTChunkIndexTranslateOne(ToDirector.East));
          break;
        case ToDirector.North:
          North = new GTChunkMapNode(this, ToDirector.North, Index.GTChunkIndexTranslateOne(ToDirector.North));
          break;
        case ToDirector.South:
          South = new GTChunkMapNode(this, ToDirector.South, Index.GTChunkIndexTranslateOne(ToDirector.South));
          break;
        default:
          break;
      }
    }

    public void ExpandTo(Vector Point) {

    }

    public GTChunkMapNode GetNext(ToDirector NextDir, bool ExpandNull = true) {
      switch (NextDir) {
        case ToDirector.Origin:
          return this;
        case ToDirector.West:
          if (ExpandNull && West) ExpandDir(NextDir);
          return West;
        case ToDirector.East:
          if (ExpandNull && East) ExpandDir(NextDir);
          return East;
        case ToDirector.North:
          return North;
        case ToDirector.South:
          return South;
        default:
          return null;
      }
    }

    public static implicit operator bool(GTChunkMapNode Node) => Node == null;

  }

  [Serializable]
  public class GTChunkMap {
    List<GTChunkMapNode> Nodes = new List<GTChunkMapNode>();
    List<Rect> EdgesAnla = new List<Rect>();




  }

  [Serializable]
  public class Rect {
    /*
     *1-----2
     *|     |
     *0-----3 
     */
    public Vector[] Points = new Vector[4];

    public bool Overlap(Rect Other) {
      for (int i = 0; i < 4; i++) {
        if (Inside(Other.Points[i])) return true;
      }
      return false;
    }
    public bool Inside(Vector Point) {
      return (Point.X >= Points[0].X && Point.X <= Points[2].X) && (Point.Z >= Points[0].Z && Point.Z <= Points[2].Z);
    }
  }
  [Serializable]
  public struct Vector {
    public u64 X, Z;
    public Vector(u64 _X, u64 _Z) { X = _X; Z = _Z; }

    public override bool Equals(object obj) {
      return X == ((Vector)obj).X && Z == ((Vector)obj).Z;
    }

    public override string ToString() => $"({X},{Z})";

    /*    
    *           Z+
    *           ↑
    *           |
    *           |
    * X- -------|---------->X+
    *           |
    *           |
    *           Z-
    *          
    *           S
    *           ↑
    *           |
    *           |
    * W---------|---------->E
    *           |
    *           |
    *           N
    *          
    **/
    public static Vector O { get => new Vector(0, 0); }
    public static Vector W { get => new Vector(-1, 0); }
    public static Vector E { get => new Vector(1, 0); }
    public static Vector S { get => new Vector(0, 1); }
    public static Vector N { get => new Vector(0, -1); }
    public static Vector SW { get => new Vector(-1, 1); }
    public static Vector NW { get => new Vector(-1, -1); }
    public static Vector SE { get => new Vector(1, 1); }
    public static Vector NE { get => new Vector(1, -1); }

    public static Vector operator +(in Vector L, in Vector R) => new Vector(L.X + R.X, L.Z + R.Z);
    public static Vector operator -(in Vector L, in Vector R) => new Vector(L.X - R.X, L.Z - R.Z);
    public static Vector operator *(in Vector L, in u64 R) => new Vector(L.X * R, L.Z * R);
    public static Vector operator *(in Vector L, in Vector R) => new Vector(L.X * R.X, L.Z * R.Z);
    public static Vector operator /(in Vector L, in u64 R) => new Vector(L.X / R, L.Z / R);
    public static Vector operator /(in Vector L, in Vector R) => new Vector(L.X / R.X, L.Z / R.Z);

    public static bool operator ==(in Vector L, in Vector R) => L.X == R.X && L.Z == R.Z;
    public static bool operator !=(in Vector L, in Vector R) => L.X != R.X || L.Z != R.Z;

    public u64 XAndZDistanceTotal(Vector R) => Math.Abs(R.X - X) + Math.Abs(R.Z - Z);

  }


  public static class Inline {
    public static string EToString(this in (string, Vector) This) => $"{This.Item1} {This.Item2.X} {This.Item2.Z}";

    public static Vector GTChunkIndexTranslateOne(this in Vector Current, ToDirector FromDir) => new Vector(Current.X + ((FromDir & ToDirector.West) > 0 ? -1 : (FromDir & ToDirector.East) > 0 ? 1 : 0), Current.Z + ((FromDir & ToDirector.North) > 0 ? -1 : (FromDir & ToDirector.South) > 0 ? 1 : 0));
    public static Vector GTChunkIndexTranslateOne(this in Vector Current, WorldDir FromDir) => new Vector(Current.X + (FromDir == WorldDir.W ? -1 : FromDir == WorldDir.E ? 1 : 0), Current.Z + (FromDir == WorldDir.N ? -1 : FromDir == WorldDir.S ? 1 : 0));
    public static Vector WorldXZToGTChunkIndex(this in Vector Current) => (Current - FirstArea) / (GTChunkLen);
    public static Vector GTChunkIndexToMCWorldXZ(this in Vector Current) => Current * GTChunkLen + FirstArea;

    public static readonly byte[] NEXTLINE = Encoding.UTF8.GetBytes("\n");
    public static void AppendToFile(this string FilePath, byte[] Data) {
      if (!File.Exists(FilePath)) File.Create(FilePath).Dispose();
      FileStream FS = new FileStream(FilePath, FileMode.Append, FileAccess.Write);
      FS.Write(Data, 0, Data.Length);
      FS.Write(NEXTLINE, 0, NEXTLINE.Length);
      FS.Flush();
      FS.Close();
    }
    public static void AppendToFile(this string FilePath, List<string> Data) {
      if (!File.Exists(FilePath)) File.Create(FilePath).Dispose();
      FileStream FS = new FileStream(FilePath, FileMode.Append, FileAccess.Write);
      Data.ForEach(E => {
        byte[] Btys = Encoding.UTF8.GetBytes(E);
        FS.Write(Btys, 0, Btys.Length);
        FS.Write(NEXTLINE, 0, NEXTLINE.Length);
      });
      FS.Flush();
      FS.Close();
    }
    public static void FlushToFile(this string FilePath, List<string> Data) {
      if (File.Exists(FilePath)) File.Delete(FilePath);
      try {
        FileStream FS = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
        Data.ForEach(E => {
          byte[] Btys = Encoding.UTF8.GetBytes(E);
          FS.Write(Btys, 0, Btys.Length);
          FS.Write(NEXTLINE, 0, NEXTLINE.Length);
        });
        FS.Flush();
        FS.Close();
      }
      catch (Exception) {

        throw;
      }
    }
    public static IEnumerable<string> FileReadLine(this string Path) {
      StreamReader sr = new StreamReader(Path);
      string line = null;
      while ((line = sr.ReadLine()) != null) {
        yield return line;
      }
      sr.Close();
    }

    public static void Sort<T>(this List<T> This, Func<T, T, u64> Compare) {
      T Temp = default;
      for (int i = 0; i < This.Count - 1; i++) {
        for (int j = 0; j < This.Count - 1 - i; j++) {
          if (Compare(This[j], This[j + 1]) < 0) {
            Temp = This[j + 1];
            This[j + 1] = This[j];
            This[j] = Temp;
          }
        }
      }
    }

    public static bool NullIsFalse<T>(this T Object) where T : class => Object == null;

    public static string PaddingLeft(this string This, int MaxLength) {
      var chars = new char[MaxLength];
      for (int i = chars.Length - 1; i >= 0; i--) {
        int Index = This.Length - chars.Length - i;
        chars[i] = Index >= 0 ? This[Index] : ' ';
      }
      return new string(chars);
    }
    public static string PaddingRight(this string This, int MaxLength) {
      var chars = new char[MaxLength];
      for (int i = 0; i < chars.Length; i++) {
        chars[i] = i < This.Length ? This[i] : ' ';
      }
      return new string(chars);
    }
    public static string Padding(this string This, int MaxLength) {
      var chars = new char[MaxLength];
      int offset = MaxLength - This.Length / 2;
      for (int i = 0; i < chars.Length; i++) {
        int Index = i - offset < This.Length ? i - offset : -1;
        chars[i] = Index > 0 ? This[Index] : ' ';
      }
      return new string(chars);
    }
    public static bool RemoveSelectLastOne<T>(this IList<T> This, Predicate<T> Match) {
      for (int i = This.Count - 1; i >= 0; i--) {
        if (Match(This[i])) { This.RemoveAt(i); return true; }
      }
      return false;
    }
  }
}
