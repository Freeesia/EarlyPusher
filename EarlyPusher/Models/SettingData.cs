using System;
using System.Collections.ObjectModel;
using System.IO;
using SFLibs.Core.Basis;

namespace EarlyPusher.Models
{
    [Serializable]
    public class SettingData : ObservableObject
    {
        public const string FileName = "conf.xml";
        private ObservableKeyedCollection<string, ChoiceOrderMediaData> choiceOrderMediaList = new ObservableKeyedCollection<string, ChoiceOrderMediaData>(m => m.MediaPath);
        private BinkanData binkan = new BinkanData();
        private EarlyData early = new EarlyData();
        private string choiceVideoDir;
        private string sortVideoDir;
        private string standSoundPath;
        private string questionSoundPath;
        private string answerSoundPath;
        private string correctSoundPath;
        private string missSoundPath;
        private string checkSoundPath;
        private string timerImagePath;
        private string correctImagePath;
        private string maskImagePath;
        private string backImagePath;
        private string cameraDevice;
        private string timeshockBgmPath;
        private string timeshockCorrectSoundPath;

        public ObservableCollection<TeamData> TeamList { get; } = new ObservableCollection<TeamData>();

        /// <summary>
        /// ビンカン用設定
        /// </summary>
        public BinkanData Binkan
        {
            get { return this.binkan; }
            set { SetProperty(ref this.binkan, value); }
        }

        /// <summary>
        /// 早押し用設定
        /// </summary>
        public EarlyData Early
        {
            get { return this.early; }
            set { SetProperty(ref this.early, value); }
        }

        public ObservableKeyedCollection<string, ChoiceOrderMediaData> ChoiceOrderMediaList
        {
            get { return choiceOrderMediaList; }
        }

        public string ChoiceVideoDir
        {
            get { return this.choiceVideoDir; }
            set { SetProperty(ref this.choiceVideoDir, value); }
        }

        public string SortVideoDir
        {
            get { return this.sortVideoDir; }
            set { SetProperty(ref this.sortVideoDir, value, SortVideoDirChanged); }
        }

        public string StandSoundPath
        {
            get { return this.standSoundPath; }
            set { SetProperty(ref this.standSoundPath, value); }
        }

        public string QuestionSoundPath
        {
            get { return this.questionSoundPath; }
            set { SetProperty(ref this.questionSoundPath, value); }
        }

        public string AnswerSoundPath
        {
            get { return answerSoundPath; }
            set { SetProperty(ref answerSoundPath, value); }
        }

        public string CorrectSoundPath
        {
            get { return this.correctSoundPath; }
            set { SetProperty(ref this.correctSoundPath, value); }
        }

        public string MissSoundPath
        {
            get { return this.missSoundPath; }
            set { SetProperty(ref this.missSoundPath, value); }
        }

        public string CheckSoundPath
        {
            get { return this.checkSoundPath; }
            set { SetProperty(ref this.checkSoundPath, value); }
        }

        public string TimerImagePath
        {
            get { return this.timerImagePath; }
            set { SetProperty(ref this.timerImagePath, value); }
        }

        public string CorrectImagePath
        {
            get { return this.correctImagePath; }
            set { SetProperty(ref this.correctImagePath, value); }
        }

        public string MaskImagePath
        {
            get { return this.maskImagePath; }
            set { SetProperty(ref this.maskImagePath, value); }
        }

        public string BackImagePath
        {
            get { return this.backImagePath; }
            set { SetProperty(ref this.backImagePath, value); }
        }

        public string CameraDevice
        {
            get { return this.cameraDevice; }
            set { SetProperty(ref this.cameraDevice, value); }
        }

        public string TimeshockBgmPath
        {
            get { return this.timeshockBgmPath; }
            set { SetProperty(ref this.timeshockBgmPath, value); }
        }

        public string TimeshockCorrectSoundPath
        {
            get { return this.timeshockCorrectSoundPath; }
            set { SetProperty(ref this.timeshockCorrectSoundPath, value); }
        }

        private void SortVideoDirChanged()
        {
            if (!string.IsNullOrEmpty(this.SortVideoDir) && Directory.Exists(this.SortVideoDir))
            {
                foreach (string path in Directory.EnumerateFiles(this.SortVideoDir, "*", SearchOption.AllDirectories))
                {
                    if (!this.ChoiceOrderMediaList.Contains(path))
                    {
                        this.ChoiceOrderMediaList.Add(new ChoiceOrderMediaData(path));
                    }
                }
            }
        }
    }
}
