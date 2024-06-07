using CommunityToolkit.Mvvm.ComponentModel;
using DormPuzzle.Game.Tetris;

namespace DormPuzzle.Models;

public partial class SolutionBind : ObservableObject
{
    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private Solution solution;

    [ObservableProperty]
    private int score;

    public SolutionBind(string name, Solution solution)
    {
        Name = name;
        Solution = solution;
        Score = solution.Score;
    }
}
