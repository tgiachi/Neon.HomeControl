#addin "Cake.XdtTransform"
#addin "Cake.Docker"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var solutionFile = GetFiles("./*.sln").First();
var solution = new Lazy<SolutionParserResult>(() => ParseSolution(solutionFile));
var distDir = Directory("./dist");
var buildDir = Directory("./build");

Task("Clean")
	.IsDependentOn("Clean-Outputs")
	.Does(() => 
	{
		 MSBuild(solutionFile, settings => settings
			.SetConfiguration(configuration)
			.WithTarget("Clean")
			.SetVerbosity(Verbosity.Minimal));
	});

Task("Clean-Outputs")
	.Does(() => 
	{
		CleanDirectory(buildDir);
		CleanDirectory(distDir);
	});



Task("Build")
	.IsDependentOn("Clean-Outputs")
    .Does(() =>
	{
		NuGetRestore(solutionFile);

		 MSBuild(solutionFile, settings => settings
			.SetConfiguration(configuration)
			.WithTarget("Rebuild")
			.SetVerbosity(Verbosity.Minimal));
    });

Task("Websites")
	.Does(() =>
	{
		var webProjects = solution.Value
			.Projects
			.Where(p => p.Name.EndsWith(".Web"));

		foreach(var project in webProjects)
		{
			Information("Publishing {0}", project.Name);
			
			var publishDir = distDir + Directory(project.Name);

			 MSBuild(project.Path, settings => settings
				.SetConfiguration(configuration)
				.WithProperty("DeployOnBuild", "true")
				.WithProperty("WebPublishMethod", "FileSystem")
				.WithProperty("DeployTarget", "WebPublish")
				.WithProperty("publishUrl", MakeAbsolute(publishDir).FullPath)
				.SetVerbosity(Verbosity.Minimal));

			Zip(publishDir, distDir + File(project.Name + ".zip"));
		}
	});

Task("Consoles")
	.Does(() =>
	{
		var consoleProjects = solution.Value
			.Projects
			.Where(p => p.Name.EndsWith(".Console"));

		foreach(var project in consoleProjects)
		{
			Information("Publishing {0}", project.Name);

			var projectDir = project.Path.GetDirectory(); 
			var publishDir = distDir + Directory(project.Name);

			Information("Copying to output directory");
			CopyDirectory(
				projectDir.Combine("bin").Combine(configuration),
				publishDir);

			var configFile = publishDir + File(project.Name + ".exe.config");
			var transformFile = projectDir.CombineWithFilePath("App." + configuration + ".config");
			Information("Transforming configuration file");
			XdtTransformConfig(configFile, transformFile, configFile);

			Zip(publishDir, distDir + File(project.Name + ".zip"));
		}
	});
	
Task("Default")
	.IsDependentOn("Websites")
	.IsDependentOn("Consoles");

RunTarget(target);
RunTarget(target);