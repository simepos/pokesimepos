using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using PokeAPI;

namespace PokedexGeneratorV2
{
    class Program
    {
        static readonly Size CardSize = new Size(250, 290);
        static readonly int RoundedRectangleRadius = 8;
        static readonly Rectangle CardBorderRectangle = new Rectangle(0, 0, CardSize.Width, CardSize.Height);
        static readonly Point NamePosition = new Point(5, 3);
        static readonly float textSize = 20.0f;
        static readonly Rectangle ImageBorderRectangle = new Rectangle(10, 10+NamePosition.Y+ Convert.ToInt32(textSize), CardSize.Width - 20, CardSize.Width - 20);
        static readonly Rectangle ImageRectangle = new Rectangle(ImageBorderRectangle.X + 2, ImageBorderRectangle.Y + 2, ImageBorderRectangle.Width - 4, ImageBorderRectangle.Height - 4);
        static readonly Point TypePosition = new Point(ImageRectangle.X + ImageRectangle.Width / 2, ImageRectangle.Y + ImageRectangle.Height);
        

        static readonly Dictionary<string, Color> PokemonTypeColors = new Dictionary<string, Color>() { 
            {"flying",      getColorFromRGBString("A890F0") },
            {"bug",         getColorFromRGBString("A8B820") },
            {"dark",        getColorFromRGBString("705848") },
            {"dragon",      getColorFromRGBString("7038F8") },
            {"electric",    getColorFromRGBString("F8D030") },
            {"fairy",       getColorFromRGBString("EE99AC") },
            {"fighting",    getColorFromRGBString("C03028") },
            {"fire",        getColorFromRGBString("F08030") },
            {"ghost",       getColorFromRGBString("705898") },
            {"grass",       getColorFromRGBString("78C850") },
            {"ground",      getColorFromRGBString("E0C068") },
            {"ice",         getColorFromRGBString("98D8D8") },
            {"water",       getColorFromRGBString("6890F0") },
            {"steel",       getColorFromRGBString("B8B8D0") },
            {"rock",        getColorFromRGBString("B8A038") },
            {"psychic",     getColorFromRGBString("F85888") },
            {"poison",      getColorFromRGBString("A040A0") },
            {"normal",      getColorFromRGBString("A8A878") }
        };


        static void Main(string[] args)
        {
            int StartAt = 3;
            int HowMany = 2;

            for(int i=StartAt;i<StartAt+HowMany;i++)
            {
                PokemonSpecies pkmnSpecies = DataFetcher.GetApiObject<PokemonSpecies>(i).Result;

                ProcessPokemon(pkmnSpecies, "./export/");
            }
        }

