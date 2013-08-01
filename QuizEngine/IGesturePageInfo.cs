using System;

namespace QuizEngine.Controls
{
    public interface IGesturePageInfo
    {
        String Id { get; }
        String Title { get; }
        String Description { get; }
        string QuestionImage { get; }
        GesturePageBase PlayArea { get; }
        void ResetQuestionAnswerIcon();
    }
}
