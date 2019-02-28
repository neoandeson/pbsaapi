using System;
using System.Collections.Generic;

namespace DataService.Components.Schedule
{
    public class Scheduler
    {
        private List<Slot> BookedList { get; set; }

        public List<FreeTime> AvailableList { get; private set; }

        public Scheduler(List<FreeTime> availableList)
        {
            AvailableList = availableList;
            BookedList = new List<Slot>();
        }

        public void AddBookedSlot(Slot slot)
        {
            BookedList.Add(slot);
            
            List<FreeTime> toRemove = new List<FreeTime>();
            List<FreeTime> toAdd = new List<FreeTime>();
            foreach (var freeTime in AvailableList)
            {
                if (slot.EndPoint.GetMinuteLength() <= freeTime.Start.GetMinuteLength() ||
                    freeTime.End.GetMinuteLength() <= slot.StartPoint.GetMinuteLength())
                {
                    continue;
                }

                if (freeTime.Start.GetMinuteLength() <= slot.StartPoint.GetMinuteLength() &&
                    slot.EndPoint.GetMinuteLength() <= freeTime.End.GetMinuteLength())
                {
                    toRemove.Add(freeTime);
                    toAdd.AddRange(freeTime.SplitFreeTimes(slot));
                    continue;
                }

                if (slot.StartPoint.GetMinuteLength() <= freeTime.Start.GetMinuteLength() &&
                    freeTime.End.GetMinuteLength() <= slot.EndPoint.GetMinuteLength())
                {
                    toRemove.Add(freeTime);
                    continue;
                }

                if (slot.EndPoint.GetMinuteLength() <= freeTime.End.GetMinuteLength())
                {
                    freeTime.Start = slot.EndPoint;
                    if (freeTime.GetDurationMinute() == 0)
                    {
                        toRemove.Add(freeTime);
                    }
                    continue;
                }
                
                if (freeTime.Start.GetMinuteLength() <= slot.StartPoint.GetMinuteLength())
                {
                    freeTime.End = slot.StartPoint;
                    if (freeTime.GetDurationMinute() == 0)
                    {
                        toRemove.Add(freeTime);
                    }
                }
            }
            
            foreach (var freeTime in toRemove)
            {
                AvailableList.Remove(freeTime);
            }
            
            AvailableList.AddRange(toAdd);
        }

        public void AddBookedSlots(IEnumerable<Slot> slots)
        {
            if (slots == null) return;
            foreach (var slot in slots)
            {
                AddBookedSlot(slot);
            }
        }

        public List<TimePoint> GetAvailableTimes(int durationMinute)
        {
            List<TimePoint> result = new List<TimePoint>();
            
            AvailableList.ForEach(time =>
            {
                result.AddRange(time.CalculateAvailableSlots(durationMinute));
            });
            
            result.Sort((p1, p2) => p1.GetMinuteLength() - p2.GetMinuteLength());

            return result;
        }

        public bool Book(Slot bookedSlot)
        {
            TimePoint bookedTimePoint = bookedSlot.StartPoint;
            int durationMinute = bookedSlot.DurationMinute;

            FreeTime freeTime = null;
            
            AvailableList.ForEach(time =>
            {
                List<TimePoint> availableSlots = time.CalculateAvailableSlots(durationMinute);
                if (availableSlots.Find(point => point.Equals(bookedTimePoint)) != null)
                {
                    freeTime = time;
                }
            });

            if (freeTime == null)
            {
                //Console.WriteLine("There is no available time for you");
                return false;
            }

            AvailableList.Remove(freeTime);
            AvailableList.AddRange(freeTime.SplitFreeTimes(bookedSlot));
            AvailableList.Sort((ft1, ft2) => ft1.Start.GetMinuteLength() - ft2.Start.GetMinuteLength());
                
            BookedList.Add(bookedSlot);
            BookedList.Sort((slot1, slot2) => slot1.StartPoint.GetMinuteLength() - slot2.StartPoint.GetMinuteLength());
                
            //Console.WriteLine("You have booked successfully");
            return true;
        }

        public void PrintInfo()
        {
            Console.WriteLine($"Booked slots: {BookedList.Count}");
            BookedList.ForEach(Console.WriteLine);
            Console.WriteLine($"Available time: {AvailableList.Count}");
            AvailableList.ForEach(Console.WriteLine);
        }
    }
}