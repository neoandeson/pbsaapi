namespace DataService.Constants
{
    public static class ComplaintConstants
    {
        public static class ComplaintType
        {
            public static readonly string CustomerNotFound = "customerNotFound";
            public static readonly string LongTimeWaiting = "longTimeWaiting";
            
            public static string[] Array = {CustomerNotFound, LongTimeWaiting};
        }
        
        public static class ComplaintState
        {
            public static readonly string Pending = "pending";
            public static readonly string Approved = "approved";
            public static readonly string Rejected = "rejected";
            
            public static string[] Array = {Pending, Approved, Rejected};
        }
    }
}