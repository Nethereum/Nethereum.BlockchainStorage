using System;

namespace Nethereum.RPC.Eth.DTOs
{
    public static class Address
    {
        public const string EmptyAsHex = "0x0";
    }

    public static class AddressExtensions
    {
        public const string EmptyAddressHex = Address.EmptyAsHex;

        public static bool IsAnEmptyAddress(this string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return true;

            return address == EmptyAddressHex;
        }

        public static bool IsNotAnEmptyAddress(this string address)
        {
            return !address.IsAnEmptyAddress();
        }

        public static string AddressValueOrEmpty(this string address)
        {
            return address.IsAnEmptyAddress() ? EmptyAddressHex : address;
        }

        public static bool EqualsAddress(this string address1, string address2)
        {
            return (address1.AddressValueOrEmpty())
                .Equals(address2.AddressValueOrEmpty(), StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsEmptyOrEqualsAddress(this string address1, string candidate)
        {
            return address1.IsAnEmptyAddress() || address1.EqualsAddress(candidate);
        }
    }
}
