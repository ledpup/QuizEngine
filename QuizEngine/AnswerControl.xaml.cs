using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace QuizEngine
{
    public sealed partial class MyUserControl1 : UserControl
    {
        public MyUserControl1(QuizQuestion quizQuestion)
        {
            this.InitializeComponent();

            if (quizQuestion == null)
                throw new NullReferenceException("No quiz question has been assigned.");

            txtTitle.Text = string.Format("{0}.", quizQuestion.QuestionNumber);
            txtDescription.Text = quizQuestion.Question;
            txtExplanation.Text = quizQuestion.FullExplanation;

            if (!string.IsNullOrEmpty(quizQuestion.Image))
                imgQuestionImage.Source = new BitmapImage(new Uri("ms-appx:///" + quizQuestion.ImageFullPath));

            var answerCorrect = quizQuestion.SelectedAnswer == quizQuestion.CorrectAnswer;

            if (quizQuestion.SelectedAnswer == null)
            {
                YourTextAnswer.Text = "[Not answered]";
                YourTextAnswer.Foreground = (SolidColorBrush) Application.Current.Resources["RedBrush"];
                YourTextAnswer.SetValue(Grid.ColumnSpanProperty, 2);
            }
            else
            {
                var answerColour = answerCorrect
                                       ? (SolidColorBrush)Application.Current.Resources["GreenBrush"]
                                       : (SolidColorBrush)Application.Current.Resources["RedBrush"];
                BuildAnswer(YourTextAnswer, YourImageAnswer, quizQuestion.SelectedAnswer, answerColour);
            }
            
            
            BuildAnswer(CorrectTextAnswer, CorrectImageAnswer, quizQuestion.CorrectAnswer, (SolidColorBrush) Application.Current.Resources["GreenBrush"] );

            //var image = answerCorrect ? "Tick.png" : "Cross.png";

            //imgCorrect.Source = new BitmapImage(new Uri("ms-appx:///Assets/" + image));
        }

        private void BuildAnswer(TextBlock textAnswer, Image imageAnswer, Answer answer, SolidColorBrush foreground)
        {
            if (!string.IsNullOrEmpty(answer.Text))
            {
                textAnswer.Text = answer.Text;
                if (foreground != null)
                {
                    textAnswer.Foreground = foreground;
                }
            }

            if (!string.IsNullOrEmpty(answer.Image))
            {
                var uri = new Uri("ms-appx:///Assets/Quizzes/" + MainPage.Quiz + "/" + answer.Image);
                imageAnswer.Source = new BitmapImage(uri);
            }
            else
            {
                textAnswer.SetValue(Grid.ColumnSpanProperty, 2);
            }
        }
    }
}
