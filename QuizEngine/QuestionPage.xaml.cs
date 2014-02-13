using System.ComponentModel;
using System.Runtime.CompilerServices;
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
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace QuizEngine
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QuestionPage : GesturePageBase//, INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        //public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        readonly QuizQuestion _quizQuestion;
        private bool _practiceMode;

        public QuestionPage(bool practiceMode, QuestionAnswer quizQuestion)
            : base(new ZoomedOutInfo { Text = quizQuestion.Question.QuestionNumber.ToString(), Image = "Unanswered.png" })
        {
            InitializeComponent();

            _practiceMode = practiceMode;
            _quizQuestion = quizQuestion.Question;

            //_quizQuestion = quizQuestion;
            //Id = _quizQuestion.QuestionNumber.ToString();
            txtTitle.DataContext = _quizQuestion.Title;// string.Format("{0}. Category: {1}. Difficulty: {2}", _quizQuestion.Title, _quizQuestion.Category, _quizQuestion.Difficulty);
            Description = _quizQuestion.Question;
            QuestionImage = "Assets/Quizzes/" + MainPage.Quiz + "/" +  _quizQuestion.Image;

            DescriptionBorder.DataContext = QuestionDescriptionColumnSpan;
            txtDescription.SetValue(Grid.ColumnSpanProperty, QuestionDescriptionColumnSpan);
            brdExplanation.DataContext = QuestionDescriptionColumnSpan;
            Answers.DataContext = AnswersColumnSpan;
            ExplanationView.DataContext = null;

            if (!_practiceMode)
            {
                txtDescription.IsHitTestVisible = false;
            }

            txtDescription.DataContext = Description;

            brdMediaBorderVisibility.Visibility = ImageBorderVisibility;

            imgQuestionImage.DataContext = QuestionImage;
            
            if (_quizQuestion.ImageUrl != null)
            {
                ImageUrl.NavigateUri = new Uri(_quizQuestion.ImageUrl);
                ImageUrl.Content = "Source";
                ImageUrl.Visibility = Visibility.Visible;
            }
            if (_quizQuestion.ImageText != null)
            {
                ImageText.Text = _quizQuestion.ImageText;
                ImageText.Visibility = Visibility.Visible;
            }
            

            if (!string.IsNullOrEmpty(_quizQuestion.Audio))
            {
                Media.Visibility = Visibility.Visible;
                MediaContent.Source = new Uri("ms-appx:///Assets/Quizzes/" + MainPage.Quiz + "/" + _quizQuestion.Audio);
                MediaContent.MediaEnded += MediaContent_MediaEnded;
            }
            
            Answers.ItemWidth = ItemWidth;

            DisplayAnswers();
            
        }

        void MediaContent_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaContent.Stop();
            Pause_Click(MediaContent, new RoutedEventArgs());
        }

        private void AnimateHand()
        {
            if (_quizQuestion.QuestionNumber == 1)
            {
                imageToAnimate.Visibility = Visibility.Visible;
                SwipeText.Visibility = Visibility.Visible;
                var f = Resources["Storyboard1"] as Storyboard;
                if (f != null) f.Begin();
            }
        }


        //private string id;
        //public string Id { get { return id; } private set { id = value; NotifyPropertyChanged(); } }

        //private string title;
        //public string Title { get { return title; } private set { title = value; NotifyPropertyChanged(); } }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public string QuestionImage { get; private set; }

        public Visibility ImageBorderVisibility
        {
            get
            {
                if (!string.IsNullOrEmpty(_quizQuestion.Audio))
                {
                    return Visibility.Visible; 
                }
                return QuestionImage.EndsWith(".png") || QuestionImage.EndsWith(".jpg") ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public int AnswersColumnSpan
        {
            get
            {
                if (_quizQuestion.ImageAnswers)
                    return 2;

                return 1;
            }
        }

        public int QuestionDescriptionColumnSpan
        {
            get
            {
                if (ImageBorderVisibility == Visibility.Visible)
                    return 1;

                if (Description.Length < 300)
                    return 1;

                return 2;
            }
        }

        double ItemWidth
        {
            get
            {
                return _quizQuestion.ImageAnswers ? 240 : 700;
            }
        }

        protected override void OnSelected()
        {
            //if (_wasPlaying)
            //{
            //    MediaContent.Play();    
            //}
        }

        protected override void OnUnselected()
        {
            //MediaContent.Pause();
        }

        private void DisplayAnswers()
        {
            foreach (var answer in _quizQuestion.Answers)
            {
                var buttonContent = new StackPanel();

                var button = new Button
                {
                    Tag = answer.Id,
                    Content = buttonContent,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(12),
                    //Margin = new Thickness(2),
                    Background = new SolidColorBrush(new Color {A = 125}),
                };

                button.Holding += ButtonOnHolding;
                button.RightTapped += ButtonOnRightTapped;

                if (!string.IsNullOrEmpty(answer.Image))
                {
                    var uri = new Uri("ms-appx:///Assets/Quizzes/" + MainPage.Quiz + "/" + answer.Image);
                    var image = new Image
                    {
                        Source = new BitmapImage(uri),
                        Stretch = Stretch.Uniform,
                        MaxHeight = 270,
                    };

                    button.HorizontalContentAlignment = HorizontalAlignment.Center;

                    buttonContent.Children.Add(image);
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

                    if (!string.IsNullOrEmpty(answer.Image))
                    {
                        textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                        textBlock.ClearValue(MinWidthProperty);
                    }

                    if (!string.IsNullOrEmpty(textBlock.Text))
                        buttonContent.Children.Add(textBlock);
                }

                button.Click += button_Click;
                Answers.Children.Add(button);
            }
        }

        private void ButtonOnRightTapped(object sender, RightTappedRoutedEventArgs rightTappedRoutedEventArgs)
        {
            EnlargeAnswerImage(sender);
        }

        private void ButtonOnHolding(object sender, HoldingRoutedEventArgs holdingRoutedEventArgs)
        {
            EnlargeAnswerImage(sender);
        }

        private void EnlargeAnswerImage(object sender)
        {
            var buttonContect = (StackPanel)((Button)sender).Content;
            var image = buttonContect.Children.SingleOrDefault(x => x is Image);
            if (image != null)
            {
                FullSizeImage(((Image)image).Source);
            }
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            AnimateHand();

            var button = ((Button) sender);

            foreach (Button answer in Answers.Children.Where(x => x is Button))
                answer.Background = Background = new SolidColorBrush(new Color { A = 125 });

            var newAnswer = _quizQuestion.Answers.Single(x => x.Id == int.Parse(button.Tag.ToString()));

            brdExplanation.Visibility = Visibility.Collapsed;

            string explanationText = "";

            if (_quizQuestion.SelectedAnswer == newAnswer)
            {
                _quizQuestion.SelectedAnswer = null;
                ExplanationView.DataContext = null;
                return;
            }

            _quizQuestion.SelectedAnswer = newAnswer;

            ZoomedOutInfo.Image = _quizQuestion.SelectedAnswer == null ? "Unanswered.png" : "Answered.png";

            if (_practiceMode)
            {
                explanationText = _quizQuestion.FullExplanation;
                brdExplanation.Visibility = string.IsNullOrEmpty(explanationText) ? Visibility.Collapsed : Visibility.Visible;

                if (_quizQuestion.SelectedAnswer.Score > 0)
                {
                    button.Background = (SolidColorBrush) Application.Current.Resources["GreenBrush"];
                }
                else
                {
                    button.Background = (SolidColorBrush) Application.Current.Resources["RedBrush"];
                }
            }
            else
            {
                button.Background = (SolidColorBrush)Application.Current.Resources["AppBlueBrush"];
            }

            ExplanationView.DataContext = string.Format("<p>{0}</p>", explanationText);
        }
 
        private void imgQuestionImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FullSizeImage(imgQuestionImage.Source);
        }

        private void FullSizeImage(ImageSource imageSource)
        {
            FullscreenImage.Visibility = Visibility.Visible;
            FullscreenImage.Source = imageSource;

            brdMediaBorderVisibility.Visibility = Visibility.Collapsed;
            Mainscreen.Opacity = 0.4;
            foreach (Button button in Answers.Children)
            {
                button.IsEnabled = false;
            }
        }

        private void FullscreenImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DismissFullScreenImage();
        }

        private void DismissFullScreenImage()
        {
            FullscreenImage.Visibility = Visibility.Collapsed;

            if (!string.IsNullOrEmpty(_quizQuestion.Image))
            {
                brdMediaBorderVisibility.Visibility = Visibility.Visible;
            }
            Mainscreen.Opacity = 1;
            foreach (Button button in Answers.Children)
            {
                button.IsEnabled = true;
            }
        }

        void Play_Click(object sender, RoutedEventArgs e)
        {
            MediaContent.Play();
            PlayMedia.Visibility = Visibility.Collapsed;
            PauseMedia.Visibility = Visibility.Visible;
        }

        void Pause_Click(object sender, RoutedEventArgs e)
        {
            MediaContent.Pause();
            PauseMedia.Visibility = Visibility.Collapsed;
            PlayMedia.Visibility = Visibility.Visible;
        }

        private void FullscreenImage_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            DismissFullScreenImage();
        }
    }
}
