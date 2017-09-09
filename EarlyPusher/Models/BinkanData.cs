using System;
using System.Collections.ObjectModel;
using SFLibs.Core.Basis;

namespace EarlyPusher.Models
{
    [Serializable]
    public class BinkanData : ObservableObject
    {
        private string correctPath;
        private string incorrectPath;
        private string pushPath;
        private string questionPath;

        /// <summary>
        /// ビンカン用のヒント動画
        /// </summary>
        public ObservableCollection<string> Hints { get; } = new ObservableCollection<string>();

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
    }
}
