using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Stori
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Classes.TimelineDataAccess dataAccess;

        int buttonHeight = 100;
        int buttonWidth = 600;
        int buttonFontSize = 48;
        Thickness buttonMargin = new Thickness(5);

        public MainPage()
        {
            this.InitializeComponent();

            this.dataAccess = new Classes.TimelineDataAccess();

            populateTimelines();
        }

        private async Task populateTimelines()
        {
            //uncomment the next line to delete all timelines and events
            //await this.dataAccess.DeleteAllTimelineData();

            List<Classes.TimeSystem> allTimelines = await this.dataAccess.getAllTimelines();

            Button newTimelineButton = new Button()
            {
                Height = buttonHeight,
                Width = buttonWidth,
                FontSize = buttonFontSize,
                Margin = buttonMargin,
                Background = new SolidColorBrush(Windows.UI.Colors.Orange),
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                Content = "+ New Timeline"
            };
            newTimelineButton.AddHandler(UIElement.TappedEvent, new TappedEventHandler(NewTimelineButton_Click), true);

            TimelineList.Children.Add(newTimelineButton);

            if (allTimelines == null)
            {
                return;
            }

            foreach (Classes.TimeSystem timeline in allTimelines)
            {
                Button timelineButton = new Button()
                {
                    Height = buttonHeight,
                    Width = buttonWidth,
                    FontSize = buttonFontSize,
                    Margin = buttonMargin,
                    Content = timeline.name
                };
                timelineButton.AddHandler(UIElement.TappedEvent, new TappedEventHandler((object sender, TappedRoutedEventArgs e) =>
                {
                    NavigateToTimeline(timeline);
                }), true);

                TimelineList.Children.Add(timelineButton);
            }
            return;
        }

        private void NewTimelineButton_Click(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CreateTimeline1));
        }

        private void NavigateToTimeline(Classes.TimeSystem timeline)
        {
            this.Frame.Navigate(typeof(Timeline), timeline);
        }
    }
}
