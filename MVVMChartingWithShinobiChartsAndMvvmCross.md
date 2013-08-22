# MVVM Charting with ShinobiCharts and MvvmCross

## Introduction

Model-View-ViewModel is a software design pattern made popular by Microsoft with
the introduction of WPF. It excels at separation of concerns in scenarios which
contain a persistent view, such as in desktop and mobile applications.

In this post we'll give a quick summary of the MVVM design pattern and its
application to mobile development, including looking at MvvmCross. We'll also
create a ShinobiChart suitable for use with MVVM data-binding.

![diagram](img/MVVMPattern.png?raw=true)

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

![image](img/screenshot.png?raw=true)

In our `ShinobiDemo.Core` portable library project we'll create `ChartViewModel`,
which can inherit from `MvxViewModel`. This allows property change notifications
to be processed correctly by the framework.

Our view-model has a collection of C# objects, which are from our data domain.
These objects have properties on them which we will use to create the datapoints
later on, but the important point here is that we know nothing about the
technology which is going to display our view model - that's not the concern
of the view-model layer. In our demo we'll use this `ExampleDataClass`:

    public class ExampleDataClass
    {
        public ExampleDataClass (double time, double lower, double upper)
        {
            this.Time = time;
            this.Lower = lower;
            this.Upper = upper;
        }

        public double Time { get; private set; }
        public double Lower { get; private set; }
        public double Upper { get; private set; }
    }

We need 2 properties on the view-model itself - one to provide the frequency,
one the resultant data objects (which will be plotted):

    private IList<ExampleDataClass> _source;
    public IList<ExampleDataClass> Source {
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
        // create the upper and lower component
        var dps = new List<ExampleDataClass>();
        for (double phase = 0; phase < Math.PI; phase+= (Math.PI / 100))
        {
            dps.Add (new ExampleDataClass (phase,
                                           Math.Sin (phase * this.Frequency + Math.PI) + this.Frequency * 2.5,
                                           Math.Sin (phase * this.Frequency) + this.Frequency * 2.5));
        }

        // And then save this off - making sure to use the setter
        this.Source = dps;
    }

This method generates a set of data points which represent a wave at the
specified frequency.

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

### Building a bindable data source helper

MvvmCross provides bindings for all the standard UI controls within each of the
platforms it supports. However, we want to display our results in a ShinobiChart,
which does not have bindings. We'll therefore create a datasource helper class
which we can bind to, and which will provide the data to the chart in an
appropriate fashion.

We've created a new iOS library project to contain this reusable class - 
`ShinobiCharts.Touch.MvvmCrossBindings`, within which there is a class called
`BindableDataSourceHelper`. This class has 2 properties:

    private IList<T> _data;
    public IList<T> Data {
        get { return _data; }
        set {
            _data = value;
            DataUpdated ();
        }
    }

    private Func<SChartSeries> _chartSeriesCreator;
    public Func<SChartSeries> ChartSeries {
        get{ return _chartSeriesCreator; }
        set {
            _chartSeriesCreator = value;
            SeriesUpdated ();
        }
    }

The `Data` property is a collection of generic C# objects - these are the domain
objects from our view-model, and we'll bind it as such later on. The `ChartSeries`
property is a lambda which returns an `SChartSeries` object. This will get called
whenever our helper requires a new series object.

Both of these properties call respective update methods when they have been set:

    private void DataUpdated ()
    {
        // Replace the datasource with a new one
        _dataSource = new ChartDataSource<T> (_data, ChartSeries(),
                                           o => {
            // Get the value for the specified property key and convert it to an NSObject
            return o.GetPropertyValue (_xValueKey).ConvertToNSObject ();
        },
                                           o => {
            return o.GetPropertyValue (_yValueKey).ConvertToNSObject ();
        });
        _chart.DataSource = _dataSource;
        // Need to redraw to see the changes
        _chart.RedrawChart ();
    }

    private void SeriesUpdated ()
    {
        // If we don't have a datasource then we don't need to do anything
        if (_dataSource != null) {
            _dataSource.Series = ChartSeries();
            _chart.RedrawChart ();
        }
    }

The important features of these methods are:
- When the data is updated we create a new `ChartDataSource` object. This is a
private inner class, which is used to interface with the chart. It accepts the
data list, a chart series and a couple of lambdas which explain how to extract
appropriate values from the domain objects.
- After we've set the `DataSource` property on the chart we need to redraw the
chart.
- When the series is updated we don't need a new datasource - we can provide the
new series and redraw the chart.

We find the x and y values from the domain object by looking up the value of
a property whose name is provided at construction time:

    public BindableDataSourceHelper (ShinobiChart chart,
                                     Func<SChartSeries> seriesCreator,
                                     string xValueKey, string yValueKey)
    {
        ...
    }

There are a couple of other classes in this project which provide extension methods
on `System.Object`:
- `PropertyExtensions` provides `GetPropertyValue()` which uses reflection to
obtain the value of a property with a given (string) name.
- `NSObjectConversionExtensions` provides `ConvertToNSObject()` which is a method
which turns an unknown .net object to the best approximation `NSObject` subclass.


### Using the BindableDataSource

In a new `ShinobiDemo.Touch` single-view iOS application project we'll create the
view to bind to the view-model. We'll create 2 UI components in a standard manner:


    [Register("ChartView")]
    public class ChartView : MvxViewController
    {
        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            // Create a chart
            var chart = new ShinobiChart (new RectangleF (0, 40, View.Bounds.Width, View.Bounds.Height-40),
                                          SChartAxisType.Number, SChartAxisType.Number);
            chart.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            // Create a datasource helper
            _dsHelper = new BindableDataSourceHelper<ExampleDataClass> (chart,
                            () => { return new SChartLineSeries (); },
                            "Time", "Lower"); 
            Add (chart);

            // Create a UISlider
            var slider = new UISlider (new RectangleF (0, 0, View.Bounds.Width, 40));
            slider.MinValue = 0;
            slider.MaxValue = 5;
            slider.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            Add (slider);
        }
    }

Here we've created the 2 controls and added them as subviews. We created the
datasource helper specifying:
- The `ShinobiChart` we wish it to manage the data for
- A lambda describing how to create a new `SChartSeries` object
- The names of the properties on our domain objects we wish to use as the x and
y values for the data points.


## Binding to the view-model

Binding is done differently on each of the supported platforms - most use additions
to the XML layout specification to bind a particular control to a property on the
view-model but this isn't an option for iOS. Therefore binding is done in code:

    public override void ViewDidLoad () 
    {
        ...

        // Create the binding 
        var set = this.CreateBindingSet<ChartView, ChartViewModel> ();
        // Bind the datasource helper to the view model
        set.Bind (_dsHelper).For (s => s.Data).To (vm => vm.Source).OneWay();
        // And the UISlider
        set.Bind (slider).To (vm => vm.Frequency);
        set.Apply ();
    }

We've created a binding set from the current view to the viewmodel, and then
created the individual bindings for the controls. The datasource helper is a little more
complicated because we've got to specify which property on the helper gets bound
to which property on the view-model, and also that the binding is one-way - i.e.
it's not possible for the helper object to update the view-model's data.
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

The `BindableDataSourceHelper` class can be used without modification for different
series types, and with no modification for different domain objects. You can
pull the code into your solution and give it a go.

Feel free to fork this project on Github and investigate these potential
improvments. Or suggest other ways it could be improved - hit me up on github
or twitter [@iwantmyrealname](https://twitter.com/iwantmyrealname).


---
The MvvmPattern image used is Creative Commons licensed by Ugaya40 (Own work)
CC-BY-SA-3.0, via Wikimedia Commons
