using System;
using System.Windows.Input;
using EarlyPusher.Models;
using Ookii.Dialogs.Wpf;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.Modules.EarlySettingTab.ViewModels
{
	public class SubjectViewModel : ViewModelBase<SubjectData>
	{
		public ICommand RefPathCommand { get; }

		public SubjectViewModel( SubjectData model ) : base( model )
		{
			this.RefPathCommand = new DelegateCommand( RefPath );
		}

		private void RefPath( object obj )
		{
			var dlg = new VistaFolderBrowserDialog();
			if( string.IsNullOrEmpty( this.Model.Path ) )
			{
				dlg.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
			}
			else
			{
				dlg.SelectedPath = this.Model.Path;
			}
			if( dlg.ShowDialog() == true )
			{
				this.Model.Path = dlg.SelectedPath;
			}
		}
	}
}
