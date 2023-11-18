using System;

namespace MPI_2.DTOs
{
    [Serializable]
    public class Product
    {
        public int ProductId { get; set; }
        public string Productname { get; set; }

        public override string ToString()
        {
            return $"{ProductId}-{Productname}";
        }
    }
}
