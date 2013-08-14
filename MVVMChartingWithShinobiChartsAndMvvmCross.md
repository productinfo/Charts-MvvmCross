# MVVM Charting with ShinobiCharts and MvvmCross

## Introduction

Model-View-ViewModel is a software design pattern made popular by Microsoft with
the introduction of WPF. It excels at separation of concerns in scenarios which
contain a persistent view, such as in desktop and mobile applications.

In this post we'll give a quick summary of the MVVM design pattern and its
application to mobile development, including looking at MvvmCross. We'll also
create a ShinobiChart suitable for use with MVVM data-binding.

DIAGRAM

## Model-View-ViewModel

The principle behind MVVM is that business logic is contained within the model
object, display logic within the view-model and the actual layout within the
view. The view communicates with the view-model via databinding - that is to say
that there are data properties on the view-model which the view understands how
to display, and consequently update as the user interacts with the UI. Thus the
view needs no logic associated with the data itself, and the view-model needs
to know nothing about how the data is displayed.

This means that different views can be placed on the front of different
view-models - e.g. a chart or a grid could represent the same data. Extending this
means that view-models can be shared cross-platform - e.g. you could write your
view-models in C#, and then share them between iOS, Android, WP8, WPF, Silverlight
and Windows 8.

In order to use the MVVM pattern quite a lot of groundwork has to be laid - the
mechanics of performing the data binding between views and view-models isn't
generally a language feature. This is where the rather excellent MvvmCross comes
in.


## MvvmCross

MvvmCross is a project which implements the MVVM pattern in a cross-platform
manner. It is written in C# and supports the iOS, Android, WP, Windows8 and
Silverlight platforms. The view-models can be put in a portable class library
allowing easy sharing between the different platforms. Then you would just need
to write each of the view projects - one for each of the platforms you desire.

I encourage you to take a look at the MvvmCross project - it's really quite
established and fun to use. My only mild criticism is the lack of documentation - 
Stuart has made over 30 really detailed videos, and further tutorial projects,
but other written documentation is currently a little lacking. It'd be a great
place to get started contributing to the open source world for anybody interested.


## MvvmCross with ShinobiCharts

This post won't go into loads of detail about how to get MvvmCross working for
your project - there is plenty of direction out there to explain that. Instead
we're going to look at how to bind a view model to a ShinobiChart. We're going
to develop the solution in Xamarin Studio - but you can use Visual Studio, provided
you've got it set up correctly to interface with Xamarin.


### Creating a View Model

We're going to build an app which shows sine waves of given frequencies, with
a slider control which allows changing the frequency of the waves:

SCREENSHOT HERE

In our `ShinobiDemo.Core` portable library project we'll create `ChartViewModel`,
which can inherit from `MvxViewModel`. This allows property changed notifications
to be processed correctly by the framework.

We need 2 properties on the view-model - one to provide the frequency, one the
resultant data points (which will be plotted):

    private IEnumerable<IEnumerable<DataPoint>> _source;
    public IEnumerable<IEnumerable<DataPoint>> Source {
        get { return _source; }
        set {
            _source = value;
            RaisePropertyChanged (() => Source);
        }
    }

    private double _frequency;
    public double Frequency {
        get { return _frequency;}
        set {
            _frequency = value;
            UpdateDataPoints ();
            RaisePropertyChanged (() => Frequency);
        }
    }

The only significant part of these properties is that they fire the notification
that a property has changed when they are set. Changing the frequency also
causes a call to `UpdateDataPoints`:

    private void UpdateDataPoints()
    {
        var dps = new List<List<DataPoint>>();
        // create the upper and lower component
        var upperHarmonic = new List<DataPoint>();
        var lowerHarmonic = new List<DataPoint>();
        for (double phase = 0; phase < Math.PI; phase+= (Math.PI / 100))
        {
            upperHarmonic.Add(new DataPoint(phase, Math.Sin(phase * this.Frequency) + this.Frequency * 2.5));
            lowerHarmonic.Add(new DataPoint(phase, Math.Sin(phase * this.Frequency + Math.PI) + this.Frequency * 2.5));
        }

        // add each to the collection
        dps.Add(upperHarmonic);
        dps.Add(lowerHarmonic);

        // And then save this off - making sure to use the setter
        this.Source = dps;
    }

This method generates a set of data points which represent a wave at the
specified frequency. It's important to note here that we've created a completely
generic `DataPoint` class, which stores a pair of `double` values:

    public class DataPoint
    {
        public DataPoint (double xValue, double yValue)
        {
            this.XValue = xValue;
            this.YValue = yValue;
        }

        private double _xValue;
        public double XValue {
            get { return _xValue;}
            set { _xValue = value;}
        }

        private double _yValue;
        public double YValue {
            get { return _yValue;}
            set { _yValue = value;}
        }
    }

