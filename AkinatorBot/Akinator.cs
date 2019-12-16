using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AkinatorBot.DataProvider;
using Newtonsoft.Json;

namespace AkinatorBot
{
    public class Akinator : IAkinator
    {
        public const int QuestionNumber = 20;
        public Akinator(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
            gameCounter = dataProvider.GetGameCount();
            charactersWithProbability = dataProvider.GetCharacters()
                .Select(x => new CharacterWithProbability
                {
                    Character = x,
                    Probability = Math.Max(x.Count / (double) gameCounter, 0.1)
                })
                .ToList();
            history = new List<HistoryEntry>();
            questions = dataProvider.GetQuestions();
        }

        public AkinatorAnswer Start()
        {
            return NextQuestionInternal();
        }

        public AkinatorAnswer NextQuestion(UserAnswer userAnswer)
        {
            history.Add(new HistoryEntry
            {
                QuestionId = nextQuestionId,
                UserAnswer = userAnswer
            });

            if(characterToSuppose != null)
            {
                if (userAnswer == UserAnswer.Yes)
                {
                    //RecalculateCharacters(userAnswer);
                    RecalculateEnd(characterToSuppose);
                    gameCounter++;
                    Save();
                    return null;
                }
                else
                {
                    characterToSuppose = null;
                }
            }

            RecalculateCharacters(userAnswer);
            var nextQuestion = NextQuestionInternal();

            if (nextQuestion.AkinatorAnswerType == AkinatorAnswerType.Answer)
            {
                characterToSuppose = nextQuestion.CharacterToSuppose;
            }

            return nextQuestion;
        }

        public void AddCharacter(string name)
        {
            var characterQuestions = new List<CharacterQuestion>();
            for (var i = 1; i <= questions.Length; i++)
            {
                characterQuestions.Add(new CharacterQuestion
                {
                    Count = 1,
                    Id = i,
                    Probability = 0.5d
                });
            }

            AddCharacter(new CharacterEntry
            {
                Name = name,
                Count = 0,
                Questions = characterQuestions.ToArray()
            });
        }

        public void AddCharacter(CharacterEntry character)
        {
            charactersWithProbability.Add(new CharacterWithProbability
            {
                Character = character,
                Probability = 0.5
            });
        }

        public void Save()
        {
            dataProvider.Save(charactersWithProbability.Select(x => x.Character));
            dataProvider.SaveGameCount(gameCounter);
        }

        private void EndGame()
        {
            
        }

        private AkinatorAnswer NextQuestionInternal()
        {
            var bestCharacter = GetTheBestCharacter();
            var bestQuestion = GetBestQuestion(bestCharacter);
            nextQuestionId = bestQuestion;

            questionCounter++;

            if (questionCounter == QuestionNumber)
                return new AkinatorAnswer
                {
                    AkinatorAnswerType = AkinatorAnswerType.Answer,
                    Message = "Предпологаю, что это " + bestCharacter.Name,
                    CharacterToSuppose = bestCharacter
                };

            return new AkinatorAnswer
            {
                AkinatorAnswerType = AkinatorAnswerType.Question,
                Message = questions[bestQuestion] + "?"
            };
        }

        private int GetBestQuestion(CharacterEntry bestCharacter)
        {
            var questions = bestCharacter.Questions
                .Where(x => history.All(y => y.QuestionId != x.Id))
                .ToArray();
            var maxP = questions.Max(x => x.Probability);
            var ids = questions.Where(x => x.Probability >= maxP)
                .Select(x => x.Id).ToArray();

            if (ids.Length == 1)
                return ids[0];
            return ids[random.Next(ids.Length - 1)];
        }

        private void RecalculateCharacters(UserAnswer userAnswer)
        {
            foreach (var character in charactersWithProbability)
            {
                var p = character.Probability;
                var q = character.Character.Questions.First(x => x.Id == nextQuestionId).Probability;
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
            foreach (var question in character.Questions)
            {
                var questionFromHistory = history.FirstOrDefault(x => x.QuestionId == question.Id);
                var usedAndYes = questionFromHistory != null && questionFromHistory.UserAnswer == UserAnswer.Yes;
                question.Probability =
                    (question.Count * question.Probability + (usedAndYes ? 1 : 0)) / (question.Count + 1);
                if (usedAndYes) question.Count++;
            }

            character.Count++;
        }

        private CharacterEntry GetTheBestCharacter()
        {
            return charactersWithProbability.OrderByDescending(x => x.Probability).First().Character;
        }

        private CharacterEntry characterToSuppose;
        private int questionCounter;
        private int nextQuestionId;
        private int gameCounter;
        private Random random = new Random(Guid.NewGuid().GetHashCode());

        private List<HistoryEntry> history { get; set; }
        private List<CharacterWithProbability> charactersWithProbability;
        private readonly IDataProvider dataProvider;
        private string[] questions;
    }
}