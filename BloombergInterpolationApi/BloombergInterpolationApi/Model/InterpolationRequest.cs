using System;
using System.Collections.Generic;

namespace BloombergInterpolationApi.Model
{
    public class InterpolationRequest
    {
        public DateTime RootDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public  List<InterpolationRequestItem> InterpolationRequestItems{ get; set; }
    }

    public class InterpolationRequestItem
    {
        public string TickerName { get; set; }
        public decimal Value { get; set; }
    }
    
}