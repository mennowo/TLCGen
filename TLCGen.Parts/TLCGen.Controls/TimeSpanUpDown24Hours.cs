using System;

namespace TLCGen.Controls
{
	public class TimeSpanUpDown24Hours : Xceed.Wpf.Toolkit.TimeSpanUpDown
    {
		protected override void OnTextChanged( string previousValue, string currentValue)
		{

		}

        protected override TimeSpan? ConvertTextToValue(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            
            var hours = Convert.ToInt32(text.Substring(0, 2));
            int days = 0;
            if (hours == 24)
            {
                days = 1;
                hours = 0;
            }
            return new TimeSpan(days, hours, Convert.ToInt32(text.Substring(3, 2)), 0);
        }

        protected override string ConvertValueToText()
        {
            if (!this.Value.HasValue)
                return string.Empty;

            int days = this.Value.Value.Days;
            int hours = this.Value.Value.Hours;
            int mins = this.Value.Value.Minutes;
            if (days == 1)
                hours = 24;

            return hours.ToString("00") + ":" + mins.ToString("00");
        }

        protected override void OnIncrement()
        {
            if (this.Value.HasValue)
            {
                var v = this.Value.Value.Add(TimeSpan.FromMinutes(30));
                if(v.Days >= 1)
                {
                    this.Value = TimeSpan.FromDays(1);
                }
                else
                {
                    this.Value = v;
                }
            }
        }

        protected override void OnDecrement()
        {
            if (this.Value.HasValue)
            {
                var v = this.Value.Value.Add(TimeSpan.FromMinutes(-30));
                if(this.Value.Value < TimeSpan.FromTicks(0))
                {
                    this.Value = new TimeSpan(0);
                }
                else
                {
                    this.Value = v;
                }
            }
        }
    }
}
