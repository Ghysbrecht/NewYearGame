using NewYearGame.Logic;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NewYearGame
{
    public partial class MainWindow : Window
    {
        private SimpleLogger _log;
        private GameBrain _gameBrain;
        private ScoreBoard _scoreBoard;
        private Random _random;

        private TextBlock[] _currentScoreTextBlocks;
        private TextBlock[] _completeScoreTextBlocks;

        public MainWindow()
        {
            InitializeComponent();
            _log = new SimpleLogger();
            _gameBrain = new GameBrain(_log);

            if (_gameBrain.GetGamesCount() == 0)
            {
                MessageBox.Show("No games found! Please check the settings JSON", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
            InitializeScoreBlocks(DefaultValues.TEAMNAMES);
            _scoreBoard = new ScoreBoard(_log);
            _random = new Random();
            UpdateMainWindow();
        }

        #region Main ClickHandlers
        private void OpenResponsesPageButton_Click(object sender, RoutedEventArgs e)
        {
            ResponsesWindow window = new ResponsesWindow(_log);
            window.Show();
        }

        private void OpenScoreBoardButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _scoreBoard.Show();
            }
            catch(Exception exc)
            {
                AddToScreenLog("Exception when opening scoreboard! Creating new one...");
                _scoreBoard = new ScoreBoard(_log);
            }
            
            UpdateScoreBoard();
        }
        private void NextRoundButton_Click(object sender, RoutedEventArgs e)
        {
            _gameBrain.NextRound();
            UpdateAllWindows();
        }

        private void ShowAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            _scoreBoard.SetAnswer(_gameBrain.GetCurrentRound().Answer);
        }
        #endregion

        #region Update Methods
        private void UpdateAllWindows()
        {
            UpdateScoreBoard();
            UpdateMainWindow();
        }
        private void UpdateMainWindow()
        {
            CurrentGameTextBox.Text = $"{_gameBrain.GetCurrentGameInt() + 1}/{_gameBrain.GetGamesCount()}";
            CurrentRoundTextBox.Text = $"{_gameBrain.GetCurrentRoundInt() + 1}/{_gameBrain.GetRoundsCount()}";
            PlayStatusRectangle.Fill = IsCurrentRoundSongType() ? Brushes.DarkGreen : Brushes.DarkRed;

            RoundTitleTextBlock.Text = _gameBrain.GetCurrentRound().Title;
            RoundObjectiveTextBlock.Text = _gameBrain.GetCurrentRound().Objective;
            RoundAnswerTextBlock.Text = _gameBrain.GetCurrentRound().Answer;
            RoundTypeTextBlock.Text = _gameBrain.GetCurrentRound().Type;
            RoundMaxScoreTextBlock.Text = $"{_gameBrain.GetCurrentRound().MaxScore}";

            int[] currentScores = _gameBrain.GetCurrentScores();
            int[] completeScores = _gameBrain.GetCompleteScores();
            for (int i = 0; i < _gameBrain.GetCurrentScores().Count(); i++)
            {
                _currentScoreTextBlocks[i].Text = $"{currentScores[i]}";
                _completeScoreTextBlocks[i].Text = $"{completeScores[i]}";
            }
        }

        private void UpdateScoreBoard()
        {
            _scoreBoard.SetCurrentRound(_gameBrain.GetCurrentRound());
            _scoreBoard.SetGameText(_gameBrain.GetCurrentGameInt(), _gameBrain.GetGamesCount());
            _scoreBoard.SetRoundText(_gameBrain.GetCurrentRoundInt(), _gameBrain.GetRoundsCount());
            _scoreBoard.SetScores(_gameBrain.GetCurrentScores());
        }
        #endregion

        #region Game Control
        private void DecrementGameButton_Click(object sender, RoutedEventArgs e)
        {
            _gameBrain.PreviousGame();
            UpdateAllWindows();
        }

        private void IncrementGameButton_Click(object sender, RoutedEventArgs e)
        {
            _gameBrain.NextGame();
            UpdateAllWindows();
        }

        private void GameSetTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox txtBox)
            {
                _gameBrain.SetGame(ToInt(txtBox.Text) - 1);
                txtBox.Text = "";
                UpdateAllWindows();
            }
        }
        #endregion

        #region Round Control
        private void DecrementRoundButton_Click(object sender, RoutedEventArgs e)
        {
            _gameBrain.PreviousRound();
            UpdateAllWindows();
        }

        private void RoundSetTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox txtBox)
            {
                _gameBrain.SetRound(ToInt(txtBox.Text) - 1);
                txtBox.Text = "";
                UpdateAllWindows();
            }
        }

        private void IncrementRoundButton_Click(object sender, RoutedEventArgs e)
        {
            _gameBrain.NextRound();
            UpdateAllWindows();
        }
        #endregion

        #region Helper Methods
        private int ToInt(string number)
        {
            int j = 0;
            Int32.TryParse(number, out j);
            return j;
        }

        private void AddToScreenLog(string log)
        {
            string totalString = DateTime.Now.ToShortTimeString() + ": " + log + "\n";
            LogTextBox.Text = totalString + LogTextBox.Text;
        }
        #endregion

        #region Play Song Methods
        private void PlaySoundButton_Click(object sender, RoutedEventArgs e)
        {
            if(IsCurrentRoundSongType())
            {
                PlaySound(_gameBrain.GetCurrentRound().FilePath);
            }
        }

        private bool IsCurrentRoundSongType()
        {
            return _gameBrain.GetCurrentRound().Type == "Song";
        }

        private void PlaySound(string path)
        {
            _log.Debug("Playing sound > " + path);
            try
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(path);
                player.Play();
                PlayStatusRectangle.Fill = Brushes.LightGreen;
            }
            catch(Exception e)
            {
                _log.Debug("Playing song failed! > " + e.ToString());
                PlayStatusRectangle.Fill = Brushes.Yellow;
            }
            
        }
        #endregion

        #region Scores Methods
        // Initializes the score blocks per team
        private void InitializeScoreBlocks(string[] teams)
        {
            _currentScoreTextBlocks = new TextBlock[teams.Count()];
            _completeScoreTextBlocks = new TextBlock[teams.Count()];

            for (int i=0; i < teams.Count(); i++)
            {
                Grid grid = CreateGridBlockForTeam(teams[i], i);
                ScoresStackPanel.Children.Add(grid);
            }
        }

        // Creates the main grid block for a team
        private Grid CreateGridBlockForTeam(string team, int teamNumber)
        {
            // Main Grid
            Grid grid = new Grid()
            {
                Background = Brushes.LightGray,
                Margin = new Thickness(5, 0, 0, 0),
                Height = 140,
                Width = 100,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            // Team name header TextBlock
            TextBlock textBlock = new TextBlock()
            {
                Name = team + "_TeamNameTextBlock",
                Text = team,
                Height = 30, 
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Top,
                Padding = new Thickness(5),
                Width = 100,
                FontSize = 14,
                TextAlignment = TextAlignment.Center,
                Background = Brushes.DarkGray
            };
            grid.Children.Add(textBlock);

            // Current score TextBlock
            TextBlock currentScoreTextBlock = new TextBlock()
            {
                Name = team + "_CurrentScoreTextBlock",
                Height = 40,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 30,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 40)
            };
            _currentScoreTextBlocks[teamNumber] = currentScoreTextBlock;
            grid.Children.Add(currentScoreTextBlock);

            // Complete score TextBlock
            TextBlock completeScoreTextBlock = new TextBlock()
            {
                Name = team + "_CompleteScoreTextBlock",
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 15,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };
            _completeScoreTextBlocks[teamNumber] = completeScoreTextBlock;
            grid.Children.Add(completeScoreTextBlock);

            grid.Children.Add(CreateScoreAdjustmentStackPanelForTeam(team));

            return grid;
        }
        
        // Creates a stackpanel used to edit the score per team
        private StackPanel CreateScoreAdjustmentStackPanelForTeam(string teamName)
        {
            StackPanel stackPanel = new StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, 5)
            };

            Button decrementButton = new Button()
            {
                Name = teamName + "_decrementButton",
                Content = "-",
                Width = 30,
                Height = 30
            };
            decrementButton.Click += DecrementScoreButton_Click;
            stackPanel.Children.Add(decrementButton);

            TextBox txtBox = new TextBox()
            {
                Name = teamName + "_scoreAddTextBox",
                Width = 30,
                Height = 30,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            txtBox.KeyUp += ScoreAddTextBox_KeyUp;
            stackPanel.Children.Add(txtBox);

            Button incrementButton = new Button()
            {
                Name = teamName + "_incrementButton",
                Content = "+",
                Width = 30,
                Height = 30
            };
            incrementButton.Click += IncrementScoreButton_Click;
            stackPanel.Children.Add(incrementButton);

            return stackPanel;
        }

        // Decrement the score of the team when the - button is clicked
        private void DecrementScoreButton_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn)
            {
                string teamName = btn.Name.Split('_')[0];
                int teamNumber = DefaultValues.TEAMNAMES.ToList().IndexOf(teamName);
                _gameBrain.DecrementScore(teamNumber);
                UpdateAllWindows();
                AddToScreenLog("DECREMENTED score for " + teamName);
            }
        }

        // Add the typed score to the team
        private void ScoreAddTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && sender is TextBox txtbox)
            {
                string teamName = txtbox.Name.Split('_')[0];
                int teamNumber = DefaultValues.TEAMNAMES.ToList().IndexOf(teamName);
                int score = ToInt(txtbox.Text);
                _gameBrain.AddScore(score, teamNumber);
                txtbox.Text = "";
                UpdateAllWindows();
                AddToScreenLog("ADDED " + score + " points for " + teamName);
            }
        }

        // Increment the score a team when the + button is clicked
        private void IncrementScoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string teamName = btn.Name.Split('_')[0];
                int teamNumber = DefaultValues.TEAMNAMES.ToList().IndexOf(teamName);
                _gameBrain.IncrementScore(teamNumber);
                UpdateAllWindows();
                AddToScreenLog("INCREMENTED score for " + teamName);
            }
        }
        #endregion

        // Generate a letter and show it on the scoreboard
        private void OpenLetterGeneratorButton_Click(object sender, RoutedEventArgs e)
        {
            char letter = (char)('A' + _random.Next(0, 26));
            _scoreBoard.SetGeneratedLetter("" + letter);
        }

        // Update the scoreboard to the default view (Used after generating a letter)
        private void UpdateScoreBoardButton_Click(object sender, RoutedEventArgs e)
        {
            _scoreBoard.HideGeneratedLetter();
        }
    }
}