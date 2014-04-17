using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNotebookModels.Models
{
    [Table]
    public class NoteTemplate
    {
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public string Id { get; set; }

        [Column(CanBeNull = false)]
        public string Name { get; set; }
    }
}
