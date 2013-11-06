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

            if (quizQuestion.SelectedAnswer == null)
            {
                YourAnswer.Content = new TextBlock
                    {
                        Text = "[Not answered]",
                        Foreground = (SolidColorBrush)Application.Current.Resources["RedBrush"],
                        Style = Application.Current.Resources["AppDescriptionTextStyle"] as Style
                    };
            }
            else if (quizQuestion.SelectedAnswer != quizQuestion.CorrectAnswer)
            {
                YourAnswer.Content = BuildAnswer(quizQuestion.SelectedAnswer, (SolidColorBrush) Application.Current.Resources["RedBrush"]);
            }
            CorrectAnswer.Content = BuildAnswer(quizQuestion.CorrectAnswer, (SolidColorBrush) Application.Current.Resources["GreenBrush"]);
        }

        private object BuildAnswer(Answer answer, SolidColorBrush foreground)
        {
            var stackPanel = new StackPanel {Orientation = Orientation.Horizontal};

            if (!string.IsNullOrEmpty(answer.Image))
            {
                var uri = new Uri("ms-appx:///Assets/Quizzes/" + MainPage.Quiz + "/" + answer.Image);
                var image = new Image
                    {
                        Source = new BitmapImage(uri),
                        Stretch = Stretch.Uniform,
                        MaxHeight = 100,
                    };

                stackPanel.Children.Add(image);
            }

            if (!string.IsNullOrEmpty(answer.Text))
            {
                var answerText = new TextBlock
                    {
                        Text = answer.Text,
                        Style = Application.Current.Resources["AppDescriptionTextStyle"] as Style
                    };
                if (foreground != null)
                {
                    answerText.Foreground = foreground;
                }

                stackPanel.Children.Add(answerText);
            }

            return stackPanel;
        }
    }
}
