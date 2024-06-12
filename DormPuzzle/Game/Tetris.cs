using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DormPuzzle.Models;

namespace DormPuzzle.Game.Tetris
{
    public readonly record struct BlockCellId(int I, int R);

    public readonly record struct BlockCells
    {
        public static readonly int MaxWidth = 4;
        public static readonly int MaxHeight = 4;

        public BlockCellId Id => new(Type - 1, Rot);

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
        public bool Has { get; }

        [InlineArray(3 * 3)]
        private struct CellsArray
        {
            public bool V;
        }

        private readonly CellsArray _cellsArray;

        private static readonly BlockCells[,] _blockArray = new BlockCells[11, 4];

        //private static readonly List<List<BlockCells>> _blocks;

        [UnscopedRef]
        public ReadOnlySpan<bool> Cells
        {
            get
            {
                ReadOnlySpan<bool> __ = _cellsArray;
                return __[..(Width * Height)];
            }
        }

        public static int NumTypes => 11;

        public static int? NumRots(int type)
        {
            if (!IsValidType(type)) return null;
            var i = 0;
            for (; i < 4; i++)
            {
                if (!_blockArray[type - 1, i].Has) break;
            }
            return i;
        }

        public static ref readonly BlockCells Get(BlockCellId id) => ref _blockArray[id.I, id.R];
        public static ref readonly BlockCells Get(int type, int rot) => ref _blockArray[type - 1, rot];

        public static bool IsValidType(int type) => 0 < type && type <= NumTypes;

        static BlockCells()
        {
            var bases = new BlockCells[]
            {
                new(1, 0, new byte[,]
                {
                    { 1, 1 },
                    { 1, 1 }
                }),
                new(2, 0, new byte[,]
                {
                    { 1, 1, 1, 1 }
                }),
                new(3, 0, new byte[,]
                {
                    { 1, 1, 0 },
                    { 0, 1, 1 }
                }),
                new(4, 0, new byte[,]
                {
                    { 0, 1, 1 },
                    { 1, 1, 0 },
                }),
                new(5, 0, new byte[,]
                {
                    { 1, 0, 0 },
                    { 1, 1, 1 },
                }),
                new(6, 0, new byte[,]
                {
                    { 0, 0, 1 },
                    { 1, 1, 1 },
                }),
                new(7, 0, new byte[,]
                {
                    { 0, 1, 0 },
                    { 1, 1, 1 },
                }),
                new(8, 0, new byte[,]
                {
                    { 0, 1, 0 },
                    { 1, 1, 1 },
                    { 0, 1, 0 },
                }),
                new(9, 0, new byte[,]
                {
                    { 1 }
                }),
                new(10, 0, new byte[,]
                {
                    { 1, 1 }
                }),
                new(11, 0, new byte[,]
                {
                    { 1, 0 },
                    { 1, 1 },
                }),
            };

            foreach (var blk in bases)
            {
                var set = new List<BlockCells>();
                _blockArray[blk.Id.I, blk.Id.R] = blk;
                set.Add(blk);

                // 旋转三次，只保留 Cells 不同的
                int rot = 1;
                for (var i = 1; i < 4; i++)
                {
                    var rotated = blk.RotateCW(rot);
                    if (!set.Exists(x => x.AreCellsSame(rotated)))
                    {
                        set.Add(rotated);
                        _blockArray[rotated.Id.I, rotated.Id.R] = rotated;
                        rot++;
                    }
                }
            }
        }

        private BlockCells(int type, int rot, byte[,] cells)
        {
            var height = cells.GetLength(0);
            var width = cells.GetLength(1);

            Debug.Assert(width <= MaxWidth);
            Debug.Assert(height <= MaxHeight);

            Type = type;
            Rot = rot;
            Width = width;
            Height = height;
            Has = true;

            var index = 0;
            var numCells = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    _cellsArray[index] = cells[y, x] == 1;
                    if (_cellsArray[index])
                        numCells++;
                    index++;
                }
            }
            NumCells = numCells;
        }

        private BlockCells(int type, int rot, int width, int height, CellsArray cellsArray)
        {
            Type = type;
            Rot = rot;
            Width = width;
            Height = height;
            _cellsArray = cellsArray;
            foreach (var cell in Cells)
            {
                if (cell) NumCells++;
            }
            Has = true;
        }

        private BlockCells RotateCW(int rot)
        {
            var width = Height;
            var height = Width;

            var cells = new CellsArray();
            var index = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    cells[index++] = Cells[(width - x - 1) * Width + y];
                }
            }

            return new BlockCells(Type, rot, width, height, cells);
        }

        /// <summary>
        /// 用来检查两个 Block 是否有完全一样的 Cells
        /// </summary>
        private bool AreCellsSame(in BlockCells other)
        {
            if (!(Width == other.Width && Height == other.Height)) return false;

            var a = Cells;
            var b = other.Cells;
            for (var i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }

            return true;
        }

        public bool GetCellUnchecked(int x, int y) => Cells[x + y * Width];

        public bool Equals(BlockCells other) => Type == other.Type && Rot == other.Rot;

        public override int GetHashCode() => HashCode.Combine(Type, Rot);
    }

    public readonly record struct Placement(int X, int Y, BlockCellId BlockId)
    {
        public readonly int X = X;
        public readonly int Y = Y;
        public readonly BlockCellId BlockId = BlockId;

        public BlockCells Block => BlockCells.Get(BlockId);
    }

    public struct Solution()
    {
        public List<Placement> Placements = [];
        public int Score = 0;

        public Solution Clone()
        {
            var res = new Solution
            {
                Placements = [.. Placements],
                Score = Score
            };

            return res;
        }
    }

    public class Map
    {
        public static readonly int EmptyCell = 0;
        public static readonly int WallCell = -1;

        public readonly int Width;
        public readonly int Height;
        private readonly sbyte[] _map;

        public int NumNotWall => _map.Count(static c => c != WallCell);

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            _map = new sbyte[Width * Height];
        }

        public static bool IsValidCell(int type) => type == EmptyCell || type == WallCell || BlockCells.IsValidType(type);

        private int IndexOfUnchecked(int x, int y) => x + y * Width;

        private int IndexOfChecked(int x, int y) => IndexOf(x, y) ?? throw new ArgumentException("Invalid cell index");

        public int? IndexOf(int x, int y) => 0 <= x && x < Width && 0 <= y && y < Height ? IndexOfUnchecked(x, y) : null;

        public (int, int)? XYOf(int index) => 0 <= index && index < Width * Height ? (index % Width, index / Width) : null;

        public void SetCell(int x, int y, int type) => SetCell(IndexOfChecked(x, y), type);

        public void SetCell(int index, int type) => _map[index] = IsValidCell(type) ? (sbyte)type : throw new ArgumentException("Invalid cell type");

        public int GetCell(int x, int y) => _map[IndexOfChecked(x, y)];

        public int GetCell(int index) => _map[index];

        public void Clear() => _map.AsSpan().Clear();

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
        public bool SetBlock(int x, int y, in BlockCells block, int? altnative = null, bool force = false) =>
            IndexOf(x, y) is { } index && SetBlock(index, block, altnative, force);

        /// <summary>
        /// 功能和同名重载一样
        /// </summary>
        public bool SetBlock(int index, in BlockCells block, int? altnative = null, bool force = false)
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
                        var src = block.GetCellUnchecked(w, h);
                        if (dst != EmptyCell && src)
                            return false;
                    }
                }
            }

            for (var h = 0; h < block.Height; h++)
            {
                for (var w = 0; w < block.Width; w++)
                {
                    if (block.GetCellUnchecked(w, h))
                    {
                        _map[index + w + h * Width] = (sbyte)cell;
                    }
                }
            }
            return true;
        }

        public bool CanSetBlock(int index, in BlockCells block)
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
                    var src = block.GetCellUnchecked(w, h);
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

        private readonly int TotalBlockNums => BlockNums.Aggregate(0, (acc, b) => acc + b);

        public SolveOptions(int width, int height, int[] blockNums)
        {
            Width = width;
            Height = height;
            BlockNums = new int[BlockCells.NumTypes];
            blockNums.CopyTo(BlockNums, 0);

            Walls = null;
            MaxSolutions = 50000;
            PreferLargerNumber = false;
            KeepTopOnly = false;
        }

        public readonly int[] MakeOrderedTypes()
        {
            var res = new int[BlockCells.NumTypes];
            for (var i = 0; i < res.Length; i++)
            {
                res[i] = i + 1;
            }

            if (PreferLargerNumber)
            {
                var keys = new int[BlockNums.Length];
                BlockNums.CopyTo(keys, 0);
                Array.Sort(keys, res, Comparer<int>.Create((a, b) => b.CompareTo(a)));
            }
            return res;
        }

        private readonly Map MakeMap()
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
            private readonly int[] _types;
            /// <summary>
            /// 各个类型 Block 的数量，[0] 不使用
            /// </summary>
            private readonly int[] _blockNums;
            private readonly List<Solution> _slns;
            private readonly Map _map;
            private Solution _tempSln;
            private int _remainBlocks;
            private readonly int _maxSolutions;

            public SolveDfs(SolveOptions options)
            {
                _types = options.MakeOrderedTypes();
                _blockNums = new int[BlockCells.NumTypes + 1];
                options.BlockNums.CopyTo(_blockNums, 1);
                _maxSolutions = options.MaxSolutions;
                _remainBlocks = options.TotalBlockNums;
                _map = options.MakeMap();
                _slns = [];
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

                    var numRots = BlockCells.NumRots(type);
                    for (int rot = 0; rot < numRots; rot++)
                    {
                        ref readonly var block = ref BlockCells.Get(type, rot);
                        Debug.Assert(block.Has, nameof(block) + " != null");
                        if (!_map.CanSetBlock(index, block))
                        {
                            continue;
                        }

                        _blockNums[type] -= 1;
                        _remainBlocks -= 1;

                        _map.SetBlock(index, block, force: true);
                        var (x, y) = _map.XYOf(index)!.Value;
                        _tempSln.Placements.Add(new Placement(x, y, block.Id));

                        if (Solve(index + 1))
                        {
                            return true;
                        }

                        _tempSln.Placements.RemoveAt(_tempSln.Placements.Count - 1);
                        _map.SetBlock(index, block, Map.EmptyCell, true);

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
                return [];

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
