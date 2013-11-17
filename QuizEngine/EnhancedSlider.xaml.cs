namespace QuizEngine
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;

    /// <summary>
    /// A Slider Control enhanced with labels and direct manual input.
    /// </summary>
    public partial class EnhancedSlider : UserControl
    {
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(EnhancedSlider), new PropertyMetadata(0));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(EnhancedSlider), new PropertyMetadata(100));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(EnhancedSlider), new PropertyMetadata(0));

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(EnhancedSlider), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ValueSuffixProperty =
            DependencyProperty.Register("ValueSuffix", typeof(string), typeof(EnhancedSlider), new PropertyMetadata(string.Empty));

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public string ValueSuffix
        {
            get { return (string)GetValue(ValueSuffixProperty); }
            set { SetValue(ValueSuffixProperty, value); }
        }

        public EnhancedSlider()
        {
            this.InitializeComponent();
        }

        ///// <summary>
        ///// Enables the textBox for input.
        ///// </summary>
        //private void SliderButton_Click(object sender, RoutedEventArgs e)
        //{
        //    this.textBox.IsEnabled = true;
        //    this.textBox.Focus(FocusState.Programmatic);
        //}

        ///// <summary>
        ///// Returns to 'Label' mode.
        ///// </summary>
        //private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    this.textBox.IsEnabled = false;
        //}

        ///// <summary>
        ///// Return to 'Label' mode on Enter key.
        ///// </summary>
        //private void TextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        //{
        //    if (e.Key == Windows.System.VirtualKey.Enter)
        //    {
        //        this.textBox.IsEnabled = false;
        //    }
        //}
    }
}
