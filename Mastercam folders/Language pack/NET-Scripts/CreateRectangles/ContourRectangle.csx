// -----------------------------------------------------------------------------------------
// <copyright file="ContourRectangle.csx" company="CNC Software, LLC">
//   Copyright (c) 2024 CNC Software, LLC
// </copyright>
// <Author>
//  sdk@Mastercam.com
// </Author>
// <summary>
//   Demonstrates creating a contour operation
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
// using System.Linq;

// using Mastercam.App.Exceptions;
// using Mastercam.Database;
// using Mastercam.Database.Types;
// using Mastercam.GeometryUtility;
// using Mastercam.IO;
// using Mastercam.IO.Types;
// using Mastercam.Math;
// using Mastercam.Operations;
// using Mastercam.Operations.Types;
// using Mastercam.Support;
// using Mastercam.Tools; 

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
	///<summary>
	/// Main entry point to script
	/// </summary>
	public void Run()
	{
		RunScript();
	}

#region private methods

	///<summary>
	/// Main entry point into the script
	/// </summary>
	private void RunScript()
	{
		var demo = new ContourRectangle();
		demo.Run();
	}

#endregion
}

/// <summary>
/// Defines the ContourRectangle class.
/// </summary>
public class ContourRectangle
{
	public void Run()
	{
		try
		{
			// start a new session
			var success = FileManager.New(true);
			if (!success)
			{
				return;
			}

			// define the size of the rectangle
			var point1 = new Point3D(0, 0, 0);
			var point2 = new Point3D(10, 10, 0);

			// create the rectangle
			var lines = GeometryCreationManager.CreateRectangle(point1, point2);
			if (lines.Any())
			{
				// cast to geometry type to pass to chain manager
				var geometries = lines.OfType<Geometry>().ToArray();

				// chain the rectangle
				var chains = ChainManager.ChainGeometry(geometries);

				// create a contour operation 
				var contour = new ContourOperation
				{
					Linking =
					{
						TopStock = 0, Depth = -.250
					},
						CutParams =
					{
						ContourSubtype = ContourSubtypeType.Basic
					},
						CutterComp =
					{
						Direction = CutterCompDir.CutterCompLeft,
							RollCorners = CutterCompRoll.CutterCompRollAll
					},
						LeadInOut =
					{
						Enabled = true,
							OverLap = 1,
							Entry =
						{
							LineLength = 1,
								RampHeight = 1,
								ArcRadius = 0.0,
								ArcSweep = 1,
								HelixHeight = 1,
								UsePoint = true
						},
							Exit =
						{
							LineLength = 1,
								RampHeight = 1,
								ArcRadius = 0.0,
								ArcSweep = 1,
								HelixHeight = 1,
								UsePoint = true
						}
					},
						OperationTool = new EndMillFlatTool
					{
						Diameter = 0.75,
							Number = 1,
							DiameterOffset = 1,
							Coolant = CoolantMode.COOL_FLOOD,
							Flutes = 2,
							HolderDia = 2.0,
							HolderLength = 3,
							IsMetric = false,
							Length = 2,
							LengthOffset = 1,
							FileName = "Created by NET-Script"
					}
				};

				success = contour.OperationTool.Commit();
				if (!success)
				{
					throw new MastercamException("failed to create tool");
				}

				// apply the chains
				contour.SetChainArray(chains);

				success = contour.Commit();
				if (!success)
				{
					throw new MastercamException("failed to create operation");
				}

				success = contour.Regenerate();
				if (!success)
				{
					throw new MastercamException("failed to regenerate operation");
				}
			}
		}
		catch (MastercamException e)
		{
			DialogManager.Error("Mastercam Exception", e.Message);
		}
		catch (Exception e)
		{
			DialogManager.Error("System Exception", e.Message);
		}
		finally
		{
			// Set view to the ISO
			ViewManager.GraphicsView = SearchManager.GetViews((int) GraphicsViewType.Iso)[0];

			GraphicsManager.FitScreen();
		}
	}
}

#endregion
