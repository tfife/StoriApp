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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Stori
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddOrEditEvent : Page
    {
        bool editMode;
        Classes.Event myEvent;
        Classes.TimeSystem timeSystem;
        Classes.Timeline originalTimeline;
        Unit deepestUnit;

        Classes.EventDataAccess dataAccess = new Classes.EventDataAccess();
        
        public AddOrEditEvent()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Classes.Event)
            {
                this.editMode = true;
                this.myEvent = (Classes.Event)e.Parameter;
                this.timeSystem = ((Classes.Event)e.Parameter).timeline.timeSystem;
                this.originalTimeline = ((Classes.Event)e.Parameter).timeline;
                this.PopulateForEditEvent();
            }
            if (e.Parameter is Classes.TimeSystem)
            {
                this.editMode = false;
                PageHeaderText.Text = ((Classes.TimeSystem)e.Parameter).getDaysInMonth(1, 2020).ToString();

                this.timeSystem = (Classes.TimeSystem)e.Parameter;
                this.myEvent = new Classes.Event(new Classes.Timeline(timeSystem));
                this.originalTimeline = myEvent.timeline;
                this.PopulateForNewEvent();
            }
            else
            {
                PageHeaderText.Text = "Error?";
            }
            base.OnNavigatedTo(e);
        }

        private void PopulateForEditEvent()
        {
            PageHeaderText.Text = "Edit Event";

            StartYearLabel.Text = timeSystem.eoYear;
            StartMonthLabel.Text = timeSystem.eoMonth;
            StartDayLabel.Text = timeSystem.eoDay;
            StartHourLabel.Text = timeSystem.eoHour;
            StartMinuteLabel.Text = timeSystem.eoMinute;
            StartSecondLabel.Text = timeSystem.eoSecond;
            
            EndYearLabel.Text = timeSystem.eoYear;
            EndMonthLabel.Text = timeSystem.eoMonth;
            EndDayLabel.Text = timeSystem.eoDay;
            EndHourLabel.Text = timeSystem.eoHour;
            EndMinuteLabel.Text = timeSystem.eoMinute;
            EndSecondLabel.Text = timeSystem.eoSecond;

            StartYearNumber.Value = myEvent.startDateTime.year;
            EndYearNumber.Value = myEvent.endDateTime.year;

            deepestUnit = Unit.year;

            if (myEvent.includesMonth)
            {
                StartMonthNumber.Value = myEvent.startDateTime.month;
                EndMonthNumber.Value = myEvent.endDateTime.month;

                StartMonthLabel.Visibility = Visibility.Visible;
                StartMonthNumber.Visibility = Visibility.Visible;

                EndMonthLabel.Visibility = Visibility.Visible;
                EndMonthNumber.Visibility = Visibility.Visible;
                deepestUnit = Unit.month;
            }

            if (this.myEvent.includesDay)
            {
                StartDayNumber.Value = myEvent.startDateTime.day;
                EndDayNumber.Value = myEvent.endDateTime.day;

                StartDayLabel.Visibility = Visibility.Visible;
                StartDayNumber.Visibility = Visibility.Visible;

                EndDayLabel.Visibility = Visibility.Visible;
                EndDayNumber.Visibility = Visibility.Visible;

                deepestUnit = Unit.day;
            }

            if (myEvent.includesHour)
            {
                StartHourNumber.Value = myEvent.startDateTime.hour;
                EndHourNumber.Value = myEvent.endDateTime.hour;

                StartHourLabel.Visibility = Visibility.Visible;
                StartHourNumber.Visibility = Visibility.Visible;

                EndHourLabel.Visibility = Visibility.Visible;
                EndHourNumber.Visibility = Visibility.Visible;

                deepestUnit = Unit.hour;
            }

            
            if (myEvent.includesMinute)
            {
                StartMinuteNumber.Value = myEvent.startDateTime.minute;
                EndMinuteNumber.Value = myEvent.endDateTime.minute;

                StartMinuteLabel.Visibility = Visibility.Visible;
                StartMinuteNumber.Visibility = Visibility.Visible;

                EndMinuteLabel.Visibility = Visibility.Visible;
                EndMinuteNumber.Visibility = Visibility.Visible;

                deepestUnit = Unit.minute;
            }

            if (this.myEvent.includesSecond)
            {
                StartSecondNumber.Value = myEvent.startDateTime.second;
                EndSecondNumber.Value = myEvent.endDateTime.second;

                StartSecondLabel.Visibility = Visibility.Visible;
                StartSecondNumber.Visibility = Visibility.Visible;

                EndSecondLabel.Visibility = Visibility.Visible;
                EndSecondNumber.Visibility = Visibility.Visible;

                deepestUnit = Unit.second;
            }

            SubmitButton.Content = "Save!";

            this.handleTextButtons();
            this.assignFieldLimits();
        }

        private void PopulateForNewEvent()
        {
            PageHeaderText.Text = "New Event";

            StartYearLabel.Text = timeSystem.eoYear;
            StartMonthLabel.Text = timeSystem.eoMonth;
            StartDayLabel.Text = timeSystem.eoDay;
            StartHourLabel.Text = timeSystem.eoHour;
            StartMinuteLabel.Text = timeSystem.eoMinute;
            StartSecondLabel.Text = timeSystem.eoSecond;

            EndYearLabel.Text = timeSystem.eoYear;
            EndMonthLabel.Text = timeSystem.eoMonth;
            EndDayLabel.Text = timeSystem.eoDay;
            EndHourLabel.Text = timeSystem.eoHour;
            EndMinuteLabel.Text = timeSystem.eoMinute;
            EndSecondLabel.Text = timeSystem.eoSecond;

            deepestUnit = Unit.year;

            SubmitButton.Content = "Create!";
            this.handleTextButtons();
            this.assignFieldLimits();
        }

        private void assignFieldLimits()
        {
            StartMonthNumber.Maximum = EndMonthNumber.Maximum = timeSystem.monInYear;
            StartHourNumber.Maximum = EndHourNumber.Maximum = timeSystem.hourInDay;
            StartMinuteNumber.Maximum = EndMinuteNumber.Maximum = timeSystem.minInHour;
            StartSecondNumber.Maximum = EndSecondNumber.Maximum = timeSystem.secInMin;
        }


        //these next two functions are to control the max days depending on the currently-entered month

        private void StartMonthNumber_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            int newMax = timeSystem.getDaysInMonth((int)StartMonthNumber.Value, (int)StartYearNumber.Value);
            if (StartDayNumber.Value > newMax)
            {
                StartDayNumber.Value = newMax;
            }
            StartDayNumber.Maximum = newMax;
        }

        private void EndMonthNumber_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            int newMax = timeSystem.getDaysInMonth((int)EndMonthNumber.Value, (int)EndYearNumber.Value);
            if (EndDayNumber.Value > newMax)
            {
                EndDayNumber.Value = newMax;
            }
            EndDayNumber.Maximum = newMax;
        }


        private void StartYearNumber_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            if (this.timeSystem.isLeapYear((int)StartYearNumber.Value) && (int)StartMonthNumber.Value == 2)
            {
                StartDayNumber.Maximum = timeSystem.getDaysInMonth((int)StartMonthNumber.Value, (int)StartYearNumber.Value);

            }
        }

        private void EndYearNumber_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            if (this.timeSystem.isLeapYear((int)EndYearNumber.Value) && (int)EndMonthNumber.Value == 2)
            {
                EndDayNumber.Maximum = timeSystem.getDaysInMonth((int)EndMonthNumber.Value, (int)EndYearNumber.Value);

            }
        }

        private void AddMonthOrDayTextButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (deepestUnit == Unit.year)
            {
                StartMonthLabel.Visibility = Visibility.Visible;
                StartMonthNumber.Visibility = Visibility.Visible;
                EndMonthLabel.Visibility = Visibility.Visible;
                EndMonthNumber.Visibility = Visibility.Visible;
                deepestUnit = Unit.month;
            }
            else if (deepestUnit == Unit.month)
            {
                StartDayLabel.Visibility = Visibility.Visible;
                StartDayNumber.Visibility = Visibility.Visible;
                EndDayLabel.Visibility = Visibility.Visible;
                EndDayNumber.Visibility = Visibility.Visible;

                deepestUnit = Unit.day;
            }

            this.handleTextButtons();
        }

        private void AddTimeUnitTextButton_Tapped(object sender, TappedRoutedEventArgs e)
        {

            if (deepestUnit == Unit.day)
            {
                StartHourLabel.Visibility = Visibility.Visible;
                StartHourNumber.Visibility = Visibility.Visible;
                EndHourLabel.Visibility = Visibility.Visible;
                EndHourNumber.Visibility = Visibility.Visible;

                deepestUnit = Unit.hour;
            }
            else if (deepestUnit == Unit.hour)
            {
                StartMinuteLabel.Visibility = Visibility.Visible;
                StartMinuteNumber.Visibility = Visibility.Visible;
                EndMinuteLabel.Visibility = Visibility.Visible;
                EndMinuteNumber.Visibility = Visibility.Visible;
                deepestUnit = Unit.minute;
            } 
            else if (deepestUnit == Unit.minute)
            {
                StartSecondLabel.Visibility = Visibility.Visible;
                StartSecondNumber.Visibility = Visibility.Visible;
                EndSecondLabel.Visibility = Visibility.Visible;
                EndSecondNumber.Visibility = Visibility.Visible;

                deepestUnit = Unit.second;
            }

            this.handleTextButtons();
        }

        private void handleTextButtons()
        {
            string baseString = "+ Add ";
            switch (deepestUnit)
            {
                case Unit.year:
                    AddMonthOrDayTextButton.Text = baseString + timeSystem.eoMonth;
                    AddMonthOrDayTextButton.Visibility = Visibility.Visible;
                    AddTimeUnitTextButton.Visibility = Visibility.Collapsed;

                    this.myEvent.includesDay = true;
                    this.myEvent.includesMonth = false;
                    break;
                case Unit.month:
                    AddMonthOrDayTextButton.Text = baseString + timeSystem.eoDay;
                    AddMonthOrDayTextButton.Visibility = Visibility.Visible;
                    AddTimeUnitTextButton.Visibility = Visibility.Collapsed;

                    this.myEvent.includesMonth = true;
                    this.myEvent.includesDay = false;
                    break;
                case Unit.day:
                    AddTimeUnitTextButton.Text = baseString + timeSystem.eoHour;
                    AddMonthOrDayTextButton.Visibility = Visibility.Collapsed;
                    AddTimeUnitTextButton.Visibility = Visibility.Visible;

                    this.myEvent.includesDay = true;
                    this.myEvent.includesHour = false;
                    break;
                case Unit.hour:
                    AddTimeUnitTextButton.Text = baseString + timeSystem.eoMinute;
                    AddMonthOrDayTextButton.Visibility = Visibility.Collapsed;
                    AddTimeUnitTextButton.Visibility = Visibility.Visible;

                    this.myEvent.includesHour = true;
                    this.myEvent.includesMinute = false;
                    break;
                case Unit.minute:
                    AddTimeUnitTextButton.Text = baseString + timeSystem.eoSecond;
                    AddMonthOrDayTextButton.Visibility = Visibility.Collapsed;
                    AddTimeUnitTextButton.Visibility = Visibility.Visible;

                    this.myEvent.includesMinute = true;
                    this.myEvent.includesSecond = false;
                    break;
                //default will be for seconds
                default:
                    AddMonthOrDayTextButton.Visibility = Visibility.Collapsed;
                    AddTimeUnitTextButton.Visibility = Visibility.Collapsed;

                    this.myEvent.includesSecond = true;
                    break;
            }
                
        }

        private bool validateForm()
        {
            DateOrderErrorText.Visibility = Visibility.Collapsed;

            if (string.IsNullOrEmpty(EventTitleTextBox.Text))
            {
                EventTitleTextBox.Focus(FocusState.Programmatic);
                return false;
            }

            //start date fields
            if (this.myEvent.includesMonth && !(StartMonthNumber.Value > 0))
            {
                StartMonthNumber.Focus(FocusState.Programmatic);
                return false;
            }
            if (this.myEvent.includesDay && !(StartDayNumber.Value > 0))
            {
                StartDayNumber.Focus(FocusState.Programmatic);
                return false;
            }
            if (this.myEvent.includesHour && !(StartHourNumber.Value >= 0))
            {
                StartHourNumber.Focus(FocusState.Programmatic);
                return false;
            }
            if (this.myEvent.includesMinute && !(StartMinuteNumber.Value >= 0))
            {
                StartMinuteNumber.Focus(FocusState.Programmatic);
                return false;
            }
            if (this.myEvent.includesSecond && !(StartSecondNumber.Value >= 0))
            {
                StartSecondNumber.Focus(FocusState.Programmatic);
                return false;
            }

            //End date fields
            if (this.myEvent.includesMonth && !(EndMonthNumber.Value > 0))
            {
                EndMonthNumber.Focus(FocusState.Programmatic);
                return false;
            }
            if (this.myEvent.includesDay && !(EndDayNumber.Value > 0))
            {
                EndDayNumber.Focus(FocusState.Programmatic);
                return false;
            }
            if (this.myEvent.includesHour && !(EndHourNumber.Value >= 0))
            {
                EndHourNumber.Focus(FocusState.Programmatic);
                return false;
            }
            if (this.myEvent.includesMinute && !(EndMinuteNumber.Value >= 0))
            {
                EndMinuteNumber.Focus(FocusState.Programmatic);
                return false;
            }
            if (this.myEvent.includesSecond && !(EndSecondNumber.Value >= 0))
            {
                EndSecondNumber.Focus(FocusState.Programmatic);
                return false;
            }

            Classes.CustomDateTime startDate = new Classes.CustomDateTime(
                timeSystem.GetTimeSystem(), 
                (int)StartYearNumber.Value, 
                (int)StartMonthNumber.Value, 
                (int)StartDayNumber.Value,
                (int)StartHourNumber.Value,
                (int)StartMinuteNumber.Value,
                (int)StartSecondNumber.Value );

            Classes.CustomDateTime endDate = new Classes.CustomDateTime(
                timeSystem.GetTimeSystem(),
                (int)EndYearNumber.Value,
                (int)EndMonthNumber.Value,
                (int)EndDayNumber.Value,
                (int)EndHourNumber.Value,
                (int)EndMinuteNumber.Value,
                (int)EndSecondNumber.Value);

            //check that end comes after start
            if (endDate <= startDate)
            {
                DateOrderErrorText.Visibility = Visibility.Visible;
                return false;
            }

            myEvent.startDateTime = startDate;
            myEvent.endDateTime = endDate;

            myEvent.title = EventTitleTextBox.Text;
            myEvent.description = EventDescriptionTextBox.Text;

            if (!editMode)
            {
                myEvent.timeline = originalTimeline;
            }

            return true;
        }

        private enum Unit
        {
            year,
            month,
            day,
            hour,
            minute,
            second
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (validateForm()) 
            {
                this.SaveAndShowTimeilne();
            }
            return;
        }

        private async void SaveAndShowTimeilne()
        {
            await dataAccess.SaveNewEvent(myEvent);
            this.Frame.Navigate(typeof(Timeline), timeSystem);
            return;
        }
    }
}
