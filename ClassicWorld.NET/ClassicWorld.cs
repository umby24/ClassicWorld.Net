using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using fNbt;

namespace ClassicWorld_NET
{
    public interface IMetadataStructure {
        NbtCompound Read(NbtCompound Metadata);
        NbtCompound Write();
    }

    public struct ForeignMeta : IMetadataStructure {
        public NbtTag[] Tags { get; set; }

        public NbtCompound Read(NbtCompound Metadata) {
            Tags = new NbtTag[Metadata.Tags.Count()];
            Metadata.CopyTo(Tags, 0);

            foreach (NbtTag b in Tags) 
                Metadata.Remove(b);

            return Metadata;
        }

        public NbtCompound Write() {
            var newCompound = new NbtCompound("Metadata");

            if (Tags != null) {
                foreach (NbtTag b in Tags) 
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
        public string TextureURL;
        public byte SideBlock;
        public byte EdgeBlock;
        public short SideLevel;

        public NbtCompound Read(NbtCompound Metadata) {
            NbtCompound CPEData = Metadata.Get<NbtCompound>("CPE");

            if (CPEData != null) {
                if (CPEData["ClickDistance"] != null) {
                    ClickDistanceVersion = CPEData["ClickDistance"]["ExtensionVersion"].IntValue;
                    ClickDistance = CPEData["ClickDistance"]["Distance"].ShortValue;
                }

                if (CPEData["CustomBlocks"] != null) {
                    CustomBlocksVersion = CPEData["CustomBlocks"]["ExtensionVersion"].IntValue;
                    CustomBlocksLevel = CPEData["CustomBlocks"]["SupportLevel"].ShortValue;
                    CustomBlocksFallback = CPEData["CustomBlocks"]["Fallback"].ByteArrayValue;
                }

                if (CPEData["EnvColors"] != null) {
                    EnvColorsVersion = CPEData["EnvColors"]["ExtensionVersion"].IntValue;
                    SkyColor = new short[] { CPEData["EnvColors"]["Sky"]["R"].ShortValue, CPEData["EnvColors"]["Sky"]["G"].ShortValue, CPEData["EnvColors"]["Sky"]["B"].ShortValue };
                    CloudColor = new short[] { CPEData["EnvColors"]["Cloud"]["R"].ShortValue, CPEData["EnvColors"]["Cloud"]["G"].ShortValue, CPEData["EnvColors"]["Cloud"]["B"].ShortValue };
                    FogColor = new short[] { CPEData["EnvColors"]["Fog"]["R"].ShortValue, CPEData["EnvColors"]["Fog"]["G"].ShortValue, CPEData["EnvColors"]["Fog"]["B"].ShortValue };
                    AmbientColor = new short[] { CPEData["EnvColors"]["Ambient"]["R"].ShortValue, CPEData["EnvColors"]["Ambient"]["G"].ShortValue, CPEData["EnvColors"]["Ambient"]["B"].ShortValue };
                    SunlightColor = new short[] { CPEData["EnvColors"]["Sunlight"]["R"].ShortValue, CPEData["EnvColors"]["Sunlight"]["R"].ShortValue, CPEData["EnvColors"]["Sunlight"]["R"].ShortValue };
                }

                if (CPEData["EnvMapAppearance"] != null) {
                    EnvMapAppearanceVersion = CPEData["EnvMapAppearance"]["ExtensionVersion"].IntValue;
                    TextureURL = CPEData["EnvMapAppearance"]["TextureURL"].StringValue;
                    SideBlock = CPEData["EnvMapAppearance"]["SideBlock"].ByteValue;
                    EdgeBlock = CPEData["EnvMapAppearance"]["EdgeBlock"].ByteValue;
                    SideLevel = CPEData["EnvMapAppearance"]["SideLevel"].ShortValue;
                }

                Metadata.Remove(CPEData);
            }

            return Metadata;
        }

        public NbtCompound Write() {
            var BaseCPE = new NbtCompound("CPE");

            if (ClickDistanceVersion > 0) {
                var ClickDistanceTag = new NbtCompound("ClickDistance") {
                    new NbtInt("ExtensionVersion", ClickDistanceVersion),
                    new NbtShort("Distance", ClickDistance)
                };

                BaseCPE.Add(ClickDistanceTag);
            }

            if (CustomBlocksVersion > 0) {
                var CustomBlocksTag = new NbtCompound("CustomBlocks") {
                    new NbtInt("ExtensionVersion", CustomBlocksVersion),
                    new NbtShort("SupportLevel", CustomBlocksLevel),
                    new NbtByteArray("Fallback", CustomBlocksFallback)
                };

                BaseCPE.Add(CustomBlocksTag);
            }

            if (EnvColorsVersion > 0) {
               var EnvColorsTag = new NbtCompound("EnvColors") {
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

                BaseCPE.Add(EnvColorsTag);
            }

            if (EnvMapAppearanceVersion > 0) {
                var EnvAppearanceTag = new NbtCompound("EnvMapAppearance") {
                    new NbtInt("ExtensionVersion",EnvMapAppearanceVersion),
                    new NbtString("TextureURL",TextureURL),
                    new NbtByte("SideBlock",SideBlock),
                    new NbtByte("EdgeBlock", EdgeBlock),
                    new NbtShort("SideLevel", SideLevel)
                };

                BaseCPE.Add(BaseCPE);
            }

            if (BaseCPE.Tags.Count() > 0)
                return BaseCPE;
            else
                return null;
        }
    }

    public class ClassicWorld {
        public byte FormatVersion;
        public string MapName;
        public byte[] UUID;
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
        NbtCompound Basetag;

        /// <summary>
        /// Creates a new ClassicWorld map.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        public ClassicWorld(short X, short Y, short Z) {
            RandomNumberGenerator Random = RandomNumberGenerator.Create();

            UUID = new byte[16]; // -- Generate a random UUID
            Random.GetBytes(UUID);

            BlockData = new byte[X * Y * Z]; // -- Creates a blank map.

            FormatVersion = 1;
            SizeX = X;
            SizeY = Y;
            SizeZ = Z;
            TimeCreated = GetCurrentUnixTime();
            LastAccessed = GetCurrentUnixTime();
            LastModified = GetCurrentUnixTime();

            MetadataParsers = new Dictionary<string, IMetadataStructure>();
            MetadataParsers.Add("CPE", new CPEMetadata());
        }

        /// <summary>
        /// Loads an already existing ClassicWorld map
        /// </summary>
        /// <param name="Filename">The path to the map file.</param>
        public ClassicWorld(string Filename) {
            var myFile = new NbtFile(Filename);
            Basetag = myFile.RootTag;

            if (Basetag.Name != "ClassicWorld")
                throw new FormatException("Not a valid Classicworld file. Basetag name is not 'ClassicWorld'.");

            MetadataParsers = new Dictionary<string, IMetadataStructure>();
            MetadataParsers.Add("CPE", new CPEMetadata());
        }

        /// <summary>
        /// Loads the ClassicWorld map.
        /// </summary>
        public void Load() {
            FormatVersion = Basetag["FormatVersion"].ByteValue;

            if (FormatVersion != 1)
                throw new FormatException("Unsupported format version: " + FormatVersion.ToString());

            Foreignmeta = new ForeignMeta();

            MapName = Basetag["Name"].StringValue;

            UUID = Basetag["UUID"].ByteArrayValue;

            SizeX = Basetag["X"].ShortValue;
            SizeY = Basetag["Y"].ShortValue;
            SizeZ = Basetag["Z"].ShortValue;

            var CreatedBy = Basetag.Get<NbtCompound>("CreatedBy");

            if (CreatedBy != null) {
                CreatingService = CreatedBy["Service"].StringValue;
                CreatingUsername = CreatedBy["Username"].StringValue;
            }

            var Mapgen = Basetag.Get<NbtCompound>("MapGenerator");

            if (Mapgen != null) {
                GeneratingSoftware = Mapgen["Software"].StringValue;
                GeneratorName = Mapgen["MapGeneratorName"].StringValue;
            }

            if (Basetag["TimeCreated"] != null)
                TimeCreated = Basetag["TimeCreated"].LongValue;

            if (Basetag["LastAccessed"] != null)
                LastAccessed = Basetag["LastAccessed"].LongValue;

            if (Basetag["LastModified"] != null)
                LastModified = Basetag["LastModified"].LongValue;

            var Spawnpoint = Basetag.Get<NbtCompound>("Spawn");

            if (Spawnpoint == null)
                throw new FormatException("Spawn not found.");
            else {
                SpawnX = Spawnpoint["X"].ShortValue;
                SpawnY = Spawnpoint["Y"].ShortValue;
                SpawnZ = Spawnpoint["Z"].ShortValue;
                SpawnRotation = Spawnpoint["H"].ByteValue;
                SpawnLook = Spawnpoint["P"].ByteValue;
            }

            BlockData = Basetag["BlockArray"].ByteArrayValue;

            var Metadata = Basetag.Get<NbtCompound>("Metadata");

            if (Metadata != null) {
                // -- Let user-defined metadata parsers parse metadata...
                foreach (IMetadataStructure Meta in MetadataParsers.Values)
                    Metadata = Meta.Read(Metadata);

                // -- Store all foreign metadata
                Metadata = Foreignmeta.Read(Metadata);
            }

            // -- Now that the map is loaded, we have to ensure all of the required values were included.

            if (BlockData == null)
                throw new FormatException("BlockArray not found.");

            if (SizeX == 0 || SizeY == 0 || SizeZ == 0)
                throw new FormatException("Map size not found.");

            if (FormatVersion == 0 || MapName == null || UUID == null)
                throw new FormatException("Map header information not found.");

            if (LastAccessed != 0)
                LastAccessed = GetCurrentUnixTime();

            Basetag = null;
        }

        /// <summary>
        /// Saves the ClassicWorld map.
        /// </summary>
        public void Save(string Filename) {
            var NbtMetadata = Foreignmeta.Write();

            foreach (IMetadataStructure b in MetadataParsers.Values) {
                var Nbt = b.Write();

                if (Nbt != null)
                    NbtMetadata.Add(Nbt);
            }

            var compound = new NbtCompound("ClassicWorld") {
                new NbtByte("FormatVersion", 1),
                new NbtString("Name", MapName),
                new NbtByteArray("UUID", UUID),
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
                NbtMetadata
            };

            if (CreatingService != null && CreatingUsername != null) {
                var CreatedByTag = new NbtCompound("CreatedBy") {
                    new NbtString("Service", CreatingService),
                    new NbtString("Username", CreatingUsername)
                };

                compound.Add(CreatedByTag);
            }

            if (GeneratingSoftware != null && GeneratorName != null) {
                var MapGenerator = new NbtCompound("MapGenerator") {
                    new NbtString("Software", GeneratingSoftware),
                    new NbtString("MapGeneratorName", GeneratorName)
                };

                compound.Add(MapGenerator);
            }

            if (TimeCreated != 0.0)
                compound.Add(new NbtLong("TimeCreated", TimeCreated));

            if (LastAccessed != 0.0)
                compound.Add(new NbtLong("LastAccessed", LastAccessed));

            if (LastModified != 0.0)
                compound.Add(new NbtLong("LastModified", LastModified));

            var myFile = new NbtFile(compound);
            myFile.SaveToFile(Filename, NbtCompression.GZip);

            compound = null;
            myFile = null;
        }

        private static readonly DateTime UnixEpoch =
            new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
            
        private long GetCurrentUnixTime() {
            TimeSpan timeSinceEpoch = (DateTime.UtcNow - UnixEpoch);
            return (long)timeSinceEpoch.TotalSeconds;
        }
    }
}
