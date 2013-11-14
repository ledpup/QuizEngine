using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using QuizEngine.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace QuizEngine
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewQuizAttemptPage : LayoutAwarePage
    {
        public NewQuizAttemptPage()
        {
            this.InitializeComponent();

            NewMethod();

            Window.Current.SizeChanged += VisualStateChanged;
        }

        private async void NewMethod()
        {
            _quizConfig = new QuizConfig();

            await ReadQuizData();

            Easy.IsChecked = true;
            Medium.IsChecked = true;
            Hard.IsChecked = true;
            Easy_Click(Easy, null);
            Medium_Click(Medium, null);
            Hard_Click(Hard, null);
            Practice.IsChecked = true;
            Practice_Checked(null, null);
            //NumberOfQuestions.Maximum = _completeQuizQuestions.Count;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var f0 = Resources["Storyboard0"] as Storyboard;
            if (f0 != null) f0.Begin();
            var f1 = Resources["Storyboard1"] as Storyboard;
            if (f1 != null) f1.Begin();
            var f2 = Resources["Storyboard2"] as Storyboard;
            if (f2 != null) f2.Begin();
        }

        public class QuizConfig
        {
            public QuizConfig()
            {
                Difficulties = new List<string>();
            }

            public List<string> Difficulties;
            public int NumberOfQuestions;
        }

        private QuizConfig _quizConfig;

        List<QuizQuestion> _quizQuestions;
        private List<QuizQuestion> _completeQuizQuestions;
        private async Task ReadQuizData()
        {
            var quizText = await ExtensionMethods.ReadQuizFromFile(@"Assets\Quizzes\" + MainPage.Quiz + ".txt");

            _completeQuizQuestions = ExtensionMethods.Deserialize<List<QuizQuestion>>(quizText);

            //_completeQuizQuestions.Reverse();
        }

        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            _quizConfig.NumberOfQuestions = (int)NumberOfQuestions.Value;
            _quizQuestions.Shuffle();
            //_quizQuestions.Reverse();
            _quizQuestions = _quizQuestions.Take(_quizConfig.NumberOfQuestions).ToList();

            for (var i = 0; i < _quizQuestions.Count; i++)
            {
                _quizQuestions[i].QuestionNumber = i + 1;

                for (var j = 0; j < _quizQuestions[i].Answers.Count; j++)
                {
                    _quizQuestions[i].Answers[j].Id = j;
                }
            }

            Frame.Navigate(typeof(MainPage), new QuizAttempt(_quizQuestions, (bool)Practice.IsChecked));
        }

        private void SetDifficulty(CheckBox button, string difficulty)
        {
            if (button.IsChecked == true)
            {
                _quizConfig.Difficulties.Add(difficulty); 
            }
            else
            {
                _quizConfig.Difficulties.Remove(difficulty);
            }
            ConfigureQuestions(_quizConfig);
        }

        private void ConfigureQuestions(QuizConfig quizConfig)
        {
            _quizQuestions = _completeQuizQuestions.Where(x => quizConfig.Difficulties.Contains(x.Difficulty)).ToList();

            if (_quizQuestions.Count >= NumberOfQuestions.Minimum)
            {
                NumberOfQuestions.Maximum = _quizQuestions.Count;
                NumberOfQuestions.IsEnabled = true;
            }
            else
            {
                NumberOfQuestions.Maximum = NumberOfQuestions.Minimum;
                NumberOfQuestions.IsEnabled = false;
            }

            DifficultyWarning.Visibility = Visibility.Collapsed;
            StartQuiz.IsEnabled = true;
            if (_quizQuestions.Count == 0)
            {
                StartQuiz.IsEnabled = false;
                DifficultyWarning.Visibility = Visibility.Visible;
            }
        }

        private void Easy_Click(object sender, RoutedEventArgs e)
        {
            SetDifficulty((CheckBox)sender, "Easy");
        }

        private void Medium_Click(object sender, RoutedEventArgs e)
        {
            SetDifficulty((CheckBox)sender, "Medium");
        }

        private void Hard_Click(object sender, RoutedEventArgs e)
        {
            SetDifficulty((CheckBox)sender, "Hard");
        }

        private void Test_Checked(object sender, RoutedEventArgs e)
        {
            QuizTypeDescription.Text = "Test: Timed quiz. Answers revealed at the end of quiz. One minute per question.";
        }

        private void Practice_Checked(object sender, RoutedEventArgs e)
        {
            QuizTypeDescription.Text = "Practice: No time-limit. Answers are displayed as you go. Green highlight = correct answer. Red highlight = incorrect answer.";
        }

        private void VisualStateChanged(object sender, WindowSizeChangedEventArgs e)
        {
            var visualState = DetermineVisualState(ApplicationView.Value);

            if (visualState == "Snapped" || visualState == "Filled")
            {
                VisualStateManager.GoToState(this, "Filled", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "FullScreenLandscape", false);
            }
        }
    }
}
