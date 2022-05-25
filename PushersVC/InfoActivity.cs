using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using Google.Android.Material.Snackbar;
using Java.IO;
using Java.Nio;

namespace PushersVC
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class InfoActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (Xamarin.Essentials.AppInfo.RequestedTheme == Xamarin.Essentials.AppTheme.Dark)
            {
                FindViewById<TextView>(Resource.Id.Title).SetTextColor(Android.Graphics.Color.ParseColor("#CF6679"));
                FindViewById<TextView>(Resource.Id.EnglishText).SetTextColor(Android.Graphics.Color.ParseColor("#CF6679"));
                FindViewById<TextView>(Resource.Id.PersianText).SetTextColor(Android.Graphics.Color.ParseColor("#CF6679"));
                FindViewById<TextView>(Resource.Id.CreatorID).SetTextColor(Android.Graphics.Color.ParseColor("#CF6679"));

                FindViewById<LinearLayout>(Resource.Id.sideNavbarHeaderBox).Background = GetDrawable(Resource.Drawable.side_nav_bar_dark);
                FindViewById<TextView>(Resource.Id.sideNavbarHeaderText).SetTextColor(Android.Graphics.Color.ParseColor("#CF6679"));
                FindViewById<TextView>(Resource.Id.sideNavbarHeaderSubText).SetTextColor(Android.Graphics.Color.ParseColor("#CF6679"));
            }
            else
            {
                FindViewById<TextView>(Resource.Id.Title).SetTextColor(Android.Graphics.Color.ParseColor("#99f2c8"));
                FindViewById<TextView>(Resource.Id.EnglishText).SetTextColor(Android.Graphics.Color.ParseColor("#99f2c8"));
                FindViewById<TextView>(Resource.Id.PersianText).SetTextColor(Android.Graphics.Color.ParseColor("#99f2c8"));
                FindViewById<TextView>(Resource.Id.CreatorID).SetTextColor(Android.Graphics.Color.ParseColor("#99f2c8"));

                FindViewById<LinearLayout>(Resource.Id.sideNavbarHeaderBox).Background = GetDrawable(Resource.Drawable.side_nav_bar_light);
                FindViewById<TextView>(Resource.Id.sideNavbarHeaderText).SetTextColor(Android.Graphics.Color.ParseColor("#99f2c8"));
                FindViewById<TextView>(Resource.Id.sideNavbarHeaderSubText).SetTextColor(Android.Graphics.Color.ParseColor("#99f2c8"));
            }
            return true;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            #region UI
            if (Xamarin.Essentials.AppInfo.RequestedTheme == Xamarin.Essentials.AppTheme.Dark)
            {
                int[][] states = new int[][] {
                     new int[] {Android.Resource.Attribute.StateEnabled} }; // enabled
                int[] colors = new int[]
                        {Android.Graphics.Color.ParseColor("#CF6679")};

                Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#CF6679"));
                Window.SetNavigationBarColor(Android.Graphics.Color.Transparent);
            }
            else
            {
                Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#99f2c8"));
                Window.SetNavigationBarColor(Android.Graphics.Color.White);
            }
            #endregion
            #region Events
            SetContentView(Resource.Layout.activity_info);
            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);
        }
        #endregion
        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }
        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            int id = menuItem.ItemId;

            if (id == Resource.Id.nav_record)
            {
                Intent intent = new Intent(this, typeof(MainActivity));

                StartActivity(intent);
            }
            if (id == Resource.Id.nav_info)
            {
                Intent intent = new Intent(this, typeof(InfoActivity));

                StartActivity(intent);
            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
    }
}