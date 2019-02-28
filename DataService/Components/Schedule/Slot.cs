namespace DataService.Components.Schedule
{
    public class Slot
    {
        public TimePoint StartPoint { get; set; }
        public TimePoint EndPoint => StartPoint.AddMinute(DurationMinute);
        public int DurationMinute { get; set; }

        public Slot(TimePoint startPoint, int durationMinute)
        {
            StartPoint = startPoint;
            DurationMinute = durationMinute;
        }

        public override string ToString()
        {
            return $"Start at: {StartPoint}, last in {DurationMinute} minutes";
        }
    }
}