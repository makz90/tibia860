using OpenTibia.Common.Structures;
using System.Xml.Linq;

namespace OpenTibia.FileFormats.Xml.Spawns
{
    public class Npc
    {
        public static Npc Load(Spawn spawn, XElement npcNode)
        {
            Npc npc = new Npc();

            npc.Name = (string)npcNode.Attribute("name");

            npc.Position = new Position(spawn.Center.X + (int)npcNode.Attribute("x"), spawn.Center.Y + (int)npcNode.Attribute("y"), spawn.Center.Z);

            return npc;
        }

        public string Name { get; set; }

        public Position Position { get; set; }
    }
}