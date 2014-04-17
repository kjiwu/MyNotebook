using MyNotebookModels.Interfaces;
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
    public class NoteTemplateDataContext : DataContext, INotebookDataContext<NoteTemplate>
    {
        public NoteTemplateDataContext()
            : base(Constants.DB_NoteConfigPath)
        {
            if (!this.DatabaseExists())
            {
                IsolatedStorageHelper.CreateDir(Constants.DB_Path);
                this.CreateDatabase();
            }
        }

        public Table<NoteTemplate> Templates;

        public void Insert(NoteTemplate value)
        {
            var IsHave = Templates.Any(x => x.Id.Equals(value.Id));
            if (IsHave)
            {
                Update(value.Id, value);
            }
            else
            {
                Templates.InsertOnSubmit(value);
                this.SubmitChanges();
            }
        }

        public void Remove(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(string id, NoteTemplate newValue)
        {
            throw new NotImplementedException();
        }
    }
}
