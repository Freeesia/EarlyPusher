using System.Collections.Generic;
using SFLibs.Core.Basis;

namespace EarlyPusher.Models
{
    public class ChoiceOrderMediaData : ObservableObject
    {
        private List<Choice> choiceOrder = new List<Choice>(4);
        private string mediaPath;
        private string choiceAImagePath;
        private string choiceBImagePath;
        private string choiceCImagePath;
        private string choiceDImagePath;

        private ChoiceOrderMediaData()
        {
        }

        public ChoiceOrderMediaData(string path)
        {
            this.MediaPath = path;
            this.ChoiceOrder.Add(Choice.A);
            this.ChoiceOrder.Add(Choice.A);
            this.ChoiceOrder.Add(Choice.A);
            this.ChoiceOrder.Add(Choice.A);
        }

        public List<Choice> ChoiceOrder
        {
            get { return this.choiceOrder; }
        }

        public string MediaPath
        {
            get { return this.mediaPath; }
            set { SetProperty(ref this.mediaPath, value); }
        }

        public string ChoiceAImagePath
        {
            get { return this.choiceAImagePath; }
            set { SetProperty(ref this.choiceAImagePath, value); }
        }

        public string ChoiceBImagePath
        {
            get { return this.choiceBImagePath; }
            set { SetProperty(ref this.choiceBImagePath, value); }
        }

        public string ChoiceCImagePath
        {
            get { return this.choiceCImagePath; }
            set { SetProperty(ref this.choiceCImagePath, value); }
        }

        public string ChoiceDImagePath
        {
            get { return this.choiceDImagePath; }
            set { SetProperty(ref this.choiceDImagePath, value); }
        }
    }
}
