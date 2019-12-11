using AkinatorBot.DataProvider;

namespace AkinatorBot
{
    public class Akinator : IAkinator
    {
        public Akinator(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        public AkinatorAnswer Start()
        {
            return new AkinatorAnswer
            {
                AkinatorAnswerType = AkinatorAnswerType.Question,
                Message = "lalala"
            };
        }

        public AkinatorAnswer NextQuestion(UserAnswer userAnswer)
        {
            return new AkinatorAnswer
            {
                AkinatorAnswerType = AkinatorAnswerType.Question,
                Message = "lalala"
            }; ;
        }

        private readonly IDataProvider dataProvider;
    }
}