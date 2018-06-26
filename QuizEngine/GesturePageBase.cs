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
using System.Runtime.CompilerServices;
using QuizEngine.Common;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
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
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// This <see cref="Windows.UI.Xaml.Controls.Page"/> does not have direct access to the <see cref="Windows.UI.Xaml.Controls.AppBar"/>
        /// because it is used in this app as the base class for <see cref="SemanticFlipView"/> items. Therefore the page is displayed inside
        /// the <see cref="SemanticZoomPage"/> and has to use that page's AppBar.
        /// </remarks>
        /// <seealso cref="SemanticZoomPage"/>
        public GesturePageBase(ZoomedOutInfo zoomedOutInfo)
        {
            DefaultViewModel = new LayoutAwarePage.ObservableDictionary<String, Object>();

            ZoomedOutInfo = zoomedOutInfo;

            // Create GesturePageInfo
            // NOTE: we need a separate class to avoid issues when using this control as data item for the SemanticZoom
            _appPageInfo = new GesturePageInfo(this);

            // The content of the global app bar in this app is a grid that contains
            // two panels for contextual and non-contextual items respectively.
            //var appBar = (Windows.UI.Xaml.Controls.AppBar)QuizEngine.MainPage.Current.FindName("globalAppBar");
            //var grid = appBar.Content as Windows.UI.Xaml.Controls.Grid;
            //_contextualItemsPanel = grid.Children[0] as Windows.UI.Xaml.Controls.StackPanel;
            //_nonContextualItemsPanel = grid.Children[1] as Windows.UI.Xaml.Controls.StackPanel;

            // Create data structures for links and non contextual items, they will be populated by extending classes
            //_nonContextualItems = new List<Windows.UI.Xaml.Controls.Button>();
            //_links = new Dictionary<string, Uri>();

            //_isSelected = false;
        }

        protected IGesturePageInfo _appPageInfo;
        public IGesturePageInfo AppPageInfo
        {
            get { return _appPageInfo; }
        }

        // The content of the global app bar in this app is a grid that contains
        // two panels for contextual and non-contextual items respectively.
        // We store references to these panels for use by this class and its extensions.
        protected readonly Windows.UI.Xaml.Controls.StackPanel _contextualItemsPanel;
        protected readonly Windows.UI.Xaml.Controls.StackPanel _nonContextualItemsPanel;

        // Non contextual app bar items for this page, used by Selected to configure the app bar
        protected readonly List<Windows.UI.Xaml.Controls.Button> _nonContextualItems;

        public static readonly DependencyProperty DefaultViewModelProperty =
            DependencyProperty.Register("DefaultViewModel", typeof(IObservableMap<String, Object>),
            typeof(GesturePageBase), null);

        public ZoomedOutInfo ZoomedOutInfo;

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
        public GesturePageInfo(GesturePageBase gesturePageBase)
        {
            PlayArea = gesturePageBase;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Id { get { return PlayArea.ZoomedOutInfo.Text; } }

        public void UpdateZoomedOutImage()
        {
            NotifyPropertyChanged("ZoomedOutImage");
        }

        public ImageSource ZoomedOutImage
        {
            get
            {
                return new BitmapImage(new Uri(string.Format("ms-appx:///Assets/{0}", PlayArea.ZoomedOutInfo.Image)));
            }
        }
        public GesturePageBase PlayArea { get; private set; }
    }
}
