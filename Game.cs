namespace FirstPersonGameEngine
{
    using JunoEngine.Systems;
    using System.Collections.Generic;
    using System.Drawing;

    public class Game
    {
        public Unit[] units;

        public Color[] wallColors = new Color[]
        {
            Color.White,
            Color.DarkBlue,
            Color.Green,
            Color.Blue
        };

        public Color GetWallColor(char wallCharacter)
        {
            int val;
            int.TryParse(wallCharacter.ToString(), out val);

            var color = wallColors[0];

            if (val < wallColors.Length)
                color = wallColors[val];

            return color;
        }

        public int mapHeight = 32, mapWidth = 32;

        public string mapData
        {
            get
            {
                return $"00000000000000000000000000000000" +
                       $"0              02              2" +
                       $"0   o          02              2" +
                       $"0              02              2" +
                       $"11  1111111   002              2" +
                       $"1        1     02              2" +
                       $"1        1  o                  2" +
                       $"1        1     02              2" +
                       $"1111111  11111112  2222222222222" +
                       $"1             10               2" +
                       $"1             10               2" +
                       $"1   o         100  222222222   2" +
                       $"1             10           00000" +
                       $"1      p      10               0" +
                       $"1111111111111110           0   0" +
                       $"000000000000000000000    000   0" +
                       $"0                              0" +
                       $"0                              0" +
                       $"0                              0" +
                       $"0                              0" +
                       $"0                              0" +
                       $"0                              0" +
                       $"0                              0" +
                       $"0                              0" +
                       $"0                              0" +
                       $"0                              0" +
                       $"0                              0" +
                       $"3333333333333    333333333333333" +
                       $"3                              3" +
                       $"3                              3" +
                       $"3                              3" +
                       $"33333333333333333333333333333333";
            }
        }

        public Game(params Unit[] units)
        {
            var list = GetUnitsFromMap();

            foreach (var unit in units)
            {
                list.Add(unit);
            }

            this.units = list.ToArray();
        }

        public List<Unit> GetUnitsFromMap()
        {
            var units = new List<Unit>();

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    var character = mapData[(y * mapWidth + x)];

                    // item is not a wall, and is not floor
                    if (char.IsDigit(character) == false && character != ' ')
                    {
                        var pos = new Vector3(x, y);

                        units.Add(new Unit(pos, 30, character));
                    }
                }
            }

            return units;
        }

        public Vector3 GetUnitPosistion(Unit unit)
        {
            var pos = Vector3.one;

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (mapData[(y * mapWidth + x)] == unit.character)
                    {
                        pos = new Vector3(x, y);
                    }
                }
            }

            return pos;
        }
    }
}