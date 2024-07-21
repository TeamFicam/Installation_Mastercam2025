// -----------------------------------------------------------------------------------------
// <copyright file="LevelManager.csx" company="CNC Software, LLC">
//   Copyright (c) 2024 CNC Software LLC
// </copyright>
// <Author>
//  sdk@mastercam.com
// </Author>
// <summary>
//   Showcases various uses of the LevelManager class.
// </summary>
// -----------------------------------------------------------------------------------------

// Assemblies Import for External Editor
#region Assemblies Import

// NOTE: Update paths to reference your Mastercam 2025 path
// #r "C:\Program Files\Mastercam 2025\NETHook3_0.dll"

#endregion

// Using for External Editor
#region Namespace Import

// using System;
// using System.Collections.Generic;
// using System.Linq;

// using Mastercam.Curves;
// using Mastercam.Database.Types;
// using Mastercam.GeometryUtility;
// using Mastercam.IO;
// using Mastercam.IO.Types;
// using Mastercam.Math;
// using Mastercam.Support;

#endregion

#region Consts

// title for dialogs
const string title = "NET-Script";

// level for all geometry created
const int level = 100;

// level name for our block geometry
const string levelName = "BLOCK";

// level set name for our block geometry
const string levelSetName = "GEOMETRY";

#endregion

// run our script
var app = new App();
app.Run();

#region Classes

/// <summary>
/// Defines our top level App class
/// </summary>
public class App
{
	///<summary>
	/// Main entry point to script
	/// </summary>
	public void Run()
	{
		// file new, discard anything on screen
		var _ = FileManager.New();

		// Draw a block 1" deep on level 100
		var result = DrawBlock(new Point3D
			{
				x = 0.0, y = 0.0, z = 0.0
			}, new Point3D
			{
				x = 12.0, y = 12.0, z = 0
			}, -1, level);

		if (result.IsFailure)
		{
			DialogManager.Error(result.Error, title);
		}

		// Set the level name
		var success = LevelsManager.SetLevelName(level, levelName);
		if (!success)
		{
			DialogManager.Error($"Failed to set level name for level {level}", title);
		}

		// Set the level set name for our level
		success = LevelsManager.SetLevelSetName(level, levelSetName);
		if (!success)
		{
			DialogManager.Error($"Failed to set level set name for level {level}", title);
		}
	}

#region Private Methods

	/// <summary> Draws a block. </summary>
	///
	/// <param name="topPoint1"> The first top point position. </param>
	/// <param name="topPoint2"> The second top point position. </param>
	/// <param name="depth"> The depth in Z. </param>
	/// <param name="level">  The level number. </param>
	private Result DrawBlock(Point3D topPoint1, Point3D topPoint2, double depth, int level)
	{
		try
		{
			// define the top rectangle
			var topRectangle = GeometryCreationManager.CreateRectangle(topPoint1, topPoint2).ToList();

			// set level and commit to database
			var result = SetLevelAndCommit(level, topRectangle);
			if (result.IsFailure)
			{
				return Result.Fail(result.Error);
			}

			// select all the geometry we just created filtering on lines
			SelectionManager.SelectGeometryByMask(QuickMaskType.Lines);

			// translate a copy in Z in the same view as the original
			if (!GeometryManipulationManager.TranslateGeometry(
				new Point3D(0, 0, topPoint1.z),
				new Point3D(0, 0, depth),
				ViewManager.GraphicsView,
				ViewManager.GraphicsView,
				true))
			{
				return Result.Fail("Failed to translate geometry");
			}

			// define the connection vertices
			var vertices = new List<LineGeometry>()
			{
				new LineGeometry(new Point3D(topPoint1.x, topPoint1.y, topPoint1.z), new Point3D(topPoint1.x, topPoint1.y, depth)),
							new LineGeometry(new Point3D(topPoint1.x, topPoint2.y, topPoint1.z), new Point3D(topPoint1.x, topPoint2.y, depth)),
							new LineGeometry(new Point3D(topPoint2.x, topPoint2.y, topPoint1.z), new Point3D(topPoint2.x, topPoint2.y, depth)),
							new LineGeometry(new Point3D(topPoint2.x, topPoint1.y, topPoint1.z), new Point3D(topPoint2.x, topPoint1.y, depth))
			};

			// set level and commit to database
			result = SetLevelAndCommit(level, vertices);
			if (result.IsFailure)
			{
				return Result.Fail(result.Error);
			}
		}
		catch (Exception e)
		{
			// generic error message
			var message = $"An error occured executing script: {e.Message}";
			return Result.Fail(message);
		}
		// clean up screen (this code will always excute)
		finally
		{
			// clear the result
			GraphicsManager.ClearColors(new GroupSelectionMask(true));

			// repaint screen
			GraphicsManager.Repaint();

			// get the system ISO view number
			const int ViewNumber = (int) GraphicsViewType.Iso;

			// set the view
			ViewManager.GraphicsView = SearchManager.GetViews(ViewNumber)[0];

			// Zoom out
			GraphicsManager.FitScreen();
		}

		return Result.Ok();
	}