If you're used to using ShinobiCharts you might wonder why we aren't just creating
`SChartDataPoint` objects. This is due to code sharing. The view-models aren't
aware of ShinobiCharts at all - we could decide to render the same data using
ShinobiGrids - at which point `SChartDataPoint` objects are pretty useless. Or
we might want to create an Android implementation, which will use different
data point objects.

The only other part we need to ensure we've got in the shared library is
registration of the view-model. This is done with an `MvxApplication` subclass:

    public class App : MvxApplication
    {
        public override void Initialize ()
        {
            CreatableTypes ()
                .EndingWith ("Service")
                .AsInterfaces ()
                .RegisterAsLazySingleton ();

            RegisterAppStart<ViewModels.ChartViewModel> ();
        }
    }

The first bit of code is registering concrete implementations of interfaces
for the IoC container. From our point of view, we're most interested in the next
line which registers our view model as the view model which starts the application.


### Building a ShinobiChart for binding

MvvmCross provides bindings for all the standard UI controls within each of the
platforms it supports. However, we want to display our results in a ShinobiChart,
which does not have bindings. Therefore we'll create a subclass of `ShinobiChart`
which will support binding. In our `ShinobiDemo.Touch` project we create a new
class:

    [Register("BoundShinobiChart")]
    public class BoundShinobiChart : ShinobiChart
    {
        #region Member variables
        private BoundDataSource _boundDataSource;
        #endregion

        #region Constructors
        public BoundShinobiChart (IntPtr h)
            : base(h)
        {
        }

        public BoundShinobiChart(RectangleF frame)
            : base(frame)
        {
            UpdateData ();
        }

        public BoundShinobiChart(RectangleF frame, SChartTheme theme)
            : base(frame, theme)
        {
            UpdateData ();
        }

        public BoundShinobiChart(RectangleF frame, SChartAxisType xAxisType, SChartAxisType yAxisType)
            : base(frame, xAxisType, yAxisType)
        {
            UpdateData();
        }

        #endregion

        #region Properties for binding
        private IEnumerable<IEnumerable<DataPoint>> _data;
        public IEnumerable<IEnumerable<DataPoint>> Data {
            get { return _data;}
            set {
                _data = value;
                UpdateData ();
            }
        }
        #endregion

        private void UpdateData()
        {
            _boundDataSource = new BoundDataSource (_data);
            this.DataSource = _boundDataSource;
            this.RedrawChart ();
        }
    }

This class is really quite simple. Firstly we expose all the appropriate
constructors, within each we call `UpdateData()`. We also create a `Data` property
whose type is the same as that exposed on the view-model. In the setter for this
property we call the aforementioned `UpdateData()` method. `UpdateData()` creates
a new datasource with the provided data, sets it as the chart's datasource and
then redraws the chart.

The missing part of this class is the `BoundDataSource` class. We create it as
a private inner class:

    #region Auxiliary classes
    private class BoundDataSource : SChartDataSource
    {
        private IList<IList<SChartDataPoint>> _data;
        private IList<SChartSeries> _series;

        public BoundDataSource(IntPtr h) : base(h)
        {
        }

        public BoundDataSource(IEnumerable<IEnumerable<DataPoint>> data)
        {
            if(data != null) {
                _data = new List<IList<SChartDataPoint>> ();
                foreach(var series in data) {
                    var newSeries = series
                        .Select(dp => new SChartDataPoint()
                                { XValue = new NSNumber(dp.XValue),
                                  YValue = new NSNumber(dp.YValue) })
                        .ToList();
                    _data.Add (newSeries);
                    UpdateSeries();
                }
            } else {
                _data = null;
                _series = null;
            }
        }

        private void UpdateSeries()
        {
            _series = new List<SChartSeries> ();
            foreach (var seriesData in _data) {
                _series.Add (new SChartLineSeries());
            }
        }

        #region implemented abstract members of SChartDataSource
        public override int GetNumberOfSeries (ShinobiChart chart)
        {
            if (_series != null) {
                return _series.Count;
            } else {
                return 1;
            }

        }

        public override SChartSeries GetSeries (ShinobiChart chart, int dataSeriesIndex)
        {
            if (_series != null) {
                return _series [dataSeriesIndex];
            } else {
                return new SChartLineSeries();
            }
        }
        public override int GetNumberOfDataPoints (ShinobiChart chart, int dataSeriesIndex)
        {
            if (_data != null) {
                return _data [dataSeriesIndex].Count;
            } else {
                return 0;
            }
        }
        public override SChartData GetDataPoint (ShinobiChart chart, int dataIndex, int dataSeriesIndex)
        {
            if (_data != null) {
                return _data [dataSeriesIndex] [dataIndex];
            } else {
                return null;
            }
        }
        #endregion
    }
    #endregion

