namespace MintaProjekt.Models
{
    public class PhoneNumber
    {
        public string CountryCode { get; set; } = "+36";
        public string? SelectedAreaCode { get; set; }
        public string? LocalPhoneNumber { get; set; }

        // Full phone number
        public override string ToString()
        {
            return $"{CountryCode}{SelectedAreaCode}{LocalPhoneNumber}";
        }

        // Static method to parse a full phone number into its parts
        public static PhoneNumber Parse(string fullPhoneNumber)
        {
            var phoneNumber = new PhoneNumber();
            if (!string.IsNullOrEmpty(fullPhoneNumber) && fullPhoneNumber.StartsWith("+36"))
            {
                phoneNumber.CountryCode = "+36";
                phoneNumber.SelectedAreaCode = fullPhoneNumber.Substring(3, 2);
                phoneNumber.LocalPhoneNumber = fullPhoneNumber.Substring(5);
            }
            return phoneNumber;
        }

        // Check for null values
        public bool HasInvalidProperties()
        {
            return string.IsNullOrWhiteSpace(CountryCode) ||
                   string.IsNullOrWhiteSpace(SelectedAreaCode) ||
                   string.IsNullOrWhiteSpace(LocalPhoneNumber);     
        }
    }

}