	/// <summary>
	/// Set the level number and commit the geoemtry to the Mastercam database
	/// </summary>
	/// <param name="level">The level number for the geometry</param>
	/// <returns>True if success, false otherwise</returns>
	private Result SetLevelAndCommit(int level, List<LineGeometry> lines)
	{
		// iterate over our list of line geometry we defined
		foreach (var line in lines)
		{
			// set the level number for the geometry
			line.Level = level;

			// commit the geometry to the database
			if (!line.Commit())
			{
				return Result.Fail("Failed to commit geometry to database.");
			}
		}

		return Result.Ok();
	}

#endregion
}

/// <summary> 
/// Defines the Result class that can be used to wrap a functions return value
/// allowing the developer to add a nicely formatted error message.
/// </summary>
public class Result
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Result"/> class.  Specialised constructor for use only by derived class. 
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Thrown when the requested operation is invalid. 
	/// </exception>
	/// <param name="isSuccess">
	/// True if this object is success, false if not. 
	/// </param>
	/// <param name="error">
	/// The error message. 
	/// </param>
	protected Result(bool isSuccess, string error)
	{
		if (isSuccess && error != string.Empty)
		{
			throw new InvalidOperationException();
		}

		if (!isSuccess && error == string.Empty)
		{
			throw new InvalidOperationException();
		}

		IsSuccess = isSuccess;
		Error = error;
	}

	/// <summary> Gets a value indicating whether this object is success. </summary>
	/// <value> True if this object is success, false if not. </value>
	public bool IsSuccess
	{
		get;
	}

	/// <summary> Gets the error. </summary>
	/// <value> The error. </value>
	public string Error
	{
		get;
	}

	/// <summary> Gets a value indicating whether this object is failure. </summary>
	/// <value> True if this object is failure, false if not. </value>
	public bool IsFailure => !IsSuccess;

	/// <summary> Handles the Fail response. </summary>
	/// <param name="message"> The message. </param>
	/// <returns> A Result. </returns>
	public static Result Fail(string message) => new Result(false, message);

	/// <summary> Handles the Fail response. </summary>
	/// <typeparam name="T"> Generic type parameter. </typeparam>
	/// <param name="message"> The message. </param>
	/// <returns> A Result of type T. </returns>
	public static Result<T> Fail<T>(string message) => new Result<T>(default(T), false, message);

	/// <summary> Gets the ok. </summary>
	/// <returns> A Result. </returns>
	public static Result Ok() => new Result(true, string.Empty);

	/// <summary> Gets the ok. </summary>
	/// <typeparam name="T"> Generic type parameter. </typeparam>
	/// <param name="value"> The value. </param>
	/// <returns> A Result. </returns>
	public static Result<T> Ok<T>(T value) => new Result<T>(value, true, string.Empty);
}

/// <summary> Defines the Result class. </summary>
/// <typeparam name="T"> Generic type parameter. </typeparam>
public class Result<T> : Result
{
	/// <summary> The value. </summary>
	private readonly T value;

	/// <summary> Gets the value. </summary>
	/// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
	/// <value> The value. </value>
	public T Value
	{
		get
		{
			if (!IsSuccess)
			{
				throw new InvalidOperationException();
			}

			return value;
		}
	}

	/// <summary>
	/// Describes the <see cref="Result{T}"/> class.  Constructor. 
	/// </summary>
	/// <param name="value">
	/// The value of type T. 
	/// </param>
	/// <param name="isSuccess">
	/// True if this object is success. 
	/// </param>
	/// <param name="error">
	/// The error message. 
	/// </param>
	protected internal Result(T v, bool isSuccess, string error)
			: base (isSuccess, error) => value = v;
}

#endregion
