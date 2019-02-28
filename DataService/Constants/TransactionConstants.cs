namespace DataService.Constants
{
    public static class TransactionConstants
    {
        public static class TransactionState
        {
            public static readonly string Completed = "completed";
            public static readonly string TemporaryDeposit = "temporaryDeposit";
            
            public static string[] Array = {Completed, TemporaryDeposit};
        }

        public static class TransactionDetail
        {
            public static readonly string DepositTransaction = "Deposit transaction";
            public static readonly string TopupTransaction = "Topup transaction";
            public static readonly string WithdrawTransaction = "Withdraw transaction";
            public static readonly string PaidTransaction = "Paid transaction";
            public static readonly string BarberCompensationTransaction = "Barber cancelled booking";
            public static readonly string CustomerCompensationTransaction = "Customer cancelled booking";
        }
    }
}