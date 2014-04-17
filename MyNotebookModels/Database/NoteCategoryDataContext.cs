using MyNotebookModels.Interfaces;
using MyNotebookModels.Models;
using MyNotebookUtility.Common;
using System.Data.Linq;
using System.Linq;
using MyNotebookUtility.Tools;
using System.IO;


namespace MyNotebookModels.Database
{
    public class NoteCategoryDataContext : DataContext, INotebookDataContext<NoteCategory>
    {
        public NoteCategoryDataContext()
            : base(Constants.DB_NoteConfigPath)
        {
            if (!this.DatabaseExists())
            {
                IsolatedStorageHelper.CreateDir(Constants.DB_Path);
                this.CreateDatabase();
            }
        }

        public Table<NoteCategory> Categories;

        public void Insert(NoteCategory value)
        {
            var haveCategory = Categories.Select(p => p.Id == value.Id).Any();

            if (!haveCategory)
            {
                Categories.InsertOnSubmit(value);
                this.SubmitChanges();
            }
            else
            {
                Update(value.Id, value);
            }
        }

        public void Remove(string id)
        {
            var category = from c in Categories where c.Id.Equals(id) select c;
            foreach (var c in category)
            {
                Categories.DeleteOnSubmit(c);
            }

            this.SubmitChanges();
        }

        public void Update(string id, NoteCategory newValue)
        {
            var category = from c in Categories where c.Id.Equals(id) select c;
            foreach (var c in category)
            {
                c.Name = newValue.Name;
            }

            this.SubmitChanges();
        }
    }
}
