using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GTOresMayHere_Console {
  using u64 = System.Int64;
  using static Program;
  public class Program {
    public const u64 MCChunkLen = 16;
    public const u64 GTChunkLen = MCChunkLen * 3;
    public static readonly Vector SecondArea = (-8, 24);
    public static readonly Vector FirstArea = (24, 24);
    public static readonly Vector ThridArea = (-8, -8);
    public static readonly Vector FourthArea = (24, -8);


    static void Main(string[] args) {

    }

    static void GetNearest4GTChunk(Vector Current, Vector[] Chunks) {

    }



    public enum ToDirector : sbyte {
      /*    
       *          Z
       *          ↑
       *          |
       *          |
       * ---------|---------->X
       *          |
       *          |
       *          
       *          
       *          ↑
       *          |
       *          |
       * ---------|---------->X
       *          |
       *          |
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
       World =  Index.GTChunkIndexToMCWorldXZ();
    }



    public void ExpandEachDir() {
      if (West) {
        West = new GTChunkMapNode(this, ToDirector.West, (IndexX, IndexZ).GTChunkIndexTranslateOne(ToDirector.West));
      }
      if (East) {
        East = new GTChunkMapNode(this, ToDirector.East, (IndexX, IndexZ).GTChunkIndexTranslateOne(ToDirector.East));
      }
      if (North) {
        North = new GTChunkMapNode(this, ToDirector.North, (IndexX, IndexZ).GTChunkIndexTranslateOne(ToDirector.North));
      }
      if (South) {
        South = new GTChunkMapNode(this, ToDirector.South, (IndexX, IndexZ).GTChunkIndexTranslateOne(ToDirector.South));
      }
    }

    public void ExpandDir(ToDirector NextDir) {
      switch (NextDir) {
        case ToDirector.Origin:
          break;
        case ToDirector.West:
          West = new GTChunkMapNode(this, ToDirector.West, (IndexX, IndexZ).GTChunkIndexTranslateOne(ToDirector.West));
          break;
        case ToDirector.East:
          East = new GTChunkMapNode(this, ToDirector.East, (IndexX, IndexZ).GTChunkIndexTranslateOne(ToDirector.East));
          break;
        case ToDirector.North:
          North = new GTChunkMapNode(this, ToDirector.North, (IndexX, IndexZ).GTChunkIndexTranslateOne(ToDirector.North));
          break;
        case ToDirector.South:
          South = new GTChunkMapNode(this, ToDirector.South, (IndexX, IndexZ).GTChunkIndexTranslateOne(ToDirector.South));
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

    public static Vector operator +(in Vector L, in Vector R) => new Vector(L.X + R.X, L.Z + R.Z);
    public static Vector operator -(in Vector L, in Vector R) => new Vector(L.X - R.X, L.Z - R.Z);
    public static Vector operator *(in Vector L, in u64 R) => new Vector(L.X * R, L.Z * R);
    public static Vector operator /(in Vector L, in u64 R) => new Vector(L.X / R, L.Z / R);


  }


  public static class Inline {
    public static Vector GTChunkIndexTranslateOne(this in Vector Current, ToDirector FromDir) =>new Vector (Current.X + ((FromDir & ToDirector.West) > 0 ? -1 : (FromDir & ToDirector.East) > 0 ? 1 : 0), Current.Z + ((FromDir & ToDirector.North) > 0 ? -1 : (FromDir & ToDirector.South) > 0 ? 1 : 0));
    public static Vector WorldXZToGTChunkIndex(this in Vector Current) => (Current - FirstArea) / (GTChunkLen);
    public static Vector GTChunkIndexToMCWorldXZ(this in Vector Current) => Current * GTChunkLen + FirstArea;




    public static bool NullIsFalse<T>(this T Object) where T : class => Object == null;
  }
}
