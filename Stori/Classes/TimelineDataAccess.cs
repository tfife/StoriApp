using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace Stori.Classes
{
    class TimelineDataAccess
    {
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        string fileName = "timelines.json";

        public async Task SaveNewTimeline(TimeSystem newTimeline)
        {
            List<Classes.TimeSystem> allTimelines = new List<Classes.TimeSystem>();

            var file = await localFolder.TryGetItemAsync(fileName) as IStorageFile;
            if (file == null)
            {
                newTimeline.timelineId = 1;
                allTimelines.Add(newTimeline);
            }
            else
            {
                var text = await FileIO.ReadTextAsync(file);
                allTimelines = JsonConvert.DeserializeObject<List<Classes.TimeSystem>>(text);
                int maxId = 0;

                if(allTimelines == null)
                {
                    allTimelines = new List<TimeSystem>();
                }

                foreach (Classes.TimeSystem timeline in allTimelines)
                {
                    if (timeline.timelineId >= maxId)
                    {
                        maxId = timeline.timelineId + 1;
                    }
                }
                allTimelines.Add(newTimeline);
            }

            var newFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            var json = JsonConvert.SerializeObject(allTimelines);
            await FileIO.WriteTextAsync(newFile, json);
        }

        public async Task<List<TimeSystem>> getAllTimelines()
        {
            List<Classes.TimeSystem> allTimelines = new List<Classes.TimeSystem>();
            
            var file = await localFolder.TryGetItemAsync(fileName) as IStorageFile;
            if (file == null)
            {
                return null;
            }
            else
            {
                var text = await FileIO.ReadTextAsync(file);
                allTimelines = JsonConvert.DeserializeObject<List<Classes.TimeSystem>>(text);
                return allTimelines;
            }
        }

        //Deletes all timeline files. Used for development
        public async Task DeleteAllTimelineData()
        {

            List<Classes.TimeSystem> allTimelines = await this.getAllTimelines();

            EventDataAccess eDataAccess = new EventDataAccess();
            
            foreach(TimeSystem timeline in allTimelines)
            {
                await eDataAccess.DeleteAllEventsForTimeline(timeline);
            }
            var file = await localFolder.TryGetItemAsync(fileName) as IStorageFile;
            if (file != null)
            {
                System.IO.File.Delete(file.Path);
            }
        }
    }
}
