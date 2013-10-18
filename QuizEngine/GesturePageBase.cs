//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using QuizEngine.Common;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace QuizEngine.Controls
{

    /// <summary>
    /// Basic class for each gesture page, defines the properties that are used by the UI.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public class GesturePageBase : Windows.UI.Xaml.Controls.Page
    {
        ///// <summary>
        ///// Creates a button that when clicked displays a flyout with the specified <paramref name="links"/>.
        ///// Clicking on a link opens a web browser on that page.
        ///// </summary>
        ///// <param name="links"></param>
        ///// <returns></returns>
        //internal static Windows.UI.Xaml.Controls.Button CreateLinksAppBarButton(Dictionary<string, Uri> links)
        //{
        //    var popup = new Windows.UI.Popups.PopupMenu();
        //    Windows.UI.Popups.UICommandInvokedHandler popupHandler = async command =>
        //    {
        //        await Windows.System.Launcher.LaunchUriAsync(links[command.Label]);
        //    };

        //    foreach (var item in links)
        //    {
        //        popup.Commands.Add(new Windows.UI.Popups.UICommand(item.Key, popupHandler));
        //    }

        //    var button = new Windows.UI.Xaml.Controls.Button
        //    {
        //        Style = App.Current.Resources["LinksAppBarButtonStyle"] as Style
        //    };
        //    button.Click += async (sender, e) =>
        //    {
        //        var btnSender = sender as Windows.UI.Xaml.Controls.Button;
        //        var transform = btnSender.TransformToVisual(Window.Current.Content) as Windows.UI.Xaml.Media.MatrixTransform;
        //        var point = transform.TransformPoint(new Windows.Foundation.Point());             

        //        await popup.ShowAsync(point);
        //    };

        //    return button;
        //}



        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// This <see cref="Windows.UI.Xaml.Controls.Page"/> does not have direct access to the <see cref="Windows.UI.Xaml.Controls.AppBar"/>
        /// because it is used in this app as the base class for <see cref="SemanticFlipView"/> items. Therefore the page is displayed inside
        /// the <see cref="SemanticZoomPage"/> and has to use that page's AppBar.
        /// </remarks>
        /// <seealso cref="SemanticZoomPage"/>
        public GesturePageBase()
        {
            // Create GesturePageInfo
            // NOTE: we need a separate class to avoid issues when using this control as data item for the SemanticZoom
            _appPageInfo = new GesturePageInfo(this);

            // The content of the global app bar in this app is a grid that contains
            // two panels for contextual and non-contextual items respectively.
            var appBar = (Windows.UI.Xaml.Controls.AppBar)QuizEngine.MainPage.Current.FindName("globalAppBar");
            var grid = appBar.Content as Windows.UI.Xaml.Controls.Grid;
            _contextualItemsPanel = grid.Children[0] as Windows.UI.Xaml.Controls.StackPanel;
            _nonContextualItemsPanel = grid.Children[1] as Windows.UI.Xaml.Controls.StackPanel;

            // Create data structures for links and non contextual items, they will be populated by extending classes
            _nonContextualItems = new List<Windows.UI.Xaml.Controls.Button>();
            //this._links = new Dictionary<string, Uri>();

            DefaultViewModel = new LayoutAwarePage.ObservableDictionary<String, Object>();

            _isSelected = false;
        }

        protected IGesturePageInfo _appPageInfo;
        public IGesturePageInfo AppPageInfo
        {
            get { return _appPageInfo; }
        }

        public ImageBrush ZoomedOutImage;

        //protected readonly Dictionary<string, Uri> _links;
        //public IEnumerable<KeyValuePair<string, Uri>> Links
        //{
        //    get { return this._links; }
        //}

        // The content of the global app bar in this app is a grid that contains
        // two panels for contextual and non-contextual items respectively.
        // We store references to these panels for use by this class and its extensions.
        protected readonly Windows.UI.Xaml.Controls.StackPanel _contextualItemsPanel;
        protected readonly Windows.UI.Xaml.Controls.StackPanel _nonContextualItemsPanel;

        // Non contextual app bar items for this page, used by Selected to configure the app bar
        protected readonly List<Windows.UI.Xaml.Controls.Button> _nonContextualItems;

        // Whether this is the selected page in the FlipView
        private bool _isSelected;
        internal bool Selected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    if (value)
                    {
                        OnSelected();
                    }
                    else
                    {
                        OnUnselected();
                    }

                    _isSelected = value;
                }
            }
        }

        // Called when this becomes the selected page in the FlipView
        protected virtual void OnSelected()
        {
            // Populate the panel for non contextual items
            foreach (var item in _nonContextualItems)
            {
                _nonContextualItemsPanel.Children.Add(item);
            }
        }

        // Called when this is no longer the selected page in the FlipView
        protected virtual void OnUnselected()
        {
            // Remove items from the panel for non contextual items
            foreach (var item in _nonContextualItems)
            {
                _nonContextualItemsPanel.Children.Remove(item);
            }
        }

        public static readonly DependencyProperty DefaultViewModelProperty =
            DependencyProperty.Register("DefaultViewModel", typeof(IObservableMap<String, Object>),
            typeof(GesturePageBase), null);

        protected IObservableMap<String, Object> DefaultViewModel
        {
            get
            {
                return this.GetValue(DefaultViewModelProperty) as IObservableMap<String, Object>;
            }

            set
            {
                this.SetValue(DefaultViewModelProperty, value);
            }
        }
    }



    sealed class GesturePageInfo : IGesturePageInfo, INotifyPropertyChanged
    {
        //private readonly QuizQuestion _quizQuestion;

        public GesturePageInfo(GesturePageBase playArea)//id, String title, String description, string questionImage, GesturePageBase playArea)
        {
            PlayArea = playArea;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropretyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public void UpdateZoomedOutImage()
        {
            NotifyPropretyChanged("ZoomedOutImage");
        }

        public ImageSource ZoomedOutImage
        {
            get
            {
                //if (_quizQuestion.SelectedAnswer == null)
                //{
                //    return new BitmapImage(new Uri("ms-appx:///Assets/Unanswered.png"));
                //}
                return new BitmapImage(new Uri("ms-appx:///Assets/Answered.png"));
            }
        }

        

        public GesturePageBase PlayArea { get; private set; }

        public bool Selected { get { return PlayArea.Selected; } set { PlayArea.Selected = value; } }
    }
}
