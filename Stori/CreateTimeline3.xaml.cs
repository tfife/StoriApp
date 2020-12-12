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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Stori
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateTimeline3 : Page
    {
        Classes.TimeSystem timeSystem;
        public CreateTimeline3()
        {
            this.InitializeComponent();
            //this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Classes.TimeSystem && !string.IsNullOrWhiteSpace(((Classes.TimeSystem)e.Parameter).name))
            {
                this.timeSystem = (Classes.TimeSystem)e.Parameter;
                //TimelineName.Text = ((Classes.TimeSystem)e.Parameter).name;
                this.PopulateRows();
            }
            base.OnNavigatedTo(e);
        }

        private void PopulateRows()
        {
            //add label and rows for day names
            DayNameLabel.Text = this.timeSystem.eoDay;
            for (int i = 0; i < 7; i++)
            {
                this.AddRowToDays();
            }
            AddDayRowsButton.Content = "+ Add " + this.timeSystem.eoDay;

            //add label and rows for month names
            MonthNameLabel.Text = this.timeSystem.eoMonth;
            for (int i = 1; i <= this.timeSystem.monInYear; i++)
            {
                AddRowToMonths(i);
            }
        }

        private void AddRowToDays()
        {
            int num = DayRowsStackPanel.Children.Count() + 1;
            string defaultName;
            switch (num)
            {
                case 1:
                    defaultName = "Sunday";
                    break;
                case 2:
                    defaultName = "Monday";
                    break;
                case 3:
                    defaultName = "Tuesday";
                    break;
                case 4:
                    defaultName = "Wednesday";
                    break;
                case 5:
                    defaultName = "Thursday";
                    break;
                case 6: defaultName = "Friday";
                    break;
                case 7: defaultName = "Saturday";
                    break;
                default:
                    defaultName = this.timeSystem.eoDay + " " + num.ToString();
                    break;
            }

            StackPanel row = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            TextBlock text = new TextBlock() {
                Text = num.ToString("00") + ". name: "
            };
            TextBox textBox = new TextBox()
            {
                Text = defaultName,
                MinWidth = 300,
                Margin = new Thickness(16,0,0,0)
            };
            Button deleteButton = new Button()
            {
                Content = "X",
                Margin = new Thickness(16, 0, 0, 0),
                Background = new SolidColorBrush(Windows.UI.Colors.Red),
                Foreground = new SolidColorBrush(Windows.UI.Colors.White)
        };
            deleteButton.AddHandler(UIElement.TappedEvent, new TappedEventHandler(RemoveDayRow_Click), true);
            row.Children.Add(text);
            row.Children.Add(textBox);
            row.Children.Add(deleteButton);

            DayRowsStackPanel.Children.Add(row);
        }

        private void AddRowToMonths(int num)
        {
            string defaultName;
            switch (num)
            {
                case 1:
                    defaultName = "January";
                    break;
                case 2:
                    defaultName = "February";
                    break;
                case 3:
                    defaultName = "March";
                    break;
                case 4:
                    defaultName = "April";
                    break;
                case 5:
                    defaultName = "May";
                    break;
                case 6:
                    defaultName = "June";
                    break;
                case 7:
                    defaultName = "July";
                    break;
                case 8:
                    defaultName = "August";
                    break;
                case 9:
                    defaultName = "September";
                    break;
                case 10:
                    defaultName = "October";
                    break;
                case 11:
                    defaultName = "November";
                    break;
                case 12:
                    defaultName = "December";
                    break;
                default:
                    defaultName = this.timeSystem.eoMonth + " " + num.ToString();
                    break;
            }

            StackPanel row = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            TextBlock text = new TextBlock()
            {
                Text = num.ToString("00") + ". name: "
            };
            TextBox textBox = new TextBox()
            {
                Text = defaultName,
                MinWidth = 300,
                Margin = new Thickness(16, 0, 0, 0)
            };
            row.Children.Add(text);
            row.Children.Add(textBox);

            MonthRowsStackPanel.Children.Add(row);
        }
        private void RemoveDayRow_Click(object sender, TappedRoutedEventArgs e)
        {
            DayRowsStackPanel.Children.Remove(VisualTreeHelper.GetParent((Button)sender) as UIElement);
            for (int i = 0; i < DayRowsStackPanel.Children.Count(); i++)
            {
                ((TextBlock)((StackPanel)DayRowsStackPanel.Children[i]).Children[0]).Text = (i + 1).ToString() + ". name: ";
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CreateTimeline2));
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!validateForm())
            {
                return;
            } 
            else
            {
                this.timeSystem.dayNames.Clear();
                this.timeSystem.monNames.Clear();

                foreach (StackPanel panel in DayRowsStackPanel.Children)
                {
                    this.timeSystem.dayNames.Add(((TextBox)panel.Children[1]).Text);
                }

                foreach (StackPanel panel in MonthRowsStackPanel.Children)
                {
                    this.timeSystem.monNames.Add(((TextBox)panel.Children[1]).Text);
                }

                this.Frame.Navigate(typeof(CreateTimeline4), timeSystem);
            }
        }

        public bool validateForm()
        {
            foreach (StackPanel panel in DayRowsStackPanel.Children)
            {
                if (string.IsNullOrEmpty(((TextBox)panel.Children[1]).Text))
                {
                    ((TextBox)panel.Children[1]).Focus(FocusState.Programmatic);
                    return false;
                }
            }

            foreach (StackPanel panel in MonthRowsStackPanel.Children)
            {
                if (string.IsNullOrEmpty(((TextBox)panel.Children[1]).Text))
                {
                    ((TextBox)panel.Children[1]).Focus(FocusState.Programmatic);
                    return false;
                }
            }
            return true;
        }

        private void AddDayRowsButton_Click(object sender, RoutedEventArgs e)
        {
            AddRowToDays();
        }
    }
}
