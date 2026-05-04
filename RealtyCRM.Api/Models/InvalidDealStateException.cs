using System;

namespace RealtyCRM.Api.Models
{
    /// <summary>
    /// Исключение, выбрасываемое при невалидном состоянии объекта недвижимости для совершения сделки.
    /// </summary>
    public class InvalidDealStateException : Exception
    {
        public InvalidDealStateException(string message) : base(message) { }
    }
}
