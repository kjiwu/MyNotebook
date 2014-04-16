using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MyNotebookCustomControls.TimeAndWeather
{
    [TemplatePart(Name = DotName, Type = typeof(TextBlock))]
    public class Watch : Control
    {
        public const string DotName = "tbkDot";

        TextBlock tbkDot;

        #region constructor

        public Watch()
        {
            DefaultStyleKey = typeof(Watch);

            Hour = DateTime.Now.Hour.ToString("d2");
            Minute = DateTime.Now.Minute.ToString("d2");
            Week = FormatWeek(DateTime.Now.DayOfWeek.ToString(), Thread.CurrentThread.CurrentCulture);
            Year = DateTime.Now.ToString("MM/dd");

            var ob = Observable.GenerateWithTime(0, x => true, x => DateTime.Now, x => TimeSpan.FromSeconds(1), x => x + 1);
            ob.ObserveOnDispatcher().Subscribe(time =>
            {
                Hour = time.Hour.ToString("d2");
                Minute = time.Minute.ToString("d2");

                Week = FormatWeek(time.DayOfWeek.ToString(), Thread.CurrentThread.CurrentCulture);
                Year = time.ToString("MM/dd");

                Debug.WriteLine(time);
            });
        }

        #endregion

        #region OnApplyTemplate

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            tbkDot = GetTemplateChild(DotName) as TextBlock;
            if (null != tbkDot)
            {
                DotAnimation();
            }
        }

        #endregion

        #region DotAnimation

        private void DotAnimation()
        {
            if (null == tbkDot)
                return;

            Storyboard storyboard = new Storyboard();

            DoubleAnimation f = new DoubleAnimation();
            f.From = 0;
            f.To = 1;
            f.Duration = TimeSpan.FromSeconds(1);
            f.RepeatBehavior = RepeatBehavior.Forever;
            f.AutoReverse = true;

            Storyboard.SetTarget(f, tbkDot);
            Storyboard.SetTargetProperty(f, new PropertyPath("UIElement.Opacity"));
            storyboard.Children.Add(f);
            storyboard.Begin();
        }

        #endregion

        #region Hour

        public static readonly DependencyProperty HourProperty = DependencyProperty.Register("Hour", typeof(string), typeof(Watch), new PropertyMetadata(default(string)));

        public string Hour
        {
            get
            {
                return GetValue(HourProperty) as string;
            }
            set
            {
                SetValue(HourProperty, value);
            }
        }

        #endregion

        #region Minute

        public static readonly DependencyProperty MinuteProperty = DependencyProperty.Register("Minute", typeof(string), typeof(Watch), new PropertyMetadata(default(string)));

        public string Minute
        {
            get
            {
                return GetValue(MinuteProperty) as string;
            }
            set
            {
                SetValue(MinuteProperty, value);
            }
        }

        #endregion

        #region Week

        public static readonly DependencyProperty WeekProperty = DependencyProperty.Register("Week", typeof(string), typeof(Watch), new PropertyMetadata(default(string)));

        public string Week
        {
            get
            {
                return GetValue(WeekProperty) as string;
            }
            set
            {
                SetValue(WeekProperty, value);
            }
        }

        #endregion

        #region Year

        public static readonly DependencyProperty YearProperty = DependencyProperty.Register("Year", typeof(string), typeof(Watch), new PropertyMetadata(default(string)));

        public string Year
        {
            get
            {
                return GetValue(YearProperty) as string;
            }
            set
            {
                SetValue(YearProperty, value);
            }
        }

        #endregion

        #region FormatWeek

        private string FormatWeek(string value, CultureInfo culture)
        {
            string result = value;

            switch (culture.TwoLetterISOLanguageName)
            {
                case "zh":
                    switch (result)
                    {
                        case "Monday":
                            result = "星期一";
                            break;
                        case "Tuesday":
                            result = "星期二";
                            break;
                        case "Wednesday":
                            result = "星期三";
                            break;
                        case "Thursday":
                            result = "星期四";
                            break;
                        case "Friday":
                            result = "星期五";
                            break;
                        case "Saturday ":
                            result = "星期六";
                            break;
                        case "Sunday ":
                            result = "星期天";
                            break;

                    }
                    break;
            }

            return result;
        }

        #endregion
    }
}
