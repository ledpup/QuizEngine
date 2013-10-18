﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace QuizEngine
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewQuizAttemptPage : Page
    {
        public NewQuizAttemptPage()
        {
            NewMethod();


            

            

            
        }

        private async void NewMethod()
        {
            _quizConfig = new QuizConfig();

            await ReadQuizData();

            this.InitializeComponent();

            ConfigureQuestions(_quizConfig);

            Easy.IsChecked = true;
            Medium.IsChecked = true;
            Hard.IsChecked = true;
            Easy_Click(Easy, null);
            Medium_Click(Medium, null);
            Hard_Click(Hard, null);

            //NumberOfQuestions.Maximum = _completeQuizQuestions.Count;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //NewMethod();



            //ConfigureQuestions(quizConfig);
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

            _completeQuizQuestions.Reverse();
        }

        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            _quizConfig.NumberOfQuestions = (int)NumberOfQuestions.Value;

            _quizQuestions.Shuffle();
            _quizQuestions = _quizQuestions.Take(_quizConfig.NumberOfQuestions).ToList();

            for (var i = 0; i < _quizQuestions.Count; i++)
            {
                _quizQuestions[i].QuestionNumber = i + 1;

                for (var j = 0; j < _quizQuestions[i].Answers.Count; j++)
                {
                    _quizQuestions[i].Answers[j].Id = j;
                }
            }

            Frame.Navigate(typeof(MainPage), _quizQuestions);
        }

        private void SetDifficulty(CheckBox button, string difficulty)
        {
            //button.Background = new SolidColorBrush(new Color { A = 125 });

            //button.IsChecked = !button.IsChecked;

            if (button.IsChecked == true)
            {
                _quizConfig.Difficulties.Add(difficulty); 
            }
            else
            {
                _quizConfig.Difficulties.Remove(difficulty);
                //button.Background = (SolidColorBrush)Application.Current.Resources["GreenBrush"];
            }
            ConfigureQuestions(_quizConfig);
        }

        private void ConfigureQuestions(QuizConfig quizConfig)
        {
            _quizQuestions = _completeQuizQuestions.Where(x => quizConfig.Difficulties.Contains(x.Difficulty)).ToList();
            NumberOfQuestions.Maximum = _quizQuestions.Count;
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
    }
}
