// -----------------------------------------------------------------------------------------
// <copyright file="create_points.csx" company="CNC Software, LLC">
//   Copyright (c) 2024 CNC Software, LLC
// </copyright>
// <Author>
//  sdk@Mastercam.com
// </Author>
// <summary>
//   Creates 2 points.
// </summary>
// -----------------------------------------------------------------------------------------

// Assemblies Import for External Editor
#region Assemblies Import

// NOTE: Update paths to reference your Mastercam 2025 path
// #r "C:\Program Files\Mastercam 2025\NETHook3_0.dll"

#endregion

// Using for External Editor
#region Namespace Import

// using System.Linq;

// using Mastercam.BasicGeometry;
// using Mastercam.Curves;
// using Mastercam.IO;
// using Mastercam.Support;

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

#region private methods

	private void RunScript()
	{
		// new session
		FileManager.New(true);

		// define two points
		PointGeometry pt1 = new PointGeometry();
		PointGeometry pt2 = new PointGeometry(1, 1, 0);

		// create the points
		var success = pt1.Commit();
		if (!success)
		{
			DialogManager.Error("Error", "Failed to create Point 1");
			return;
		}

		success = pt2.Commit();
		if (!success)
		{
			DialogManager.Error("Error", "Failed to create Point 2");
			return;
		}

		// create a line between the two points
		var line1 = new LineGeometry(pt1.Data, pt2.Data);

		success = line1.Commit();
		if (!success)
		{
			DialogManager.Error("Error", "Failed to create line");
			return;
		}

		GraphicsManager.Repaint(true);

		var geometry = SearchManager.GetGeometry();
		if (geometry.Any())
		{
			DialogManager.OK($"Count = {geometry.GetLength(0)}", "GetGeometry");
		}
	}
#endregion

}

#endregion