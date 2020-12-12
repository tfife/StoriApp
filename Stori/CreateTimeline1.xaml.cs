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
    public sealed partial class CreateTimeline1 : Page
    {
        TimelineType timelineType = TimelineType.Custom;
        Classes.TimeSystem timeSystem;
        public CreateTimeline1()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            CustomTime.IsChecked = true;
            initializeDefaultTimeSystem();
        }

        public enum TimelineType
        {
            Standard,
            Custom
        }

        private void StandardTime_Checked(object sender, RoutedEventArgs e)
        {
            timelineType = TimelineType.Standard;
        }

        private void CustomTime_Checked(object sender, RoutedEventArgs e)
        {
            timelineType = TimelineType.Custom;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewTimelineName.Text))
            {
                NewTimelineName.Focus(FocusState.Programmatic);
                return;
            }

            this.timeSystem.name = NewTimelineName.Text;

            if (this.timelineType == TimelineType.Custom)
            {
                this.Frame.Navigate(typeof(CreateTimeline2), timeSystem);
            }
            else
            {
                Classes.TimelineDataAccess dataAccess = new Classes.TimelineDataAccess();

                await dataAccess.SaveNewTimeline(timeSystem);

                this.Frame.Navigate(typeof(AddOrEditEvent), timeSystem);
            }

        }

        private void NewTimelineName_FocusDisengaged(Control sender, FocusDisengagedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(NewTimelineName.Text))
            {
                NewTimelineName.Focus(FocusState.Programmatic);
                return;
            }
        }

        private void initializeDefaultTimeSystem()
        {
            this.timeSystem = new Classes.TimeSystem(
                "year",
                "month",
                "day",
                "hour",
                "minute",
                "second",
                12,
                24,
                60,
                60,
                true,
                true,
                new List<string>() { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" },
                new List<string>() { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" },
                new List<int>() { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 });
        }
    }
}
