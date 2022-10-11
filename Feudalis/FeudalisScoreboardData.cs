using System;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Feudalis
{
    public class FeudalisScoreboardData : IScoreboardData
    {
        public MissionScoreboardComponent.ScoreboardHeader[] GetScoreboardHeaders()
        {
            return new []
            {
                new MissionScoreboardComponent.ScoreboardHeader("avatar", missionPeer => "", bot => ""),
                new MissionScoreboardComponent.ScoreboardHeader("name", (missionPeer) => missionPeer.GetComponent<MissionPeer>().DisplayedName, bot => new TextObject("{=hvQSOi79}Bot").ToString()),
                new MissionScoreboardComponent.ScoreboardHeader("bounty", missionPeer => missionPeer.GetComponent<FeudalisMissionRepresentative>().Bounty.ToString(), bot => 0.ToString()),
            };
        }
    }
}
