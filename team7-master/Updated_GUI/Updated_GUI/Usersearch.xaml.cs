using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Updated_GUI
{
    /// <summary>
    /// Interaction logic for Usersearch.xaml
    /// </summary>
    public partial class Usersearch : Page
    {
        public Usersearch()
        {
            InitializeComponent();
           
        }
        private void SelectSearchButton_Click(object sender, RoutedEventArgs e)
        {            
            
        }

        private void UserShowMapButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void SelectUserSearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
           
        }

        private void SelectUserListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            
        }

        /// <summary>
        /// Enables or Disables all grid elements except the search a user area
        /// </summary>
        /// <param name="mode"></param>
        private void ChangeGridStates(bool mode)
        {
            userInformationGroup.IsEnabled = mode;
            favoritesGroup.IsEnabled = mode;
            friendsGroup.IsEnabled = mode;
            recentReviewsGroup.IsEnabled = mode;
            friendsReviewsGroup.IsEnabled = mode;
        }

        /// <summary>
        /// Populate user information based off the instance of a User
        /// </summary>
        /// <param name="currentUser">Selected user from the search list results</param>
        private void FillUserInformation()
        {           
            //userLatitude.Text = currentUser.Latitude.ToString();
            //userLongitude.Text = currentUser.Longitude.ToString();

            // WSU EVERETT COORDS (COMMENT ME OUT AND UNCOMMENT ABOVE WHEN IMPLEMENT COORDS)
            userLatitude.Text = "48.0050156";
            userLongitude.Text = "-122.1995253";
        }

        /// <summary>
        /// Populate the friends data grid
        /// </summary>
        /// <param name="currentUser"></param>
        private void FillFriendsInformation()
        {
            
        }

        /// <summary>
        /// Populate favorite business grid
        /// </summary>
        /// <param name="currentUser"></param>
        private void FillFavoriteBusinessGrid()
        {
           
        }

        private void FillFriendsReviewsGrid()
        {
            
        }

        private void FillRecentReviewsGrid()
        {
           
        }

        private void FavoritesGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }
    }
}
