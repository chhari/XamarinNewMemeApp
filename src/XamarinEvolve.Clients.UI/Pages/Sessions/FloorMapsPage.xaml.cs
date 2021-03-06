﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace XamarinEvolve.Clients.UI
{
    public partial class FloorMapsPage : ContentPage
    {
        public FloorMapsPage()
        {
            InitializeComponent();

            CarouselMaps.ItemsSource = new List<EvolveMap>
            {
                new EvolveMap
                {
                 //   Local = "floor_1.png",
                    Url = "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/16602385_1413398652043672_4820266781039836011_o.jpg",
                   // Title = "Floor Maps (1/2)"
                },
                new EvolveMap
                {
                   // Local = "floor_2.png",
                    Url = "http://rdgw.kaizen.massopencloud.org/swift/v1/Harry/pics/16602385_1413398652043672_4820266781039836011_o.jpg",
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
        }
    }
}
