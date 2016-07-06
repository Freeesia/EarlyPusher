using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

		private ViewModelsAdapter<SubjectViewModel,SubjectData> adapter;
		private SubjectViewModel selectedSubject;

		public DelegateCommand AddSubjectCommand { get; }
		public DelegateCommand RemSubjectCommand { get; }
		public DelegateCommand UpSubjectCommand { get; }
		public DelegateCommand DownSubjectCommand { get; }

		public ObservableVMCollection<SubjectData, SubjectViewModel> Subjects { get; }

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

		public SubjectViewModel SelectedSubject
		{
			get { return this.selectedSubject; }
			set { SetProperty( ref this.selectedSubject, value, SelectedSubjectChanged ); }
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

			this.AddSubjectCommand = new DelegateCommand( AddSubject );
			this.RemSubjectCommand = new DelegateCommand( RemSubject, CanRemSubject );
			this.UpSubjectCommand = new DelegateCommand( UpSubject, CanUpSubject );
			this.DownSubjectCommand = new DelegateCommand( DownSubject, CanDownSubject );

			this.Subjects = new ObservableVMCollection<SubjectData, SubjectViewModel>();

			this.adapter = new ViewModelsAdapter<SubjectViewModel, SubjectData>( m => new SubjectViewModel( m ) );
		}

		private void SelectedSubjectChanged()
		{
			this.RemSubjectCommand.RaiseCanExecuteChanged();
			this.UpSubjectCommand.RaiseCanExecuteChanged();
			this.DownSubjectCommand.RaiseCanExecuteChanged();
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

		private void AddSubject( object obj )
		{
			this.Parent.Data.Early.Subjects.Add( new SubjectData() { Name = "問題" } );
		}

		private bool CanRemSubject( object obj )
		{
			return this.SelectedSubject != null;
		}

		private void RemSubject( object obj )
		{
			Debug.Assert( this.SelectedSubject != null );

			this.Parent.Data.Early.Subjects.Remove( this.SelectedSubject.Model );
			this.SelectedSubject = null;
		}

		private bool CanUpSubject( object obj )
		{
			return this.SelectedSubject != null && this.Subjects.FirstOrDefault() != this.SelectedSubject;
		}

		private void UpSubject( object obj )
		{
			var target = this.SelectedSubject.Model;
			var index = this.Parent.Data.Early.Subjects.IndexOf( target );
			this.Parent.Data.Early.Subjects.RemoveAt( index );
			this.Parent.Data.Early.Subjects.Insert( index - 1, target );
			this.SelectedSubject = this.Subjects[target];
		}

		private bool CanDownSubject( object obj )
		{
			return this.SelectedSubject != null && this.Subjects.LastOrDefault() != this.SelectedSubject;
		}

		private void DownSubject( object obj )
		{
			var target = this.SelectedSubject.Model;
			var index = this.Parent.Data.Early.Subjects.IndexOf( target );
			this.Parent.Data.Early.Subjects.RemoveAt( index );
			this.Parent.Data.Early.Subjects.Insert( index + 1, target );
			this.SelectedSubject = this.Subjects[target];
		}

		#endregion

		public override void LoadData()
		{
			this.adapter.Adapt( this.Subjects, this.Parent.Data.Early.Subjects );

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
