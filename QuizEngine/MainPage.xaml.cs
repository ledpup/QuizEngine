﻿using QuizEngine.Common;
using QuizEngine.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace QuizEngine
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : LayoutAwarePage
    {
        public static MainPage Current;

        //public const string Quiz = "Human Anatomy and Physiology";
        //public const string Quiz = "Biochemistry";
        public const string Quiz = "Spanish Civil War";

        // Data source for the semantic zoom
        private List<IGesturePageInfo> _pages;

        // General links
        //private Dictionary<string, Uri> _links;
        //private Button _linksButton;

        QuizAttempt _quizAttempt;
        readonly Random _random = new Random();

        //private NewQuizAttemptPage.QuizConfig _quizConfig;

        public MainPage()//NewQuizAttemptPage.QuizConfig quizConfig)
        {
            this.InitializeComponent();

            Current = this;

            //Window.Current.SizeChanged += VisualStateChanged;

            //SnappedQuizName.Text = MainPage.QuizTitle;
            // Links button
            //this._links = new Dictionary<string, Uri>();
            //this._links["Doc: Touch Interaction Design"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/hh465415.aspx");
            //this._links["Doc: Guidelines for panning"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/hh465310.aspx");
            //this._links["API: Windows.UI.Input namespace"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/br212145.aspx");
            //this._links["API: GestureRecognizer class"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/br241937.aspx");
            //this._links["API: ManipulationStarted event"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.input.gesturerecognizer.manipulationstarted.aspx");
            //this._linksButton = GesturePageBase.CreateLinksAppBarButton(this._links);

        }

        private void NewMethod1()
        {
            var quizQuestions = _quizAttempt.QuizQuestions;

            PrepareQuestions(quizQuestions);

            _pages = new List<IGesturePageInfo>();
            foreach (var quizQuestion in quizQuestions)
            {
                var questionAndanswer = new QuestionAnswer {Question = quizQuestion};
                _pages.Add(new QuestionPage(_quizAttempt.PracticeMode, questionAndanswer).AppPageInfo);
            }
            _pages.Add(new FinishQuizPage(_quizAttempt, this).AppPageInfo);

            gesturesViewSource.Source = _pages;

            SelectBackgroundImage(BackImage, _random, "backgrounds");
            //SelectBackgroundImage(BackgroundImageSnappedOrFilledScreen, _random, "backgrounds - main");

            //OnSelectionChanged(_pages.First(), new SelectionChangedEventArgs(new List<object>( _pages.Where(x => x != _pages.First()).ToList()) , new List<object> { _pages.First() }));
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _quizAttempt = (QuizAttempt)e.Parameter;

            NewMethod1();

            if (!_quizAttempt.PracticeMode)
            {
                _quizAttempt.DispatcherTimer.Tick += DispatcherTimer_Tick;
            }

        }

        void DispatcherTimer_Tick(object sender, object e)
        {
            var quizTimeOut = _quizAttempt.UpdateTimeRemainingOnQuiz();
            
            TimeRemaining.Text =  _quizAttempt.QuizTimeRemaining.ToString();

            if (quizTimeOut)
            {
                _quizAttempt.EndQuiz();
                
                Frame.Navigate(typeof(ResultsPage), _quizAttempt);
            }
        }

        public static async Task<IReadOnlyList<StorageFile>> GetFiles(string folder)
        {
            StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets\\Quizzes\\" + Quiz + " " + folder);
            
            if (folder != null)
                return await storageFolder.GetFilesAsync();
            else
                return null;
        }

        public static async void SelectBackgroundImage(ImageBrush image, Random random, string folder)
        {
            var fileList = await GetFiles(folder);

            var backgroundIndex = random.Next(fileList.Count);

            var backgroundImage = new BitmapImage(new Uri(string.Format("ms-appx:///Assets/Quizzes/{0} {1}/{2}", Quiz, folder, fileList[backgroundIndex].Name)));

            image.ImageSource = backgroundImage;
        }

        //private BitmapImage _backgroundImage;
        //public BitmapImage BackgroundImage
        //{
        //    get
        //    {
        //        if (_backgroundImage == null)
        //            throw new Exception("Background image has not been set");

        //        return _backgroundImage;
        //    }
        //}

        private void PrepareQuestions(IEnumerable<QuizQuestion> quizQuestions)
        {
            foreach (var question in quizQuestions)
            {
                question.Answers.Shuffle();

                if (question.MaxNumberOfAnswers > 0)
                {
                    if (question.MaxNumberOfAnswers >= question.Answers.Count)
                        throw new Exception("Max number of answers must be less than the number of answers.");

                    question.Answers.RemoveRange(question.MaxNumberOfAnswers,
                                                 question.Answers.Count - question.MaxNumberOfAnswers);
                }

                var lastAnswer = question.Answers.SingleOrDefault(x => x.IsLast);
                if (lastAnswer != null)
                {
                    var lastAnswerCurrentIndex = question.Answers.IndexOf(lastAnswer);
                    question.Answers[lastAnswerCurrentIndex] = question.Answers.Last();
                    question.Answers[question.Answers.Count - 1] = lastAnswer;
                }

                if (question.DynamicAnswer || question.KeyValue)
                {
                    var selectedAnswer = _random.Next(question.Answers.Count);
                    question.Answers[selectedAnswer].Score = question.Score;
                    if (question.KeyValue)
                    {
                        if (_random.Next(2) == 0 || String.IsNullOrEmpty(question.ValueQuestion))
                        {
                            question.Key = true;
                            question.Question = question.KeyQuestion;
                            question.Question = question.Question.Replace("*answer text*", question.Answers[selectedAnswer].Key);
                        }
                        else
                        {
                            question.Question = question.ValueQuestion;
                            question.Question = question.Question.Replace("*answer text*", question.Answers[selectedAnswer].Text);
                        }
                    }
                    else if (question.DynamicAnswer)
                    {
                        if (_random.Next(2) == 0 || String.IsNullOrEmpty(question.ValueQuestion))
                        {
                            question.Question = question.KeyQuestion;
                            question.Question = question.Question.Replace("*answer text*", question.Answers[selectedAnswer].Text);
                        }
                        else
                        {
                            question.Image = question.Answers[selectedAnswer].Image;
                            if (!string.IsNullOrEmpty(question.Answers[selectedAnswer].Key))
                            {
                                question.ImageText = question.Answers[selectedAnswer].Key;
                            }
                            question.Question = question.ValueQuestion;
                            foreach (var answer in question.Answers)
                            {
                                answer.Image = "";
                            }
                            question.TextAnswer = true;
                        }
                    }
                    {
                        question.Question = question.Question.Replace("*answer text*",
                                                                      question.Answers[selectedAnswer].Text);
                    }
                }
            }
            
        }


        public static string QuizTitle { get { return Quiz +" Quiz"; } }
    }
}
