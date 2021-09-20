using System;
using System.Collections;
using System.Collections.Generic;

namespace DockerIt.Entities
{
    public class Order
    {
        public Order()
        {
            OrderLines = new HashSet<OrderLine>();
        }

        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public int SalespersonPersonID { get; set; }
        public int ContactPersonID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public bool IsUndersupplyBackordered { get; set; }
        public int LastEditedBy { get; set; }

        public ICollection<OrderLine> OrderLines { get; private set; }
    }
}
