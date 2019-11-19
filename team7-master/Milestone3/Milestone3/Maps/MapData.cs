using Microsoft.Maps.MapControl.WPF;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Team7GUI
{
    class MapData
    {
        /// <summary>
        /// Determines what type is coming in and opens a map based on what it is
        /// </summary>
        /// <param name="element"></param>
        public MapData(Information element)
        {
            if (element is Business business)
            {
                BusinessMap(business);
            }
            else if (element is Users users)
            {
                UsersMap(users);
            }
        }

        /// <summary>
        /// Opens a map centered on a business with a textblock of information about the business
        /// </summary>
        /// <param name="currentBusiness"></param>
        private void BusinessMap(Business currentBusiness)
        {
            StringBuilder businessText = new StringBuilder();
            businessText.AppendLine(currentBusiness.Name);
            businessText.AppendLine(currentBusiness.Address);
            businessText.AppendLine("Review Count: " + currentBusiness.Reviewcount);
            businessText.AppendLine("Review Rating: " + currentBusiness.Reviewrating);
            businessText.Append("Check-Ins: " + currentBusiness.Numcheckins);
            SolidColorBrush background = new SolidColorBrush(Colors.Violet)
            {
                Opacity = 0.75
            };

            TextBlock businessInformation = new TextBlock
            {
                Text = businessText.ToString(),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Padding = new Thickness(5),
                Background = background,
                FontSize = 16,
                FontWeight = FontWeights.UltraBold
            };
            Border border = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0),
                Child = businessInformation
            };

            Location center = new Location(currentBusiness.Latitude, currentBusiness.Longitude);
            MapLayer mapLayer = new MapLayer();
            mapLayer.AddChild(border, center);

            Pushpin businessPin = new Pushpin
            {
                Location = center
            };

            Map popupMap = new Map
            {
                Center = center,
                Name = "Business"
            };
            popupMap.Children.Add(businessPin);
            popupMap.Children.Add(mapLayer);

            CreatePopup(popupMap);
        }

        /// <summary>
        /// Opens a map at the users last recorded Lat/Long coordinates
        /// </summary>
        /// <param name="users"></param>
        private void UsersMap(Users users)
        {
            Location center = new Location(Convert.ToDouble(users.Latitude), Convert.ToDouble(users.Longitude));

            Pushpin userPin = new Pushpin
            {
                Location = center
            };

            Map popupMap = new Map
            {
                Center = center,
                Name = "User"
            };
            popupMap.Children.Add(userPin);
            CreatePopup(popupMap);
        }

        private void CreatePopup(Map map)
        {
            map.CredentialsProvider = new ApplicationIdCredentialsProvider("ArVh14HJTS5NYlLej7SaUdGNP0YR6zMVjMizCx4S-qpwo_nmK9sBvHsH3xfwXqau");
            map.ZoomLevel = 14;

            Popup mapPopup = new Popup
            {
                PlacementRectangle = new Rect(new Size(SystemParameters.FullPrimaryScreenWidth, SystemParameters.FullPrimaryScreenHeight)),
                StaysOpen = false,
                Height = 540,
                Width = 960,
                Placement = PlacementMode.Center,
                Child = map,
                PopupAnimation = PopupAnimation.Fade,
                IsOpen = true
            };

            // Only for business to hide the Business Info box when user zooms out far enough
            if (map.Name == "Business")
            {
                map.ViewChangeOnFrame += new EventHandler<MapEventArgs>(ThisMap_ViewChangeOnFrame);
            }
        }

        private void ThisMap_ViewChangeOnFrame(object sender, MapEventArgs e)
        {
            if (sender is Map map)
            {
                Location spot = map.Center;

                if (map.ZoomLevel < 11)
                {
                  map.Children[1].Visibility = Visibility.Hidden;
                }
                else
                {
                  map.Children[1].Visibility = Visibility.Visible;
                }
            }
        }
    }
}
