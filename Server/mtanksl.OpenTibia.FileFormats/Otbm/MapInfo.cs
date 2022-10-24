﻿using OpenTibia.IO;
using System.Collections.Generic;

namespace OpenTibia.FileFormats.Otbm
{
    public class MapInfo
    {
        public static MapInfo Load(ByteArrayFileTreeStream stream, ByteArrayStreamReader reader)
        {
            MapInfo mapInfo = new MapInfo();

            stream.Seek(Origin.Current, 1);

            while (true)
            {
                switch ( (OtbmAttribute)reader.ReadByte() )
                {
                    case OtbmAttribute.Description:

                        mapInfo.descriptions.Add( reader.ReadString() );

                        break;

                    case OtbmAttribute.SpawnFile:

                        mapInfo.SpawnFile = reader.ReadString();

                        break;

                    case OtbmAttribute.HouseFile:

                        mapInfo.HouseFile = reader.ReadString();

                        break;

                    default:

                        stream.Seek(Origin.Current, -1);

                        return mapInfo;
                }
            }
        }

        private List<string> descriptions = new List<string>();

        public List<string> Descriptions
        {
            get
            {
                return descriptions;
            }
        }

        public string SpawnFile { get; set; }

        public string HouseFile { get; set; }
    }
}