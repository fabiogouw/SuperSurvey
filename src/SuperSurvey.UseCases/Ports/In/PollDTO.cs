using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases.Ports.In
{
    public class PollDTO
    {
        public class OptionDTO
        {
            public OptionDTO()
            {
                Description = string.Empty;
                PictureUrl = string.Empty;
            }
            public int Id { get; set; }
            public string Description { get; set; }
            public string PictureUrl { get; set; }
        }
        public PollDTO()
        {
            Name = string.Empty;
        }
        public int Id {  get; set; } 
        public string Name {  get; set; }
        public List<OptionDTO> Options { get; set; } = new List<OptionDTO>();
    }
}
