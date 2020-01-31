using System;

namespace LazyEntityGraph.EntityFramework.Tests.Model.TPC
{
    public class SubscriptionInvoice : Invoice
    {
        public int Period { get; set; }
    }
}
