using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloombergInterpolationApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BloombergInterpolationApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BloombergInterpolationController : ControllerBase
    {
        [HttpPost]
        public InterpolationResponse GetInterpolationResponse(InterpolationRequest input)
        {
            var tickerNameValueList = new List<DateValueDto>();
            foreach (var item in input.InterpolationRequestItems)
            {
                GetDateByTickerNameForFx(item.TickerName, input.RootDate);
                var tickerDate = GetDateByTickerNameForFx(item.TickerName, input.RootDate);
                tickerNameValueList.Add(new DateValueDto
                {
                    Date = tickerDate,
                    Value = item.Value
                });
            }

            var myList = FillInterpolated(tickerNameValueList);
            var response = new InterpolationResponse();
            var indexDate = input.StartDate;
            var items = new List<InterpolationResponseItem>();
            while (indexDate<=input.EndDate)
            {

                    items.Add(new InterpolationResponseItem
                    {
                        Date = indexDate,
                        Value = myList.FirstOrDefault(o=>o.Date==indexDate)?.Value
                    });

                response.InterpolationResponseItems=items;
                indexDate=indexDate.AddDays(1);
            }
            return response;
        }


        private static DateTime GetDateByTickerNameForFx(string tickerName, DateTime rootDate)
        {
            string[] tickerNameSplit = tickerName.Split(' ');
            string lastStr = tickerNameSplit.Last().ToUpper();

            int num = 0;
            string a = "";

            for (int i = 0; i < lastStr.Length; i++)
            {
                if (Char.IsDigit(lastStr[i]))
                {
                    a += lastStr[i];
                }
            }

            if (a.Length > 0)
            {
                num = int.Parse(a);


                if (lastStr.Contains("M"))
                {
                    return rootDate.AddMonths(num);
                }
                else if (lastStr.Contains("W"))
                {
                    return rootDate.AddDays(num * 7);
                }
                else if (lastStr.Contains("Y"))
                {
                    return rootDate.AddYears(num);
                }
                else if (lastStr.Contains("D"))
                {
                    return rootDate.AddDays(num);
                }
            }

            if (lastStr.Contains("ON"))
            {
                return rootDate;
            }

            return default(DateTime);
        }

        private List<DateValueDto> FillInterpolated(List<DateValueDto> dateValueDtos)
        {
            dateValueDtos = dateValueDtos.OrderBy(o => o.Date).ToList();
            if (dateValueDtos.Count == 0)
            {
                return null;
            }

            var response = new List<DateValueDto>();

            for (int i = 0; i < dateValueDtos.Count-1 ; i++)
            {
                var indexDate = dateValueDtos[i].Date;
                var endDate = dateValueDtos[i + 1].Date;
                response.Add(new DateValueDto
                {
                    Date = dateValueDtos[i].Date,
                    Value = dateValueDtos[i].Value
                });
                while (indexDate<endDate.AddDays(-1))
                {
                    indexDate = indexDate.AddDays(1);
                    var interpolatedValue=GetLinearInterpolatedValue(dateValueDtos[i].Value
                        , dateValueDtos[i+1].Value
                        , dateValueDtos[i].Date
                        , dateValueDtos[i+1].Date
                        , indexDate);
                    response.Add(new DateValueDto
                    {
                        Date = indexDate,
                        Value = interpolatedValue
                    });
                }
            }
            return response.OrderBy(o => o.Date).ToList();

        }

        private decimal GetLinearInterpolatedValue(decimal valueStart, decimal valueFinal, DateTime startDate,
            DateTime endDate, DateTime expectedDate)
        {
            int dateDifferent = (endDate - startDate).Days;
            int dateDifferentMultiply = (expectedDate - startDate).Days;
            return valueStart + (dateDifferentMultiply * ((valueFinal - valueStart) / dateDifferent));
        }
    }
}