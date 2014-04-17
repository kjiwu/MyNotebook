using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MyNotebookModels.Models
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void RaisePropertyChanged<T>(Expression<Func<T, string>> expression)
        {
            var handler = PropertyChanged;
            if (null != handler && null != expression)
            {
                var memberInfo = expression.Body as MemberExpression;
                if (null != memberInfo)
                {
                    handler(this, new PropertyChangedEventArgs(memberInfo.Member.Name));
                }
            }
        }
    }
}
