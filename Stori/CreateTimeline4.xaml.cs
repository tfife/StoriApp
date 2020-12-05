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
                populateRows();
            }
            base.OnNavigatedTo(e);
        }

        private void populateRows()
        {
            for (int i = 1; i <= this.timeSystem.daysInMonths.Count; i++)
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
                    Text = i.ToString() + ". "
                };
                muxc.NumberBox numberBox = new muxc.NumberBox()
                {
                    Value = num,
                    Minimum = 1
                };
                TextBlock text2 = new TextBlock()
                {
                    Text = this.timeSystem.eoDay + "(s) in " + this.timeSystem.monNames[i - 1].ToString()
                };
                row.Children.Add(text1);
                row.Children.Add(numberBox);
                row.Children.Add(text2);

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
                this.timeSystem.daysInMonths.Clear();

                foreach (StackPanel panel in MonthRowsStackPanel.Children)
                {
                    this.timeSystem.daysInMonths.Add((int)((muxc.NumberBox)panel.Children[1]).Value);
                }

                this.Frame.Navigate(typeof(CreateTimeline4), timeSystem);
            }
        }

        private bool validateForm()
        {
            foreach (StackPanel panel in MonthRowsStackPanel.Children)
            {
                if ((int)((muxc.NumberBox)panel.Children[1]).Value < 1)
                {
                    ((muxc.NumberBox)panel.Children[1]).Focus(FocusState.Programmatic);
                    MonthNameLabel.Text = "BAD";
                    return false;
                }
            }
            MonthNameLabel.Text = "WORKED";
            return true;
        }
    }
}
