using System.Collections.Generic;
using System.Linq;
using AkinatorBot.DataProvider;

namespace AkinatorBot
{
    public class Akinator : IAkinator
    {
        public const int QuestionNumber = 20;
        public Akinator(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
            charactersWithProbability = dataProvider.GetCharacters()
                .ToDictionary(x => x, x => x.Count / (double)gameCounter);
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
                    Message = "Suppose:",
                    CharacterToSuppose = bestCharacter
                };

            return new AkinatorAnswer
            {
                AkinatorAnswerType = AkinatorAnswerType.Question,
                Message = nextQuestionId.ToString()
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
            foreach (var character in charactersWithProbability.Keys)
            {
                var p = charactersWithProbability[character];
                var q = character.Questions.First(x => x.Id == nextQuestionId).Probability;
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

                charactersWithProbability[character] = p;
            }
        }

        private void RecalculateEnd(CharacterEntry character)
        {
            foreach (var question in character.Questions)
            {
                var used = history.Any(x => x.QuestionId == question.Id);
                question.Probability =
                    (question.Count * question.Probability + (used ? 1 : 0)) / (question.Count + 1);
                if (used) question.Count++;
            }

            character.Count++;
        }

        private CharacterEntry GetTheBestCharacter()
        {
            return charactersWithProbability.OrderByDescending(x => x.Value).First().Key;
        }

        private CharacterEntry characterToSuppose;
        private int questionCounter;
        private int nextQuestionId;
        private int gameCounter;

        private List<HistoryEntry> history { get; set; }
        private Dictionary<CharacterEntry, double> charactersWithProbability;
        private readonly IDataProvider dataProvider;
    }
}