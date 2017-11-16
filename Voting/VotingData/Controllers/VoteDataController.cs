using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using VotingData.Models;


namespace VotingData.Controllers
{
    [Route("api/[controller]")]
    public class VoteDataController : Controller
    {
        private readonly IReliableStateManager _stateManager;
        private const string CharactersName = "Characters";

        public VoteDataController(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        // GET api/VoteData
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var ct = new CancellationToken();

            var result = new List<KeyValuePair<int, Character>>();
            try
            {
                var characterDictionary = await _stateManager.GetOrAddAsync<IReliableDictionary<int, Character>>(CharactersName);

                using (ITransaction tx = _stateManager.CreateTransaction())
                {
                    var list = await characterDictionary.CreateEnumerableAsync(tx);
                    var enumerator = list.GetAsyncEnumerator();
                    

                    while (await enumerator.MoveNextAsync(ct))
                    {
                        result.Add(enumerator.Current);
                    }

                    // init 
                    if (!result.Any())
                    {
                        result.AddRange(await Init.Populate(characterDictionary, tx));
                    }

                    return Json(result);
                }
            }
            catch
            {
                return new BadRequestResult();
            }
        }

        // PUT api/VoteData/characterId
        [HttpPut("{characterId}")]
        public async Task<IActionResult> Put(int characterId)
        {
            try
            {
                IReliableDictionary<int, Character> characterDictionary =
                    await _stateManager.GetOrAddAsync<IReliableDictionary<int, Character>>(CharactersName);

                using (ITransaction tx = _stateManager.CreateTransaction())
                {
                    var characterConditionalValue = await characterDictionary.TryGetValueAsync(tx, characterId);

                    if (!characterConditionalValue.HasValue)
                        return new BadRequestResult();

                    var character = characterConditionalValue.Value;
                    
                    // this is invalid, it will not update the replicas
                    // character.Votes++;

                    var cloned = (Character)character.Clone();
                    cloned.Votes += 2;

                    await characterDictionary.TryUpdateAsync(tx, characterId, cloned, character);
                    
                    // alternative
                    //await characterDictionary.AddOrUpdateAsync(tx, characterId, character, (value, oldvalue) =>
                    //{
                    //    oldvalue.Votes++;
                    //    return oldvalue;
                    //});

                    await tx.CommitAsync();
                }

                return new OkResult();
            }
            catch
            {
                return new BadRequestResult();
            }
        }
        
        [HttpDelete("{characterId}")]
        public async Task<IActionResult> Delete(int characterId)
        {
            try
            {
                var characterDictionary = await _stateManager.GetOrAddAsync<IReliableDictionary<int, Character>>(CharactersName);
                var ct = new CancellationTokenSource();

                using (ITransaction tx = _stateManager.CreateTransaction())
                {
                    var count = await characterDictionary.GetCountAsync(tx);
                    if (count <= 5)
                        return new BadRequestResult();

                    var characterConditionalValue = await characterDictionary.TryRemoveAsync(tx, characterId);

                    if (!characterConditionalValue.HasValue)
                        return new BadRequestResult();

                    await tx.CommitAsync();
                    return new OkResult();
                }
            }
            catch
            {
                return new BadRequestResult();
            }
        }

        
    }
}
