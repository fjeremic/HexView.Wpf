# HexView.Wpf
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://github.com/fjeremic/HexView.Wpf/blob/master/LICENSE)
[![Build Status](https://fjeremic.visualstudio.com/HexView.Wpf/_apis/build/status/fjeremic.HexView.Wpf?branchName=master)](https://fjeremic.visualstudio.com/HexView.Wpf/_build/latest?definitionId=2&branchName=master)

A WPF control for displaying binary data in a traditional hex view.

<br/>
<p align="center">
  <img align="center" alt="HexViewer demo project" src="https://raw.githubusercontent.com/fjeremic/HexView.Wpf/assets/HexViewer.png"/>
</p>
<br/>

## Installing / Getting started

The best way to obtain the library is via the [HexView.Wpf NuGet package](https://www.nuget.org/packages/HexView.Wpf/):
 
```shell
dotnet add package HexView.Wpf
```

Alternatively you may clone this git repository and build the library for use within your projects:

```shell
dotnet build
```

## Features

- Supports viewing different data types, formats, signedness
- Supports showing/hiding sections of the control
- Supports selection and copying
- Supports mouse scrolling and keyboard navigation
- Supports fixed and arbitrary column widths via binding
- Able to adaptively display columns upon resizing the control
- Built in context menu for controlling display of data
- Supports template styling of the control
- Adheres to Windows default styling and colors

## Screenshots

These screenshots were generated using the demo application found in the [HexView.Wpf.Demo](/HexView.Wpf.Demo) project of this repository which uses the control to implement a simple file hex viewer application.

<br/>
<p align="center">
  <img align="center" src="https://raw.githubusercontent.com/fjeremic/HexView.Wpf/assets/HexViewResize.gif"/>
</p>
<br/>
<p align="center">
  <img align="center" src="https://raw.githubusercontent.com/fjeremic/HexView.Wpf/assets/HexViewerContextMenu.png"/>
</p>
<br/>
<p align="center">
  <img align="center" src="https://raw.githubusercontent.com/fjeremic/HexView.Wpf/assets/HexViewerLongSelected.png"/> 
</p>
<br/>
<p align="center">
  <img align="center" src="https://raw.githubusercontent.com/fjeremic/HexView.Wpf/assets/HexViewerSignedShort.png"/>
</p>
<br/>
<p align="center">
  <img align="center" src="https://raw.githubusercontent.com/fjeremic/HexView.Wpf/assets/HexViewerTextOnly.png"/> 
</p>
