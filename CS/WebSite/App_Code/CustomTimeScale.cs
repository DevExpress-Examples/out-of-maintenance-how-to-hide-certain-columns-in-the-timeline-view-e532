using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DevExpress.XtraScheduler;

    /* Implement a base class for the scales of less than a day.
     * The visible time starts at 6AM on working days, 8AM on Saturdays, 9AM on Sundays.
     * End of the visible interval corresponds to 3PM on working days, 12PM on weekends.  
     */
    public abstract class LessThanADayTimeScale : TimeScale {
        // Holds the scale increment, in minutes.
        int scaleValue;
        TimeSpan scaleValueTime;

        public LessThanADayTimeScale(int ScaleValue) {
            scaleValue = ScaleValue;
            scaleValueTime = TimeSpan.FromMinutes(ScaleValue);
        }

        // Gets the start of the first time interval.
        protected virtual int FirstIntervalStart(DateTime date) {
            switch (date.DayOfWeek) {
                case DayOfWeek.Saturday:
                    return 8 * 60;
                case DayOfWeek.Sunday:
                    return 9 * 60;
                default:
                    return 6 * 60;
            }
        }

        // Gets the start of the last time interval.
        protected virtual int LastIntervalStart(DateTime date) {
            switch (date.DayOfWeek) {
                case DayOfWeek.Saturday:
                    return 12 * 60 - scaleValue;
                case DayOfWeek.Sunday:
                    return 12 * 60 - scaleValue;
                default:
                    return 15 * 60 - scaleValue;
            }
        }        

        // Gets the value used to order the scales.
        protected override TimeSpan SortingWeight {
            get { return TimeSpan.FromMinutes(scaleValue + 1); }
        }

        public override DateTime Floor(DateTime date) {
            
            // Performs edge calculations.
            if (date == DateTime.MinValue || date == DateTime.MaxValue)
                return RoundToScaleInterval(date);

            // Rounds down to the last interval in the previous date.
            if (date.TimeOfDay.TotalMinutes < FirstIntervalStart(date))
                return RoundToVisibleIntervalEdge(date.AddDays(-1), LastIntervalStart(date));
            
            // Rounds down to the last interval in the current date.
            if (date.TimeOfDay.TotalMinutes > LastIntervalStart(date))
                return RoundToVisibleIntervalEdge(date, LastIntervalStart(date));
            
            // Rounds down to the scale node.
            return RoundToScaleInterval(date);
        }

        protected DateTime RoundToVisibleIntervalEdge(DateTime dateTime, int minutes) {
            return dateTime.Date.AddMinutes(minutes);
        }
        protected DateTime RoundToScaleInterval(DateTime date) {
            return DevExpress.XtraScheduler.Native.DateTimeHelper.Floor(date, TimeSpan.FromMinutes(scaleValue));
        }
        // Checks for edge conditions.
        protected override bool HasNextDate(DateTime date) {
            return date <= (DateTime.MaxValue - scaleValueTime);
        }
        
        public override DateTime GetNextDate(DateTime date) {
            if (HasNextDate(date)) {
                return (date.TimeOfDay.TotalMinutes > LastIntervalStart(date) - scaleValue) ?
                    RoundToVisibleIntervalEdge(date.AddDays(1), FirstIntervalStart(date.AddDays(1))) : date.AddMinutes(scaleValue);
            }
            else return date;
        }
    }

#region  Custom Scales Implementation
    public class My20MinutesScale : LessThanADayTimeScale {
        const int myScaleValue = 20;
        public My20MinutesScale()
            : base(myScaleValue) {
        }

        protected override string DefaultDisplayFormat {
            get { return "HH:mm"; }
        }

        protected override string DefaultMenuCaption {
            get {
                return "My20Minutes";
            }
        }

    }
    public class My15MinutesScale : LessThanADayTimeScale {
        const int myScaleValue = 15;
        public My15MinutesScale()
            : base(myScaleValue) {
        }

        protected override string DefaultDisplayFormat {
            get { return "HH:mm"; }
        }

        protected override string DefaultMenuCaption {
            get {
                return "My15Minutes";
            }
        }
    
    }

    public class MyHourScale : LessThanADayTimeScale {
        const int myScaleValue = 60;
            public MyHourScale()
            : base(myScaleValue) {
        }


        protected override string DefaultDisplayFormat {
            get { return "hh tt"; }
        }

        protected override string DefaultMenuCaption {
            get {
                return "MyHour";
            }
        }
    
    }


public class CustomTimeScaleDay : LessThanADayTimeScale {
    
    const int myScaleValue = 1440;
        public CustomTimeScaleDay()
            : base(myScaleValue) 
        {
        }
    
    public override DateTime Floor(DateTime date) {

        // Performs edge calculations.
        if (date == DateTime.MinValue)
            return date.AddMinutes(FirstIntervalStart(date));

        DateTime start = date.Date;

        // Rounds down to the previous date.
        if (date.TimeOfDay.TotalMinutes < FirstIntervalStart(start))
            start = start.AddDays(-1);


        // Rounds down to the scale node.
        return start.AddMinutes(FirstIntervalStart(start));
    }

    protected override string DefaultDisplayFormat {
        get { return "d ddd"; }
    }

    protected override string DefaultMenuCaption {
        get {
            return "CustomDay";
        }
    }

}
#endregion