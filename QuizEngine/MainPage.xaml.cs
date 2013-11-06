﻿using QuizEngine.Common;
using QuizEngine.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
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
    public sealed partial class MainPage : LayoutAwarePage
    {
        public static MainPage Current;

        //public const string Quiz = "Human Body Systems";
        //public const string Quiz = "Chemistry";
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
            //await LoadQuiz();

            var quizQuestions = _quizAttempt.QuizQuestions;

            PrepareQuestions(quizQuestions);

            //var quizAttempt = new QuizAttempt();

            _pages = new List<IGesturePageInfo>();
            foreach (var quizQuestion in quizQuestions)
            {
                var questionAndanswer = new QuestionAnswer {Question = quizQuestion};
                //quizAttempt.Answers.Add(questionAndanswer);
                //quizAttempt.Answers.Add(new QuestionAnswer { Question = quizQuestion });
                _pages.Add(new QuestionPage(questionAndanswer).AppPageInfo);
            }
            _pages.Add(new FinishQuizPage(_quizAttempt, this).AppPageInfo);

            gesturesViewSource.Source = _pages;

            BackImage.ImageSource = BackgroundImage;
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

            DispatcherTimerSetup();
        }

        //private async Task LoadQuiz()
        //{

        //    _quizQuestions.Reverse();

        //    PrepareQuestions(_quizQuestions);
        //}
        DispatcherTimer _dispatcherTimer;
        int _timesToTick;
        private int _timesTicked;
        private DateTimeOffset _quizEnds;

        void DispatcherTimerSetup()
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            _timesToTick = _quizAttempt.QuizQuestions.Count;
            _quizAttempt.QuizStart = DateTimeOffset.Now;
            _dispatcherTimer.Start();
            _quizEnds = _quizAttempt.QuizStart.AddSeconds(_timesToTick);
        }

        void DispatcherTimer_Tick(object sender, object e)
        {
            _timesTicked++;

            var timeTaken = _quizAttempt.QuizStart.AddSeconds(_timesTicked);
            var timeRemaining = _quizEnds - timeTaken;
            TimeRemaining.Text = timeRemaining.ToString();

            if (_timesTicked >= _timesToTick)
            {
                _dispatcherTimer.Stop();
                _quizAttempt.EndQuiz();
                
                Frame.Navigate(typeof(EndOfQuizResultsSummary), _quizAttempt);
            }
        }

        public BitmapImage BackgroundImage
        {
            get
            {
                return new BitmapImage(new Uri("ms-appx:///Assets/Quizzes/" + Quiz + ".png"));

                //return new ImageBrush {ImageSource = bitmapImage};
            }
        }




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




        // SelectionChanged event handler for the SemanticFlipView
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Close the app bar
            this.globalAppBar.IsOpen = false;

            // Let the page know it is has been unselected
            foreach (var item in e.RemovedItems)
            {
                ((IGesturePageInfo)item).Selected = false;
            }

            // Let the page know it has been selected
            foreach (var item in e.AddedItems)
            {
                ((IGesturePageInfo)item).Selected = true;
            }
        }

        // ViewChangeStarted event handler for the SemanticZoom
        private void OnViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            // Close the app bar
            this.globalAppBar.IsOpen = false;

            if (!e.IsSourceZoomedInView)
            {
                // Unsnap the app (if necessary) before going into zoomed in view
                if (ApplicationView.Value == ApplicationViewState.Snapped)
                {
                    if (!ApplicationView.TryUnsnap())
                    {
                        // Could not unsnap, go back to zoomed out view
                        this.semanticZoom.IsZoomedInViewActive = false;
                    }
                }
            }
            else
            {
                // Reset the flip view's selected item so that when we go back to zoomed in view
                // SelectionChanged event is triggered for sure
                this.zoomedInFlipView.SelectedIndex = -1;
            }

            foreach (var page in _pages)
            {
                page.UpdateZoomedOutImage();
            }
        }

        // ViewChangeCompleted event handler for the SemanticZoom
        private void OnViewChangeCompleted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (e.IsSourceZoomedInView)
            {
                // Going to zoomed out view

                //this.nonContextualItemsPanel.Children.Add(this._linksButton);
                
                
            }
            else
            {
                // Going to zoomed in view
                
                //this.nonContextualItemsPanel.Children.Remove(this._linksButton);
            }
        }
    }

    public class QuizAttempt
    {
        public QuizAttempt(List<QuizQuestion> quizQuestions, bool practiceMode)
        {
            QuizQuestions = quizQuestions;
            PracticeMode = practiceMode;
            QuizStart = DateTimeOffset.Now;
        }

        public void EndQuiz()
        {
            if (QuizEnd != DateTimeOffset.MinValue)
                throw new Exception("The quiz has already ended.");

            QuizEnd = DateTimeOffset.Now;
        }

        public bool PracticeMode;

        public List<QuizQuestion> QuizQuestions;
        public DateTimeOffset QuizStart;
        public DateTimeOffset QuizEnd;

        private float Score
        {
            get { return QuizQuestions.Where(x => x.SelectedAnswer != null).Sum(x => x.SelectedAnswer.Score); }
        }

        private float ScoreOutOf { get { return QuizQuestions.Sum(x => x.Answers.Single(a => a.Score > 0).Score); } }
        private double ScorePercentage { get { return Math.Round((Score / ScoreOutOf) * 100); } }

        public string QuizResult()
        {
            return string.Format("{0}/{1} ({2}%)", Score, ScoreOutOf, ScorePercentage);
        }

        public int MaxQuizDuration
        {
            get { return QuizQuestions.Count; }
        }
        public bool QuizDurationExpired
        {
            get { return QuizDuration.Minutes > MaxQuizDuration; }
        }

        public TimeSpan QuizDuration
        {
            get { return QuizEnd.Subtract(QuizStart); }
        }
    }

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
