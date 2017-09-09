using System.Windows;
using SFLibs.Core.Basis;

namespace EarlyPusher.ViewModels
{
    public abstract class OperateTabVMBase : ViewModelBase
    {
        #region プロパティ

        public MainVM Parent { get; }

        public virtual UIElement PlayView
        {
            get;
            set;
        }

        public UIElement View
        {
            get;
            protected set;
        }

        public string Header
        {
            get;
            protected set;
        }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OperateTabVMBase(MainVM parent)
        {
            this.Parent = parent;
        }

        #region 設定読み書き

        /// <summary>
        /// 設定を読み込みます。
        /// </summary>
        public virtual void LoadData()
        {
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
    }
}
