using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSurvey.Domain
{
    public class Option
    {
        public Option()
        {
            Description = string.Empty;
            PictureUrl = string.Empty;
            Poll = Poll.Empty;
        }
        public int Id { get; set; }
        public Poll Poll {  get; internal set; }
        public string Description { get; set; }

        public string PictureUrl { get; set; }
        public int VoteCount { get; set; }
        public DateTime UpdatedAt { get; set; }

        public void IncreaseVoteCount()
        {
            VoteCount++;
        }

        public class Builder
        {
            private readonly Option _object = new();
            public Option Build()
            {
                return _object;
            }

            public Builder WithId(int id)
            {
                _object.Id = id;
                return this;
            }

            public Builder WithPoll(Poll poll)
            {
                _object.Poll = poll;
                return this;
            }

            public Builder WithDescription(string description)
            {
                _object.Description = description;
                return this;
            }

            public Builder WithPictureUrl(string pictureUrl)
            {
                _object.PictureUrl = pictureUrl;
                return this;
            }

            public Builder WithVoteCount(int voteCount)
            {
                _object.VoteCount = voteCount;
                return this;
            }

            public Builder WithUpdatedAt(DateTime updatedAt)
            {
                _object.UpdatedAt = updatedAt;
                return this;
            }
        }
    }
}
