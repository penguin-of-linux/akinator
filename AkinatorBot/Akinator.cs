using System;
using System.Collections.Generic;
using System.Linq;
using AkinatorBot.DataProvider;

namespace AkinatorBot
{
    public class Akinator : IAkinator
    {
        public const int QuestionNumber = 20;
        public const int QuestionCountToSuppose = 15;

        public Akinator(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            _gameCounter = dataProvider.GetGameCount();
            _charactersWithProbability = dataProvider.GetCharacters()
                .Select(x => new CharacterWithProbability
                {
                    Character = x,
                    Probability = -1
                })
                .ToList();
            _history = new List<HistoryEntry>();
            _questions = dataProvider.GetQuestions();
            _supposedCharacters = new HashSet<CharacterEntry>();
        }


        public AkinatorAnswer Start()
        {
            _supposedCharacters.Clear();
            _started = true;
            _characterToSuppose = null;
            _nextQuestionId = 0;
            foreach (var character in _charactersWithProbability)
                character.Probability = Math.Max(character.Character.Count / (double) _gameCounter, 0.1);
            return NextQuestionInternal();
        }

        public AkinatorAnswer NextQuestion(UserAnswer userAnswer)
        {
            if (!_started)
                return null;
            _history.Add(new HistoryEntry
            {
                QuestionId = _nextQuestionId,
                UserAnswer = userAnswer
            });

            if (_characterToSuppose != null)
            {
                if (userAnswer == UserAnswer.Yes)
                {
                    EndGame();
                    return null;
                }

                _supposedCharacters.Add(_characterToSuppose);
                _characterToSuppose = null;
            }

            RecalculateCharacters(userAnswer);
            var nextQuestion = NextQuestionInternal();

            if (nextQuestion.AkinatorAnswerType == AkinatorAnswerType.Answer)
                _characterToSuppose = nextQuestion.CharacterToSuppose;
            if (nextQuestion.AkinatorAnswerType == AkinatorAnswerType.Surrender)
                EndGame();

            return nextQuestion;
        }

        public AkinatorAnswer Suppose()
        {
            if (!_started)
                return null;
            var bestCharacter = GetTheBestCharacter();
            if (bestCharacter == null)
            {
                return new AkinatorAnswer
                {
                    AkinatorAnswerType = AkinatorAnswerType.Surrender,
                    Message = "I give up("
                };
            }
            return SupposeInternal(bestCharacter);
        }

        public void AddCharacter(string name)
        {
            var characterQuestions = new List<CharacterQuestion>();
            for (var i = 1; i <= _questions.Length; i++)
                characterQuestions.Add(new CharacterQuestion
                {
                    Count = 1,
                    Id = i,
                    Probability = 0.5d
                });

            AddCharacter(new CharacterEntry
            {
                Name = name,
                Count = 0,
                Questions = characterQuestions.ToArray()
            });
        }

        public void AddCharacter(CharacterEntry character)
        {
            _charactersWithProbability.Add(new CharacterWithProbability
            {
                Character = character,
                Probability = 0.5
            });
        }

        public void Save()
        {
            _dataProvider.Save(_charactersWithProbability.Select(x => x.Character));
            _dataProvider.SaveGameCount(_gameCounter);
        }

        private AkinatorAnswer SupposeInternal(CharacterEntry character)
        {
            _characterToSuppose = character;
            return new AkinatorAnswer
            {
                AkinatorAnswerType = AkinatorAnswerType.Answer,
                Message = "Предпологаю, что это " + character.Name,
                CharacterToSuppose = character
            };
        }

        private void EndGame()
        {
            _started = false;
            RecalculateEnd(_characterToSuppose);
            _gameCounter++;
            Save();
        }

        private AkinatorAnswer NextQuestionInternal()
        {
            var bestCharacter = GetTheBestCharacter();
            if (bestCharacter == null)
            {
                return new AkinatorAnswer
                {
                    AkinatorAnswerType = AkinatorAnswerType.Surrender,
                    Message = "I give up("
                };
            }
            var bestQuestion = GetBestQuestion(bestCharacter);
            _nextQuestionId = bestQuestion;

            _questionCounter++;

            if (_questionCounter % QuestionCountToSuppose == QuestionCountToSuppose - 1)
                return SupposeInternal(bestCharacter);

            return new AkinatorAnswer
            {
                AkinatorAnswerType = AkinatorAnswerType.Question,
                Message = _questions[bestQuestion - 1] + "?"
            };
        }

        private int GetBestQuestion(CharacterEntry bestCharacter)
        {
            var questions = bestCharacter.Questions
                .Where(x => _history.All(y => y.QuestionId != x.Id))
                .ToArray();
            var maxP = questions.Max(x => x.Probability);
            var ids = questions.Where(x => x.Probability >= maxP)
                .Select(x => x.Id).ToArray();

            if (ids.Length == 1)
                return ids[0];
            return ids[_random.Next(ids.Length - 1)];
        }

        private void RecalculateCharacters(UserAnswer userAnswer)
        {
            foreach (var character in _charactersWithProbability)
            {
                var p = character.Probability;
                var q = character.Character.Questions.First(x => x.Id == _nextQuestionId).Probability;
                switch (userAnswer)
                {
                    case UserAnswer.Yes:
                        p *= q;
                        break;
                    case UserAnswer.No:
                        p *= 1 - q;
                        break;
                    case UserAnswer.DontKnow:
                        p *= 0.5;
                        break;
                }

                character.Probability = p;
            }
        }

        private void RecalculateEnd(CharacterEntry character)
        {
            if (character == null)
                return;

            foreach (var question in character.Questions)
            {
                var questionFromHistory = _history.FirstOrDefault(x => x.QuestionId == question.Id);
                var usedAndYes = questionFromHistory != null && questionFromHistory.UserAnswer == UserAnswer.Yes;
                question.Probability =
                    (question.Count * question.Probability + (usedAndYes ? 1 : 0)) / (question.Count + 1);
                if (usedAndYes) question.Count++;
            }

            character.Count++;
        }

        private CharacterEntry GetTheBestCharacter()
        {
            return _charactersWithProbability
                .Where(x => !_supposedCharacters.Contains(x.Character))
                .OrderByDescending(x => x.Probability)
                .FirstOrDefault()?.Character;
        }

        private List<CharacterWithProbability> _charactersWithProbability;
        private List<HistoryEntry> _history;
        private CharacterEntry _characterToSuppose;
        private bool _started;
        private int _gameCounter;
        private int _nextQuestionId;
        private int _questionCounter;
        private HashSet<CharacterEntry> _supposedCharacters;

        private readonly string[] _questions;
        private readonly IDataProvider _dataProvider;
        private readonly Random _random = new Random(Guid.NewGuid().GetHashCode());
    }
}