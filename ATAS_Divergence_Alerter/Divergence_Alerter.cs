namespace ATAS.Indicators
{
    using System.Windows.Media;
    using System.ComponentModel.DataAnnotations;
    public class Divergence_Alerter : Indicator
    {
        [Display(ResourceType = null, GroupName = "Configuration", Name = "Activate alert at bar close", Order = 001)]
        public bool AlertAtNewBar { get; set; } = false;

        [Display(ResourceType = null, GroupName = "Configuration", Name = "Activate alert for current bar", Order = 001)]
        public bool AlertAtCurrentBar { get; set; } = false;

        [Display(ResourceType = null, GroupName = "Alert Style", Name = "Alert file", Order = 101)]
        public string AlertFile { get; set; } = "alert1";

        [Display(ResourceType = null, GroupName = "Alert Style", Name = "Background color", Order = 102)]
        public Color BackgroundColor { get; set; }

        [Display(ResourceType = null, GroupName = "Alert Style", Name = "Text color", Order = 103)]
        public Color TextColor { get; set; }

        protected int _lastBarNumber = 0;
        protected bool _currentBarAlreadyAlerted = false;
        protected string AlertString { get; set; } = "Divergence Alert - ";

        public Divergence_Alerter() : base(true)
        {

        }

        protected override void OnCalculate(int bar, decimal value)
        {
            if (bar != _lastBarNumber)
            {
                _lastBarNumber = bar;
                _currentBarAlreadyAlerted = false;

                if (CurrentBar - 1 == _lastBarNumber)
                {
                    if (AlertAtNewBar)
                    {
                        CheckDivergence(bar - 1);
                    }
                }
            }

            if (AlertAtCurrentBar)
            {
                if (CurrentBar - 1 == _lastBarNumber)
                {
                    CheckDivergence(bar);
                }
            }
        }

        private void CheckDivergence(int bar)
        {
            IndicatorCandle candle = GetCandle(bar);

            if (candle.Delta < 0 && candle.Close > candle.Open && !_currentBarAlreadyAlerted)
            {
                /* candle is long and divergence is direction is short*/
                string label = AlertString + $"Bar Direction: Long - Divergence Direction: Short";

                AddAlert(AlertFile, InstrumentInfo.Instrument, label, BackgroundColor, TextColor);
                _currentBarAlreadyAlerted = true;                
            }
            else if (candle.Delta > 0 && candle.Open > candle.Close && !_currentBarAlreadyAlerted)
            {
                /* candle is short and divergence direction is long */
                string label = AlertString + $"Bar Direction: Short - Divergence Direction: Long";
                AddAlert(AlertFile, InstrumentInfo.Instrument, label, BackgroundColor, TextColor);
                _currentBarAlreadyAlerted = true;                
            }
            else if (candle.Delta > 0 && candle.Close > candle.Open && _currentBarAlreadyAlerted)
            {
                _currentBarAlreadyAlerted = false;                
            }
            else if (candle.Delta < 0 && candle.Open > candle.Close && _currentBarAlreadyAlerted)
            {
                _currentBarAlreadyAlerted = false;                
            }
            else
            {
                // do nothing
            }
        }
    }
}

