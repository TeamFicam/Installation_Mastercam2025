// -----------------------------------------------------------------------------------------
// <copyright file="CreateBoltCircle.csx" company="CNC Software, LLC">
//   Copyright (c) 2024 CNC Software, LLC
// </copyright>
// <Author>
//  sdk@Mastercam.com
// </Author>
// <summary>
//   Creates a bolt circle pattern.
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

// using Mastercam.IO;
// using Mastercam.Curves;

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

#region Private Fields

#endregion

#region Constructor

#endregion

#region Public Methods

	///<summary>
	/// Main entry point to script
	/// </summary>
	public void Run()
	{
		// clear screen
		FileManager.New();

		// call the function
		CreateBoltCircle();
	}

#endregion

#region Private Methods

	/// <summary>
	/// Creates a bolt circle
	/// </summary>
	private void CreateBoltCircle()
	{
		// set up some values
		var patternRadius = 10.00;
		var holeDiameter = 1.00;
		var numberOfHoles = 6;
		var angle = 0.0;

		// incremental angle in radians 
		var angleIncrement = Math.PI * 2 / numberOfHoles;

		// clear screen
		FileManager.New();

		for (int i = 0; i < numberOfHoles; i++)
		{
			// define a starting circle
			var circle = new ArcGeometry();
			circle.Data.Radius = holeDiameter / 2;
			circle.Data.StartAngleDegrees = 0.0;
			circle.Data.EndAngleDegrees = 360.00;

			circle.Data.CenterPoint.x = patternRadius * Math.Cos(angle);
			circle.Data.CenterPoint.y = patternRadius * Math.Sin(angle);
			circle.Data.CenterPoint.z = 0;

			angle += angleIncrement;

			if (!circle.Commit())
			{
				DialogManager.Error("Failed to create circle geometry", "CreateBoltCircle");
				return;
			}
		}

		GraphicsManager.FitScreen();
	}

#endregion

}

#endregion