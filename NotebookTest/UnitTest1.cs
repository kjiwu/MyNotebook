using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyNotebookModels.Models;
using MyNotebookModels.Database;
using MyNotebookUtility.Tools;
using MyNotebookUtility.Common;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace NotebookTest
{
    [TestClass]
    public class UnitTest1
    {
        private static string categoryId = Guid.NewGuid().ToString("n");

        [TestMethod]
        public void TestCategoryDatabase()
        {
            using (NoteCategoryDataContext database = new NoteCategoryDataContext())
            {
                NoteCategory category = new NoteCategory()
                {
                    Id = categoryId,
                    Name = "HelloWorld"
                };

                database.Insert(category);

                var categories = from c in database.Categories
                                 where c.Id == categoryId
                                 select c.Name;

                Assert.AreEqual("HelloWorld", categories.FirstOrDefault());

                Assert.AreNotEqual(0, database.Categories.Count());

                database.Update(categoryId, new NoteCategory()
                {
                    Name = "Unit Test"
                });

                var cn = from c in database.Categories
                         where c.Id == categoryId
                         select c.Name;

                Assert.AreEqual(1, cn.Count());
                Assert.AreEqual("Unit Test", cn.FirstOrDefault());

                Assert.AreNotEqual(0, database.Categories.Count());
                database.Remove(categoryId);

                Assert.AreEqual(3, database.Categories.Count());

                Assert.IsTrue(database.Categories.First(p => p.Id == "1") != null);
                Assert.IsTrue(database.Categories.First(p => p.Id == "2") != null);
                Assert.IsTrue(database.Categories.First(p => p.Id == "3") != null);
            }

            using (NoteTemplateDataContext db = new NoteTemplateDataContext())
            {
                string id = Guid.NewGuid().ToString("n");
                NoteTemplate template = new NoteTemplate();
                template.Id = id;
                template.Name = "OK";

                db.Insert(template);
                var templateArr = from t in db.Templates select t;
                Assert.AreEqual(1, templateArr.Count());

                template.Name = "NewValue";
                db.Update(template.Id, template);
                templateArr = from t in db.Templates where t.Name.Equals("NewValue") select t;
                Assert.AreEqual(1, templateArr.Count());

                db.Remove(id);
                templateArr = from t in db.Templates select t;
                Assert.AreEqual(0, templateArr.Count());                
            }
        }
    }
}
