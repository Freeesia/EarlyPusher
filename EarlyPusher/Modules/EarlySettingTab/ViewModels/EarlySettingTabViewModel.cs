using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using EarlyPusher.Models;
using EarlyPusher.Modules.EarlySettingTab.Views;
using EarlyPusher.Utils;
using EarlyPusher.ViewModels;
using Microsoft.Win32;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.Modules.EarlySettingTab.ViewModels
{
	public class EarlySettingTabViewModel : OperateTabVMBase
	{
		private string pushPath;
		private string correctPath;
		private string incorrectPath;
		private string questionPath;

		private ViewModelsAdapter<SetViewModel,SetData> adapter;
		private SetViewModel selectedSet;

		public DelegateCommand AddSetCommand { get; }
		public DelegateCommand RemSetCommand { get; }

		public ObservableVMCollection<SetData, SetViewModel> Sets { get; }

		/// <summary>
		/// プッシュ音
		/// </summary>
		public string PushPath
		{
			get { return this.pushPath; }
			set { SetProperty( ref this.pushPath, value ); }
		}

		/// <summary>
		/// 正解音
		/// </summary>
		public string CorrectPath
		{
			get { return this.correctPath; }
			set { SetProperty( ref this.correctPath, value ); }
		}

		/// <summary>
		/// 不正解音
		/// </summary>
		public string IncorrectPath
		{
			get { return this.incorrectPath; }
			set { SetProperty( ref this.incorrectPath, value ); }
		}

		/// <summary>
		/// 出題音
		/// </summary>
		public string QuestionPath
		{
			get { return this.questionPath; }
			set { SetProperty( ref this.questionPath, value ); }
		}

		public SetViewModel SelectedSet
		{
			get { return this.selectedSet; }
			set { SetProperty( ref this.selectedSet, value, SelectedSetChanged ); }
		}

		public DelegateCommand OpenPushPathCommand { get; }
		public DelegateCommand OpenCorrectPathCommand { get; }
		public DelegateCommand OpenIncorrectPathCommand { get; }
		public DelegateCommand OpenQuestionPathCommand { get; }

		public EarlySettingTabViewModel( MainVM parent ) : base( parent )
		{
			this.View = new EarlySettingTabView();
			this.Header = "早押し設定";

			this.OpenPushPathCommand = new DelegateCommand( OpenPushPath );
			this.OpenCorrectPathCommand = new DelegateCommand( OpenCorrectPath );
			this.OpenIncorrectPathCommand = new DelegateCommand( OpenIncorrectPath );
			this.OpenQuestionPathCommand = new DelegateCommand( OpenQuestionPath );

			this.AddSetCommand = new DelegateCommand( AddSet );
			this.RemSetCommand = new DelegateCommand( RemSet, CanRemSet );

			this.Sets = new ObservableVMCollection<SetData, SetViewModel>();

			this.adapter = new ViewModelsAdapter<SetViewModel, SetData>( m => new SetViewModel( m ) );
		}

		private void SelectedSetChanged()
		{
			this.RemSetCommand.RaiseCanExecuteChanged();
		}

		#region コマンド

		private void OpenPushPath( object obj )
		{
			var dlg = new OpenFileDialog();
			if( dlg.ShowDialog( App.Current.MainWindow ) == true )
			{
				var baseDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
				this.Parent.Data.Early.PushPath = PathUtility.GetRelativePath( baseDir, dlg.FileName );
			}
		}

		private void OpenCorrectPath( object obj )
		{
			var dlg = new OpenFileDialog();
			if( dlg.ShowDialog( App.Current.MainWindow ) == true )
			{
				var baseDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
				this.Parent.Data.Early.CorrectPath = PathUtility.GetRelativePath( baseDir, dlg.FileName );
			}
		}

		private void OpenIncorrectPath( object obj )
		{
			var dlg = new OpenFileDialog();
			if( dlg.ShowDialog( App.Current.MainWindow ) == true )
			{
				var baseDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
				this.Parent.Data.Early.IncorrectPath = PathUtility.GetRelativePath( baseDir, dlg.FileName );
			}
		}

		private void OpenQuestionPath( object obj )
		{
			var dlg = new OpenFileDialog();
			if( dlg.ShowDialog( App.Current.MainWindow ) == true )
			{
				var baseDir = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
				this.Parent.Data.Early.QuestionPath = PathUtility.GetRelativePath( baseDir, dlg.FileName );
			}
		}

		private void AddSet( object obj )
		{
			this.Parent.Data.Early.Sets.Add( new SetData() { Name = "Set" } );
		}

		private bool CanRemSet( object obj )
		{
			return this.SelectedSet != null;
		}

		private void RemSet( object obj )
		{
			Debug.Assert( this.SelectedSet != null );

			this.Parent.Data.Early.Sets.Remove( this.SelectedSet.Model );
			this.SelectedSet = null;
		}

		#endregion

		public override void LoadData()
		{
			this.adapter.Adapt( this.Sets, this.Parent.Data.Early.Sets );

			this.PushPath = this.Parent.Data.Early.PushPath;
			this.CorrectPath = this.Parent.Data.Early.CorrectPath;
			this.IncorrectPath = this.Parent.Data.Early.IncorrectPath;
			this.QuestionPath = this.Parent.Data.Early.QuestionPath;

			this.Parent.Data.Early.PropertyChanged += Early_PropertyChanged;
		}

		private void Early_PropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			this.PushPath = this.Parent.Data.Early.PushPath;
			this.CorrectPath = this.Parent.Data.Early.CorrectPath;
			this.IncorrectPath = this.Parent.Data.Early.IncorrectPath;
			this.QuestionPath = this.Parent.Data.Early.QuestionPath;
		}
	}
}
