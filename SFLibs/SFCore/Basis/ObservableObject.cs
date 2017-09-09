using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SFLibs.Core.Basis
{
    public abstract class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging, IDisposable
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<T>(ref T member, T value, Action changed = null, Action<T> changing = null, [CallerMemberName]string memberName = "MemberName")
        {
            if (!EqualityComparer<T>.Default.Equals(member, value))
            {
                SetPropertyForce(ref member, value, changed, changing, memberName);
            }
        }

        protected void SetPropertyForce<T>(ref T member, T value, Action changed = null, Action<T> changing = null, [CallerMemberName]string memberName = "MemberName")
        {
            NotifyPropertyChanging(memberName);
            changing?.Invoke(value);
            member = value;
            NotifyPropertyChanged(memberName);
            changed?.Invoke();
        }

        public void NotifyPropertyChanged<MemberType>(Expression<Func<MemberType>> expression)
        {
            NotifyPropertyChanged(((MemberExpression)expression.Body).Member.Name);
        }

        public virtual void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NotifyPropertyChanging<MemberType>(Expression<Func<MemberType>> expression)
        {
            NotifyPropertyChanging(((MemberExpression)expression.Body).Member.Name);
        }

        public virtual void NotifyPropertyChanging(string propertyName)
        {
            this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeManaged();
            }
            DisposeUnmanaged();
        }

        protected virtual void DisposeManaged()
        {
        }

        protected virtual void DisposeUnmanaged()
        {
        }

        ~ObservableObject()
        {
            Dispose(false);
        }
    }
}
