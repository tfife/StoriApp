using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Shapes;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Stori
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Timeline : Page
    {

        int eventHeight = 16;
        int eventPadding = 8;
        int tickWidth = 32;
        int tickHeight = 16;
        int lineWidth = 3;
        int timelineWidth = 6000;
        int timelineBottomPadding = 300;
        int timelineTopPadding = 3000;
        int pageIndex = 0;

        Canvas tlCanvas;
        Classes.Timeline timeline;

        public Timeline()
        {
            this.InitializeComponent();


            Classes.TimeSystem timeSystem = new Classes.TimeSystem(
                "year",
                "month",
                "day",
                "hour",
                "minute",
                "second",
                //12,
                //24,
                //60,
                //60,
                10,
                10,
                10,
                10,
                true,
                new List<string>() { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" },
                new List<string>() { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" },
                //new List<int>() { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 }
                new List<int>() { 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 });
            
            Classes.CustomDateTime startDate = new Classes.CustomDateTime(timeSystem, 2020, 1, 1, dayNameIndex: 3);
            Classes.CustomDateTime endDate = new Classes.CustomDateTime(timeSystem, 2121, 3, 31);

            timeline = new Classes.Timeline(startDate, endDate, 3, timeSystem, currentZoomLevel: Classes.Timeline.ZoomLevel.HourMinute);

            tlCanvas = new Canvas
            { 
                Height = getTimelineHeight(),
                Width = timelineWidth,
            };

            populateTimeline();
            scrollToBottom();
            populateEvents();
            
        }

        int getTimelineHeight()
        {
            return timelineBottomPadding + timelineTopPadding + lineWidth;
        }

        void populateTimeline()
        {
            //clear existing timeline
            tlCanvas.Children.Clear();

            //add label to the top 
            TimelineLabelTextBlock.Text = timeline.GetLabelForPage(pageIndex);

            //add the ticks to the main line
            Classes.CustomDateTime refDateTime = timeline.GetStartDateForPage(pageIndex).GetDateTime();
            int offset = eventPadding;
            bool topLabelRequired = true;
            int tickCount = 0;

            while (refDateTime <= timeline.GetEndDateForPage(pageIndex).GetDateTime())
            {
                //add the top tick
                var topTick = new Line();
                topTick.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                topTick.StrokeThickness = lineWidth;

                topTick.X1 = 0;
                topTick.Y1 = 0;
                topTick.X2 = 0;
                topTick.Y2 = 0 - tickHeight;

                topTick.Tapped += (object sender, TappedRoutedEventArgs e) =>
                {
                    DateTickClicked(refDateTime.GetDateTime());
                };

                Canvas.SetTop(topTick, timelineTopPadding);
                Canvas.SetLeft(topTick, offset);

                tlCanvas.Children.Add(topTick);

                //also add a label if necessary
                if (topLabelRequired)
                {
                    TextBlock topLabel = new TextBlock
                    {
                        FontSize = 18,
                        TextAlignment = TextAlignment.Center,
                        Text = timeline.GetTopTickLabel(refDateTime),
                        Width = 100,
                    };

                    topLabel.Tapped += (object sender, TappedRoutedEventArgs e) =>
                    {
                        DateTickClicked(refDateTime.GetDateTime());
                    };

                    Canvas.SetTop(topLabel, timelineTopPadding - tickHeight - 22);
                    Canvas.SetLeft(topLabel, offset - 50);
                    tlCanvas.Children.Add(topLabel);
                }

                topLabelRequired = !topLabelRequired;

                //if necessary, add the bottom tick
                if (timeline.IsBottomTickRequired(refDateTime))
                {
                    Line bottomTick = new Line
                    {
                        Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
                        StrokeThickness = lineWidth,

                        X1 = 0,
                        Y1 = 0,
                        X2 = 0,
                        Y2 = 0 + tickHeight
                    };

                    Canvas.SetTop(bottomTick, timelineTopPadding);
                    Canvas.SetLeft(bottomTick, offset);

                    tlCanvas.Children.Add(bottomTick);

                    //also add a label
                    TextBlock bottomLabel = new TextBlock
                    {
                        FontSize = 18,
                        TextAlignment = TextAlignment.Center,
                        Text = timeline.GetBottomTickLabel(refDateTime),
                        Width = 150,
                    };

                    Canvas.SetTop(bottomLabel, timelineTopPadding + tickHeight);
                    Canvas.SetLeft(bottomLabel, offset - 75);
                    tlCanvas.Children.Add(bottomLabel);

                }
                offset += tickWidth;
                timeline.IterateTickCustomDateTime(refDateTime);
                tickCount++;
            }

            //set the canvas width to fit all ticks and add the line
            tlCanvas.Width = offset;

            var line = new Line();
            line.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
            line.StrokeThickness = lineWidth;
            //line.Width = timelineWidth;
            line.X1 = 0;
            line.Y1 = 0;
            line.X2 = offset;
            line.Y2 = 0;

            Canvas.SetTop(line, timelineTopPadding);
            Canvas.SetLeft(line, 0);

            tlCanvas.Children.Add(line);
            //set content to be the canvas
            ScrollableCanvasContainer.Content = tlCanvas;
        }

        async void scrollToBottom()
        {
            await Task.Delay(10);
            ScrollableCanvasContainer.ChangeView(null, timelineTopPadding, null);
        }

        void populateEvents()
        {

        }

        private void TimelineNextButton_Click(object sender, RoutedEventArgs e)
        {
            pageIndex++;
            ControlEnabledButtons();
            populateTimeline();

        }

        private void TimelinePreviousButton_Click(object sender, RoutedEventArgs e)
        {
            pageIndex--;
            ControlEnabledButtons();
            populateTimeline();
        }

        void ControlEnabledButtons()
        {
            if (pageIndex <= 0)
            {
                //just in case it somehow got out of range
                pageIndex = 0;
                //disable prev button
                TimelinePreviousButton.IsEnabled = false;
            }
            else
            {
                //enable prev button
                TimelinePreviousButton.IsEnabled = true;
            }

            if (pageIndex >= timeline.pageLabels.Count - 1)
            {
                //just in case it somehow got out of range
                pageIndex = timeline.pageLabels.Count - 1;
                //disable next button
                TimelineNextButton.IsEnabled = false;
            }
            else
            {
                //enable next button
                TimelineNextButton.IsEnabled = true;
            }
        }

        void DateTickClicked(Classes.CustomDateTime date)
        {
            if (timeline.ZoomIn())
            {
                pageIndex = timeline.GetPageIndexForCustomDateTime(date);
                populateTimeline();
                scrollToBottom();
                populateEvents();
            }
        }
    }
}
