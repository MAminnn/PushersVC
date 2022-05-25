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
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        Recorder audioRecorder = new Recorder();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
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
            #region Permisions
            try
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.RecordAudio) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.RecordAudio }, 1);
                }
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ManageExternalStorage) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.RecordAudio }, 2);
                }
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.RecordAudio }, 3);
                }
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.RecordAudio }, 4);
                }
            }
            catch (Exception)
            {

            }
            #endregion
            #region SetEvents

            SetContentView(Resource.Layout.activity_main);
            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            FindViewById<Button>(Resource.Id.btnStart).Click += this.StartClicked;
            FindViewById<Button>(Resource.Id.btnStop).Click += this.StopClicked;
            FindViewById<Button>(Resource.Id.HalfXCheckBox).Click += this.CheckAudioSpeedSelection;
            FindViewById<Button>(Resource.Id.Speed2XCheckBox).Click += this.CheckAudioSpeedSelection;
            FindViewById<Button>(Resource.Id.btn_Save).Click += this.SaveClicked;
            FindViewById<Button>(Resource.Id.btn_Discard).Click += this.DiscardClicked;
            FindViewById<Button>(Resource.Id.btnPlay).Click += this.PlayClicked;
            FindViewById<CheckBox>(Resource.Id.EchoCheckBox).Click += this.CheckAudioWindowStlye;
            FindViewById<CheckBox>(Resource.Id.NoiseCheckBox).Click += this.CheckAudioWindowStlye;

            FindViewById<Button>(Resource.Id.btn_Save).Enabled = false;
            FindViewById<Button>(Resource.Id.btn_Discard).Enabled = false;


            if (Xamarin.Essentials.AppInfo.RequestedTheme == Xamarin.Essentials.AppTheme.Dark)
            {

                Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#CF6679"));
                Window.SetNavigationBarColor(Android.Graphics.Color.Rgb(27, 27, 27));
                int[][] states = new int[][] {
                     new int[] {Android.Resource.Attribute.StateEnabled}, // enabled
                     new int[] {Android.Resource.Attribute.StateActive} };
                int[] colors = new int[]
                        {Android.Graphics.Color.ParseColor("#CF6679"),
                        Android.Graphics.Color.ParseColor("#CF6679")};
                for (int i = 0; i < FindViewById<RelativeLayout>(Resource.Id.btnContainer).ChildCount; i++)
                {
                    ((MaterialButton)FindViewById<RelativeLayout>(Resource.Id.btnContainer).GetChildAt(i)).SetBackgroundColor(Android.Graphics.Color.Transparent);
                    ((MaterialButton)FindViewById<RelativeLayout>(Resource.Id.btnContainer).GetChildAt(i)).SetTextColor(Android.Graphics.Color.ParseColor("#CF6679"));
                    ((MaterialButton)FindViewById<RelativeLayout>(Resource.Id.btnContainer).GetChildAt(i)).SetOutlineSpotShadowColor(Android.Graphics.Color.ParseColor("#CF6679"));
                }
                for (int i = 0; i < FindViewById<RelativeLayout>(Resource.Id.btnContainer2).ChildCount; i++)
                {
                    ((MaterialButton)FindViewById<RelativeLayout>(Resource.Id.btnContainer2).GetChildAt(i)).BackgroundTintList = new Android.Content.Res.ColorStateList(states, colors);
                    ((MaterialButton)FindViewById<RelativeLayout>(Resource.Id.btnContainer2).GetChildAt(i)).SetTextColor(Android.Graphics.Color.Black);
                    ((MaterialButton)FindViewById<RelativeLayout>(Resource.Id.btnContainer2).GetChildAt(i)).SetOutlineSpotShadowColor(Android.Graphics.Color.ParseColor("#CF6679"));
                }
                FindViewById<SeekBar>(Resource.Id.PitchSeekBar).BackgroundTintList = new Android.Content.Res.ColorStateList(states, colors);
                FindViewById<SeekBar>(Resource.Id.PitchSeekBar).ProgressTintList = new Android.Content.Res.ColorStateList(states, colors);
                FindViewById<SeekBar>(Resource.Id.PitchSeekBar).ThumbTintList = new Android.Content.Res.ColorStateList(states, colors);
                for (int i = 0; i < FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer1).ChildCount; i++)
                {
                    var demo = ((CheckBox)FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer1).GetChildAt(i)).ButtonTintList = new Android.Content.Res.ColorStateList(states, colors);
                    ((CheckBox)FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer1).GetChildAt(i)).SetTextColor(Android.Graphics.Color.ParseColor("#CF6679"));
                }
                for (int i = 0; i < FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer2).ChildCount; i++)
                {
                    var demo = ((CheckBox)FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer2).GetChildAt(i)).ButtonTintList = new Android.Content.Res.ColorStateList(states, colors);
                    ((CheckBox)FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer2).GetChildAt(i)).SetTextColor(Android.Graphics.Color.ParseColor("#CF6679"));
                }
                for (int i = 0; i < FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer3).ChildCount; i++)
                {
                    var demo = ((CheckBox)FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer3).GetChildAt(i)).ButtonTintList = new Android.Content.Res.ColorStateList(states, colors);
                    ((CheckBox)FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer3).GetChildAt(i)).SetTextColor(Android.Graphics.Color.ParseColor("#CF6679"));
                }
            }
            else
            {
                Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#99f2c8"));
                Window.SetNavigationBarColor(Android.Graphics.Color.Rgb(27, 27, 27));

                int[][] states = new int[][] {
                     new int[] {Android.Resource.Attribute.StateEnabled} }; // enabled
                int[] colors = new int[]
                        {Android.Graphics.Color.ParseColor("#99f2c8")};

                for (int i = 0; i < FindViewById<RelativeLayout>(Resource.Id.btnContainer).ChildCount; i++)
                {
                    ((MaterialButton)FindViewById<RelativeLayout>(Resource.Id.btnContainer).GetChildAt(i)).SetBackgroundColor(Android.Graphics.Color.Transparent);
                    ((MaterialButton)FindViewById<RelativeLayout>(Resource.Id.btnContainer).GetChildAt(i)).SetTextColor(Android.Graphics.Color.ParseColor("#99f2c8"));
                    ((MaterialButton)FindViewById<RelativeLayout>(Resource.Id.btnContainer).GetChildAt(i)).SetOutlineSpotShadowColor(Android.Graphics.Color.ParseColor("#99f2c8"));
                }
                for (int i = 0; i < FindViewById<RelativeLayout>(Resource.Id.btnContainer2).ChildCount; i++)
                {
                    ((MaterialButton)FindViewById<RelativeLayout>(Resource.Id.btnContainer2).GetChildAt(i)).BackgroundTintList = new Android.Content.Res.ColorStateList(states, colors);
                    ((MaterialButton)FindViewById<RelativeLayout>(Resource.Id.btnContainer2).GetChildAt(i)).SetTextColor(Android.Graphics.Color.White);
                    ((MaterialButton)FindViewById<RelativeLayout>(Resource.Id.btnContainer2).GetChildAt(i)).SetOutlineSpotShadowColor(Android.Graphics.Color.ParseColor("#99f2c8"));
                }

                FindViewById<SeekBar>(Resource.Id.PitchSeekBar).BackgroundTintList = new Android.Content.Res.ColorStateList(states, colors);
                FindViewById<SeekBar>(Resource.Id.PitchSeekBar).ProgressTintList = new Android.Content.Res.ColorStateList(states, colors);
                FindViewById<SeekBar>(Resource.Id.PitchSeekBar).ThumbTintList = new Android.Content.Res.ColorStateList(states, colors);
                for (int i = 0; i < FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer1).ChildCount; i++)
                {
                    var demo = ((CheckBox)FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer1).GetChildAt(i)).ButtonTintList = new Android.Content.Res.ColorStateList(states, colors);
                    ((CheckBox)FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer1).GetChildAt(i)).SetTextColor(Android.Graphics.Color.ParseColor("#99f2c8"));
                }
                for (int i = 0; i < FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer2).ChildCount; i++)
                {
                    var demo = ((CheckBox)FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer2).GetChildAt(i)).ButtonTintList = new Android.Content.Res.ColorStateList(states, colors);
                    ((CheckBox)FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer2).GetChildAt(i)).SetTextColor(Android.Graphics.Color.ParseColor("#99f2c8"));
                }
                for (int i = 0; i < FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer3).ChildCount; i++)
                {
                    var demo = ((CheckBox)FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer3).GetChildAt(i)).ButtonTintList = new Android.Content.Res.ColorStateList(states, colors);
                    ((CheckBox)FindViewById<RelativeLayout>(Resource.Id.CheckBoxContainer3).GetChildAt(i)).SetTextColor(Android.Graphics.Color.ParseColor("#99f2c8"));
                }
            }
            #endregion
        }
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
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (Xamarin.Essentials.AppInfo.RequestedTheme == Xamarin.Essentials.AppTheme.Dark)
            {
                FindViewById<LinearLayout>(Resource.Id.sideNavbarHeaderBox).Background = GetDrawable(Resource.Drawable.side_nav_bar_dark);
                FindViewById<TextView>(Resource.Id.sideNavbarHeaderText).SetTextColor(Android.Graphics.Color.ParseColor("#CF6679"));
                FindViewById<TextView>(Resource.Id.sideNavbarHeaderSubText).SetTextColor(Android.Graphics.Color.ParseColor("#CF6679"));
                FindViewById<FloatingActionButton>(Resource.Id.fab).SetImageDrawable(Resources.GetDrawable(Resource.Drawable.ic_dialog_info_dark));
            }
            else
            {
                FindViewById<LinearLayout>(Resource.Id.sideNavbarHeaderBox).Background = GetDrawable(Resource.Drawable.side_nav_bar_light);
                FindViewById<TextView>(Resource.Id.sideNavbarHeaderText).SetTextColor(Android.Graphics.Color.ParseColor("#99f2c8"));
                FindViewById<TextView>(Resource.Id.sideNavbarHeaderSubText).SetTextColor(Android.Graphics.Color.ParseColor("#99f2c8"));
                FindViewById<FloatingActionButton>(Resource.Id.fab).SetImageDrawable(Resources.GetDrawable(Resource.Drawable.ic_dialog_info_light));
            }
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }
        private void FabOnClick(object sender, EventArgs eventArgs)
        {

            Android.App.AlertDialog.Builder builder1 = new Android.App.AlertDialog.Builder(this);
            builder1.SetMessage("PitchShifting: \n تعیین کننده ی تُن صدا ، هرچه راست تر ، صدا نازک تر \n Noisy: \n بر روی صدای طبیعی اعمال نمیشود ، ولی بر روی صدا هایی با تُن تغییر یافته نویز می اندازد ، ترکیب " +
                "این افکت با نازک ترین صدای ممکن یک ترکیب کاربردی و سمی است \n Robatic: \n صدایی همانند صدای بریده بریده ربات را اعمال میکند \n Wavy: \n صدایی موجی و متلاطم \n Echoing: \n صدایی اکویی ، انگار از ته چا صدا می آید \n \n \n توجه : از اعمال همزمان چندین افکت خودداری کنید زیرا نتیجه بر خلاف تصورتان جالب نخواهد بود "
                + "\n\n\n برای استفاده از صدای خروجی در شبکه های اجتماعی مراحل زیر را انجام دهید : \n\n فایل خروجی در صورتی که دکمه ی save را فشار دهید در پوشه ی دانلود ها قرار می گیرد ، حال برای استفاده در تلگرام ، در قسمت تایپ پیام سنجاقک کنار را فشار دهید و قسمت file را انتخاب نمایید ، فایل مورد نظر پایین تر در قسمت دسته بندی فایل ها قرار گرفته است");
            builder1.SetNeutralButton("اوکب", (sender, e) =>
             {

             });
            Android.App.AlertDialog alert11 = builder1.Create();
            alert11.Show();
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

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
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        private void StartClicked(object sender, EventArgs e)
        {
            FindViewById<Button>(Resource.Id.btnStart).Text = "Recording";
            audioRecorder.Record();
        }
        private void StopClicked(object sender, EventArgs e)
        {
            float shift = FindViewById<SeekBar>(Resource.Id.PitchSeekBar).Progress;
            bool hasnoise = FindViewById<CheckBox>(Resource.Id.NoiseCheckBox).Checked;
            bool hasecho = FindViewById<CheckBox>(Resource.Id.EchoCheckBox).Checked;
            bool robotized = FindViewById<CheckBox>(Resource.Id.RobotizedCheckBox).Checked;
            bool Wavy = FindViewById<CheckBox>(Resource.Id.WavyCheckBox).Checked;
            bool Speed2X = FindViewById<CheckBox>(Resource.Id.Speed2XCheckBox).Checked;
            bool SpeedHalfX = FindViewById<CheckBox>(Resource.Id.HalfXCheckBox).Checked;
            Tuple<float, int, bool, bool, bool, bool> audioParams;
            if (hasnoise)
            {
                audioParams = new Tuple<float, int, bool, bool, bool, bool>(shift, 128, robotized, Wavy, Speed2X, SpeedHalfX);
            }
            else if (hasecho)
            {
                audioParams = new Tuple<float, int, bool, bool, bool, bool>(shift, 8192, robotized, Wavy, Speed2X, SpeedHalfX);
            }
            else
            {
                audioParams = new Tuple<float, int, bool, bool, bool, bool>(shift, 2048, robotized, Wavy, Speed2X, SpeedHalfX);
            }
            audioRecorder.Stop(audioParams);
            FindViewById<Button>(Resource.Id.btnStart).Enabled = false;
            FindViewById<Button>(Resource.Id.btnStop).Enabled = false;
            FindViewById<Button>(Resource.Id.btn_Save).Enabled = true;
            FindViewById<Button>(Resource.Id.btn_Discard).Enabled = true;

            FindViewById<Button>(Resource.Id.btnStart).Text = "Start";
        }
        private void PlayClicked(object sender, EventArgs e)
        {
            audioRecorder.Play();
        }
        private void DiscardClicked(object sender, EventArgs e)
        {
            if (audioRecorder._audioName != null)
            {
                try
                {
                    Java.IO.File file = new Java.IO.File(audioRecorder._audioPath);
                    file.Delete();
                    FindViewById<Button>(Resource.Id.btnStart).Enabled = true;
                    FindViewById<Button>(Resource.Id.btnStop).Enabled = true;
                    FindViewById<Button>(Resource.Id.btn_Save).Enabled = false;
                    FindViewById<Button>(Resource.Id.btn_Discard).Enabled = false;
                }
                catch (Exception)
                {

                }
            }
        }
        private void SaveClicked(object sender, EventArgs e)
        {
            if (audioRecorder._audioName != null)
            {
                try
                {

                    //Java.IO.FileInputStream file = new FileInputStream(audioRecorder._audioPath);
                    //Java.IO.FileOutputStream fileOutput = new FileOutputStream("/storage/emulated/0/Download/" + Path.ChangeExtension(audioRecorder._audioName, "ogg"));
                    //for (int i = 0; i < file.Available(); i++)
                    //{
                    //    fileOutput.Write(file.Read());
                    //}
                    NWaves.Audio.WaveFile audioFile = new NWaves.Audio.WaveFile(new FileStream(audioRecorder._audioPath, FileMode.OpenOrCreate));
                    audioFile.SaveTo(new FileStream(Path.ChangeExtension(audioRecorder._audioPath, "ogg"), FileMode.OpenOrCreate));
                    Java.IO.File afile = new Java.IO.File(audioRecorder._audioPath);
                    afile.Delete();
                    FindViewById<Button>(Resource.Id.btnStart).Enabled = true;
                    FindViewById<Button>(Resource.Id.btnStop).Enabled = true;
                    FindViewById<Button>(Resource.Id.btn_Save).Enabled = false;
                    FindViewById<Button>(Resource.Id.btn_Discard).Enabled = false;
                }
                catch (Exception)
                {

                }
            }
        }
        private void CheckAudioWindowStlye(object sender, EventArgs e)
        {
            CheckBox senderObj = (CheckBox)sender;
            if (senderObj.Id == Resource.Id.EchoCheckBox)
            {
                FindViewById<CheckBox>(Resource.Id.NoiseCheckBox).Checked = false;
            }
            if (senderObj.Id == Resource.Id.NoiseCheckBox)
            {
                FindViewById<CheckBox>(Resource.Id.EchoCheckBox).Checked = false;
            }
        }
        private void CheckAudioSpeedSelection(object sender, EventArgs e)
        {
            CheckBox senderObj = (CheckBox)sender;
            if (senderObj.Id == Resource.Id.Speed2XCheckBox)
            {
                FindViewById<CheckBox>(Resource.Id.HalfXCheckBox).Checked = false;
            }
            if (senderObj.Id == Resource.Id.HalfXCheckBox)
            {
                FindViewById<CheckBox>(Resource.Id.Speed2XCheckBox).Checked = false;
            }
        }
    }
}

