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
    public static readonly (u64, u64) SecondArea = (-8, 24);
    public static readonly (u64, u64) FirstArea = (24, 24);
    public static readonly (u64, u64) ThridArea = (-8, -8);
    public static readonly (u64, u64) FourthArea = (24, -8);


    static void Main(string[] args) {

    }

    static void GetNearest4GTChunk((u64, u64) Current, (u64, u64)[] Chunks) {

    }


    
    public enum ToDirector : sbyte {
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
    public u64 IndexX, IndexZ;
    public u64 WorldX, WorldZ;
    public GTChunkMapNode West, East, North, South;
    public GTChunkMapNode[] Nexts = new GTChunkMapNode[4];

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
    public GTChunkMapNode(GTChunkMapNode From, ToDirector FromDir, u64 IndexX, u64 IndexZ) : this(From, FromDir) {
      var World = (IndexX, IndexZ).GTChunkIndexToMCWorldXZ();
      WorldX = World.Item1;
      WorldZ = World.Item2;
    }
    public GTChunkMapNode(GTChunkMapNode From, ToDirector Dir, (u64, u64) Index) : this(From, Dir) {
      var World = Index.GTChunkIndexToMCWorldXZ();
      WorldX = World.Item1;
      WorldZ = World.Item2;
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

    public GTChunkMapNode GetNext(ToDirector NextDir,bool ExpandNull = true) {
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



  public static class Inline {
    public static int DirectorToArrayIndex(this ToDirector Dir)=>
    public static (u64, u64) GTChunkIndexTranslateOne(this in (u64, u64) Current, ToDirector FromDir) => (Current.Item1 + ((FromDir & ToDirector.West) > 0 ? -1 : (FromDir & ToDirector.East) > 0 ? 1 : 0), Current.Item2 + ((FromDir & ToDirector.North) > 0 ? -1 : (FromDir & ToDirector.South) > 0 ? 1 : 0));
    public static (u64, u64) WorldXZToGTChunkIndex(this in (u64, u64) Current) => Current.Subtract(FirstArea).Divide(GTChunkLen);
    public static (u64, u64) GTChunkIndexToMCWorldXZ(this in (u64, u64) Current) =>;

    public static (u64, u64) Subtract(this (u64, u64) L, (u64, u64) R) => (L.Item1 - R.Item1, L.Item2 - R.Item2);
    public static (u64, u64) Subtract(this (u64, u64) L, u64 R) => (L.Item1 - R, L.Item2 - R);
    public static (u64, u64) Add(this (u64, u64) L, (u64, u64) R) => (L.Item1 + R.Item1, L.Item2 + R.Item2);
    public static (u64, u64) Add(this (u64, u64) L, u64 R) => (L.Item1 + R, L.Item2 + R);

    public static (u64, u64) Divide(this (u64, u64) L, (u64, u64) R) => (L.Item1 / R.Item1, L.Item2 / R.Item2);
    public static (u64, u64) Divide(this (u64, u64) L, u64 R) => (L.Item1 / R, L.Item2 / R);

    public static (u64, u64) Multiply(this (u64, u64) L, (u64, u64) R) => (L.Item1 * R.Item1, L.Item2 * R.Item2);
    public static (u64, u64) Multiply(this (u64, u64) L, u64 R) => (L.Item1 * R, L.Item2 * R);


    public static bool NullIsFalse<T>(this T Object) where T : class => Object == null;
  }
}
