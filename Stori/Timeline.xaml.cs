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
using System.Diagnostics;

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
        int timelineTopPadding = 100;
        //int pageIndex = 0;
        int initialOffset = 100;

        Canvas tlCanvas;
        Classes.Timeline timeline;
        List<Classes.Event> allEvents;
        List<Classes.Event> filteredEvents;
        List<string> tags;
        List<string> activeTags;

        Classes.Event selectedEvent;
        Rectangle selectedEventItem;

        Classes.EventDataAccess dataAccess = new Classes.EventDataAccess();

        public Timeline()
        {
            this.InitializeComponent();

            tlCanvas = new Canvas
            {
                Height = getTimelineHeight(),
                Width = timelineWidth,
            };
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Classes.TimeSystem)
            {
                this.timeline = new Classes.Timeline((Classes.TimeSystem)e.Parameter);

                populateTimeline();
                //scrollToBottom();
                //populateEvents();
            }
            base.OnNavigatedTo(e);

        }

        int getTimelineHeight()
        {
            return timelineBottomPadding + timelineTopPadding + lineWidth;
        }

        async void populateTimeline()
        {
            //clear existing canvas
            tlCanvas.Children.Clear();

            await this.getEventsFromFile();

            //if there are no events, just go to the event creation page
            if (allEvents == null || allEvents.Count == 0)
            {
                NavigateToNewEvent();
                return;
            }
            
            this.allEvents = new List<Classes.Event>(allEvents).OrderBy(o => o.startDateTime).ToList();

            List<Classes.Event> allEventsByEndDate = new List<Classes.Event>(allEvents).OrderBy(o => o.endDateTime).ToList();

            //Debug.WriteLine(allEventsByEndDate.ToString());
            FilterEvents();

            int offset = initialOffset;
            List<int> layerOffsets = new List<int>();
            List<List<Rectangle>> eventItemsByLayer = new List<List<Rectangle>>();
            Classes.CustomDateTime refDateTime = filteredEvents[0].startDateTime.GetDateTime();

            bool topLabelRequired = true;
            int tickCount = 0;

            //Canvas.SetTop(topTick, timelineTopPadding);
            List<UIElement> noVerticalOffset = new List<UIElement>();

            //Canvas.SetTop(topLabel, timelineTopPadding - tickHeight - 48);
            List<TextBlock> topLabels = new List<TextBlock>();

            //Canvas.SetTop(bottomLabel, timelineTopPadding + tickHeight);
            List<TextBlock> bottomLabels = new List<TextBlock>();

            //for each event, add ticks leading up to it and then the event itself.
            for (int index = 0; index < filteredEvents.Count; index++)
            {
                Classes.Event currentEvent = filteredEvents[index];

                //Debug.WriteLine(currentEvent.startDateTime.year.ToString());

                //add any/all ticks previous to the event start date
                while (refDateTime < currentEvent.startDateTime)
                {
                    //add the top tick
                    Line topTick = new Line();
                    topTick.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                    topTick.StrokeThickness = lineWidth;

                    topTick.X1 = 0;
                    topTick.Y1 = 0;
                    topTick.X2 = 0;
                    topTick.Y2 = 0 - tickHeight;

                    Canvas.SetLeft(topTick, offset);
                    noVerticalOffset.Add(topTick);

                    //also add a label if necessary
                    if (topLabelRequired && !this.timeline.IsBottomTickRequired(refDateTime))
                    {
                        TextBlock topLabel = new TextBlock
                        {
                            FontSize = 18,
                            TextAlignment = TextAlignment.Center,
                            Text = timeline.GetTopTickLabel(refDateTime),
                            Width = 100,
                        };
                                                
                        Canvas.SetLeft(topLabel, offset - 50);
                        topLabels.Add(topLabel);
                    }

                    //if necessary, add the bottom tick
                    if (timeline.IsBottomTickRequired(refDateTime))
                    {
                        topLabelRequired = false;
                        Line bottomTick = new Line
                        {
                            Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
                            StrokeThickness = lineWidth,

                            X1 = 0,
                            Y1 = 0,
                            X2 = 0,
                            Y2 = 0 + tickHeight
                        };

                        Canvas.SetLeft(bottomTick, offset);

                        noVerticalOffset.Add(bottomTick);

                        //also add a label
                        TextBlock bottomLabel = new TextBlock
                        {
                            FontSize = 18,
                            TextAlignment = TextAlignment.Center,
                            Text = timeline.GetBottomTickLabel(refDateTime),
                            Width = 150,
                        };

                        
                        Canvas.SetLeft(bottomLabel, offset - 75);
                        bottomLabels.Add(bottomLabel);
                    }

                    topLabelRequired = !topLabelRequired;

                    offset += tickWidth;
                    timeline.IterateTickCustomDateTime(refDateTime);
                    tickCount++;

                    //Debug.WriteLine("end of iteration for while loop");
                }

                //now do the event itself
                int width = this.timeline.GetEventWidth(currentEvent, tickWidth, eventPadding);

                //Debug.WriteLine("GetEventWidth finished");
                //create the event item
                Rectangle eventItem = new Rectangle()
                {
                    Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
                    Fill = new SolidColorBrush(Windows.UI.Colors.SlateGray),
                    Height = this.eventHeight,
                    Width = width,
                    RadiusX = 10,
                    RadiusY = 10
                };

                eventItem.AddHandler(UIElement.TappedEvent, new TappedEventHandler((object sender, TappedRoutedEventArgs e) =>
                {
                    this.EventClicked(currentEvent, (Rectangle)sender);
                }), true);

                Canvas.SetLeft(eventItem, offset - eventPadding);

                int eventEnd = offset - eventPadding + width;

                //now figure out which layer to put it in so a vertical height can be given later
                int eventLayer = 0;
                bool isLayerAssigned = false;

                //for each later in the offsets....
                for (int i = 0; i < layerOffsets.Count; i++)
                {
                    //if the offset for this layer is less than the current offset, then we're good to add it to this layer
                    if (layerOffsets[i] < offset - eventPadding)
                    {
                        //assign the layer, say it's been assigned, and set the new layer offset
                        eventLayer = i;
                        isLayerAssigned = true;
                        layerOffsets[i] = eventEnd;
                        Debug.WriteLine("Event range: " + (offset - eventPadding) + "-" + eventEnd);
                        break;
                    }
                }

                Debug.WriteLine("Event range: " + (offset - eventPadding) + "-" + eventEnd);
                // if none of the currently existing offsets is less than the needed one
                //add a fresh layer
                if (isLayerAssigned == false)
                {
                    Debug.WriteLine("add a layer");
                    layerOffsets.Add(eventEnd);
                    eventItemsByLayer.Add(new List<Rectangle>());
                    eventLayer = layerOffsets.Count - 1;
                }
                eventItemsByLayer[eventLayer].Add(eventItem);
            }

            //Debug.WriteLine("about to enter end day loop");
            //repeat adding ticks until the latest end date
            while (refDateTime <= allEventsByEndDate[allEventsByEndDate.Count - 1].endDateTime)
            {
                //Debug.WriteLine("begin end day loop");
                //Debug.WriteLine("end date month =" + allEventsByEndDate[allEventsByEndDate.Count - 1].endDateTime.month.ToString());
                //Debug.WriteLine("ref date month =" + refDateTime.month.ToString());

                //add the top tick
                var topTick = new Line();
                topTick.Stroke = new SolidColorBrush(Windows.UI.Colors.Green);
                topTick.StrokeThickness = lineWidth;

                topTick.X1 = 0;
                topTick.Y1 = 0;
                topTick.X2 = 0;
                topTick.Y2 = 0 - tickHeight;

                Canvas.SetLeft(topTick, offset);
                noVerticalOffset.Add(topTick);

                //also add a label if necessary
                if (topLabelRequired && !this.timeline.IsBottomTickRequired(refDateTime))
                {
                    TextBlock topLabel = new TextBlock
                    {
                        FontSize = 18,
                        TextAlignment = TextAlignment.Center,
                        Text = timeline.GetTopTickLabel(refDateTime),
                        Width = 100,
                    };

                    Canvas.SetLeft(topLabel, offset - 50);
                    topLabels.Add(topLabel);
                }

                //if necessary, add the bottom tick
                if (timeline.IsBottomTickRequired(refDateTime))
                {
                    topLabelRequired = false;
                    Line bottomTick = new Line
                    {
                        Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
                        StrokeThickness = lineWidth,

                        X1 = 0,
                        Y1 = 0,
                        X2 = 0,
                        Y2 = 0 + tickHeight
                    };

                    Canvas.SetLeft(bottomTick, offset);

                    noVerticalOffset.Add(bottomTick);

                    //also add a label
                    TextBlock bottomLabel = new TextBlock
                    {
                        FontSize = 18,
                        TextAlignment = TextAlignment.Center,
                        Text = timeline.GetBottomTickLabel(refDateTime),
                        Width = 150,
                    };


                    Canvas.SetLeft(bottomLabel, offset - 75);
                    bottomLabels.Add(bottomLabel);
                }

                topLabelRequired = !topLabelRequired;

                offset += tickWidth;
                timeline.IterateTickCustomDateTime(refDateTime);
                tickCount++;

                //Debug.WriteLine("end of while iteration");
            }


            //ADD ONE MORE TICK 

            //add the last top tick
            Line lastTick = new Line();
            lastTick.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
            lastTick.StrokeThickness = lineWidth;

            lastTick.X1 = 0;
            lastTick.Y1 = 0;
            lastTick.X2 = 0;
            lastTick.Y2 = 0 - tickHeight;

            Canvas.SetLeft(lastTick, offset);
            noVerticalOffset.Add(lastTick);

            //also add the last top label if necessary
            if (topLabelRequired && !this.timeline.IsBottomTickRequired(refDateTime))
            {
                TextBlock topLabel = new TextBlock
                {
                    FontSize = 18,
                    TextAlignment = TextAlignment.Center,
                    Text = timeline.GetTopTickLabel(refDateTime),
                    Width = 100,
                };

                Canvas.SetLeft(topLabel, offset - 50);
                topLabels.Add(topLabel);
            }

            //if necessary, add the last bottom tick
            if (timeline.IsBottomTickRequired(refDateTime))
            {
                topLabelRequired = false;
                Line bottomTick = new Line
                {
                    Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
                    StrokeThickness = lineWidth,

                    X1 = 0,
                    Y1 = 0,
                    X2 = 0,
                    Y2 = 0 + tickHeight
                };

                Canvas.SetLeft(bottomTick, offset);

                noVerticalOffset.Add(bottomTick);

                //also add a label
                TextBlock bottomLabel = new TextBlock
                {
                    FontSize = 18,
                    TextAlignment = TextAlignment.Center,
                    Text = timeline.GetBottomTickLabel(refDateTime),
                    Width = 150,
                };


                Canvas.SetLeft(bottomLabel, offset - 75);
                bottomLabels.Add(bottomLabel);
            }


            //Now add all the Top levels
            timelineTopPadding += tickHeight;
            timelineTopPadding += 50; //room for the labels

            timelineTopPadding += (eventHeight + eventPadding) * layerOffsets.Count();
            if (timelineTopPadding < 300)
            {
                timelineTopPadding = 300;
            }

            //set the canvas width to fit all ticks and add the line
            tlCanvas.Width = offset += initialOffset;
            tlCanvas.Height = timelineTopPadding + timelineBottomPadding;

            //add the main line
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

            //add all the children elements now that top is known
            foreach (Line tick in noVerticalOffset)
            {
                Canvas.SetTop(tick, timelineTopPadding);
                tlCanvas.Children.Add(tick);
            }
            int topLabelTop = timelineTopPadding - tickHeight - 48;
            foreach (TextBlock label in topLabels)
            {
                Canvas.SetTop(label, topLabelTop);
                tlCanvas.Children.Add(label);
            }
            foreach (TextBlock label in bottomLabels)
            {
                Canvas.SetTop(label, timelineTopPadding + tickHeight + eventPadding);
                tlCanvas.Children.Add(label);
            }
            //add the events
            for (int i = 0; i < eventItemsByLayer.Count; i++)
            {
                Debug.WriteLine("layer = " + i.ToString());
                foreach (Rectangle eventItem in eventItemsByLayer[i])
                {
                    Canvas.SetTop(eventItem, (topLabelTop - eventHeight - eventPadding) - i * (eventHeight + eventPadding));
                    tlCanvas.Children.Add(eventItem);
                }
            }

            //set content to be the canvas
            ScrollableCanvasContainer.Content = tlCanvas;
            scrollToBottom();
        }

        async Task getEventsFromFile()
        {
            this.allEvents = await dataAccess.getAllEventsForTimeline(this.timeline.timeSystem);

            Debug.WriteLine("inside getEventsFromFile");
        }

        async void scrollToBottom()
        {
            await Task.Delay(10);
            ScrollableCanvasContainer.ChangeView(null, timelineTopPadding, null);
        }

        private void FilterEvents()
        {
            Debug.WriteLine("entered FilterEvents()");
            this.filteredEvents = this.allEvents;

            Debug.WriteLine("FilterEvents Finished!");
        }


        private void NewEventButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToNewEvent();
        }

        private void NavigateToNewEvent()
        {
            //TimelineLabelTextBlock.Text  = this.timeline.timeSystem.GetDaysInMonthText();
            this.Frame.Navigate(typeof(AddOrEditEvent), this.timeline.timeSystem);
        }

        private void EventClicked(Classes.Event myEvent, Rectangle senderRec)
        {
            ClearSelectedEvent();
            this.selectedEvent = myEvent;
            this.selectedEventItem = senderRec;

            senderRec.Stroke = new SolidColorBrush(Windows.UI.Colors.Fuchsia);
            EventDetailTitle.Text = myEvent.eventId.ToString() + myEvent.title;
            EventDetailDates.Text = myEvent.startDateTime.GetDate(myEvent.includesMonth, myEvent.includesDay, myEvent.includesHour, myEvent.includesMinute, myEvent.includesSecond) 
                + " - " 
                + myEvent.endDateTime.GetDate(myEvent.includesMonth, myEvent.includesDay, myEvent.includesHour, myEvent.includesMinute, myEvent.includesSecond);
            EventDetailDescription.Text = myEvent.description;

            string tags = "";
            if (myEvent.tags != null && myEvent.tags.Count > 0)
            {
                tags += "Tags: ";
                for (int i = 0; i < myEvent.tags.Count - 1; i++)
                {
                    tags += myEvent.tags[i] + ", ";
                }
                tags += myEvent.tags[myEvent.tags.Count - 1];
            }
                EventDetailTags.Text = tags;

            EventDetailsStackPanel.Visibility = Visibility.Visible;

        }

        private void ClearSelectedEvent()
        {
            if (selectedEventItem != null)
            {
                this.selectedEventItem.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
            }
            
            this.selectedEventItem = null;
            this.selectedEvent = null;
        }

        private void CloseEventDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedEventItem is null)
            {
                return;
            }
            selectedEventItem.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
            EventDetailsStackPanel.Visibility = Visibility.Collapsed;
            ClearSelectedEvent();
        }

        private void EditEventButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedEvent is null)
            {
                return;
            }
            Debug.WriteLine("event: " + this.selectedEvent.title);
            this.Frame.Navigate(typeof(AddOrEditEvent), this.selectedEvent);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }
    }
}
