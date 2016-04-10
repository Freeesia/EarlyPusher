using System;
using System.Windows.Input;
using EarlyPusher.Models;
using Ookii.Dialogs.Wpf;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.Modules.EarlySettingTab.ViewModels
{
	public class SetViewModel : ViewModelBase<SetData>
	{
		public ICommand RefPathCommand { get; }

		public SetViewModel( SetData model ) : base( model )
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
