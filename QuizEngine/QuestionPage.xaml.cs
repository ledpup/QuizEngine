using QuizEngine.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace QuizEngine
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QuestionPage : GesturePageBase
    {
        QuizQuestion _quizQuestion;
        //public const string Quiz = "Human Body Systems";
        //public const string Quiz = "Chemistry";
        public const string Quiz = "Spanish Civil War";


        public QuestionPage(QuestionAnswer quizQuestion) :
            base(quizQuestion.Question)
        {
            InitializeComponent();

            _quizQuestion = quizQuestion.Question;

            _appPageInfo = new GesturePageInfo(_quizQuestion, this);//uniqueId, title, description, similarTo, imagePath, this);

            Answers.ItemWidth = ItemWidth;

            DisplayAnswers();
        }

        double ItemWidth
        {
            get
            {
                return _quizQuestion.ImageAnswers ? 240 : 700;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private void DisplayAnswers()
        {
            foreach (var answer in _quizQuestion.Answers)
            {

                var stackPanel = new StackPanel();

                var button = new Button
                {
                    Tag = answer.Id,
                    Content = stackPanel,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(12),
                    //Margin = new Thickness(2),
                    Background = new SolidColorBrush(new Color {A = 125}),
                };

                if (!string.IsNullOrEmpty(answer.Image))
                {
                    var uri = new Uri("ms-appx:///Assets/Quizzes/" + Quiz + "/" + answer.Image);
                    var image = new Image
                    {
                        Source = new BitmapImage(uri),
                        Stretch = Stretch.Uniform,
                        MaxHeight = 270,
                    };

                    button.HorizontalContentAlignment = HorizontalAlignment.Center;

                    stackPanel.Children.Add(image);
                }

                if (!string.IsNullOrEmpty(answer.Text))
                {
                    var textBlock = new TextBlock
                                                {
                                                    Text = answer.Text, 
                                                    MinWidth = 400,
                                                    TextWrapping = TextWrapping.Wrap,
                                                };

                    if (_quizQuestion.KeyValue)
                    {
                        textBlock.Text = _quizQuestion.Key ? answer.Text : answer.Key;
                    }

                    if (_quizQuestion.DynamicAnswer)
                    {
                        textBlock.Text = _quizQuestion.TextAnswer ? answer.Text : "";
                    }

                    //if (!string.IsNullOrEmpty(textBlock.Text))
                    //{
                    //    button.VerticalContentAlignment = VerticalAlignment.Top;
                    //}

                    // Ensure that first letter is capitalised
                    //if (!string.IsNullOrEmpty(textBlock.Text))
                    //    textBlock.Text = char.ToUpper(textBlock.Text[0]) + textBlock.Text.Substring(1);

                    if (!string.IsNullOrEmpty(answer.Image))
                    {
                        textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                        textBlock.ClearValue(MinWidthProperty);
                    }

                    if (!string.IsNullOrEmpty(textBlock.Text))
                        stackPanel.Children.Add(textBlock);
                }

                button.Click += button_Click;
                Answers.Children.Add(button);
            }
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            var button = ((Button) sender);

            foreach (Button answer in Answers.Children.Where(x => x is Button))
                answer.Background = Background = new SolidColorBrush(new Color { A = 125 });

            var newAnswer = _quizQuestion.Answers.Single(x => x.Id == int.Parse(button.Tag.ToString()));

            if (_quizQuestion.SelectedAnswer == newAnswer)
            {
                _quizQuestion.SelectedAnswer = null;
                Explanation.Text = "";
                return;
            }

            _quizQuestion.SelectedAnswer = newAnswer;

            Explanation.Text = "" + _quizQuestion.Explanation;
            Explanation.Text += " " + _quizQuestion.SelectedAnswer.Explanation;
            Explanation.Text = Explanation.Text.Trim();

            if (_quizQuestion.SelectedAnswer.Score > 0)
            {
                button.Background = (SolidColorBrush) Application.Current.Resources["GreenBrush"];
            }
            else
            {
                button.Background = (SolidColorBrush) Application.Current.Resources["RedBrush"];
            }
        }
    }
}
