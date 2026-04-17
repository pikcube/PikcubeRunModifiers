namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Extensions
{
    //Mostly utilities to get asset paths.
    public static class StringExtensions
    {
        extension(string path)
        {
            

            public string ImagePath()
            {
                return Path.Join(MainFile.ModId, "images", path);
            }

            public string CardImagePath()
            {
                return Path.Join(MainFile.ModId, "images", "card_portraits", path);
            }

            public string BigCardImagePath()
            {
                return Path.Join(MainFile.ModId, "images", "card_portraits", "big", path);
            }

            public string PowerImagePath()
            {
                return Path.Join(MainFile.ModId, "images", "powers", path);
            }

            public string BigPowerImagePath()
            {
                return Path.Join(MainFile.ModId, "images", "powers", "big", path);
            }

            public string RelicImagePath()
            {
                return Path.Join(MainFile.ModId, "images", "relics", path);
            }

            public string BigRelicImagePath()
            {
                return Path.Join(MainFile.ModId, "images", "relics", "big", path);
            }
        }
    }
}