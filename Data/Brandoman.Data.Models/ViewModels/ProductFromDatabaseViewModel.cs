using System;
using System.Collections.Generic;
using System.Text;

namespace Brandoman.Data.Models.ViewModels
{
    public class ProductFromDatabaseViewModel
    {
        public int id { get; set; }

        public int product_category_id { get; set; }

        public int sex_id { get; set; }

        public string name { get; set; }

        public string content { get; set; }

        public string[] characteristics { get; set; }

        public int active { get; set; }

        public int locale_id { get; set; }
    }
}
