# HexView.Wpf
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://github.com/fjeremic/HexView.Wpf/blob/master/LICENSE)
[![Build Status](https://fjeremic.visualstudio.com/HexView.Wpf/_apis/build/status/fjeremic.HexView.Wpf?branchName=master)](https://fjeremic.visualstudio.com/HexView.Wpf/_build/latest?definitionId=2&branchName=master)

A WPF control for displaying binary data in a traditional hex view.

<p align="center">
 <img align="center" alt="HexViewer demo project" src="https://user-images.githubusercontent.com/16259104/81234100-195c5700-8fc6-11ea-8f30-3d486b4db6a0.png"/>
</p>

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

<p align="center">
  <img align="center" src="https://user-images.githubusercontent.com/16259104/81235055-0f3b5800-8fc8-11ea-93f8-74535bd0ead2.gif"/>
</p>
<p align="center">
  <img align="center" src="https://user-images.githubusercontent.com/16259104/81234300-807a0b80-8fc6-11ea-9b36-02aa74be4c83.png"/>
</p>
<p align="center">
  <img align="center" src="https://user-images.githubusercontent.com/16259104/81234315-88d24680-8fc6-11ea-9c7d-a961ca995cf3.png"/> 
</p>
<p align="center">
  <img align="center" src="https://user-images.githubusercontent.com/16259104/81234349-92f44500-8fc6-11ea-9dc7-bf24c1fc4a4b.png"/>
</p>
<p align="center">
  <img align="center" src="https://user-images.githubusercontent.com/16259104/81234367-9a1b5300-8fc6-11ea-8e75-0eade884bc9b.png"/> 
</p>