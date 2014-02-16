using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace QuizEngine
{
    public class QuestionAnswer
    {
        public QuizQuestion Question;
        public Answer Answer;
    }

    [DataContractAttribute]
    public class QuizQuestion
    {
        public int QuestionNumber;
        public string Title { get { return "Question " + QuestionNumber; } }
        [DataMemberAttribute]
        public string Category;
        [DataMemberAttribute]
        public string Difficulty;
        [DataMemberAttribute]
        public string Question;
        [DataMemberAttribute]
        public bool KeyValue;
        [DataMemberAttribute]
        public string KeyQuestion;
        [DataMemberAttribute]
        public string ValueQuestion;
        public bool Key;
        public bool TextAnswer;
        [DataMemberAttribute]
        public bool DynamicAnswer;
        [DataMemberAttribute]
        public int MaxNumberOfAnswers;
        public bool ImageAnswers { get { return Answers.Any(x => !string.IsNullOrEmpty(x.Image)); } }
        [DataMemberAttribute]
        public string Image;
        [DataMemberAttribute]
        public string ImageText;
        [DataMemberAttribute]
        public string ImageUrl;
        [DataMemberAttribute]
        public string Audio;
        [DataMemberAttribute]
        public string Copyright;
        [DataMemberAttribute]
        public string Explanation;
        public Answer SelectedAnswer;
        [DataMemberAttribute]
        public List<Answer> Answers;

        private Answer _correctAnswer;
        public Answer CorrectAnswer
        {
            get
            {
                if (_correctAnswer == null)
                    _correctAnswer = Answers.Single(x => x.Score > 0);
                return _correctAnswer;
            }
        }

        [DataMemberAttribute]
        public float Score
        {
            get
            {
                if (_score == 0) _score = 1;

                return _score;
            }
            set
            {
                _score = value;
            }
        }

        public string FullExplanation
        {
            get
            {
                var explanation = "" + Explanation;
                if (SelectedAnswer != null)
                    explanation += " " + SelectedAnswer.Explanation;

                return explanation.Trim();
            }

        }

        private float _score;


        public string ImageFullPath { get { return "Assets/Quizzes/" + MainPage.Quiz + "/" + Image; } }
    }

    [DataContractAttribute]
    public class Answer
    {
        public int Id;
        [DataMemberAttribute]
        public string Key;
        [DataMemberAttribute]
        public string Text;
        [DataMemberAttribute]
        public string Image;
        [DataMemberAttribute]
        public float Score;
        [DataMemberAttribute]
        public bool IsLast;
        [DataMemberAttribute]
        public string Explanation;
    }
}
