using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XamarinEvolve.DataObjects;
using XamarinEvolve.Clients.Portable;
using FormsToolkit;

namespace XamarinEvolve.Clients.UI
{
    public partial class SessionDetailsPage : ContentPage
    {
        SessionDetailsViewModel ViewModel => vm ?? (vm = BindingContext as SessionDetailsViewModel);
        SessionDetailsViewModel vm;
        public SessionDetailsPage(Session session)
        {
            InitializeComponent();
            CarouselMaps.ItemsSource = new List<EvolveMap>
            {
                new EvolveMap
                {
                 //   Local = "floor_1.png",
                    Url = session.Title
                   // Title = "Floor Maps (1/2)"
                },
                new EvolveMap
                {
                   // Local = "floor_2.png",
                    Url = "https://s3.amazonaws.com/xamarin-releases/evolve-2016/floor_2.png",
                   // Title = "Floor Maps (2/2)"
                }
            };


            if (Device.OS == TargetPlatform.Android || Device.OS == TargetPlatform.iOS)
            {

                Title = "Floor Maps (1/2)";
                CarouselMaps.ItemSelected += (sender, args) =>
                {
                    var current = args.SelectedItem as EvolveMap;
                    if (current == null)
                        return;
                    Title = current.Title;
                };
            }

            try
            {
                FavoriteButtonAndroid.Clicked += (sender, e) =>
                {
                    Device.BeginInvokeOnMainThread(() => FavoriteIconAndroid.Grow());
                };
                FavoriteButtoniOS.Clicked += (sender, e) =>
                {
                    Device.BeginInvokeOnMainThread(() => FavoriteIconiOS.Grow());
                };
            }
            catch
            {

            }           

            //ListViewSpeakers.ItemSelected += async (sender, e) => 
            //    {
            //        var speaker = ListViewSpeakers.SelectedItem as Speaker;
            //        if(speaker == null)
            //            return;
                    
            //        var speakerDetails = new SpeakerDetailsPage(vm.Session.Id);

            //        speakerDetails.Speaker = speaker;
            //        App.Logger.TrackPage(AppPage.Speaker.ToString(), speaker.FullName);
            //        await NavigationService.PushAsync(Navigation, speakerDetails);
            //        ListViewSpeakers.SelectedItem = null;
            //    };


            ButtonRate.Clicked += async (sender, e) => 
            {
                    //if(!Settings.Current.IsLoggedIn)
                    //{
                    //    DependencyService.Get<ILogger>().TrackPage(AppPage.Login.ToString(), "Feedback");
                    //    MessagingService.Current.SendMessage(MessageKeys.NavigateLogin);
                    //    return;
                    //}
                    await NavigationService.PushModalAsync(Navigation, new EvolveNavigationPage(new FeedbackPage(ViewModel.Session)));
            };
            BindingContext = new SessionDetailsViewModel(Navigation, session); 
            ViewModel.LoadSessionCommand.Execute(null);

        }

        void ListViewTapped (object sender, ItemTappedEventArgs e)
        {
            var list = sender as ListView;
            if (list == null)
                return;
            list.SelectedItem = null;
        }

           

        //void MainScroll_Scrolled (object sender, ScrolledEventArgs e)
        //{
        //    if (e.ScrollY > SessionDate.Y)
        //        Title = ViewModel.Session.ShortTitle;
        //    else
        //        Title = "Session Details";
        //}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //MainScroll.Scrolled += MainScroll_Scrolled;
            //ListViewSpeakers.ItemTapped += ListViewTapped;
            
            if(Device.OS == TargetPlatform.Android || Device.OS == TargetPlatform.iOS)
            Application.Current.AppLinks.RegisterLink(ViewModel.Session.GetAppLink());

            var count = ViewModel?.Session?.Speakers?.Count ?? 0;
            var adjust = Device.OS != TargetPlatform.Android ? 1 : -count + 1;
            //if((ViewModel?.Session?.Speakers?.Count ?? 0) > 0)
            //    ListViewSpeakers.HeightRequest = (count * ListViewSpeakers.RowHeight) - adjust;

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        //    MainScroll.Scrolled -= MainScroll_Scrolled;
        //    ListViewSpeakers.ItemTapped -= ListViewTapped;
        }

        protected override  void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            vm = null;

          //  ListViewSpeakers.HeightRequest = 0;



        }
    }
}

