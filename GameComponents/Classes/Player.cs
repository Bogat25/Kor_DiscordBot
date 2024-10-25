using System;

namespace Discord_Kor.GameComponents.Classes
{
    public class Player
    {
        // Properties
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Occupation { get; set; }
        public string FamilyStatus { get; set; }
        public string Background { get; set; }
        public string PoliticalView { get; set; }
        public int Appearance { get; set; }
        public bool IsAlive { get; set; } = true;
        public bool AlreadyVote { get; set; } = false;
        public int ReceivedVotes { get; set; } = 0;
        public bool? isCooperating;

        // Constructors
        public Player() { }

        public Player(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public Player(string id, string name, int age, string occupation, string familyStatus, string background, string politicalView, int appearance)
        {
            Id = id;
            Name = name;
            Age = age;
            Occupation = occupation;
            FamilyStatus = familyStatus;
            Background = background;
            PoliticalView = politicalView;
            Appearance = appearance;
            IsAlive = true;
            ReceivedVotes = 0;
        }

        // Methods
        public void ReceiveVote()
        {
            ReceivedVotes++;
        }

        public void Eliminate()
        {
            IsAlive = false;
        }

        public override string ToString()
        {
            string status = IsAlive ? "Életben van" : "Kiesett";
            return $@"
                **Játékos Neve:** {Name}
                **Kor:** {Age}
                **Foglalkozás:** {Occupation}
                **Családi állapot:** {FamilyStatus}
                **Előélet:** {Background}
                **Politikai nézet:** {PoliticalView}
                **Kinézet:** {Appearance}/10
                **Státusz:** {status}
                ";
        }


        // CreatePersonality method
        public void CreatePersonality()
        {
            // Random generator
            Random random = new Random();

            // Large arrays of possible values
            string[] occupations = { "Programmer", "Engineer", "Doctor", "Nurse", "Mechanic", "Baker", "Lawyer", "Teacher", "Architect", "Artist", "Waiter", "Mason", "Accountant", "Dentist", "Gardener", "Painter", "Chef", "Carpenter", "Actor", "Musician" };
            string[] familyStatuses = { "Married", "Divorced", "Widowed", "Single", "In a Relationship", "No Children" };
            string[] backgrounds = { "Unknown", "Veteran", "Ex-criminal", "Businessman", "Scientist", "Artist", "War Hero", "Activist", "Terrorist", "Drug Dealer", "Teacher" };
            string[] politicalViews = { "Anarchist", "Nationalist", "Totalitarian", "Separatist", "Supremacist", "Revolutionary", "Autocrat", "Radical", "Far Right", "Far Left" };

            // Assign random values
            Age = random.Next(18, 80); // Random age between 18 and 80
            Occupation = occupations[random.Next(occupations.Length)];
            FamilyStatus = familyStatuses[random.Next(familyStatuses.Length)];
            Background = backgrounds[random.Next(backgrounds.Length)];
            PoliticalView = politicalViews[random.Next(politicalViews.Length)];
            Appearance = random.Next(1, 11); // Random appearance between 1 and 10
        }
    }
}