        static Color getColorFromRGBString(string RGBString)
        {
            int R = int.Parse(RGBString.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
            int G = int.Parse(RGBString.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
            int B = int.Parse(RGBString.Substring(4,2), System.Globalization.NumberStyles.HexNumber);

            return Color.FromArgb(R, G, B);
        }

        static void ProcessPokemon(PokemonSpecies pkmnSpecies, string savePath)
        {
            foreach(PokemonSpeciesVariety pkmnSpeciesVariety in pkmnSpecies.Varieties)
            {
                Bitmap pkmnImage = new Bitmap(CardSize.Width, CardSize.Height);
                Graphics canvas = Graphics.FromImage(pkmnImage);

                Pokemon pkmn = pkmnSpeciesVariety.Pokemon.GetObject().Result;

                DrawBackground(pkmnSpecies, pkmn, canvas);
                DrawBorders(pkmnSpecies, pkmn, canvas);
                DrawImage(pkmnSpecies, pkmn, canvas);
                DrawTypes(pkmnSpecies, pkmn, canvas);
                DrawText(pkmnSpecies, pkmn, canvas);
                //DrawStats();

                GenerateImageFile(pkmnSpecies, pkmn, pkmnImage, savePath);
            }
            

        }

        private static void GenerateImageFile(PokemonSpecies pkmnSpecies, Pokemon pkmn, Bitmap pkmnImage, string savePath)
        {
            if(!System.IO.Directory.Exists(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
            }
            string pkmnFileName = pkmnSpecies.ID.ToString("D3") + "-" + pkmn.Name + ".png";
            pkmnImage.Save(savePath + pkmnFileName, System.Drawing.Imaging.ImageFormat.Png);
        }

        private static void DrawText(PokemonSpecies pkmnSpecies, Pokemon pkmn, Graphics canvas)
        {
            Font f = new Font(FontFamily.GenericMonospace, 20.0f, FontStyle.Bold & FontStyle.Underline);
            string formatedPokemonName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pkmn.Name.Replace('-', ' ').Replace('_', ' '));
            canvas.DrawString(formatedPokemonName, f, Brushes.Black, NamePosition);
        }

        private static void DrawTypes(PokemonSpecies pkmnSpecies, Pokemon pkmn, Graphics canvas)
        {
            if(pkmn.Types.Length == 2)
            {
                Bitmap bmpType1 = (Bitmap)PokemonTypeIcons.ResourceManager.GetObject(pkmn.Types[1].Type.Name);
                Bitmap bmpType2 = (Bitmap)PokemonTypeIcons.ResourceManager.GetObject(pkmn.Types[0].Type.Name);

                canvas.DrawImage(bmpType1, new Point(TypePosition.X - 34, TypePosition.Y));
                canvas.DrawImage(bmpType2, new Point(TypePosition.X + 2, TypePosition.Y));
            }
            else
            {
                Bitmap bmpType1 = (Bitmap)PokemonTypeIcons.ResourceManager.GetObject(pkmn.Types[0].Type.Name);
                canvas.DrawImage(bmpType1, new Point(TypePosition.X-16,TypePosition.Y));
            }
        }

        private static void DrawImage(PokemonSpecies pkmnSpecies, Pokemon pkmn, Graphics canvas)
        {
            Image lImage;
            string fileName = @".\Assets\PokemonArtwork\" + pkmn.Name.Replace(pkmnSpecies.Name, pkmnSpecies.ID.ToString())+ ".png";
            if (System.IO.File.Exists(fileName))
            {
                lImage = Bitmap.FromFile(fileName);
            }
            else
            {
                fileName = @".\Assets\PokemonSprites\" + pkmn.Name.Replace(pkmnSpecies.Name, pkmnSpecies.ID.ToString()) + ".png";
                if (System.IO.File.Exists(fileName))
                {
                    lImage = Bitmap.FromFile(fileName);
                }
                else
                {
                    fileName = "";
                    if (System.IO.File.Exists(fileName))
                    {
                        lImage = Bitmap.FromFile(fileName);
                    }
                    else
                    {
                        lImage = new Bitmap(ImageRectangle.Width, ImageRectangle.Height);
                    }
                }
            }

            canvas.DrawImage(lImage, ImageRectangle);
        }

        private static void DrawBorders(PokemonSpecies pkmnSpecies, Pokemon pkmn, Graphics canvas)
        {
            Pen p = new Pen(Brushes.LightGoldenrodYellow);
            p.Width = 2;
            p.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;

            canvas.DrawRoundedRectangle(p, CardBorderRectangle, RoundedRectangleRadius);

            canvas.DrawRoundedRectangle(p, ImageBorderRectangle, RoundedRectangleRadius);
        }

        private static void DrawBackground(PokemonSpecies pkmnSpecies, Pokemon pkmn, Graphics canvas)
        {
            Brush theBrush;
            if(pkmn.Types.Length == 2)
            {
                System.Drawing.Drawing2D.LinearGradientBrush lgb = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(new Point(0, 0), CardSize), PokemonTypeColors[pkmn.Types[1].Type.Name], PokemonTypeColors[pkmn.Types[0].Type.Name], 0.0f);
                theBrush = lgb;
            }
            else
            {
                theBrush = new SolidBrush(PokemonTypeColors[pkmn.Types[0].Type.Name]);
            }
            canvas.FillRectangle(theBrush, new Rectangle(new Point(0, 0), CardSize));
        }
    }
}
