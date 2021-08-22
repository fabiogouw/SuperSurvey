using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.UseCases.Ports.In
{
    public class PollDTO
    {
        public int Id {  get; set; } 
        public string Name {  get; set; }
        public List<string> Options { get; private set; } = new List<string>();
    }
}
