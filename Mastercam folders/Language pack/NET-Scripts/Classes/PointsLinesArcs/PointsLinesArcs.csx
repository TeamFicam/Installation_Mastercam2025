// -----------------------------------------------------------------------------------------
// <copyright file="PointLineArc.csx" company="CNC Software, LLC">
//   Copyright (c) 2024 CNC Software, LLC
// </copyright>
// <Author>
//  sdk@mastercam.com
// </Author>
// <summary>
//   Shows examples of creating .NET point, line, arc, rectangle
//   and setting some misc properties.
// </summary>
// -----------------------------------------------------------------------------------------

// Assemblies Import for External Editor
#region Assemblies Import

// NOTE: Update paths to reference your Mastercam 2025 path
// #r "C:\Program Files\Mastercam 2025\NETHook3_0.dll"

#endregion

// Using for External Editor
#region Namespace Import
/*
using Mastercam.IO;
using Mastercam.IO.Types;
using Mastercam.BasicGeometry;
using Mastercam.Math;
using Mastercam.Curves;
*/
#endregion

// instantiate
var app = new App();

// Execute the script
app.Run();

#region Classes

/// <summary>
/// Defines our top level App class
/// </summary>
public class App
{
#region Public Methods

	///<summary>
	/// Main entry point to script
	/// </summary>
	public void Run()
	{
		RunScript();
	}

#endregion

#region Private Methods

	/// run the examples
	private void RunScript()
	{
		// clear the screen for a new session
		FileManager.New();

		// create a point in the center
		CreatePoint();

		// create a line
		CreateLine();

		// create an arc
		CreateArc();

		// zoom all
		GraphicsManager.FitScreen();
	}

	/// <summary>
	/// Create a point
	/// </summary>
	private void CreatePoint()
	{
		/*
	Dim point 
	point = New McPt()
	With point
		.X = 0
		.Y = 0
		.Z = 0
		.PType = 0
	End With
	*/

		// New way to define a point
		var point = new PointGeometry();
		point.Data.x = 0;
		point.Data.y = 0;
		point.Data.z = 0;
		point.PointStyle = PointStyleType.Dot;

		// set some properties
		point.Level = 9;
		point.Color = 15;

		// create by commiting to the database
		var success = point.Commit();
		if (!success)
		{
			DialogManager.Error("CreatePoint failed.", "Mastercam");
		}
		else
		{
			// update the level for this geometry
			var ok = SetLevel(9, "Point Levels", "Geometry");
			if (!ok)
			{
				DialogManager.Error("Failed to set level data for point.", "Mastercam");
			}
		}
	}

	/// <summary>
	/// Create a line
	/// </summary>
	private void CreateLine()
	{
		/*
	Dim line 
	line = New McLn()
	With line
		.X1 = 0
		.Y1 = 0
		.Z1 = 0
		.X2 = 0
		.Y2 = 0
		.Z2 = 0
	End With
	*/

		// define line end point co-ordinates
		var point1 = new Point3D(0, 0, 0);
		var point2 = new Point3D(10, 0, 0);

		// define a new line
		var line = new LineGeometry(point1, point2);

		// set some properties
		line.Level = 10;
		line.Color = 5;

		// create by commiting to the database
		var success = line.Commit();
		if (!success)
		{
			DialogManager.Error("CreateLine failed.", "Mastercam");
		}
		else
		{
			// update the level for this geometry
			var ok = SetLevel(10, "Line Levels", "Geometry");
			if (!ok)
			{
				DialogManager.Error("Failed to set level data for line.", "Mastercam");
			}
		}
	}

	/// <summary>
	/// Creates an Arc
	/// </summary>
	private void CreateArc()
	{
		/*
	Dim arc 
	arc = New McAr()
	With arc
		.X = 0
		.Y = 0
		.Z = 0
		.R = 0
		.SA = 0
		.SW = 0
		.View = 0
	End With
	*/


		// define a starting circle
		var circle = new ArcGeometry();
		circle.Data.Radius = .25;
		circle.Data.StartAngleDegrees = 0.0;
		circle.Data.EndAngleDegrees = 360.00;
		circle.Data.CenterPoint.x = 0;
		circle.Data.CenterPoint.y = 0;
		circle.Data.CenterPoint.z = 0;

		// set some properties
		circle.Level = 12;
		circle.Color = 10;

		// commit to the database
		var success = circle.Commit();
		if (!success)
		{
			DialogManager.Error("CreateArc failed.", "Mastercam");
		}
		else
		{
			// update the level for this geometry
			var ok = SetLevel(12, "Arc Levels", "Geometry");
			if (!ok)
			{
				DialogManager.Error("Failed to set level data for arc.", "Mastercam");
			}
		}


	}

	/// <summary>
	/// Sets the level name and set for the level number
	/// </summary>
	/// <param name="number">The level number</param>
	/// <param name="name">The level name</param>
	/// <param name="set">The level set name</param>
	/// <returns>True on success, false otherwise</returns>
	private bool SetLevel(int number, string name, string set)
	{
		// set the level name and number
		var success = LevelsManager.SetLevelName(number, name);
		if (!success)
		{
			return false;
		}

		// set the level set name
		success = LevelsManager.SetLevelSetName(number, set);
		if (!success)
		{
			return false;
		}

		// if we get here we are good to go
		return true;
	}

#endregion

}

#endregion
