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
            charactersWithProbability = dataProvider.GetCharacters()
                .Select(x => new CharacterWithProbability
                {
                    Character = x,
                    Probability = x.Count / (double) gameCounter
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
                    RecalculateCharacters(userAnswer);
                    RecalculateEnd(characterToSuppose);
                    EndGame();
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
                    Count = 0,
                    Id = i,
                    Probability = 0.1d
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
                Probability = 0.1
            });
        }

        public void Save()
        {
            dataProvider.Save(charactersWithProbability.Select(x => x.Character));
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
            var question = bestCharacter.Questions
                .OrderByDescending(x => x.Probability)
                .Select(x => x.Id)
                .Except(history.Select(x => x.QuestionId))
                .First();
            return question;
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
        private int gameCounter = 1;

        private List<HistoryEntry> history { get; set; }
        private List<CharacterWithProbability> charactersWithProbability;
        private readonly IDataProvider dataProvider;
        private string[] questions;
    }
}