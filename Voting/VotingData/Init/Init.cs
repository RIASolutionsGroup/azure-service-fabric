using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VotingData.Models;

namespace VotingData
{
    public static class Init
    {
        internal static async Task<KeyValuePair<int, Character>[]> Populate(IReliableDictionary<int, Character> characterDictionary, ITransaction tx)
        {
            var kvpArray = new[]
            {
                new KeyValuePair<int, Character>(1, SetCharacter("Black Widow", "BlackWidow.png", "Natalia Alianovna Romanova", "Human")),
                new KeyValuePair<int, Character>(2, SetCharacter("Captain America", "CaptainAmerica.jpg", "Steven \"Steve\" Grant Rogers", "Enhanced ")),
                new KeyValuePair<int, Character>(3, SetCharacter("Iron Man", "IronMan.png", "Anthony Edward \"Tony\" Stark", "Human")),
                new KeyValuePair<int, Character>(4, SetCharacter("Nick Fury", "NickFury.jpg", "Nicholas Joseph Fury", "Human")),
                new KeyValuePair<int, Character>(5, SetCharacter("Professor X", "ProfessorX.jpg", "Charles Francis Xavier", "Mutant")),
                new KeyValuePair<int, Character>(6, SetCharacter("Spider-Man", "Spider-Man.png", "Peter Benjamin Parker", "Human mutate")),
                new KeyValuePair<int, Character>(7, SetCharacter("Thanos", "Thanos.jpg", "Thanos", "Mutant Titanian")),
                new KeyValuePair<int, Character>(8, SetCharacter("Thor", "Thor.jpg", "Thor Odinson", "Asgardian")),
                new KeyValuePair<int, Character>(9, SetCharacter("Wolverine", "Wolverine.png", "James \"Logan\" Howlett", "Mutant"))
            };

            foreach (var kv in kvpArray)
            {
                await characterDictionary.AddAsync(tx, kv.Key, kv.Value);
            }
            await tx.CommitAsync();

            return kvpArray;
        }

        private static Character SetCharacter(string name, string path, string realName, string origin)
        {
            return new Character()
            {
                Path = path,
                Name = name,
                RealName = realName,
                Origin = origin,
                Votes = 0
            };
        }
    }
}
