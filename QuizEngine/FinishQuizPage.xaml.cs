using QuizEngine.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace QuizEngine
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FinishQuizPage : GesturePageBase
    {
        public FinishQuizPage(QuizAttempt quizAttempt, MainPage rootPage)
            : base(new ZoomedOutInfo { Image = "Fin.png" })
        {
            this.InitializeComponent();

            _quizAttempt = quizAttempt;
            _rootPage = rootPage;
        }

        QuizAttempt _quizAttempt;
        private MainPage _rootPage;

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            _quizAttempt.EndQuiz();
            _rootPage.Frame.Navigate(typeof(ResultsPage), _quizAttempt);
        }

        

        //public void ResetQuestionAnswerIcon()
        //{
        //    NotifyPropretyChanged("ZoomedOutImage");
        //}

        ////public bool Selected { get; set; }

        //public event PropertyChangedEventHandler PropertyChanged;

        //private void NotifyPropretyChanged(string property)
        //{
        //    if (PropertyChanged != null)
        //        PropertyChanged(this, new PropertyChangedEventArgs(property));
        //}
    }
}
