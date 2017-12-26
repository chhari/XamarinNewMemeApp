using System;
using XamarinEvolve.DataStore.Abstractions;
using System.Threading.Tasks;
using System.Collections.Generic;
using XamarinEvolve.DataObjects;

using XamarinEvolve.DataStore.Mock;
using System.Linq;
using Xamarin.Forms;

namespace XamarinEvolve.DataStore.Mock
{
    public class SessionStore : BaseStore<Session>, ISessionStore
    {

        List<Session> sessions;
        ISpeakerStore speakerStore;
        ICategoryStore categoryStore;
        IFavoriteStore favoriteStore;
        IFeedbackStore feedbackStore;
        public SessionStore()
        {
            speakerStore = DependencyService.Get<ISpeakerStore>();
            favoriteStore = DependencyService.Get<IFavoriteStore>();
            categoryStore = DependencyService.Get<ICategoryStore>();
            feedbackStore = DependencyService.Get<IFeedbackStore>();
        }

        #region ISessionStore implementation

        public async override Task<Session> GetItemAsync(string id)
        {
            if (!initialized)
                await InitializeStore();
            
            return sessions.FirstOrDefault(s => s.Id == id);
        }

        public async override Task<IEnumerable<Session>> GetItemsAsync(bool forceRefresh = false)
        {
            if (!initialized)
                await InitializeStore();
            
            return sessions as IEnumerable<Session>;
        }

        public async Task<IEnumerable<Session>> GetSpeakerSessionsAsync(string speakerId)
        {
            if (!initialized)
                await InitializeStore();
            
            var results =  from session in sessions
                           where session.StartTime.HasValue
                           orderby session.StartTime.Value
                           from speaker in session.Speakers
                           where speaker.Id == speakerId
                           select session;
            
            return results;
        }

        public async Task<IEnumerable<Session>> GetNextSessions()
        {
            if (!initialized)
                await InitializeStore();

            var date = DateTime.UtcNow.AddMinutes(-30);

            var results = (from session in sessions
                where (session.IsFavorite && session.StartTime.HasValue && session.StartTime.Value > date)
                                    orderby session.StartTime.Value
                                    select session).Take(2);


            var enumerable = results as Session[] ?? results.ToArray();
            return !enumerable.Any() ? null : enumerable;
        }

        #endregion

        #region IBaseStore implementation
        bool initialized = false;
        public async override Task InitializeStore()
        {
            if (initialized)
                return;
            
            initialized = true;
            var categories = (await categoryStore.GetItemsAsync()).ToArray();
            await speakerStore.InitializeStore();
            var speakers = (await speakerStore.GetItemsAsync().ConfigureAwait(false)).ToArray();
            sessions = new List<Session>();
            int speaker = 0;
            int speakerCount = 0;
            int room = 0;
            int category = 0;
            var day = new DateTime(2016, 4, 27, 13, 0, 0, DateTimeKind.Utc);
            int dayCount = 0;
            for (int i = 0; i < titles.Length; i++)
            {
                var sessionSpeakers = new List<Speaker>();
                speakerCount++;
                
                for (int j = 0; j < speakerCount; j++)
                {
                    sessionSpeakers.Add(speakers[speaker]);
                    speaker++;
                    if (speaker >= speakers.Length)
                        speaker = 0;
                }

                if (i == 1)
                    sessionSpeakers.Add(sessions[0].Speakers.ElementAt(0));

                var cat = categories[category];
                category++;
                if (category >= categories.Length)
                    category = 0;

                var ro = rooms[room];
                room++;
                if (room >= rooms.Length)
                    room = 0;

                sessions.Add(new Session
                    {
                        Id = i.ToString(),
                        Abstract = "This is an abstract that is going to tell us all about how awsome this session is and that you should go over there right now and get ready for awesome!.",
                        MainCategory = cat,
                        Room = ro,
                        Speakers = sessionSpeakers,
                        Title = titles[i],
                        ShortTitle = titlesShort[i],
                        RemoteId = i.ToString()
                    });
                
                sessions[i].IsFavorite = await favoriteStore.IsFavorite(sessions[i].Id);
                sessions[i].FeedbackLeft = await feedbackStore.LeftFeedback(sessions[i]);

                SetStartEnd(sessions[i], day);

                if (i == titles.Length / 2)
                {
                    dayCount = 0;
                    day = new DateTime(2016, 4, 28, 13, 0, 0, DateTimeKind.Utc);
                }
                else
                {
                    dayCount++;
                    if (dayCount == 2)
                    {
                        day = day.AddHours(1);
                        dayCount = 0;
                    }
                }


                if (speakerCount > 2)
                    speakerCount = 0;
            }


            sessions.Add(new Session
                {
                    Id = sessions.Count.ToString(),
                    Abstract = "Coming soon",
                    MainCategory = categories[0],
                    Room = rooms[0],
                    //Speakers = new List<Speaker>{ speakers[0] },
                    Title = "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/16602385_1413398652043672_4820266781039836011_o.jpg",
                    ShortTitle = "Awesome",
                });
            sessions[sessions.Count - 1].IsFavorite = await favoriteStore.IsFavorite(sessions[sessions.Count - 1].Id);
            sessions[sessions.Count - 1].FeedbackLeft = await feedbackStore.LeftFeedback(sessions[sessions.Count - 1]);
            sessions[sessions.Count - 1].StartTime = null;
            sessions[sessions.Count - 1].EndTime = null;
        }

        void SetStartEnd(Session session, DateTime day)
        {
            session.StartTime = day;
            session.EndTime = session.StartTime.Value.AddHours(1);
        }

        public Task<Session> GetAppIndexSession (string id)
        {
            return GetItemAsync (id);
        }

        Room [] rooms = new [] 
        {
                new Room {Name = "Fossy Salon"},
                new Room {Name = "Crick Salon"},
                new Room {Name = "Franklin Salon"},
                new Room {Name = "Goodall Salon"},
                new Room {Name = "Linnaeus Salon"},
                new Room {Name = "Watson Salon"},
        };


        

        string[] titles = new [] {
           "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/89.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/8.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/4.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/3.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/2.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/99.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/1.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/16602385_1413398652043672_4820266781039836011_o.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/13662050_1293113744072164_8721619517729221552_o.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/13775409_1294892523894286_8278342499755378475_n.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/14706844_1288607441189461_3113964534552769392_o.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/15134693_1320629227987282_7354213665812755962_n.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/16730516_1413466118703592_5869738651918592213_n.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/17157381_1438190552897815_7030514511683453120_o.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/17880005_1476180302432173_6276343290501939225_o.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/17903857_1476193769097493_4522852155707959353_n.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/17884469_1476209412429262_952343732811229873_n.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/16602385_1413398652043672_4820266781039836011_o.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/16602385_1413398652043672_4820266781039836011_o.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/16602385_1413398652043672_4820266781039836011_o.jpg",
            "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/16602385_1413398652043672_4820266781039836011_o.jpg",


        };

        string[] titlesShort = new [] {
             "Stunning iOS Apps",
            "Material Design",
            "Making apps better",
            "3 Platforms: 1 codebase",
            "Mastering XAML",
            "NuGet your code",
            "iBeacons",
            "Wearables and IoT",
            "The next great app",
            "iOS Best Practices",
            "Navigation patterns",
            "Is your app secure?",
            "Xamarin.Insights",
            "xUnit",
            "Test Cloud at MixRadio",
            "Reactive programming",
            "Augmented reality",
            "OWASP mobile security"


        };

        #endregion
    }
}

