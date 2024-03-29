//using System;
//using System.Collections.Generic; // no use
//using System.ComponentModel; // no use either, basically already included in the resx
//using System.Data; // again, no use
//using System.Drawing; // still no use, done in resx file
//using System.Text;
//using System.Windows.Forms; // for derivation on the SplashScreen and the Application.Run() function

// Basically no using statements required here as everything is covered by the DLL itself, weird
namespace Yekyaa.FFXIEncoding
{
    partial class SplashScreen : System.Windows.Forms.Form
    {
        #region SplashScreen Variables (publics then privates)
        static public string EncodingVersion = System.String.Empty;
        static public string TextToDisplay = "FFXI Encoding\r\nprovided by Yekyaa";

        private string m_sStatus = System.String.Empty; // "Loading Splash Screen";
        private int m_sProgressCompletion = 0;
        // Fade in and out.
        private double m_dblOpacityIncrement = .10;
        private double m_dblOpacityDecrement = .20;
        private const int TIMER_INTERVAL = 50;
        static private readonly System.Drawing.Image[] _icons = { FFXIEncodingResources.WeaponIcon, FFXIEncodingResources.JapaneseWeapon, FFXIEncodingResources.EnglishWeapon, FFXIEncodingResources.DeutschWeapon, FFXIEncodingResources.FrenchWeapon, FFXIEncodingResources.WeaponIcon };
        static private SplashScreen ms_frmSplash = null;
        static private System.Threading.Thread ms_oThread = null;
        static private int LanguagePreference = FFXIATPhraseLoader.ffxiLanguages.LANG_ENGLISH;
        static private bool visibility = true;
        #endregion

