// -----------------------------------------------------------------------------------------
// <copyright file="BatchImportFiles.csx" company="">CNC Software, LLC</copyright>
// <Author>sdk@mastercam.com</Author>
// <summary>
//   This script will prompt the user for a folder containing dxf files to be imported and 
//   saved as Mastercam part files.
// </summary>
// -----------------------------------------------------------------------------------------

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
		// Clear the current session, if user cancels exit the script
		if (!FileManager.New(true))
		{
			return ;
		}

		try
		{
			var files = PromptForFiles();

			// Validate that we have at least 1 file to process
			if (files.Any())
			{
				// Determine how many files to proccess
				var fileCount = files.Length;

				// Ask user if they want to continue
				var ok = DialogManager.YesNo(string.Format(AppStrings.Continue, fileCount), DialogStrings.Title);
				if (ok != DialogReturnType.Yes)
				{
					return ;
				}

				// The current file counter
				var index = 0;

				// Remove any prompts
				PromptManager.Clear();

				// Iterate over the array
				foreach (var file in files)
				{
					// increment the file counter
					index++;

					// Display some information to the user while Mastercam is processing
					PromptManager.WriteString(string.Format(AppStrings.Prompt, index, fileCount, file));

					// Attempt to open this dxf file
					var success = FileManager.Open(file);
					if (success)
					{
						// Build the full filepath to a Mastercam file replacing the search file extension (.dxf) with .mcam
						var mastercamFile = file.Replace(AppStrings.SearchExtension, AppStrings.MastercamExtension);

						// Attempt to save the file
						success = FileManager.SaveAs(mastercamFile);
						if (!success)
						{
							// Inform user file save failed and show the filepath we tried to save
							DialogManager.Error(string.Format(AppStrings.FileSaveFailed, mastercamFile), DialogStrings.Title);
							return ;
						}
					}
					else
					{
						// Inform user file open failed and show the filepath we tried to open
						DialogManager.OK(string.Format(AppStrings.FileOpenFailed, file), DialogStrings.Title);
						return ;
					}
				}

				// If we get here we are done
				DialogManager.OK(AppStrings.ImportComplete, DialogStrings.Title);
			}
			else
			{
				// Failed to get a list of files
				DialogManager.OK(AppStrings.NoFilesFound, DialogStrings.Title);
				return ;
			}
		}
		catch (Exception ex)
		{
			// Inform the user an unhandled exception has occured
			DialogManager.Exception(new MastercamException(ex.Message, ex.InnerException));
		}
		finally
		{
			// Make sure to remove any prompts
			PromptManager.Clear();
		}
	}

#region Private Methods

	/// <summary>
	/// Private method that will get a list of files from a folder
	/// </summary>
	/// <returns>An array of file names on success, an empty array on failure or cancel</returns>
	private string[] PromptForFiles()
	{
		try
		{
			// browse for folder
			using (var browser = new FolderBrowserDialog())
			{
				browser.ShowNewFolderButton = false;
				browser.Description = DialogStrings.Description;

				// Display the browser and check for a valid result
				if (browser.ShowDialog() == DialogResult.OK)
				{
					var folder = browser.SelectedPath;
					return Directory.GetFiles(folder, AppStrings.SearchPattern, SearchOption.AllDirectories);
				}
			}
		}
		catch (Exception ex)
		{
			// Inform the user an unhandled exception has occured
			DialogManager.Exception(new MastercamException(ex.Message, ex.InnerException));
		}

		// returns default state
		return default;
	}

#endregion
}

/// <summary>
/// Class that contains our dialog strings
/// </summary>
public abstract class DialogStrings
{
	// dialog title
	public static string Title = "Batch Import Files";
	// dialog description
	public static string Description = "Choose Files Folder";
}

/// <summary>
/// Class that will contain our strings for the script
/// </summary>
public abstract class AppStrings
{
	// message
	public static string NoFilesFound = "There are no files in the selected folder";

	// message
	public static string FileSaveFailed = "Failed to save file {0}";

	// message
	public static string FileOpenFailed = "Failed to open file {0}";

	// message
	public static string ImportComplete = "Import complete";

	// message
	public static string ImportError = "Error importing files:";

	// mastercam prompt
	public static string Prompt = "Importing file {0} of {1}...\n{2}";

	// message
	public static string Continue = "{0} files found, do you wish to continue?";

	// edit to suit the file type to import
	public static string SearchPattern = "*.dxf";
	public static string SearchExtension = ".dxf";

	// file type to save
	public static string MastercamExtension = ".mcam";
}

#endregion
