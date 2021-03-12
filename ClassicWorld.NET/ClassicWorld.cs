using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using fNbt;

namespace ClassicWorld.NET
{
    public interface IMetadataStructure {
        NbtCompound Read(NbtCompound metadata);
        NbtCompound Write();
    }

    public struct ForeignMeta : IMetadataStructure {
        private NbtTag[] Tags { get; set; }

        public NbtCompound Read(NbtCompound metadata) {
            Tags = new NbtTag[metadata.Tags.Count()];
            metadata.CopyTo(Tags, 0);

            foreach (NbtTag b in Tags) 
                metadata.Remove(b);

            return metadata;
        }

        public NbtCompound Write() {
            var newCompound = new NbtCompound("Metadata");

            if (Tags == null) 
                return newCompound;

            foreach (NbtTag b in Tags) {
                ((NbtCompound) b.Parent)?.Remove(b);

                newCompound.Add(b);
            }

            return newCompound;
        }
    }

    public struct CPEMetadata : IMetadataStructure {
        public int ClickDistanceVersion;
        public short ClickDistance;

        public int CustomBlocksVersion;
        public short CustomBlocksLevel;
        public byte[] CustomBlocksFallback;

        public int EnvColorsVersion;
        public short[] SkyColor;
        public short[] CloudColor;
        public short[] FogColor;
        public short[] AmbientColor;
        public short[] SunlightColor;

        public int EnvMapAppearanceVersion;
        public string TextureUrl;
        public byte SideBlock;
        public byte EdgeBlock;
        public short SideLevel;

        public byte Weather;

        public NbtCompound Read(NbtCompound metadata) {
            var cpeData = metadata.Get<NbtCompound>("CPE");

            if (cpeData == null) 
                return metadata;

            if (cpeData["ClickDistance"] != null) {
                if (cpeData["ClickDistance"]["ExtensionVersion"] != null) {
                    ClickDistanceVersion = cpeData["ClickDistance"]["ExtensionVersion"].IntValue;
                    ClickDistance = cpeData["ClickDistance"]["Distance"].ShortValue;
                }
            }

            if (cpeData["CustomBlocks"] != null) {
                if (cpeData["CustomBlocks"]["ExtensionVersion"] != null) {
                    CustomBlocksVersion = cpeData["CustomBlocks"]["ExtensionVersion"].IntValue;
                    CustomBlocksLevel = cpeData["CustomBlocks"]["SupportLevel"].ShortValue;
                    CustomBlocksFallback = cpeData["CustomBlocks"]["Fallback"].ByteArrayValue;
                }
            }

            if (cpeData["EnvColors"] != null) {
                if (cpeData["EnvColors"]["ExtensionVersion"] != null) {
                    EnvColorsVersion = cpeData["EnvColors"]["ExtensionVersion"].IntValue;
                    SkyColor = new[] { cpeData["EnvColors"]["Sky"]["R"].ShortValue, cpeData["EnvColors"]["Sky"]["G"].ShortValue, cpeData["EnvColors"]["Sky"]["B"].ShortValue };
                    CloudColor = new[] { cpeData["EnvColors"]["Cloud"]["R"].ShortValue, cpeData["EnvColors"]["Cloud"]["G"].ShortValue, cpeData["EnvColors"]["Cloud"]["B"].ShortValue };
                    FogColor = new[] { cpeData["EnvColors"]["Fog"]["R"].ShortValue, cpeData["EnvColors"]["Fog"]["G"].ShortValue, cpeData["EnvColors"]["Fog"]["B"].ShortValue };
                    AmbientColor = new[] { cpeData["EnvColors"]["Ambient"]["R"].ShortValue, cpeData["EnvColors"]["Ambient"]["G"].ShortValue, cpeData["EnvColors"]["Ambient"]["B"].ShortValue };
                    SunlightColor = new[] { cpeData["EnvColors"]["Sunlight"]["R"].ShortValue, cpeData["EnvColors"]["Sunlight"]["R"].ShortValue, cpeData["EnvColors"]["Sunlight"]["R"].ShortValue };
                }
            }

            if (cpeData["EnvMapAppearance"] != null) {
                if (cpeData["EnvMapAppearance"]["ExtensionVersion"] != null) {
                    EnvMapAppearanceVersion = cpeData["EnvMapAppearance"]["ExtensionVersion"].IntValue;
                    TextureUrl = cpeData["EnvMapAppearance"]["TextureURL"].StringValue;
                    SideBlock = cpeData["EnvMapAppearance"]["SideBlock"].ByteValue;
                    EdgeBlock = cpeData["EnvMapAppearance"]["EdgeBlock"].ByteValue;
                    SideLevel = cpeData["EnvMapAppearance"]["SideLevel"].ShortValue;
                }
            }

            if (cpeData["EnvWeatherType"] != null)
                Weather = cpeData["EnvWeatherType"]["WeatherType"].ByteValue;

            metadata.Remove(cpeData);

            return metadata;
        }

