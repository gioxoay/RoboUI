namespace RoboUI
{
    internal static class Constants
    {
        public const string ShortDatePattern = "dd/MM/yyyy";

        public const string FullDatePattern = "dd/MM/yyyy HH:mm:ss";

        public static class Validation
        {
            public const string Date = "Please enter a valid date.";
            public const string Digits = "Please enter only digits.";
            public const string Email = "Please enter a valid email address.";
            public const string EqualTo = "Please enter the same value again.";
            public const string MaxLength = "Please enter no more than {0} characters.";
            public const string MinLength = "Please enter at least {0} characters.";
            public const string Number = "Please enter a valid number.";
            public const string PhoneNumber = "Please enter a valid phone number.";
            public const string Range = "Please enter a value between {0} and {1}.";
            public const string RangeLength = "Please enter a value between {0} and {1} characters long.";
            public const string RangeMax = "Please enter a value less than or equal to {0}.";
            public const string RangeMin = "Please enter a value greater than or equal to {0}.";
            public const string Required = "Please enter a value.";
            public const string Url = "Please enter a valid URL.";
            public const string CreditCard = "Please enter a valid credit card number.";
            public const string LettersOnly = "Please enter letters only.";
        }
    }
}
