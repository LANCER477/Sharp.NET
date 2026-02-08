using SharpKnP321.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Data.Dto
{
    [TableName("News")]
    internal class News
    {
        public Guid     Id       { get; set; }
        public Guid     AuthorId { get; set; }
        public String   Title    { get; set; } = null!;
        public String   Content  { get; set; } = null!;
        public DateTime Moment   { get; set; }

        public override string ToString() => 
            $"{Moment:dd.MM} {Title} <|> {Content[..30]}... ({Id.ToString()[..3]}...)";
    }
}
