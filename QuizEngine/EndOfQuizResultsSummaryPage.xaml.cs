using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuizEngine.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
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
    public sealed partial class EndOfQuizResultsSummary : LayoutAwarePage
    {
        public EndOfQuizResultsSummary()
        {
            this.InitializeComponent();

            Window.Current.SizeChanged += VisualStateChanged;
        }

        QuizAttempt _quizAttempt;

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _quizAttempt = (QuizAttempt)e.Parameter;

            Results.Text = _quizAttempt.QuizResult();
            
            Duration.Text = _quizAttempt.QuizDuration.ToString();

            foreach (var question in _quizAttempt.QuizQuestions)
            {
                QuestionsAndAnswers.Children.Add(new MyUserControl1(question));   
            }
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (NewQuizAttemptPage));
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
