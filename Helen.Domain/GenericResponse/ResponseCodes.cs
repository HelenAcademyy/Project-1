using System;

namespace Helen.Domain.GenericResponse
{
    public static class ResponseCodes
    {
        public static (string Code, string Description) Lagos()
        {
            return ("1000", "Lagos");
        }

        public static (string Code, string Description) Nigeria()
        {
            return ("1001", "Nigeria");
        }

        public static (string Code, string Description) NorthAfrica()
        {
            return ("1011", "NorthAfrica");
        }


        public static (string Code, string Description) WestAfrica()
        {
            return ("1011", "Bad request");
        }
        public static (string Code, string Description) EastAfrica()
        {
            return ("1110", "Bad request");
        }
        public static (string Code, string Description) International()
        {
            return ("111", "Bad request");
        }
    }
}
