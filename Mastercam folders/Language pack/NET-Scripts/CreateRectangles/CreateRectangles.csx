// -----------------------------------------------------------------------------------------
// <copyright file="CreateRectangles.csx" company="CNC Software, LLC">
//   Copyright (c) 2024 CNC Software, LLC
// </copyright>
// <Author>
//  sdk@Mastercam.com
// </Author>
// <summary>
//   Creates an array of rectangles.
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

// using Mastercam.GeometryUtility;
// using Mastercam.IO;
// using Mastercam.Math;

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
		CreateRectangles();
	}

#endregion

#region Private Methods

	/// <summary>
	/// Creates an array of rectangles
	/// </summary>
	private void CreateRectangles()
	{
		// width of rect
		var width = 10.00;

		// length of rect
		var height = 10.00;

		// spacing
		var horizontalSpacing = 2.00;

		// spacing
		var verticalSpacing = 2.00;

		// number of rectangles to create
		var count = 20;

		// clear screen
		FileManager.New();

		// set flag
		var error = false;

		// create an array of rectangles
		for (int x = 0; x < count; x++)
		{
			for (int y = 0; y < count; y++)
			{
				var point1 = new Point3D(x * (width + horizontalSpacing), y * (height + verticalSpacing), 0);
				var point2 = new Point3D(point1.x + width, point1.y + height, 0);

				var lines = GeometryCreationManager.CreateRectangle(point1, point2);

				// using linq
				if (!lines.Any())
				{
					error = true;

					// exit inner loop
					break;
				}
			}

			if (error)
			{
				// exit outer loop
				break;
			}
		}

		if (error)
		{
			DialogManager.Error("Failed to create rectangle.", "Create Rectangles");
		}
		else
		{
			GraphicsManager.FitScreen();
		}
	}

#endregion
}

#endregion