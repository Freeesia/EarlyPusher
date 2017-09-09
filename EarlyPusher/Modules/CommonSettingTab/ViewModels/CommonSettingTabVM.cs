using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using EarlyPusher.Manager;
using EarlyPusher.Models;
using EarlyPusher.Modules.CommonSettingTab.Views;
using EarlyPusher.ViewModels;
using SFLibs.Commands;
using SFLibs.Core.Adapters;
using SFLibs.Core.Basis;
using SFLibs.Core.Extensions;
using SlimDX.DirectInput;

namespace EarlyPusher.Modules.CommonSettingTab.ViewModels
{
    public class CommonSettingTabVM : OperateTabVMBase
    {
        private ObservableVMCollection<TeamData, CommonTeamVM> teams = new ObservableVMCollection<TeamData, CommonTeamVM>();
        private ObservableCollection<CommonMemberVM> members = new ObservableCollection<CommonMemberVM>();
        private ObservableCollection<string> devices = new ObservableCollection<string>();
        private ViewModelsAdapter<CommonTeamVM, TeamData> teamAdapter;

        private long updateTime;

        private CommonTeamVM selectedTeam;
        private CommonMemberVM selectedMember;

        #region プロパティ

        public DelegateCommand SearchCommand { get; }
        public DelegateCommand AddTeamCommand { get; }
        public DelegateCommand DelTeamCommand { get; }

        public DelegateCommand AddMemberCommand { get; }
        public DelegateCommand DelMemberCommand { get; }
        public DelegateCommand AllKeyLockCommand { get; }
        public DelegateCommand AllKeyReleaseCommand { get; }

        /// <summary>
        /// チームのリスト
        /// </summary>
        public ObservableVMCollection<TeamData, CommonTeamVM> Teams
        {
            get { return this.teams; }
        }

        /// <summary>
        /// メンバーのリスト
        /// </summary>
        public ObservableCollection<CommonMemberVM> Members
        {
            get { return this.members; }
        }

        /// <summary>
        /// 選択しているチーム
        /// </summary>
        public CommonTeamVM SelectedTeam
        {
            get { return this.selectedTeam; }
            set { SetProperty(ref this.selectedTeam, value, SeletedTeamChanged); }
        }

        /// <summary>
        /// 選択しているメンバー
        /// </summary>
        public CommonMemberVM SelectedMember
        {
            get { return selectedMember; }
            set { SetProperty(ref selectedMember, value, SelectedMemberChanged); }
        }

        /// <summary>
        /// デバイスのリスト
        /// </summary>
        public ObservableCollection<string> Devices
        {
            get
            {
                return this.devices;
            }
        }

        /// <summary>
        /// デバイスの更新間隔
        /// </summary>
        public long UpdateTime
        {
            get { return updateTime; }
            set { SetProperty(ref updateTime, value); }
        }

        #endregion

        public CommonSettingTabVM(MainVM parent)
            : base(parent)
        {
            this.View = new CommonSettingTabView();
            this.Header = "共通設定";

            this.AddMemberCommand = new DelegateCommand(AddMember);
            this.DelMemberCommand = new DelegateCommand(DelMember, CanDelMember);

            this.AllKeyLockCommand = new DelegateCommand(AllKeyLock);
            this.AllKeyReleaseCommand = new DelegateCommand(AllKeyRelease);

            this.SearchCommand = new DelegateCommand(SearchDevice);
            this.AddTeamCommand = new DelegateCommand(AddTeam);
            this.DelTeamCommand = new DelegateCommand(DelTeam, CanDelTeam);

            this.Parent.Manager.Devices.CollectionChanged += Devices_CollectionChanged;
            this.Parent.Manager.PropertyChanged += Manager_PropertyChanged;
        }

        private void AllKeyLock(object obj)
        {
            this.Members.ForEach(m => m.IsKeyLock = true);
        }

        private void AllKeyRelease(object obj)
        {
            this.Members.ForEach(m => m.IsKeyLock = false);
        }

        public override UIElement PlayView
        {
            get { return null; }
        }

        #region 設定読み書き

