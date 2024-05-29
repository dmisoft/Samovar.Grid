﻿using System.Reactive.Subjects;

namespace Samovar.Blazor;

public interface IDeclarativeColumnModel
    : IColumnModel
{
	public DeclarativeColumnWidthMode DeclaratedWidthMode { get; }
	public double DeclaratedWidth { get; }
	public BehaviorSubject<string> DeclaratedWidthParameter { get; set; }
	public BehaviorSubject<string> Title { get; }
}
