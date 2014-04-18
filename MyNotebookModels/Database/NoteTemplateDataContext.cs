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
    public class NoteTemplateDataContext : NotebookDataContextBase, INotebookDataContext<NoteTemplate>
    {
        public NoteTemplateDataContext()
            : base(Constants.DB_NoteConfigPath)
        {
            
        }

        public Table<NoteTemplate> Templates
        {
            get
            {
                return this.GetTable<NoteTemplate>();
            }
        }

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
            var template = from t in Templates where t.Id.Equals(id) select t;
            Templates.DeleteAllOnSubmit(template);
            this.SubmitChanges();
        }

        public void Update(string id, NoteTemplate newValue)
        {
            var template = from t in Templates where t.Id.Equals(id) select t;
            foreach (NoteTemplate t in template)
            {
                t.Name = newValue.Name;
            }

            this.SubmitChanges();
        }
    }
}
