using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyPusher.Models;
using StFrLibs.Core.Adapters;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.Modules.EarlyTab.ViewModels
{
	public class TeamEarlyVM : ViewModelBase<TeamData>
	{
		private ObservableVMCollection<MemberData, MemberEarlyVM> members = new ObservableVMCollection<MemberData, MemberEarlyVM>();

		#region プロパティ

		public ObservableVMCollection<MemberData, MemberEarlyVM> Members
		{
			get { return this.members; }
		}

		public OperateEarlyVM Parent { get; private set; }

		public DelegateCommand AddPointCommand { get; private set; }

		#endregion

		public TeamEarlyVM( OperateEarlyVM parent, TeamData data )
			: base( data )
		{
			this.Parent = parent;

			this.AddPointCommand = new DelegateCommand( AddPoint );
		}

		private void AddPoint( object obj )
		{
			this.Model.Point += this.Parent.GetPoint;
			this.Parent.GetPoint = this.Parent.InitPoint;
		}

		private void CreateVM( MemberData member )
		{
			member.PropertyChanged += MemberData_PropertyChanged;
			if( !string.IsNullOrEmpty( member.Name ) )
			{
				this.Members.Add( new MemberEarlyVM( this, member ) );
			}
		}

		#region オーバーライド

		public override void AttachModel()
		{
			this.Model.Members.CollectionChanged += MemberDatas_CollectionChanged;
			foreach( MemberData member in this.Model.Members )
			{
				CreateVM( member );
			}

			base.AttachModel();
		}

		public override void DettachModel()
		{
			this.Model.Members.CollectionChanged -= MemberDatas_CollectionChanged;
			foreach( var member in this.Model.Members )
			{
				member.PropertyChanged -= MemberData_PropertyChanged;
			}
			this.members.Clear();

			base.DettachModel();
		}

		#endregion

		#region イベント

		private void MemberDatas_CollectionChanged( object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e )
		{
			if( e.OldItems != null )
			{
				foreach( MemberData member in e.OldItems )
				{
					member.PropertyChanged -= MemberData_PropertyChanged;
					this.Members.Remove( member );
				}
			}

			if( e.NewItems != null )
			{
				foreach( MemberData member in e.NewItems )
				{
					CreateVM( member );
				}
			}
		}

		private void MemberData_PropertyChanged( object sender, System.ComponentModel.PropertyChangedEventArgs e )
		{
			var member = sender as MemberData;
			if( string.IsNullOrEmpty( member.Name ) )
			{
				this.Members.Remove( member );
			}
			else if( !this.Members.Contains( member ) )
			{
				this.Members.Add( new MemberEarlyVM( this, member ) );
			}
		}

		#endregion
	}
}
