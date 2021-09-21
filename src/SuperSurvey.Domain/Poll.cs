using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperSurvey.Domain
{
    public class Poll
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime UpdatedAt { get; private set; }
        public List<Option> Options { get; private set; } = new List<Option>();

        public Option GetOption(int optionId)
        {
            return Options.Single(o => o.Id == optionId);
        }

        public void AddVote(Vote vote)
        {
            var option = Options.Single(o => o.Id == vote.ChosenOption);
            option.IncreaseVoteCount();
        }

        public class Builder
        {
            private Poll _object = new Poll();
            public Poll Build()
            {
                return _object;
            }

            public Builder WithId(int id)
            {
                _object.Id = id;
                return this;
            }

            public Builder WithName(string name)
            {
                _object.Name = name;
                return this;
            }

            public Builder WithExpiresAt(DateTime expiresAt)
            {
                _object.ExpiresAt = expiresAt;
                return this;
            }

            public Builder WithUpdatedAt(DateTime updatedAt)
            {
                _object.UpdatedAt = updatedAt;
                return this;
            }

            public Builder WithOption(Option option)
            {
                _object.Options.Add(option);
                option.Poll = _object;
                return this;
            }
        }
    }
}
