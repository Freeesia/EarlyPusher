using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;
using EarlyPusher.Basis;
using EarlyPusher.Manager;
using EarlyPusher.Models;
using EarlyPusher.Views;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using SlimDX.DirectInput;
using StFrLibs.Core.Basis;
using StFrLibs.Core.Commands;

namespace EarlyPusher.ViewModels
{
	public abstract class OperateTabVMBase : ViewModelBase
	{
		private MainVM parent;

		private MediaVM answerSound;

		private Dictionary<TeamVM,MemberVM> choicePanels = new Dictionary<TeamVM, MemberVM>();
		private bool isChoiceVisible;

		#region プロパティ

		public DelegateCommand OpenCommand { get; private set; }

		public MainVM Parent
		{
			get { return this.parent; }
		}

		/// <summary>
		/// 解答音
		/// </summary>
		public MediaVM AnswerSound
		{
			get { return answerSound; }
			set { SetProperty( ref answerSound, value ); }
		}

		/// <summary>
		/// 選択を表示する。
		/// </summary>
		public bool IsChoiceVisible
		{
			get { return this.isChoiceVisible; }
			set { SetProperty( ref this.isChoiceVisible, value ); }
		}

		public abstract UIElement PlayView
		{
			get;
		}
		
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OperateTabVMBase( MainVM parent )
		{
			this.parent = parent;

			this.OpenCommand = new DelegateCommand( Open, CanOpen );
		}

		#region コマンド関係

		private bool CanOpen( object obj )
		{
			//return this.Mode == PlayMode.Choice4;
			return true;
		}

		private void Open( object obj )
		{
			this.IsChoiceVisible = true;
		}

		#endregion

		#region 設定読み書き

		/// <summary>
		/// 設定を読み込みます。
		/// </summary>
		public virtual void LoadData()
		{
			this.AnswerSound = new MediaVM() { FilePath = this.parent.Data.AnswerSoundPath };
		}

		/// <summary>
		/// 設定を保存します。
		/// </summary>
		public virtual void SaveData()
		{
		}

		#endregion

		public virtual void Activate()
		{
		}

		public virtual void Deactivate()
		{
		}

		private void InitChoice()
		{
			this.choicePanels.Clear();
			this.IsChoiceVisible = false;
		}

		private void SetKeyChoiceMode( DeviceKeyEventArgs e )
		{
			//var item = this.Members.FirstOrDefault( i => i.Model.DeviceGuid == e.InstanceID && i.Model.Key == e.Key );
			//if( item == null )
			//{
			//	return;
			//}

			//if( this.choicePanels.ContainsKey(item.Parent) )
			//{
			//	this.choicePanels[item.Parent].Rank = string.Empty;
			//}

			//item.Rank = string.Format( "{0}", item.Parent.Members.IndexOf( item ) + 1 );
			//this.choicePanels[item.Parent] = item;
		}
	}
}