Although seemingly quite long, this class is a fairly trivial implementation of
an `SChartDataSource` - the abstract class used to provide data to a ShinobiChart.
It contains 2 private member variables - one to store the collection of series,
the other the datapoints associated with these series. These are populated
during construction:

    public BoundDataSource(IEnumerable<IEnumerable<DataPoint>> data)
    {
        if(data != null) {
            _data = new List<IList<SChartDataPoint>> ();
            foreach(var series in data) {
                var newSeries = series
                    .Select(dp => new SChartDataPoint()
                            { XValue = new NSNumber(dp.XValue),
                              YValue = new NSNumber(dp.YValue) })
                    .ToList();
                _data.Add (newSeries);
                UpdateSeries();
            }
        } else {
            _data = null;
            _series = null;
        }
    }

If we've got a valid data object we loop through the series it contains, converting
the `DataPoint` objects to `SChartDataPoint` objects. This updates the `_data`
member variable - we call `UpdateSeries()` to perform a similar process on the
collection of `SChartSeries` objects:

    private void UpdateSeries()
    {
        _series = new List<SChartSeries> ();
        foreach (var seriesData in _data) {
            _series.Add (new SChartLineSeries());
        }
    }

In this simple example we only allow line series, and `double,double` data points.
This could be extended to be more generic.

The remainder of this class represents the required override abstract methods for
an `SChartDataSource` - each of which does as expected - pulling the relevant
objects from the member variables we set up at construction time.


### Wiring the BoundShinobiChart with the view model

The final part of this puzzle is wiring the view-model up with the newly created
`BoundShinobiChart`. We create a view which will contain an instance of a
`BoundShinobiChart`, and a `UISlider`:


    [Register("ChartView")]
    public class ChartView : MvxViewController
    {
        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            var chart = new BoundShinobiChart (View.Bounds, SChartAxisType.Number, SChartAxisType.Number);
            Add (chart);

            var slider = new UISlider (new RectangleF (10, 120, 300, 40));
            slider.MinValue = 0;
            slider.MaxValue = 5;
            Add (slider);
        }
    }

Here we've created the 2 controls and added them as subviews. Binding is done
differently on each of the supported platforms - most use additions to the XML
layout specification to bind a particular control to a property on the view-model
but this isn't an option for iOS. Therefore binding is done in code:

    public override void ViewDidLoad () 
    {
        ...

        var set = this.CreateBindingSet<ChartView, ShinobiDemo.Core.ViewModels.ChartViewModel> ();
        set.Bind (chart).For (s => s.Data).To (vm => vm.Source).OneWay();
        set.Bind (slider).To (vm => vm.Frequency);
        set.Apply ();
    }

We've created a binding set from the current view to the viewmodel, and then
created the individual bindings for the controls. The chart is a little more
complicated because we've got to specify which property on the chart gets bound
to which property on the view-model, and also that the binding is one-way - i.e.
it's not possible for the chart data object to update the view-model's data.
The `UISlider` binding is a little easier - it's two-way and the default binding
on the control is the one we want to use.

This is all you have to do to wire it up! Well, there's a little bit of boiler-plate
remaining, but this is standard MvvmCross stuff. We create a class which sets
up this iOS project as a mvx view project:

    public class Setup : MvxTouchSetup
    {
        public Setup(MvxApplicationDelegate applicationDelegate, UIWindow window)
            : base(applicationDelegate, window)
        {
        }

        protected override Cirrious.MvvmCross.ViewModels.IMvxApplication CreateApp ()
        {
            return new Core.App ();
        }
    }

This returns the view models which we've specified in our `Core` portable class
library for wiring up purposes. We then just update the `AppDelegate` to reference
this setup class:

    [Register ("AppDelegate")]
    public partial class AppDelegate : MvxApplicationDelegate
    {
        UIWindow window;

        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
        {
            // create a new window instance based on the screen size
            window = new UIWindow (UIScreen.MainScreen.Bounds);
            
            var setup = new Setup (this, window);
            setup.Initialize ();

            var startup = Mvx.Resolve<IMvxAppStart> ();
            startup.Start ();


            // make the window visible
            window.MakeKeyAndVisible ();
            
            return true;
        }
    }

We inherit from `MvxApplicationDelegate`, create an instance of our `Setup` class,
before using the IoC container to find an app start object and invoking it.


## Conclusion

This post has demonstrated how to use MvvmCross with ShinobiCharts - allowing
the display logic to be pulled out of the view itself. This allows cross-platform
code-sharing - e.g. using ShinobiCharts for Android we would be able to create a
new front-end for the same app without changing/replicating any of the point
generation logic. We would just need to write an equivalent subclass of the
ShinobiChart suitable for databinding, and then create the views using the XML
schema.

There are lots of ways in which this simple demo could be improved:
- We currently only look at binding chart data - it would be trivial to extend
this to chart title, axis labels etc.
- This project only supports `double, double` datapoints, and line series. A more
generally useful version would allow all the options supported by ShinobiCharts.
- The `BoundShinobiChart` could be wrapped up in an MvvmCross plugin to ease use
across different apps.

Feel free to fork this project on Github and investigate these potential
improvments. Or suggest other ways it could be improved - hit me up on github
or twitter [@iwantmyrealname](https://twitter.com/iwantmyrealname).







