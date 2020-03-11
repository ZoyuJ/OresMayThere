using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using u64 = System.Int64;
using static GTOresMayHere.WorldInfo;


namespace GTOresMayHere {
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
  public static class WorldInfo {
    public const u64 MCChunkLen = 16;
    public const u64 GTChunkLen = MCChunkLen * 3;
    public static readonly Vector SecondArea = new Vector(-8, 24);
    public static readonly Vector FirstArea = new Vector(24, 24);
    public static readonly Vector ThridArea = new Vector(-8, -8);
    public static readonly Vector FourthArea = new Vector(24, -8);

    public static readonly Vector ChunkSize = new Vector(MCChunkLen, MCChunkLen);

    public static void InputToWorldDirect(ConsoleKey Key,out ToDirector Direct) {

    }




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
    XNeg = 0x01,//1
    XPos = 0x02,//2
    ZNeg = 0x10,//16
    ZPos = 0x20,//32
    XPosAndZNeg = 0x12,//18
    XNegAndZNeg = 0x11,//17
    XPosAndZPos = 0x22,//34
    xNegAndZPos = 0x21,//33
  }

  public enum WorldDir {
    O = 0,
    W = 1,
    E = 2,
    S = 3,
    N = 4,
  }
  public static class Inline {
    public static string EToString(this in (string, Vector) This) => $"{This.Item1} {This.Item2.X} {This.Item2.Z}";

    public static Vector GTChunkIndexTranslateOne(this in Vector Current, ToDirector FromDir) => new Vector(Current.X + ((FromDir & ToDirector.XNeg) > 0 ? -1 : (FromDir & ToDirector.XPos) > 0 ? 1 : 0), Current.Z + ((FromDir & ToDirector.ZNeg) > 0 ? -1 : (FromDir & ToDirector.ZPos) > 0 ? 1 : 0));
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
