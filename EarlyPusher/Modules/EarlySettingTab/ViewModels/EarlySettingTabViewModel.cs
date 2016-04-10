using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EarlyPusher.Models;
using EarlyPusher.Modules.EarlySettingTab.Views;
using EarlyPusher.ViewModels;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.Modules.EarlySettingTab.ViewModels
{
	public class EarlySettingTabViewModel : OperateTabVMBase
	{
		private ViewModelsAdapter<SetViewModel,SetData> adapter;
		private SetViewModel selectedSet;

		public DelegateCommand AddSetCommand { get; }
		public DelegateCommand RemSetCommand { get; }

		public ObservableVMCollection<SetData, SetViewModel> Sets { get; }

		public SetViewModel SelectedSet
		{
			get { return this.selectedSet; }
			set { SetProperty( ref this.selectedSet, value, SelectedSetChanged ); }
		}

		public EarlySettingTabViewModel( MainVM parent ) : base( parent )
		{
			this.View = new EarlySettingTabView();
			this.Header = "早押し設定";

			this.AddSetCommand = new DelegateCommand( AddSet );
			this.RemSetCommand = new DelegateCommand( RemSet, CanRemSet );

			this.Sets = new ObservableVMCollection<SetData, SetViewModel>();

			this.adapter = new ViewModelsAdapter<SetViewModel, SetData>( CreateSetViewModel );
		}

		private SetViewModel CreateSetViewModel( SetData arg )
		{
			return new SetViewModel( arg );
		}

		private void SelectedSetChanged()
		{
			this.RemSetCommand.RaiseCanExecuteChanged();
		}

		private void AddSet( object obj )
		{
			this.Parent.Data.Sets.Add( new SetData() { Name = "Set" } );
		}

		private bool CanRemSet( object obj )
		{
			return this.SelectedSet != null;
		}

		private void RemSet( object obj )
		{
			Debug.Assert( this.SelectedSet != null );

			this.Parent.Data.Sets.Remove( this.SelectedSet.Model );
			this.SelectedSet = null;
		}

		public override void LoadData()
		{
			this.adapter.Adapt( this.Sets, this.Parent.Data.Sets );
		}
	}
}
