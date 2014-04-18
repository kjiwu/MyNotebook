using MyNotebookModels.Interfaces;
using MyNotebookModels.Models;
using MyNotebookUtility.Common;
using System.Data.Linq;
using System.Linq;
using MyNotebookUtility.Tools;
using System.IO;
using MyNotebookUtility.Resources;
using System.Collections.Generic;


namespace MyNotebookModels.Database
{
    public class NoteCategoryDataContext : NotebookDataContextBase, INotebookDataContext<NoteCategory>
    {
        public NoteCategoryDataContext()
            : base(Constants.DB_NoteConfigPath)
        {
            InitSystemCategories();
        }

        public Table<NoteCategory> Categories
        {
            get
            {
                return this.GetTable<NoteCategory>();
            }
        }

        private List<string> systemCategoryIds = new List<string>() { "1", "2", "3" };

        private void InitSystemCategories()
        {
            Insert(new NoteCategory()
            {
                Id = "1",
                Name = AppResources.NoteCategory_Life
            });

            Insert(new NoteCategory()
            {
                Id = "2",
                Name = AppResources.NoteCategory_Leisure
            });

            Insert(new NoteCategory()
            {
                Id = "3",
                Name = AppResources.NoteCategory_Rural
            });
        }

        public void Insert(NoteCategory value)
        {
            var haveCategory = Categories.Any(x => x.Id.Equals(value.Id));

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
            if (systemCategoryIds.Contains(id))
                return;

            var category = from c in Categories where c.Id.Equals(id) select c;
            foreach (var c in category)
            {
                Categories.DeleteOnSubmit(c);
            }

            this.SubmitChanges();
        }

        public void Update(string id, NoteCategory newValue)
        {
            if (systemCategoryIds.Contains(id))
                return;

            var category = from c in Categories where c.Id.Equals(id) select c;
            foreach (var c in category)
            {
                c.Name = newValue.Name;
            }

            this.SubmitChanges();
        }
    }
}
