using System;
using System.Collections.Generic;

namespace BloombergInterpolationApi.Model
{
    public class InterpolationResponse
    {
        public List<InterpolationResponseItem> InterpolationResponseItems { get; set; }
    }

    public class InterpolationResponseItem
    {
        public DateTime Date { get; set; }
        public decimal? Value { get; set; }
    }
}