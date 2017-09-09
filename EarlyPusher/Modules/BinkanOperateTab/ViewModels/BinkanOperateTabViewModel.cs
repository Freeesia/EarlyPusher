using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using EarlyPusher.Manager;
using EarlyPusher.Models;
using EarlyPusher.Modules.BinkanOperateTab.Views;
using EarlyPusher.Utils;
using EarlyPusher.ViewModels;
using SFLibs.Commands;
using SFLibs.Core.Adapters;
using SFLibs.Core.Basis;

namespace EarlyPusher.Modules.BinkanOperateTab.ViewModels
{
    public class BinkanOperateTabViewModel : OperateTabVMBase
    {
        private ViewModelsAdapter<MediaVM, string> mediaAdapter;
        private ViewModelsAdapter<TeamViewModel, TeamData> teamAdapter;
        private MediaVM selectedMedia;
        private bool receivable;
        private MemberViewModel answerMember;
        private int addPoint;

        private MediaVM pushSound = new MediaVM();
        private MediaVM correctSound = new MediaVM();
        private MediaVM incorrectSound = new MediaVM();
        private MediaVM questionSound = new MediaVM();
        private bool playingQuestion;

        #region プロパティ

        public bool PlayingQuestion
        {
            get { return this.playingQuestion; }
            set { SetProperty(ref this.playingQuestion, value); }
        }

        /// <summary>
        /// ヒント動画一覧
        /// </summary>
        public ObservableCollection<MediaVM> Medias { get; }

        public MediaVM SelectedMedia
        {
            get { return this.selectedMedia; }
            set { SetProperty(ref this.selectedMedia, value, SelectedMediaChanged); }
        }

        /// <summary>
        /// プッシュ受付可能
        /// </summary>
        public bool Receivable
        {
            get { return this.receivable; }
            set { SetProperty(ref this.receivable, value); }
        }

        /// <summary>
        /// チーム一覧
        /// </summary>
        public ObservableVMCollection<TeamData, TeamViewModel> Teams { get; }

        /// <summary>
        /// 解答権を持つチーム
        /// </summary>
        public MemberViewModel AnswerMember
        {
            get { return this.answerMember; }
            set { SetProperty(ref this.answerMember, value, AnswerMemberChanged, AnswerMemberChanging); }
        }

        /// <summary>
        /// 追加するポイント
        /// </summary>
        public int AddPoint
        {
            get { return this.addPoint; }
            set { SetProperty(ref this.addPoint, value); }
        }

        public DelegateCommand PlayOrPauseCommand { get; }
        public DelegateCommand CorrectCommand { get; }
        public DelegateCommand IncorrectCommand { get; }

        #endregion

        public BinkanOperateTabViewModel(MainVM parent) : base(parent)
        {
            this.Header = "ビンカン";
            this.View = new BinkanOperateTabView();

            this.mediaAdapter = new ViewModelsAdapter<MediaVM, string>(CreateMediaViewModel, DeleteMediaViewModel);
            this.teamAdapter = new ViewModelsAdapter<TeamViewModel, TeamData>(CrateTeamViewModel);

            this.Medias = new ObservableCollection<MediaVM>();
            this.Teams = new ObservableVMCollection<TeamData, TeamViewModel>();

            this.PlayOrPauseCommand = new DelegateCommand(PlayOrPause, p => this.SelectedMedia != null);
            this.CorrectCommand = new DelegateCommand(Correct, p => this.AnswerMember != null);
            this.IncorrectCommand = new DelegateCommand(Incorrect, p => this.AnswerMember != null);

            this.questionSound.MediaStoped += QuestionSound_MediaStoped;

            this.PlayView = new BinkanPlayView();
        }

        #region コマンド

        private void SelectedMediaChanged()
        {
            this.PlayOrPauseCommand.RaiseCanExecuteChanged();
        }

        private void PlayOrPause(object obj)
        {
            if (!this.SelectedMedia.IsPlaying)
            {
                this.questionSound.Play();
                this.PlayingQuestion = true;
            }
            else
            {
                this.SelectedMedia.Pause();
            }
        }

        private void QuestionSound_MediaStoped(object sender, EventArgs e)
        {
            this.SelectedMedia.Play();
            this.PlayingQuestion = false;
        }

        private void AnswerMemberChanging(MemberViewModel old)
        {
            if (this.AnswerMember != null)
            {
                this.AnswerMember.Answerable = false;
            }
        }

        private void AnswerMemberChanged()
        {
            if (this.AnswerMember != null)
            {
                this.AnswerMember.Answerable = true;
            }
            this.CorrectCommand.RaiseCanExecuteChanged();
            this.IncorrectCommand.RaiseCanExecuteChanged();
        }

