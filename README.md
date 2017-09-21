MVVM Charting with ShinobiCharts and MvvmCross
=====================

Model-View-ViewModel is a software design pattern which suits multi-platform UI
projects really well. In this project we create an iOS app using MvvmCross, a
popular open source implementation of the MVVM pattern in Xamarin.iOS.

![Screenshot](screenshot.png?raw=true)


Directory structure
------------------

The code is all inside the `ShinobiDemo` directory - which contains a sln
referencing 4 projects:

- `ShinobiCharts.MvvmCrossBinding`. This contains a `BindableDataSourceHelper` class
which you can drop into your own projects to support binding a ShinobiChart to a
MvvmCross view-model. Requires a reference to the ShinobiCharts component. Also
has a couple of utility classes required by the helper.
- `ShinobiDemo.Touch.Tests`. Contains some unit tests for the Shinobi bindings
project (above).
- `ShinobiDemo.Core`. The view-model and mvx application for the demo project
which shows how to use the bindable data source helper with a ShinobiChart.
- `ShinobiDemo.Touch`. The iOS front end to the demo application. This uses the
view-model from the core project and the bindable data source helper to create a
simple single-view iPhone application. Check here to see and example which uses
the data source helper.


Building the project
------------------

This project uses ShinobiCharts and MvvmCross to build a Xamarin.iOS app. You'll
need [Xamarin Studio](http://xamarin.com/), [MvvmCross](https://github.com/slodge/mvvmcross)
and a copy of ShinobiCharts. If you don't have it yet, you can download a free
trial from the [ShinobiCharts website](http://www.shinobicontrols.com/shinobicharts/).


Contributing
------------

We'd love to see your contributions to this project - please go ahead and fork it and send us a pull request when you're done! Or if you have a new project you think we should include here, email info@shinobicontrols.com to tell us about it.

License
-------

The [Apache License, Version 2.0](license.txt) applies to everything in this repository, and will apply to any user contributions.
