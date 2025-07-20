using System;
using System.IO;

namespace LamiaSimulation
{
    public class NameGenerator
    {
        private static NameGenerator _Instance;

        public static NameGenerator Instance
        {
            get
            {
                if(_Instance is null)
                    _Instance = new NameGenerator();
                return _Instance;
            }
        }

        private string[] firstNames;
        private string[] surnames;

        public NameGenerator()
        {
            firstNames = File.ReadAllLines(Path.Combine(/*AppDomain.CurrentDomain.BaseDirectory, */Consts.FilenameDataDirectory, Consts.FilenameDataFirstNames));
            surnames = File.ReadAllLines(Path.Combine(/*AppDomain.CurrentDomain.BaseDirectory, */Consts.FilenameDataDirectory, Consts.FilenameDataSurnames));
        }

        public string GenerateFirstName()
        {
            var random = new Random();
            return firstNames[random.Next(firstNames.Length)];
        }

        public string GenerateSurname()
        {
            var random = new Random();
            return surnames[random.Next(surnames.Length)];
        }

        public string GenerateFullName()
        {
            return GenerateFirstName() + " " + GenerateSurname();
        }
    }
}