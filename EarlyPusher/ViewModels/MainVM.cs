﻿using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using EarlyPusher.Manager;
using EarlyPusher.Models;
using EarlyPusher.Modules.BinkanOperateTab.ViewModels;
using EarlyPusher.Modules.BinkanSettingTab.ViewModels;
using EarlyPusher.Modules.CommonSettingTab.ViewModels;
using EarlyPusher.Modules.EarlySettingTab.ViewModels;
using EarlyPusher.Modules.EarlyTab.ViewModels;
using EarlyPusher.Views;
using SFLibs.Commands;
using SFLibs.Core.Basis;

namespace EarlyPusher.ViewModels
{
    public class MainVM : ViewModelBase
    {
        private SettingData data;

        private PlayWindow window;
        private DeviceManager manager;

        private List<OperateTabVMBase> tabs = new List<OperateTabVMBase>();

        private OperateTabVMBase selectedTab;

        #region プロパティ

        public DelegateCommand WindowCommand { get; private set; }
        public DelegateCommand WindowMaxCommand { get; private set; }

        public DelegateCommand LoadedCommand { get; private set; }
        public DelegateCommand ClosingCommand { get; private set; }

        /// <summary>
        /// ボタンデバイスの管理クラス
        /// </summary>
        public DeviceManager Manager => this.manager;

        public SettingData Data => this.data;

        public IEnumerable<OperateTabVMBase> Tabs => this.tabs;

        public OperateTabVMBase SelectedTab
        {
            get { return this.selectedTab; }
            set { SetProperty(ref this.selectedTab, value, SelectedTabChanged, SelectedTabChanging); }
        }

        #endregion

        public MainVM()
        {
            this.WindowCommand = new DelegateCommand(ShowCloseWindow, CanShowCloseWindow);
            this.WindowMaxCommand = new DelegateCommand(MaximazeWindow, CanMaximaize);

            this.LoadedCommand = new DelegateCommand(Inited, null);
            this.ClosingCommand = new DelegateCommand(Closing, null);

            this.manager = new DeviceManager();

            this.tabs.Add(new CommonSettingTabVM(this));
            //this.tabs.Add( new OperateSetting2VM( this ) );
            //this.tabs.Add( new OperateChoiceVM( this ) );
            this.tabs.Add(new EarlySettingTabViewModel(this));
            this.tabs.Add(new OperateEarlyVM(this));
            this.tabs.Add(new BinkanSettingTabViewModel(this));
            this.tabs.Add(new BinkanOperateTabViewModel(this));
            //this.tabs.Add( new OperateOrderVM( this ) );
            //this.tabs.Add( new OperateTimeShockVM( this ) );
        }

        private void SelectedTabChanging(OperateTabVMBase old)
        {
            if (this.SelectedTab != null)
            {
                this.SelectedTab.Deactivate();
            }
        }

        private void SelectedTabChanged()
        {
            if (this.SelectedTab != null)
            {
                this.SelectedTab.Activate();
            }
            this.WindowCommand.RaiseCanExecuteChanged();
        }

        #region コマンド

        private bool CanShowCloseWindow(object obj)
        {
            return this.SelectedTab.PlayView != null;
        }

        /// <summary>
        /// プレイウィンドウの開閉処理
        /// </summary>
        /// <param name="obj"></param>
        private void ShowCloseWindow(object obj)
        {
            if (this.window != null)
            {
                this.window.Close();
                this.window = null;

            }
            else
            {
                this.window = new PlayWindow();
                this.window.DataContext = this.SelectedTab;
                this.window.Show();
            }
            this.WindowMaxCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// プレイウィンドウの最大化可能かどうか
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanMaximaize(object obj)
        {
            return this.window != null;
        }

        /// <summary>
        /// プレイウィンドウの最大化・通常化
        /// </summary>
        /// <param name="obj"></param>
        private void MaximazeWindow(object obj)
        {
            Contract.Assert(this.window != null);

            if (this.window.WindowState != System.Windows.WindowState.Maximized)
            {
                this.window.WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                this.window.WindowState = System.Windows.WindowState.Normal;
            }
        }

        /// <summary>
        /// ウィンドウが開くときの処理
        /// </summary>
        /// <param name="obj"></param>
        private void Inited(object obj)
        {
            LoadData();
            this.SelectedTab = this.Tabs.First();
        }

        /// <summary>
        /// ウィンドウが閉じるときの処理
        /// </summary>
        /// <param name="obj"></param>
        private void Closing(object obj)
        {
            SaveData(SettingData.FileName);
            if (this.window != null)
            {
                this.window.Close();
            }

            this.manager.Dispose();
        }

        #endregion

        #region 設定読み書き

        /// <summary>
        /// 設定を読み込みます。
        /// </summary>
        private void LoadData()
        {
            try
            {
                if (File.Exists(SettingData.FileName))
                {
                    using (FileStream file = new FileStream(SettingData.FileName, FileMode.Open))
                    {
                        XmlSerializer xml = new XmlSerializer(typeof(SettingData));
                        this.data = xml.Deserialize(file) as SettingData;
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (this.data == null)
                {
                    this.data = new SettingData();
                }
            }

            //4択用に必ず1チーム4人は確保する。
            //foreach( var team in this.data.TeamList )
            //{
            //	while( team.Members.Count < 4 )
            //	{
            //		team.Members.Add( new MemberData() );
            //	}
            //}

            foreach (var tab in this.Tabs)
            {
                tab.LoadData();
            }
        }

        /// <summary>
        /// 設定を保存します。
        /// </summary>
        public void SaveData(string path)
        {
            foreach (var tab in this.Tabs)
            {
                tab.SaveData();
            }

            using (Stream file = new FileStream(path, FileMode.Create))
            {
                XmlSerializer xml = new XmlSerializer(typeof(SettingData));
                xml.Serialize(file, this.data);
            }
        }

        #endregion

    }
}
