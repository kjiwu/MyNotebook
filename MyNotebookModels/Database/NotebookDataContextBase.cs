using MyNotebookModels.Models;
using MyNotebookUtility.Common;
using MyNotebookUtility.Tools;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNotebookModels.Database
{
    public class NotebookDataContextBase : DataContext
    {
        public NotebookDataContextBase(string connectionStr)
            : base(connectionStr)
        {
            if (!this.DatabaseExists())
            {
                IsolatedStorageHelper.CreateDir(Constants.DB_Path);
                this.CreateDatabase();
            }
        }

        private Table<NoteCategory> Categories;

        private Table<NoteTemplate> Templates;
    }
}
