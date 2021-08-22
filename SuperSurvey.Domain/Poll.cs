using System;

namespace SuperSurvey.Domain
{
    public class Poll
    {
        private Option[] _options;

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Expiration { get; set; }

        public Option GetOption(string option)
        {
            throw new NotImplementedException();
        }
        public Option[] GetResults()
        {
            throw new NotImplementedException();
        }
        public void AddVote(Vote vote)
        {

        }
    }
}
