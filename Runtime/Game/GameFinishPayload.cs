namespace Ouroboros.Common.Game
{
    public struct GameFinishPayload
    {
        public enum LossReason
        {
            PlayerDead,
            TimeIsUp,
            GiveUp,
            LeftArea,
        }

        public bool IsWin;
        public LossReason LossReasonType;
        public string SourceObjectId;
    }
}