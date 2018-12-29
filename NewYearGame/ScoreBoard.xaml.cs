using NewYearGame.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NewYearGame
{
    /// <summary>
    /// Interaction logic for ScoreBoard.xaml
    /// </summary>
    public partial class ScoreBoard : Window
    {
        private SimpleLogger _logger;
        private TextBlock[] _scoreBlocks;
        public ScoreBoard(SimpleLogger logger)
        {
            _logger = logger;
            InitializeComponent();

            // Scores
            AddTeamScores(DefaultValues.TEAMNAMES);

            // Start the clock
            DispatcherTimer timer = new DispatcherTimer(
                new TimeSpan(0, 0, 1),
                DispatcherPriority.Normal,
                delegate
                {
                    this.TimeTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");
                },
                this.Dispatcher
            ); 
        }

        #region Score Methods
        // Set the score TextBlocks to the given value
        public void SetScores(int[] scores)
        {
            int maxScore = scores.Max();
            for(int i=0; i<scores.Length; i++)
            {
                _scoreBlocks[i].Text = $"{scores[i]}";
                if (scores[i] == maxScore)
                {
                    _scoreBlocks[i].Foreground = Brushes.Red;
                }
                else
                {
                    _scoreBlocks[i].Foreground = Brushes.Black;
                }
            }
        }  

        // Add the score labels per team to the bottom bar
        private void AddTeamScores(string[] teams)
        {
            InitializeScoreBlocks(teams);
            List<Grid> grids = new List<Grid>();
            for(int i=0; i < teams.Length; i++)
            {
                Grid grid = new Grid()
                {
                    Width = 200,
                    Height = 150,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                TextBlock block = new TextBlock()
                {
                    Text = teams[i],
                    Height = 80,
                    FontSize = 35,
                    FontFamily = new FontFamily("Tw Cen MT"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top
                };

                grid.Children.Add(block);
                grid.Children.Add(_scoreBlocks[i]);
                grids.Add(grid);
            }

            grids.ForEach(grid => ScorePanel.Children.Add(grid));
        }

        // Initialize the global variable that stores the score TextBlocks
        private void InitializeScoreBlocks(string[] teams)
        {
            List<TextBlock> list = new List<TextBlock>();
            foreach(string team in teams)
            {
                TextBlock txt = new TextBlock()
                {
                    Text = "0",
                    Height = 100,
                    FontSize = 50,
                    FontFamily = new FontFamily("Tw Cen MT"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom
                };

                list.Add(txt);
            }
            _scoreBlocks = list.ToArray();
        }
        #endregion

        #region Round Methods
        public void SetCurrentRound(Round round)
        {
            RoundTitleTextBlock.Text = round.Title;
            RoundDescriptionTextBlock.Text = round.Objective;
            RoundScoreTextBlock.Text = $"+{round.MaxScore}";
            RoundAnswerTextBlock.Text = "";
        }

        public void SetAnswer(string answer)
        {
            RoundAnswerTextBlock.Text = answer;
        }

        public void SetRoundText(int currentRound, int maxRounds)
        {
            RoundTextBlock.Text = $"{currentRound + 1}/{maxRounds}";
        }

        public void SetGameText(int currentGame, int maxGames)
        {
            GameTextBlock.Text = $"{currentGame + 1}/{maxGames}";
        }
        #endregion

        #region Generate Letter Methods
        // Sets the letter and sets the visibility of that grid to visable
        public void SetGeneratedLetter(string letter)
        {
            DefaultRoundGrid.Visibility = Visibility.Collapsed;
            LetterGenGrid.Visibility = Visibility.Visible;
            GenerateLetterTextBlock.Text = letter;
        }

        public void HideGeneratedLetter()
        {
            DefaultRoundGrid.Visibility = Visibility.Visible;
            LetterGenGrid.Visibility = Visibility.Collapsed;
        }
        #endregion
    }
}
