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

        public async Task SaveNewTimeline(Timeline newTimeline)
        {
            List<Classes.Timeline> allTimelines = new List<Classes.Timeline>();

            string fileName = "timelines.json";
            var file = await localFolder.TryGetItemAsync(fileName) as IStorageFile;
            if (file == null)
            {
                newTimeline.timelineId = 1;
                allTimelines.Add(newTimeline);
            }
            else
            {
                var text = await FileIO.ReadTextAsync(file);
                allTimelines = JsonConvert.DeserializeObject<List<Classes.Timeline>>(text);
                int maxId = 0;
                foreach (Classes.Timeline timeline in allTimelines)
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
    }
}
