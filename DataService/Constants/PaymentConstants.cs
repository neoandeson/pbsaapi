namespace DataService.Constants
{
    public static class PaymentConstants
    {
        public static class PaymentType
        {
            public static readonly string Cash = "cash";
            public static readonly string Wallet = "wallet";
            
            public static string[] Array = {Cash, Wallet};
        }
        
        public static class PaymentAmount
        {
            public static readonly decimal MinimumBalance = 0;
            public static readonly decimal BookingDepositAmount = 10000;
        }

        public static class PaymentDescription
        {
            public static readonly string NotAvailable_VN = "Không xác định được";
            public static readonly string NotPayable_VN = "Không thể thanh toán";
            public static readonly string Payable_VN = "Có thể thanh toán";
            public static readonly string NotSuccess_VN = "Không thành công";
            public static readonly string NotEnoughMoney_VN = "Không đủ tiền";

            public static readonly string Payable = "Payable";
            public static readonly string NotPayable = "Not Payable";
            public static readonly string NotAvailable = "Not Available";
        }
    }
}