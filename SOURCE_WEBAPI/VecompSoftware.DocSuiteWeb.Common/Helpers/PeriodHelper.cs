using System;

namespace VecompSoftware.DocSuiteWeb.Common.Helpers
{
    public class PeriodHelper
    {
        public static DateTimeOffset GetCurrentPeriodStart(string periodName, DateTimeOffset startindgPeriod)
        {
            DateTime firstQuarter = new DateTime(startindgPeriod.Year, 1, 1);
            DateTime secondQuarter = new DateTime(startindgPeriod.Year, 4, 1);
            DateTime thirdQuarter = new DateTime(startindgPeriod.Year, 7, 1);
            DateTime fourthQuarter = new DateTime(startindgPeriod.Year, 10, 1);
            string result = string.Concat(" - ", periodName); ;
            switch (periodName)
            {
                case "Annuale":
                    {
                        startindgPeriod = firstQuarter;
                        break;
                    }
                case "Semestrale":
                    {
                        if (startindgPeriod.Month < thirdQuarter.Month)
                        {
                            startindgPeriod = firstQuarter;
                        }
                        if (startindgPeriod.Month >= thirdQuarter.Month)
                        {
                            startindgPeriod = thirdQuarter;
                        }
                        break;
                    }
                case "Trimestrale":
                    {
                        if (startindgPeriod.Month >= firstQuarter.Month && startindgPeriod.Month < secondQuarter.Month)
                        {
                            startindgPeriod = firstQuarter;
                        }
                        if (startindgPeriod.Month >= secondQuarter.Month && startindgPeriod.Month < thirdQuarter.Month)
                        {
                            startindgPeriod = secondQuarter;
                        }
                        if (startindgPeriod.Month >= thirdQuarter.Month && startindgPeriod.Month < fourthQuarter.Month)
                        {
                            startindgPeriod = thirdQuarter;
                        }
                        if (startindgPeriod.Month >= fourthQuarter.Month)
                        {
                            startindgPeriod = fourthQuarter;
                        }
                        break;
                    }
                case "Mensile":
                    {
                        startindgPeriod = firstQuarter;
                        break;
                    }
            }
            return startindgPeriod;
        }
    }
}