        private void Correct(object obj)
        {
            if (this.AddPoint == 0)
            {
                if (MessageBox.Show(App.Current.MainWindow, "追加ポイント0だけどいいの？", string.Empty, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    return;
                }
            }

            this.correctSound.Play();
            this.AnswerMember.Parent.Add(this.AddPoint);
            this.AddPoint = 0;

            this.AnswerMember = null;
        }

        private void Incorrect(object obj)
        {
            this.incorrectSound.Play();
            this.AnswerMember = null;
        }

        #endregion

        #region 読み込み

        public override void LoadData()
        {
            this.Medias.Clear();
            this.SelectedMedia = null;

            this.mediaAdapter.Adapt(this.Medias, this.Parent.Data.Binkan.Hints);

            if (this.Medias.Count > 0)
            {
                this.SelectedMedia = this.Medias[0];
            }

            this.Teams.Clear();
            this.teamAdapter.Adapt(this.Teams, this.Parent.Data.TeamList);

            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            {
                this.pushSound.FilePath = PathUtility.GetAbsolutePath(baseDir, this.Parent.Data.Binkan.PushPath);
                this.pushSound.LoadFile();
            }
            {
                this.correctSound.FilePath = PathUtility.GetAbsolutePath(baseDir, this.Parent.Data.Binkan.CorrectPath);
                this.correctSound.LoadFile();
            }
            {
                this.incorrectSound.FilePath = PathUtility.GetAbsolutePath(baseDir, this.Parent.Data.Binkan.IncorrectPath);
                this.incorrectSound.LoadFile();
            }
            {
                this.questionSound.FilePath = PathUtility.GetAbsolutePath(baseDir, this.Parent.Data.Binkan.QuestionPath);
                this.questionSound.LoadFile();
            }
            this.Parent.Data.Binkan.PropertyChanged += Binkan_PropertyChanged;
        }

        private void Binkan_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (e.PropertyName == nameof(this.Parent.Data.Binkan.PushPath))
            {
                this.pushSound.FilePath = PathUtility.GetAbsolutePath(baseDir, this.Parent.Data.Binkan.PushPath);
                this.pushSound.LoadFile();
            }

            if (e.PropertyName == nameof(this.Parent.Data.Binkan.CorrectPath))
            {
                this.correctSound.FilePath = PathUtility.GetAbsolutePath(baseDir, this.Parent.Data.Binkan.CorrectPath);
                this.correctSound.LoadFile();
            }

            if (e.PropertyName == nameof(this.Parent.Data.Binkan.IncorrectPath))
            {
                this.incorrectSound.FilePath = PathUtility.GetAbsolutePath(baseDir, this.Parent.Data.Binkan.IncorrectPath);
                this.incorrectSound.LoadFile();
            }

            if (e.PropertyName == nameof(this.Parent.Data.Binkan.QuestionPath))
            {
                this.questionSound.FilePath = PathUtility.GetAbsolutePath(baseDir, this.Parent.Data.Binkan.QuestionPath);
                this.questionSound.LoadFile();
            }
        }

        #endregion

        #region ViewMode関連

        private TeamViewModel CrateTeamViewModel(TeamData arg)
        {
            return new TeamViewModel(arg);
        }

        private MediaVM CreateMediaViewModel(string arg)
        {
            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var media = new MediaVM() { FilePath = PathUtility.GetAbsolutePath(baseDir, arg) };
            media.LoadFile();
            media.Media.MediaEnded += Media_MediaEnded;
            media.MediaPlayed += Media_MediaPlayed;

            return media;
        }

        private void DeleteMediaViewModel(MediaVM media)
        {
            media.Media.MediaEnded -= Media_MediaEnded;
            media.MediaPlayed -= Media_MediaPlayed;
        }

        #endregion

        #region アクティブ

        public override void Activate()
        {
            this.Parent.Manager.KeyPushed += Manager_KeyPushed;
        }

        public override void Deactivate()
        {
            this.Parent.Manager.KeyPushed -= Manager_KeyPushed;
        }

        #endregion

        private void Media_MediaPlayed(object sender, EventArgs e)
        {
            this.Receivable = true;
        }

        private void Media_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            var index = this.Medias.IndexOf(this.SelectedMedia);

            if (index < this.Medias.Count - 1)
            {
                this.SelectedMedia = this.Medias[index + 1];
                this.SelectedMedia.Play();
            }
        }

        private void Manager_KeyPushed(object sender, DeviceKeyEventArgs e)
        {
            if (this.Receivable)
            {
                var member = this.Teams.SelectMany(t => t.Members).FirstOrDefault(m => m.Model.DeviceGuid == e.InstanceID && m.Model.Key == e.Key);
                if (member?.Parent?.Pushable ?? false)
                {
                    this.pushSound.Play();
                    this.AnswerMember = member;
                    this.SelectedMedia.Pause();

                    this.Receivable = false;
                }
            }
        }
    }
}