        public NbtCompound Write() {
            var baseCPE = new NbtCompound("CPE");

            if (ClickDistanceVersion > 0) {
                var clickDistanceTag = new NbtCompound("ClickDistance") {
                    new NbtInt("ExtensionVersion", ClickDistanceVersion),
                    new NbtShort("Distance", ClickDistance)
                };

                baseCPE.Add(clickDistanceTag);
            }

            if (CustomBlocksVersion > 0) {
                var customBlocksTag = new NbtCompound("CustomBlocks") {
                    new NbtInt("ExtensionVersion", CustomBlocksVersion),
                    new NbtShort("SupportLevel", CustomBlocksLevel),
                    new NbtByteArray("Fallback", CustomBlocksFallback)
                };

                baseCPE.Add(customBlocksTag);
            }

            if (EnvColorsVersion > 0) {
               var envColorsTag = new NbtCompound("EnvColors") {
                    new NbtInt("ExtensionVersion", EnvColorsVersion),
                    new NbtCompound("Sky") {
                        new NbtShort("R", SkyColor[0]),
                        new NbtShort("G", SkyColor[1]),
                        new NbtShort("B", SkyColor[2])
                    },
                    new NbtCompound("Cloud") {
                        new NbtShort("R", CloudColor[0]),
                        new NbtShort("G", CloudColor[1]),
                        new NbtShort("B", CloudColor[2])
                    },
                    new NbtCompound("Fog") {
                        new NbtShort("R", FogColor[0]),
                        new NbtShort("G", FogColor[1]),
                        new NbtShort("B", FogColor[2])
                    },
                    new NbtCompound("Ambient") {
                        new NbtShort("R", AmbientColor[0]),
                        new NbtShort("G", AmbientColor[1]),
                        new NbtShort("B", AmbientColor[2])
                    },
                    new NbtCompound("Sunlight") {
                        new NbtShort("R", SunlightColor[0]),
                        new NbtShort("G", SunlightColor[1]),
                        new NbtShort("B", SunlightColor[2])
                    }
                };

                baseCPE.Add(envColorsTag);
            }

            if (EnvMapAppearanceVersion > 0) {
                var envAppearanceTag = new NbtCompound("EnvMapAppearance") {
                    new NbtInt("ExtensionVersion",EnvMapAppearanceVersion),
                    new NbtString("TextureURL",TextureUrl),
                    new NbtByte("SideBlock",SideBlock),
                    new NbtByte("EdgeBlock", EdgeBlock),
                    new NbtShort("SideLevel", SideLevel)
                };

                baseCPE.Add(envAppearanceTag);
            }

            var weatherTag = new NbtCompound("EnvWeatherType") {
                new NbtByte("WeatherType", Weather),
            };

            baseCPE.Add(weatherTag);

            return baseCPE.Tags.Any() ? baseCPE : null;
        }
    }

    public class Classicworld {
        public byte FormatVersion;
        public string MapName;
        public byte[] Uuid;
        public short SizeX, SizeY, SizeZ;
        public string CreatingService, CreatingUsername;
        public string GeneratingSoftware, GeneratorName;
        public long TimeCreated, LastAccessed, LastModified;
        public short SpawnX, SpawnY, SpawnZ;
        public byte SpawnRotation, SpawnLook;
        public byte[] BlockData;
        public ForeignMeta Foreignmeta;
        public Dictionary<string, IMetadataStructure> MetadataParsers;

        // -- Non-public variables
        NbtCompound _basetag;

        /// <summary>
        /// Creates a new Classicworld map.
        /// </summary>
        /// <param name="x">X size of the map.</param>
        /// <param name="y">Y size of the map.</param>
        /// <param name="z">Z size of the map.</param>
        public Classicworld(short x, short y, short z) {
            var random = RandomNumberGenerator.Create();

            Uuid = new byte[16]; // -- Generate a random UUID
            random.GetBytes(Uuid);

            BlockData = new byte[x * y * z]; // -- Creates a blank map.

            FormatVersion = 1;
            SizeX = x;
            SizeY = y;
            SizeZ = z;
            TimeCreated = GetCurrentUnixTime();
            LastAccessed = GetCurrentUnixTime();
            LastModified = GetCurrentUnixTime();

            MetadataParsers = new Dictionary<string, IMetadataStructure> {{"CPE", new CPEMetadata()}};
        }

        /// <summary>
        /// Loads an already existing Classicworld map
        /// </summary>
        /// <param name="filename">The path to the map file.</param>
        public Classicworld(string filename) {
            var myFile = new NbtFile(filename);
            _basetag = myFile.RootTag;

            if (_basetag.Name != "ClassicWorld")
                throw new FormatException("Not a valid Classicworld file. Basetag name is not 'ClassicWorld'.");

            MetadataParsers = new Dictionary<string, IMetadataStructure> {{"CPE", new CPEMetadata()}};
        }

