using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.Domain
{
    public class Vote
    {
        public int Id {  get; set; }
        public int ChosenOption { get; set; }
        public int PollId { get; private set; }

        public class Builder
        {
            private Vote _object = new Vote();
            public Vote Build()
            {
                return _object;
            }

            public Builder WithId(int id)
            {
                _object.Id = id;
                return this;
            }

            public Builder WithPollId(int id)
            {
                _object.PollId = id;
                return this;
            }

            public Builder WithChosenOption(int id)
            {
                _object.ChosenOption = id;
                return this;
            }
        }
    }
}
