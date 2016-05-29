using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EarlyPusher.Models;
using EarlyPusher.Modules.EarlyTab.Views;
using EarlyPusher.ViewModels;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.Modules.EarlyTab.ViewModels
{
	public class OperateEarlyVM : OperateTabVMBase
	{
		private ViewModelsAdapter<TeamEarlyVM,TeamData> adapter;
		private IEnumerable<SetData> sets;
		private SetData selectedSet;

		private PlayEarlyView view = new PlayEarlyView();

		private MediaVM selectedMedia;
		private MediaVM pushSound;
		private MediaVM correctSound;
		private MediaVM missSound;

		private TeamEarlyVM answerTeam;
		private bool receivable;
		private int addPoint;

		#region プロパティ

		public DelegateCommand CorrectCommand { get; }
		public DelegateCommand MissCommand { get; }

		public IEnumerable<SetData> Sets
		{
			get { return this.sets; }
			set { SetProperty( ref this.sets, value ); }
		}

		public SetData SelectedSet
		{
			get { return this.selectedSet; }
			set { SetProperty( ref this.selectedSet, value, SetChanged ); }
		}

		/// <summary>
		/// メディアのリスト
		/// </summary>
		public ObservableHashVMCollection<MediaVM> Medias { get; }

		/// <summary>
		/// チームのリスト
		/// </summary>
		/// <remarks>
		/// プレイウィンドウに表示するチームごとの列のリスト
		/// </remarks>
		public ObservableVMCollection<TeamData, TeamEarlyVM> Teams { get; }

		/// <summary>
		/// 選択しているメディア
		/// </summary>
		public MediaVM SelectedMedia
		{
			get { return this.selectedMedia; }
			set { SetProperty( ref this.selectedMedia, value, null, SelectedMediaChanging ); }
		}

		/// <summary>
		/// プッシュ受付可能
		/// </summary>
		public bool Receivable
		{
			get { return this.receivable; }
			set { SetProperty( ref this.receivable, value ); }
		}

		/// <summary>
		/// 解答権を持つチーム
		/// </summary>
		public TeamEarlyVM AnswerTeam
		{
			get { return this.answerTeam; }
			set { SetProperty( ref this.answerTeam, value, AnswerTeamChanged, AnswerTeamChanging ); }
		}

		/// <summary>
		/// 追加するポイント
		/// </summary>
		public int AddPoint
		{
			get { return this.addPoint; }
			set { SetProperty( ref this.addPoint, value ); }
		}

		#endregion

		public OperateEarlyVM( MainVM parent )
			: base( parent )
		{
			this.Teams = new ObservableVMCollection<TeamData, TeamEarlyVM>();
			this.Medias = new ObservableHashVMCollection<MediaVM>();

			this.View = new OperateEarlyView();
			this.Header = "早押し";
			this.CorrectCommand = new DelegateCommand( Correct );
			this.MissCommand = new DelegateCommand( Miss );
		}

		public override UIElement PlayView
		{
			get { return this.view; }
		}

		#region 設定読み書き

		public override void LoadData()
		{
			base.LoadData();

			this.Parent.Data.PropertyChanged += Data_PropertyChanged;

			if( !string.IsNullOrEmpty( this.Parent.Data.AnswerSoundPath ) )
			{
				this.pushSound = new MediaVM();
				this.pushSound.FilePath = this.Parent.Data.AnswerSoundPath;
				this.pushSound.LoadFile();
			}

			if( !string.IsNullOrEmpty( this.Parent.Data.CorrectSoundPath ) )
			{
				this.correctSound = new MediaVM();
				this.correctSound.FilePath = this.Parent.Data.CorrectSoundPath;
				this.correctSound.LoadFile();
			}

			if( !string.IsNullOrEmpty( this.Parent.Data.MissSoundPath ) )
			{
				this.missSound = new MediaVM();
				this.missSound.FilePath = this.Parent.Data.MissSoundPath;
				this.missSound.LoadFile();
			}

			this.adapter = new ViewModelsAdapter<TeamEarlyVM, TeamData>( m => new TeamEarlyVM( m ) );
			this.adapter.Adapt( this.Teams, this.Parent.Data.TeamList );

			this.Sets = this.Parent.Data.Sets;
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

		private void Data_PropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			switch( e.PropertyName )
			{
				case "AnswerSoundPath":
					if( !string.IsNullOrEmpty( this.Parent.Data.AnswerSoundPath ) )
					{
						this.pushSound = new MediaVM();
						this.pushSound.FilePath = this.Parent.Data.AnswerSoundPath;
						this.pushSound.LoadFile();
					}
					else
					{
						this.pushSound = null;
					}
					break;
				case "CorrectSoundPath":
					if( !string.IsNullOrEmpty( this.Parent.Data.CorrectSoundPath ) )
					{
						this.correctSound = new MediaVM();
						this.correctSound.FilePath = this.Parent.Data.CorrectSoundPath;
						this.correctSound.LoadFile();
					}
					else
					{
						this.correctSound = null;
					}
					break;
				case "MissSoundPath":
					if( !string.IsNullOrEmpty( this.Parent.Data.MissSoundPath ) )
					{
						this.missSound = new MediaVM();
						this.missSound.FilePath = this.Parent.Data.MissSoundPath;
						this.missSound.LoadFile();
					}
					else
					{
						this.missSound = null;
					}
					break;
				default:
					break;
			}
		}

		private void Manager_KeyPushed( object sender, Manager.DeviceKeyEventArgs e )
		{
			if( this.Receivable )
			{
				var team = this.Teams.FirstOrDefault( t => t.Model.Members.Any( m => m.DeviceGuid == e.InstanceID && m.Key == e.Key ) );
				if( team?.Pushable ?? false )
				{
					this.pushSound.Play();
					this.AnswerTeam = team;
					this.SelectedMedia.Pause();
				}
			}
		}

		private void SetChanged()
		{
			if( this.SelectedSet != null )
			{
				this.Medias.Clear();
				foreach( string path in Directory.EnumerateFiles( this.SelectedSet.Path, "*", SearchOption.AllDirectories ) )
				{
					var media = new MediaVM() { FilePath = path };
					media.LoadFile();
					this.Medias.Add( media );
				}
			}
		}

		/// <summary>
		/// 選択しているメディアが変わるとき、以前のメディアを停止します。
		/// </summary>
		private void SelectedMediaChanging()
		{
			if( this.SelectedMedia != null )
			{
				this.SelectedMedia.Stop();
			}
		}

		private void AnswerTeamChanging()
		{
			if( this.AnswerTeam != null )
			{
				this.AnswerTeam.Answerable = false;
			}
		}

		private void AnswerTeamChanged()
		{
			if( this.AnswerTeam != null )
			{
				this.AnswerTeam.Answerable = true;
			}
			this.CorrectCommand.RaiseCanExecuteChanged();
			this.MissCommand.RaiseCanExecuteChanged();
		}

		#endregion

		#region コマンド

		private void Correct( object obj )
		{
			if( this.AddPoint == 0 )
			{
				if( MessageBox.Show( App.Current.MainWindow, "追加ポイント0だけどいいの？", string.Empty, MessageBoxButton.OKCancel ) != MessageBoxResult.OK )
				{
					return;
				}
			}

			this.correctSound.Play();
			this.AnswerTeam.Add( this.AddPoint );
			this.AddPoint = 0;

			this.AnswerTeam = null;
		}

		private void Miss( object obj )
		{
			this.missSound.Play();
			this.AnswerTeam = null;
		}

		#endregion
	}
}