        /// <summary>
        /// Loads the Classicworld map.
        /// </summary>
        public void Load() {
            BlockData = null;
            FormatVersion = _basetag["FormatVersion"].ByteValue;

            if (FormatVersion != 1)
                throw new FormatException("Unsupported format version: " + FormatVersion);

            Foreignmeta = new ForeignMeta();
            
            if (_basetag.Contains("Name"))
                MapName = _basetag["Name"].StringValue;
            else
                MapName = "ClassicWorldMap";

            Uuid = _basetag["UUID"].ByteArrayValue;

            SizeX = _basetag["X"].ShortValue;
            SizeY = _basetag["Y"].ShortValue;
            SizeZ = _basetag["Z"].ShortValue;

            var createdBy = _basetag.Get<NbtCompound>("CreatedBy");

            if (createdBy != null) {
                CreatingService = createdBy["Service"].StringValue;
                CreatingUsername = createdBy["Username"].StringValue;
            }

            var mapgen = _basetag.Get<NbtCompound>("MapGenerator");

            if (mapgen != null) {
                GeneratingSoftware = mapgen["Software"].StringValue;
                GeneratorName = mapgen["MapGeneratorName"].StringValue;
            }

            if (_basetag["TimeCreated"] != null)
                TimeCreated = _basetag["TimeCreated"].LongValue;

            if (_basetag["LastAccessed"] != null)
                LastAccessed = _basetag["LastAccessed"].LongValue;

            if (_basetag["LastModified"] != null)
                LastModified = _basetag["LastModified"].LongValue;

            var spawnpoint = _basetag.Get<NbtCompound>("Spawn");

            if (spawnpoint == null)
                throw new FormatException("Spawn not found.");

            SpawnX = spawnpoint["X"].ShortValue;
            SpawnY = spawnpoint["Y"].ShortValue;
            SpawnZ = spawnpoint["Z"].ShortValue;
            SpawnRotation = spawnpoint["H"].ByteValue;
            SpawnLook = spawnpoint["P"].ByteValue;

            BlockData = _basetag["BlockArray"].ByteArrayValue;

            var metadata = _basetag.Get<NbtCompound>("Metadata");

            if (metadata != null) {
                // -- Let user-defined metadata parsers parse metadata...
                foreach (var meta in MetadataParsers.Values)
                    metadata = meta.Read(metadata);

                // -- Store all foreign metadata
                metadata = Foreignmeta.Read(metadata);
            }

            // -- Now that the map is loaded, we have to ensure all of the required values were included.

            if (BlockData == null)
                throw new FormatException("BlockArray not found.");

            if (SizeX == 0 || SizeY == 0 || SizeZ == 0)
                throw new FormatException("Map size not found.");

            if (FormatVersion == 0 || MapName == null || Uuid == null)
                throw new FormatException("Map header information not found.");

            if (LastAccessed != 0)
                LastAccessed = GetCurrentUnixTime();

            _basetag = null;
        }

        /// <summary>
        /// Saves the Classicworld map.
        /// </summary>
        /// <param name="filename">The file name/path to save the map to.</param>
        public void Save(string filename) {
            var nbtMetadata = Foreignmeta.Write();

            foreach (var b in MetadataParsers.Values) {
                var nbt = b.Write();

                if (nbt != null)
                    nbtMetadata.Add(nbt);
            }

            var compound = new NbtCompound("ClassicWorld") {
                new NbtByte("FormatVersion", 1),
                new NbtString("Name", MapName),
                new NbtByteArray("UUID", Uuid),
                new NbtShort("X", SizeX),
                new NbtShort("Y", SizeY),
                new NbtShort("Z", SizeZ),
                new NbtCompound("Spawn") {
                    new NbtShort("X", SpawnX),
                    new NbtShort("Y", SpawnY),
                    new NbtShort("Z", SpawnZ),
                    new NbtByte("H", SpawnRotation),
                    new NbtByte("P", SpawnLook)
                },
                new NbtByteArray("BlockArray", BlockData),
                nbtMetadata
            };

            if (CreatingService != null && CreatingUsername != null) {
                var createdByTag = new NbtCompound("CreatedBy") {
                    new NbtString("Service", CreatingService),
                    new NbtString("Username", CreatingUsername)
                };

                compound.Add(createdByTag);
            }

            if (GeneratingSoftware != null && GeneratorName != null) {
                var mapGenerator = new NbtCompound("MapGenerator") {
                    new NbtString("Software", GeneratingSoftware),
                    new NbtString("MapGeneratorName", GeneratorName)
                };

                compound.Add(mapGenerator);
            }

            compound.Add(new NbtLong("TimeCreated", TimeCreated));

            compound.Add(new NbtLong("LastAccessed", LastAccessed));

            compound.Add(new NbtLong("LastModified", LastModified));

            var myFile = new NbtFile(compound);
            myFile.SaveToFile(filename, NbtCompression.GZip);
        }

        private static readonly DateTime UnixEpoch =
            new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
            
        private static long GetCurrentUnixTime() {
            var timeSinceEpoch = (DateTime.UtcNow - UnixEpoch);
            return (long)timeSinceEpoch.TotalSeconds;
        }
    }
}
