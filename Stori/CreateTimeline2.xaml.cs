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
    public sealed partial class CreateTimeline2 : Page
    {
        Classes.TimeSystem timeSystem;
        public CreateTimeline2()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Classes.TimeSystem && !string.IsNullOrWhiteSpace(((Classes.TimeSystem)e.Parameter).name))
            {
                this.timeSystem = (Classes.TimeSystem)e.Parameter;
                TimelineName.Text = ((Classes.TimeSystem)e.Parameter).name;
            }
            else
            {
                TimelineName.Text = "Error Fetching Name";
            }
            base.OnNavigatedTo(e);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CreateTimeline1));
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {

            if (!this.validateForm())
            {
                return;
            }

            timeSystem.eoSecond = NameSecond.Text;
            timeSystem.eoMinute = NameMinute.Text;
            timeSystem.eoHour = NameHour.Text;
            timeSystem.eoDay = NameDay.Text;
            timeSystem.eoMonth = NameMonth.Text;
            timeSystem.eoYear = NameYear.Text;

            timeSystem.secInMin = (int)NumSeconds.Value;
            timeSystem.minInHour = (int)NumMinutes.Value;
            timeSystem.hourInDay = (int)NumHours.Value;

            if (VariableDaysCheckBox.IsChecked == false)
            {
                List<int> daysInMonths = new List<int>();
                
                for (int i = 0; i < (int)NumMonths.Value; i++)
                {
                    daysInMonths.Add((int)NumDays.Value);
                }

                this.timeSystem.SetDaysInMonths(daysInMonths);
            }            
            timeSystem.monInYear = (int)NumMonths.Value;

            this.Frame.Navigate(typeof(CreateTimeline3), timeSystem);
        }

        private bool validateForm()
        {
            //check seconds
            if (string.IsNullOrWhiteSpace(NameSecond.Text)) {
                NameSecond.Focus(FocusState.Programmatic);
                return false;
            }
            if ((int)NumSeconds.Value < 2)
            {
                NumSeconds.Focus(FocusState.Programmatic);
                return false;
            }
            //check minutes
            if (string.IsNullOrWhiteSpace(NameMinute.Text))
            {
                NameMinute.Focus(FocusState.Programmatic);
                return false;
            }
            if ((int)NumMinutes.Value < 2)
            {
                NumMinutes.Focus(FocusState.Programmatic);
                return false;
            }
            //check hours
            if (string.IsNullOrWhiteSpace(NameHour.Text))
            {
                NameHour.Focus(FocusState.Programmatic);
                return false;
            }
            if ((int)NumHours.Value < 2)
            {
                NumHours.Focus(FocusState.Programmatic);
                return false;
            }
            //check days
            if (string.IsNullOrWhiteSpace(NameDay.Text))
            {
                NameDay.Focus(FocusState.Programmatic);
                return false;
            }
            if (VariableDaysCheckBox.IsChecked == false && (int)NumDays.Value < 2)
            {
                NumDays.IsEnabled = true;
                NumDays.Focus(FocusState.Programmatic);
                return false;
            }
            //check months
            if (string.IsNullOrWhiteSpace(NameMonth.Text))
            {
                NameMonth.Focus(FocusState.Programmatic);
                return false;
            }
            if ((int)NumMonths.Value < 2)
            {
                NumMonths.Focus(FocusState.Programmatic);
                return false;
            }
            //check year name
            if (string.IsNullOrWhiteSpace(NameYear.Text))
            {
                NameYear.Focus(FocusState.Programmatic);
                return false;
            }

            return true;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            NumDays.IsEnabled = !(bool)((CheckBox)sender).IsChecked;
        }

    }
}
