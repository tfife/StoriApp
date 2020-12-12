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
using muxc = Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
namespace Stori
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateTimeline4 : Page
    {
        Classes.TimeSystem timeSystem;
        
        public CreateTimeline4()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Classes.TimeSystem)
            {
                this.timeSystem = (Classes.TimeSystem)e.Parameter;
                if (!this.timeSystem.variableLengthMonths)
                {
                    SaveAndShowTimeline();
                    return;
                }
                populateRows();
            }
            base.OnNavigatedTo(e);
        }

        private void populateRows()
        {
            for (int i = 1; i <= this.timeSystem.monInYear; i++)
            {
                int num; 

                switch(i)
                {
                    case 1:
                        num = 31;
                        break;
                    case 2:
                        num = 28;
                        break;
                    case 3:
                        num = 31;
                        break;
                    case 4:
                        num = 30;
                        break;
                    case 5:
                        num = 31;
                        break;
                    case 6:
                        num = 30;
                        break;
                    case 7:
                        num = 31;
                        break;
                    case 8:
                        num = 31;
                        break;
                    case 9:
                        num = 30;
                        break;
                    case 10:
                        num = 31;
                        break;
                    case 11:
                        num = 30;
                        break;
                    case 12:
                        num = 31;
                        break;
                    default:
                        num = 30;
                        break;
                }

                StackPanel row = new StackPanel()
                {
                    Orientation = Orientation.Horizontal
                };
                TextBlock text1 = new TextBlock()
                {
                    Text = i.ToString("00") + ". "
                };
                muxc.NumberBox numberBox = new muxc.NumberBox()
                {
                    Value = num,
                    Minimum = 1,
                    Margin = new Thickness(16, 0, 0, 0)
                };
                TextBlock text2 = new TextBlock()
                {
                    Text = this.timeSystem.eoDay + "(s) in " + this.timeSystem.monNames[i - 1].ToString(),
                    Margin = new Thickness(16, 0, 0, 0)
                };
                row.Children.Add(text1);
                row.Children.Add(numberBox);
                row.Children.Add(text2);

                if (i == 2 && this.timeSystem.leapYearEnabled)
                {
                    CheckBox useLeapYearCheckBox = new CheckBox()
                    {
                        IsChecked = true,
                        Content = "use leap year (follows standard rules)",
                        Margin = new Thickness(16, 0, 0, 0)
                    };

                    useLeapYearCheckBox.AddHandler(UIElement.TappedEvent, new TappedEventHandler(UseLeapYearCheckBox_Click), true);
                    row.Children.Add(useLeapYearCheckBox);
                }

                MonthRowsStackPanel.Children.Add(row);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CreateTimeline3));
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!validateForm())
            {
                return;
            }
            else
            {
                List<int> daysInMonths = new List<int>();

                foreach (StackPanel panel in MonthRowsStackPanel.Children)
                {
                    daysInMonths.Add((int)((muxc.NumberBox)panel.Children[1]).Value);
                }

                this.timeSystem.SetDaysInMonths(daysInMonths);

                this.SaveAndShowTimeline();
            }
        }

        private async void SaveAndShowTimeline()
        {
            Classes.TimelineDataAccess dataAccess = new Classes.TimelineDataAccess();

            await dataAccess.SaveNewTimeline(timeSystem);

            this.Frame.Navigate(typeof(AddOrEditEvent), timeSystem);

        }

        private bool validateForm()
        {
            foreach (StackPanel panel in MonthRowsStackPanel.Children)
            {
                if ((int)((muxc.NumberBox)panel.Children[1]).Value < 1)
                {
                    ((muxc.NumberBox)panel.Children[1]).Focus(FocusState.Programmatic);
                    //MonthNameLabel.Text = "BAD";
                    return false;
                }
            }
            //MonthNameLabel.Text = "WORKED";
            return true;
        }

        private void UseLeapYearCheckBox_Click(object sender, RoutedEventArgs e)
        {
            this.timeSystem.leapYearEnabled = ((CheckBox)sender).IsChecked == true;
        }
    }
}
