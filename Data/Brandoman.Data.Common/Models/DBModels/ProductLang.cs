﻿namespace Brandoman.Data.Common.Models.DBModels
{
    public class ProductLang : BaseDeletableModel<int>
    {
        public Lang Lang { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public bool Active { get; set; }
    }
}
