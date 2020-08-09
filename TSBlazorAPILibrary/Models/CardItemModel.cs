using System;
using System.Collections.Generic;
using System.Text;

namespace TSBlazorAPILibrary.Models
{
    public class CardItemModel
    {
        public int Id { get; set; }
        public string CreatorId { get; set; }
        public string Title { get; set; }
        public string CreatorName { get; set; }
        public string Description { get; set; }

    }
}
