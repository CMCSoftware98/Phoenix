using Google.Cloud.Firestore;
using Phoenix.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.Shared.Models
{
    [FirestoreData]
    public class MatchResult
    {
        [FirestoreProperty]
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

        [FirestoreProperty]
        public string Url { get; set; }

        [FirestoreProperty]
        public MatchCondition MatchCondition { get; set; }

        [FirestoreProperty]
        public int CTSideScore { get; set; }

        [FirestoreProperty]
        public int TSideScore { get; set; }

        [FirestoreProperty]
        public List<RoundWinner> RoundWinners { get; set; } = new List<RoundWinner>();
    }
}
