using Syncfusion.UI.Xaml.Charts;
using System;
using System.Windows;

namespace Lab1.RandomGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private uint From { get; set; }
        private uint To { get; set; }
        private uint Multiplier { get; set; }
        private uint Mod { get; set; }
        private uint StartValue { get; set; }
        private uint CountOfElementsTextBox { get; set; }
        
        private static void VerifyThatIneNumberMortThenOther(uint bigger, uint less)
        {
            if (less > bigger)
            {
                throw new ArgumentOutOfRangeException($"{bigger} should be less then {less}");
            }
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            To = Convert.ToUInt32(toTextBox.Text);
            From = Convert.ToUInt32(fromTextBox.Text);

            Mod = Convert.ToUInt32(modTextBox.Text);
            StartValue = Convert.ToUInt32(startValueTextBox.Text);
            Multiplier = Convert.ToUInt32(multiplierTextBox.Text);
            CountOfElementsTextBox = Convert.ToUInt32(countOfElementsTextBox.Text);

            VerifyThatIneNumberMortThenOther(To, From);
            VerifyThatIneNumberMortThenOther(Mod, Multiplier);

            var generator = new Generator(StartValue, Multiplier, Mod, From, To, CountOfElementsTextBox);

            CreateChart(generator);

            dispersionTextBox.Text = generator.Dispersion.ToString();
            expectationTextBox.Text = generator.Expectation.ToString();
            sigmaLimitTextBox.Text = generator.SigmaLimit.ToString();

            ShowResult(generator);
        }

        private static void ShowResult(Generator generator)
        {
            var window = new ShowPeriodWindow();

            var period = generator.Period;
            var aperiods = generator.APeriod;

            GetValuesFromPeriod(period, generator, window, true);

            foreach (var aperiod in aperiods)
            {
                GetValuesFromPeriod(aperiod, generator, window, false);
            }

            window.Show();
        }


        private static void GetValuesFromPeriod(int period, Generator generator, ShowPeriodWindow window, bool isPeriod)
        {
            var values = generator.GetValuesFromPeriod(period);

            if (isPeriod)
            {
                foreach (var value in values)
                {
                    window.PeriodTextBlock.Text += value + "  ";
                }

                window.PeriodCountTextBlock.Text = period.ToString();
            }
            else
            {
                foreach (var value in values)
                {
                    window.APeriodTextBlock.Text += value + "  ";
                }

                window.APeriodTextBlock.Text += ";   ";

                window.APeriodCountTextBlock.Text = period.ToString();
            }
        }

        private void CreateChart(Generator generator)
        {
            MyChart.Series.Clear();

            //Adding Legends for the chart
            var legend = new ChartLegend();
            MyChart.Legend = legend;

            //Initializing column series
            var series = new ColumnSeries
            {
                ItemsSource = generator.GetResult(),
                XBindingPath = "Range",
                YBindingPath = "InnerNumbers",
                ShowTooltip = true,
                Label = "InnerNumbers"
            };
            series.Label = "Range";

            //Setting adornment to the chart series
            series.AdornmentsInfo = new ChartAdornmentInfo() { ShowLabel = true };

            //Adding Series to the Chart Series Collection
            MyChart.Series.Add(series);
        }
    }
}