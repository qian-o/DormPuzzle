using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DormPuzzle.Models;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace DormPuzzle.Game.Tetris
{
    public sealed class Block: IEquatable<Block>
    {
        public static readonly int MaxWidth = 4;
        public static readonly int MaxHeight = 4;

        /// <summary>
        /// 从 1 开始，每一个为一种类型的 Block
        /// </summary>
        public int Type { get; }
        /// <summary>
        /// 从 0 开始，每一个为一种类型 Block 的不同旋转摆放
        /// </summary>
        public int Rot { get; }
        public int Width { get; }
        public int Height { get; }
        public int NumCells { get; }

        private readonly bool[] _cells;

        private static readonly List<List<Block>> _blocks;

        public static int NumTypes => _blocks.Count;

        public static int? NumRots(int type)
        {
            if (IsValidType(type))
                return _blocks[type - 1].Count;
            return null;
        }

        public static Block? Get(int type, int rot)
        {
            if (NumRots(type) is { } num && num > rot)
                return _blocks[type - 1][rot];
            return null;
        }

        public static bool IsValidType(int type)
        {
            return 0 < type && type <= NumTypes;
        }

        static Block()
        {
            var bases = new Block[]
            {
                new Block(1, 0, new byte[,]
                {
                    { 1, 1 },
                    { 1, 1 }
                }),
                new Block(2, 0, new byte[,]
                {
                    { 1, 1, 1, 1 }
                }),
                new Block(33, 0, new byte[,]
                {
                    { 1, 1, 0 },
                    { 0, 1, 1 }
                }),
                new Block(4, 0, new byte[,]
                {
                    { 0, 1, 1 },
                    { 1, 1, 0 },
                }),
                new Block(5, 0, new byte[,]
                {
                    { 1, 0, 0 },
                    { 1, 1, 1 },
                }),
                new Block(6, 0, new byte[,]
                {
                    { 0, 0, 1 },
                    { 1, 1, 1 },
                }),
                new Block(7, 0, new byte[,]
                {
                    { 0, 1, 0 },
                    { 1, 1, 1 },
                }),
                new Block(8, 0, new byte[,]
                {
                    { 0, 8, 0 },
                    { 8, 8, 8 },
                    { 0, 8, 0 },
                }),
                new Block(9, 0, new byte[,]
                {
                    { 1 }
                }),
                new Block(10, 0, new byte[,]
                {
                    { 1, 1 }
                }),
                new Block(11, 0, new byte[,]
                {
                    { 1, 0 },
                    { 1, 1 },
                }),
            };

            var blocks = new List<List<Block>>(bases.Length);

            foreach (var blk in bases)
            {
                var set = new List<Block>();
                blocks.Add(set);
                set.Add(blk);

                // 旋转三次，只保留 Cells 不同的
                int rot = 1;
                for (var i = 1; i < 4; i++)
                {
                    var rotated = blk.RotateCW(rot);
                    if (!set.Exists(x => x.AreCellsSame(rotated)))
                    {
                        set.Add(rotated);
                        rot++;
                    }
                }
            }

            _blocks = blocks;
        }

        private Block(int type, int rot, byte[,] cells)
        {
            var height = cells.GetLength(0);
            var width = cells.GetLength(1);

            Debug.Assert(width <= MaxWidth);
            Debug.Assert(height <= MaxHeight);

            Type = type;
            Rot = rot;
            Width = width;
            Height = height;

            _cells = new bool[Width * Height];
            var index = 0;
            var numCells = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    _cells[index] = cells[y, x] == 1;
                    if (_cells[index])
                        numCells++;
                    index++;
                }
            }
            NumCells = numCells;
        }

        private Block RotateCW(int rot)
        {
            var width = Height;
            var height = Width;

            var cells = new byte[height, width];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    cells[y, x] = cells[width - x - 1, y];
                }
            }

            return new Block(Type, rot, cells);
        }

        /// <summary>
        /// 用来检查两个 Block 是否有完全一样的 Cells
        /// </summary>
        private bool AreCellsSame(Block other)
        {
            return Width == other.Width && Height == other.Height && _cells.SequenceEqual(other._cells);
        }

        public bool? GetCell(int x, int y)
        {
            if (0 <= x && x < Width && 0 <= y && y <= Height)
                return _cells[x + y * Width];
            return null;
        }

        public override bool Equals(object? obj) => this.Equals(obj as Block);

        public bool Equals(Block? other)
        {
            if (other is null)
                return false;
            
            if (ReferenceEquals(this, other))
                return true;

            return Type == other.Type &&
                Rot == other.Rot;
        }

        public static bool operator ==(Block? left, Block? right) => left is { } l && l.Equals(right);

        public static bool operator !=(Block? left, Block? right) => !(left == right);
    }

    public struct Placement
    {
        public int X;
        public int Y;
        public Block Block;
    }

    public struct Solution
    {
        public List<Placement> Placements;
        public int Score;

        public Solution Clone()
        {
            var res = new Solution
            {
                Placements = new List<Placement>(Placements.Count),
                Score = Score
            };

            foreach (var p in Placements)
            {
                res.Placements.Add(p);
            }

            return res;
        }
    }

    public class Map
    {
        public static readonly int EmptyCell = 0;
        public static readonly int WallCell = 0;

        public readonly int Width;
        public readonly int Height;
        private readonly sbyte[] _map;

        public int NumNotWall => _map.Count(c => c != WallCell);

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            _map = new sbyte[Width * Height];
        }

        public static bool IsValidCell(int type)
        {
            return type == EmptyCell || type == WallCell || Block.IsValidType(type);
        }

        private int IndexOfUnchecked(int x, int y)
        {
            return x + y * Width;
        }

        private int IndexOfChecked(int x, int y)
        {
            if (IndexOf(x, y) is { } index)
            {
                return index;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public int? IndexOf(int x, int y)
        {
            if (0 <= x && x < Width && 0 <= y && y < Height)
            {
                return IndexOfUnchecked(x, y);
            }
            return null;
        }

        public (int, int)? XYOf(int index)
        {
            if (0 <= index && index < Width * Height)
            {
                return (index % Width, index / Width);
            }
            return null;
        }

        public void SetCell(int x, int y, int type)
        {
            SetCell(IndexOfChecked(x, y), type);
        }

        public void SetCell(int index, int type)
        {
            if (IsValidCell(type))
            {
                _map[index] = (sbyte)type;
            }
            else
            {
                throw new ArgumentException("Invalid cell type");
            }
        }

        public int GetCell(int x, int y) => _map[IndexOfChecked(x, y)];

        public int GetCell(int index) => _map[index];

        public void Clear()
        {
            for (int i = 0; i < _map.Length; i++)
            {
                _map[i] = (sbyte)EmptyCell;
            }
        }

        /// <summary>
        /// 在地图上摆放一个 Block，x 和 y 为摆放位置，以该位置为 block 的左上角来摆放 block，摆放的时候会检查要摆放的位置是否为空，只有全都为空时才会摆放。
        /// 如果 altnative 不为 null，则摆放 block 时用 altnative 来代替 block 中不为空的 Cell，可以用来取消摆放一个 Block。
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        /// <param name="block">要摆放的 Block</param>
        /// <param name="altnative">替换的 Cell</param>
        /// <param name="force">是否强制摆放，为 true 时直接替换掉原有的 Cell</param>
        /// <returns>是否摆放成功</returns>
        public bool SetBlock(int x, int y, Block block, int? altnative = null, bool force = false)
        {
            if (IndexOf(x, y) is { } index)
            {
                return SetBlock(index, block, altnative, force);
            }
            return false;
        }

        /// <summary>
        /// 功能和同名重载一样
        /// </summary>
        public bool SetBlock(int index, Block block, int? altnative = null, bool force = false)
        {
            if (!(0 <= index && index < Width * Height))
                return false;
            var cell = altnative ?? block.Type;
            Debug.Assert(IsValidCell(cell));

            var dstY = index / Width;
            var dstX = index % Width;

            if ((dstX + block.Width) > Width || (dstY + block.Height) > Height)
                return false;

            if (!force)
            {
                for (var h = 0; h < block.Height; h++)
                {
                    for (var w = 0; w < block.Width; w++)
                    {
                        var dst = _map[index + w + h * Width];
                        var src = block.GetCell(w, h) ?? true;
                        if (dst != EmptyCell && src)
                            return false;
                    }
                }
            }

            for (var h = 0; h < block.Height; h++)
            {
                for (var w = 0; w < block.Width; w++)
                {
                    _map[index + w + h * Width] = (sbyte)cell;
                }
            }
            return true;
        }

        public bool CanSetBlock(int index, Block block)
        {
            if (!(0 <= index && index < Width * Height))
                return false;

            var dstY = index / Width;
            var dstX = index % Width;

            if ((dstX + block.Width) > Width || (dstY + block.Height) > Height)
                return false;

            for (var h = 0; h < block.Height; h++)
            {
                for (var w = 0; w < block.Width; w++)
                {
                    var dst = _map[index + w + h * Width];
                    var src = block.GetCell(w, h) ?? true;
                    if (dst != EmptyCell && src)
                        return false;
                }
            }
            return true;
        }
    }

    public struct SolveOptions
    {
        /// <summary>
        /// 地图宽度
        /// </summary>
        public int Width;

        /// <summary>
        /// 地图高度
        /// </summary>
        public int Height;

        /// <summary>
        /// 依次为每个类型 Block 的数量
        /// </summary>
        public int[] BlockNums;

        /// <summary>
        /// 墙体位置
        /// </summary>
        public Location[]? Walls;

        /// <summary>
        /// 最大搜索方案数量，这个数值过大会导致计算耗时增加
        /// </summary>
        public int MaxSolutions;

        /// <summary>
        /// 数量越多的 Block 优先级越高
        /// </summary>
        public bool PreferLargerNumber;

        /// <summary>
        /// 只保留最高分的方案
        /// </summary>
        public bool KeepTopOnly;

        private int TotalBlockNums => BlockNums.Aggregate(0, (acc, b) => acc + b);

        public SolveOptions(int width, int height, int[] blockNums)
        {
            Width = width;
            Height = height;
            BlockNums = new int[Block.NumTypes];
            blockNums.CopyTo(BlockNums, 0);

            Walls = null;
            MaxSolutions = 50000;
            PreferLargerNumber = false;
            KeepTopOnly = false;
        }

        public int[] MakeOrderedTypes()
        {
            var res = new int[Block.NumTypes];
            for (var i = 0; i < res.Length; i++)
            {
                res[i] = i + 1;
            }

            if (PreferLargerNumber)
            {
                Array.Sort(BlockNums, res, Comparer<int>.Create((a, b) => b.CompareTo(a)));
            }
            return res;
        }

        private Map MakeMap()
        {
            var map = new Map(Width, Height);
            if (Walls != null)
            {
                foreach (var wall in Walls)
                {
                    map.SetCell(wall.Column, wall.Row, Map.WallCell);
                }
            }
            return map;
        }

        private struct SolveDfs
        {
            /// <summary>
            /// 按照优先级排过序的类型列表
            /// </summary>
            private int[] _types;
            /// <summary>
            /// 各个类型 Block 的数量，[0] 不使用
            /// </summary>
            private int[] _blockNums;
            private List<Solution> _slns;
            private Map _map;
            private Solution _tempSln;
            private int _remainBlocks;
            private int _maxSolutions;

            public SolveDfs(SolveOptions options)
            {
                _types = options.MakeOrderedTypes();
                _blockNums = new int[Block.NumTypes + 1];
                options.BlockNums.CopyTo(_blockNums, 1);
                _maxSolutions = options.MaxSolutions;
                _remainBlocks = options.TotalBlockNums;
                _map = options.MakeMap();
                _slns = new List<Solution>();
                _tempSln = new Solution();
            }

            public List<Solution> Solve()
            {
                Solve(0);
                return _slns;
            }

            private bool Solve(int index)
            {
                if (index >= _map.Width * _map.Height || _remainBlocks <= 0)
                {
                    _slns.Add(_tempSln.Clone());
                    return _slns.Count >= _maxSolutions;
                }

                for (int i = 0; i < _types.Length; i++)
                {
                    var type = _types[i];
                    if (_blockNums[type] <= 0)
                    {
                        continue;
                    }

                    var numRots = Block.NumRots(type);
                    for (int rot = 0; rot < numRots; rot++)
                    {
                        var block = Block.Get(type, rot);
                        Debug.Assert(block != null, nameof(block) + " != null");
                        if (!_map.CanSetBlock(index, block))
                        {
                            continue;
                        }

                        _blockNums[type] -= 1;
                        _remainBlocks -= 1;

                        _map.SetBlock(index, block);
                        var (x, y) = _map.XYOf(index)!.Value;
                        _tempSln.Placements.Add(new Placement
                        {
                            X = x,
                            Y = y,
                            Block = block,
                        });

                        if (Solve(index + 1))
                        {
                            return true;
                        }

                        _tempSln.Placements.RemoveAt(_tempSln.Placements.Count - 1);

                        _blockNums[type] += 1;
                        _remainBlocks += 1;
                    }
                }

                return Solve(index + 1);
            }
        }

        public static List<Solution> Solve(SolveOptions options)
        {
            var totalBlocks = options.TotalBlockNums;
            if (totalBlocks == 0)
                return new List<Solution>();

            var map = options.MakeMap();
            var notWalls = map.NumNotWall;

            var solve = new SolveDfs(options);
            var slns = solve.Solve();
            if (slns.Count == 0)
                return slns;

            var span = CollectionsMarshal.AsSpan(slns);

            for (int i = 0; i < span.Length; i++)
            {
                int numCells = 0;
                int numBlk8 = 0;

                foreach (var sln in span[i].Placements)
                {
                    numCells += sln.Block.NumCells;
                    if (sln.Block.Type == 8)
                    {
                        numBlk8 += 1;
                    }
                }

                // 基础分
                int score = (int)Math.Ceiling(numCells / (float)notWalls * 90.0);

                // 完全研析额外 50 分
                if (numCells == notWalls)
                {
                    score += 50;
                }

                // 8 号额外加 10 分
                score += numBlk8 * 10;
                span[i].Score = score;
            }

            slns.Sort((a, b) => b.Score.CompareTo(a.Score));

            // 只保留最高分
            if (options.KeepTopOnly)
            {
                int topScore = slns.First().Score;
                var second = slns.FindIndex(s => s.Score != topScore);
                if (second != -1)
                {
                    slns.RemoveRange(second, slns.Count - second);
                }
            }
            
            return slns;
        }
    }
}
