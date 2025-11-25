
namespace Ouroboros.Common.Performance
{
    public interface IStatsTrackable
    {
        string Key { get; }
        void StartTracking();
        void StopTracking();
        void ResetStats();
    }
}