        #region SplashScreen Methods
        #region SplashScreen Methods (static Methods [publics then privates])
        /// <summary>
        /// A static method to create the thread and launch the SplashScreen form.
        /// </summary>
        /// <param name="langpref">FFXIATPhraseLoader.ffxiLanguage value indicating which we're working with.</param>
        static public void ShowSplashScreen(int langpref)
        {
            // Make sure it is only launched once.
            if (ms_frmSplash != null)
                return;
            ms_oThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SplashScreen.ShowForm));
            //ThreadExceptionDialog.CheckForIllegalCrossThreadCalls = false;
            ms_oThread.IsBackground = true;
            ms_oThread.SetApartmentState(System.Threading.ApartmentState.STA);
            ms_oThread.Start((object)langpref);
        }

        /// <summary>
        /// Static method for computing the value needed for a given number of steps.
        /// </summary>
        /// <param name="num">Number of steps we want to take.</param>
        /// <returns>Returns the value needed to increase the ProgressBar value by to match step * value = Maximum</returns>
        static public int ComputeStep(int num)
        {
            if (ms_frmSplash != null)
            {
                int step = ms_frmSplash.progressBar.Maximum / num;
                if ((step * num) > ms_frmSplash.progressBar.Maximum)
                    step -= 1;
                if (step <= 0)
                    step = 1; // deal with it.
                return step;
            }
            return 1;
        }

        /// <summary>
        /// Static method for updating the status label and ProgressBar.
        /// </summary>
        /// <param name="newStatus">String we want displayed in the label.</param>
        /// <param name="progressStep">Amount to increase the ProgressBar value by.</param>
        static public void SetStatus(string newStatus, int progressStep)
        {
            if (ms_frmSplash == null)
                return;
            if (newStatus != System.String.Empty)
                ms_frmSplash.m_sStatus = newStatus;
            ms_frmSplash.m_sProgressCompletion += progressStep;
            if (ms_frmSplash.m_sProgressCompletion < ms_frmSplash.progressBar.Minimum)
                ms_frmSplash.m_sProgressCompletion = ms_frmSplash.progressBar.Minimum;
            else if (ms_frmSplash.m_sProgressCompletion > ms_frmSplash.progressBar.Maximum)
                ms_frmSplash.m_sProgressCompletion = ms_frmSplash.progressBar.Maximum;
        }

        /// <summary>
        /// Static method for updating the status label only.
        /// </summary>
        /// <param name="newStatus">String we want displayed in the label.</param>
        static public void SetStatus(string newStatus)
        {
            if (ms_frmSplash == null)
                return;
            if (newStatus != System.String.Empty)
                ms_frmSplash.m_sStatus = newStatus;
        }

        /// <summary>
        /// Static method for setting the ProgressBar status to the minimum effectively clearing the ProgressBar status.
        /// </summary>
        static public void ClearProgress()
        {
            if (ms_frmSplash != null)
                ms_frmSplash.m_sProgressCompletion = 0;
        }

        /// <summary>
        /// Static method for setting the ProgressBar status to the maximum for showing the end and not just a half-full bar.
        /// </summary>
        static public void EndProgress()
        {
            if (ms_frmSplash != null)
                ms_frmSplash.m_sProgressCompletion = ms_frmSplash.progressBar.Maximum;
        }

        /// <summary>
        /// A static method for closing the SplashScreen.
        /// </summary>
        static public void CloseForm()
        {
            if (ms_frmSplash != null && ms_frmSplash.IsDisposed == false)
            {
                // Make it start going away.
                ms_frmSplash.m_dblOpacityIncrement = -
                   ms_frmSplash.m_dblOpacityDecrement;
            }
            ms_oThread = null;  // we do not need these any more.
            ms_frmSplash = null;
        }

        // A private entry point for the thread.
        /// <summary>
        /// A private entry point for the thread.
        /// </summary>
        /// <param name="langpref">FFXIATPhraseLoader.ffxiLanguage value cast as an object indicating which we're working with.</param>
        static private void ShowForm(object langpref)
        {
            ms_frmSplash = new SplashScreen();

            LanguagePreference = (int)langpref;

            if ((LanguagePreference <= FFXIATPhraseLoader.ffxiLanguages.NUM_LANG_MAX) &&
                (LanguagePreference >= FFXIATPhraseLoader.ffxiLanguages.NUM_LANG_MIN))
                PictureBox = Icons[LanguagePreference];
            else PictureBox = Icons[0];

            ms_frmSplash.programVersion.Text = SplashScreen.TextToDisplay;

            ms_frmSplash.encodingVersion.Text = SplashScreen.EncodingVersion;

            ms_frmSplash.progressBar.Value = 0;

            System.Windows.Forms.Application.Run(ms_frmSplash);
        }
        #endregion

        #region SplashScreen Methods (Event Handler)
        // Tick Event handler for the Timer control.  
        // Handle fade in and fade out.  Also
        // handle the smoothed progress bar.
        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (m_dblOpacityIncrement > 0)
            {
                if (this.Opacity < 1)
                    this.Opacity += m_dblOpacityIncrement;
            }
            else
            {
                if (this.Opacity > 0)
                    this.Opacity += m_dblOpacityIncrement;
                else
                    this.Close();
            }
            label1.Text = m_sStatus;
            try
            {
                if (m_sProgressCompletion > progressBar.Maximum)
                    progressBar.Value = progressBar.Maximum;
                else if (m_sProgressCompletion < progressBar.Minimum)
                    progressBar.Value = progressBar.Minimum;
                else progressBar.Value = m_sProgressCompletion;
            }
            catch (System.ArgumentException)
            {
                progressBar.Value = m_sProgressCompletion;
            }
            progressBar.Visible = visibility;
        }

        private void SplashScreenEncoding_Click(object sender, System.EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"http://forums.windower.net/topic/11409-yekyaas-ffxi-me-v2-offline-macro-editor/");
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    System.Windows.Forms.MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                System.Windows.Forms.MessageBox.Show(other.Message);
            }
        }

        private void FFXITag_Click(object sender, System.EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"http://www.playonline.com/ff11us/index.shtml");
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    System.Windows.Forms.MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                System.Windows.Forms.MessageBox.Show(other.Message);
            }
        }
        #endregion
        #endregion

        #region SplashScreen Properites
        // A property returning the splash screen instance
        static public SplashScreen SplashForm
        {
            get
            {
                return ms_frmSplash;
            }
        }

        static public System.Drawing.Image PictureBox
        {
            set
            {
                if ((ms_frmSplash != null) && (ms_frmSplash.pictureBox1 != null))
                    ms_frmSplash.pictureBox1.BackgroundImage = value;
            }
        }

        static public bool ProgressVisibility
        {
            get
            {
                return visibility;
            }
            set
            {
                visibility = value;
            }
        }

        static public System.Drawing.Image[] Icons
        {
            get { return _icons; }
        }
        #endregion

        #region SplashScreen Constructor
        public SplashScreen()
        {
            InitializeComponent();
            System.Windows.Forms.Application.EnableVisualStyles();
            this.Opacity = .00;
            timer1.Interval = TIMER_INTERVAL;
            if (!timer1.Enabled)
                timer1.Start();
        }
        #endregion
    }
}
