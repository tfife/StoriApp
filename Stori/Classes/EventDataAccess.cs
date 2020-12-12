using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Stori.Classes
{
    class EventDataAccess
    {
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        public async Task SaveNewEvent(Event newEvent)
        {
            List<Classes.Event> allEvents = new List<Classes.Event>();

            string fileName = "events" + newEvent.timeline.timeSystem.timelineId + ".json";
            var file = await localFolder.TryGetItemAsync(fileName) as IStorageFile;
            if (file == null)
            {
                newEvent.eventId = 1;
                allEvents.Add(newEvent);
            }
            else
            {
                var text = await FileIO.ReadTextAsync(file);
                allEvents = JsonConvert.DeserializeObject<List<Classes.Event>>(text);
                int maxId = 0;

                if (allEvents == null)
                {
                    allEvents = new List<Event>();
                }

                foreach (Classes.Event myEvent in allEvents)
                {
                    if (myEvent.eventId >= maxId)
                    {
                        maxId = myEvent.eventId + 1;
                    }
                }
                allEvents.Add(newEvent);
            }

            var newFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            var json = JsonConvert.SerializeObject(allEvents);
            await FileIO.WriteTextAsync(newFile, json);
        }

        public async Task UpdateEvent(Event updateEvent)
        {
            List<Classes.Event> allEvents;

            string fileName = "events" + updateEvent.timeline.timeSystem.timelineId.ToString() + ".json";
            var file = await localFolder.TryGetItemAsync(fileName) as IStorageFile;
            if (file == null)
            {
                return;
            }
            else
            {
                var text = await FileIO.ReadTextAsync(file);
                allEvents = JsonConvert.DeserializeObject<List<Classes.Event>>(text);

                if (allEvents == null)
                {
                    return;
                }

                for (int i = 0; i < allEvents.Count; i++)
                {
                    if (allEvents[i].eventId == updateEvent.eventId)
                    {
                        allEvents[i] = updateEvent;
                        break;
                    }
                }
            }

            var newFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            var json = JsonConvert.SerializeObject(allEvents);
            await FileIO.WriteTextAsync(newFile, json);
        }

        public async Task<List<Event>> getAllEventsForTimeline(TimeSystem timeline)
        {
            List<Classes.Event> allEvents;

            string fileName = "events" + timeline.timelineId.ToString() + ".json";
            IStorageFile file = await localFolder.TryGetItemAsync(fileName) as IStorageFile;
            if (file == null)
            {
                return new List<Classes.Event>();
            }
            var text = await FileIO.ReadTextAsync(file);
            allEvents = JsonConvert.DeserializeObject<List<Classes.Event>>(text);

            return allEvents;
        }

        //deletes the event file for a given timeline. Used for development
        public async Task DeleteAllEventsForTimeline(TimeSystem timeline)
        {
            string fileName = "events" + timeline.timelineId.ToString() + ".json";
            IStorageFile file = await localFolder.TryGetItemAsync(fileName) as IStorageFile;
            if (file != null)
            {
                System.IO.File.Delete(file.Path);
            }
        }
    }
}
