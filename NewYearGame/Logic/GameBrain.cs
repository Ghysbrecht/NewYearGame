using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewYearGame.Logic
{
    public class GameBrain
    {
        private List<Game> _games { get; set; }
        private SimpleLogger _logger { get; set; }

        private int _currentGame { get; set; }
        private int _currentRound { get; set; }

        public GameBrain(SimpleLogger logger)
        {
            _logger = logger;
            _games = new List<Game>();

            // Set the current game and round to 0
            _currentGame = 0;
            _currentRound = 0;

            // Parse the JSON into games
            HandleJson(DefaultValues.JSON_PATH);
            _logger.Info("Games Found: " + _games.Count);

            // Set the score list
            InitializeScores(DefaultValues.TEAMNAMES.Count());
        }

        #region Get Game/Round Methods
        public Game GetCurrentGame()
        {
            if (GetGamesCount() == 0)
            {
                return null;
            }
            return _games.ElementAt(_currentGame);
        }

        public int GetCurrentGameInt()
        {
            return _currentGame;
        }

        public Round GetCurrentRound()
        {
            if (GetRoundsCount() == 0)
            {
                return null;
            }
            return _games.ElementAt(_currentGame)?.Rounds.ElementAt(_currentRound);
        }

        public int GetCurrentRoundInt()
        {
            return _currentRound;
        }

        public int GetGamesCount()
        {
            return _games.Count;
        }

        public int GetRoundsCount()
        {
            return GetCurrentGame() == null ? 0 : GetCurrentGame().GetAmmountOfRounds();
        }
        #endregion

        #region GameControl Methods
        // Go to the next game if possible
        public bool NextGame()
        {
            if ((_currentGame + 1) < GetGamesCount())
            {
                _currentGame++;
                _currentRound = 0;
            }
            return false;
        }

        // Return to the previous game if possible
        public bool PreviousGame()
        {
            if (_currentGame != 0)
            {
                _currentGame--;
                _currentRound = GetRoundsCount() - 1;
            }
            return false;
        }

        // Go to the next round, increment game if needed
        public void NextRound()
        {
            if ((_currentRound + 1) >= GetRoundsCount())
            {
                // Last round in game reached, go to next game
                NextGame();
            }
            else
            {
                _currentRound++;
            }
        }

        // Go to the previous round, decrement game if needed
        public void PreviousRound()
        {
            if (_currentRound == 0)
            {
                // Already on first round, go to the previous game
                PreviousGame();
            }
            else
            {
                _currentRound--;
            }
        }

        public void SetRound(int round)
        {
            if (round >= 0 && round < GetRoundsCount())
            {
                _currentRound = round;
            }
        }

        public void SetGame(int game)
        {
            if (game >= 0 && game < GetGamesCount())
            {
                _currentGame = game;
                _currentRound = 0;
            }
        }
        #endregion

        #region Score Methods
        public void InitializeScores(int teams)
        {
            foreach(Game game in _games)
            {
                int[] scores = new int[teams];
                for (int i = 0; i < teams; i++)
                {
                    scores[i] = 0;
                }
                game.Scores = scores;
            }
        }
        // Increments the score with the ammount of points for that round
        public void IncrementScore(int teamNumber)
        {
            if (teamNumber >= 0 && teamNumber < GetCurrentScores().Count())
            {
                GetCurrentScores()[teamNumber] = GetCurrentScores()[teamNumber] + GetCurrentRound().MaxScore;
            }
        }
        // Decrements the score with the ammount of points for that round
        public void DecrementScore(int teamNumber)
        {
            if (teamNumber >= 0 && teamNumber < GetCurrentScores().Count() && GetCurrentScores()[teamNumber] > 0)
            {
                int tempScore = GetCurrentScores()[teamNumber];
                tempScore -= GetCurrentRound().MaxScore;
                if(tempScore < 0)
                {
                    tempScore = 0;
                }
                GetCurrentScores()[teamNumber] = tempScore;
            }
        }

        public void AddScore(int score, int teamNumber)
        {
            if (teamNumber >= 0 && teamNumber < GetCurrentScores().Count() && (GetCurrentScores()[teamNumber] + score) >= 0)
            {
                GetCurrentScores()[teamNumber] = GetCurrentScores()[teamNumber] + score;
            }
        }

        public int[] GetCurrentScores()
        {
            return _games.ElementAt(_currentGame).Scores;
        }

        public int[] GetCompleteScores()
        {
            List<int> completeScores = new List<int>();
            foreach(var name in DefaultValues.TEAMNAMES)
            {
                completeScores.Add(0);
            }

            foreach(Game game in _games)
            {
                for(int i=0; i < game.Scores.Count(); i++)
                {
                    completeScores[i] += game.Scores[i];
                }
            }
            return completeScores.ToArray();
        }
        #endregion

        #region JSON
        public void HandleJson(string path)
        {
            _logger.Debug("Loading JSON from " + path);
            if (!System.IO.File.Exists(path))
            {
                _logger.Error("File does not exist!");
                return;
            }

            string json = System.IO.File.ReadAllText(path);
            if (json == String.Empty)
            {
                _logger.Error("File is empty!");
                return;
            }

            _logger.Debug("Complete JSON:\n" + json);

            try
            {
                _games = GamesFactory.createFromJson(json);
            }
            catch (Exception e)
            {
                _logger.Error("Exception thrown while parsing JSON > " + e.ToString());
            }
        }
        #endregion
    }
}