        public override void LoadData()
        {
            base.LoadData();

            this.teamAdapter = new ViewModelsAdapter<CommonTeamVM, TeamData>(CreateTeamVM, DeleteTeamVM);
            this.teamAdapter.Adapt(this.Teams, this.Parent.Data.TeamList);
        }

        /// <summary>
        /// チームのモデルからVMを作る時の処理
        /// </summary>
        /// <param name="data">モデル</param>
        /// <returns>VM</returns>
        private CommonTeamVM CreateTeamVM(TeamData data)
        {
            var vm = new CommonTeamVM(data);
            vm.Members.CollectionChanged += Members_CollectionChanged;
            return vm;
        }

        /// <summary>
        /// VMが削除されたときの処理
        /// </summary>
        /// <param name="vm"></param>
        private void DeleteTeamVM(CommonTeamVM vm)
        {
            this.Members.RemoveWhere(m => m.Parent == vm);
            vm.Members.CollectionChanged -= Members_CollectionChanged;
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

        #region イベント

        /// <summary>
        /// 更新にかかった時間をアップデートします。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Manager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UpdateTime")
            {
                this.UpdateTime = this.Parent.Manager.UpdateTime;
            }
        }

        /// <summary>
        /// メンバーが追加削除されたときにメンバー一覧を更新します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Members_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (CommonMemberVM member in e.OldItems)
                {
                    this.Members.Remove(member);
                }
            }

            if (e.NewItems != null)
            {
                foreach (CommonMemberVM member in e.NewItems)
                {
                    this.Members.Add(member);
                }
            }
        }

        /// <summary>
        /// 選択しているチームが変わったとき削除条件を再チェックします。
        /// </summary>
        private void SeletedTeamChanged()
        {
            this.DelTeamCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// 選択しているメンバーが変わったとき、削除条件を再チェックします。
        /// </summary>
        private void SelectedMemberChanged()
        {
            this.DelMemberCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// ボタンが押された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Manager_KeyPushed(object sender, DeviceKeyEventArgs e)
        {
            if (this.SelectedMember != null && !this.SelectedMember.IsKeyLock)
            {
                this.SelectedMember.Model.DeviceGuid = e.InstanceID;
                this.SelectedMember.Model.Key = e.Key;
            }
        }

        /// <summary>
        /// 検出しているデバイスが変化したときにリストアップします。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Devices_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewItems != null)
                    {
                        foreach (Device d in e.NewItems)
                        {
                            this.devices.Add(d.Information.InstanceName);
                        }
                    }
                    if (e.OldItems != null)
                    {
                        foreach (Device d in e.OldItems)
                        {
                            this.devices.Remove(d.Information.InstanceName);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.devices.Clear();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region コマンド
        /// <summary>
        /// メンバーの追加
        /// </summary>
        /// <param name="obj"></param>
        private void AddMember(object obj)
        {
            var team = obj as TeamData;
            team.Members.Add(new MemberData());
        }

        /// <summary>
        /// メンバーを削除できるかチェック
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanDelMember(object obj)
        {
            return this.SelectedMember != null;
        }

        /// <summary>
        /// メンバーの削除
        /// </summary>
        /// <param name="obj"></param>
        private void DelMember(object obj)
        {
            this.SelectedMember.Parent.Model.Members.Remove(this.SelectedMember.Model);
            this.SelectedMember = null;
        }

        /// <summary>
        /// デバイスの検索
        /// </summary>
        /// <param name="obj"></param>
        private void SearchDevice(object obj)
        {
            this.Parent.Manager.SearchDevice();
        }

        /// <summary>
        /// チームの追加
        /// </summary>
        /// <param name="obj"></param>
        private void AddTeam(object obj)
        {
            var team = new TeamData() { TeamName = "チーム" };
            for (int i = 0; i < 4; i++)
            {
                team.Members.Add(new MemberData());
            }
            this.Parent.Data.TeamList.Add(team);
        }

        /// <summary>
        /// チームを削除できるかチェック
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanDelTeam(object obj)
        {
            return this.SelectedTeam != null;
        }

        /// <summary>
        /// チームの削除
        /// </summary>
        /// <param name="obj"></param>
        private void DelTeam(object obj)
        {
            this.Parent.Data.TeamList.Remove(this.SelectedTeam.Model);
            this.SelectedTeam = null;
        }

        #endregion

    }
}
