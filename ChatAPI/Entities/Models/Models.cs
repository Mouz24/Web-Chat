using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Message
    {
        public Guid Id { get; set; }    
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid SenderId { get; set; }
    }

    public class Tag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class MessageWithTags
    {
        public Guid Id { get; set; }

        public Guid MessageId { get; set; }
        [ForeignKey("MessageId")]
        public Message message { get; set; }

        public int? TagId { get; set; }
        [ForeignKey("TagId")]
        public Tag? tag { get; set; }
    }
}
