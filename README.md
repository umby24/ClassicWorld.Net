ClassicWorld.NET
================
ClassicWorld.NET is a general use library for loading and saving Minecraft Classic maps saved in the ClassicWorld format.


The ClassicWorld format is documented [Here](http://wiki.vg/ClassicWorld_file_format), and is used by the Classicube Client for saving worlds.


Features
--------
* Load and save GZip-compressed ClassicWorld maps.
* Provides an interface (and two examples) for saving and loading custom map Metadata.
* Supports CPE Metadata
* Perserves Metadata of other servers

Examples
--------
Loading a map

    var CWMap = new ClassicWorld(Filename);
	CWMap.Load();

That's it! Now you can access each of the public members of the map, including the block data.


Saving a map

	var CWMap = new ClassicWorld(Filename);
	CWMap.Load();
	CWMap.Save("MyWorld.cw");

That's it! The map will be saved in the current working directory as "MyWorld.cw", gzip compressed.
Any metadata that the library did not reconize will be there as well.

Adding a custom Metadata Handler, Loading a map, changing some properties, then saving it.

	var CWMap = new ClassicWorld(Filename);
	CWMap.MetadataParsers.Add("MyCustom", MyCustomParser);
	CWMap.Load();
	
	// -- Changes the map spawn point.
	CWMap.SpawnX = 2;
	CWMap.SpawnY = 30;
	CWMap.SpawnZ = 55;
	
	// -- Changes the block at index 20 to Wood
	CWMap.BlockData[20] = 5;
	
	// -- Modify CPE Data
	CWMap.MetadataParsers["CPE"].TextureURL = "http://google.com/";
	
	CWMap.Save("MyWorld.cw");

That's it! If you wish to know the names of the properties, simply browse the source!

Pull requests welcome.
