using System;
using System.IO;
using EarlyPusher.Models;
using Microsoft.Win32;
using SFLibs.Commands;
using SFLibs.Core.Basis;

namespace EarlyPusher.Modules.Setting2Tab.ViewModels
{
    public class MediaSetting2VM : ViewModelBase<ChoiceOrderMediaData>
    {
        private Choice choice1;
        private Choice choice2;
        private Choice choice3;
        private Choice choice4;

        public string FilePath { get; private set; }

        public string FileName
        {
            get { return Path.GetFileName(this.FilePath); }
        }

        public Choice[] Choices
        {
            get { return (Choice[])Enum.GetValues(typeof(Choice)); }
        }

        public Choice Choice1
        {
            get { return this.choice1; }
            set { SetProperty(ref this.choice1, value, SetChoiceOrder); }
        }

        public Choice Choice2
        {
            get { return this.choice2; }
            set { SetProperty(ref this.choice2, value, SetChoiceOrder); }
        }

        public Choice Choice3
        {
            get { return this.choice3; }
            set { SetProperty(ref this.choice3, value, SetChoiceOrder); }
        }

        public Choice Choice4
        {
            get { return this.choice4; }
            set { SetProperty(ref this.choice4, value, SetChoiceOrder); }
        }

        public DelegateCommand SelectChoiceACommand { get; private set; }
        public DelegateCommand SelectChoiceBCommand { get; private set; }
        public DelegateCommand SelectChoiceCCommand { get; private set; }
        public DelegateCommand SelectChoiceDCommand { get; private set; }

        public MediaSetting2VM(ChoiceOrderMediaData model)
            : base(model)
        {
            this.FilePath = this.Model.MediaPath;

            this.choice1 = this.Model.ChoiceOrder[0];
            this.choice2 = this.Model.ChoiceOrder[1];
            this.choice3 = this.Model.ChoiceOrder[2];
            this.choice4 = this.Model.ChoiceOrder[3];

            this.SelectChoiceACommand = new DelegateCommand(SelectChoiceA);
            this.SelectChoiceBCommand = new DelegateCommand(SelectChoiceB);
            this.SelectChoiceCCommand = new DelegateCommand(SelectChoiceC);
            this.SelectChoiceDCommand = new DelegateCommand(SelectChoiceD);
        }

        private void SelectChoiceA(object obj)
        {
            var dlg = new OpenFileDialog();
            if (!string.IsNullOrEmpty(this.Model.ChoiceAImagePath))
            {
                dlg.InitialDirectory = Path.GetDirectoryName(this.Model.ChoiceAImagePath);
            }
            if (dlg.ShowDialog() == true)
            {
                this.Model.ChoiceAImagePath = dlg.FileName;
            }
        }

        private void SelectChoiceB(object obj)
        {
            var dlg = new OpenFileDialog();
            if (!string.IsNullOrEmpty(this.Model.ChoiceBImagePath))
            {
                dlg.InitialDirectory = Path.GetDirectoryName(this.Model.ChoiceBImagePath);
            }
            if (dlg.ShowDialog() == true)
            {
                this.Model.ChoiceBImagePath = dlg.FileName;
            }
        }

        private void SelectChoiceC(object obj)
        {
            var dlg = new OpenFileDialog();
            if (!string.IsNullOrEmpty(this.Model.ChoiceCImagePath))
            {
                dlg.InitialDirectory = Path.GetDirectoryName(this.Model.ChoiceCImagePath);
            }
            if (dlg.ShowDialog() == true)
            {
                this.Model.ChoiceCImagePath = dlg.FileName;
            }
        }

        private void SelectChoiceD(object obj)
        {
            var dlg = new OpenFileDialog();
            if (!string.IsNullOrEmpty(this.Model.ChoiceDImagePath))
            {
                dlg.InitialDirectory = Path.GetDirectoryName(this.Model.ChoiceDImagePath);
            }
            if (dlg.ShowDialog() == true)
            {
                this.Model.ChoiceDImagePath = dlg.FileName;
            }
        }

        private void SetChoiceOrder()
        {
            this.Model.ChoiceOrder[0] = this.Choice1;
            this.Model.ChoiceOrder[1] = this.Choice2;
            this.Model.ChoiceOrder[2] = this.Choice3;
            this.Model.ChoiceOrder[3] = this.Choice4;
        }
    }
}
