using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class MessageWithTagsDTO
    {
        public Guid Id { get; set; }

        public Guid MessageId { get; set; }
        [ForeignKey("MessageId")]
        public Message message { get; set; }

        public List<int?> TagId { get; set; }
        [ForeignKey("TagId")]

        public List<Tag?> tag { get; set; }
    }
}
