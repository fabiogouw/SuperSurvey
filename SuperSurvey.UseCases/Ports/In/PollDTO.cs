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
            public int Id { get; set; }
            public string Description { get; set; }
        }
        public int Id {  get; set; } 
        public string Name {  get; set; }
        public List<OptionDTO> Options { get; set; } = new List<OptionDTO>();
    }
}
