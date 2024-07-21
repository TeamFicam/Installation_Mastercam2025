// -----------------------------------------------------------------------------------------
// <copyright file="CreatePanel.csx" company="CNC Software, LLC">
//   Copyright (c) 2024 CNC Software, LLC
// </copyright>
// <Author>
//  sdk@Mastercam.com
// </Author>
// <summary>
//   Create a side panel
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

// using Mastercam.App.Exceptions;
// using Mastercam.Curves;
// using Mastercam.GeometryUtility;
// using Mastercam.IO;
// using Mastercam.Math;

#endregion

#region Private Consts

/// <summary> The panel level. </summary>
private const int PanelLevel = 2;

/// <summary> The dado level. </summary>
private const int DadoLevel = 3;

/// <summary> The hole level. </summary>
private const int HoleLevel = 4;

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
		var result = Create();
		if (result.IsFailure)
		{
			DialogManager.OK(result.Error, "Error");
		}
		else
		{
			GraphicsManager.FitScreen();
			DialogManager.OK("Process complete.", "Create Panel");
		}
	}

#region Private Methods

	/// <summary> Creates a new Cabinet part. </summary>
	///
	/// <returns> A Result error if fail, success otherwise. </returns>
	private Result Create()
	{
		// Start a new session
		var success = FileManager.New(true);
		if (!success)
		{
			return Result.Fail("Failed to open new session");
		}

		// Create Holes
		var result = CreateHoles();
		if (result.IsFailure)
		{
			return Result.Fail(result.Error);
		}

		// Create Dados
		result = CreateDados();
		if (result.IsFailure)
		{
			return Result.Fail(result.Error);
		}

		result = CreateOutline();
		return result.IsFailure ? Result.Fail(result.Error) : Result.Ok();
	}

	/// <summary> Creates the outline. </summary>
	///
	/// <returns> A Result error if fail, success otherwise. </returns>
	private Result CreateOutline()
	{
		try
		{
			// define the profile
			var profile = new List<Line3D>
			{
				new Line3D(new Point3D(0, 0, 0), new Point3D(0, 10, 0)),
			new Line3D(new Point3D(0, 10, 0), new Point3D(10, 10, 0)),
			new Line3D(new Point3D(10, 10, 0), new Point3D(10, 2, 0)),
			new Line3D(new Point3D(10, 2, 0), new Point3D(8, 2, 0)),
			new Line3D(new Point3D(8, 2, 0), new Point3D(8, 0, 0)),
			new Line3D(new Point3D(8, 0, 0), new Point3D(0, 0, 0))
			};

			// create profile and commit to database
			profile.ForEach(l =>
				{
					var line = new LineGeometry(l)
					{
						Level = PanelLevel
					};

					if (!line.Commit())
					{
						throw new MastercamException("Failed to create panel line");
					}
				});
		}
		catch (MastercamException e)
		{
			return Result.Fail(e.Message);
		}
		catch (Exception e)
		{
			return Result.Fail(e.Message);
		}

		return Result.Ok();
	}

	/// <summary> Creates the dados. </summary>
	///
	/// <returns> A Result error if fail, success otherwise. </returns>
	private Result CreateDados()
	{
		try
		{
			// bottom dado
			var point1 = new Point3D(.250, .500, 0);
			var point2 = new Point3D(7.500, .250, 0);

			var bottomDado = GeometryCreationManager.CreateRectangle(point1, point2);
			if (!bottomDado.Any())
			{
				return Result.Fail("Create bottom dado failed.");
			}

			// side dado
			point1 = new Point3D(9.500, 9.750, 0);
			point2 = new Point3D(9.750, 2.225, 0);

			var sideDado = GeometryCreationManager.CreateRectangle(point1, point2);
			if (!sideDado.Any())
			{
				return Result.Fail("Create side dado failed.");
			}

			// combine 
			var all = new List<LineGeometry>();
			all.AddRange(bottomDado);
			all.AddRange(sideDado);

			// iterate over the list and set the level
			all.ForEach(l =>
				{
					l.Level = DadoLevel;
					if (!l.Commit())
					{
						throw new MastercamException("Failed to create dados");
					}
				});
		}
		catch (MastercamException e)
		{
			return Result.Fail(e.Message);
		}
		catch (Exception e)
		{
			return Result.Fail(e.Message);
		}

		return Result.Ok();
	}

	/// <summary> Creates the dowel holes. </summary>
	///
	/// <returns> The Result of the method </returns>
	private Result CreateHoles()
	{
		try
		{
			// 2 columns 6" apart
			for (var i = 1; i < 12; i += 6)
			{
				for (var j = 2; j <= 7; j++)
				{
					// 5mm holes
					var hole = new ArcGeometry
					{
						Data =
						{
							Radius = 0.0635,
						CenterPoint = new Point3D(i, 1.2598 * j, 0),
						StartAngleDegrees = 0,
						EndAngleDegrees = 360
						},
					Level = HoleLevel
					};

					if (!hole.Commit())
					{
						return Result.Fail("Failed to create arc");
					}
				}
			}
		}
		catch (Exception e)
		{
			return Result.Fail(e.Message);
		}

		return Result.Ok();
	}

#endregion
}

/// <summary> A result. </summary>
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
	/// The error. 
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

/// <summary> A result. </summary>
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
	/// The value. 
	/// </param>
	/// <param name="isSuccess">
	/// True if this object is success. 
	/// </param>
	/// <param name="error">
	/// The error. 
	/// </param>
	// ReSharper disable once StyleCop.SA1201
	// ReSharper disable once StyleCop.SA1642
	protected internal Result(T v, bool isSuccess, string error)
			: base (isSuccess, error) => value = v;
}

#endregion
