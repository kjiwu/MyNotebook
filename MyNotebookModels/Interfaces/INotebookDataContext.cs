using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNotebookModels.Interfaces
{
    interface INotebookDataContext<T>
    {
        void Insert(T value);

        void Remove(string id);

        void Update(string id, T newValue);
    }
}
