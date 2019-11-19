using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Team7GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RunBusinessStuff();
            ChangeGridStates(false);
        }

        private Users CurrentUser { get; set; }

        /// <summary>
        /// Populates the users results based on search parameter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectSearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear results and disable information sections until a valid result chosen
            ChangeGridStates(false);
            selectUserListBox.ClearValue(ItemsControl.ItemsSourceProperty);
            selectUserListBox.ItemsSource = Query.SearchUsers(selectUserSearchTextBox.Text);

            // If search name was not found
            if (selectUserListBox.Items.Count == 0)
            {
                selectUserListBox.Items.Add("No results found");
            }
        }

        /// <summary>
        /// Opens a map of the current users location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserShowMapButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectUserListBox.SelectedItem is Users users)
            {
                new MapData(users);
            }
        }

        /// <summary>
        /// Clears the default text in the search box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectUserSearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Clear the "<search a user>" dialogue
            selectUserSearchTextBox.Clear();
        }

        /// <summary>
        /// Actor selects a user and that currentUser populates all the fields and grids
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectUserListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Result must be selected to populate. If user selects the 'No results found', will not trigger
            if (selectUserListBox.SelectedItem is Users users)
            {
                CurrentUser = users;
                ChangeGridStates(true);
                FillUserInformation(users);
                FillFavoriteBusinessGrid(users);
                FillFriendsInformation(users);
                FillRecentReviewsGrid(users);
                FillFriendsReviewsGrid(users);
            }
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
        private void FillUserInformation(Users currentUser)
        {
            userName.Text = currentUser.Name;
            userStars.Text = currentUser.Averagestars.ToString("0.00");
            userFans.Text = currentUser.Fans.ToString();
            userYelpingSince.Text = currentUser.Yelpingsince;
            userFunny.Text = currentUser.Funny.ToString();
            userCool.Text = currentUser.Cool.ToString();
            userUseful.Text = currentUser.Useful.ToString();
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
        private void FillFriendsInformation(Users currentUser)
        {
            friendsGrid.ClearValue(ItemsControl.ItemsSourceProperty);
            friendsGrid.ItemsSource = Query.FriendsInformation(currentUser);
        }

        /// <summary>
        /// Populate favorite business grid
        /// </summary>
        /// <param name="currentUser"></param>
        private void FillFavoriteBusinessGrid(Users currentUser)
        {
            favoritesGrid.ClearValue(ItemsControl.ItemsSourceProperty);
            favoritesGrid.ItemsSource = Query.FavoriteBusiness(currentUser);
        }

        /// <summary>
        /// Populate the Friends Reviews
        /// </summary>
        /// <param name="currentUser"></param>
        private void FillFriendsReviewsGrid(Users currentUser)
        {
            friendsReviewsGrid.ClearValue(ItemsControl.ItemsSourceProperty);
            friendsReviewsGrid.ItemsSource = Query.FriendsReviews(currentUser);
        }

        /// <summary>
        /// Populates the Recent Reviews
        /// </summary>
        /// <param name="currentUser"></param>
        private void FillRecentReviewsGrid(Users currentUser)
        {
            recentReviewsGrid.ClearValue(ItemsControl.ItemsSourceProperty);
            recentReviewsGrid.ItemsSource = Query.RecentReviews(currentUser);
        }

        /// <summary>
        /// Opens Map of business that was double clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FavoritesGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (favoritesGrid.SelectedItem is Business currentBusiness)
            {
                new MapData(currentBusiness);
            }
        }

        #region businesssection

        private Users thisUser = new Users();

        private void RunBusinessStuff()
        {
            TemporaryMethodDeleteMeWhenFullyImplemented();
            AddStates();
            AddColumns();
            thisUser = tempUserBox.SelectedItem as Users;
        }

        public static string buildConnectionString()
        {
            return "Host=localhost; Username=postgres; Password=team7; Database=milestone2db";
        }

        private void TemporaryMethodDeleteMeWhenFullyImplemented()
        {
            tempUserBox.ClearValue(ItemsControl.ItemsSourceProperty);
            tempUserBox.ItemsSource = Query.BaseQuery("users", new Dictionary<string, string>() { { "userid", "om5ZiponkpRqUNa3pVPiRg" } });

            // TEMPORARY AREA
            tempUserBox.SelectedIndex = 0;

            Users thisUser = tempUserBox.SelectedItem as Users;
            CurrentUser = thisUser;
        }

        /// <summary>
        /// Populates the State dropdown menu
        /// </summary>
        private void AddStates()
        {
            stateBox.ClearValue(ItemsControl.ItemsSourceProperty);
            stateBox.ItemsSource = Query.BusinessStates();
        }

        /// <summary>
        /// Creates all necessary columns for the search results of businesses
        /// </summary>
        private void AddColumns()
        {
            #region business columns

            // Columns for the business grid
            businessGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Business Name",
                Binding = new Binding("Name"),
                Width = 200
            });

            businessGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Address",
                Binding = new Binding("Address"),
                Width = 195
            });

            businessGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "City",
                Binding = new Binding("City"),
                Width = 80
            });

            businessGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "State",
                Binding = new Binding("State"),
                Width = 35
            });

            businessGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Distance \n(miles)",
                Binding = new Binding("Distance"),
                Width = 50
            });

            businessGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Stars",
                Binding = new Binding("Stars"),
                Width = 32
            });

            businessGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "# Of \nReviews",
                Binding = new Binding("Reviewcount"),
                Width = 40
            });

            businessGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Avg \nReview \nRating",
                Binding = new Binding("Reviewrating"),
                Width = 40
            });

            businessGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Total \nCheckins",
                Binding = new Binding("Numcheckins"),
            });

            #endregion business columns

            #region friends reviews columns

            // Make columns for friends reviews
            friendsReviews.Columns.Add(new DataGridTextColumn
            {
                Header = "Name",
                Binding = new Binding("Username"),
                Width = 100
            });

            friendsReviews.Columns.Add(new DataGridTextColumn
            {
                Header = "Date",
                Binding = new Binding("Reviewdate"),
                Width = 75
            });

            // Allows the reviewtext section to wrap
            var style = new Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
            friendsReviews.Columns.Add(new DataGridTextColumn
            {
                Header = "Text",
                Binding = new Binding("Reviewtext"),
                ElementStyle = style
            });

            #endregion friends reviews columns
        }

        /// <summary>
        /// When a state is chosen on the dropdown combobox
        /// </summary>
        /// <param name="sender">N/a</param>
        /// <param name="e">N/a</param>
        private void StateBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string currentState = stateBox.SelectedItem.ToString();

            cityBox.ClearValue(ItemsControl.ItemsSourceProperty);
            zipBox.ClearValue(ItemsControl.ItemsSourceProperty);

            cityBox.ItemsSource = Query.BusinessCities(currentState);
        }

        /// <summary>
        /// When a user selects a city
        /// </summary>
        /// <param name="sender">N/a</param>
        /// <param name="e">N/a</param>
        private void CityBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            zipBox.ClearValue(ItemsControl.ItemsSourceProperty);
            numberOfBusinesses.Text = string.Empty;

            // Since changing state needs to clear current contents of the city box then repopulate, this event triggers when clearing
            if (cityBox.Items.IsEmpty)
            {
                return;
            }

            string currentState = stateBox.SelectedItem.ToString();
            string currentCity = cityBox.SelectedItem.ToString();
            zipBox.ItemsSource = Query.BusinessZipCodes(currentState, currentCity);
        }

        /// <summary>
        /// When user selects a zip code
        /// </summary>
        /// <param name="sender">N/a</param>
        /// <param name="e">N/a</param>
        private void ZipBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            categoryBox.ClearValue(ItemsControl.ItemsSourceProperty);
            businessGrid.ClearValue(ItemsControl.ItemsSourceProperty);

            // Since changing city needs to clear current contents of the zip box then repopulate, this event triggers when clearing
            if (zipBox.Items.IsEmpty)
            {
                return;
            }

            string currentState = stateBox.SelectedItem.ToString();
            string currentCity = cityBox.SelectedItem.ToString();
            string currentZipCode = zipBox.SelectedItem.ToString();
            businessGrid.ItemsSource = Query.BusinessList(currentState, currentCity, currentZipCode);
            categoryBox.ItemsSource = Query.BusinessCategories(currentState, currentCity, currentZipCode);

            numberOfBusinesses.Text = "Number of Businesses found: " + businessGrid.Items.Count.ToString();
            SelectedCategoryCount.Text = categoryBox.SelectedItems.Count.ToString();
        }

        /// <summary>
        /// When user selects a business on the search results
        /// </summary>
        /// <param name="sender">N/a</param>
        /// <param name="e">N/a</param>
        private void BusinessGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            friendsReviews.ClearValue(ItemsControl.ItemsSourceProperty);
            if (businessGrid.SelectedItem is Business selectedBusiness)
            {
                Dictionary<string, string> businessInformation = Query.BusinessInformation(selectedBusiness);

                // Activates the business information area. Stays gray until a proper business is selected
                businessInfoGroup.IsEnabled = true;
                businessInfoBox.Text = new StringBuilder().AppendLine(selectedBusiness.Name).AppendLine(selectedBusiness.Address).AppendLine(selectedBusiness.City + ", " + selectedBusiness.State + " " + selectedBusiness.Zipcode).ToString();
                businessAttributesTextBox.Text = businessInformation["attributes"];
                businessHoursTextBox.Text = businessInformation["hours"];
                businessCategoriesTextBox.Text = businessInformation["categories"];
                friendsReviews.ItemsSource = Query.BusinessFriendsReviews(selectedBusiness, CurrentUser);
            }

            // Means the business grid was cleared (such as changing state or city), so we deactivate the business info area
            else
            {
                businessInfoGroup.IsEnabled = false;
                businessInfoBox.Text = string.Empty;
                businessAttributesTextBox.Text = string.Empty;
                businessHoursTextBox.Text = string.Empty;
                businessCategoriesTextBox.Text = string.Empty;
            }

            // Restore visible elements to default
            friendsReviews.Visibility = Visibility.Visible;
            writeReviewGroup.Visibility = Visibility.Hidden;
            writeReviewButton.Content = "Write a Review";
        }

        /// <summary>
        /// When a category filter is added or removed
        /// </summary>
        /// <param name="sender">N/a</param>
        /// <param name="e">N/a</param>
        private void CategoryBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            businessGrid.ClearValue(ItemsControl.ItemsSourceProperty);

            string filters = string.Empty;
            if (categoryBox.SelectedItems.Count > 0)
            {
                string currentState = stateBox.SelectedItem.ToString();
                string currentCity = cityBox.SelectedItem.ToString();
                string currentZipCode = zipBox.SelectedItem.ToString();
                List<string> filterList = new List<string>();

                foreach (string item in categoryBox.SelectedItems)
                {
                    filterList.Add(item);
                }

                businessGrid.ItemsSource = Query.UpdateBusinessGrid(currentState, currentCity, currentZipCode, filterList);

                List<string> categories = Query.UpdateCategoriesList(currentState, currentCity, currentZipCode, filterList, e);

                IList list = (IList)categoryBox.ItemsSource;

                if (e.AddedItems.Count > 0)
                {
                    foreach (string category in categories)
                    {
                        if (categoryBox.Items.Contains(category))
                        {
                            list.Remove(category);
                        }
                    }
                }
                else if (e.RemovedItems.Count > 0)
                {
                    foreach (string category in categories)
                    {
                        if (!categoryBox.Items.Contains(category))
                        {
                            list.Add(category);
                        }
                    }
                }

                categoryBox.Items.Refresh();
            }

            // All category filters removed, rather than re-writing an entire SQL query,
            // I just have it pretend a zipcode change event happened to invoke a reinitialization of
            // the main grid with currently selected state, city, and zip.
            else
            {
                ZipBox_SelectionChanged(sender, e);
            }
            numberOfBusinesses.Text = "Number of Businesses found: " + businessGrid.Items.Count.ToString();
            SelectedCategoryCount.Text = categoryBox.SelectedItems.Count.ToString();
        }

        /// <summary>
        /// The Write Review button simply changes the interface to show a textbox and rating dropdown. Same button will go back to default
        /// </summary>
        /// <param name="sender">N/a</param>
        /// <param name="e">N/a</param>
        private void WriteReviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (writeReviewButton.Content.ToString() == "Write a Review")
            {
                friendsReviews.Visibility = Visibility.Hidden;
                writeReviewGroup.Visibility = Visibility.Visible;
                writeReviewButton.Content = "      Back to\nFriends Reviews";

                // Logic to take what is written in the reviewBox and upload as a review
            }
            else
            {
                writeReviewBox.Clear();
                ratingComboBox.SelectedIndex = 0;
                friendsReviews.Visibility = Visibility.Visible;
                writeReviewGroup.Visibility = Visibility.Hidden;
                writeReviewButton.Content = "Write a Review";
            }
        }

        private void SubmitReviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(writeReviewBox.Text))
            {
                MessageBox.Show("Review must have a comment", "Comment required");
                return;
            }
            if (ratingComboBox.SelectedIndex == 0)
            {
                MessageBox.Show("Review must have a rating", "Rating required");
                return;
            }

            Review review = new Review
            {
                Userid = CurrentUser.Userid,
                Businessid = ((Business)businessGrid.SelectedItem).Businessid,
                Stars = Convert.ToDouble(ratingComboBox.Text),
                Reviewtext = writeReviewBox.Text
            };

            try
            {
                Query.SubmitReview(review);
            }
            catch
            {
                MessageBox.Show("Review submission failure", "Failure");
            }

            // Stores current business selected
            int currentBusinessIndex = businessGrid.SelectedIndex;

            // Invoke a refresh of businessgrid
            ZipBox_SelectionChanged(null, null);

            // Restore business selected after submitting review
            businessGrid.SelectedIndex = currentBusinessIndex;

            MessageBox.Show("Successfully submitted review", "Success!");
        }

        /// <summary>
        /// Open Reviews window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowReviewsButton_Click(object sender, RoutedEventArgs e)
        {
            Reviews reviewpage = new Reviews(((Business)businessGrid.SelectedItem).Businessid)
            {
                Owner = this
            };
            reviewpage.Show();
        }

        /// <summary>
        /// Checks into the business
        /// </summary>
        /// <param name="sender">N/a</param>
        /// <param name="e">N/a</param>
        private void CheckInButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Query.CheckIn(((Business)businessGrid.SelectedItem).Businessid);
            }
            catch
            {
                MessageBox.Show("Failed to check in!", "Failure!");
            }

            // Stores current business selected
            int currentBusinessIndex = businessGrid.SelectedIndex;

            // Invoke a refresh of businessgrid
            ZipBox_SelectionChanged(null, null);

            // Restore business selected after checking in
            businessGrid.SelectedIndex = currentBusinessIndex;

            MessageBox.Show("Checked in Successfully!", "Success!");
        }

        #endregion businesssection

        private void ResetCategories_Click(object sender, RoutedEventArgs e)
        {
            categoryBox.UnselectAll();
        }
    }
}