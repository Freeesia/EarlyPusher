using System.ComponentModel;

namespace SFLibs.Core.Basis
{
    public abstract class ViewModelBase<T> : ViewModelBase where T : INotifyPropertyChanged
    {
        private T model;
        public T Model
        {
            get => this.model;
            private set => SetProperty(ref this.model, value);
        }

        public ViewModelBase()
        {
        }

        public ViewModelBase(T model)
        {
            SetModel(model);
        }

        public virtual void SetModel(T model)
        {
            DettachModel();
            this.Model = model;
            AttachModel();
        }

        public override void AttachModel()
        {
            if (this.Model != null)
            {
                this.Model.PropertyChanged += Model_PropertyChanged;
            }
        }

        public override void DettachModel()
        {
            if (this.Model != null)
            {
                this.Model.PropertyChanged -= Model_PropertyChanged;
            }
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ApplyFromModel(e.PropertyName);
        }

        public virtual void ApplyFromModel(string propertyName)
        {
        }
    }

    public abstract class ViewModelBase : ObservableObject
    {
        public virtual void AttachModel() { }

        public virtual void DettachModel() { }
    }
}
