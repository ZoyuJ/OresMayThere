using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GTOresMayHere.WorldInfo;
using Newtonsoft.Json;


namespace GTOresMayHere {
  [Serializable]
  [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
  public struct Vector {
    [JsonProperty(PropertyName ="1")]
    public int X { get; set; }
    [JsonProperty(PropertyName = "2")]
    public int Z { get; set; }
    public Vector(int _X, int _Z) { X = _X; Z = _Z; }

    public override bool Equals(object obj) {
      return X == ((Vector)obj).X && Z == ((Vector)obj).Z;
    }

    public override string ToString() => $"({X},{Z})";
    public string ToShortestString() => $"{X},{Z}";
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
    public static Vector operator *(in Vector L, in int R) => new Vector(L.X * R, L.Z * R);
    public static Vector operator *(in Vector L, in Vector R) => new Vector(L.X * R.X, L.Z * R.Z);
    public static Vector operator /(in Vector L, in int R) => new Vector(L.X / R, L.Z / R);
    public static Vector operator /(in Vector L, in Vector R) => new Vector(L.X / R.X, L.Z / R.Z);

    public static bool operator ==(in Vector L, in Vector R) => L.X == R.X && L.Z == R.Z;
    public static bool operator !=(in Vector L, in Vector R) => L.X != R.X || L.Z != R.Z;

    public int XAndZDistanceTotal(Vector R) => Math.Abs(R.X - X) + Math.Abs(R.Z - Z);

  }

  [Serializable]
  [JsonObject(MemberSerialization =MemberSerialization.OptIn)]
  public struct IndexDetail {
    [JsonProperty(PropertyName ="1")]
    public Vector Index { get; set; }
    [JsonProperty(PropertyName = "2")]
    public string OreType { get; set; }
    [JsonIgnore]
    public bool Detected { get; set; }

    public static IndexDetail None = new IndexDetail() { Index = new Vector(), OreType = "Nothing", Detected = false };
    public static IndexDetail UndetectedPoint(in Vector Index) => new IndexDetail() { Index = Index, OreType = "Nothing", Detected = false };
    public static IndexDetail FromFileNameParts(string[] Parts, bool Detected = true) {
      var New = new IndexDetail();
      New.Index = new Vector(Convert.ToInt32(Parts[0]), Convert.ToInt32(Parts[1]));
      New.OreType = Parts[2];
      New.Detected = Detected;
      return New;
    }

    
  }

  public static class WorldInfo {
    public const int MCChunkLen = 16;
    public const int GTChunkLen = MCChunkLen * 3;
    public static readonly Vector SecondArea = new Vector(-8, 24);
    public static readonly Vector FirstArea = new Vector(24, 24);
    public static readonly Vector ThridArea = new Vector(-8, -8);
    public static readonly Vector FourthArea = new Vector(24, -8);

    public static readonly Vector ChunkSize = new Vector(MCChunkLen, MCChunkLen);




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
    Undefined = -1,
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

    public static void Sort<T>(this List<T> This, Func<T, T, int> Compare) {
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

  [Serializable]
  public class ExtenWayPoint {
    public ExtenWayPoint(string Name, int X, int Z) {
      name = Name;
      x = X;
      z = Z;
      AssembleId();
      RandomColor();
    }
    void AssembleId() {
      id = $"{name}_{x},{y},{z}";
    }
    private readonly static Random ColorGenRandom = new Random();
    void RandomColor() {
      r = ColorGenRandom.Next(0, 256);
      g = ColorGenRandom.Next(0, 256);
      b = ColorGenRandom.Next(0, 256);
    }
    public string id { get; set; }
    public string name { get; set; }
    public string icon { get; set; } = "waypoint-normal.png";
    public int x { get; set; }
    public int y { get; set; } = 60;
    public int z { get; set; }
    public int r { get; set; }
    public int g { get; set; }
    public int b { get; set; }
    public bool enable { get; set; } = true;
    public string type { get; set; } = "Normal";
    public string origin { get; set; } = "JourneyMap";
    public int[] dimensions { get; set; } = new int[] { 6 };

    public string WayPointFileName() { return $"{id}.waypoint"; }

  }
}
